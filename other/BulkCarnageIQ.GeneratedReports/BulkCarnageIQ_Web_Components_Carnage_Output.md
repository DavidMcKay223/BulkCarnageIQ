# Directory: Components\Carnage

## File: CalorieBarChart.razor

```C#
@using Microsoft.JSInterop
@using Microsoft.Extensions.Hosting
@inject IJSRuntime JSRuntime

<canvas id="calorieChart"></canvas>

@code {
    [Parameter] public List<string>? Labels { get; set; }
    [Parameter] public List<int>? Data { get; set; }
    [Parameter] public string? ChartLabel { get; set; }

    public async Task InitializeChart()
    {
        var chartData = new
        {
            labels = Labels,
            values = Data,
            datasetLabel = ChartLabel
        };

        await JSRuntime.InvokeVoidAsync("renderBarChart", "calorieChart", chartData);
    }
}

```

## File: GroupMealQuickAdd.razor

```C#
@using BulkCarnageIQ.Core.Carnage

<div class="card shadow-sm rounded p-3 mb-4">
    <h5 class="mb-3">
        <i class="bi bi-lightning-charge-fill text-warning"></i>
        Quick Add Grouped Meals
    </h5>

    @if (GroupItems?.Any() == true)
    {
        <div class="row g-3">
            @foreach (var group in GroupItems)
            {
                <div class="col-md-3">
                    <div class="card h-100 shadow-sm">
                        <div class="card-body d-flex flex-column">
                            <h6 class="card-title text-primary">@group.GroupName</h6>
                            <div class="flex-grow-1 overflow-auto" style="max-height: 300px; min-height: 100px;">
                                @foreach (var entry in group.Entries)
                                {
                                    <div class="form-check">
                                        <input class="form-check-input"
                                               type="checkbox"
                                               id="entry_@entry.Id"
                                               @onchange="(e) => ToggleSelection(entry, (bool)e.Value!)"
                                               checked="@SelectedItems.Contains(entry)" />
                                        <label class="form-check-label" for="entry_@entry.Id">
                                            @entry.RecipeName
                                        </label>
                                    </div>
                                }
                            </div>
                        </div>
                    </div>
                </div>
            }
        </div>

        <button class="btn btn-success mt-3" @onclick="AddSelectedItems">
            <i class="bi bi-plus-circle"></i> Add Selected Items (@SelectedItems.Count)
        </button>
    }
    else
    {
        <div class="text-muted">No group meals available.</div>
    }
</div>

@code {
    [Parameter] public List<GroupFoodItem> GroupItems { get; set; } = new();
    [Parameter] public EventCallback<List<GroupFoodItemEntry>> OnMealsAdded { get; set; }

    private HashSet<GroupFoodItemEntry> SelectedItems { get; set; } = new();

    private void ToggleSelection(GroupFoodItemEntry entry, bool isSelected)
    {
        if (isSelected)
            SelectedItems.Add(entry);
        else
            SelectedItems.Remove(entry);
    }

    private async Task AddSelectedItems()
    {
        await OnMealsAdded.InvokeAsync(SelectedItems.ToList());
        SelectedItems.Clear();
    }
}

```

## File: MacroAlertPanel.razor

