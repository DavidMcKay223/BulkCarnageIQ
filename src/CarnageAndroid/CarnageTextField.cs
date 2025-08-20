using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Util;
using Android.Widget;
using Google.Android.Material.TextField;
using Java.Time.Format;
using Kotlin;
using static System.Net.Mime.MediaTypeNames;

namespace CarnageAndroid
{
    public class CarnageTextField : TextInputEditText
    {
        public CarnageTextField(Context context) : base(context) => Init();
        public CarnageTextField(Context context, IAttributeSet attrs) : base(context, attrs) => Init();
        public CarnageTextField(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr) => Init();

        private void Init()
        {
            BackgroundTintList = ColorStateList.ValueOf(CarnageStyle.SlateGray);
            SetHintTextColor(CarnageStyle.OffWhite);
        }

        public CarnageTextField WithHint(string hint)
        {
            Hint = hint;
            return this;
        }

        public CarnageTextField WithText(string text)
        {
            Text = text;
            return this;
        }
    }
}
