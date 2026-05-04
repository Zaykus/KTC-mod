$ErrorActionPreference = "Stop"

$workspace = "d:\vs CODE\KTC dev\KTC MOD 4.0"
$releasesDir = Join-Path $workspace "Releases"
$workDir = Join-Path $workspace "temp_work"

if (Test-Path $releasesDir) { Remove-Item -Path $releasesDir -Recurse -Force }
New-Item -ItemType Directory -Path $releasesDir | Out-Null

if (Test-Path $workDir) { Remove-Item -Path $workDir -Recurse -Force }
New-Item -ItemType Directory -Path $workDir | Out-Null

$version = "v2.1.0-beta2"

$targets = @(
    @{ file="BepInEx-Unity.Mono-win-x86-6.0.0-be.755+3fab71a.zip"; name="KTC_Mod_$version`_Mono_Win_x86"; type="Mono"; os="win" },
    @{ file="BepInEx-Unity.Mono-win-x64-6.0.0-be.755+3fab71a.zip"; name="KTC_Mod_$version`_Mono_Win_x64"; type="Mono"; os="win" },
    @{ file="BepInEx-Unity.Mono-linux-x86-6.0.0-be.755+3fab71a.zip"; name="KTC_Mod_$version`_Mono_Linux_x86"; type="Mono"; os="linux" },
    @{ file="BepInEx-Unity.Mono-linux-x64-6.0.0-be.755+3fab71a.zip"; name="KTC_Mod_$version`_Mono_Linux_x64"; type="Mono"; os="linux" },
    @{ file="BepInEx-Unity.IL2CPP-win-x86-6.0.0-be.755+3fab71a.zip"; name="KTC_Mod_$version`_IL2CPP_Win_x86"; type="IL2CPP"; os="win" },
    @{ file="BepInEx-Unity.IL2CPP-win-x64-6.0.0-be.755+3fab71a.zip"; name="KTC_Mod_$version`_IL2CPP_Win_x64"; type="IL2CPP"; os="win" },
    @{ file="BepInEx-Unity.IL2CPP-linux-x64-6.0.0-be.755+3fab71a.zip"; name="KTC_Mod_$version`_IL2CPP_Linux_x64"; type="IL2CPP"; os="linux" }
)

$baseUrl = "https://builds.bepinex.dev/projects/bepinex_be/755/"
$speechDllPath = "C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Speech\v4.0_4.0.0.0__31bf3856ad364e35\System.Speech.dll"

foreach ($target in $targets) {
    Write-Host "Processing $($target.name)..."
    $encodedFile = $target.file -replace '\+', '%2B'
    $url = $baseUrl + $encodedFile
    $zipPath = Join-Path $workDir $target.file
    
    # Download
    Write-Host "  Downloading $url..."
    Invoke-WebRequest -Uri $url -OutFile $zipPath

    # Extract
    $extractDir = Join-Path $workDir $target.name
    Expand-Archive -Path $zipPath -DestinationPath $extractDir

    # Create plugins folder
    $pluginsDir = Join-Path $extractDir "BepInEx\plugins\KingdomEnhanced"
    New-Item -ItemType Directory -Path $pluginsDir | Out-Null

    # Copy DLL
    if ($target.type -eq "Mono") {
        $dllPath = Join-Path $workspace "KingdomEnhanced\bin\BIE6_Mono\KingdomEnhanced.dll"
    } else {
        $dllPath = Join-Path $workspace "KingdomEnhanced\bin\BIE6_IL2CPP\KingdomEnhanced.dll"
    }
    Copy-Item -Path $dllPath -Destination $pluginsDir

    # Handle speech DLL
    if ($target.os -eq "win") {
        Write-Host "  Adding speech dll for win..."
        if (Test-Path $speechDllPath) {
            Copy-Item -Path $speechDllPath -Destination $pluginsDir
        } else {
            Write-Warning "System.Speech.dll not found at $speechDllPath!"
        }
    }

    # Zip release
    $releaseZip = Join-Path $releasesDir "$($target.name).zip"
    Write-Host "  Creating release package $releaseZip..."
    Compress-Archive -Path "$extractDir\*" -DestinationPath $releaseZip

    # Clean up
    Remove-Item -Path $extractDir -Recurse -Force
    Remove-Item -Path $zipPath -Force
}

Remove-Item -Path $workDir -Recurse -Force
Write-Host "All releases packaged successfully in $releasesDir!"
