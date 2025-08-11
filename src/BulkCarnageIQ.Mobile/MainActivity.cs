using Android.Animation;
using Android.App;
using Android.Content;
using Android.OS;
using BulkCarnageIQ.Core.Carnage;
using BulkCarnageIQ.Infrastructure.Persistence;
using BulkCarnageIQ.Mobile.Components.Pages;
using CarnageAndroid;
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

        bool isDrawerOpen = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            fragmentContainer = FindViewById<FrameLayout>(Resource.Id.fragment_container);
            drawerPanel = FindViewById<LinearLayout>(Resource.Id.drawer_panel);

            // These containers must exist in your XML (as in your last posted layout)
            titleContainer = FindViewById<LinearLayout>(Resource.Id.title_container);
            hamburgerContainer = FindViewById<LinearLayout>(Resource.Id.hamburger_container);

            // Inject CarnageTextView title dynamically
            var txtAppTitle = new CarnageTextView(this)
                .WithText(GetString(Resource.String.app_name))
                .AsTitle();
            titleContainer.AddView(txtAppTitle);

            // Inject Hamburger button dynamically
            var btnHamburger = new CarnageButton(this)
                .WithText(GetString(Resource.String.app_btn_hamburger_text))
                .SetStyle(CarnageButtonStyle.Primary)
                .OnClick(() => ToggleDrawer());
            hamburgerContainer.AddView(btnHamburger);

            // Build drawer buttons dynamically
            BuildDrawerMenu();

            InitializeApp();

            if (savedInstanceState == null)
            {
                LoadFragment(new HomeFragment());
            }
        }

        void BuildDrawerMenu()
        {
            drawerPanel.RemoveAllViews();

            var menuItems = new[]
            {
                new { Text = "Home", Click = new Action(() => { LoadFragment(new HomeFragment()); ToggleDrawer(); }) },
                new { Text = "Food Tracker", Click = new Action(() => { LoadFragment(new TrackerFragment()); ToggleDrawer(); }) },
                // Add more menu items here dynamically as needed
            };

            foreach (var item in menuItems)
            {
                var btn = new CarnageButton(this)
                    .WithText(item.Text)
                    .SetStyle(CarnageButtonStyle.Primary)
                    .OnClick(item.Click);

                drawerPanel.AddView(btn);
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
            if (!dbContext.FoodItems.Any())
            {
                string jsonText;

                using (var stream = Resources.OpenRawResource(Resource.Raw.seed_data))
                using (var reader = new StreamReader(stream))
                {
                    jsonText = reader.ReadToEnd();
                }

                var seedItems = JsonSerializer.Deserialize<SeedData>(jsonText);

                dbContext.FoodItems.AddRange(seedItems.FoodItems);
                dbContext.SaveChanges();
            }

            // Optional: call API to sync fresh data
            // SyncDataFromServer(dbContext);
        }
    }

    public class SeedData
    {
        public List<FoodItem> FoodItems { get; set; }
    }
}
