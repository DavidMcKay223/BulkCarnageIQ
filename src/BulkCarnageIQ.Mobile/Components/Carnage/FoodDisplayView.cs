using Android.Content;
using Android.Content.Res;
using Android.Net;
using AndroidX.CardView.Widget;
using BulkCarnageIQ.Core.Carnage;
using CarnageAndroid;
using CarnageAndroid.UI;
using Google.Android.Material.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Android.Icu.Text.CaseMap;

namespace BulkCarnageIQ.Mobile.Components.Carnage
{
    public class FoodDisplayView : MaterialCardView
    {
        private FoodItem _currentFood;

        public FoodDisplayView(Context context, FoodItem foodItem) : base(context)
        {
            _currentFood = foodItem;

            Radius = Context.DpToPx(8);
            CardElevation = Context.DpToPx(4);

            this.WithMargins(0, 0, 0, CarnageStyle.PaddingLarge);

            SetBackgroundColor(CarnageStyle.MidnightBlue);
            SetCardBackgroundColor(CarnageStyle.DarkCharcoal);

            Init(context);
        }

        private void Init(Context context)
        {
            var container = new LinearLayout(context) { Orientation = Android.Widget.Orientation.Vertical };
            container.SetBackgroundColor(CarnageStyle.DarkCharcoal);

            AddView(container);

            container.AddView(context.CarnageTextView(_currentFood.RecipeName).AsTitle().WithMargins(0, CarnageStyle.PaddingSmall, 0, CarnageStyle.PaddingSmall));
            if (!string.IsNullOrEmpty(_currentFood.BrandType)) container.AddView(context.CarnageTextView($"Brand: {_currentFood.BrandType}"));
            if (!string.IsNullOrEmpty(_currentFood.GroupName)) container.AddView(context.CarnageTextView($"Group: {_currentFood.GroupName}"));
            container.AddView(context.CarnageTextView($"Servings: {_currentFood.Servings} {_currentFood.MeasurementType} ({_currentFood.MeasurementServings}x)"));
            container.AddView(context.CarnageTextView($"Calories: {_currentFood.TotalCalories} (per serving {_currentFood.CaloriesPerServing})"));
            container.AddView(context.CarnageTextView($"Protein: {_currentFood.Protein}g"));
            container.AddView(context.CarnageTextView($"Carbs: {_currentFood.Carbs}g"));
            container.AddView(context.CarnageTextView($"Fats: {_currentFood.Fats}g"));
            container.AddView(context.CarnageTextView($"Fiber: {_currentFood.Fiber}g"));
            if (!string.IsNullOrEmpty(_currentFood.Link)) container.AddView(context.CarnageTextView(_currentFood.Link));
        }
    }
}
