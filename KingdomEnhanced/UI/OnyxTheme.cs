using UnityEngine;
using System;
using System.Collections.Generic;

namespace KingdomEnhanced.UI
{
    public static class OnyxTheme
    {
        private static bool _initialized;

        // Cached Textures
        public static Texture2D WindowBg;
        public static Texture2D SidebarBg;
        public static Texture2D CardBg;
        public static Texture2D AccentPill;
        public static Texture2D ToggleOn;
        public static Texture2D ToggleOff;
        public static Texture2D SelectionBox;

        // Color Palette (Onyx Electric)
        public static readonly Color C_Onyx      = new Color(0.07f, 0.07f, 0.08f, 0.98f);
        public static readonly Color C_Sidebar   = new Color(0.05f, 0.05f, 0.06f, 1.00f);
        public static readonly Color C_Glass     = new Color(1.00f, 1.00f, 1.00f, 0.04f);
        public static readonly Color C_Electric  = new Color(0.00f, 0.64f, 1.00f, 1.00f);
        public static readonly Color C_ElectricDim = new Color(0.00f, 0.40f, 0.70f, 1.00f);
        public static readonly Color C_Text      = new Color(0.95f, 0.95f, 0.98f, 1.00f);
        public static readonly Color C_TextDim   = new Color(0.55f, 0.58f, 0.65f, 1.00f);
        public static readonly Color C_Danger    = new Color(1.00f, 0.25f, 0.30f, 1.00f);

        public static void Initialize()
        {
            if (_initialized) return;

            // Generate textures once and cache them
            WindowBg     = CreateRoundedTex(32, 32, 12, C_Onyx);
            SidebarBg    = CreateRoundedTex(32, 32, 12, C_Sidebar, true); // Left-side rounding only
            CardBg       = CreateRoundedTex(32, 32, 6, C_Glass);
            AccentPill   = CreateRoundedTex(32, 24, 12, C_Electric);
            ToggleOn     = CreateRoundedTex(48, 24, 12, C_Electric);
            ToggleOff    = CreateRoundedTex(48, 24, 12, new Color(0.2f, 0.2f, 0.22f, 1f));
            SelectionBox = CreateRoundedTex(32, 32, 4, new Color(1f, 1f, 1f, 0.1f));

            _initialized = true;
        }

        private static Texture2D CreateRoundedTex(int width, int height, int radius, Color color, bool leftOnly = false)
        {
            // IL2CPP Safety: Explicit RGBA32 and no mipmaps
            var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.hideFlags = HideFlags.HideAndDontSave;
            tex.filterMode = FilterMode.Bilinear;

            var pixels = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool isRounded = false;

                    // Corner Check
                    if (!leftOnly)
                    {
                        // Top Right
                        if (x >= width - radius && y >= height - radius)
                        {
                            if (Vector2.Distance(new Vector2(x, y), new Vector2(width - radius - 1, height - radius - 1)) > radius)
                                isRounded = true;
                        }
                        // Bottom Right
                        else if (x >= width - radius && y < radius)
                        {
                            if (Vector2.Distance(new Vector2(x, y), new Vector2(width - radius - 1, radius)) > radius)
                                isRounded = true;
                        }
                    }

                    // Top Left
                    if (x < radius && y >= height - radius)
                    {
                        if (Vector2.Distance(new Vector2(x, y), new Vector2(radius, height - radius - 1)) > radius)
                            isRounded = true;
                    }
                    // Bottom Left
                    else if (x < radius && y < radius)
                    {
                        if (Vector2.Distance(new Vector2(x, y), new Vector2(radius, radius)) > radius)
                            isRounded = true;
                    }

                    pixels[y * width + x] = isRounded ? Color.clear : color;
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }
    }
}
