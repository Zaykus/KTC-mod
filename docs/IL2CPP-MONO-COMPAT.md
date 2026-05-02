# IL2CPP / Mono 双兼容架构

本文档分析 `KingdomMapModDev` 项目如何通过**构建系统**、**条件编译**和**抽象层**三条路径实现单一代码库同时兼容 IL2CPP 和 Mono 两套 BepInEx 运行时。

---

## 1. 构建系统（MSBuild）

### 1.1 三配置模型

所有 `.csproj` 定义三种构建配置，通过 `DefineConstants` 注入编译时符号：

```xml
<PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Debug|AnyCPU'">
    <TargetFramework>net6.0</TargetFramework>
    <DefineConstants>IL2CPP,BIE,BIE6</DefineConstants>
</PropertyGroup>
<PropertyGroup Condition="'$(Configuration)|$(Platform)'=='BIE6_IL2CPP|AnyCPU'">
    <TargetFramework>net6.0</TargetFramework>
    <DefineConstants>IL2CPP,BIE,BIE6</DefineConstants>
</PropertyGroup>
<PropertyGroup Condition="'$(Configuration)|$(Platform)'=='BIE6_Mono|AnyCPU'">
    <TargetFramework>netstandard2.1</TargetFramework>
    <DefineConstants>MONO,BIE,BIE6</DefineConstants>
</PropertyGroup>
```

| 配置 | 目标框架 | 编译常量 | 用途 |
|:---|:---|:---|:---|
| `Debug` | `net6.0` | `IL2CPP, BIE, BIE6` | 开发调试（与 IL2CPP 相同的 DLL 引用） |
| `BIE6_IL2CPP` | `net6.0` | `IL2CPP, BIE, BIE6` | 发布 IL2CPP 版本 |
| `BIE6_Mono` | `netstandard2.1` | `MONO, BIE, BIE6` | 发布 Mono 版本 |

### 1.2 配置条件的 DLL 引用

```xml
<!-- BIE6_IL2CPP / Debug -->
<ItemGroup Condition="'$(Configuration)'=='BIE6_IL2CPP' or '$(Configuration)'=='Debug'">
    <Reference Include="BepInEx.Unity.IL2CPP">
        <HintPath>..\deps\KTC-ModDevLibs\BIE6_IL2CPP\core\BepInEx.Unity.IL2CPP.dll</HintPath>
        <Private>False</Private>
    </Reference>
    <!-- Il2Cpp interop DLLs -->
    <Reference Include="Assembly-CSharp">
        <HintPath>..\deps\KTC-ModDevLibs\BIE6_IL2CPP\interop\Assembly-CSharp.dll</HintPath>
    </Reference>
    <Reference Include="Il2Cppmscorlib">
        <HintPath>..\deps\KTC-ModDevLibs\BIE6_IL2CPP\interop\Il2Cppmscorlib.dll</HintPath>
    </Reference>
    <!-- ... -->
</ItemGroup>

<!-- BIE6_Mono -->
<ItemGroup Condition="'$(Configuration)'=='BIE6_Mono'">
    <Reference Include="BepInEx.Unity.Mono">
        <HintPath>..\deps\KTC-ModDevLibs\BIE6_Mono\core\BepInEx.Unity.Mono.dll</HintPath>
    </Reference>
    <Reference Include="Assembly-CSharp">
        <HintPath>..\deps\KTC-ModDevLibs\BIE6_Mono\Managed\Assembly-CSharp-publicized.dll</HintPath>
    </Reference>
    <!-- 注意：无 Il2Cpp 系列 DLL（Il2Cppmscorlib, Il2CppInterop 等） -->
</ItemGroup>
```

**关键差异**：
- IL2CPP：引用 `BepInEx.Unity.IL2CPP` + 全套 `Il2CppInterop` / `Il2CppSystem` DLL
- Mono：引用 `BepInEx.Unity.Mono` + `Assembly-CSharp-publicized.dll`（publicized 版本暴露所有私有成员），**不需要** Il2Cpp 专用 DLL

所有 DLL 统一由 Git Submodule `deps/KTC-ModDevLibs` 提供，按 `BIE6_IL2CPP/` 和 `BIE6_Mono/` 目录组织。

---

## 2. Plugin 入口点（基类切换）

每个 mod 的 Plugin 类是兼容性的核心入口，通过**条件编译**选择不同的基类和生命周期方法。

### 2.1 标准模式

