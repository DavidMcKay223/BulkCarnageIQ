using Android.Views;
using BulkCarnageIQ.Core.Carnage;
using BulkCarnageIQ.Infrastructure.Persistence;
using BulkCarnageIQ.Infrastructure.Repositories;
using CarnageAndroid.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Mobile.Components.Pages
{
    public class FoodListFragment : Fragment
    {
        private LinearLayout fixedContentLayout;
        private LinearLayout dynamicContentLayout;

        private UserProfile currentUserProfile;
        private FoodItemService foodItemService;

        public FoodListFragment(AppDbContext db, UserProfile userProfile) : base()
        {
            currentUserProfile = userProfile;
            foodItemService = new FoodItemService(db);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_dynamic, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            fixedContentLayout = view.FindViewById<LinearLayout>(Resource.Id.fixed_content);
            dynamicContentLayout = view.FindViewById<LinearLayout>(Resource.Id.dynamic_content);

            fixedContentLayout.AddView(Context.CarnageTextView("Food List").AsTitle());

            var foodList = foodItemService.GetPagingAsync().Result;

            if (foodList == null || !foodList.Any())
            {
                dynamicContentLayout.AddView(Context.CarnageTextView("No food items found."));
            }
            else
            {
                foreach (var food in foodList)
                {
                    dynamicContentLayout.AddView(new Carnage.FoodDisplayView(Context, food));
                }
            }
        }
    }
}
