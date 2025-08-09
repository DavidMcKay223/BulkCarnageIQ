using Android.Views;

namespace BulkCarnageIQ.Mobile.Components.Pages
{
    public class TrackerFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_tracker, container, false);

            return view;
        }
    }
}