```csharp
using BepInEx;
using BepInEx.Logging;

#if IL2CPP
using BepInEx.Unity.IL2CPP;
using KingdomMod.SharedLib.Attributes;
#endif

#if MONO
using BepInEx.Unity.Mono;
#endif

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("KingdomTwoCrowns.exe")]
public class MyPlugin :
#if IL2CPP
    BasePlugin          // IL2CPP 基类
#else
    BaseUnityPlugin     // Mono 基类
#endif
{
    public static MyPlugin Instance;

    // 日志适配
    public ManualLogSource LogSource
#if IL2CPP
        => Log;          // BasePlugin.Log
#else
        => Logger;       // BaseUnityPlugin.Logger
#endif

#if IL2CPP
    public override void Load()
    {
        // IL2CPP 必须注册所有含 [RegisterTypeInIl2Cpp] 的 MonoBehaviour
        RegisterTypeInIl2Cpp.RegisterAssembly(Assembly.GetExecutingAssembly());
        Init();
    }
#else
    internal void Awake()    // Mono 使用 Unity MonoBehaviour 生命周期
    {
        Init();
    }
#endif

    private void Init()
    {
        Instance = this;
        LogSource.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} loaded!");
        // 公共初始化逻辑
    }
}
```

### 2.2 差异表

| 项目 | IL2CPP | Mono |
|:---|:---|:---|
| 基类 | `BasePlugin` (BepInEx.Unity.IL2CPP) | `BaseUnityPlugin` (BepInEx.Unity.Mono) |
| 入口方法 | `override void Load()` | `void Awake()` |
| 日志属性 | `Log` | `Logger` |
| 类型注册 | 必须调用 `ClassInjector.RegisterTypeInIl2Cpp()` | 不需要 |

---

## 3. MonoBehaviour（类型注册和构造函数）

IL2CPP 要求所有 `MonoBehaviour` 子类在运行时通过 `ClassInjector` 注册，且构造函数必须调用基类的 `(IntPtr)` 构造函数。

### 3.1 标准模式

```csharp
#if IL2CPP
using KingdomMod.SharedLib.Attributes;
#endif

#if IL2CPP
[RegisterTypeInIl2Cpp]      // 自定义 Attribute，自动触发类型注册
#endif
public class MyHolder : MonoBehaviour
{
    public static MyHolder Instance { get; private set; }

#if IL2CPP
    public MyHolder(IntPtr ptr) : base(ptr) { }   // Il2CppObjectBase 要求的构造函数
#endif

    public static void Initialize(MyPlugin plugin)
    {
        // 创建 GameObject 并挂载组件
        Instance = new GameObject("MyHolder").AddComponent<MyHolder>();
        DontDestroyOnLoad(Instance.gameObject);
    }
}
```

### 3.2 `[RegisterTypeInIl2Cpp]` Attribute

此自定义 Attribute 挂载在 IL2CPP-only 的 `#if` 块中（Mono 编译时整个类不存在），它的静态方法 `RegisterAssembly()` 反射扫描程序集中所有带此 Attribute 的类并调用 `ClassInjector.RegisterTypeInIl2Cpp()`：

```csharp
#if IL2CPP
using Il2CppInterop.Runtime.Injection;

[AttributeUsage(AttributeTargets.Class)]
public class RegisterTypeInIl2Cpp : Attribute
{
    public static void RegisterAssembly(Assembly asm)
    {
        foreach (var type in asm.GetTypes())
        {
            var attr = type.GetCustomAttribute<RegisterTypeInIl2Cpp>(false);
            if (attr == null) continue;
            ClassInjector.RegisterTypeInIl2Cpp(type);
        }
    }
}
#endif
```

---

## 4. 集合兼容层（CompatCollections）

IL2CPP 和 Mono 的集合类型位于不同命名空间但 API 相同，通过 **条件 using** 实现无缝切换：

```csharp
#if IL2CPP
using Il2CppSystem.Collections.Generic;      // Il2Cpp 版本的 List<T>, Dictionary<K,V>, HashSet<T>
using Il2CppInterop.Runtime.InteropTypes.Arrays;
#else
using System.Collections.Generic;           // 托管版本的 List<T>, Dictionary<K,V>, HashSet<T>
#endif
```

由于 `List<T>` 在 Il2Cpp 和 Mono 下恰好引用不同的类型但 API 签名完全一致，文件其余代码无需修改即可通用于两种运行时。

### 4.1 工厂方法

`CompatCollections` 提供创建和转换的工厂方法：

```csharp
public static class CompatCollections
{
    public static List<T> CreateList<T>(params T[] items) { /* new List<T>() */ }
    public static HashSet<T> CreateHashSet<T>(params T[] items) { /* new HashSet<T>() */ }
    public static Dictionary<TKey, TValue> CreateDictionary<TKey, TValue>(...)
}
```

