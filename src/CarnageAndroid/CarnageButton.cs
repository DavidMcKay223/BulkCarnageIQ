using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Google.Android.Material.Button;
using System;

namespace CarnageAndroid
{
    public class CarnageButton : MaterialButton
    {
        public CarnageButton(Context context)
            : base(context, null, Resource.Attribute.materialButtonStyle) => Init();

        public CarnageButton(Context context, IAttributeSet attrs)
            : base(context, attrs, Resource.Attribute.materialButtonStyle) => Init();

        public CarnageButton(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr) => Init();

        private void Init()
        {
            SetAllCaps(false);
            StrokeWidth = 0;
            CornerRadius = Context.DpToPx(CarnageStyle.CornerRadius);
            Elevation = CarnageStyle.Elevation;
            LetterSpacing = 0.05f;
            Typeface = Typeface.Create("sans-serif-medium", TypefaceStyle.Normal);

            BackgroundTintList = ColorStateList.ValueOf(CarnageStyle.PrimaryRed);

            this
                .WithTextColor(CarnageStyle.White)
                .WithTextSize(ComplexUnitType.Sp, CarnageStyle.FontSizeMedium)
                .WithPaddingDp(CarnageStyle.PaddingMedium);
        }

        public CarnageButton WithText(string text)
        {
            Text = text;
            return this;
        }

        public CarnageButton WithColor(Color color)
        {
            BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(color);
            return this;
        }

        public CarnageButton OnClick(Action action)
        {
            Click += (s, e) => action();
            return this;
        }

        public CarnageButton AsPill()
        {
            ViewTreeObserver.GlobalLayout += (sender, e) =>
            {
                CornerRadius = Height / 2;
            };
            return this;
        }
    }
}
