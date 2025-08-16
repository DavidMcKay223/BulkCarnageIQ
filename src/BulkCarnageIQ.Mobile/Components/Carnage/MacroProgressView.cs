using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using CarnageAndroid;
using CarnageAndroid.UI;

namespace BulkCarnageIQ.Mobile.Components.Carnage
{
    public class MacroProgressView : LinearLayout
    {
        public MacroProgressView(Context context) : base(context)
        {
            Initialize(context);
        }

        public MacroProgressView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize(context);
        }

        private void Initialize(Context context)
        {
            Orientation = Orientation.Vertical;
            SetPadding(context.DpToPx(8), context.DpToPx(8), context.DpToPx(8), context.DpToPx(8));
        }

        public MacroProgressView Add(string name, float current, float goal, string format = " g")
        {
            var itemView = CreateSingleMacroView(name, current, goal, format);
            AddView(itemView);
            return this;
        }

        private View CreateSingleMacroView(string name, float current, float goal, string format = " g")
        {
            var container = new LinearLayout(Context)
            {
                Orientation = Orientation.Vertical,
                LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent)
            };

            // Top row: name + amounts
            var topRow = new LinearLayout(Context)
            {
                Orientation = Orientation.Vertical,
                LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent)
            };
            topRow.SetPadding(0, 0, 0, Context.DpToPx(4));

            topRow.AddView(Context.CarnageTextView(CarnageTextViewStyle.Title, name));
            topRow.AddView(Context.CarnageTextView(CarnageTextViewStyle.Secondary, $"{current:N1}{format} / {goal:N1}{format}"));

            container.AddView(topRow);

            // Progress bar
            var progressBar = Context.CarnageLinearProgress(CarnageProgressStyle.Primary);

            float ratio = current / goal;
            int progressPercent = (int)(ratio * 100);
            progressPercent = progressPercent > 100 ? 100 : progressPercent;
            progressBar.Progress = progressPercent;

            Color color = GetColorForRatio(ratio);
            progressBar.ProgressDrawable.SetColorFilter(color, PorterDuff.Mode.SrcIn);

            container.AddView(progressBar);

            // Status text
            var statusText = new TextView(Context)
            {
                LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent),
                Text = GetStatusText(ratio, current, goal, format),
                TextSize = 12f
            };
            statusText.SetTextColor(color);
            statusText.SetPadding(0, Context.DpToPx(4), 0, 0);

            container.AddView(statusText);

            return container;
        }

        private Color GetColorForRatio(float ratio)
        {
            if (ratio > 1.0f)
                return new Color(Color.ParseColor("#D32F2F")); // Red
            if (ratio >= 0.85f)
                return new Color(Color.ParseColor("#FBC02D")); // Yellow
            return new Color(Color.ParseColor("#388E3C"));     // Green
        }

        private string GetStatusText(float ratio, float current, float goal, string format = "g")
        {
            if (ratio > 1.0f)
                return $"Over Limit by {(current - goal):F1}{format}";
            if (ratio >= 0.85f)
                return "Getting Close";
            return "On Track";
        }
    }
}
