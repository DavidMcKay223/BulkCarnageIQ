# Directory: Components\Pages

## File: Auth.razor

```C#
@page "/auth"

@using Microsoft.AspNetCore.Authorization

@attribute [Authorize]

<PageTitle>Auth</PageTitle>

<h1>You are authenticated</h1>

<AuthorizeView>
    Hello @context.User.Identity?.Name!
</AuthorizeView>
```

## File: Error.razor

```C#
@page "/Error"
@using System.Diagnostics

<PageTitle>Error</PageTitle>

<h1 class="text-danger">Error.</h1>
<h2 class="text-danger">An error occurred while processing your request.</h2>

@if (ShowRequestId)
{
    <p>
        <strong>Request ID:</strong> <code>@RequestId</code>
    </p>
}

<h3>Development Mode</h3>
<p>
    Swapping to <strong>Development</strong> environment will display more detailed information about the error that occurred.
</p>
<p>
    <strong>The Development environment shouldn't be enabled for deployed applications.</strong>
    It can result in displaying sensitive information from exceptions to end users.
    For local debugging, enable the <strong>Development</strong> environment by setting the <strong>ASPNETCORE_ENVIRONMENT</strong> environment variable to <strong>Development</strong>
    and restarting the app.
</p>

@code{
    [CascadingParameter]
    private HttpContext? HttpContext { get; set; }

    private string? RequestId { get; set; }
    private bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    protected override void OnInitialized() =>
        RequestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier;
}

```

## File: FoodList.razor

```C#
@page "/FoodList"
@rendermode InteractiveServer
@using BulkCarnageIQ.Core.Carnage
@using BulkCarnageIQ.Core.Carnage.Report
@using BulkCarnageIQ.Core.Contracts
@inject IFoodItemService FoodItemService

<PageTitle>Food List</PageTitle>

<h2 class="my-4 text-center">
    <i class="bi bi-egg-fried"></i> Food Items
</h2>

<!-- Filters Section in a Card -->
<div class="card shadow-sm mb-4">
    <div class="card-header bg-primary text-white">
        <h4 class="mb-0">
            <i class="bi bi-funnel"></i> Filters
        </h4>
    </div>
    <div class="card-body">
        <div class="d-flex flex-wrap mb-3">
            <!-- Recipe Name Dropdown -->
            <div class="me-3">
                <label for="recipeName">Recipe Name</label>
                <select class="form-select" id="recipeName" @bind="filter.SelectedRecipeName">
                    <option value="">Select Recipe</option>
                    @foreach (var recipe in recipeNames)
                    {
                        <option value="@recipe">@recipe</option>
                    }
                </select>
            </div>

            <!-- Group Name Dropdown -->
            <div class="me-3">
                <label for="groupName">Group Name</label>
                <select class="form-select" id="groupName" @bind="filter.SelectedGroupName">
                    <option value="">Select Group</option>
                    @foreach (var group in groupNames)
                    {
                        <option value="@group">@group</option>
                    }
                </select>
            </div>

            <!-- Brand Type Dropdown -->
            <div class="me-3">
                <label for="brandType">Brand Type</label>
                <select class="form-select" id="brandType" @bind="filter.SelectedBrandType">
                    <option value="">Select Brand</option>
                    @foreach (var brand in brandTypes)
                    {
                        <option value="@brand">@brand</option>
                    }
                </select>
            </div>
        </div>

        <div class="d-flex flex-wrap">
            <!-- Flags Section -->
            <div class="form-check me-3">
                <input type="checkbox" class="form-check-input" id="highProtein" @bind="filter.IsHighProtein" />
                <label class="form-check-label" for="highProtein">High Protein</label>
            </div>
            <div class="form-check me-3">
                <input type="checkbox" class="form-check-input" id="lowCarb" @bind="filter.IsLowCarb" />
                <label class="form-check-label" for="lowCarb">Low Carb</label>
            </div>
            <div class="form-check me-3">
                <input type="checkbox" class="form-check-input" id="keto" @bind="filter.IsKeto" />
                <label class="form-check-label" for="keto">Keto</label>
            </div>
            <div class="form-check me-3">
                <input type="checkbox" class="form-check-input" id="bulkMeal" @bind="filter.IsBulkMeal" />
                <label class="form-check-label" for="bulkMeal">Bulk Meal</label>
            </div>
            <div class="form-check me-3">
                <input type="checkbox" class="form-check-input" id="highCarb" @bind="filter.IsHighCarb" />
                <label class="form-check-label" for="highCarb">High Carb</label>
            </div>
            <div class="form-check me-3">
                <input type="checkbox" class="form-check-input" id="lowFiber" @bind="filter.IsLowFiber" />
                <label class="form-check-label" for="lowFiber">Low Fiber</label>
            </div>
            <div class="form-check me-3">
                <input type="checkbox" class="form-check-input" id="highFat" @bind="filter.IsHighFat" />
                <label class="form-check-label" for="highFat">High Fat</label>
            </div>
            <div class="form-check me-3">
                <input type="checkbox" class="form-check-input" id="balancedMeal" @bind="filter.IsBalancedMeal" />
                <label class="form-check-label" for="balancedMeal">Balanced Meal</label>
            </div>
            <div class="form-check me-3">
                <input type="checkbox" class="form-check-input" id="lowProtein" @bind="filter.IsLowProtein" />
                <label class="form-check-label" for="lowProtein">Low Protein</label>
            </div>
        </div>

        <div class="d-flex flex-wrap mb-3 mt-3">
            <div class="me-3">
                <button class="btn btn-sm btn-outline-secondary" @onclick="ClearFilters">
                    <i class="bi bi-x-circle"></i> Reset Filters
                </button>
            </div>
        </div>
    </div>
</div>

@if (foodItems == null)
{
    <div class="text-center my-5">
        <div class="spinner-border text-primary" role="status"></div>
    </div>
}
else if (foodItems.Count == 0)
{
    <p>No food items found.</p>
}
else
{
    var filteredItems = filter.ApplyFilters(foodItems);
    var groupedItems = filteredItems.GroupBy(item => item.GroupName ?? "Ungrouped");

    foreach (var group in groupedItems)
    {
        <div class="card shadow-lg mb-4 rounded-4 border-0">
            <div class="card-header bg-primary text-white rounded-top">
                <h4 class="mb-0">
                    <i class="bi bi-collection"></i> @group.Key
                </h4>
            </div>
            <div class="card-body">
                <div class="row row-cols-1 row-cols-md-2 row-cols-lg-3 g-4">
                    @foreach (var item in group)
                    {
                        <div class="col">
                            <div class="card h-100 shadow rounded-4 border-0">
                                @if (!string.IsNullOrEmpty(item.PictureLink))
                                {
                                    <img src="@item.PictureLink" alt="@item.RecipeName" class="card-img-top img-fluid rounded-top" style="max-height: 300px; width: 100%; height: auto; object-fit: contain;" />
                                }
                                else
                                {
                                    <img src="photos/FoodItemPlaceHolder.jpg" alt="@item.RecipeName" class="card-img-top img-fluid rounded-top" style="max-height: 300px; width: 100%; height: auto; object-fit: contain;" />
                                }

                                <div class="card-body d-flex flex-column">
                                    <h5 class="card-title d-flex justify-content-between align-items-center">
                                        @item.RecipeName
                                        <span class="badge bg-secondary">@item.BrandType</span>
                                    </h5>

                                    <p class="text-muted small mb-2">
                                        @item.Servings x @item.MeasurementServings @item.MeasurementType
                                    </p>

                                    <div class="mb-2">
                                        @if (item.IsBreakfast)
                                        {
                                            <span class="badge bg-warning text-dark me-1"><i class="bi bi-sunrise"></i> Breakfast</span>
                                        }
                                        @if (item.IsLunch)
                                        {
                                            <span class="badge bg-success text-white me-1"><i class="bi bi-box"></i> Lunch</span>
                                        }
                                        @if (item.IsDinner)
                                        {
                                            <span class="badge bg-dark text-white me-1"><i class="bi bi-moon"></i> Dinner</span>
                                        }
                                        @if (item.IsSnack)
                                        {
                                            <span class="badge bg-info text-dark me-1"><i class="bi bi-cookie"></i> Snack</span>
                                        }
                                    </div>

                                    <div class="mb-2">
                                        @if (item.IsHighProtein)
                                        {
                                            <span class="badge bg-success me-1"><i class="bi bi-fire"></i> High Protein</span>
                                        }
                                        @if (item.IsLowCarb)
                                        {
                                            <span class="badge bg-info text-dark me-1"><i class="bi bi-arrow-down-circle"></i> Low Carb</span>
                                        }
                                        @if (item.IsKeto)
                                        {
                                            <span class="badge bg-warning text-dark me-1"><i class="bi bi-droplet-half"></i> Keto</span>
                                        }
                                        @if (item.IsBulkMeal)
                                        {
                                            <span class="badge bg-danger me-1"><i class="bi bi-graph-up"></i> Bulk</span>
                                        }
                                        @if (item.IsHighCarb)
                                        {
                                            <span class="badge bg-danger text-white me-1"><i class="bi bi-cup-straw"></i> High Carb</span>
                                        }
                                        @if (item.IsLowFiber)
                                        {
                                            <span class="badge bg-secondary me-1"><i class="bi bi-flower3"></i> Low Fiber</span>
                                        }
                                        @if (item.IsHighFat)
                                        {
                                            <span class="badge bg-dark text-white me-1"><i class="bi bi-droplet"></i> High Fat</span>
                                        }
                                        @if (item.IsBalancedMeal)
                                        {
                                            <span class="badge bg-primary me-1"><i class="bi bi-lightning-charge"></i> Balanced Meal</span>
                                        }
                                        @if (item.IsLowProtein)
                                        {
                                            <span class="badge bg-info me-1"><i class="bi bi-egg"></i> Low Protein</span>
                                        }
                                    </div>

                                    <ul class="list-group list-group-flush small mb-3">
                                        <li class="list-group-item"><i class="bi bi-lightning-charge"></i> <strong>Calories:</strong> @item.CaloriesPerServing</li>
                                        <li class="list-group-item"><i class="bi bi-bar-chart-line"></i> <strong>Protein:</strong> @item.Protein g</li>
                                        <li class="list-group-item"><i class="bi bi-cup-straw"></i> <strong>Carbs:</strong> @item.Carbs g</li>
                                        <li class="list-group-item"><i class="bi bi-droplet"></i> <strong>Fats:</strong> @item.Fats g</li>
                                        <li class="list-group-item"><i class="bi bi-flower3"></i> <strong>Fiber:</strong> @item.Fiber g</li>
                                    </ul>

                                    @if (!string.IsNullOrEmpty(item.Link))
                                    {
                                        <a href="@item.Link" class="btn btn-outline-primary btn-sm mt-auto" target="_blank">
                                            <i class="bi bi-box-arrow-up-right"></i> Nutrition Info
                                        </a>
                                    }
                                </div>
                            </div>
                        </div>
                    }
                </div>
            </div>
        </div>
    }
}

@code {
    private List<FoodItem> foodItems;
    private List<string> recipeNames = [];
    private List<string> groupNames = [];
    private List<string> brandTypes = [];
    private FoodItemFilter filter = new FoodItemFilter();

    protected override async Task OnInitializedAsync()
    {
        // Fetch food items from service
        foodItems = await FoodItemService.GetAllAsync();

        // Get the distinct values for RecipeName, GroupName, and BrandType for the dropdowns
        recipeNames = foodItems.Select(f => f.RecipeName).Distinct().ToList();
        groupNames = foodItems.Select(f => f.GroupName ?? "Ungrouped").Distinct().ToList();
        brandTypes = foodItems.Select(f => f.BrandType).Distinct().ToList();
    }

    private void ClearFilters()
    {
        filter = new FoodItemFilter();
        StateHasChanged();
    }
}

```

