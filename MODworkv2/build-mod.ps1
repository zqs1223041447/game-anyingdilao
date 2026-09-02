[CmdletBinding()]
param(
    [string]$Version = "",
    [ValidateSet("Release", "Debug")]
    [string]$Configuration = "Release"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Read-DefaultModVersion {
    param([string]$PropsPath)

    [xml]$props = Get-Content -LiteralPath $PropsPath -Raw
    foreach ($group in @($props.Project.PropertyGroup)) {
        if ($null -ne $group.ModVersion -and -not [string]::IsNullOrWhiteSpace([string]$group.ModVersion)) {
            return [string]$group.ModVersion
        }
    }
    throw "ModVersion was not found in $PropsPath"
}

function Assert-ReferencesPresent {
    param([string]$ProjectPath)

    [xml]$projectXml = Get-Content -LiteralPath $ProjectPath -Raw
    $projectDirectory = Split-Path -Parent $ProjectPath
    $missing = New-Object 'System.Collections.Generic.List[string]'
    foreach ($result in @(Select-Xml -Xml $projectXml -XPath "//Reference[HintPath]")) {
        $hintPath = [string]$result.Node.HintPath
        $candidate = [System.IO.Path]::GetFullPath((Join-Path $projectDirectory $hintPath))
        if (-not (Test-Path -LiteralPath $candidate -PathType Leaf)) {
            [void]$missing.Add($candidate)
        }
    }

    if ($missing.Count -gt 0) {
        $preview = ($missing | Select-Object -First 10) -join [Environment]::NewLine
        throw ("Missing {0} reference DLL(s). Restore MODworkv2\refs from the game Managed directory first.{1}{2}" -f $missing.Count, [Environment]::NewLine, $preview)
    }
}

function Write-Utf8Bom {
    param(
        [string]$Path,
        [string]$Content
    )

    $encoding = New-Object System.Text.UTF8Encoding($true)
    [System.IO.File]::WriteAllText($Path, $Content, $encoding)
}

$modRoot = $PSScriptRoot
$repoRoot = Split-Path -Parent $modRoot
$projectDirectory = Join-Path $modRoot "decompiled"
$projectPath = Join-Path $projectDirectory "Assembly-CSharp.csproj"
$propsPath = Join-Path $projectDirectory "Directory.Build.props"
$installerTemplatePath = Join-Path $modRoot "packaging\install.template.ps1"
$buildsRoot = Join-Path $modRoot "builds"

if ([string]::IsNullOrWhiteSpace($Version)) {
    $Version = Read-DefaultModVersion -PropsPath $propsPath
}
if ($Version -notmatch '^[0-9]+\.[0-9]+\.[0-9]+$') {
    throw "Version must use three numeric parts, for example 1.35.0."
}

$dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
if ($null -eq $dotnet) {
    throw ".NET SDK was not found. Install .NET SDK 8.x and reopen PowerShell."
}

Assert-ReferencesPresent -ProjectPath $projectPath

$date = Get-Date -Format "yyyy-MM-dd"
$packageName = "ShadowDungeon-MOD-V{0}_{1}" -f $Version, $date
$packageDirectory = Join-Path $buildsRoot $packageName
$zipPath = Join-Path $buildsRoot ($packageName + ".zip")
if ((Test-Path -LiteralPath $packageDirectory) -or (Test-Path -LiteralPath $zipPath)) {
    throw "Output already exists for V$Version. Increment ModVersion or move the existing package before rebuilding."
}

$artifactRoot = Join-Path $modRoot ".artifacts"
$buildDirectory = Join-Path $artifactRoot ([Guid]::NewGuid().ToString("N"))
New-Item -ItemType Directory -Path $buildDirectory -Force | Out-Null
New-Item -ItemType Directory -Path $buildsRoot -Force | Out-Null

try {
    Write-Host "Cleaning project..."
    & $dotnet.Path clean $projectPath -c $Configuration -v minimal
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet clean failed with exit code $LASTEXITCODE."
    }

    $buildStartedUtc = [DateTime]::UtcNow
    Write-Host "Building Assembly-CSharp V$Version..."
    & $dotnet.Path build $projectPath -c $Configuration --no-incremental --output $buildDirectory "-p:ModVersion=$Version" -v minimal
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet build failed with exit code $LASTEXITCODE."
    }

    $builtDll = Join-Path $buildDirectory "Assembly-CSharp.dll"
    if (-not (Test-Path -LiteralPath $builtDll -PathType Leaf)) {
        throw "Build reported success but Assembly-CSharp.dll was not produced."
    }
    $builtItem = Get-Item -LiteralPath $builtDll
    if ($builtItem.LastWriteTimeUtc -lt $buildStartedUtc.AddSeconds(-2)) {
        throw "The produced DLL predates this build and may be stale."
    }

    $assemblyVersion = [System.Reflection.AssemblyName]::GetAssemblyName($builtDll).Version.ToString()
    $versionInfo = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($builtDll)
    $expectedFileVersion = "$Version.0"
    if ($assemblyVersion -ne "0.0.0.0") {
        throw "Unexpected Unity assembly identity version: $assemblyVersion"
    }
    if ($versionInfo.FileVersion -ne $expectedFileVersion) {
        throw "File version verification failed. Expected $expectedFileVersion, got $($versionInfo.FileVersion)."
    }
    if ($versionInfo.ProductVersion -ne $Version) {
        throw "Product version verification failed. Expected $Version, got $($versionInfo.ProductVersion)."
    }

    $dllHash = (Get-FileHash -LiteralPath $builtDll -Algorithm SHA256).Hash.ToUpperInvariant()
    New-Item -ItemType Directory -Path $packageDirectory | Out-Null
    $packagedDll = Join-Path $packageDirectory "Assembly-CSharp.dll"
    Copy-Item -LiteralPath $builtDll -Destination $packagedDll
    $packagedHash = (Get-FileHash -LiteralPath $packagedDll -Algorithm SHA256).Hash.ToUpperInvariant()
    if ($packagedHash -ne $dllHash) {
        throw "Packaged DLL hash mismatch."
    }

    $installer = Get-Content -LiteralPath $installerTemplatePath -Raw
    $installer = $installer.Replace("__MOD_VERSION__", $Version)
    $installer = $installer.Replace("__FILE_VERSION__", $expectedFileVersion)
    $installer = $installer.Replace("__DLL_SHA256__", $dllHash)
    Write-Utf8Bom -Path (Join-Path $packageDirectory "install.ps1") -Content $installer

    $gitCommit = "unavailable"
    $git = Get-Command git -ErrorAction SilentlyContinue
    if ($null -ne $git) {
        $previousErrorActionPreference = $ErrorActionPreference
        $ErrorActionPreference = "Continue"
        $candidateCommit = (& $git.Path -C $repoRoot rev-parse HEAD 2>$null)
        $ErrorActionPreference = $previousErrorActionPreference
        if ($LASTEXITCODE -eq 0 -and -not [string]::IsNullOrWhiteSpace([string]$candidateCommit)) {
            $gitCommit = ([string]$candidateCommit).Trim()
        }
    }

    $buildInfo = @"
