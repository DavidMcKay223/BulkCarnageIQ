# Directory: Components\Layout

## File: MainLayout.razor

```C#
@inherits LayoutComponentBase

<div class="page">
    <div class="sidebar">
        <NavMenu />
    </div>

    <main>
        <div class="top-row px-4">
            <AuthorizeView>
                <Authorized>
                    <div class="nav-item px-3">
                        <form action="Account/Logout" method="post">
                            <AntiforgeryToken />
                            <input type="hidden" name="ReturnUrl" value="" />
                            <button type="submit" class="nav-link">
                                <span class="bi bi-arrow-bar-left-nav-menu" aria-hidden="true"></span> Logout
                            </button>
                        </form>
                    </div>
                </Authorized>
                <NotAuthorized>
                    <div class="nav-item px-3">
                        <NavLink class="nav-link" href="Account/Register">
                            <span class="bi bi-person-nav-menu" aria-hidden="true"></span> Register
                        </NavLink>
                    </div>
                    <div class="nav-item px-3">
                        <NavLink class="nav-link" href="Account/Login">
                            <span class="bi bi-person-badge-nav-menu" aria-hidden="true"></span> Login
                        </NavLink>
                    </div>
                </NotAuthorized>
            </AuthorizeView>
        </div>

        <article class="content px-4">
            @Body
        </article>
    </main>
</div>

<div id="blazor-error-ui" data-nosnippet>
    An unhandled error has occurred.
    <a href="." class="reload">Reload</a>
    <span class="dismiss">🗙</span>
</div>

```

## File: NavMenu.razor

```C#
@implements IDisposable

@inject NavigationManager NavigationManager

<div class="top-row ps-3 navbar navbar-dark">
    <div class="container-fluid">
        <a class="navbar-brand" href="">BulkCarnageIQ.Web</a>
    </div>
</div>

<input type="checkbox" title="Navigation menu" class="navbar-toggler" />

<div class="nav-scrollable" onclick="document.querySelector('.navbar-toggler').click()">
    <nav class="nav flex-column">
        <div class="nav-item px-3">
            <NavLink class="nav-link d-flex align-items-center gap-2 py-2" href="" Match="NavLinkMatch.All">
                <i class="bi bi-house-door-fill fs-5 me-2 d-inline-flex align-items-center justify-content-center"></i>
                <span>Home</span>
            </NavLink>
        </div>

        <div class="nav-item px-3">
            <NavLink class="nav-link d-flex align-items-center gap-2 py-2" href="/MealTracker">
                <i class="bi bi-clipboard-data fs-5 me-2 d-inline-flex align-items-center justify-content-center"></i>
                <span>Meal Tracker</span>
            </NavLink>
            <div class="collapse show" id="foodTrackerMenu">
                <div class="nav-item px-3 ms-3">
                    <NavLink class="nav-link d-flex align-items-center gap-2 py-2" href="/MealTrackerReport">
                        <i class="bi bi-graph-up fs-5 me-2 d-inline-flex align-items-center justify-content-center"></i>
                        Meal Report
                    </NavLink>
                </div>
                <div class="nav-item px-3 ms-3">
                    <NavLink class="nav-link d-flex align-items-center gap-2 py-2" href="/MealTrackerTopFoods">
                        <i class="bi bi-graph-up fs-5 me-2 d-inline-flex align-items-center justify-content-center"></i>
                        Top Foods
                    </NavLink>
                </div>
                <div class="nav-item px-3 ms-3">
                    <NavLink class="nav-link d-flex align-items-center gap-2 py-2" href="/MealTrackerTopGroupNames">
                        <i class="bi bi-graph-up fs-5 me-2 d-inline-flex align-items-center justify-content-center"></i>
                        Top Groups
                    </NavLink>
                </div>
                <div class="nav-item px-3 ms-3">
                    <NavLink class="nav-link d-flex align-items-center gap-2 py-2" href="/MealTrackerReportMacro">
                        <i class="bi bi-graph-up fs-5 me-2 d-inline-flex align-items-center justify-content-center"></i>
                        Report Macro
                    </NavLink>
                </div>
            </div>
        </div>

        <div class="nav-item px-3">
            <NavLink class="nav-link d-flex align-items-center gap-2 py-2" href="/GroceryList">
                <i class="bi bi-basket-fill fs-5 me-2 d-inline-flex align-items-center justify-content-center"></i>
                <span>Grocery List</span>
            </NavLink>
        </div>

        <div class="nav-item px-3">
            <NavLink class="nav-link d-flex align-items-center gap-2 py-2" href="/FoodList">
                <i class="bi bi-list-ul fs-5 me-2 d-inline-flex align-items-center justify-content-center"></i>
                <span>Food List</span>
            </NavLink>
        </div>

        <div class="nav-item px-3">
            <NavLink class="nav-link d-flex align-items-center gap-2 py-2" href="/UserProfile">
                <i class="bi bi-person-circle fs-5 me-2 d-inline-flex align-items-center justify-content-center"></i>
                <span>User Profile</span>
            </NavLink>
            <div class="collapse show" id="userProfileMenu">
                <div class="nav-item px-3 ms-3">
                    <NavLink class="nav-link d-flex align-items-center gap-2 py-2" href="/WeightTracker">
                        <i class="bi bi-graph-up fs-5 me-2 d-inline-flex align-items-center justify-content-center"></i>
                        Weight
                    </NavLink>
                </div>
            </div>
        </div>
    </nav>
</div>

@code {
    private string? currentUrl;

    protected override void OnInitialized()
    {
        currentUrl = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
        NavigationManager.LocationChanged += OnLocationChanged;
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        currentUrl = NavigationManager.ToBaseRelativePath(e.Location);
        StateHasChanged();
    }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= OnLocationChanged;
    }
}

```

