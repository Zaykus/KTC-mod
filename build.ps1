# Kingdom Enhanced Mod - Build Script for Windows PowerShell
param(
    [switch]$SkipMono
)

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$Csproj = Join-Path $ScriptDir "KingdomEnhanced\KingdomEnhanced.csproj"

$failed = $false

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Kingdom Enhanced - Build All Configs" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

Write-Host "`n[1/2] Building BIE6_IL2CPP..." -ForegroundColor Yellow
dotnet build $Csproj -c BIE6_IL2CPP
if ($LASTEXITCODE -ne 0) {
    Write-Host "BIE6_IL2CPP build FAILED!" -ForegroundColor Red
    $failed = $true
} else {
    Write-Host "BIE6_IL2CPP build succeeded." -ForegroundColor Green
}

if (-not $SkipMono) {
    Write-Host "`n[2/2] Building BIE6_Mono..." -ForegroundColor Yellow
    dotnet build $Csproj -c BIE6_Mono
    if ($LASTEXITCODE -ne 0) {
        Write-Host "BIE6_Mono build FAILED!" -ForegroundColor Red
        $failed = $true
    } else {
        Write-Host "BIE6_Mono build succeeded." -ForegroundColor Green
    }
}

if ($failed) {
    Write-Host "`nBuild completed with errors." -ForegroundColor Red
    exit 1
} else {
    Write-Host "`nAll builds succeeded!" -ForegroundColor Green
}
