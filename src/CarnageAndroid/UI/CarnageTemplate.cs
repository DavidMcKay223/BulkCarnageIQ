using Android.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarnageAndroid.UI
{
    public static class CarnageTemplate
    {
        public static CarnageButton CarnageButton(this Context context, CarnageButtonStyle style = CarnageButtonStyle.Default, string text = "", Action? onClick = null)
        {
            var btn = new CarnageButton(context)
                          .WithStyle(style)
                          .WithText(text)
                          .AsPill();

            if (onClick != null)
                btn.OnClick(onClick);

            return btn;
        }

        public static CarnageTextView CarnageTextView(this Context context, CarnageTextViewStyle style = CarnageTextViewStyle.Default, string text = "")
        {
            return new CarnageTextView(context)
                .WithStyle(style)
                .WithText(text);
        }

        public static CarnageTextField CarnageTextField(this Context context, CarnageTextFieldStyle style = CarnageTextFieldStyle.Default, string text = "")
        {
            return new CarnageTextField(context)
                .WithStyle(style)
                .WithText(text);
        }

        public static CarnageLinearProgress CarnageLinearProgress(this Context context, CarnageProgressStyle style = CarnageProgressStyle.Default)
        {
            return new CarnageLinearProgress(context)
                .WithStyle(style);
        }

        public static CarnageCircularProgress CarnageCircularProgress(this Context context, CarnageProgressStyle style = CarnageProgressStyle.Default)
        {
            return new CarnageCircularProgress(context)
                .WithStyle(style);
        }
    }
}