## File: GroceryList.razor

```C#
@page "/GroceryList"
@rendermode InteractiveServer
@using System.Threading
@using BulkCarnageIQ.Core.Carnage
@using BulkCarnageIQ.Core.Contracts
@inject IGroceryListService GroceryListService
@inject IMealEntryService MealEntryService
@inject AuthenticationStateProvider AuthStateProvider
@inject ILogger<GroceryList> Logger

<h3><i class="bi bi-cart"></i> Grocery List</h3>

@if (initialLoadComplete)
{
    <div class="mb-3">
        <div class="col-md position-relative">
            <label class="form-label">Search and Add Item</label>
            <InputText class="form-control" @bind-Value="newItemName" @oninput="OnMealNameChanged" placeholder="Type to search..." />
            @if (suggestions.Any())
            {
                <ul class="list-group position-absolute w-100 mt-1" style="z-index: 1050;">
                    @foreach (var suggestion in suggestions)
                    {
                        <li class="list-group-item list-group-item-action" @onclick="() => SelectSuggestion(suggestion)" role="button">
                            @suggestion
                        </li>
                    }
                </ul>
            }
        </div>
    </div>

    @if (!string.IsNullOrEmpty(errorMessage))
    {
        <div class="alert alert-danger" role="alert">
            @errorMessage
        </div>
    }

    @if (!items.Any())
    {
        <p>No items yet.</p>
    }
    else
    {
        @foreach (var group in items.GroupBy(x => x.GroupName))
        {
            <div class="card mb-4">
                <div class="card-header bg-primary text-white rounded-top">
                    <h5 class="mb-0">@(!string.IsNullOrEmpty(group.Key) ? group.Key : "Ungrouped")</h5>
                </div>
                <div class="card-body">
                    <div class="row">
                        @foreach (var item in group)
                        {
                            <div class="col-md-6 col-lg-4 mb-3" @key="item.Id">
                                <div class="card @(item.IsCompleted ? "opacity-50" : "") @(item.IsSaving ? "opacity-75" : "")">
                                    <div class="card-body d-flex justify-content-between align-items-center">
                                        <div>
                                            <input type="checkbox" class="form-check-input me-2"
                                                   checked="@item.IsCompleted"
                                                   disabled="@item.IsSaving"
                                                   @onchange="@(() => ToggleCompleted(item))" />
                                            <strong>@item.RecipeName</strong><br />
                                            <small>@item.MeasurementServings @item.MeasurementType</small>
                                        </div>
                                        <div>
                                            <button class="btn btn-sm btn-outline-secondary me-2 @(item.IsSaving ? "disabled" : "")" @onclick="() => ToggleFavorite(item)">
                                                @if (item.IsSaving && item.LastAction == ItemAction.ToggleFavorite)
                                                {
                                                    <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                                                }
                                                else
                                                {
                                                    <i class="bi @(item.IsFavorite ? "bi-star-fill text-warning" : "bi-star")"></i>
                                                }
                                            </button>
                                            <button class="btn btn-sm btn-outline-danger @(item.IsSaving ? "disabled" : "")" @onclick="() => Remove(item)">
                                                @if (item.IsSaving && item.LastAction == ItemAction.Remove)
                                                {
                                                    <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                                                }
                                                else
                                                {
                                                    <i class="bi bi-trash"></i>
                                                }
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        }
                    </div>
                </div>
            </div>
        }
    }

    <div class="mt-3">
        <button class="btn btn-outline-success me-2" @onclick="GenerateFromMeals" disabled="@isGenerating">
            @if (isGenerating)
            {
                <span class="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true"></span>
            }
            <i class="bi bi-arrow-repeat"></i> Auto-generate from Meals
        </button>
        <button class="btn btn-outline-danger" @onclick="ClearCompleted" disabled="@isClearing">
            @if (isClearing)
            {
                <span class="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true"></span>
            }
            <i class="bi bi-trash"></i> Clear Completed
        </button>
    </div>
}
else
{
    <p><em>Loading grocery list...</em> <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span></p>
}

@code {
    private class GroceryListItemUI : GroceryListItem
    {
        public bool IsSaving { get; set; }
        public ItemAction? LastAction { get; set; }

        public static GroceryListItemUI FromGroceryListItem(GroceryListItem item)
        {
            return new GroceryListItemUI
                {
                    Id = item.Id,
                    UserId = item.UserId,
                    RecipeName = item.RecipeName,
                    GroupName = item.GroupName,
                    MeasurementType = item.MeasurementType,
                    MeasurementServings = item.MeasurementServings,
                    IsCompleted = item.IsCompleted,
                    IsFavorite = item.IsFavorite,
                    CreatedDate = item.CreatedDate,
                    IsSaving = false,
                    LastAction = null
                };
        }
    }

    private enum ItemAction { ToggleFavorite, ToggleComplete, Remove }

    private List<GroceryListItemUI> items = new();
    private string? newItemName;
    private bool initialLoadComplete = false;
    private string userId = "";
    private List<string> suggestions = new();
    private string? errorMessage;

    private bool isAdding = false;
    private bool isGenerating = false;
    private bool isClearing = false;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var auth = await AuthStateProvider.GetAuthenticationStateAsync();
            userId = auth.User?.Identity?.Name ?? string.Empty;

            await LoadItems();
            initialLoadComplete = true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during component initialization for user {UserId}", userId);
            errorMessage = "Failed to initialize the grocery list. Please try refreshing.";
            initialLoadComplete = true;
        }
    }

    private async Task LoadItems()
    {
        errorMessage = null;
        try
        {
            var fetchedItems = await GroceryListService.GetListForUserAsync(userId);
            items = fetchedItems.Select(GroceryListItemUI.FromGroceryListItem).ToList();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading grocery list items for user {UserId}", userId);
            errorMessage = "Failed to load items. Please try again later.";
            items = new();
        }
    }

    private async Task Add()
    {
        if (string.IsNullOrWhiteSpace(newItemName) || isAdding) return;

        isAdding = true;
        errorMessage = null;
        StateHasChanged();

        var item = await MealEntryService.GetFoodItemByNameAsync(newItemName);

        if (item == null)
        {
            errorMessage = "Item not found. Please try a different name.";
            isAdding = false;
            StateHasChanged();
            return;
        }

        var itemToAdd = new GroceryListItem
            {
                RecipeName = newItemName.Trim(),
                GroupName = item.GroupName,
                UserId = userId,
                CreatedDate = DateOnly.FromDateTime(DateTime.UtcNow),
                IsFavorite = false,
                IsCompleted = false,
                MeasurementType = item.MeasurementType,
                MeasurementServings = item.MeasurementServings * item.Servings
            };

        try
        {
            var addedItem = await GroceryListService.AddItemAsync(itemToAdd);

            if (addedItem != null)
            {
                items.Insert(0, GroceryListItemUI.FromGroceryListItem(addedItem));
                newItemName = "";
                suggestions.Clear();
            }
            else
            {
                Logger.LogWarning("AddItemAsync returned null for user {UserId}, item name {ItemName}. Reloading list as fallback.", userId, itemToAdd.RecipeName);
                await LoadItems();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error adding item to grocery list for user {UserId}, item name {ItemName}", userId, newItemName);
            errorMessage = "Failed to add item. Please try again.";
        }
        finally
        {
            isAdding = false;
            StateHasChanged();
        }
    }

    private void ClearSuggestions()
    {
        // Clear suggestions when focus is lost
        suggestions.Clear();
    }

    private async Task ToggleFavorite(GroceryListItemUI item)
    {
        if (item.IsSaving) return;

        item.IsSaving = true;
        item.LastAction = ItemAction.ToggleFavorite;
        StateHasChanged();

        try
        {
            item.IsFavorite = !item.IsFavorite;
            await GroceryListService.ToggleFavoriteAsync(item.Id, userId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error toggling favorite for item {ItemId}", item.Id);
            // Revert change on error
            item.IsFavorite = !item.IsFavorite;
            errorMessage = "Failed to update favorite status. Please try again.";
        }
        finally
        {
            item.IsSaving = false;
            item.LastAction = null;
            StateHasChanged();
        }
    }

    private async Task ToggleCompleted(GroceryListItemUI item)
    {
        if (item.IsSaving) return;

        item.IsSaving = true;
        item.LastAction = ItemAction.ToggleComplete;
        StateHasChanged();

        try
        {
            item.IsCompleted = !item.IsCompleted;
            await GroceryListService.ToggleCompletedAsync(item.Id, userId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error toggling completion for item {ItemId}", item.Id);
            // Revert change on error
            item.IsCompleted = !item.IsCompleted;
            errorMessage = "Failed to update completion status. Please try again.";
        }
        finally
        {
            item.IsSaving = false;
            item.LastAction = null;
            StateHasChanged();
        }
    }

    private async Task Remove(GroceryListItemUI item)
    {
        if (item.IsSaving) return;

        item.IsSaving = true;
        item.LastAction = ItemAction.Remove;
        StateHasChanged();

        try
        {
            await GroceryListService.RemoveItemAsync(item.Id, userId);
            items.Remove(item);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error removing item {ItemId} from grocery list", item.Id);
            errorMessage = "Failed to remove item. Please try again.";
        }
        finally
        {
            item.IsSaving = false;
            item.LastAction = null;
            StateHasChanged();
        }
    }

    private async Task GenerateFromMeals()
    {
        if (isGenerating) return;

        isGenerating = true;
        StateHasChanged();

        try
        {
            await GroceryListService.AutoGenerateFromRecentMealsAsync(userId);
            await LoadItems();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error generating grocery list from meals for user {UserId}", userId);
            errorMessage = "Failed to generate grocery list from meals. Please try again.";
        }
        finally
        {
            isGenerating = false;
            StateHasChanged();
        }
    }

    private async Task ClearCompleted()
    {
        if (isClearing) return;

        isClearing = true;
        StateHasChanged();

        try
        {
            var completedItems = items.Where(i => i.IsCompleted).ToList();
            foreach (var completedItem in completedItems)
            {
                await Remove(completedItem);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error clearing completed items for user {UserId}", userId);
            errorMessage = "Failed to clear completed items. Please try again.";
        }
        finally
        {
            isClearing = false;
            StateHasChanged();
        }
    }

    private async Task OnMealNameChanged(ChangeEventArgs e)
    {
        newItemName = e.Value?.ToString() ?? string.Empty;
        var input = newItemName;

        if (input.Length >= 2)
        {
            suggestions = await MealEntryService.SearchFoodNamesAsync(input);
        }
        else
        {
            suggestions.Clear();
        }
        StateHasChanged();
    }

    private async Task SelectSuggestion(string suggestion)
    {
        newItemName = suggestion;
        suggestions.Clear();
        await Add();
    }
}

```

