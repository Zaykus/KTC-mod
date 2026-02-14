namespace KingdomEnhanced.Core
{
    /// <summary>
    /// Single source of truth for mod version.
    /// Update ONLY here — all other files reference these constants.
    /// </summary>
    public static class ModVersion
    {
        public const string MAJOR = "1";
        public const string MINOR = "5";
        public const string PATCH = "0";
        public const string FULL = MAJOR + "." + MINOR + "." + PATCH;
        public const string DISPLAY = "v" + FULL;
    }
}
