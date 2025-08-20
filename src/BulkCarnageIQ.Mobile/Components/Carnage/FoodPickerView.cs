using Android.Content;
using Android.Content.Res;
using Android.Util;
using Android.Views;
using BulkCarnageIQ.Core.Carnage;
using BulkCarnageIQ.Infrastructure.Persistence;
using BulkCarnageIQ.Infrastructure.Repositories;
using CarnageAndroid;
using CarnageAndroid.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Mobile.Components.Carnage
{
    public class FoodPickerView : LinearLayout
    {
        public SeekBar ServingsSeekBar { get; private set; }
        public TextView ServingsPreview { get; private set; }
        public TextView CaloriesPreview { get; private set; }
        public float StepSize { get; set; } = 0.5f;
        private FoodItem _currentFood;

        public FoodPickerView(Context context) : base(context)
        {
            Init(context);
        }

        private void Init(Context context)
        {
            Orientation = Android.Widget.Orientation.Vertical;
            SetPadding(context.DpToPx(8), context.DpToPx(8), context.DpToPx(8), context.DpToPx(8));

            var servingContainer = new LinearLayout(context)
            {
                Orientation = Android.Widget.Orientation.Vertical,
                LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent)
            };

            // Top row: Servings (left) + Calories (right)
            var topRow = new LinearLayout(context)
            {
                Orientation = Android.Widget.Orientation.Horizontal,
                LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent)
            };

            ServingsPreview = context.CarnageTextView("Servings: 0");
            CaloriesPreview = context.CarnageTextView("Calories: 0");

            var lpLeft = new LinearLayout.LayoutParams(0, LayoutParams.WrapContent, 1f);
            var lpRight = new LinearLayout.LayoutParams(0, LayoutParams.WrapContent, 1f);

            ServingsPreview.LayoutParameters = lpLeft;
            CaloriesPreview.LayoutParameters = lpRight;
            CaloriesPreview.Gravity = GravityFlags.End;

            topRow.AddView(ServingsPreview);
            topRow.AddView(CaloriesPreview);

            servingContainer.AddView(topRow);

            // SeekBar under row
            ServingsSeekBar = new SeekBar(context)
            {
                Max = 20,
                Progress = 2
            };
            ServingsSeekBar.ProgressTintList = ColorStateList.ValueOf(CarnageAndroid.CarnageStyle.VividBlue);
            ServingsSeekBar.ThumbTintList = ColorStateList.ValueOf(CarnageAndroid.CarnageStyle.VividBlue);

            servingContainer.AddView(ServingsSeekBar);

            AddView(servingContainer);

            ServingsSeekBar.ProgressChanged += (s, e) => UpdateCalories();
        }

        public void UpdateFoodSelection(FoodItem selectedFood)
        {
            _currentFood = selectedFood;
            if (_currentFood != null)
            {
                ServingsSeekBar.Progress = (int)(_currentFood.Servings / StepSize);
            }

            UpdateCalories();
        }

        public float Progress {             
            get => ServingsSeekBar.Progress * StepSize;
            set
            {
                if (value < 0) value = 0;
                if (value > ServingsSeekBar.Max * StepSize) value = ServingsSeekBar.Max * StepSize;
                ServingsSeekBar.Progress = (int)(value / StepSize);
                UpdateCalories();
            }
        }

        private void UpdateCalories()
        {
            if (_currentFood != null)
            {
                float servings = ServingsSeekBar.Progress * StepSize;
                ServingsPreview.Text = $"Servings: {_currentFood.MeasurementServings * servings:N1} {_currentFood.MeasurementType}";
                CaloriesPreview.Text = $"Calories: {_currentFood.CaloriesPerServing * servings:N1}";
            }
            else
            {
                ServingsPreview.Text = "Servings: 0";
                CaloriesPreview.Text = "Calories: 0";
            }
        }
    }
}
