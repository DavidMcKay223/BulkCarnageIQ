using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Widget;
using Java.Time.Format;

namespace CarnageAndroid
{
    public class CarnageTextField : EditText
    {
        public CarnageTextField(Context context) : base(context) => Init();
        public CarnageTextField(Context context, IAttributeSet attrs) : base(context, attrs) => Init();
        public CarnageTextField(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr) => Init();

        private void Init()
        {
            SetStyle(CarnageTextFieldStyle.Default);
            BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(CarnageStyle.PrimaryColor);
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

        public CarnageTextField SetStyle(CarnageTextFieldStyle style)
        {
            switch (style)
            {
                case CarnageTextFieldStyle.Filled:
                    SetBackgroundColor(CarnageStyle.SecondaryColor);
                    SetTextColor(CarnageStyle.TextPrimaryColor);
                    break;
                case CarnageTextFieldStyle.Outline:
                    // Apply outline drawable or border here
                    SetBackgroundColor(CarnageStyle.BackgroundColor);
                    SetTextColor(CarnageStyle.TextPrimaryColor);
                    break;
                case CarnageTextFieldStyle.Error:
                    SetBackgroundColor(CarnageStyle.DangerColor);
                    SetTextColor(CarnageStyle.BackgroundColor);
                    break;
                default:
                    SetBackgroundColor(CarnageStyle.BackgroundColor);
                    SetTextColor(CarnageStyle.TextPrimaryColor);
                    break;
            }
            return this;
        }
    }
}
