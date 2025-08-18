using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Widget;

namespace CarnageAndroid
{
    public enum CarnageIconPosition
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public enum CarnageIcon
    {
        Home,
        Settings,
        Favorite,
        FoodTracker,
        Tracker,
        Fitness,
        Profile,
        Trash,
        Delete
    }

    public static class CarnageIconResolver
    {
        public static int GetResource(CarnageIcon icon) => icon switch
        {

            CarnageIcon.Home => Resource.Drawable.ic_home,
            //CarnageIcon.Settings => Resource.Drawable.ic_settings,
            //CarnageIcon.Favorite => Resource.Drawable.ic_favorite,
            CarnageIcon.FoodTracker => Resource.Drawable.ic_food_tracker,
            CarnageIcon.Tracker => Resource.Drawable.ic_tracker,
            CarnageIcon.Fitness => Resource.Drawable.ic_fitness,
            //CarnageIcon.Profile => Resource.Drawable.ic_profile,
            CarnageIcon.Trash => Resource.Drawable.ic_trash,
            CarnageIcon.Delete => Resource.Drawable.ic_delete,
            _ => Resource.Drawable.ic_home,
        };
    }

    public static class CarnageStyle
    {
        // Base Palette
        public static readonly Color White = Color.ParseColor("#FFFFFF");
        public static readonly Color Charcoal = Color.ParseColor("#211212");
        public static readonly Color MediumGray = Color.ParseColor("#757575");
        public static readonly Color DeepBrown = Color.ParseColor("#472424");

        // Accent Palette (Reds)
        public static readonly Color PrimaryRed = Color.ParseColor("#BD0F0F");
        public static readonly Color DarkMaroon = Color.ParseColor("#663333");
        public static readonly Color PaleRose = Color.ParseColor("#C99191");
        public static readonly Color DarkestBrown = Color.ParseColor("#331A1A");

        // Accent Palette (Blues)
        public static readonly Color MidnightBlue = Color.ParseColor("#1E2533");
        public static readonly Color VividBlue = Color.ParseColor("#3B72F4");
        public static readonly Color DarkCharcoal = Color.ParseColor("#2B3344");
        public static readonly Color OffWhite = Color.ParseColor("#E0E0E0");
        public static readonly Color SlateGray = Color.ParseColor("#384256");

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
