param(
    [Parameter(Mandatory)]
    [string]$Source,
    [Parameter(Mandatory)]
    [string]$Destination,
    [string]$Exclusions = ""
)

$excludeList = @()
if (-not [string]::IsNullOrWhiteSpace($Exclusions)) {
    $excludeList = $Exclusions.Split(";", [System.StringSplitOptions]::RemoveEmptyEntries)
}

Write-Host "Copying items from $Source to $Destination"
Write-Host "Exclusions: $($excludeList -join ", ")"

New-Item -Path $Destination -ItemType Directory -Force | Out-Null

Get-ChildItem -Path $Source -Exclude $excludeList |
    Copy-Item -Destination $Destination -Recurse