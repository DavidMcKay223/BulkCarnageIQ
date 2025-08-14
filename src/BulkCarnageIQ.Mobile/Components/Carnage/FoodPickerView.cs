using Android.Content;
using Android.Util;
using BulkCarnageIQ.Core.Carnage;
using BulkCarnageIQ.Infrastructure.Persistence;
using BulkCarnageIQ.Infrastructure.Repositories;
using CarnageAndroid;
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
            Orientation = Orientation.Vertical;
            SetPadding(context.DpToPx(8), context.DpToPx(8), context.DpToPx(8), context.DpToPx(8));

            var servingContainer = new LinearLayout(context)
            {
                Orientation = Orientation.Vertical,
                LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent)
            };

            ServingsSeekBar = new SeekBar(context)
            {
                Max = 20,
                Progress = 2
            };
            servingContainer.AddView(ServingsSeekBar);

            ServingsPreview = new CarnageTextView(context)
                .WithText("Servings: 0")
                .WithStyle(CarnageTextViewStyle.Secondary);
            servingContainer.AddView(ServingsPreview);

            CaloriesPreview = new CarnageTextView(context)
                .WithText("Calories: 0")
                .WithStyle(CarnageTextViewStyle.Secondary);
            servingContainer.AddView(CaloriesPreview);

            AddView(servingContainer);

            ServingsSeekBar.ProgressChanged += (s, e) => UpdateCalories();
        }

        public void UpdateFoodSelection(FoodItem selectedFood)
        {
            _currentFood = selectedFood;
            if (_currentFood != null)
            {
                ServingsSeekBar.Progress = (int)(_currentFood.Servings / StepSize);
                UpdateCalories();
            }
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
