# Directory: Components\Account\Shared

## File: ExternalLoginPicker.razor

```C#
@using Microsoft.AspNetCore.Authentication
@using Microsoft.AspNetCore.Identity
@using BulkCarnageIQ.Core.Identity

@inject SignInManager<ApplicationUser> SignInManager
@inject IdentityRedirectManager RedirectManager

@if (externalLogins.Length == 0)
{
    <div>
        <p>
            There are no external authentication services configured. See this <a href="https://go.microsoft.com/fwlink/?LinkID=532715">article
            about setting up this ASP.NET application to support logging in via external services</a>.
        </p>
    </div>
}
else
{
    <form class="form-horizontal" action="Account/PerformExternalLogin" method="post">
        <div>
            <AntiforgeryToken />
            <input type="hidden" name="ReturnUrl" value="@ReturnUrl" />
            <p>
                @foreach (var provider in externalLogins)
                {
                    <button type="submit" class="btn btn-primary" name="provider" value="@provider.Name" title="Log in using your @provider.DisplayName account">@provider.DisplayName</button>
                }
            </p>
        </div>
    </form>
}

@code {
    private AuthenticationScheme[] externalLogins = [];

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    protected override async Task OnInitializedAsync()
    {
        externalLogins = (await SignInManager.GetExternalAuthenticationSchemesAsync()).ToArray();
    }
}

```

## File: ManageLayout.razor

```C#
@inherits LayoutComponentBase
@layout BulkCarnageIQ.Web.Components.Layout.MainLayout

<h1>Manage your account</h1>

<div>
    <h2>Change your account settings</h2>
    <hr />
    <div class="row">
        <div class="col-lg-3">
            <ManageNavMenu />
        </div>
        <div class="col-lg-9">
            @Body
        </div>
    </div>
</div>

```

## File: ManageNavMenu.razor

```C#
@using Microsoft.AspNetCore.Identity
@using BulkCarnageIQ.Core.Identity

@inject SignInManager<ApplicationUser> SignInManager

<ul class="nav nav-pills flex-column">
    <li class="nav-item">
        <NavLink class="nav-link" href="Account/Manage" Match="NavLinkMatch.All">Profile</NavLink>
    </li>
    <li class="nav-item">
        <NavLink class="nav-link" href="Account/Manage/Email">Email</NavLink>
    </li>
    <li class="nav-item">
        <NavLink class="nav-link" href="Account/Manage/ChangePassword">Password</NavLink>
    </li>
    @if (hasExternalLogins)
    {
        <li class="nav-item">
            <NavLink class="nav-link" href="Account/Manage/ExternalLogins">External logins</NavLink>
        </li>
    }
    <li class="nav-item">
        <NavLink class="nav-link" href="Account/Manage/TwoFactorAuthentication">Two-factor authentication</NavLink>
    </li>
    <li class="nav-item">
        <NavLink class="nav-link" href="Account/Manage/PersonalData">Personal data</NavLink>
    </li>
</ul>

@code {
    private bool hasExternalLogins;

    protected override async Task OnInitializedAsync()
    {
        hasExternalLogins = (await SignInManager.GetExternalAuthenticationSchemesAsync()).Any();
    }
}

```

## File: RedirectToLogin.razor

```C#
@inject NavigationManager NavigationManager

@code {
    protected override void OnInitialized()
    {
        NavigationManager.NavigateTo($"Account/Login?returnUrl={Uri.EscapeDataString(NavigationManager.Uri)}", forceLoad: true);
    }
}

```

## File: ShowRecoveryCodes.razor

```C#
<StatusMessage Message="@StatusMessage" />
<h3>Recovery codes</h3>
<div class="alert alert-warning" role="alert">
    <p>
        <strong>Put these codes in a safe place.</strong>
    </p>
    <p>
        If you lose your device and don't have the recovery codes you will lose access to your account.
    </p>
</div>
<div class="row">
    <div class="col-md-12">
        @foreach (var recoveryCode in RecoveryCodes)
        {
            <div>
                <code class="recovery-code">@recoveryCode</code>
            </div>
        }
    </div>
</div>

@code {
    [Parameter]
    public string[] RecoveryCodes { get; set; } = [];

    [Parameter]
    public string? StatusMessage { get; set; }
}

```

## File: StatusMessage.razor

```C#
@if (!string.IsNullOrEmpty(DisplayMessage))
{
    var statusMessageClass = DisplayMessage.StartsWith("Error") ? "danger" : "success";
    <div class="alert alert-@statusMessageClass" role="alert">
        @DisplayMessage
    </div>
}

@code {
    private string? messageFromCookie;

    [Parameter]
    public string? Message { get; set; }

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    private string? DisplayMessage => Message ?? messageFromCookie;

    protected override void OnInitialized()
    {
        messageFromCookie = HttpContext.Request.Cookies[IdentityRedirectManager.StatusCookieName];

        if (messageFromCookie is not null)
        {
            HttpContext.Response.Cookies.Delete(IdentityRedirectManager.StatusCookieName);
        }
    }
}

```

