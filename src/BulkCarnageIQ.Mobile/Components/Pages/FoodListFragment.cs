using Android.Views;
using AndroidX.RecyclerView.Widget;
using BulkCarnageIQ.Core.Carnage;
using BulkCarnageIQ.Core.Carnage.Report;
using BulkCarnageIQ.Infrastructure.Persistence;
using BulkCarnageIQ.Infrastructure.Repositories;
using BulkCarnageIQ.Mobile.Components.Carnage.Feed;
using CarnageAndroid;
using CarnageAndroid.UI;
using MikePhil.Charting.Highlight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        private FoodAdapter adapter;
        private FoodItemFilter filter;

        public FoodListFragment(AppDbContext db, UserProfile userProfile) : base()
        {
            currentUserProfile = userProfile;
            foodItemService = new FoodItemService(db);
            filter = new FoodItemFilter();
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

            var foodList = foodItemService.GetAllAsync().Result;

            if (foodList == null || !foodList.Any())
            {
                dynamicContentLayout.AddView(Context.CarnageTextView("No food items found."));
                return;
            }

            // RecyclerView setup
            var recycler = new RecyclerView(Context);
            recycler.SetLayoutManager(new LinearLayoutManager(Context));
            adapter = new FoodAdapter(Context, foodList, currentUserProfile);
            recycler.SetAdapter(adapter);
            dynamicContentLayout.AddView(recycler);

            // Filter binder
            var binder = new FoodFilterBinder(filter, adapter);

            // Add all checkboxes to fixedContentLayout
            AddFilterCheckboxes(fixedContentLayout, binder);
        }

        private void AddFilterCheckboxes(LinearLayout container, FoodFilterBinder binder)
        {
            // Row 1
            var row1 = new LinearLayout(container.Context) { Orientation = Orientation.Horizontal };
            row1.AddView(CreateCheckBox("High Protein", nameof(FoodItemFilter.IsHighProtein), binder));
            row1.AddView(CreateCheckBox("Low Carb", nameof(FoodItemFilter.IsLowCarb), binder));
            row1.AddView(CreateCheckBox("Keto", nameof(FoodItemFilter.IsKeto), binder));
            container.AddView(row1);

            // Row 2
            var row2 = new LinearLayout(container.Context) { Orientation = Orientation.Horizontal };
            row2.AddView(CreateCheckBox("Bulk Meal", nameof(FoodItemFilter.IsBulkMeal), binder));
            row2.AddView(CreateCheckBox("High Carb", nameof(FoodItemFilter.IsHighCarb), binder));
            row2.AddView(CreateCheckBox("High Fiber", nameof(FoodItemFilter.IsHighFiber), binder));
            container.AddView(row2);

            // Row 3
            var row3 = new LinearLayout(container.Context) { Orientation = Orientation.Horizontal };
            row3.AddView(CreateCheckBox("High Fat", nameof(FoodItemFilter.IsHighFat), binder));
            row3.AddView(CreateCheckBox("Balanced Meal", nameof(FoodItemFilter.IsBalancedMeal), binder));
            row3.AddView(CreateCheckBox("Low Protein", nameof(FoodItemFilter.IsLowProtein), binder));
            container.AddView(row3);
        }

        private Android.Widget.CheckBox CreateCheckBox(string text, string propertyName, FoodFilterBinder binder)
        {
            var cb = new Android.Widget.CheckBox(Context) { Text = text };
            cb.SetTextColor(CarnageStyle.OffWhite);
            binder.BindCheckBox(cb, propertyName);

            var lp = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            lp.SetMargins(0, 0, 16, 16);
            cb.LayoutParameters = lp;

            return cb;
        }
    }
}
