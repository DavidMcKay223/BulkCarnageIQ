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
        private LinearLayout resultsLayout;

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
            
            resultsLayout = new LinearLayout(Context)
            {
                Orientation = Orientation.Vertical,
                LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
            };

            fixedContentLayout.AddView(Context.CarnageTextView("User Profile").AsTitle());

            var activityOptions = new List<string> { "sedentary", "lightly active", "moderately active", "very active", "super active" };
            var goalOptions = new List<string> { "maintain", "lose0.5", "lose1", "lose2", "gain0.5", "gain1" };
            var dietOptions = Enum.GetNames(typeof(DietType)).ToList();

            //var usernameField = Context.CarnageTextField(currentUserProfile.UserName);
            var ageField = Context.CarnageTextField(currentUserProfile.Age.ToString());
            var heightField = Context.CarnageTextField(currentUserProfile.HeightInches.ToString());
            var weightField = Context.CarnageTextField(currentUserProfile.WeightPounds.ToString());
            var activitySpinnerField = Context.CarnageSpinner(activityOptions, currentUserProfile.ActivityLevel);
            var goalSpinnerField = Context.CarnageSpinner(goalOptions, currentUserProfile.GoalType);
            var dietTypeSpinnerField = Context.CarnageSpinner(dietOptions, currentUserProfile.DietType.ToString());

            //dynamicContentLayout.AddView(Context.CarnageTextView("Username:"));
            //dynamicContentLayout.AddView(usernameField);

            dynamicContentLayout.AddView(Context.CarnageTextView("Age:"));
            dynamicContentLayout.AddView(ageField);

            dynamicContentLayout.AddView(Context.CarnageTextView("Height (inches):"));
            dynamicContentLayout.AddView(heightField);

            dynamicContentLayout.AddView(Context.CarnageTextView("Weight (pounds):"));
            dynamicContentLayout.AddView(weightField);

            dynamicContentLayout.AddView(Context.CarnageTextView("Activity Level:"));
            dynamicContentLayout.AddView(activitySpinnerField);

            dynamicContentLayout.AddView(Context.CarnageTextView("Goal Type:"));
            dynamicContentLayout.AddView(goalSpinnerField);

            dynamicContentLayout.AddView(Context.CarnageTextView("Diet Type:"));
            dynamicContentLayout.AddView(dietTypeSpinnerField);

            dynamicContentLayout.AddView(Context.CarnageButton("Save", () =>
            {
                //currentUserProfile.UserName = usernameField.Text;
                currentUserProfile.Age = float.TryParse(ageField.Text, out var age) ? age : currentUserProfile.Age;
                currentUserProfile.HeightInches = float.TryParse(heightField.Text, out var h) ? h : currentUserProfile.HeightInches;
                currentUserProfile.WeightPounds = float.TryParse(weightField.Text, out var w) ? w : currentUserProfile.WeightPounds;
                currentUserProfile.ActivityLevel = string.IsNullOrWhiteSpace(activitySpinnerField.Text) ? "sedentary" : activitySpinnerField.Text;
                currentUserProfile.GoalType = string.IsNullOrWhiteSpace(goalSpinnerField.Text) ? "maintain" : goalSpinnerField.Text;
                currentUserProfile.DietType = Enum.TryParse<DietType>(dietTypeSpinnerField.Text, true, out var diet) ? diet : DietType.None;

                currentUserProfile.CalculateGoals();
                userProfileService.SaveUserProfile(currentUserProfile).Wait();

                LoadData();
            }));

            dynamicContentLayout.AddView(resultsLayout);

            LoadData();
        }

        private void LoadData()
        {
            resultsLayout.RemoveAllViews();

            resultsLayout.AddView(Context.CarnageTextView("Daily Targets").AsTitle());
            resultsLayout.AddView(Context.CarnageTextView($"Calories: {currentUserProfile.CalorieGoal:F0} kcal"));
            resultsLayout.AddView(Context.CarnageTextView($"Protein: {currentUserProfile.ProteinGoal:F0} g"));
            resultsLayout.AddView(Context.CarnageTextView($"Carbs: {currentUserProfile.CarbsGoal:F0} g"));
            resultsLayout.AddView(Context.CarnageTextView($"Fats: {currentUserProfile.FatGoal:F0} g"));
            resultsLayout.AddView(Context.CarnageTextView($"Fiber: {currentUserProfile.FiberGoal:F0} g"));
        }
    }
}
