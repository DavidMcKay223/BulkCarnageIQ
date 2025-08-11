using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Widget;
using Java.Time.Format;

namespace CarnageAndroid
{
    public class CarnageButton : Button
    {
        public CarnageButton(Context context) : base(context) => Init();
        public CarnageButton(Context context, IAttributeSet attrs) : base(context, attrs) => Init();
        public CarnageButton(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr) => Init();

        private void Init()
        {
            SetStyle(CarnageButtonStyle.Default);
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
                    paddingDp = CarnageStyle.PaddingLarge;
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

            SetBackgroundColor(backgroundColor);
            SetTextColor(textColor);
            SetTextSize(Android.Util.ComplexUnitType.Sp, CarnageStyle.FontSizeMedium);
            SetPaddingDp(paddingDp);
            return this;
        }

        private void SetPaddingDp(int dp)
        {
            float scale = Context.Resources.DisplayMetrics.Density;
            int px = (int)(dp * scale + 0.5f);
            SetPadding(px, px, px, px);
        }
    }
}