## File: Home.razor

```C#
@page "/"
@rendermode InteractiveServer
@using BulkCarnageIQ.Core.Carnage
@using BulkCarnageIQ.Core.Carnage.Report
@using BulkCarnageIQ.Core.Contracts
@inject IMealEntryService MealService
@inject IUserProfileService UserProfileService
@inject AuthenticationStateProvider AuthProvider

<div class="container mt-4">
    @if (DailyMacros != null)
    {
        <div class="card shadow-sm mb-4">
            <div class="card-body">
                <h4 class="card-title mb-3"><i class="bi bi-nut me-2"></i> Macros Today</h4>
                <BulkCarnageIQ.Web.Components.Carnage.MacroAlertPanel DailyMacros="DailyMacros" UserProfile="UserProfile" />
            </div>
        </div>
    }

    @if (WeeklyMacros != null)
    {
        <div class="card shadow-sm mb-4">
            <div class="card-body">
                <h4 class="card-title mb-3"><i class="bi bi-graph-up-arrow me-2"></i> Macro Heatmap This Week</h4>
                <BulkCarnageIQ.Web.Components.Carnage.WeeklyMacroHeatmap 
                    WeeklyData="WeeklyMacros"
                    ProteinGoal="UserProfile!.ProteinGoal"
                    CarbsGoal="UserProfile!.CarbsGoal"
                    FatsGoal="UserProfile!.FatGoal"
                    FiberGoal="UserProfile!.FiberGoal" />
            </div>
        </div>
    }

    @if (labels != null && calories != null)
    {
        <div class="card shadow-sm mb-4">
            <div class="card-body">
                <h4 class="card-title mb-3"><i class="bi bi-graph-up-arrow me-2"></i> Calories This Week</h4>
                <BulkCarnageIQ.Web.Components.Carnage.CalorieBarChart @ref="CalorieBarChart" Labels="labels" Data="calories" ChartLabel="Calories Eaten" />
            </div>
        </div>
    }
</div>

@code {
    private List<string>? labels;
    private List<int>? calories;
    private MacroSummary? DailyMacros;
    private Dictionary<string, MacroSummary>? WeeklyMacros;
    private UserProfile? UserProfile;

    private BulkCarnageIQ.Web.Components.Carnage.CalorieBarChart? CalorieBarChart;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthProvider.GetAuthenticationStateAsync();
        var userId = authState.User?.Identity?.Name;

        if (!string.IsNullOrWhiteSpace(userId))
        {
            var data = await MealService.GetCaloriesByDayAsync(userId);

            // Sort by weekday order
            string[] weekdayOrder = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];
            labels = weekdayOrder.Where(day => data.ContainsKey(day)).ToList();
            calories = labels.Select(day => (int)data[day]).ToList();

            // Daily Macros:
            DailyMacros = await MealService.GetMacroSummaryAsync(DateOnly.FromDateTime(DateTime.Today), userId);

            // Weekly Macros:
            WeeklyMacros = await MealService.GetMacroSummariesByDateRangeAsync(
                DateOnly.FromDateTime(DateTime.Today.AddDays(-6)),
                DateOnly.FromDateTime(DateTime.Today),
                userId
            );

            // Fetch UserProfile
            UserProfile = await UserProfileService.GetUserProfile(userId);
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (CalorieBarChart != null)
        {
            await CalorieBarChart.InitializeChart();
        }
    }
}

```

## File: MealTracker.razor

