@echo off
cd /d "f:\vs CODE\KingdomEnhanced - Copy"
csc.exe DLLAnalyzer.cs -out:DLLAnalyzer.exe
if %ERRORLEVEL% EQU 0 (
    DLLAnalyzer.exe
) else (
    echo Compilation failed
)