```C#
@using BulkCarnageIQ.Core.Carnage
@using BulkCarnageIQ.Core.Carnage.Report

@if (DailyMacros != null)
{
    <div class="row row-cols-1 row-cols-sm-2 row-cols-lg-4 g-4">
        @foreach (var macro in GetMacros())
        {
            <div class="col">
                <div class="p-4 border rounded-xl shadow-md bg-white">
                    <div class="d-flex justify-content-between font-weight-bold">
                        <span>@macro.Name</span>
                        <span>@macro.Current.ToString("N2") g / @macro.Goal.ToString("N2") g</span>
                    </div>
                    <div class="w-100 h-3 bg-secondary rounded-3 overflow-hidden mt-2">
                        <div class="@GetBarColor(macro.Current, macro.Goal) h-100" style="width:@GetBarWidth(macro.Current, macro.Goal)"></div>
                    </div>
                    <p class="text-muted mt-1">@GetStatus(macro.Current, macro.Goal)</p>
                </div>
            </div>
        }
    </div>
}

@code {
    [Parameter] public MacroSummary? DailyMacros { get; set; }
    [Parameter] public UserProfile? UserProfile { get; set; }

    // Method to return the macro values
    private IEnumerable<(string Name, float Current, float Goal)> GetMacros()
    {
        if (DailyMacros == null || UserProfile == null) return [];

        float SafeGoal(float? value, float fallback) =>
            (value.HasValue && value.Value > 0) ? value.Value : fallback;

        return new[]
        {
            ("Protein", DailyMacros.Protein, SafeGoal(UserProfile.ProteinGoal, 120f)),
            ("Carbs", DailyMacros.Carbs, SafeGoal(UserProfile.CarbsGoal, 220f)),
            ("Fats", DailyMacros.Fats, SafeGoal(UserProfile.FatGoal, 80f)),
            ("Fiber", DailyMacros.Fiber, SafeGoal(UserProfile.FiberGoal, 25f))
        };
    }

    private string GetStatus(float current, float goal)
    {
        var ratio = current / goal;
        if (ratio > 1.0) return $"🔴 Over Limit by {current - goal:F1}g";
        if (ratio >= 0.85) return "🟡 Getting Close";
        return "🟢 On Track";
    }

    private string GetBarColor(float current, float goal)
    {
        var ratio = current / goal;
        if (ratio > 1.0) return "bg-red-500";
        if (ratio >= 0.85) return "bg-yellow-400";
        return "bg-green-500";
    }

    private string GetBarWidth(float current, float goal)
    {
        var width = Math.Min(current / goal * 100, 100);
        return $"{width}%";
    }
}

```

## File: WeeklyMacroHeatmap.razor

```C#
@using BulkCarnageIQ.Core.Carnage.Report

<div class="table-responsive">
    <table class="table table-bordered text-center align-middle">
        <thead class="table-light">
            <tr>
                <th>Day</th>
                @foreach (var macro in MacroNames)
                {
                    <th>@macro</th>
                }
            </tr>
        </thead>
        <tbody>
            @if (WeeklyData != null)
            {
                foreach (var day in WeeklyData)
                {
                    <tr>
                        <td class="fw-bold">@day.Key</td>
                        @foreach (var macro in MacroNames)
                        {
                            var value = GetMacroValue(day.Value, macro);
                            var goal = GetGoal(macro);
                            var css = GetCellClass(value, goal);
                            <td class="@css">@value.ToString("N2") g / @goal.ToString("N2") g</td>
                        }
                    </tr>
                }
            }
        </tbody>
    </table>
</div>

@code {
    [Parameter] public Dictionary<string, MacroSummary> WeeklyData { get; set; } = new();
    [Parameter] public float ProteinGoal { get; set; } = 120f;
    [Parameter] public float CarbsGoal { get; set; } = 220f;
    [Parameter] public float FatsGoal { get; set; } = 80f;
    [Parameter] public float FiberGoal { get; set; } = 25f;

    private readonly string[] MacroNames = ["Protein", "Carbs", "Fats", "Fiber"];

    private float GetMacroValue(MacroSummary summary, string macro) => macro switch
    {
        "Protein" => summary.Protein,
        "Carbs" => summary.Carbs,
        "Fats" => summary.Fats,
        "Fiber" => summary.Fiber,
        _ => 0f
    };

    private float GetGoal(string macro) => macro switch
    {
        "Protein" => ProteinGoal,
        "Carbs" => CarbsGoal,
        "Fats" => FatsGoal,
        "Fiber" => FiberGoal,
        _ => 0f
    };

    private string GetCellClass(float value, float goal)
    {
        var ratio = value / goal;
        if (ratio > 1.0) return "bg-danger text-white";
        if (ratio >= 0.85f) return "bg-warning";
        return "bg-success text-white";
    }
}

```

