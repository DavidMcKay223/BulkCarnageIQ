using Android.App;
using Android.OS;
using Android.Views;

namespace BulkCarnageIQ.Mobile.Components.Pages
{
    public class HomeFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Inflate your fragment layout
            return inflater.Inflate(Resource.Layout.fragment_home, container, false);
        }
    }
}
