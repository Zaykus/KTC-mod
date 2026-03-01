@echo off
dotnet build "f:\vs CODE\KTC dev\KTC MOD\KingdomEnhanced\KingdomEnhanced.csproj" -c Release
copy /Y "f:\vs CODE\KTC dev\KTC MOD\KingdomEnhanced\bin\Release\net6.0-windows\KingdomEnhanced.dll" "F:\vs CODE\mods\BepInEx\plugins\KingdomEnhanced.dll"
