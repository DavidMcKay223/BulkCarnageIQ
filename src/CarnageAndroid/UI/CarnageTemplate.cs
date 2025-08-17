using Android.Content;
using Android.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarnageAndroid.UI
{
    public static class CarnageTemplate
    {
        public static CarnageButton CarnageButton(this Context context, string text = "", Action? onClick = null)
        {
            var btn = new CarnageButton(context)
                          .WithText(text)
                          .AsPill();

            if (onClick != null)
                btn.OnClick(onClick);

            return btn;
        }

        public static CarnageButtonIcon CarnageButtonIcon(this Context context, CarnageIcon icon, string text = "", Action? onClick = null)
        {
            var btn = new CarnageButtonIcon(context)
                .WithPosition(CarnageIconPosition.Left)
                .WithIcon(icon, CarnageStyle.White)
                .WithText(text, CarnageStyle.White);

            if (onClick != null)
                btn.OnClick(onClick);

            return btn;
        }

        public static CarnageTextView CarnageTextView(this Context context, string text = "")
        {
            return new CarnageTextView(context)
                .WithText(text);
        }

        public static CarnageTextField CarnageTextField(this Context context, string text = "")
        {
            return new CarnageTextField(context)
                .WithText(text);
        }

        public static CarnageLinearProgress CarnageLinearProgress(this Context context)
        {
            return new CarnageLinearProgress(context);
        }

        public static CarnageCircularProgress CarnageCircularProgress(this Context context)
        {
            return new CarnageCircularProgress(context);
        }
    }
}
