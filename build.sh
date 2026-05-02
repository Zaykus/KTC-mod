#!/bin/bash
# Kingdom Enhanced Mod - Build Script for Linux / Steam Deck
set -e

SKIP_MONO=false
while [[ $# -gt 0 ]]; do
    case "$1" in
        --skip-mono) SKIP_MONO=true; shift ;;
        *) echo "Usage: $0 [--skip-mono]"; exit 1 ;;
    esac
done

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
CSPROJ="$SCRIPT_DIR/KingdomEnhanced/KingdomEnhanced.csproj"
FAILED=false

echo -e "\033[36m========================================\033[0m"
echo -e "\033[36m  Kingdom Enhanced - Build All Configs\033[0m"
echo -e "\033[36m========================================\033[0m"

echo -e "\n\033[33m[1/2] Building BIE6_IL2CPP...\033[0m"
dotnet build "$CSPROJ" -c BIE6_IL2CPP
if [ $? -ne 0 ]; then
    echo -e "\033[31mBIE6_IL2CPP build FAILED!\033[0m"
    FAILED=true
else
    echo -e "\033[32mBIE6_IL2CPP build succeeded.\033[0m"
fi

if [ "$SKIP_MONO" = false ]; then
    echo -e "\n\033[33m[2/2] Building BIE6_Mono...\033[0m"
    dotnet build "$CSPROJ" -c BIE6_Mono
    if [ $? -ne 0 ]; then
        echo -e "\033[31mBIE6_Mono build FAILED!\033[0m"
        FAILED=true
    else
        echo -e "\033[32mBIE6_Mono build succeeded.\033[0m"
    fi
fi

if [ "$FAILED" = true ]; then
    echo -e "\n\033[31mBuild completed with errors.\033[0m"
    exit 1
else
    echo -e "\n\033[32mAll builds succeeded!\033[0m"
fi
