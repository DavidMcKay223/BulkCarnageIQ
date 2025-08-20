using Android.Content;
using Android.Content.Res;
using Android.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarnageAndroid
{
    public class CarnageSpinner : Spinner
    {
        private readonly ArrayAdapter<string> adapter;

        public CarnageSpinner(Context context, List<string> items, string selected = "")
            : base(context)
        {
            adapter = new CarnageArrayAdapter(context, items);
            Adapter = adapter;

            if (!string.IsNullOrEmpty(selected))
            {
                var index = adapter.GetPosition(selected);
                if (index >= 0) SetSelection(index);
            }

            DropDownWidth = LayoutParams.MatchParent;
        }

        public string Text { get { return SelectedItem?.ToString() ?? ""; } }
    }
}
