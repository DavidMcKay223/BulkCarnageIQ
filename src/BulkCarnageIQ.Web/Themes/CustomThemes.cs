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
                Primary = "#d9230f",
                PrimaryContrastText = "#ffffff",
                Secondary = "#1e1e1e",
                SecondaryContrastText = "#ffffff",
                Tertiary = "#570e06",
                TertiaryContrastText = "#f8f9fa",
                Info = "#fd5533", // blood orange
                InfoContrastText = "#ffffff",
                Success = "#4caf50",
                SuccessContrastText = "#1b3e20",
                Warning = "#ff9800",
                WarningContrastText = "#3a2200",
                Error = "#f44336",
                ErrorContrastText = "#2a0c0a",
                Dark = "#121212",
                DarkContrastText = "#ffffff",
                TextPrimary = "#f2f2f2",
                TextSecondary = "#bbbbbb",
                DrawerBackground = "#1a1a1a",
                DrawerText = "#ffffff",
                DrawerIcon = "#fd5533",
                AppbarBackground = "#000000",
                AppbarText = "#ffffff",
                Surface = "#121212",
                LinesDefault = "#2c2c2c",
                TableLines = "#2c2c2c",
                TableStriped = "#1c1c1c",
                TableHover = "#222222",
                Divider = "#2c2c2c",
                GrayLight = "#6c6c6c",
                GrayLighter = "#9e9e9e",
                GrayDark = "#333333",
                HoverOpacity = 0.08,
                RippleOpacity = 0.1,
                RippleOpacitySecondary = 0.2,
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
