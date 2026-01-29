# Kingdom Two Crowns - Class Metadata Extraction Guide

## Files Generated

1. **FinalComprehensiveStubs.cs** - Complete C# class stubs with all members
2. **class_analysis_detailed.txt** - Detailed analysis with statistics
3. **detailed_class_analysis.txt** - Alternative detailed view

## Class Summary

### Director (Weather, Seasons, Blood Moons, Time)
- **Fields:** 364 (including season, weather, time management)
- **Properties:** 726 (day/night cycles, timestamps, season transitions)
- **Methods:** 263
- **Key Members:**
  - `DayNightID`, `ChariotDayID`, `ChariotNightID`
  - `SeasonAutumn`, `SeasonSpring`, `SeasonSummer`, `SeasonWinter`
  - `CurrentSeason`, `CurrentSeasonDay`, `CurrentSeasonStartDay`
  - `TimeScales`, `Timeline`, `Timestamp`, `UtcTimeStamp`
  - `dayCounter`, `dayEnd`, `dayStart`, `nightEnd`, `nightStart`
  - `bloodMoonMaterial`

### Banker (Treasury/Bank)
- **Fields:** 25 (bank, stash, gold management)
- **Properties:** 41
- **Methods:** 11
- **Key Members:**
  - `stash`, `stashHeight`, `stashedCoins`
  - `BankerID`, `Banker`, `bankerPrefab`
  - `GoldenAppleGrown`, `GoldenApplePaid`
  - `SetStash()`, `SendStashCount()`, `CanDropGoldNugget()`
  - `goldTierConditions`, `goldRankRequirement`

### Castle (Stash and Upgrades)
- **Fields:** 39 (castle structure, upgrades)
- **Properties:** 60
- **Methods:** 45
- **Key Members:**
  - Castle upgrade levels
  - Building mechanics
  - Stash integration

### Kingdom (Players and Citizens)
- **Fields:** 200 (kingdom management, population)
- **Properties:** 361
- **Methods:** 360
- **Key Members:**
  - Player lists and citizen tracking
  - Population management
  - Kingdom state

### EnemyManager (Greed Tracking)
- **Fields:** 191 (enemy spawning, wave control)
- **Properties:** 458
- **Methods:** 339
- **Key Members:**
  - Enemy wave management
  - Greed spawn logic
  - Attack coordination

### EnemyWaveSpawner (Wave Control)
- **Fields:** 167 (wave spawning mechanics)
- **Properties:** 421
- **Methods:** 321
- **Key Members:**
  - Wave timing and spawning
  - Enemy type selection
  - Difficulty scaling

### Steed (Stamina, Speed, Hunger)
- **Fields:** 152 (mount mechanics)
- **Properties:** 303
- **Methods:** 116
- **Key Members:**
  - `SteedStaminaID`, `Stamina`
  - `APSpeed`, `SpeedModifer`, `ActualSpeed`, `CurrentSpeed`
  - `hungerInterval`, `eatFullStaminaDelay`
  - `chargeSpeed`, `chaseSpeed`, `moveSpeed`
  - `DebugInfiniteStamina`
  - All mount types (Bear, Wolf, Griffin, Unicorn, etc.)

### CitizenHouse (Spawn Timers, Population)
- **Fields:** 162 (house mechanics, citizen spawning)
- **Properties:** 409
- **Methods:** 308
- **Key Members:**
  - Spawn timer logic
  - Population capacity
  - Citizen generation

### Building (Building Mechanics)
- **Fields:** 25 (construction, upgrades)
- **Properties:** 49
- **Methods:** 71
- **Key Members:**
  - Building construction
  - Upgrade system
  - Resource requirements

### Wallet (Coin Limits)
- **Fields:** 83 (coin management)
- **Properties:** 121
- **Methods:** 32
- **Key Members:**
  - Coin capacity
  - Wallet upgrades
  - Coin collection

