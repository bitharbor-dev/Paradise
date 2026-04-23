param(
    [Parameter(Mandatory)]
    [string]$WorkingDirectory,
    [string]$TextToReplace = "",
    [string]$ReplaceWith = "",
    [string]$SkipFiles = ""
)

$patterns = @()
if (-not [string]::IsNullOrWhiteSpace($TextToReplace)) {
    $patterns = $TextToReplace.Split(";;", [System.StringSplitOptions]::RemoveEmptyEntries)
}

$exclusions = @()
if (-not [string]::IsNullOrWhiteSpace($SkipFiles)) {
    $exclusions = $SkipFiles.Split(";", [System.StringSplitOptions]::RemoveEmptyEntries)
}

$escapedPatterns = $patterns |
    Sort-Object Length -Descending |
        ForEach-Object { [regex]::Escape($_) }

$pattern = ($escapedPatterns -join "|")

Write-Host "Replacing text in $WorkingDirectory"
Write-Host "Patterns: $($patterns -join ", ")"
Write-Host "Skipping files: $($exclusions -join ", ")"

Get-ChildItem $WorkingDirectory -Recurse -File |
    Where-Object { $exclusions -notcontains $_.Name } |
        ForEach-Object {
            $path = $_.FullName
            $content = Get-Content $path -Raw

            if ($content -match $pattern) {
                Write-Host "Processing '$path'"

                $updated = $content -replace $pattern, $ReplaceWith

                if ($updated -ne $content) {
                    Set-Content -Path $path -Value $updated -Encoding UTF8
                }
            }
        }

Write-Host "Replacement complete"