```C#
@page "/MealTracker"
@rendermode InteractiveServer
@using BulkCarnageIQ.Core.Carnage
@using BulkCarnageIQ.Core.Contracts
@inject IMealEntryService MealEntryService
@inject IFoodItemService FoodItemService
@inject IGroupFoodService GroupFoodService
@inject AuthenticationStateProvider AuthProvider
@using System.Linq

<h3><i class="bi bi-calendar-week"></i> Meal Tracker</h3>

<div class="mb-4">
    <EditForm Model="@newEntry" OnValidSubmit="SaveMeal">
        <DataAnnotationsValidator />
        <ValidationSummary />

        <div class="row g-2 align-items-end">
            <div class="col-md-2">
                <label class="form-label">Date</label>
                <InputDate class="form-control" @bind-Value="newEntry.Date" />
            </div>
            <div class="col-md-2">
                <label class="form-label">Meal Type</label>
                <InputSelect class="form-control" @bind-Value="newEntry.MealType">
                    <option value="Breakfast">Breakfast</option>
                    <option value="Lunch">Lunch</option>
                    <option value="Dinner">Dinner</option>
                    <option value="Snack">Snack</option>
                </InputSelect>
            </div>
            <div class="col-md position-relative">
                <label class="form-label">Meal Name</label>
                <InputText class="form-control" @bind-Value="newEntry.MealName" @oninput="OnMealNameChanged" placeholder="Type to search..." />
                @if (suggestions.Any())
                {
                    <ul class="list-group position-absolute w-100" style="z-index: 1050;">
                        @foreach (var suggestion in suggestions)
                        {
                            <li class="list-group-item list-group-item-action"
                            @onclick="() => SelectSuggestion(suggestion)" role="button">
                                @suggestion
                            </li>
                        }
                    </ul>
                }
            </div>
            <div class="col-md-auto">
                <button type="submit" class="btn btn-success w-100">
                    <i class="bi bi-plus-circle"></i> Add
                </button>
            </div>
        </div>
    </EditForm>
</div>

<BulkCarnageIQ.Web.Components.Carnage.GroupMealQuickAdd OnMealsAdded="HandleGroupMealsAdded" GroupItems="groupFoodItems" />

@if (entries is null)
{
    <p><em>Loading meals...</em></p>
}
else if (!entries.Any())
{
    <p>No meals tracked yet.</p>
}
else
{
    @foreach (var monthGroup in entries.GroupBy(e => new { Year = e.Date.Year, Month = e.Date.Month }).OrderByDescending(g => g.Key.Year).ThenByDescending(g => g.Key.Month))
    {
        <div class="mb-4">
            <h4 class="text-primary mb-3">
                <span role="button" @onclick="@(() => ToggleMonth(monthGroup.Key))">
                    <i class="bi @(IsMonthExpanded(monthGroup.Key) ? "bi-chevron-down" : "bi-chevron-right") me-2"></i>
                    @(new DateTime(monthGroup.Key.Year, monthGroup.Key.Month, 1).ToString("MMMM yyyy"))
                </span>
            </h4>

            @if (IsMonthExpanded(monthGroup.Key))
            {
                foreach (var dateGroup in monthGroup.GroupBy(e => e.Date).OrderByDescending(g => g.Key))
                {
                    <div class="ms-3 mb-4">
                        <h5 class="text-secondary mb-2">
                            <span role="button" @onclick="@(() => ToggleDate(dateGroup.Key))">
                                <i class="bi @(IsExpanded(dateGroup.Key) ? "bi-chevron-down" : "bi-chevron-right") me-2"></i>
                                @dateGroup.Key.ToString("MMMM dd, yyyy")
                            </span>
                        </h5>

                        @if (IsExpanded(dateGroup.Key))
                        {
                            @* Your inner mealGroup logic stays the same *@
                            @foreach (var mealGroup in dateGroup.GroupBy(m => m.MealType).OrderBy(mg => GetMealTypeOrder(mg.Key)))
                            {
                                <div class="card shadow-sm mb-3">
                                    <div class="card-header bg-light d-flex justify-content-between align-items-center flex-wrap">
                                        <h6 class="mb-0 me-3">
                                            <i class="bi bi-egg-fried me-2"></i>@mealGroup.Key
                                        </h6>
                                        <small class="text-muted text-nowrap">
                                            Total: @mealGroup.Sum(m => m.Calories).ToString("N0") Cal
                                            | P:@mealGroup.Sum(m => m.Protein).ToString("N1")g
                                            | C:@mealGroup.Sum(m => m.Carbs).ToString("N1")g
                                            | F:@mealGroup.Sum(m => m.Fats).ToString("N1")g
                                        </small>
                                    </div>
                                    <div class="card-body p-0">
                                        <ul class="list-group list-group-flush">
                                            @foreach (var meal in mealGroup)
                                            {
                                                <li class="list-group-item">
                                                    <div class="row align-items-center g-2">
                                                        <div class="col-lg-3 col-md-12 fw-bold">
                                                            @meal.MealName (@meal.MeasurementServings x @meal.MeasurementType)
                                                        </div>
                                                        <div class="col-lg-2 col-md-3">
                                                            <div class="input-group input-group-sm">
                                                                <InputNumber class="form-control" @bind-Value="meal.PortionEaten" />
                                                                <button class="btn btn-outline-secondary" @onclick="() => UpdateMealMacros(meal)" title="Update Macros">
                                                                    <i class="bi bi-calculator"></i>
                                                                </button>
                                                            </div>
                                                        </div>
                                                        <div class="col-lg-6 col-md-7 small text-muted">
                                                            <span class="me-2" title="Calories"><i class="bi bi-fire me-1"></i>@meal.Calories.ToString("N0")</span>
                                                            <span class="me-2" title="Protein"><i class="bi bi-egg me-1"></i>@meal.Protein.ToString("N1")g</span>
                                                            <span class="me-2" title="Carbs"><i class="bi bi-basket me-1"></i>@meal.Carbs.ToString("N1")g</span>
                                                            <span class="me-2" title="Fats"><i class="bi bi-droplet me-1"></i>@meal.Fats.ToString("N1")g</span>
                                                            <span title="Fiber"><i class="bi bi-flower2 me-1"></i>@meal.Fiber.ToString("N1")g</span>
                                                        </div>
                                                        <div class="col-lg-1 col-md-2 text-end">
                                                            <button class="btn btn-sm btn-outline-danger" @onclick="() => DeleteMeal(meal)" title="Delete Item">
                                                                <i class="bi bi-trash"></i>
                                                            </button>
                                                        </div>
                                                    </div>
                                                </li>
                                            }
                                        </ul>
                                    </div>
                                </div>
                            }

                            <div class="mt-3 pt-3 border-top">
                                <h6 class="text-secondary">Daily Totals (@dateGroup.Key.ToString("MMMM dd"))</h6>
                                <div class="d-flex justify-content-around flex-wrap small">
                                    <span class="mx-2"><strong>Calories:</strong> @dateGroup.Sum(m => m.Calories).ToString("N0")</span>
                                    <span class="mx-2"><strong>Protein:</strong> @dateGroup.Sum(m => m.Protein).ToString("N1")g</span>
                                    <span class="mx-2"><strong>Carbs:</strong> @dateGroup.Sum(m => m.Carbs).ToString("N1")g</span>
                                    <span class="mx-2"><strong>Fats:</strong> @dateGroup.Sum(m => m.Fats).ToString("N1")g</span>
                                    <span class="mx-2"><strong>Fiber:</strong> @dateGroup.Sum(m => m.Fiber).ToString("N1")g</span>
                                </div>
                            </div>
                        }
                    </div>
                }
            }
        </div>
    }
}

@code {
    private List<string> suggestions = new();
    private List<MealEntry>? entries;
    private List<GroupFoodItem>? groupFoodItems;
    private string currentUserId = string.Empty;
    private MealEntry newEntry;
    private HashSet<DateOnly> expandedDates = new();

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthProvider.GetAuthenticationStateAsync();
        currentUserId = authState.User?.Identity?.Name ?? string.Empty;

        InitializeNewEntry(); // Set defaults including UserId

        if (!string.IsNullOrEmpty(currentUserId))
        {
            entries = await MealEntryService.GetAllAsync(currentUserId);
            // Optionally pre-expand today's date
            var today = DateOnly.FromDateTime(DateTime.Today);
            if (entries?.Any(e => e.Date == today) == true)
            {
                expandedDates.Add(today);
            }

            ToggleMonth(new { Year = today.Year, Month = today.Month });
        }
        else
        {
            entries = new List<MealEntry>();
        }

        groupFoodItems = await GroupFoodService.GetAllAsync();
    }

    private void InitializeNewEntry(DateOnly? date = null, string? mealType = null)
    {
        var targetDate = date ?? DateOnly.FromDateTime(DateTime.Today);
        newEntry = new MealEntry
            {
                Date = targetDate,
                Day = targetDate.DayOfWeek.ToString(),
                MealType = mealType ?? "Breakfast",
                MealName = string.Empty,
                GroupName = string.Empty,
                PortionEaten = 1.0F, // Default portion
                UserId = currentUserId
            };
        suggestions.Clear();
    }

    private async Task SaveMeal()
    {
        if (string.IsNullOrWhiteSpace(newEntry.MealName) || string.IsNullOrWhiteSpace(currentUserId))
        {
            return; // Add validation message if needed
        }

        var item = await MealEntryService.GetFoodItemByNameAsync(newEntry.MealName);
        if (item is not null)
        {
            newEntry.Calories = item.CaloriesPerServing * newEntry.PortionEaten;
            newEntry.Protein = item.Protein * newEntry.PortionEaten;
            newEntry.Carbs = item.Carbs * newEntry.PortionEaten;
            newEntry.Fats = item.Fats * newEntry.PortionEaten;
            newEntry.Fiber = item.Fiber * newEntry.PortionEaten;
            newEntry.MeasurementServings = item.MeasurementServings * newEntry.PortionEaten;
            newEntry.MeasurementType = item.MeasurementType;
            newEntry.GroupName = item.GroupName;
        }
        else
        {
            // Handle case where food item details aren't found - maybe clear macros or log warning
            newEntry.Calories = 0;
            newEntry.Protein = 0;
            newEntry.Carbs = 0;
            newEntry.Fats = 0;
            newEntry.Fiber = 0;
        }

        newEntry.Day = newEntry.Date.DayOfWeek.ToString();
        newEntry.UserId = currentUserId; // Ensure UserId is set

        try
        {
            await MealEntryService.AddAsync(newEntry);

            // Keep Date and Meal Type for the next entry, reset others
            DateOnly savedDate = newEntry.Date;
            string savedMealType = newEntry.MealType;

            InitializeNewEntry(savedDate, savedMealType);

            entries = await MealEntryService.GetAllAsync(currentUserId); // Reload entries
            if (!expandedDates.Contains(savedDate)) // Expand the date if it was collapsed
            {
                expandedDates.Add(savedDate);
            }
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving meal: {ex.Message}");
            // Show error message to user
        }
    }

    private async Task OnMealNameChanged(ChangeEventArgs e)
    {
        newEntry.MealName = e.Value?.ToString() ?? string.Empty; // Update bound value explicitly
        var input = newEntry.MealName;

        if (input.Length >= 2)
        {
            suggestions = await MealEntryService.SearchFoodNamesAsync(input);
        }
        else
        {
            suggestions.Clear();
        }
        StateHasChanged(); // Needed to update suggestion list UI
    }

    private async Task SelectSuggestion(string selected)
    {
        newEntry.MealName = selected;
        suggestions.Clear();

        var item = await MealEntryService.GetFoodItemByNameAsync(selected);
        if (item is not null)
        {
            newEntry.PortionEaten = item.Servings;
        }
        StateHasChanged(); // Update UI with selected name and portion
    }

    private async Task UpdateMealMacros(MealEntry meal)
    {
        var item = await MealEntryService.GetFoodItemByNameAsync(meal.MealName);
        if (item is not null)
        {
            meal.Calories = item.CaloriesPerServing * meal.PortionEaten;
            meal.Protein = item.Protein * meal.PortionEaten;
            meal.Carbs = item.Carbs * meal.PortionEaten;
            meal.Fats = item.Fats * meal.PortionEaten;
            meal.Fiber = item.Fiber * meal.PortionEaten;
            meal.MeasurementServings = item.MeasurementServings * meal.PortionEaten;
            meal.MeasurementType = item.MeasurementType;
            meal.GroupName = item.GroupName;

            try
            {
                await MealEntryService.UpdateAsync(meal);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating meal: {ex.Message}");
                // Show error message to user
            }
        }
        else
        {
            // Handle case where food item details aren't found for update - maybe show warning
            Console.WriteLine($"Could not find item details for '{meal.MealName}' during update.");
        }
    }

    private async Task DeleteMeal(MealEntry mealToDelete)
    {
        // Optional: Add JS confirmation dialog here
        bool confirmed = true;

        if (confirmed && entries != null && mealToDelete.Id > 0) // Check for valid Id
        {
            try
            {
                await MealEntryService.DeleteAsync(mealToDelete.Id);
                entries.Remove(mealToDelete);
                StateHasChanged(); // Refresh UI after removal
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting meal: {ex.Message}");
                // Show error message to user
            }
        }
        else if (mealToDelete.Id <= 0)
        {
            Console.WriteLine("Error: Meal entry has invalid Id for deletion.");
        }
    }

    private void ToggleDate(DateOnly date)
    {
        if (!expandedDates.Add(date))
        {
            expandedDates.Remove(date);
        }
    }

    private bool IsExpanded(DateOnly date) => expandedDates.Contains(date);

    private HashSet<object> expandedMonths = new();

    private void ToggleMonth(object monthKey)
    {
        if (!expandedMonths.Add(monthKey))
            expandedMonths.Remove(monthKey);
    }

    private bool IsMonthExpanded(object monthKey) => expandedMonths.Contains(monthKey);

    private int GetMealTypeOrder(string mealType)
    {
        return mealType switch
        {
            "Breakfast" => 1,
            "Lunch" => 2,
            "Dinner" => 3,
            "Snack" => 4,
            _ => 5,
        };
    }

    private async Task HandleGroupMealsAdded(List<GroupFoodItemEntry> newMeals)
    {
        if (newMeals == null || !newMeals.Any() || String.IsNullOrEmpty(currentUserId))
            return;

        foreach (var meal in newMeals)
        {
            var foodItem = await FoodItemService.GetFoodItemByName(meal.RecipeName);

            if (foodItem != null)
            {
                await MealEntryService.AddAsync(new MealEntry
                    {
                        Date = newEntry.Date,
                        Day = newEntry.Date.DayOfWeek.ToString(),
                        MealType = newEntry.MealType,
                        MealName = foodItem.RecipeName,
                        PortionEaten = meal.PortionAmount,
                        Calories = foodItem.CaloriesPerServing * meal.PortionAmount,
                        Protein = foodItem.Protein * meal.PortionAmount,
                        Carbs = foodItem.Carbs * meal.PortionAmount,
                        Fats = foodItem.Fats * meal.PortionAmount,
                        Fiber = foodItem.Fiber * meal.PortionAmount,
                        MeasurementServings = foodItem.MeasurementServings * meal.PortionAmount,
                        MeasurementType = foodItem.MeasurementType,
                        GroupName = foodItem.GroupName,
                        UserId = currentUserId
                    });
            }
        }

        DateOnly savedDate = newEntry.Date;
        entries = await MealEntryService.GetAllAsync(currentUserId); // Reload entries
        if (!expandedDates.Contains(savedDate)) // Expand the date if it was collapsed
        {
            expandedDates.Add(savedDate);
        }
        StateHasChanged();
    }
}

```

