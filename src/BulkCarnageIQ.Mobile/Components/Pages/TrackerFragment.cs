using Android.Views;

namespace BulkCarnageIQ.Mobile.Components.Pages
{
    public class TrackerFragment : Fragment
    {
        private TableLayout tableMeals;
        private Button btnAddMeal;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Inflate the fragment layout
            return inflater.Inflate(Resource.Layout.fragment_tracker, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            tableMeals = view.FindViewById<TableLayout>(Resource.Id.tableMeals);
            btnAddMeal = view.FindViewById<Button>(Resource.Id.btnAddMeal);

            btnAddMeal.Click += (s, e) => AddMeal("Omelette", 1, 350, 30, 5, 2, 1);

            // Load meals from DB/service
            var meals = LoadMealsFromDb();
            foreach (var meal in meals)
                AddMeal(meal.Name, meal.Portions, meal.Calories, meal.Protein, meal.Carbs, meal.Fats, meal.Fiber);
        }

        private void AddMeal(string name, int portions, int calories, int protein, int carbs, int fats, int fiber)
        {
            var row = new TableRow(Context);

            row.AddView(new TextView(Context) { Text = name });
            row.AddView(new TextView(Context) { Text = portions.ToString() });
            row.AddView(new TextView(Context) { Text = calories.ToString() });
            row.AddView(new TextView(Context) { Text = protein.ToString() });

            var deleteBtn = new Button(Context) { Text = "Delete" };
            deleteBtn.Click += (s, e) => tableMeals.RemoveView(row);

            row.AddView(deleteBtn);

            tableMeals.AddView(row);
        }

        private List<Meal> LoadMealsFromDb()
        {
            // Example: Replace with SQLite or REST API call
            return new List<Meal>
            {
                new Meal("Chicken Salad", 1, 400, 35, 10, 15, 5),
                new Meal("Protein Shake", 1, 200, 25, 5, 3, 2)
            };
        }
    }

    public record Meal(string Name, int Portions, int Calories, int Protein, int Carbs, int Fats, int Fiber);
}
