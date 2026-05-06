$ErrorActionPreference = 'Stop'

# ── Paths ────────────────────────────────────────────────────────────────────
$workspace   = 'd:\vs CODE\KTC dev\KTC MOD 4.0'
$releasesDir = Join-Path $workspace 'Releases'
$workDir     = Join-Path $workspace 'temp_work'
$baseZipsDir = Join-Path $workspace 'BaseZips'
$speechDll   = 'C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Speech\v4.0_4.0.0.0__31bf3856ad364e35\System.Speech.dll'
$baseUrl     = 'https://builds.bepinex.dev/projects/bepinex_be/754/'
$version     = 'v2.1.0-beta3'

# ── Prep directories ─────────────────────────────────────────────────────────
if (Test-Path $releasesDir) { Remove-Item -Path $releasesDir -Recurse -Force }
New-Item -ItemType Directory -Path $releasesDir | Out-Null

if (Test-Path $workDir) { Remove-Item -Path $workDir -Recurse -Force }
New-Item -ItemType Directory -Path $workDir | Out-Null

if (-not (Test-Path $baseZipsDir)) {
    New-Item -ItemType Directory -Path $baseZipsDir | Out-Null
    Write-Host 'Created BaseZips folder. Drop BepInEx .zip files there to skip downloading.' -ForegroundColor Cyan
}

# ── Targets ──────────────────────────────────────────────────────────────────
# Each entry: file = default remote filename, match = glob to find a local override, type, os
$targets = @(
    [pscustomobject]@{ match = '*Mono-win-x86*.zip';    file = 'BepInEx-Unity.Mono-win-x86-6.0.0-be.754+ba5d275.zip';    type = 'Mono';   os = 'win' }
    [pscustomobject]@{ match = '*Mono-win-x64*.zip';    file = 'BepInEx-Unity.Mono-win-x64-6.0.0-be.754+ba5d275.zip';    type = 'Mono';   os = 'win' }
    [pscustomobject]@{ match = '*Mono-linux-x86*.zip';  file = 'BepInEx-Unity.Mono-linux-x86-6.0.0-be.754+ba5d275.zip';  type = 'Mono';   os = 'linux' }
    [pscustomobject]@{ match = '*Mono-linux-x64*.zip';  file = 'BepInEx-Unity.Mono-linux-x64-6.0.0-be.754+ba5d275.zip';  type = 'Mono';   os = 'linux' }
    [pscustomobject]@{ match = '*IL2CPP-win-x86*.zip';  file = 'BepInEx-Unity.IL2CPP-win-x86-6.0.0-be.754+ba5d275.zip';  type = 'IL2CPP'; os = 'win' }
    [pscustomobject]@{ match = '*IL2CPP-win-x64*.zip';  file = 'BepInEx-Unity.IL2CPP-win-x64-6.0.0-be.754+ba5d275.zip';  type = 'IL2CPP'; os = 'win' }
    [pscustomobject]@{ match = '*IL2CPP-linux-x64*.zip';file = 'BepInEx-Unity.IL2CPP-linux-x64-6.0.0-be.754+ba5d275.zip';type = 'IL2CPP'; os = 'linux' }
)

# ── Main loop ─────────────────────────────────────────────────────────────────
foreach ($target in $targets) {

    # Build output name from file stem e.g. BepInEx-Unity.IL2CPP-win-x64-... -> IL2CPP_Win_x64
    $stem     = [System.IO.Path]::GetFileNameWithoutExtension($target.file)
    $outName  = 'KTC_Mod_' + $version + '_' + $target.type + '_' + ($stem -replace '^.*?-win-',    'Win_'  `
                                                                           -replace '^.*?-linux-',  'Linux_' `
                                                                           -replace 'x86.*',        'x86'   `
                                                                           -replace 'x64.*',        'x64')

    # Simpler: just parse type and os from the target itself
    $suffix  = $target.type + '_'
    if ($target.os -eq 'win')   { $suffix += 'Win_' }  else { $suffix += 'Linux_' }
    if ($target.file -like '*x86*') { $suffix += 'x86' } else { $suffix += 'x64' }
    $outName = 'KTC_Mod_' + $version + '_' + $suffix

    Write-Host ('=== Processing ' + $outName + ' ===') -ForegroundColor Cyan

    # ── Get BepInEx zip (local first, then download) ──────────────────────────
    $localZip = Get-ChildItem -Path $baseZipsDir -Filter $target.match -ErrorAction SilentlyContinue |
                Select-Object -First 1

    if ($localZip) {
        Write-Host ('  Local zip: ' + $localZip.Name) -ForegroundColor Green
        $zipPath = Join-Path $workDir $localZip.Name
        Copy-Item -Path $localZip.FullName -Destination $zipPath
    } else {
        $encodedFile = $target.file -replace '\+', '%2B'
        $url = $baseUrl + $encodedFile
        $zipPath = Join-Path $workDir $target.file
        Write-Host ('  Downloading: ' + $url)
        Invoke-WebRequest -Uri $url -OutFile $zipPath
    }

    # ── Extract ───────────────────────────────────────────────────────────────
    $extractDir = Join-Path $workDir $outName
    Expand-Archive -Path $zipPath -DestinationPath $extractDir
    Start-Sleep -Seconds 4

    # ── Copy mod DLL ──────────────────────────────────────────────────────────
    $pluginsDir = Join-Path $extractDir 'BepInEx\plugins\KingdomEnhanced'
    New-Item -ItemType Directory -Path $pluginsDir | Out-Null

    if ($target.type -eq 'Mono') {
        $dllSrc = Join-Path $workspace 'KingdomEnhanced\bin\BIE6_Mono\KingdomEnhanced.dll'
    } else {
        $dllSrc = Join-Path $workspace 'KingdomEnhanced\bin\BIE6_IL2CPP\KingdomEnhanced.dll'
    }
    Copy-Item -Path $dllSrc -Destination $pluginsDir
    Start-Sleep -Seconds 3

    # ── Copy Speech DLL (Windows only) ────────────────────────────────────────
    if ($target.os -eq 'win') {
        Write-Host '  Adding System.Speech.dll...'
        if (Test-Path $speechDll) {
            Copy-Item -Path $speechDll -Destination $pluginsDir
        } else {
            Write-Warning ('System.Speech.dll not found at: ' + $speechDll)
        }
    }

    # ── Package zip ───────────────────────────────────────────────────────────
    $releaseZip = Join-Path $releasesDir ($outName + '.zip')
    Write-Host ('  Packaging: ' + $releaseZip)
    Start-Sleep -Seconds 6
    Compress-Archive -Path ($extractDir + '\*') -DestinationPath $releaseZip

    # ── Cleanup ───────────────────────────────────────────────────────────────
    Remove-Item -Path $extractDir -Recurse -Force
    Remove-Item -Path $zipPath -Force

    Write-Host ('  Done: ' + $outName) -ForegroundColor Green
}

# ── Final cleanup ─────────────────────────────────────────────────────────────
Remove-Item -Path $workDir -Recurse -Force
Write-Host ''
Write-Host ('All releases packaged in: ' + $releasesDir) -ForegroundColor Green