## File: MealTrackerReport.razor

```C#
@page "/MealTrackerReport"
@rendermode InteractiveServer
@using BulkCarnageIQ.Core.Carnage
@using BulkCarnageIQ.Core.Contracts
@inject IMealEntryService MealEntryService
@inject IFoodItemService FoodItemService
@inject AuthenticationStateProvider AuthProvider

<h3><i class="bi bi-calendar-week"></i> Meal Tracker</h3>

@if (entries is null)
{
    <p><em>Loading meals...</em></p>
}
else if (!entries.Any())
{
    <p>No meals tracked yet.</p>
}
else
{
    <table class="table table-hover table-bordered small align-middle">
        @foreach (var dateGroup in entries.GroupBy(e => e.Date).OrderByDescending(g => g.Key))
        {
            var date = dateGroup.Key;

            <tbody class="table-group-divider">
                <tr class="table-secondary fw-bold" @onclick="@(() => ToggleDate(date))" style="cursor: pointer;">
                    <td colspan="8">
                        <i class="bi @(IsExpanded(date) ? "bi-chevron-up" : "bi-chevron-down") me-2"></i>
                        @date.ToString("MMMM dd, yyyy") (@dateGroup.First().Day)
                    </td>
                </tr>

                @if (IsExpanded(date))
                {
                    @foreach (var mealGroup in dateGroup.GroupBy(m => m.MealType))
                    {
                        <tr class="fw-bold table-info">
                            <td>@mealGroup.Key</td>
                            <th>Portion</th>
                            <th>Calories</th>
                            <th>Protein</th>
                            <th>Carbs</th>
                            <th>Fats</th>
                            <th>Fiber</th>
                            <th>Tags</th>
                        </tr>

                        @foreach (var meal in mealGroup)
                        {
                            <tr>
                                <td>@meal.MealName (@meal.MeasurementServings x @meal.MeasurementType)</td>
                                <td>@meal.PortionEaten</td>
                                <td>@meal.Calories.ToString("N2")</td>
                                <td>@meal.Protein.ToString("N2")</td>
                                <td>@meal.Carbs.ToString("N2")</td>
                                <td>@meal.Fats.ToString("N2")</td>
                                <td>@meal.Fiber.ToString("N2")</td>
                                <td>
                                    @if (meal.IsHighProtein)
                                    {
                                        <span class="badge bg-success" title="High Protein">High Protein</span>
                                    }
                                    @if (meal.IsLowCarb)
                                    {
                                        <span class="badge bg-warning" title="Low Carb">Low Carb</span>
                                    }
                                    @if (meal.IsKeto)
                                    {
                                        <span class="badge bg-dark" title="Keto">Keto</span>
                                    }
                                    @if (meal.IsBulkMeal)
                                    {
                                        <span class="badge bg-primary" title="Bulk Meal">Bulk Meal</span>
                                    }
                                    @if (meal.IsHighCarb)
                                    {
                                        <span class="badge bg-danger" title="High Carb">High Carb</span>
                                    }
                                    @if (meal.IsLowFiber)
                                    {
                                        <span class="badge bg-secondary" title="Low Fiber">Low Fiber</span>
                                    }
                                    @if (meal.IsHighFat)
                                    {
                                        <span class="badge bg-info" title="High Fat">High Fat</span>
                                    }
                                    @if (meal.IsLowProtein)
                                    {
                                        <span class="badge bg-light text-dark" title="Low Protein">Low Protein</span>
                                    }
                                    @if (meal.IsBalancedMeal)
                                    {
                                        <span class="badge bg-success" title="Balanced Meal">Balanced Meal</span>
                                    }
                                </td>
                            </tr>
                        }
                    }

                    <tr class="table-warning fw-bold">
                        <td colspan="2" class="text-end">Total</td>
                        <td>@dateGroup.Sum(e => e.Calories).ToString("N2")</td>
                        <td>@dateGroup.Sum(e => e.Protein).ToString("N2")</td>
                        <td>@dateGroup.Sum(e => e.Carbs).ToString("N2")</td>
                        <td>@dateGroup.Sum(e => e.Fats).ToString("N2")</td>
                        <td>@dateGroup.Sum(e => e.Fiber).ToString("N2")</td>
                        <td></td>
                    </tr>
                }
            </tbody>
        }
    </table>
}

@code {
    private List<string> suggestions = new();
    private List<MealEntry>? entries;
    private MealEntry newEntry = new()
        {
            Date = DateOnly.FromDateTime(DateTime.Today),
            Day = DateTime.Today.DayOfWeek.ToString(),
            MealType = "Breakfast",
            MealName = string.Empty,
            UserId = string.Empty
        };

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthProvider.GetAuthenticationStateAsync();
        var userId = authState.User?.Identity?.Name;

        entries = await MealEntryService.GetAllAsync(userId!);
    }

    private async Task SaveMeal()
    {
        var authState = await AuthProvider.GetAuthenticationStateAsync();
        var userId = authState.User?.Identity?.Name;

        if (!string.IsNullOrWhiteSpace(newEntry.MealName) && !string.IsNullOrWhiteSpace(userId))
        {
            // Ensure its populated:
            var item = await MealEntryService.GetFoodItemByNameAsync(newEntry.MealName);
            if (item is not null)
            {
                newEntry.Calories = item.CaloriesPerServing * newEntry.PortionEaten;
                newEntry.Protein = item.Protein * newEntry.PortionEaten;
                newEntry.Carbs = item.Carbs * newEntry.PortionEaten;
                newEntry.Fats = item.Fats * newEntry.PortionEaten;
                newEntry.Fiber = item.Fiber * newEntry.PortionEaten;
            }

            newEntry.Day = newEntry.Date.DayOfWeek.ToString();
            newEntry.UserId = userId;
            await MealEntryService.AddAsync(newEntry);

            string mealType = newEntry.MealType;
            DateOnly dateOnly = newEntry.Date;

            newEntry = new MealEntry
                {
                    Date = dateOnly,
                    Day = dateOnly.DayOfWeek.ToString(),
                    MealName = string.Empty,
                    MealType = mealType,
                    UserId = string.Empty
                };

            entries = await MealEntryService.GetAllAsync(userId);
        }
    }

    private async Task OnMealNameChanged(ChangeEventArgs e)
    {
        var input = e.Value?.ToString() ?? string.Empty;

        if (input.Length >= 2) // Optional: only start searching after 2 characters
        {
            suggestions = await MealEntryService.SearchFoodNamesAsync(input);
        }
        else
        {
            suggestions.Clear();
        }
    }

    private async void SelectSuggestion(string selected)
    {
        newEntry.MealName = selected;
        suggestions.Clear();

        var item = await MealEntryService.GetFoodItemByNameAsync(selected);
        if (item is not null)
        {
            newEntry.PortionEaten = item.Servings;
        }
    }
}

@code {
    private HashSet<DateOnly> expandedDates = new();

    private void ToggleDate(DateOnly date)
    {
        if (!expandedDates.Add(date))
            expandedDates.Remove(date);
    }

    private bool IsExpanded(DateOnly date) => !expandedDates.Contains(date);
}

```

