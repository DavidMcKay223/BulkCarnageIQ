using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Widget;
using MikePhil.Charting.Charts;
using MikePhil.Charting.Components;
using MikePhil.Charting.Data;
using System.Collections.Generic;

namespace BulkCarnageIQ.Mobile.Components.Carnage
{
    public class MacroDonutView : FrameLayout
    {
        private PieChart pieChart;

        public MacroDonutView(Context context) : base(context)
        {
            Init();
        }

        public MacroDonutView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init();
        }

        public MacroDonutView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Init();
        }

        private void Init()
        {
            pieChart = new PieChart(Context);

            var layoutParams = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
            pieChart.LayoutParameters = layoutParams;

            pieChart.Description.Enabled = false;
            pieChart.SetUsePercentValues(true);
            pieChart.SetExtraOffsets(5, 10, 5, 5);
            pieChart.HoleRadius = 75f; // Donut hole size in %
            pieChart.TransparentCircleRadius = 80f;
            pieChart.SetDrawEntryLabels(false); // Hide labels on slices
            pieChart.SetDrawCenterText(false);

            // Legend setup
            var legend = pieChart.Legend;
            legend.VerticalAlignment = Legend.LegendVerticalAlignment.Center;
            legend.HorizontalAlignment = Legend.LegendHorizontalAlignment.Right;
            legend.Orientation = Legend.LegendOrientation.Vertical;
            legend.SetDrawInside(false);
            legend.Enabled = true;

            pieChart.AnimateY(1000);

            AddView(pieChart);
        }

        public void SetMacros(float protein, float carbs, float fats, float fiber)
        {
            var entries = new List<PieEntry>
            {
                new PieEntry(protein, "Protein"),
                new PieEntry(carbs, "Carbs"),
                new PieEntry(fats, "Fats"),
                new PieEntry(fiber, "Fiber")
            };

            var dataSet = new PieDataSet(entries, "");
            dataSet.SliceSpace = 3f;
            dataSet.SelectionShift = 5f;

            // Colors matching your scheme
            dataSet.Colors = new List<Java.Lang.Integer> {
                Java.Lang.Integer.ValueOf(Color.ParseColor("#FF9800")), // Orange
                Java.Lang.Integer.ValueOf(Color.ParseColor("#4CAF50")), // Green
                Java.Lang.Integer.ValueOf(Color.ParseColor("#2196F3")), // Blue
                Java.Lang.Integer.ValueOf(Color.ParseColor("#9C27B0"))  // Purple
            };

            // Show values outside slices
            dataSet.YValuePosition = PieDataSet.ValuePosition.OutsideSlice;
            dataSet.ValueLinePart1Length = 0.5f;
            dataSet.ValueLinePart2Length = 0.5f;
            dataSet.ValueLineColor = Color.Gray;
            dataSet.ValueTextColor = Color.Black;
            dataSet.ValueTextSize = 14f;
            dataSet.SetDrawValues(false);

            var data = new PieData(dataSet);

            // Optional: disable default values on slices (if you want only legend to show grams)
            // data.SetDrawValues(false);

            pieChart.Data = data;

            // Customize legend to show grams
            pieChart.Legend.SetCustom(new List<LegendEntry>
            {
                new LegendEntry { Label = $"Protein {protein}g", FormColor = Color.ParseColor("#FF9800") },
                new LegendEntry { Label = $"Carbs {carbs}g", FormColor = Color.ParseColor("#4CAF50") },
                new LegendEntry { Label = $"Fats {fats}g", FormColor = Color.ParseColor("#2196F3") },
                new LegendEntry { Label = $"Fiber {fiber}g", FormColor = Color.ParseColor("#9C27B0") },
            });

            pieChart.Invalidate(); // Refresh chart
        }
    }
}
