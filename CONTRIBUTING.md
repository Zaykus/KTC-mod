# Contributing Guide

## Development Environment Setup

1. **Clone the repository (with submodules)**
   ```bash
   git clone --recurse-submodules https://github.com/Zaykus/KTC-mod.git
   cd KTC-mod
   ```

   If you already cloned without `--recurse-submodules`:
   ```bash
   git submodule update --init --recursive
   ```

2. **Configure local paths**
   ```bash
   # Windows
   copy Directory.Build.props.example Directory.Build.props
   # Linux / Steam Deck
   cp Directory.Build.props.example Directory.Build.props
   ```
   Edit `Directory.Build.props` and set `BepInExPath` to your game's BepInEx directory.

3. **Install dependencies**
   - [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
   - Kingdom Two Crowns (via Steam)

4. **Build**
   ```bash
   # Windows — builds both IL2CPP and Mono
   ./build.ps1
   # IL2CPP only
   ./build.ps1 -SkipMono

   # Linux — builds both IL2CPP and Mono
   ./build.sh
   # IL2CPP only
   ./build.sh --skip-mono
   ```

## Build Configurations

| Configuration | Target Framework | Description |
|:---|:---|:---|
| `Debug` | `net6.0` | Development build with IL2CPP references |
| `BIE6_IL2CPP` | `net6.0` | Release build for IL2CPP game version |
| `BIE6_Mono` | `netstandard2.1` | Release build for Mono game version |

## Branch Strategy

| Branch | Purpose |
|:---|:---|
| `main` | Stable releases |
| `develop` | Main development branch |
| `feature/*` | New feature development |
| `fix/*` | Bug fixes |
| `release/*` | Release preparation |

## PR Workflow

1. Create a feature branch from `develop`
2. Write code and test
3. Submit PR targeting `develop`
4. CI build must pass (both BIE6_IL2CPP and BIE6_Mono); at least one code review required
5. Merge into `develop`

## Code Style

- 4 spaces for indentation
- XML doc comments on all public methods
- Follow `.editorconfig` rules
- Use C# latest language version
- IL2CPP-only code must be wrapped in `#if IL2CPP`
- Mono-compatible code must not reference `Il2CppInterop` or `Il2CppSystem` types directly

## Commit Conventions

- Use present tense: "add" not "added"
- Keep messages concise and descriptive

## Further Reading

- [Project Architecture](docs/ARCHITECTURE.md) — codebase layers, data flow, key class dependencies
- [IL2CPP / Mono Dual Compatibility](docs/IL2CPP-MONO-COMPAT.md) — how the build system and code achieve cross-runtime support
