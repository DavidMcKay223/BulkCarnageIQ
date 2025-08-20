using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using BulkCarnageIQ.Core.Carnage;
using BulkCarnageIQ.Core.Carnage.Engine;
using BulkCarnageIQ.Core.Carnage.Report;
using BulkCarnageIQ.Infrastructure.Persistence;
using BulkCarnageIQ.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Mobile.Components.Carnage.Feed
{
    public class FoodSuggestionAdapter : RecyclerView.Adapter
    {
        private readonly Context _context;
        private readonly List<FoodItemSuggestion> _allFoods;
        private List<FoodItemSuggestion> _filteredFoods;
        private readonly UserProfile _currentUserProfile;
        private readonly MacroSummary _macroSummary;

        public FoodSuggestionAdapter(Context context, List<FoodItemSuggestion> foods, MacroSummary macroSummary, UserProfile currentUserProfile)
        {
            _context = context;
            _allFoods = foods;
            _filteredFoods = new List<FoodItemSuggestion>(_allFoods);
            _currentUserProfile = currentUserProfile;
            _macroSummary = macroSummary;
        }

        public override int ItemCount => _filteredFoods.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is FoodSuggestionViewHolder vh) vh.Bind(_filteredFoods[position]);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = new FoodSuggestionView(_context, _macroSummary, _currentUserProfile);
            return new FoodSuggestionViewHolder(view);
        }

        public void Filter(Func<FoodItemSuggestion, bool> predicate)
        {
            _filteredFoods = _allFoods.Where(predicate).ToList();
            NotifyDataSetChanged();
        }

        public void Filter(FoodItemFilter filter)
        {
            _filteredFoods = filter.ApplyOrder(filter.ApplyFilters(_allFoods), _currentUserProfile, _macroSummary).ToList();

            NotifyDataSetChanged();
        }

        public class FoodSuggestionViewHolder : RecyclerView.ViewHolder
        {
            private readonly FoodSuggestionView _view;
            public FoodSuggestionViewHolder(FoodSuggestionView view) : base(view) { _view = view; }
            public void Bind(FoodItemSuggestion item) => _view.Bind(item);
        }
    }
}
