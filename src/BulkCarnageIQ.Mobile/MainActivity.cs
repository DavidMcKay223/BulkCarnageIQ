using Android.Animation;
using Android.App;
using Android.Content;
using Android.OS;
using BulkCarnageIQ.Core.Carnage;
using BulkCarnageIQ.Infrastructure.Persistence;
using BulkCarnageIQ.Infrastructure.Repositories;
using BulkCarnageIQ.Mobile.Components.Pages;
using CarnageAndroid;
using CarnageAndroid.UI;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BulkCarnageIQ.Mobile
{
    [Activity(MainLauncher = true, Theme = "@style/AppTheme")]
    public class MainActivity : Activity
    {
        FrameLayout fragmentContainer;
        LinearLayout drawerPanel;

        LinearLayout titleContainer;
        LinearLayout hamburgerContainer;

        UserProfile userProfile;
        AppDbContext dbContext;

        bool isDrawerOpen = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            fragmentContainer = FindViewById<FrameLayout>(Resource.Id.fragment_container);
            drawerPanel = FindViewById<LinearLayout>(Resource.Id.drawer_panel);

            titleContainer = FindViewById<LinearLayout>(Resource.Id.title_container);
            hamburgerContainer = FindViewById<LinearLayout>(Resource.Id.hamburger_container);

            // Inject CarnageTextView title dynamically
            titleContainer.AddView(this.CarnageTextView(GetString(Resource.String.app_name)).AsTitle());

            // Inject Hamburger button dynamically
            hamburgerContainer.AddView(
                this.CarnageButton(GetString(Resource.String.app_btn_hamburger_text),
                    () => ToggleDrawer())
                .WithColor(CarnageStyle.MidnightBlue)
                .WithTextColor(CarnageStyle.OffWhite));

            // Build drawer buttons dynamically
            BuildDrawerMenu();

            InitializeApp();

            userProfile = GetUserProfile();
            dbContext = CreateDbContext();

            if (savedInstanceState == null)
            {
                LoadFragment(new HomeFragment(dbContext, userProfile));
            }
        }

        void BuildDrawerMenu()
        {
            drawerPanel.RemoveAllViews();

            var menuItems = new[]
            {
                new { Text = "Home", CarnageIcon = CarnageIcon.Home, Click = new Action(() => { LoadFragment(new HomeFragment(dbContext, userProfile)); ToggleDrawer(); }) },
                new { Text = "Food Tracker", CarnageIcon = CarnageIcon.FoodTracker, Click = new Action(() => { LoadFragment(new TrackerFragment(dbContext, userProfile)); ToggleDrawer(); }) },
                new { Text = "User Profile", CarnageIcon = CarnageIcon.Profile, Click = new Action(() => { LoadFragment(new UserProfileFragment(dbContext, userProfile)); ToggleDrawer(); }) },
            };

            foreach (var item in menuItems)
            {
                drawerPanel.AddView(this.CarnageButtonIcon(item.CarnageIcon, item.Text, item.Click));
            }
        }

        void ToggleDrawer()
        {
            float start = isDrawerOpen ? 0 : -drawerPanel.Width;
            float end = isDrawerOpen ? -drawerPanel.Width : 0;

            var animator = ObjectAnimator.OfFloat(drawerPanel, "translationX", start, end);
            animator.SetDuration(300);
            animator.Start();

            isDrawerOpen = !isDrawerOpen;
        }

        void LoadFragment(Android.App.Fragment fragment)
        {
            var transaction = FragmentManager.BeginTransaction();
            transaction.Replace(Resource.Id.fragment_container, fragment);
            transaction.Commit();
        }

        public override void OnBackPressed()
        {
            if (isDrawerOpen)
            {
                ToggleDrawer();
            }
            else
            {
                base.OnBackPressed();
            }
        }

        private AppDbContext CreateDbContext()
        {
            var dbPath = System.IO.Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
                "bulk_carnage.db"
            );

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;

            return new AppDbContext(options);
        }

        private UserProfile GetUserProfile()
        {
            using var dbContext = CreateDbContext();
            var userService = new UserProfileService(dbContext);

            return userService.GetUserProfile("").Result;
        }

        private void InitializeApp()
        {
            var dbPath = Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
                "bulk_carnage.db"
            );

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;

            using var dbContext = new AppDbContext(options);

            // Create DB + tables if not exists
            dbContext.Database.EnsureCreated();

            // Optional: Run migrations instead of EnsureCreated for schema changes
            //dbContext.Database.Migrate();

            // Optional: Seed initial data
            if (true)
            {
                string jsonText;

                using (var stream = Resources.OpenRawResource(Resource.Raw.seed_data))
                using (var reader = new StreamReader(stream))
                {
                    jsonText = reader.ReadToEnd();
                }

                var seedItems = JsonSerializer.Deserialize<SeedData>(jsonText);

                if (dbContext.FoodItems.Count() < seedItems.FoodItems.Count())
                {
                    foreach (var item in seedItems.FoodItems)
                    {
                        var existing = dbContext.FoodItems
                            .FirstOrDefault(f => f.RecipeName == item.RecipeName);

                        if (existing != null)
                        {
                            // Update existing item
                            existing.Servings = item.Servings;
                            existing.CaloriesPerServing = item.CaloriesPerServing;
                            existing.MeasurementServings = item.MeasurementServings;
                            existing.MeasurementType = item.MeasurementType;
                            existing.Protein = item.Protein;
                            existing.Carbs = item.Carbs;
                            existing.Fats = item.Fats;
                            existing.Fiber = item.Fiber;
                            existing.GroupName = item.GroupName;
                            existing.IsBreakfast = item.IsBreakfast;
                            existing.IsLunch = item.IsLunch;
                            existing.IsDinner = item.IsDinner;
                            existing.IsSnack = item.IsSnack;
                            existing.Link = item.Link;
                            existing.PictureLink = item.PictureLink;
                        }
                        else
                        {
                            // Insert new item
                            dbContext.FoodItems.Add(item);
                        }
                    }

                    dbContext.SaveChanges();
                }
            }
        }
    }

    public class SeedData
    {
        public List<FoodItem> FoodItems { get; set; }
    }
}
