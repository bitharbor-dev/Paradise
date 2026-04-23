param(
    [Parameter(Mandatory)]
    [string]$WorkingDirectory,
    [string]$TextToReplace = "",
    [string]$ReplaceWith = ""
)

$patterns = @()
if (-not [string]::IsNullOrWhiteSpace($TextToReplace)) {
    $patterns = $TextToReplace.Split(";;", [System.StringSplitOptions]::RemoveEmptyEntries) |
        Sort-Object Length -Descending
}

Write-Host "Renaming items in $WorkingDirectory"
Write-Host "Patterns: $($patterns -join ", ")"
Write-Host "Replacement: $ReplaceWith"

Get-ChildItem $WorkingDirectory -Recurse |
    Sort-Object { $_.FullName.Length } -Descending |
        ForEach-Object {
            $originalName = $_.Name
            $newName = $originalName

            foreach ($pattern in $patterns) {
                $escaped = [regex]::Escape($pattern)
                $newName = $newName -replace $escaped, $ReplaceWith
            }

            if ($newName -ne $originalName) {
                Write-Host "Renaming '$originalName' -> '$newName'"
                Rename-Item -LiteralPath $_.FullName -NewName $newName
            }
        }

Write-Host "Renaming complete"