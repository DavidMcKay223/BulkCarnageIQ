using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using BulkCarnageIQ.Core.Carnage;
using BulkCarnageIQ.Core.Carnage.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Mobile.Components.Carnage.Feed
{
    public class FoodAdapter : RecyclerView.Adapter
    {
        private readonly Context _context;
        private readonly List<FoodItem> _allFoods;
        private List<FoodItem> _filteredFoods;
        private UserProfile _currentUserProfile;

        public FoodAdapter(Context context, List<FoodItem> foods, UserProfile currentUserProfile)
        {
            _context = context;
            _allFoods = foods;
            _filteredFoods = new List<FoodItem>(_allFoods);
            _currentUserProfile = currentUserProfile;
        }

        public override int ItemCount => _filteredFoods.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is FoodViewHolder vh) vh.Bind(_filteredFoods[position]);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = new FoodDisplayView(_context, _currentUserProfile);
            return new FoodViewHolder(view);
        }

        public void Filter(Func<FoodItem, bool> predicate)
        {
            _filteredFoods = _allFoods.Where(predicate).ToList();
            NotifyDataSetChanged();
        }

        public void Filter(FoodItemFilter filter)
        {
            _filteredFoods = filter.ApplyFilters(_allFoods).ToList();
            NotifyDataSetChanged();
        }

        public class FoodViewHolder : RecyclerView.ViewHolder
        {
            private readonly FoodDisplayView _view;
            public FoodViewHolder(FoodDisplayView view) : base(view) { _view = view; }
            public void Bind(FoodItem item) => _view.Bind(item);
        }
    }
}
