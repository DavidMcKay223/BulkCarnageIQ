using Android.Views;
using BulkCarnageIQ.Core.Carnage;
using BulkCarnageIQ.Infrastructure.Persistence;
using BulkCarnageIQ.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarnageAndroid.UI;
using BulkCarnageIQ.Core.Carnage.Enums;

namespace BulkCarnageIQ.Mobile.Components.Pages
{
    public class UserProfileFragment : Fragment
    {
        private LinearLayout fixedContentLayout;
        private LinearLayout dynamicContentLayout;

        private UserProfile currentUserProfile;
        private UserProfileService userProfileService;

        public UserProfileFragment(AppDbContext db, UserProfile userProfile) : base()
        {
            currentUserProfile = userProfile;
            userProfileService = new UserProfileService(db);
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

            fixedContentLayout.AddView(Context.CarnageTextView("User Profile").AsTitle());

            //var usernameField = Context.CarnageTextField(currentUserProfile.UserName);
            var ageField = Context.CarnageTextField(currentUserProfile.Age.ToString());
            var heightField = Context.CarnageTextField(currentUserProfile.HeightInches.ToString());
            var weightField = Context.CarnageTextField(currentUserProfile.WeightPounds.ToString());
            var activityField = Context.CarnageTextField(currentUserProfile.ActivityLevel);
            var goalField = Context.CarnageTextField(currentUserProfile.GoalType);
            var dietTypeField = Context.CarnageTextField(currentUserProfile.DietType.ToString());

            //fixedContentLayout.AddView(Context.CarnageTextView("Username:"));
            //fixedContentLayout.AddView(usernameField);

            fixedContentLayout.AddView(Context.CarnageTextView("Age:"));
            fixedContentLayout.AddView(ageField);

            fixedContentLayout.AddView(Context.CarnageTextView("Height (inches):"));
            fixedContentLayout.AddView(heightField);

            fixedContentLayout.AddView(Context.CarnageTextView("Weight (pounds):"));
            fixedContentLayout.AddView(weightField);

            fixedContentLayout.AddView(Context.CarnageTextView("Activity Level:"));
            fixedContentLayout.AddView(activityField);

            fixedContentLayout.AddView(Context.CarnageTextView("Goal Type:"));
            fixedContentLayout.AddView(goalField);

            fixedContentLayout.AddView(Context.CarnageTextView("Diet Type:"));
            fixedContentLayout.AddView(dietTypeField);

            fixedContentLayout.AddView(Context.CarnageButton("Save", () =>
            {
                //currentUserProfile.UserName = usernameField.Text;
                currentUserProfile.Age = float.TryParse(ageField.Text, out var age) ? age : currentUserProfile.Age;
                currentUserProfile.HeightInches = float.TryParse(heightField.Text, out var h) ? h : currentUserProfile.HeightInches;
                currentUserProfile.WeightPounds = float.TryParse(weightField.Text, out var w) ? w : currentUserProfile.WeightPounds;
                currentUserProfile.ActivityLevel = string.IsNullOrWhiteSpace(activityField.Text) ? "sedentary" : activityField.Text;
                currentUserProfile.GoalType = string.IsNullOrWhiteSpace(goalField.Text) ? "maintain" : goalField.Text;
                currentUserProfile.DietType = Enum.TryParse<DietType>(dietTypeField.Text, true, out var diet) ? diet : DietType.None;

                currentUserProfile.CalculateGoals();
                userProfileService.SaveUserProfile(currentUserProfile).Wait();

                LoadData();
            }));

            LoadData();
        }

        private void LoadData()
        {
            dynamicContentLayout.RemoveAllViews();

            dynamicContentLayout.AddView(Context.CarnageTextView("Daily Targets").AsTitle());
            dynamicContentLayout.AddView(Context.CarnageTextView($"Calories: {currentUserProfile.CalorieGoal:F0} kcal"));
            dynamicContentLayout.AddView(Context.CarnageTextView($"Protein: {currentUserProfile.ProteinGoal:F0} g"));
            dynamicContentLayout.AddView(Context.CarnageTextView($"Carbs: {currentUserProfile.CarbsGoal:F0} g"));
            dynamicContentLayout.AddView(Context.CarnageTextView($"Fats: {currentUserProfile.FatGoal:F0} g"));
            dynamicContentLayout.AddView(Context.CarnageTextView($"Fiber: {currentUserProfile.FiberGoal:F0} g"));
        }
    }
}