## File: MealTrackerReportMacro.razor

```C#
@page "/MealTrackerReportMacro"
@rendermode InteractiveServer
@using BulkCarnageIQ.Core.Carnage
@using BulkCarnageIQ.Core.Contracts
@inject IMealEntryService MealEntryService
@inject IUserProfileService UserProfileService
@inject AuthenticationStateProvider AuthProvider

<h3><i class="bi bi-bar-chart-line"></i> Meal Tracker Report by Macro</h3>

@if (entries is null)
{
    <p><em>Loading meals...</em></p>
}
else if (!entries.Any())
{
    <p>No meals tracked yet.</p>
}
else
{
    <table class="table table-hover table-bordered small align-middle">
        @foreach (var monthGroup in entries
       .GroupBy(e => new { e.Date.Year, e.Date.Month })
       .OrderByDescending(g => g.Key.Year)
       .ThenByDescending(g => g.Key.Month))
        {
            var year = monthGroup.Key.Year;
            var month = monthGroup.Key.Month;
            var monthName = new DateTime(year, month, 1).ToString("MMMM yyyy");

            <tbody class="table-group-divider">
                <tr class="table-secondary fw-bold" @onclick="@(() => ToggleMonth(monthGroup.Key))" style="cursor: pointer;">
                    <td colspan="6">
                        <i class="bi @(IsExpanded(monthGroup.Key) ? "bi-chevron-up" : "bi-chevron-down") me-2"></i>
                        @monthName
                    </td>
                </tr>

                @if (IsExpanded(monthGroup.Key))
                {
                    <!-- Headers for the Month Group -->
                    <tr class="table-info fw-bold">
                        <th>Date</th>
                        <th>Total Calories</th>
                        <th>Total Protein</th>
                        <th>Total Carbs</th>
                        <th>Total Fats</th>
                        <th>Total Fiber</th>
                    </tr>

                    var dayGroups = monthGroup
                    .GroupBy(m => m.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        TotalCalories = g.Sum(m => m.Calories),
                        TotalProtein = g.Sum(m => m.Protein),
                        TotalCarbs = g.Sum(m => m.Carbs),
                        TotalFats = g.Sum(m => m.Fats),
                        TotalFiber = g.Sum(m => m.Fiber)
                    })
                    .OrderByDescending(d => d.Date)
                    .ToList();

                    @foreach (var day in dayGroups)
                    {
                        <tr>
                            <td>@day.Date.ToString("MMMM dd, yyyy")</td>
                            <td style="@GetMacroStyle(day.TotalCalories, userProfile?.CalorieGoal)">
                                @day.TotalCalories.ToString("N2") @((MarkupString)GetArrowIcon(day.TotalCalories, userProfile?.CalorieGoal))
                            </td>
                            <td style="@GetMacroStyle(day.TotalProtein, userProfile?.ProteinGoal)">
                                @day.TotalProtein.ToString("N2") @((MarkupString)GetArrowIcon(day.TotalProtein, userProfile?.ProteinGoal))
                            </td>
                            <td style="@GetMacroStyle(day.TotalCarbs, userProfile?.CarbsGoal)">
                                @day.TotalCarbs.ToString("N2") @((MarkupString)GetArrowIcon(day.TotalCarbs, userProfile?.CarbsGoal))
                            </td>
                            <td style="@GetMacroStyle(day.TotalFats, userProfile?.FatGoal)">
                                @day.TotalFats.ToString("N2") @((MarkupString)GetArrowIcon(day.TotalFats, userProfile?.FatGoal))
                            </td>
                            <td style="@GetMacroStyle(day.TotalFiber, userProfile?.FiberGoal)">
                                @day.TotalFiber.ToString("N2") @((MarkupString)GetArrowIcon(day.TotalFiber, userProfile?.FiberGoal))
                            </td>
                        </tr>
                    }

                    <tr class="table-warning fw-bold">
                        <td class="text-end">Monthly Total</td>
                        <td>@monthGroup.Sum(e => e.Calories).ToString("N2")</td>
                        <td>@monthGroup.Sum(e => e.Protein).ToString("N2")</td>
                        <td>@monthGroup.Sum(e => e.Carbs).ToString("N2")</td>
                        <td>@monthGroup.Sum(e => e.Fats).ToString("N2")</td>
                        <td>@monthGroup.Sum(e => e.Fiber).ToString("N2")</td>
                    </tr>
                }
            </tbody>
        }
    </table>
}

@code {
    private List<MealEntry>? entries;
    private HashSet<(int Year, int Month)> expandedMonths = new();
    private UserProfile? userProfile;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthProvider.GetAuthenticationStateAsync();
        var userId = authState.User?.Identity?.Name;

        entries = await MealEntryService.GetAllAsync(userId!);
        userProfile = await UserProfileService.GetUserProfile(userId!);
    }

    private void ToggleMonth(dynamic key)
    {
        var tupleKey = (Year: (int)key.Year, Month: (int)key.Month);

        if (!expandedMonths.Add(tupleKey))
            expandedMonths.Remove(tupleKey);
    }

    private bool IsExpanded(dynamic key)
    {
        var tupleKey = (Year: (int)key.Year, Month: (int)key.Month);
        return !expandedMonths.Contains(tupleKey);
    }

    private string GetMacroStyle(float actual, float? goal)
    {
        if (goal == null)
        {
            return string.Empty; // No styling
        }

        const float toleranceGreen = 0.05f;
        const float toleranceYellow = 0.20f;
        const float toleranceRed = 0.40f;
        const float extremeRed = 0.60f;

        float difference = actual - goal.Value;

        if (difference <= 0)
        {
            float percentUnder = Math.Abs(difference) / goal.Value;
            if (percentUnder >= 0.20f)
            {
                return "background-color:#A8E6CF; color:#1b4332;"; // Very Good
            }
            return "background-color:#D4F5E9; color:#1b4332;"; // Good
        }

        float percentOver = difference / goal.Value;

        if (percentOver <= toleranceGreen)
        {
            return "background-color:#D4F5E9; color:#1b4332;"; // Slightly over = still Green
        }
        else if (percentOver <= toleranceYellow)
        {
            return "background-color:#FFFACD; color:#7b5e00;"; // Mild Warning
        }
        else if (percentOver <= toleranceRed)
        {
            return "background-color:#FFB6B6; color:#7b0000;"; // Bad
        }
        else if (percentOver <= extremeRed)
        {
            return "background-color:#FF7F7F; color:#7b0000;"; // Very Bad
        }
        else
        {
            return "background-color:#E57373; color:#4b0000;"; // Extreme Bad
        }
    }

    private MarkupString GetArrowIcon(float actual, float? goal)
    {
        if (goal == null)
        {
            return new MarkupString(string.Empty); // No goal, no icon
        }

        // Logic to decide whether to show an up or down arrow
        if (Math.Abs(actual - goal.Value) / goal.Value <= 0.1f) // Within 10% tolerance
        {
            return new MarkupString(string.Empty); // No arrow if within goal range
        }

        if (actual < goal) // Below the goal
        {
            return new MarkupString("<i class='bi bi-arrow-down text-danger'></i>"); // Down arrow, red
        }
        else if (actual > goal) // Above the goal
        {
            return new MarkupString("<i class='bi bi-arrow-up text-success'></i>"); // Up arrow, green
        }

        return new MarkupString(string.Empty); // No icon
    }
}

```

## File: MealTrackerTopFoods.razor