### CurrencyBag (Bag Physics, Coin Management)
- **Fields:** 92 (bag mechanics)
- **Properties:** 169
- **Methods:** 135
- **Key Members:**
  - `COIN_BAG_MERGE_HEIGHT`
  - `COIN_BAG_SPLIT_HORI_HEIGHT_BOTTOM`
  - `COIN_BAG_SPLIT_HORI_HEIGHT_TOP`
  - Coin drop mechanics
  - Bag physics

### Damageable (Wall and Tower HP)
- **Fields:** 61 (health, damage)
- **Properties:** 125
- **Methods:** 115
- **Key Members:**
  - Health points
  - Damage calculation
  - Defense mechanics

## Usage with Harmony/AccessTools

### Basic AccessTools Examples

```csharp
// Access a private field
var stashField = AccessTools.Field(typeof(Banker), "stashedCoins");
int stashValue = (int)stashField.GetValue(bankerInstance);

// Access a property
var staminaProperty = AccessTools.Property(typeof(Steed), "Stamina");
float stamina = (float)staminaProperty.GetValue(steedInstance);

// Call a method
var setStashMethod = AccessTools.Method(typeof(Banker), "SetStash");
setStashMethod.Invoke(bankerInstance, new object[] { 1000 });

// Access a private field with underscore
var directorField = AccessTools.Field(typeof(Kingdom), "_director");
var director = directorField.GetValue(kingdomInstance);
```

### Harmony Prefix/Postfix Examples

```csharp
// Patch a method to read values
[HarmonyPatch(typeof(Steed), "UpdateStamina")]
[HarmonyPrefix]
static bool Prefix_UpdateStamina(Steed __instance)
{
    var staminaField = AccessTools.Field(typeof(Steed), "SteedStaminaID");
    int staminaId = (int)staminaField.GetValue(__instance);
    
    // Your logic here
    
    return true; // Continue to original method
}

// Patch to modify behavior
[HarmonyPatch(typeof(Banker), "SetStash")]
[HarmonyPrefix]
static bool Prefix_SetStash(Banker __instance, ref int amount)
{
    // Modify the stash amount before it's set
    amount *= 2; // Double the stash!
    return true;
}
```

### Finding Game Objects at Runtime

```csharp
// Find Director
var director = GameObject.FindObjectOfType<Director>();

// Find Kingdom
var kingdom = GameObject.FindObjectOfType<Kingdom>();

// Find all Steeds
var steeds = GameObject.FindObjectsOfType<Steed>();

// Access through hierarchy
var banker = kingdom.GetComponent<Banker>();
```

## Important Notes

1. **IL2CPP Note:** This game likely uses IL2CPP, so you may need:
   - IL2CPP Interop libraries
   - Proper pointer handling
   - Type conversion helpers

2. **Type Inference:** Types in the stubs are inferred from naming patterns:
   - `int`: count, amount, num, index, id
   - `float`: speed, stamina, health, hunger, time, timer
   - `bool`: is, has, can, enable, active
   - `string`: name, text, id
   - `object`: when type cannot be inferred

3. **Actual Types:** To get exact types at runtime:
   ```csharp
   var field = AccessTools.Field(typeof(ClassName), "fieldName");
   Type actualType = field.FieldType;
   Console.WriteLine($"Field type: {actualType.FullName}");
   ```

4. **Private Fields:** Many fields start with `_` or `__` indicating private/internal fields.

5. **NativeFieldInfoPtr:** These correspond to actual private fields in the compiled code.

## Recommended Tools

- **dnSpy** - Best for IL2CPP decompilation
- **ILSpy** - Alternative decompiler
- **Harmony** - Runtime patching
- **BepInEx** - Mod loader framework
- **IL2CPP Unhollower** - For IL2CPP type conversion

## Next Steps

1. Set up BepInEx or similar mod loader
2. Use Harmony to patch methods
3. Test AccessTools with the provided field names
4. Iterate based on actual runtime types
5. Create helper methods for common operations

## Support

If you need to verify exact types or method signatures at runtime:
1. Use reflection to inspect loaded types
2. Check method parameter types with `MethodInfo.GetParameters()`
3. Use debuggers to inspect live instances
4. Log field types during runtime

Good luck with your modding!
