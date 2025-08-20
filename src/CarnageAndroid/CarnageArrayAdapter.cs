using Android.Content;
using Android.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarnageAndroid
{
    public class CarnageArrayAdapter : ArrayAdapter<string>
    {
        public CarnageArrayAdapter(Context context, List<string> items, int textViewResourceId = Android.Resource.Layout.SimpleSpinnerItem, int dropDownViewResourceId = Android.Resource.Layout.SimpleSpinnerDropDownItem)
            : base(context, textViewResourceId, items)
        {
            SetDropDownViewResource(dropDownViewResourceId);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = base.GetView(position, convertView, parent) as TextView;

            view
                .WithTextColor(CarnageStyle.OffWhite)
                .WithBackgroundColor(CarnageStyle.SlateGray);

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