```C#
@page "/MealTrackerTopFoods"
@rendermode InteractiveServer
@using BulkCarnageIQ.Core.Carnage
@using BulkCarnageIQ.Core.Contracts
@inject IMealEntryService MealEntryService
@inject AuthenticationStateProvider AuthProvider

<h3><i class="bi bi-bar-chart-line"></i> Top Foods by Month</h3>

@if (entries is null)
{
    <p><em>Loading meals...</em></p>
}
else if (!entries.Any())
{
    <p>No meals tracked yet.</p>
}
else
{
    <table class="table table-hover table-bordered small align-middle">
        @foreach (var monthGroup in entries
       .GroupBy(e => new { e.Date.Year, e.Date.Month })
       .OrderByDescending(g => g.Key.Year)
       .ThenByDescending(g => g.Key.Month))
        {
            var year = monthGroup.Key.Year;
            var month = monthGroup.Key.Month;
            var monthName = new DateTime(year, month, 1).ToString("MMMM yyyy");

            <tbody class="table-group-divider">
                <tr class="table-secondary fw-bold" @onclick="@(() => ToggleMonth(monthGroup.Key))" style="cursor: pointer;">
                    <td colspan="9">
                        <i class="bi @(IsExpanded(monthGroup.Key) ? "bi-chevron-up" : "bi-chevron-down") me-2"></i>
                        @monthName
                    </td>
                </tr>

                @if (IsExpanded(monthGroup.Key))
                {
                    var foodGroups = monthGroup
                    .GroupBy(m => m.MealName)
                    .Select(g => new
                    {
                        MealName = String.Format("{0} ({1} x {2})", g.Key, g.Sum(m => m.MeasurementServings)?.ToString("N2"), g.First().MeasurementType),
                        GroupName = g.First().GroupName ?? "Unknown",
                        TotalPortion = g.Sum(m => m.PortionEaten),
                        TotalCalories = g.Sum(m => m.Calories),
                        TotalProtein = g.Sum(m => m.Protein),
                        TotalCarbs = g.Sum(m => m.Carbs),
                        TotalFats = g.Sum(m => m.Fats),
                        TotalFiber = g.Sum(m => m.Fiber)
                    })
                    .OrderByDescending(f => f.TotalCalories)
                    .ToList();

                    <tr class="fw-bold table-info">
                        <td>Group Name</td>
                        <td>Meal Name</td>
                        <th>Total Portion</th>
                        <th>Total Calories</th>
                        <th>Total Protein</th>
                        <th>Total Carbs</th>
                        <th>Total Fats</th>
                        <th>Total Fiber</th>
                        <th>Flags</th>
                    </tr>

                    @foreach (var food in foodGroups)
                    {
                        var caloriePerPortion = food.TotalCalories / (food.TotalPortion == 0 ? 1 : food.TotalPortion);
                        var fatToCarbRatio = food.TotalFats / (food.TotalCarbs == 0 ? 1 : food.TotalCarbs);
                        var isFatBomb = food.TotalFats > 150 && fatToCarbRatio > 1.5;
                        var isCarbBomb = food.TotalCarbs > 200 && food.TotalFiber < 10;
                        var isCalorieBomb = caloriePerPortion > 400;
                        var isFiberFail = food.TotalFiber < 5;

                        var showLowFiber = isFatBomb || isCarbBomb || isCalorieBomb;

                        <tr>
                            <td>@food.GroupName</td>
                            <td>@food.MealName</td>
                            <td>@food.TotalPortion.ToString("N2")</td>
                            <td>@food.TotalCalories.ToString("N2")</td>
                            <td>@food.TotalProtein.ToString("N2")</td>
                            <td>@food.TotalCarbs.ToString("N2")</td>
                            <td>@food.TotalFats.ToString("N2")</td>
                            <td>@food.TotalFiber.ToString("N2")</td>
                            <td>
                                @if (isFatBomb)
                                {
                                    <span class="badge bg-danger me-1"><i class="bi bi-droplet-fill"></i> Fat Bomb</span>
                                }
                                @if (isCarbBomb)
                                {
                                    <span class="badge bg-warning text-dark me-1"><i class="bi bi-cup-straw"></i> Carb Bomb</span>
                                }
                                @if (isCalorieBomb)
                                {
                                    <span class="badge bg-danger me-1"><i class="bi bi-fire"></i> Calorie Bomb</span>
                                }
                                @if (showLowFiber && isFiberFail)
                                {
                                    <span class="badge bg-secondary me-1"><i class="bi bi-emoji-dizzy"></i> Low Fiber</span>
                                }
                            </td>
                        </tr>
                    }

                    <tr class="table-warning fw-bold">
                        <td colspan="2" class="text-end">Monthly Total</td>
                        <td>@monthGroup.Sum(e => e.PortionEaten).ToString("N2")</td>
                        <td>@monthGroup.Sum(e => e.Calories).ToString("N2")</td>
                        <td>@monthGroup.Sum(e => e.Protein).ToString("N2")</td>
                        <td>@monthGroup.Sum(e => e.Carbs).ToString("N2")</td>
                        <td>@monthGroup.Sum(e => e.Fats).ToString("N2")</td>
                        <td>@monthGroup.Sum(e => e.Fiber).ToString("N2")</td>
                        <td></td>
                    </tr>
                }
            </tbody>
        }
    </table>
}

@code {
    private List<MealEntry>? entries;
    private HashSet<(int Year, int Month)> expandedMonths = new();

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthProvider.GetAuthenticationStateAsync();
        var userId = authState.User?.Identity?.Name;

        entries = await MealEntryService.GetAllAsync(userId!);
    }

    private void ToggleMonth(dynamic key)
    {
        var tupleKey = (Year: (int)key.Year, Month: (int)key.Month);

        if (!expandedMonths.Add(tupleKey))
            expandedMonths.Remove(tupleKey);
    }

    private bool IsExpanded(dynamic key)
    {
        var tupleKey = (Year: (int)key.Year, Month: (int)key.Month);
        return !expandedMonths.Contains(tupleKey);
    }
}

```

## File: MealTrackerTopGroupNames.razor

```C#
@page "/MealTrackerTopGroupNames"
@rendermode InteractiveServer
@using BulkCarnageIQ.Core.Carnage
@using BulkCarnageIQ.Core.Contracts
@inject IMealEntryService MealEntryService
@inject AuthenticationStateProvider AuthProvider

<h3><i class="bi bi-bar-chart-line"></i> Top Groups by Month</h3>

@if (entries is null)
{
    <p><em>Loading meals...</em></p>
}
else if (!entries.Any())
{
    <p>No meals tracked yet.</p>
}
else
{
    <table class="table table-hover table-bordered small align-middle">
        @foreach (var monthGroup in entries
       .GroupBy(e => new { e.Date.Year, e.Date.Month })
       .OrderByDescending(g => g.Key.Year)
       .ThenByDescending(g => g.Key.Month))
        {
            var year = monthGroup.Key.Year;
            var month = monthGroup.Key.Month;
            var monthName = new DateTime(year, month, 1).ToString("MMMM yyyy");

            <tbody class="table-group-divider">
                <tr class="table-secondary fw-bold" @onclick="@(() => ToggleMonth(monthGroup.Key))" style="cursor: pointer;">
                    <td colspan="8">
                        <i class="bi @(IsExpanded(monthGroup.Key) ? "bi-chevron-up" : "bi-chevron-down") me-2"></i>
                        @monthName
                    </td>
                </tr>

                @if (IsExpanded(monthGroup.Key))
                {
                    var groupGroups = monthGroup
                    .GroupBy(m => m.GroupName ?? "Unknown")
                    .Select(g => new
                    {
                        GroupName = String.Format("{0} ({1} items)", g.Key, g.Count()),
                        TotalPortion = g.Sum(m => m.PortionEaten),
                        TotalCalories = g.Sum(m => m.Calories),
                        TotalProtein = g.Sum(m => m.Protein),
                        TotalCarbs = g.Sum(m => m.Carbs),
                        TotalFats = g.Sum(m => m.Fats),
                        TotalFiber = g.Sum(m => m.Fiber)
                    })
                    .OrderByDescending(f => f.TotalCalories)
                    .ToList();

                    <tr class="fw-bold table-info">
                        <td>Group Name</td>
                        <th>Total Portion</th>
                        <th>Total Calories</th>
                        <th>Total Protein</th>
                        <th>Total Carbs</th>
                        <th>Total Fats</th>
                        <th>Total Fiber</th>
                        <th>Flags</th>
                    </tr>

                    @foreach (var group in groupGroups)
                    {
                        var caloriePerPortion = group.TotalCalories / (group.TotalPortion == 0 ? 1 : group.TotalPortion);
                        var fatToCarbRatio = group.TotalFats / (group.TotalCarbs == 0 ? 1 : group.TotalCarbs);
                        var isFatBomb = group.TotalFats > 150 && fatToCarbRatio > 1.5;
                        var isCarbBomb = group.TotalCarbs > 200 && group.TotalFiber < 10;
                        var isCalorieBomb = caloriePerPortion > 400;
                        var isFiberFail = group.TotalFiber < 5;

                        var showLowFiber = isFatBomb || isCarbBomb || isCalorieBomb;

                        <tr>
                            <td>@group.GroupName</td>
                            <td>@group.TotalPortion.ToString("N2")</td>
                            <td>@group.TotalCalories.ToString("N2")</td>
                            <td>@group.TotalProtein.ToString("N2")</td>
                            <td>@group.TotalCarbs.ToString("N2")</td>
                            <td>@group.TotalFats.ToString("N2")</td>
                            <td>@group.TotalFiber.ToString("N2")</td>
                            <td>
                                @if (isFatBomb)
                                {
                                    <span class="badge bg-danger me-1"><i class="bi bi-droplet-fill"></i> Fat Bomb</span>
                                }
                                @if (isCarbBomb)
                                {
                                    <span class="badge bg-warning text-dark me-1"><i class="bi bi-cup-straw"></i> Carb Bomb</span>
                                }
                                @if (isCalorieBomb)
                                {
                                    <span class="badge bg-danger me-1"><i class="bi bi-fire"></i> Calorie Bomb</span>
                                }
                                @if (showLowFiber && isFiberFail)
                                {
                                    <span class="badge bg-secondary me-1"><i class="bi bi-emoji-dizzy"></i> Low Fiber</span>
                                }
                            </td>
                        </tr>
                    }

                    <tr class="table-warning fw-bold">
                        <td class="text-end">Monthly Total</td>
                        <td>@monthGroup.Sum(e => e.PortionEaten).ToString("N2")</td>
                        <td>@monthGroup.Sum(e => e.Calories).ToString("N2")</td>
                        <td>@monthGroup.Sum(e => e.Protein).ToString("N2")</td>
                        <td>@monthGroup.Sum(e => e.Carbs).ToString("N2")</td>
                        <td>@monthGroup.Sum(e => e.Fats).ToString("N2")</td>
                        <td>@monthGroup.Sum(e => e.Fiber).ToString("N2")</td>
                        <td></td>
                    </tr>
                }
            </tbody>
        }
    </table>
}

@code {
    private List<MealEntry>? entries;
    private HashSet<(int Year, int Month)> expandedMonths = new();

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthProvider.GetAuthenticationStateAsync();
        var userId = authState.User?.Identity?.Name;

        entries = await MealEntryService.GetAllAsync(userId!);
    }

    private void ToggleMonth(dynamic key)
    {
        var tupleKey = (Year: (int)key.Year, Month: (int)key.Month);

        if (!expandedMonths.Add(tupleKey))
            expandedMonths.Remove(tupleKey);
    }

    private bool IsExpanded(dynamic key)
    {
        var tupleKey = (Year: (int)key.Year, Month: (int)key.Month);
        return !expandedMonths.Contains(tupleKey);
    }
}

```

