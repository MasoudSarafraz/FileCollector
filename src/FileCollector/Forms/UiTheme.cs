using System.Drawing;

namespace FileCollector.Forms
{
    /// <summary>
    /// Shared UI theme: colors, fonts, and spacing constants used across
    /// all forms for consistent appearance.
    /// Palette: white + very light gray + very light cream (no bright colors).
    /// </summary>
    public static class UiTheme
    {
        // ===== Colors =====

        /// <summary>Very light cream — page/form background.</summary>
        public static readonly Color BgForm = Color.FromArgb(250, 247, 242);

        /// <summary>White — cards, groups, panels.</summary>
        public static readonly Color BgPanel = Color.White;

        /// <summary>Cream tint — alternating sections, hint bars.</summary>
        public static readonly Color BgGroupAlt = Color.FromArgb(245, 241, 234);

        /// <summary>Grid header background.</summary>
        public static readonly Color BgGridHeader = Color.FromArgb(245, 242, 236);

        /// <summary>Grid alternating row.</summary>
        public static readonly Color BgGridAltRow = Color.FromArgb(250, 247, 242);

        /// <summary>Disabled input background.</summary>
        public static readonly Color DisabledBg = Color.FromArgb(239, 234, 224);

        /// <summary>Warm light gray border.</summary>
        public static readonly Color BorderLight = Color.FromArgb(230, 223, 211);

        /// <summary>Stronger border (for emphasis).</summary>
        public static readonly Color BorderStrong = Color.FromArgb(201, 192, 175);

        /// <summary>Primary text color.</summary>
        public static readonly Color TextDark = Color.FromArgb(43, 43, 43);

        /// <summary>Secondary text color.</summary>
        public static readonly Color TextMedium = Color.FromArgb(107, 107, 107);

        /// <summary>Hint/helper text color.</summary>
        public static readonly Color HintText = Color.FromArgb(142, 134, 120);

        /// <summary>Muted accent — OK button border, focus indicators.</summary>
        public static readonly Color Accent = Color.FromArgb(107, 99, 87);

        // ===== Fonts =====

        public static readonly Font FontRegular = new Font("Tahoma", 9.75F);
        public static readonly Font FontBold = new Font("Tahoma", 9.75F, FontStyle.Bold);
        public static readonly Font FontGroupTitle = new Font("Tahoma", 10F, FontStyle.Bold);
        public static readonly Font FontSmall = new Font("Tahoma", 8.5F);
        public static readonly Font FontSmallItalic = new Font("Tahoma", 8.5F, FontStyle.Italic);
        public static readonly Font FontMono = new Font("Consolas", 9F);

        // ===== Spacing constants =====

        public const int RowHeight = 32;
        public const int LabelColumnWidth = 130;
        public const int GroupPadding = 12;
        public const int GroupSpacing = 8;
        public const int ButtonHeight = 34;
        public const int ButtonWidth = 110;
    }
}
