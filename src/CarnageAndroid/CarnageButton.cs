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
            // Set base style properties
            SetAllCaps(false);
            StrokeWidth = 0;
            CornerRadius = Context.DpToPx(CarnageStyle.CornerRadius);
            Elevation = CarnageStyle.Elevation;
            LetterSpacing = 0.05f;
            Typeface = Typeface.Create("sans-serif-medium", TypefaceStyle.Normal);

            // Apply a default modern look
            WithStyle(CarnageButtonStyle.Default);
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

        public CarnageButton WithStyle(CarnageButtonStyle style)
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
                    WithLiquidEffect(CarnageStyle.PrimaryColor, CarnageStyle.PrimaryColorDark);
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

            BackgroundTintList = ColorStateList.ValueOf(backgroundColor);

            this
                .WithTextColor(textColor)
                .WithTextSize(ComplexUnitType.Sp, CarnageStyle.FontSizeMedium)
                .WithPaddingDp(paddingDp);

            return this;
        }

        public CarnageButton WithLiquidEffect(Color startColor, Color pressedColor)
        {
            var stateList = new int[][]
            {
                new int[] { Android.Resource.Attribute.StatePressed },
                new int[] { }
            };

            var colorStateList = new ColorStateList(
                stateList,
                new int[]
                {
                    pressedColor,
                    startColor
                }
            );

            BackgroundTintList = colorStateList;

            SetStrokeColorResource(Resource.Color.mtrl_btn_transparent_bg_color); // or a very subtle dark color
            SetRippleColorResource(Resource.Color.m3_ref_palette_white); // a clean, subtle ripple

            return this;
        }
    }
}
