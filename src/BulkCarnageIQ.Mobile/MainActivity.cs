using Android.Animation;
using Android.App;
using Android.OS;
using BulkCarnageIQ.Mobile.Components.Pages;

namespace BulkCarnageIQ.Mobile
{
    [Activity(MainLauncher = true, Theme = "@style/AppTheme")]
    public class MainActivity : Activity
    {
        FrameLayout fragmentContainer;
        LinearLayout drawerPanel;
        Button btnHamburger;
        Button btnHome;
        Button btnTracker;
        bool isDrawerOpen = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            fragmentContainer = FindViewById<FrameLayout>(Resource.Id.fragment_container);
            drawerPanel = FindViewById<LinearLayout>(Resource.Id.drawer_panel);
            btnHamburger = FindViewById<Button>(Resource.Id.btn_hamburger);
            btnHome = FindViewById<Button>(Resource.Id.btn_home);
            btnTracker = FindViewById<Button>(Resource.Id.btn_tracker);

            btnHamburger.Click += (s, e) =>
            {
                ToggleDrawer();
            };

            btnHome.Click += (s, e) =>
            {
                LoadFragment(new HomeFragment());
                ToggleDrawer();
            };

            btnTracker.Click += (s, e) =>
            {
                LoadFragment(new TrackerFragment());
                ToggleDrawer();
            };

            if (savedInstanceState == null)
            {
                LoadFragment(new HomeFragment());
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
    }
}
