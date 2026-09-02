[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [string]$GameRoot = ""
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$modVersion = "__MOD_VERSION__"
$expectedFileVersion = "__FILE_VERSION__"
$expectedHash = "__DLL_SHA256__"
$packageDll = Join-Path $PSScriptRoot "Assembly-CSharp.dll"

if ([string]::IsNullOrWhiteSpace($GameRoot)) {
    $GameRoot = Read-Host "Enter the Shadow Dungeon game directory"
}
$GameRoot = [System.IO.Path]::GetFullPath($GameRoot.Trim().Trim([char]34))
$managedDirectory = Join-Path $GameRoot "Shadow Dungeon_Data\Managed"
$targetDll = Join-Path $managedDirectory "Assembly-CSharp.dll"

if (-not (Test-Path -LiteralPath $packageDll -PathType Leaf)) {
    throw "Assembly-CSharp.dll is missing from the update package."
}
if (-not (Test-Path -LiteralPath $managedDirectory -PathType Container)) {
    throw "Managed directory was not found: $managedDirectory"
}
if ($null -ne (Get-Process -Name "Shadow Dungeon" -ErrorAction SilentlyContinue)) {
    throw "Shadow Dungeon is running. Close the game before installing."
}

$sourceHash = (Get-FileHash -LiteralPath $packageDll -Algorithm SHA256).Hash.ToUpperInvariant()
if ($sourceHash -ne $expectedHash) {
    throw "Package DLL hash verification failed."
}
$sourceVersion = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($packageDll).FileVersion
if ($sourceVersion -ne $expectedFileVersion) {
    throw "Package DLL file version verification failed. Expected $expectedFileVersion, got $sourceVersion."
}

if (Test-Path -LiteralPath $targetDll -PathType Leaf) {
    $currentHash = (Get-FileHash -LiteralPath $targetDll -Algorithm SHA256).Hash.ToUpperInvariant()
    if ($currentHash -eq $expectedHash) {
        Write-Host "Shadow Dungeon MOD V$modVersion is already installed and verified."
        exit 0
    }
}

$backupDll = $null
if (Test-Path -LiteralPath $targetDll -PathType Leaf) {
    $backupRoot = Join-Path $GameRoot "MOD-Backups"
    $backupDirectory = Join-Path $backupRoot (Get-Date -Format "yyyyMMdd-HHmmss")
    New-Item -ItemType Directory -Path $backupDirectory -Force | Out-Null
    $backupDll = Join-Path $backupDirectory "Assembly-CSharp.dll"
    Copy-Item -LiteralPath $targetDll -Destination $backupDll
}

try {
    Copy-Item -LiteralPath $packageDll -Destination $targetDll -Force
    $installedHash = (Get-FileHash -LiteralPath $targetDll -Algorithm SHA256).Hash.ToUpperInvariant()
    $installedVersion = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($targetDll).FileVersion
    if ($installedHash -ne $expectedHash) {
        throw "Installed DLL hash verification failed."
    }
    if ($installedVersion -ne $expectedFileVersion) {
        throw "Installed DLL file version verification failed. Expected $expectedFileVersion, got $installedVersion."
    }
}
catch {
    if ($null -ne $backupDll -and (Test-Path -LiteralPath $backupDll -PathType Leaf)) {
        Copy-Item -LiteralPath $backupDll -Destination $targetDll -Force
    }
    throw
}

Write-Host "Installed Shadow Dungeon MOD V$modVersion successfully."
Write-Host "DLL SHA256: $expectedHash"
if ($null -ne $backupDll) {
    Write-Host "Previous DLL backup: $backupDll"
}
