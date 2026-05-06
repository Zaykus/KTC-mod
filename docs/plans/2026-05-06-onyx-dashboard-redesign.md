# Onyx Dashboard UI Redesign Implementation Plan (v2)

> **For Antigravity:** REQUIRED WORKFLOW: Use `.agent/workflows/execute-plan.md` to execute this plan in single-flow mode.

**Goal:** Completely redesign the mod menu into a high-performance, sidebar-driven "Onyx Dashboard" with cached assets and IL2CPP-safe texture generation.

**Architecture:** We will implement a static `OnyxTheme` registry to cache all generated UI textures (rounded corners, gradients) in memory using `TextureFormat.RGBA32`. The `ModMenu` will be refactored to a 2-column sidebar layout while preserving the global notification and overlay systems.

**Tech Stack:** C#, Unity IMGUI (GUILayout), IL2CPP-compatible reflection.

---

### Task 1: Initialize Plan & Documentation
**Files:**
- Create: `docs/plans/2026-05-06-onyx-dashboard-redesign.md`
- Create: `docs/plans/task.md`

**Step 1: Create directories and tracker**
- [x] Run: `mkdir docs/plans`
- [x] Create: `docs/plans/task.md`

**Step 2: Commit**
```bash
git add docs/plans/
git commit -m "docs: initialize onyx redesign plan v2"
```

---

### Task 2: Implement Cached Theme System (IL2CPP Safe)
**Files:**
- Create: `KingdomEnhanced/UI/OnyxTheme.cs`

**Step 1: Write OnyxTheme with Texture Caching**
- Use `TextureFormat.RGBA32` and `mipChain: false`.
- Implement `CreateRoundedTex` with pixel-perfect rounding logic.

```csharp
private static Texture2D CreateRoundedTex(int width, int height, int radius, Color color)
{
    var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
    tex.hideFlags = HideFlags.HideAndDontSave;
    // ... pixel loop logic ...
    tex.Apply();
    return tex;
}
```

**Step 2: Commit**
```bash
git add KingdomEnhanced/UI/OnyxTheme.cs
git commit -m "feat: add OnyxTheme with IL2CPP-safe texture caching"
```

---

### Task 3: Update Feature Metadata for Search & Icons
**Files:**
- Modify: `KingdomEnhanced/UI/ModMenu.cs` (struct definition)
- Modify: `KingdomEnhanced/UI/ModMenuFeatures.cs`

**Step 1: Add Icon and Tags to FeatureMeta**
Expand the struct to support categorization and visual cues.

**Step 2: Update Build() in ModMenuFeatures**
Add icons and search tags to the registration methods.

**Step 3: Commit**
```bash
git commit -m "feat: expand feature metadata for dashboard discovery"
```

---

### Task 4: Sidebar Layout Rewrite (Preserving Overlays)
**Files:**
- Modify: `KingdomEnhanced/UI/ModMenu.cs`

**Step 1: Implement Sidebar Draw Logic**
Refactor `DrawWindow` to a 2-column layout. Sidebar width: 200px.

**Step 2: Maintain Visibility Gate Location**
Ensure `DrawNotificationLog()` and `DrawFeedbackOverlay()` remain ABOVE the `if (!_isVisible) return;` check so they function while the menu is closed.

**Step 3: Commit**
```bash
git commit -m "feat: implement sidebar layout while preserving global overlays"
```

---

### Task 5: Final Polish & Theme Application
**Files:**
- Modify: `KingdomEnhanced/UI/ModMenu.cs`
- Modify: `Shared/GuiHelper.cs`

**Step 1: Apply Onyx Styles**
Replace legacy gold colors with Onyx electric accents and apply the cached backgrounds.

**Step 2: Commit**
```bash
git commit -m "ui: finalize Onyx Dashboard visuals"
```
