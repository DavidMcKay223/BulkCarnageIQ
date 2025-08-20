using BulkCarnageIQ.Core.Carnage.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Mobile.Components.Carnage.Feed
{
    internal class FoodSuggestionFilterBinder
    {
        private readonly FoodItemFilter _filter;
        private readonly FoodSuggestionAdapter _adapter;

        public FoodSuggestionFilterBinder(FoodItemFilter filter, FoodSuggestionAdapter adapter)
        {
            _filter = filter;
            _adapter = adapter;
        }

        public void BindCheckBox(Android.Widget.CheckBox checkBox, string propertyName)
        {
            var prop = typeof(FoodItemFilter).GetProperty(propertyName);
            if (prop == null) return;

            checkBox.Checked = (bool)prop.GetValue(_filter)!;

            checkBox.CheckedChange += (s, e) =>
            {
                prop.SetValue(_filter, e.IsChecked);
                _adapter.Filter(_filter);
            };
        }
    }
}
