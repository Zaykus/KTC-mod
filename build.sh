#!/bin/bash
# Kingdom Enhanced Mod - Build Script for Linux / Steam Deck
set -e

CONFIG="${1:-Release}"
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
CSPROJ="$SCRIPT_DIR/KingdomEnhanced/KingdomEnhanced.csproj"

echo -e "\033[36mBuilding Kingdom Enhanced ($CONFIG)...\033[0m"
dotnet build "$CSPROJ" -c "$CONFIG"

if [ $? -eq 0 ]; then
    echo -e "\033[32m\nBuild succeeded!\033[0m"
else
    echo -e "\033[31m\nBuild failed!\033[0m"
    exit 1
fi
