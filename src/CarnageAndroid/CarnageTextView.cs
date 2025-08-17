using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Widget;
using Google.Android.Material.TextView;
using static Android.Webkit.WebSettings;
using static System.Net.Mime.MediaTypeNames;

namespace CarnageAndroid
{
    public class CarnageTextView : MaterialTextView
    {
        public CarnageTextView(Context context) : base(context) => Init();
        public CarnageTextView(Context context, IAttributeSet attrs) : base(context, attrs) => Init();
        public CarnageTextView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr) => Init();

        private void Init()
        {
            TextSize = CarnageStyle.FontSizeMedium;
            Typeface = Typeface.Default;
            SetTextColor(CarnageStyle.PaleRose);
        }

        public CarnageTextView WithText(string text)
        {
            Text = text;
            return this;
        }

        public CarnageTextView WithTextSize(ComplexUnitType unit, float size)
        {
            SetTextSize(unit, size);
            return this;
        }

        public CarnageTextView WithColor(Color color)
        {
            SetTextColor(color);
            return this;
        }

        public CarnageTextView AsTitle()
        {
            SetTextColor(CarnageStyle.PrimaryRed);
            TextSize = CarnageStyle.FontSizeLarge;
            Typeface = Typeface.DefaultBold;

            return this;
        }
    }
}