这些方法在 Il2CPP 下创建 `Il2CppSystem.Collections.Generic.List<T>`，在 Mono 下创建 `System.Collections.Generic.List<T>`——调用方不需要区分。

### 4.2 跨运行时转换

```csharp
// Il2Cpp List → 托管 List
public static List<T> ToManagedList<T>(this List<T> il2CppList) { ... }

// 托管集合 → Il2Cpp List
public static List<T> ToIl2CppList<T>(this IEnumerable<T> src) { ... }

// Il2Cpp Dictionary → 托管 Dictionary
public static Dictionary<TKey, TValue> ToManagedDictionary<TKey, TValue>(this Dictionary<TKey, TValue> dict) { ... }
```

### 4.3 Il2Cpp 专用扩展（Mono 不可见）

```csharp
#if IL2CPP
// Il2Cpp HashSet.ToArray() 需要 Il2CppStructArray 包装
public static T[] ToArray<T>(this HashSet<T> source) where T : unmanaged { ... }

// Il2Cpp LinkedList.AddBefore 的标准实现
public static LinkedListNode<T> AddBefore<T>(this LinkedList<T> @this, ...) { ... }
#endif
```

---

## 5. 类型转换兼容层

`Il2CppObjectBase` 提供了 `Cast<T>()` 和 `TryCast<T>()` 方法，但 Mono 中不存在这些方法。在 Mono 编译时提供等价的扩展方法：

```csharp
#if MONO
public static class ObjectExtensions
{
    public static T Cast<T>(this object @this) where T : class
        => @this as T ?? throw new InvalidCastException(...);

    public static T? TryCast<T>(this object? @this) where T : class
        => @this as T;
}
#endif
```

调用方代码统一写 `obj.Cast<T>()`，在 IL2CPP 下解析为 `Il2CppObjectBase.Cast<T>()`，在 Mono 下解析为此扩展方法。

---

## 6. NullableAttributes Polyfill（`netstandard2.1` 兼容）

Mono 构建的目标框架是 `netstandard2.1`，该框架没有内置 `System.Runtime.CompilerServices.NullableAttribute` 和 `NullableContextAttribute`。`SharedLib/Attributes/NullableAttributes.cs` 提供等价的多目标兼容实现：

```csharp
#if !NETSTANDARD2_1_OR_GREATER
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct /* ... */, Inherited = false)]
    internal sealed class NullableAttribute : Attribute { /* ... */ }
    internal sealed class NullableContextAttribute : Attribute { /* ... */ }
}
#endif
```

每个 `.csproj` 通过 `<Compile Include>` 的 `Link` 机制将此文件编译进各程序集——不需要额外引用。

---

## 7. `[HideFromIl2Cpp]` 属性

IL2CPP interop 的代码生成器会自动为所有 public 成员生成绑定代码。但以下 C# 特性无法被正确映射到 Il2Cpp：

- 泛型参数化的 delegate 类型（如 `Action<int, int>`）
- C# 原生 `event` 字段（Il2Cpp 使用 add/remove 委托对）
- `Dictionary<Type, ...>` 等复杂泛型签名

**解决方案**：对这类成员标注 `[HideFromIl2Cpp]`（来自 `Il2CppInterop.Runtime.Attributes`），告知 interop 代码生成器跳过它们。

```csharp
using Il2CppInterop.Runtime.Attributes;

[HideFromIl2Cpp]
public event GameStateEventHandler OnGameStateChanged;

[HideFromIl2Cpp]
public void SetResolvers(Dictionary<Type, List<IMarkerResolver>> resolvers) { ... }
```

这是 IL2CPP 互操作中最常见且必须掌握的技巧——标注遗漏会导致 IL2CPP 构建时的 `System.TypeLoadException`。

---

## 8. 组件解析器 / 映射器架构

OverlayMap 模组使用**策略模式 + 服务定位**实现可扩展的游戏对象→UI 标记映射系统：

```
                    ┌──────────────────┐
                    │  MapperInitializer │  ← 注册所有 Resolver / Mapper
                    └────────┬─────────┘
                             │
              ┌──────────────┼──────────────┐
              ▼              ▼              ▼
       ┌────────────┐ ┌───────────┐ ┌───────────┐
       │ CastleResolver│ │PortalResolver│ │ ... 40+   │ ← 每种游戏对象一个 Resolver
       └─────┬──────┘ └─────┬─────┘ └───────────┘
             │               │
             ▼               ▼
       ┌──────────┐   ┌───────────┐
       │ MapMarker │   │ WallLine   │  ← UI 组件
       └──────────┘   └───────────┘
```

### 8.1 接口定义

