using Android.Content;
using Android.Graphics;
using Android.Util;
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
            var btn = 
                new CarnageButton(context)
                    .WithText(text)
                    .WithTextColor(CarnageStyle.OffWhite)
                    .WithTextSize(ComplexUnitType.Sp, CarnageStyle.FontSizeMedium)
                    .WithPadding(CarnageStyle.PaddingMedium, CarnageStyle.PaddingMedium, CarnageStyle.PaddingMedium, CarnageStyle.PaddingMedium)
                    .AsPill();

            if (onClick != null)
                btn.OnClick(onClick);

            return btn;
        }

        public static CarnageButtonIcon CarnageButtonIcon(this Context context, CarnageIcon icon, string text = "", Action? onClick = null)
        {
            var btn = new CarnageButtonIcon(context)
                .WithPosition(CarnageIconPosition.Left)
                .WithIcon(icon, CarnageStyle.VividBlue)
                .WithText(text, CarnageStyle.OffWhite);

            if (onClick != null)
                btn.OnClick(onClick);

            return btn;
        }

        public static CarnageTextView CarnageTextView(this Context context, string text = "")
        {
            return new CarnageTextView(context)
                .WithText(text)
                .WithTextColor(CarnageStyle.OffWhite);
        }

        public static CarnageTextField CarnageTextField(this Context context, string text = "")
        {
            return
                new CarnageTextField(context)
                    .WithText(text)
                    .WithBackgroundColor(CarnageStyle.SlateGray)
                    .WithTextColor(CarnageStyle.OffWhite)
                    .WithPadding(CarnageStyle.PaddingMedium, CarnageStyle.PaddingMedium, CarnageStyle.PaddingMedium, CarnageStyle.PaddingMedium)
                    .WithMargins(0, CarnageStyle.PaddingSmall, 0, CarnageStyle.PaddingSmall);
        }

        public static CarnageSpinner CarnageSpinner(this Context context, List<string> items, string selected = "")
        {
            return
                new CarnageSpinner(context, items, selected)
                    .WithBackgroundColor(CarnageStyle.SlateGray)
                    .WithPadding(0, CarnageStyle.PaddingMedium, 0, CarnageStyle.PaddingMedium)
                    .WithMargins(0, CarnageStyle.PaddingSmall, 0, CarnageStyle.PaddingSmall);
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
