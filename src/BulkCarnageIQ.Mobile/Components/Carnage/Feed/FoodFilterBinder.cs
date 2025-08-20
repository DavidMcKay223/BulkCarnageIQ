using BulkCarnageIQ.Core.Carnage.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Mobile.Components.Carnage.Feed
{
    public class FoodFilterBinder
    {
        private readonly FoodItemFilter _filter;
        private readonly FoodAdapter _adapter;

        public FoodFilterBinder(FoodItemFilter filter, FoodAdapter adapter)
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
