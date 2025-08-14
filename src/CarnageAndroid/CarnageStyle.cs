using Android.Content;
using Android.Util;
using Android.Widget;
using Android.Graphics;

namespace CarnageAndroid
{
    public enum CarnageButtonStyle
    {
        Default,
        Primary,
        Danger,
        Secondary
    }

    public enum CarnageTextFieldStyle
    {
        Default,
        Filled,
        Outline,
        Error
    }

    public enum CarnageTextViewStyle
    {
        Default,
        Title,
        Primary,
        Secondary
    }

    public enum CarnageProgressStyle
    {
        Default,
        Primary,
        Danger,
        Secondary
    }

    public static class CarnageStyle
    {
        // Colors
        public static readonly string PrimaryColorHex = "#FF5722";
        public static readonly string SecondaryColorHex = "#03DAC6";
        public static readonly string DangerColorHex = "#B00020";
        public static readonly string BackgroundColorHex = "#FFFFFF";
        public static readonly string TextPrimaryColorHex = "#212121";
        public static readonly string TextSecondaryColorHex = "#757575";
        public static readonly string HintTextColorHex = "#AA000000";
        public static readonly string CharcoalGrayHex = "#121212";

        // Color objects (if you want direct Android.Graphics.Color)
        public static readonly Color PrimaryColor = Color.ParseColor(PrimaryColorHex);
        public static readonly Color SecondaryColor = Color.ParseColor(SecondaryColorHex);
        public static readonly Color DangerColor = Color.ParseColor(DangerColorHex);
        public static readonly Color BackgroundColor = Color.ParseColor(BackgroundColorHex);
        public static readonly Color TextPrimaryColor = Color.ParseColor(TextPrimaryColorHex);
        public static readonly Color TextSecondaryColor = Color.ParseColor(TextSecondaryColorHex);
        public static readonly Color HintTextColor = Color.ParseColor(HintTextColorHex);
        public static readonly Color CharcoalGray = Color.ParseColor(CharcoalGrayHex);

        // Font sizes (in SP)
        public const float FontSizeSmall = 12f;
        public const float FontSizeMedium = 16f;
        public const float FontSizeLarge = 20f;

        // Padding (in DP)
        public const int PaddingSmall = 8;
        public const int PaddingMedium = 16;
        public const int PaddingLarge = 24;

        // Corner radius for cards/buttons (in DP)
        public const float CornerRadius = 8f;

        // Elevation/shadow (in DP)
        public const float Elevation = 4f;
    }
}
