using Android.Content;
using Android.Graphics;
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
            SetStyle(CarnageButtonStyle.Default);
            SetAllCaps(false); // Optional: disable all-caps default
            StrokeWidth = 0;
            CornerRadius = DpToPx(CarnageStyle.CornerRadius);
            Elevation = CarnageStyle.Elevation;
        }

        public CarnageButton WithText(string text)
        {
            Text = text;
            return this;
        }

        public CarnageButton WithColor(string hex)
        {
            BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Color.ParseColor(hex));
            return this;
        }

        public CarnageButton OnClick(Action action)
        {
            Click += (s, e) => action();
            return this;
        }

        public CarnageButton SetStyle(CarnageButtonStyle style)
        {
            Color backgroundColor;
            Color textColor;
            int paddingDp;

            switch (style)
            {
                case CarnageButtonStyle.Primary:
                    backgroundColor = CarnageStyle.PrimaryColor;
                    textColor = CarnageStyle.BackgroundColor;
                    paddingDp = CarnageStyle.PaddingMedium;
                    break;
                case CarnageButtonStyle.Danger:
                    backgroundColor = CarnageStyle.DangerColor;
                    textColor = CarnageStyle.BackgroundColor;
                    paddingDp = CarnageStyle.PaddingMedium;
                    break;
                case CarnageButtonStyle.Secondary:
                    backgroundColor = CarnageStyle.SecondaryColor;
                    textColor = CarnageStyle.TextPrimaryColor;
                    paddingDp = CarnageStyle.PaddingSmall;
                    break;
                default:
                    backgroundColor = CarnageStyle.BackgroundColor;
                    textColor = CarnageStyle.TextPrimaryColor;
                    paddingDp = CarnageStyle.PaddingMedium;
                    break;
            }

            BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(backgroundColor);
            SetTextColor(textColor);
            SetTextSize(ComplexUnitType.Sp, CarnageStyle.FontSizeMedium);
            SetPaddingDp(paddingDp);
            return this;
        }

        private void SetPaddingDp(int dp)
        {
            int px = DpToPx(dp);
            SetPadding(px, px, px, px);
        }

        private int DpToPx(float dp)
        {
            float scale = Context.Resources.DisplayMetrics.Density;
            return (int)(dp * scale + 0.5f);
        }
    }
}
