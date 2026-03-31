function UpdatePropsFileVersionInfo {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$false)]
        [string]$BuildSourcesDirectory = $env:Build_SourcesDirectory
    )

    # Define the GitVersion environment variables and their corresponding XML properties
    $versionMap = @{
        "GITVERSION_ASSEMBLYSEMVER"         = "AssemblyVersion";
        "GITVERSION_ASSEMBLYSEMFILEVER"     = "AssemblyFileVersion";
        "GITVERSION_INFORMATIONALVERSION"   = "AssemblyInformationalVersion";
        "GITVERSION_SEMVER"                 = "ProductVersion"
    }

    $missingVariables = New-Object System.Collections.ArrayList
    $versionInfo = @{}

    # --- Retrieve and Validate GitVersion Environment Variables ---
    Write-Host "Retrieving GitVersion environment variables..."
    foreach ($envVar in $versionMap.Keys) {
        $value = Get-Item -Path "Env:$envVar" -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Value
        if ([string]::IsNullOrWhiteSpace($value)) {
            $missingVariables.Add($envVar) | Out-Null
        } else {
            $versionInfo[$envVar] = $value
        }
    }

    if ($missingVariables.Count -gt 0) {
        Write-Error "Error: The following GitVersion environment variables are missing or empty: $($missingVariables -join ', ')."
        exit 1
    }

    # --- Construct and Validate File Path ---
    $propsFilePath = Join-Path -Path $BuildSourcesDirectory -ChildPath "Directory.Build.props"

    Write-Host "Checking for file: '$propsFilePath'..."
    if (-not (Test-Path $propsFilePath -PathType Leaf)) {
        Write-Error "Error: File 'Directory.Build.props' not found at '$propsFilePath'."
        exit 1
    }

    # --- Load and Update XML ---
    Write-Host "Loading XML file and updating version information..."
    try {
        [xml]$xml = Get-Content $propsFilePath -ErrorAction Stop
    } catch {
        Write-Error "Error: Could not load XML from '$propsFilePath'. Please ensure it's a valid XML file."
        exit 1
    }

    $updated = $false
    foreach ($propertyGroup in $xml.Project.PropertyGroup) {
        foreach ($envVar in $versionMap.Keys) {
            $xmlProperty = $versionMap[$envVar]
            if ($propertyGroup.$xmlProperty) {
                if ($propertyGroup.$xmlProperty -ne $versionInfo[$envVar]) {
                    $propertyGroup.$xmlProperty = $versionInfo[$envVar]
                    Write-Host "  Updated <$xmlProperty> to '$($versionInfo[$envVar])'"
                    $updated = $true
                } else {
                    Write-Host "  <$xmlProperty> is already set to '$($versionInfo[$envVar])', no change needed."
                }
            }
        }
    }

    # --- Save XML ---
    if ($updated) {
        try {
            $xml.Save($propsFilePath)
            Write-Host "Successfully updated version info in '$propsFilePath'."
        } catch {
            Write-Error "Error: Could not save updated XML to '$propsFilePath'. $($_.Exception.Message)"
            exit 1
        }
    } else {
        Write-Host "No version information changes were required."
    }
}

UpdatePropsFileVersionInfo