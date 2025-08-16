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
        // Color objects
        public static readonly Color PrimaryColor = Color.ParseColor("#FF5722");
        public static readonly Color PrimaryColorDark = Color.ParseColor("#BF360C");
        public static readonly Color SecondaryColor = Color.ParseColor("#03DAC6");
        public static readonly Color DangerColor = Color.ParseColor("#B00020");
        public static readonly Color BackgroundColor = Color.ParseColor("#FFFFFF");
        public static readonly Color TextPrimaryColor = Color.ParseColor("#212121");
        public static readonly Color TextSecondaryColor = Color.ParseColor("#757575");
        public static readonly Color HintTextColor = Color.ParseColor("#AA000000");
        public static readonly Color CharcoalGray = Color.ParseColor("#121212");

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