```csharp
public interface IMarkerResolver
{
    Type TargetComponentType { get; }   // 此 Resolver 要匹配的游戏组件类型
    ResolverType ResolverType { get; }
    MapMarkerType? Resolve(Component component);
}

public interface IComponentMapper
{
    void Map(Component component, NotifierType notifierType, ResolverType resolverType);
}
```

### 8.2 抽象基类（简化具体 Resolver 创建）

```csharp
public abstract class SimpleResolver : IMarkerResolver
{
    public Type TargetComponentType { get; }
    public MapMarkerType MarkerType { get; }

    protected SimpleResolver(Type targetType, MapMarkerType markerType)
    { TargetComponentType = targetType; MarkerType = markerType; }

    public virtual MapMarkerType? Resolve(Component component) => MarkerType;
}
```

具体 Resolver 只需一行继承：

```csharp
public class CastleResolver : SimpleResolver
{
    public CastleResolver() : base(typeof(Castle), MapMarkerType.Castle) { }
}
```

### 8.3 IL2CPP 特殊处理：指针缓存

由于 IL2CPP 不支持 `Dictionary<Type, ...>` 的泛型运行时查找，`TopMapView` 额外维护一个 `Dictionary<IntPtr, List<IMarkerResolver>>` 指针缓存，通过 `comp.GetIl2CppType().Pointer` 作为查找键。

---

## 9. 类型反射差异处理

### 6.1 获取运行时类型

```csharp
comp.
#if IL2CPP
    GetIl2CppType()     // Il2Cpp 运行时类型系统
#else
    GetType()           // 标准 CLR 反射
#endif
    .FullName;
```

### 6.2 条件 using（using 级条件编译）

```csharp
#if IL2CPP
using System.Reflection;       // 只在 IL2CPP 中需要 Assembly 反射
using BepInEx.Unity.IL2CPP;
using KingdomMod.SharedLib.Attributes;
#endif

#if MONO
using BepInEx.Unity.Mono;
#endif
```

---

## 10. 架构总览

```
                     ┌──────────────────────────┐
                     │     C# Source Code        │
                     │  (Shared between both)     │
                     │  #if IL2CPP / #if MONO    │
                     └──────────┬───────────────┘
                                │
              ┌─────────────────┴─────────────────┐
              │                                   │
    ┌─────────▼──────────┐              ┌─────────▼──────────┐
    │  BIE6_IL2CPP Build │              │   BIE6_Mono Build  │
    │  net6.0             │              │  netstandard2.1    │
    │  IL2CPP,BIE,BIE6   │              │  MONO,BIE,BIE6     │
    └─────────┬──────────┘              └─────────┬──────────┘
              │                                   │
    ┌─────────▼──────────┐              ┌─────────▼──────────┐
    │  deps/BIE6_IL2CPP/ │              │  deps/BIE6_Mono/   │
    │  core/ + interop/  │              │  core/ + Managed/  │
    │  Il2CppInterop     │              │  Assembly-CSharp   │
    │  Il2CppSystem      │              │  (publicized)      │
    └────────────────────┘              └────────────────────┘
```

### 10.1 兼容性抽象层

| 差异点 | IL2CPP | Mono | 统一方式 |
|:---|:---|:---|:---|
| Plugin 基类 | `BasePlugin` | `BaseUnityPlugin` | `#if` 条件编译 |
| MonoBehaviour 构造 | `MonoBehaviour(IntPtr ptr)` | 默认无参 | `#if IL2CPP` 添加构造函数 |
| 类型注册 | `ClassInjector.RegisterTypeInIl2Cpp()` | 不需要 | `[RegisterTypeInIl2Cpp]` Attribute（IL2CPP only） |
| 集合类型 | `Il2CppSystem.Collections.Generic.*` | `System.Collections.Generic.*` | 条件 `using` + `CompatCollections` |
| 类型转换 | `Il2CppObjectBase.Cast<T>()` | 不存在 | `#if MONO` 提供扩展方法 |
| 组件解析器/映射器 | 策略模式 + 指针缓存 | 原生 `Dictionary<Type, ...>` | `IMarkerResolver` / `IComponentMapper` 接口 + 抽象基类 |
| 日志属性 | `Log` | `Logger` | 条件属性 `LogSource` |

### 10.2 添加新 Mod 的步骤

1. 创建 `.csproj`，导入 `ProjectSettings.shared.props`，定义三配置
2. Plugin 类使用上述标准模式（条件基类、条件入口）
3. 所有 `MonoBehaviour` 添加 `[RegisterTypeInIl2Cpp]` 属性和 `(IntPtr)` 构造函数（`#if IL2CPP` 包裹）
4. 集合操作优先使用 `CompatCollections` 工厂方法
5. 需要 Il2Cpp 专用 API 时用 `#if IL2CPP` 包裹
