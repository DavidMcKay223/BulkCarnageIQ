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
            var verticalLayout = new LinearLayout(Context)
            {
                Orientation = Orientation.Vertical,
                LayoutParameters = new LinearLayout.LayoutParams(
                    LinearLayout.LayoutParams.MatchParent,
                    LinearLayout.LayoutParams.WrapContent
                )
            };
            verticalLayout.SetPadding(Context.DpToPx(16), Context.DpToPx(12), Context.DpToPx(16), Context.DpToPx(12));

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

            var leftText = Context.CarnageTextView(name).AsTitle();
            var leftParams = new GridLayout.LayoutParams(
                GridLayout.InvokeSpec(0, GridLayout.Fill),
                GridLayout.InvokeSpec(0, GridLayout.Center)
            )
            {
                Width = 0,
                Height = ViewGroup.LayoutParams.WrapContent,
                ColumnSpec = GridLayout.InvokeSpec(0, 1, 1f)
            };
            leftText.LayoutParameters = leftParams;

            var displayGoal = goal;

            float safeGoal = goal > 0 ? goal : 1f;

            var rightText = Context.CarnageTextView($"{current:N1}{format} / {displayGoal:N1}{format}");
            var rightParams = new GridLayout.LayoutParams(
                GridLayout.InvokeSpec(0, GridLayout.Center),
                GridLayout.InvokeSpec(1, GridLayout.Center)
            )
            {
                Width = ViewGroup.LayoutParams.WrapContent,
                Height = ViewGroup.LayoutParams.WrapContent
            };
            rightText.LayoutParameters = rightParams;

            grid.AddView(leftText);
            grid.AddView(rightText);
            verticalLayout.AddView(grid);

            var progressBar = Context.CarnageLinearProgress();

            float ratio = current / safeGoal;
            int progressPercent = (int)(ratio * 100);
            progressPercent = Math.Clamp(progressPercent, 0, 100);

            progressBar.Progress = progressPercent;
            verticalLayout.AddView(progressBar);

            var statusText = Context.CarnageTextView(GetStatusText(current, goal, format))
                .WithColor(CarnageStyle.OffWhite);
            verticalLayout.AddView(statusText);

            return verticalLayout;
        }

        private string GetStatusText(float current, float goal, string format = "g")
        {
            float remaining = goal - current;

            if (remaining < 0)
                return $"Over Limit by {Math.Abs(remaining):N1}{format}";
            if (current / (goal > 0 ? goal : 1f) >= 0.85f)
                return "Getting Close";
            return "On Track";
        }
    }
}
