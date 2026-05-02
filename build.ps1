# Kingdom Enhanced Mod - Build Script for Windows PowerShell
param(
    [string]$Configuration = "Release"
)

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$Csproj = Join-Path $ScriptDir "KingdomEnhanced\KingdomEnhanced.csproj"

Write-Host "Building Kingdom Enhanced ($Configuration)..." -ForegroundColor Cyan
dotnet build $Csproj -c $Configuration

if ($LASTEXITCODE -eq 0) {
    Write-Host "`nBuild succeeded!" -ForegroundColor Green
} else {
    Write-Host "`nBuild failed with exit code $LASTEXITCODE" -ForegroundColor Red
    exit $LASTEXITCODE
}
