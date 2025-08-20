using Android.Content;
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
            Initialize();
        }

        public MacroProgressView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize();
        }

        private void Initialize()
        {
            Orientation = Orientation.Vertical;
        }

        public MacroProgressView Add(string name, float current, float goal, string format = " g")
        {
            AddView(CreateMacroRow(name, current, goal, format));
            return this;
        }

        private View CreateMacroRow(string name, float current, float goal, string format)
        {
            var container = new LinearLayout(Context)
            {
                Orientation = Orientation.Vertical,
                LayoutParameters = new LinearLayout.LayoutParams(
                    LayoutParams.MatchParent, LayoutParams.WrapContent)
            };

            // Row: name on left, progress text on right
            var row = new LinearLayout(Context)
            {
                Orientation = Orientation.Horizontal,
                LayoutParameters = new LinearLayout.LayoutParams(
                    LayoutParams.MatchParent, LayoutParams.WrapContent)
            };

            var nameText = Context.CarnageTextView(name).AsTitle();
            var progressText = Context.CarnageTextView($"{current:N1}{format} / {goal:N1}{format}");

            var lpName = new LinearLayout.LayoutParams(0, LayoutParams.WrapContent, 1f);
            var lpProgress = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);

            row.AddView(nameText, lpName);
            row.AddView(progressText, lpProgress);

            container.AddView(row);

            // Progress bar
            var progressBar = Context.CarnageLinearProgress();
            float safeGoal = goal > 0 ? goal : 1f;
            int progress = (int)Math.Clamp((current / safeGoal) * 100, 0, 100);
            progressBar.Progress = progress;
            container.AddView(progressBar);

            // Status text
            var statusText = Context.CarnageTextView(GetStatusText(current, goal, format))
                .WithColor(CarnageStyle.OffWhite);
            container.AddView(statusText);

            return container;
        }

        private string GetStatusText(float current, float goal, string format)
        {
            float remaining = goal - current;

            if (remaining < 0) return $"Over Limit by {Math.Abs(remaining):N1}{format}";
            if (current / (goal > 0 ? goal : 1f) >= 0.85f) return "Getting Close";
            return "On Track";
        }
    }
}
