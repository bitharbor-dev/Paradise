param(
    [Parameter(Mandatory)]
    [string]$Source,
    [Parameter(Mandatory)]
    [string]$Destination
)

Write-Host "Copying $Source to $Destination"

Copy-Item -Path $Source -Destination $Destination