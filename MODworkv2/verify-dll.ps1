[CmdletBinding()]
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$Path,
    [string]$ExpectedFileVersion = "",
    [string]$ExpectedSha256 = ""
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$resolved = (Resolve-Path -LiteralPath $Path).Path
$item = Get-Item -LiteralPath $resolved
$assemblyVersion = [System.Reflection.AssemblyName]::GetAssemblyName($resolved).Version.ToString()
$versionInfo = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($resolved)
$hash = (Get-FileHash -LiteralPath $resolved -Algorithm SHA256).Hash.ToUpperInvariant()

[pscustomobject]@{
    Path = $resolved
    Size = $item.Length
    AssemblyIdentityVersion = $assemblyVersion
    FileVersion = $versionInfo.FileVersion
    ProductVersion = $versionInfo.ProductVersion
    SHA256 = $hash
    LastWriteTimeUtc = $item.LastWriteTimeUtc.ToString("o")
} | Format-List

if (-not [string]::IsNullOrWhiteSpace($ExpectedFileVersion) -and $versionInfo.FileVersion -ne $ExpectedFileVersion) {
    throw "File version mismatch. Expected $ExpectedFileVersion, got $($versionInfo.FileVersion)."
}
if (-not [string]::IsNullOrWhiteSpace($ExpectedSha256) -and $hash -ne $ExpectedSha256.ToUpperInvariant()) {
    throw "SHA256 mismatch. Expected $ExpectedSha256, got $hash."
}

Write-Host "DLL verification passed."