Mod-Version: $Version
Assembly-Identity-Version: $assemblyVersion
File-Version: $($versionInfo.FileVersion)
Product-Version: $($versionInfo.ProductVersion)
DLL-SHA256: $dllHash
DLL-Size: $($builtItem.Length)
Git-Commit: $gitCommit
Build-UTC: $([DateTime]::UtcNow.ToString("o"))
"@
    Write-Utf8Bom -Path (Join-Path $packageDirectory "BUILD-INFO.txt") -Content $buildInfo

    $hashFile = "$dllHash *Assembly-CSharp.dll`r`n"
    Write-Utf8Bom -Path (Join-Path $packageDirectory "SHA256.txt") -Content $hashFile

    $readme = @"
# Shadow Dungeon MOD V$Version

This package was built from the current source tree by MODworkv2/build-mod.ps1.

Install:
1. Close the game.
2. Open PowerShell in this extracted package.
3. Run: powershell -ExecutionPolicy Bypass -File .\install.ps1 -GameRoot "D:\Path\To\Shadow Dungeon"

The installer verifies the packaged DLL hash and file version, backs up the current DLL,
installs only this package's DLL, and verifies the installed copy again.
"@
    Write-Utf8Bom -Path (Join-Path $packageDirectory "README.md") -Content $readme

    Compress-Archive -Path (Join-Path $packageDirectory "*") -DestinationPath $zipPath -CompressionLevel Optimal
    $zipHash = (Get-FileHash -LiteralPath $zipPath -Algorithm SHA256).Hash.ToUpperInvariant()

    Write-Host "Build and package verification passed."
    Write-Host "DLL: $packagedDll"
    Write-Host "DLL SHA256: $dllHash"
    Write-Host "ZIP: $zipPath"
    Write-Host "ZIP SHA256: $zipHash"
}
finally {
    if (Test-Path -LiteralPath $buildDirectory) {
        Remove-Item -LiteralPath $buildDirectory -Recurse -Force
    }
}
