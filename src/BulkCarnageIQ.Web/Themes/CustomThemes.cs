using MudBlazor;

namespace BulkCarnageIQ.Web.Themes
{
    public static class CustomThemes
    {
        public static readonly MudTheme BulkCarnageDarkTheme = new()
        {
            PaletteDark = new PaletteDark()
            {
                Black = "#000000",
                White = "#FFFFFF",

                Primary = "#ff3d00",               // vivid red-orange
                PrimaryContrastText = "#ffffff",

                Secondary = "#383838",             // lighter secondary for better contrast
                SecondaryContrastText = "#ffffff",

                Tertiary = "#6a120a",              // rich dark red-brown
                TertiaryContrastText = "#ffffff",

                Info = "#ff7043",                  // blood orange, readable on dark
                InfoContrastText = "#ffffff",

                Success = "#66bb6a",               // soft green, readable
                SuccessContrastText = "#ffffff",

                Warning = "#ffa726",               // bright orange-yellow
                WarningContrastText = "#000000",

                Error = "#ef5350",                 // light crimson red
                ErrorContrastText = "#ffffff",

                Dark = "#1a1a1a",                  // clean dark base
                DarkContrastText = "#ffffff",

                TextPrimary = "#f2f2f2",           // near-white for text
                TextSecondary = "#cccccc",         // light gray for secondary

                DrawerBackground = "#121212",
                DrawerText = "#ffffff",
                DrawerIcon = "#ff7043",

                AppbarBackground = "#000000",
                AppbarText = "#ffffff",

                Surface = "#1f1f1f",               // slightly brighter surface for chip contrast

                LinesDefault = "#2f2f2f",
                TableLines = "#2f2f2f",
                TableStriped = "#252525",
                TableHover = "#2a2a2a",
                Divider = "#333333",

                GrayLight = "#888888",
                GrayLighter = "#cccccc",
                GrayDark = "#444444",

                HoverOpacity = 0.1,
                RippleOpacity = 0.15,
                RippleOpacitySecondary = 0.25
            },
            LayoutProperties = new LayoutProperties()
            {
                DefaultBorderRadius = "6px",
                DrawerWidthLeft = "240px",
                DrawerMiniWidthLeft = "56px",
                AppbarHeight = "64px"
            },
            Typography = new Typography()
            {
                Default = new DefaultTypography
                {
                    FontFamily = new[] { "Montserrat", "Roboto", "Arial", "sans-serif" },
                    FontWeight = "400",
                    FontSize = "0.9rem"
                },
                H6 = new H6Typography
                {
                    FontWeight = "600",
                    FontSize = "1.1rem",
                    LetterSpacing = "0.5px"
                },
                Button = new ButtonTypography
                {
                    FontWeight = "600",
                    TextTransform = "uppercase"
                }
            },
            ZIndex = new ZIndex()
            {
                Drawer = 1100,
                AppBar = 1300,
                Dialog = 1400,
                Snackbar = 1500,
                Tooltip = 1600
            }
        };
    }
}
