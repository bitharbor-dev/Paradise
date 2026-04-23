param(
    [Parameter(Mandatory)]
    [string]$Location,
    [Parameter(Mandatory)]
    [string]$ShortName,
    [Parameter(Mandatory)]
    [string]$TemplateToken,
    [Parameter(Mandatory)]
    [string]$WorkingDirectory
)

Write-Host "Installing template from $Location"

$solutionName = "Solution"

dotnet new install $Location

if (Test-Path $WorkingDirectory) {
    Remove-Item $WorkingDirectory -Recurse -Force
}

New-Item -ItemType Directory -Path $WorkingDirectory | Out-Null

Write-Host "Generating solution using template '$ShortName'"

dotnet new $ShortName -n $solutionName -o $WorkingDirectory

$csproj = Get-ChildItem -Path $WorkingDirectory -Filter *.csproj -Recurse | Select-Object -First 1
if (-not $csproj) {
    throw "Validation failed - no project files generated."
}

$remaining = Select-String -Path "$WorkingDirectory\**\*" -Pattern $TemplateToken -SimpleMatch -ErrorAction SilentlyContinue
if ($remaining) {
    throw "Validation failed - unresolved tokens found."
}

Write-Host "Template validation succeeded"