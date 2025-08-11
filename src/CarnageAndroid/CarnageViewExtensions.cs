using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Util;
using Android.Views;
using static Android.Icu.Text.ListFormatter;

namespace CarnageAndroid
{
    public static class CarnageViewExtensions
    {
        public static int DpToPx(this Context context, float dp)
        {
            float scale = context.Resources.DisplayMetrics.Density;
            return (int)(dp * scale + 0.5f);
        }

        public static T WithPaddingDp<T>(this T view, int dp) where T : View
        {
            var px = view.Context.DpToPx(dp);
            view.SetPadding(px, px, px, px);
            return view;
        }

        public static T WithBackgroundColor<T>(this T view, Color color) where T : View
        {
            view.SetBackgroundColor(color);
            return view;
        }
    }

    public static class CarnageViewFluentExtensions
    {
        public static T WithWidth<T>(this T view, int dpWidth) where T : View
        {
            var pxWidth = view.Context.DpToPx(dpWidth);
            var lp = view.LayoutParameters ?? new ViewGroup.LayoutParams(pxWidth, ViewGroup.LayoutParams.WrapContent);
            lp.Width = pxWidth;
            return view;
        }

        public static T WithHeight<T>(this T view, int dpHeight) where T : View
        {
            var pxHeight = view.Context.DpToPx(dpHeight);
            var lp = view.LayoutParameters ?? new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, pxHeight);
            lp.Height = pxHeight;
            view.LayoutParameters = lp;
            return view;
        }

        public static T WithWeight<T>(this T view, float weight) where T : View
        {
            if (view.LayoutParameters is LinearLayout.LayoutParams lp)
            {
                lp.Weight = weight;
            }
            else
            {
                lp = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent)
                {
                    Weight = weight
                };
                view.LayoutParameters = lp;
            }
            return view;
        }

        public static T WithMargins<T>(this T view, int left, int top, int right, int bottom) where T : View
        {
            if (view.LayoutParameters is ViewGroup.MarginLayoutParams mlp)
            {
                mlp.SetMargins(left, top, right, bottom);
            }
            else
            {
                var lp = new ViewGroup.MarginLayoutParams(view.LayoutParameters ?? new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
                lp.SetMargins(left, top, right, bottom);
                view.LayoutParameters = lp;
            }
            return view;
        }

        public static T WithPadding<T>(this T view, int left, int top, int right, int bottom) where T : View
        {
            view.SetPadding(left, top, right, bottom);
            return view;
        }

        public static T WithBackgroundColor<T>(this T view, Color color) where T : View
        {
            view.SetBackgroundColor(color);
            return view;
        }

        public static T WithVisibility<T>(this T view, ViewStates visibility) where T : View
        {
            view.Visibility = visibility;
            return view;
        }

        public static T WithEnabled<T>(this T view, bool enabled) where T : View
        {
            view.Enabled = enabled;
            return view;
        }

        public static T WithAlpha<T>(this T view, float alpha) where T : View
        {
            view.Alpha = alpha;
            return view;
        }

        public static T WithClickable<T>(this T view, bool clickable) where T : View
        {
            view.Clickable = clickable;
            return view;
        }

        public static T WithFocusable<T>(this T view, bool focusable) where T : View
        {
            view.Focusable = focusable;
            return view;
        }

        public static T WithTranslationX<T>(this T view, float x) where T : View
        {
            view.TranslationX = x;
            return view;
        }

        public static T WithTranslationY<T>(this T view, float y) where T : View
        {
            view.TranslationY = y;
            return view;
        }

        public static T WithRotation<T>(this T view, float degrees) where T : View
        {
            view.Rotation = degrees;
            return view;
        }

        public static T WithScaleX<T>(this T view, float scale) where T : View
        {
            view.ScaleX = scale;
            return view;
        }

        public static T WithScaleY<T>(this T view, float scale) where T : View
        {
            view.ScaleY = scale;
            return view;
        }

        public static T WithElevation<T>(this T view, float dp) where T : View
        {
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
            {
                var density = view.Resources?.DisplayMetrics?.Density ?? 1f;
                view.Elevation = dp * density;
            }
            return view;
        }
    }

    public static class CarnageTextViewFluentExtensions
    {
        public static T WithTextColor<T>(this T view, Color color) where T : TextView
        {
            view.SetTextColor(color);
            return view;
        }

        public static T WithTextSize<T>(this T view, ComplexUnitType unit, float size) where T : TextView
        {
            view.SetTextSize(unit, size);
            return view;
        }
    }
}
