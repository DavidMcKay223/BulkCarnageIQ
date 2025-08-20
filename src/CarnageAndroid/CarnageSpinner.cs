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
            adapter = new CarnageSpinnerAdapter(context, items);
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

    public class CarnageSpinnerAdapter : ArrayAdapter<string>
    {
        public CarnageSpinnerAdapter(Context context, List<string> items)
            : base(context, Android.Resource.Layout.SimpleSpinnerItem, items)
        {
            SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = base.GetView(position, convertView, parent) as TextView;

            view
                .WithTextColor(CarnageStyle.OffWhite);

            return view;
        }

        public override View GetDropDownView(int position, View convertView, ViewGroup parent)
        {
            var view = base.GetDropDownView(position, convertView, parent) as TextView;

            view
                .WithTextColor(CarnageStyle.OffWhite)
                .WithBackgroundColor(CarnageStyle.SlateGray);

            return view;
        }
    }
}
