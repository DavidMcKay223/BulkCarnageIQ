using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using CarnageAndroid;
using CarnageAndroid.UI;
using Google.Android.Material.Card;

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
            // Create a vertical LinearLayout to hold the inner content (grid, progress bar, status text).
            // This ensures they are stacked correctly.
            var verticalLayout = new LinearLayout(Context)
            {
                Orientation = Orientation.Vertical,
                LayoutParameters = new LinearLayout.LayoutParams(
                    LinearLayout.LayoutParams.MatchParent,
                    LinearLayout.LayoutParams.WrapContent
                )
            };
            // Add padding to the vertical layout for a clean appearance.
            verticalLayout.SetPadding(Context.DpToPx(16), Context.DpToPx(12), Context.DpToPx(16), Context.DpToPx(12));

            // Grid inside the vertical layout for the first row of text.
            var grid = new GridLayout(Context)
            {
                ColumnCount = 2,
                LayoutParameters = new GridLayout.LayoutParams(
                    new ViewGroup.LayoutParams(
                        ViewGroup.LayoutParams.MatchParent,
                        ViewGroup.LayoutParams.WrapContent
                    )
                )
            };

            // Left text (takes the first column and expands using weight)
            var leftText = Context.CarnageTextView(name).AsTitle();
            var leftParams = new GridLayout.LayoutParams(
                GridLayout.InvokeSpec(0, GridLayout.Fill),
                GridLayout.InvokeSpec(0, GridLayout.Center)
            )
            {
                // Setting width to 0 and weight to 1f makes it span the available space.
                Width = 0,
                Height = ViewGroup.LayoutParams.WrapContent,
                ColumnSpec = GridLayout.InvokeSpec(0, 1, 1f)
            };
            leftText.LayoutParameters = leftParams;

            // Right text (second column, wraps normally)
            var rightText = Context.CarnageTextView($"{current:N1}{format} / {goal:N1}{format}");
            var rightParams = new GridLayout.LayoutParams(
                GridLayout.InvokeSpec(0, GridLayout.Center),
                GridLayout.InvokeSpec(1, GridLayout.Center)
            )
            {
                Width = ViewGroup.LayoutParams.WrapContent,
                Height = ViewGroup.LayoutParams.WrapContent
            };
            rightText.LayoutParameters = rightParams;

            // Add both text views to the grid.
            grid.AddView(leftText);
            grid.AddView(rightText);

            // Add the grid to our vertical layout.
            verticalLayout.AddView(grid);

            // Progress bar
            var progressBar = Context.CarnageLinearProgress();
            float ratio = current / goal;
            int progressPercent = (int)(ratio * 100);
            progressPercent = progressPercent > 100 ? 100 : progressPercent;
            progressBar.Progress = progressPercent;

            // Add the progress bar to the vertical layout.
            verticalLayout.AddView(progressBar);

            // Status text
            var statusText = Context.CarnageTextView(GetStatusText(ratio, current, goal, format))
                .WithColor(CarnageStyle.White);

            // Add the status text to the vertical layout.
            verticalLayout.AddView(statusText);

            return verticalLayout;
        }

        private string GetStatusText(float ratio, float current, float goal, string format = "g")
        {
            if (ratio > 1.0f)
                return $"Over Limit by {(current - goal):N1}{format}";
            if (ratio >= 0.85f)
                return "Getting Close";
            return "On Track";
        }
    }
}