## File: UserProfilePage.razor

```C#
@page "/UserProfile"
@rendermode InteractiveServer
@using BulkCarnageIQ.Core.Carnage
@using BulkCarnageIQ.Core.Carnage.Enums
@using BulkCarnageIQ.Core.Contracts
@inject IUserProfileService UserProfileService
@inject AuthenticationStateProvider AuthProvider

<h3><i class="bi bi-person-circle"></i> My Profile</h3>

@if (isLoading)
{
    <p><em>Loading profile...</em></p>
}
else if (userProfile is null)
{
    <p class="text-danger"><i class="bi bi-exclamation-circle me-2"></i> No profile found.</p>
}
else
{
    <EditForm Model="userProfile" OnValidSubmit="HandleSaveProfile">
        <DataAnnotationsValidator />
        <ValidationSummary />

        <div class="row mb-3">
            <div class="col-md-6">
                <div class="card shadow-sm mb-3">
                    <div class="card-header bg-primary text-white">
                        <i class="bi bi-person-lines-fill me-2"></i> Personal Info
                    </div>
                    <div class="card-body">
                        <div class="form-group mb-3">
                            <label for="userName" class="form-label">User Name</label>
                            <input type="text" id="userName" class="form-control" @bind="userProfile.UserName" disabled />
                        </div>

                        <div class="form-group mb-3">
                            <label for="age" class="form-label">Age</label>
                            <input type="number" id="age" class="form-control" @bind="userProfile.Age" min="1" />
                        </div>

                        <div class="form-group mb-3">
                            <label for="sex" class="form-label">Sex</label>
                            <select id="sex" class="form-select" @bind="userProfile.Sex">
                                @foreach (var sexOption in Enum.GetValues<Sex>())
                                {
                                    <option value="@sexOption">@sexOption</option>
                                }
                            </select>
                        </div>

                        <div class="form-group mb-3">
                            <label for="height" class="form-label">Height (in inches)</label>
                            <input type="number" id="height" class="form-control" @bind="userProfile.HeightInches" min="1" />
                        </div>

                        <div class="form-group mb-3">
                            <label for="weight" class="form-label">Weight (in pounds)</label>
                            <input type="number" id="weight" class="form-control" @bind="userProfile.WeightPounds" min="1" />
                        </div>

                        <div class="form-group mb-3">
                            <label for="activityLevel" class="form-label">Activity Level</label>
                            <select id="activityLevel" class="form-select" @bind="userProfile.ActivityLevel">
                                <option value="sedentary">Sedentary</option>
                                <option value="lightly active">Lightly Active</option>
                                <option value="moderately active">Moderately Active</option>
                                <option value="very active">Very Active</option>
                                <option value="super active">Super Active</option>
                            </select>
                        </div>

                        <div class="form-group mb-3">
                            <label for="goalType" class="form-label">Goal Type</label>
                            <select id="goalType" class="form-select" @bind="userProfile.GoalType">
                                <option value="maintain">Maintain weight</option>
                                <option value="lose0.5">Lose 0.5 lb/week (~250 cal deficit)</option>
                                <option value="lose1">Lose 1 lb/week (~500 cal deficit)</option>
                                <option value="lose2">Lose 2 lb/week (~1000 cal deficit)</option>
                                <option value="gain0.5">Gain 0.5 lb/week (~250 cal surplus)</option>
                                <option value="gain1">Gain 1 lb/week (~500 cal surplus)</option>
                            </select>
                        </div>
                    </div>
                </div>
            </div>

            <div class="col-md-6">
                <div class="card shadow-sm">
                    <div class="card-header bg-success text-white">
                        <i class="bi bi-graph-up-arrow me-2"></i> Nutrition Goals
                    </div>
                    <div class="card-body">
                        <div class="form-group mb-3">
                            <label for="calories" class="form-label">Calories</label>
                            <input type="number" id="calories" class="form-control" @bind="userProfile.CalorieGoal" disabled />
                        </div>

                        <div class="form-group mb-3">
                            <label for="protein" class="form-label">Protein (g)</label>
                            <input type="number" id="protein" class="form-control" @bind="userProfile.ProteinGoal" disabled />
                        </div>

                        <div class="form-group mb-3">
                            <label for="carbs" class="form-label">Carbs (g)</label>
                            <input type="number" id="carbs" class="form-control" @bind="userProfile.CarbsGoal" disabled />
                        </div>

                        <div class="form-group mb-3">
                            <label for="fat" class="form-label">Fats (g)</label>
                            <input type="number" id="fat" class="form-control" @bind="userProfile.FatGoal" disabled />
                        </div>

                        <div class="form-group mb-3">
                            <label for="fiber" class="form-label">Fiber (g)</label>
                            <input type="number" id="fiber" class="form-control" @bind="userProfile.FiberGoal" disabled />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <button type="submit" class="btn btn-primary">Save Changes</button>
    </EditForm>
}

@code {
    private UserProfile? userProfile;
    private string currentUserName = string.Empty;
    private bool isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthProvider.GetAuthenticationStateAsync();
        currentUserName = authState.User?.Identity?.Name ?? string.Empty;

        if (!string.IsNullOrEmpty(currentUserName))
        {
            userProfile = await UserProfileService.GetUserProfile(currentUserName);

            // Optional: if goals are not calculated yet (0), calculate them
            if (userProfile is not null && userProfile.CalorieGoal == 0)
            {
                userProfile.CalculateGoals();
            }
        }

        isLoading = false;
    }

    private async Task HandleSaveProfile()
    {
        // Recalculate goals when profile is saved
        userProfile?.CalculateGoals();

        // Save the updated profile
        if (userProfile != null)
        {
            await UserProfileService.SaveUserProfile(userProfile);
        }
    }
}

```

## File: WeightTracker.razor

```C#
@page "/WeightTracker"
@rendermode InteractiveServer
@using BulkCarnageIQ.Core.Carnage
@using BulkCarnageIQ.Core.Contracts
@inject IWeightLogService WeightService
@inject NavigationManager Nav
@inject AuthenticationStateProvider AuthStateProvider

<PageTitle>Weight Tracker</PageTitle>

<h3>Weight Tracker</h3>

<EditForm Model="@weightEntry" OnValidSubmit="SaveWeight">
    <div class="row mb-3">
        <div class="col-md-6">
            <label class="form-label">Date</label>
            <InputDate @bind-Value="weightEntry.Date" class="form-control" />
        </div>

        <div class="col-md-6">
            <label class="form-label">Weight (lbs)</label>
            <InputNumber @bind-Value="weightEntry.WeightLbs" class="form-control" />
        </div>
    </div>

    <button type="submit" class="btn btn-primary mb-3">Save</button>
</EditForm>

@if (weightLogs.Any())
{
    <div class="card shadow-sm mb-4">
        <div class="card-header bg-primary text-white d-flex align-items-center">
            <i class="bi bi-graph-up me-2"></i> Weight Over Time
        </div>
        <div class="card-body">
            <BulkCarnageIQ.Web.Components.Charts.LineChart Title="Weight Over Time"
                Labels="@weightLogs.Select(w => w.Date.ToShortDateString()).ToList()"
                Data="@weightLogs.Select(w => w.WeightLbs).ToList()"
                DataLabel="Weight (lbs)"
                LineColor="rgba(54, 162, 235, 1)"
                xLabel="Date"
                yLabel="Weight (lbs)" />
        </div>
    </div>

    <h5 class="mt-4">Past Weights</h5>
    <table class="table table-sm table-bordered">
        <thead>
            <tr>
                <th>Date</th>
                <th>Weight (lbs)</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var log in weightLogs.OrderByDescending(w => w.Date))
            {
                <tr>
                    <td>@log.Date.ToShortDateString()</td>
                    <td>@log.WeightLbs.ToString("N2")</td>
                </tr>
            }
        </tbody>
    </table>
}

@code {
    private List<WeightLog> weightLogs = new();
    private WeightLog weightEntry = new WeightLog() { Date = DateOnly.FromDateTime(DateTime.Today) };

    protected override async Task OnInitializedAsync()
    {
        var userId = await GetUserIdAsync();

        weightLogs = await WeightService.GetUserLogsAsync(userId, true);

        // Pre-fill today's weight if exists
        var todayLog = weightLogs.FirstOrDefault(w => w.Date == DateOnly.FromDateTime(DateTime.Today));
        if (todayLog != null)
        {
            weightEntry.WeightLbs = todayLog.WeightLbs;
        }
    }

    private async Task SaveWeight()
    {
        var userId = await GetUserIdAsync();
        await WeightService.AddOrUpdateLogAsync(userId, weightEntry.Date, weightEntry.WeightLbs);
        Nav.NavigateTo(Nav.Uri, forceLoad: true);
    }

    private async Task<string> GetUserIdAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var userId = authState.User?.Identity?.Name;
        return userId!;
    }
}

```

