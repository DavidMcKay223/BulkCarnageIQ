using Android.Graphics;
using Android.Views;
using BulkCarnageIQ.Core.Carnage;
using BulkCarnageIQ.Core.Contracts;
using BulkCarnageIQ.Infrastructure.Persistence;
using BulkCarnageIQ.Infrastructure.Repositories;
using CarnageAndroid;
using CarnageAndroid.UI;
using MikePhil.Charting.Charts;
using MikePhil.Charting.Components;
using MikePhil.Charting.Data;
using MikePhil.Charting.Formatter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Mobile.Components.Pages
{
    public class WeightTrackerFragment : Fragment
    {
        private LinearLayout fixedContentLayout;
        private LinearLayout dynamicContentLayout;
        private LineChart chart;

        private UserProfile currentUserProfile;
        private WeightLogService weightLogService;

        public WeightTrackerFragment(AppDbContext db, UserProfile userProfile) : base()
        {
            currentUserProfile = userProfile;
            weightLogService = new WeightLogService(db);
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

            fixedContentLayout.AddView(Context.CarnageTextView("Weight Tracker").AsTitle());

            chart = new LineChart(Context)
            {
                LayoutParameters = new LinearLayout.LayoutParams(
                    ViewGroup.LayoutParams.MatchParent,
                    Context.DpToPx(300))
            };

            var weightInput = Context.CarnageTextField()
                .WithHint("Enter weight (lbs)");

            var saveButton = Context.CarnageButton("Save Weight");

            CarnageButton btnDate = null;

            btnDate = Context.CarnageButton(DateTime.Today.ToString("MM/dd/yyyy"))
                .OnClick(() =>
                {
                    DateOnly dateOnly = DateOnly.FromDateTime(DateTime.Today);
                    if (DateOnly.TryParse(btnDate.Text, out DateOnly selectedDate))
                    {
                        dateOnly = selectedDate;
                    }

                    var dpd = new DatePickerDialog(Context, (sender, e) =>
                    {
                        btnDate.Text = e.Date.ToString("MM/dd/yyyy");
                    }, dateOnly.Year, dateOnly.Month - 1, dateOnly.Day);

                    dpd.Show();
                });

            saveButton
                .OnClick(() =>
                {
                    if (DateOnly.TryParse(btnDate.Text, out DateOnly date) && float.TryParse(weightInput.Text, out float weight))
                    {
                        weightLogService.AddOrUpdateLogAsync(currentUserProfile.UserName, date, weight).Wait();
                        LoadChart(chart);
                        weightInput.Text = "";
                    }
                });

            dynamicContentLayout.AddView(btnDate);
            dynamicContentLayout.AddView(weightInput);
            dynamicContentLayout.AddView(saveButton);
            dynamicContentLayout.AddView(chart);

            LoadChart(chart);
        }

        private void LoadChart(LineChart chart)
        {
            var weightEntries = weightLogService.GetUserLogsAsync(currentUserProfile.UserName, true).Result;

            var entries = new List<Entry>();
            var dates = new List<DateOnly>();
            int index = 0;

            foreach (var w in weightEntries)
            {
                entries.Add(new Entry(index, (float)w.WeightLbs));
                dates.Add(w.Date);
                index++;
            }

            var dataSet = new LineDataSet(entries, "Weight (lbs)")
            {
                Color = CarnageStyle.VividBlue,
                ValueTextColor = CarnageStyle.OffWhite,
                LineWidth = 2f,
                CircleRadius = 4f
            };
            dataSet.SetCircleColor(CarnageStyle.OffWhite);

            var data = new LineData(dataSet);
            chart.Data = data;

            chart.Description.Enabled = false;
            chart.SetBackgroundColor(CarnageStyle.MidnightBlue);
            chart.Legend.TextColor = CarnageStyle.OffWhite;

            chart.XAxis.TextColor = CarnageStyle.OffWhite;
            chart.XAxis.Position = XAxis.XAxisPosition.Bottom;
            chart.XAxis.Granularity = 1f;
            chart.XAxis.ValueFormatter = new IndexAxisValueFormatter(
                dates.Select(d => d.ToString("MM/dd/yyyy")).ToList()
            );

            chart.AxisLeft.TextColor = CarnageStyle.OffWhite;
            chart.AxisRight.Enabled = false;

            chart.Invalidate();
        }
    }
}
