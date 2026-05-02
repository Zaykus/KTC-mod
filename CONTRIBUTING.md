# Contributing Guide

## Development Environment Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/Zaykus/KTC-mod.git
   cd KTC-mod
   ```

2. **Configure local paths**
   ```bash
   # Windows
   copy Directory.Build.props.example Directory.Build.props
   # Linux / Steam Deck
   cp Directory.Build.props.example Directory.Build.props
   ```
   Edit `Directory.Build.props` and point `GameDir` to your Kingdom Two Crowns game directory.

3. **Install dependencies**
   - [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
   - Kingdom Two Crowns (via Steam)
   - BepInEx IL2CPP (comes with the game installation)

4. **Build**
   ```bash
   # Windows
   ./build.ps1
   # Linux
   ./build.sh
   ```

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
4. CI build must pass; at least one code review required
5. Merge into `develop`

## Code Style

- 4 spaces for indentation
- XML doc comments on all public methods
- Follow `.editorconfig` rules
- Use C# latest language version

## Commit Conventions

- Use present tense: "add" not "added"
- Keep messages concise and descriptive
