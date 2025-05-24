# Directory: Components\Account\Pages\Manage

## File: _Imports.razor

```C#
@layout ManageLayout
@attribute [Microsoft.AspNetCore.Authorization.Authorize]

```

## File: ChangePassword.razor

```C#
@page "/Account/Manage/ChangePassword"

@using System.ComponentModel.DataAnnotations
@using Microsoft.AspNetCore.Identity
@using BulkCarnageIQ.Core.Identity

@inject UserManager<ApplicationUser> UserManager
@inject SignInManager<ApplicationUser> SignInManager
@inject IdentityUserAccessor UserAccessor
@inject IdentityRedirectManager RedirectManager
@inject ILogger<ChangePassword> Logger

<PageTitle>Change password</PageTitle>

<h3>Change password</h3>
<StatusMessage Message="@message" />
<div class="row">
    <div class="col-xl-6">
        <EditForm Model="Input" FormName="change-password" OnValidSubmit="OnValidSubmitAsync" method="post">
            <DataAnnotationsValidator />
            <ValidationSummary class="text-danger" role="alert" />
            <div class="form-floating mb-3">
                <InputText type="password" @bind-Value="Input.OldPassword" id="Input.OldPassword" class="form-control" autocomplete="current-password" aria-required="true" placeholder="Enter the old password" />
                <label for="Input.OldPassword" class="form-label">Old password</label>
                <ValidationMessage For="() => Input.OldPassword" class="text-danger" />
            </div>
            <div class="form-floating mb-3">
                <InputText type="password" @bind-Value="Input.NewPassword" id="Input.NewPassword" class="form-control" autocomplete="new-password" aria-required="true" placeholder="Enter the new password" />
                <label for="Input.NewPassword" class="form-label">New password</label>
                <ValidationMessage For="() => Input.NewPassword" class="text-danger" />
            </div>
            <div class="form-floating mb-3">
                <InputText type="password" @bind-Value="Input.ConfirmPassword" id="Input.ConfirmPassword" class="form-control" autocomplete="new-password" aria-required="true" placeholder="Enter the new password" />
                <label for="Input.ConfirmPassword" class="form-label">Confirm password</label>
                <ValidationMessage For="() => Input.ConfirmPassword" class="text-danger" />
            </div>
            <button type="submit" class="w-100 btn btn-lg btn-primary">Update password</button>
        </EditForm>
    </div>
</div>

@code {
    private string? message;
    private ApplicationUser user = default!;
    private bool hasPassword;

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        user = await UserAccessor.GetRequiredUserAsync(HttpContext);
        hasPassword = await UserManager.HasPasswordAsync(user);
        if (!hasPassword)
        {
            RedirectManager.RedirectTo("Account/Manage/SetPassword");
        }
    }

    private async Task OnValidSubmitAsync()
    {
        var changePasswordResult = await UserManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
        if (!changePasswordResult.Succeeded)
        {
            message = $"Error: {string.Join(",", changePasswordResult.Errors.Select(error => error.Description))}";
            return;
        }

        await SignInManager.RefreshSignInAsync(user);
        Logger.LogInformation("User changed their password successfully.");

        RedirectManager.RedirectToCurrentPageWithStatus("Your password has been changed", HttpContext);
    }

    private sealed class InputModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; } = "";

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = "";
    }
}

```

## File: DeletePersonalData.razor

```C#
@page "/Account/Manage/DeletePersonalData"

@using System.ComponentModel.DataAnnotations
@using Microsoft.AspNetCore.Identity
@using BulkCarnageIQ.Core.Identity

@inject UserManager<ApplicationUser> UserManager
@inject SignInManager<ApplicationUser> SignInManager
@inject IdentityUserAccessor UserAccessor
@inject IdentityRedirectManager RedirectManager
@inject ILogger<DeletePersonalData> Logger

<PageTitle>Delete Personal Data</PageTitle>

<StatusMessage Message="@message" />

<h3>Delete Personal Data</h3>

<div class="alert alert-warning" role="alert">
    <p>
        <strong>Deleting this data will permanently remove your account, and this cannot be recovered.</strong>
    </p>
</div>

<div>
    <EditForm Model="Input" FormName="delete-user" OnValidSubmit="OnValidSubmitAsync" method="post">
        <DataAnnotationsValidator />
        <ValidationSummary class="text-danger" role="alert" />
        @if (requirePassword)
        {
            <div class="form-floating mb-3">
                <InputText type="password" @bind-Value="Input.Password" id="Input.Password" class="form-control" autocomplete="current-password" aria-required="true" placeholder="Please enter your password." />
                <label for="Input.Password" class="form-label">Password</label>
                <ValidationMessage For="() => Input.Password" class="text-danger" />
            </div>
        }
        <button class="w-100 btn btn-lg btn-danger" type="submit">Delete data and close my account</button>
    </EditForm>
</div>

@code {
    private string? message;
    private ApplicationUser user = default!;
    private bool requirePassword;

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        Input ??= new();
        user = await UserAccessor.GetRequiredUserAsync(HttpContext);
        requirePassword = await UserManager.HasPasswordAsync(user);
    }

    private async Task OnValidSubmitAsync()
    {
        if (requirePassword && !await UserManager.CheckPasswordAsync(user, Input.Password))
        {
            message = "Error: Incorrect password.";
            return;
        }

        var result = await UserManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException("Unexpected error occurred deleting user.");
        }

        await SignInManager.SignOutAsync();

        var userId = await UserManager.GetUserIdAsync(user);
        Logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

        RedirectManager.RedirectToCurrentPage();
    }

    private sealed class InputModel
    {
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";
    }
}

```

## File: Disable2fa.razor

```C#
@page "/Account/Manage/Disable2fa"

@using Microsoft.AspNetCore.Identity
@using BulkCarnageIQ.Core.Identity

@inject UserManager<ApplicationUser> UserManager
@inject IdentityUserAccessor UserAccessor
@inject IdentityRedirectManager RedirectManager
@inject ILogger<Disable2fa> Logger

<PageTitle>Disable two-factor authentication (2FA)</PageTitle>

<StatusMessage />
<h3>Disable two-factor authentication (2FA)</h3>

<div class="alert alert-warning" role="alert">
    <p>
        <strong>This action only disables 2FA.</strong>
    </p>
    <p>
        Disabling 2FA does not change the keys used in authenticator apps. If you wish to change the key
        used in an authenticator app you should <a href="Account/Manage/ResetAuthenticator">reset your authenticator keys.</a>
    </p>
</div>

<div>
    <form @formname="disable-2fa" @onsubmit="OnSubmitAsync" method="post">
        <AntiforgeryToken />
        <button class="btn btn-danger" type="submit">Disable 2FA</button>
    </form>
</div>

@code {
    private ApplicationUser user = default!;

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        user = await UserAccessor.GetRequiredUserAsync(HttpContext);

        if (HttpMethods.IsGet(HttpContext.Request.Method) && !await UserManager.GetTwoFactorEnabledAsync(user))
        {
            throw new InvalidOperationException("Cannot disable 2FA for user as it's not currently enabled.");
        }
    }

    private async Task OnSubmitAsync()
    {
        var disable2faResult = await UserManager.SetTwoFactorEnabledAsync(user, false);
        if (!disable2faResult.Succeeded)
        {
            throw new InvalidOperationException("Unexpected error occurred disabling 2FA.");
        }

        var userId = await UserManager.GetUserIdAsync(user);
        Logger.LogInformation("User with ID '{UserId}' has disabled 2fa.", userId);
        RedirectManager.RedirectToWithStatus(
            "Account/Manage/TwoFactorAuthentication",
            "2fa has been disabled. You can reenable 2fa when you setup an authenticator app",
            HttpContext);
    }
}

```

## File: Email.razor

```C#
@page "/Account/Manage/Email"

@using System.ComponentModel.DataAnnotations
@using System.Text
@using System.Text.Encodings.Web
@using Microsoft.AspNetCore.Identity
@using Microsoft.AspNetCore.WebUtilities
@using BulkCarnageIQ.Core.Identity

@inject UserManager<ApplicationUser> UserManager
@inject IEmailSender<ApplicationUser> EmailSender
@inject IdentityUserAccessor UserAccessor
@inject NavigationManager NavigationManager

<PageTitle>Manage email</PageTitle>

<h3>Manage email</h3>

<StatusMessage Message="@message"/>
<div class="row">
    <div class="col-xl-6">
        <form @onsubmit="OnSendEmailVerificationAsync" @formname="send-verification" id="send-verification-form" method="post">
            <AntiforgeryToken />
        </form>
        <EditForm Model="Input" FormName="change-email" OnValidSubmit="OnValidSubmitAsync" method="post">
            <DataAnnotationsValidator />
            <ValidationSummary class="text-danger" role="alert" />
            @if (isEmailConfirmed)
            {
                <div class="form-floating mb-3 input-group">
                    <input type="text" value="@email" id="email" class="form-control" placeholder="Enter your email" disabled />
                    <div class="input-group-append">
                        <span class="h-100 input-group-text text-success font-weight-bold">✓</span>
                    </div>
                    <label for="email" class="form-label">Email</label>
                </div>
            }
            else
            {
                <div class="form-floating mb-3">
                    <input type="text" value="@email" id="email" class="form-control" placeholder="Enter your email" disabled />
                    <label for="email" class="form-label">Email</label>
                    <button type="submit" class="btn btn-link" form="send-verification-form">Send verification email</button>
                </div>
            }
            <div class="form-floating mb-3">
                <InputText @bind-Value="Input.NewEmail" id="Input.NewEmail" class="form-control" autocomplete="email" aria-required="true" placeholder="Enter a new email" />
                <label for="Input.NewEmail" class="form-label">New email</label>
                <ValidationMessage For="() => Input.NewEmail" class="text-danger" />
            </div>
            <button type="submit" class="w-100 btn btn-lg btn-primary">Change email</button>
        </EditForm>
    </div>
</div>

@code {
    private string? message;
    private ApplicationUser user = default!;
    private string? email;
    private bool isEmailConfirmed;

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    [SupplyParameterFromForm(FormName = "change-email")]
    private InputModel Input { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        user = await UserAccessor.GetRequiredUserAsync(HttpContext);
        email = await UserManager.GetEmailAsync(user);
        isEmailConfirmed = await UserManager.IsEmailConfirmedAsync(user);

        Input.NewEmail ??= email;
    }

    private async Task OnValidSubmitAsync()
    {
        if (Input.NewEmail is null || Input.NewEmail == email)
        {
            message = "Your email is unchanged.";
            return;
        }

        var userId = await UserManager.GetUserIdAsync(user);
        var code = await UserManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = NavigationManager.GetUriWithQueryParameters(
            NavigationManager.ToAbsoluteUri("Account/ConfirmEmailChange").AbsoluteUri,
            new Dictionary<string, object?> { ["userId"] = userId, ["email"] = Input.NewEmail, ["code"] = code });

        await EmailSender.SendConfirmationLinkAsync(user, Input.NewEmail, HtmlEncoder.Default.Encode(callbackUrl));

        message = "Confirmation link to change email sent. Please check your email.";
    }

    private async Task OnSendEmailVerificationAsync()
    {
        if (email is null)
        {
            return;
        }

        var userId = await UserManager.GetUserIdAsync(user);
        var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = NavigationManager.GetUriWithQueryParameters(
            NavigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri,
            new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code });

        await EmailSender.SendConfirmationLinkAsync(user, email, HtmlEncoder.Default.Encode(callbackUrl));

        message = "Verification email sent. Please check your email.";
    }

    private sealed class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "New email")]
        public string? NewEmail { get; set; }
    }
}

```

## File: EnableAuthenticator.razor

```C#
@page "/Account/Manage/EnableAuthenticator"

@using System.ComponentModel.DataAnnotations
@using System.Globalization
@using System.Text
@using System.Text.Encodings.Web
@using Microsoft.AspNetCore.Identity
@using BulkCarnageIQ.Core.Identity

@inject UserManager<ApplicationUser> UserManager
@inject IdentityUserAccessor UserAccessor
@inject UrlEncoder UrlEncoder
@inject IdentityRedirectManager RedirectManager
@inject ILogger<EnableAuthenticator> Logger

<PageTitle>Configure authenticator app</PageTitle>

@if (recoveryCodes is not null)
{
    <ShowRecoveryCodes RecoveryCodes="recoveryCodes.ToArray()" StatusMessage="@message" />
}
else
{
    <StatusMessage Message="@message" />
    <h3>Configure authenticator app</h3>
    <div>
        <p>To use an authenticator app go through the following steps:</p>
        <ol class="list">
            <li>
                <p>
                    Download a two-factor authenticator app like Microsoft Authenticator for
                    <a href="https://go.microsoft.com/fwlink/?Linkid=825072">Android</a> and
                    <a href="https://go.microsoft.com/fwlink/?Linkid=825073">iOS</a> or
                    Google Authenticator for
                    <a href="https://play.google.com/store/apps/details?id=com.google.android.apps.authenticator2&amp;hl=en">Android</a> and
                    <a href="https://itunes.apple.com/us/app/google-authenticator/id388497605?mt=8">iOS</a>.
                </p>
            </li>
            <li>
                <p>Scan the QR Code or enter this key <kbd>@sharedKey</kbd> into your two factor authenticator app. Spaces and casing do not matter.</p>
                <div class="alert alert-info">Learn how to <a href="https://go.microsoft.com/fwlink/?Linkid=852423">enable QR code generation</a>.</div>
                <div></div>
                <div data-url="@authenticatorUri"></div>
            </li>
            <li>
                <p>
                    Once you have scanned the QR code or input the key above, your two factor authentication app will provide you
                    with a unique code. Enter the code in the confirmation box below.
                </p>
                <div class="row">
                    <div class="col-xl-6">
                        <EditForm Model="Input" FormName="send-code" OnValidSubmit="OnValidSubmitAsync" method="post">
                            <DataAnnotationsValidator />
                            <div class="form-floating mb-3">
                                <InputText @bind-Value="Input.Code" id="Input.Code" class="form-control" autocomplete="off" placeholder="Enter the code" />
                                <label for="Input.Code" class="control-label form-label">Verification Code</label>
                                <ValidationMessage For="() => Input.Code" class="text-danger" />
                            </div>
                            <button type="submit" class="w-100 btn btn-lg btn-primary">Verify</button>
                            <ValidationSummary class="text-danger" role="alert" />
                        </EditForm>
                    </div>
                </div>
            </li>
        </ol>
    </div>
}

@code {
    private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

    private string? message;
    private ApplicationUser user = default!;
    private string? sharedKey;
    private string? authenticatorUri;
    private IEnumerable<string>? recoveryCodes;

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        user = await UserAccessor.GetRequiredUserAsync(HttpContext);

        await LoadSharedKeyAndQrCodeUriAsync(user);
    }

    private async Task OnValidSubmitAsync()
    {
        // Strip spaces and hyphens
        var verificationCode = Input.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

        var is2faTokenValid = await UserManager.VerifyTwoFactorTokenAsync(
            user, UserManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

        if (!is2faTokenValid)
        {
            message = "Error: Verification code is invalid.";
            return;
        }

        await UserManager.SetTwoFactorEnabledAsync(user, true);
        var userId = await UserManager.GetUserIdAsync(user);
        Logger.LogInformation("User with ID '{UserId}' has enabled 2FA with an authenticator app.", userId);

        message = "Your authenticator app has been verified.";

        if (await UserManager.CountRecoveryCodesAsync(user) == 0)
        {
            recoveryCodes = await UserManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
        }
        else
        {
            RedirectManager.RedirectToWithStatus("Account/Manage/TwoFactorAuthentication", message, HttpContext);
        }
    }

    private async ValueTask LoadSharedKeyAndQrCodeUriAsync(ApplicationUser user)
    {
        // Load the authenticator key & QR code URI to display on the form
        var unformattedKey = await UserManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(unformattedKey))
        {
            await UserManager.ResetAuthenticatorKeyAsync(user);
            unformattedKey = await UserManager.GetAuthenticatorKeyAsync(user);
        }

        sharedKey = FormatKey(unformattedKey!);

        var email = await UserManager.GetEmailAsync(user);
        authenticatorUri = GenerateQrCodeUri(email!, unformattedKey!);
    }

    private string FormatKey(string unformattedKey)
    {
        var result = new StringBuilder();
        int currentPosition = 0;
        while (currentPosition + 4 < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
            currentPosition += 4;
        }
        if (currentPosition < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition));
        }

        return result.ToString().ToLowerInvariant();
    }

    private string GenerateQrCodeUri(string email, string unformattedKey)
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            AuthenticatorUriFormat,
            UrlEncoder.Encode("Microsoft.AspNetCore.Identity.UI"),
            UrlEncoder.Encode(email),
            unformattedKey);
    }

    private sealed class InputModel
    {
        [Required]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Verification Code")]
        public string Code { get; set; } = "";
    }
}

```

## File: ExternalLogins.razor

```C#
@page "/Account/Manage/ExternalLogins"

@using Microsoft.AspNetCore.Authentication
@using Microsoft.AspNetCore.Identity
@using BulkCarnageIQ.Core.Identity

@inject UserManager<ApplicationUser> UserManager
@inject SignInManager<ApplicationUser> SignInManager
@inject IdentityUserAccessor UserAccessor
@inject IUserStore<ApplicationUser> UserStore
@inject IdentityRedirectManager RedirectManager

<PageTitle>Manage your external logins</PageTitle>

<StatusMessage />
@if (currentLogins?.Count > 0)
{
    <h3>Registered Logins</h3>
    <table class="table">
        <tbody>
            @foreach (var login in currentLogins)
            {
                <tr>
                    <td>@login.ProviderDisplayName</td>
                    <td>
                        @if (showRemoveButton)
                        {
                            <form @formname="@($"remove-login-{login.LoginProvider}")" @onsubmit="OnSubmitAsync" method="post">
                                <AntiforgeryToken />
                                <div>
                                    <input type="hidden" name="@nameof(LoginProvider)" value="@login.LoginProvider" />
                                    <input type="hidden" name="@nameof(ProviderKey)" value="@login.ProviderKey" />
                                    <button type="submit" class="btn btn-primary" title="Remove this @login.ProviderDisplayName login from your account">Remove</button>
                                </div>
                            </form>
                        }
                        else
                        {
                            @: &nbsp;
                        }
                    </td>
                </tr>
            }
        </tbody>
    </table>
}
@if (otherLogins?.Count > 0)
{
    <h4>Add another service to log in.</h4>
    <hr />
    <form class="form-horizontal" action="Account/Manage/LinkExternalLogin" method="post">
        <AntiforgeryToken />
        <div>
            <p>
                @foreach (var provider in otherLogins)
                {
                    <button type="submit" class="btn btn-primary" name="Provider" value="@provider.Name" title="Log in using your @provider.DisplayName account">
                        @provider.DisplayName
                    </button>
                }
            </p>
        </div>
    </form>
}

@code {
    public const string LinkLoginCallbackAction = "LinkLoginCallback";

    private ApplicationUser user = default!;
    private IList<UserLoginInfo>? currentLogins;
    private IList<AuthenticationScheme>? otherLogins;
    private bool showRemoveButton;

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    [SupplyParameterFromForm]
    private string? LoginProvider { get; set; }

    [SupplyParameterFromForm]
    private string? ProviderKey { get; set; }

    [SupplyParameterFromQuery]
    private string? Action { get; set; }

    protected override async Task OnInitializedAsync()
    {
        user = await UserAccessor.GetRequiredUserAsync(HttpContext);
        currentLogins = await UserManager.GetLoginsAsync(user);
        otherLogins = (await SignInManager.GetExternalAuthenticationSchemesAsync())
            .Where(auth => currentLogins.All(ul => auth.Name != ul.LoginProvider))
            .ToList();

        string? passwordHash = null;
        if (UserStore is IUserPasswordStore<ApplicationUser> userPasswordStore)
        {
            passwordHash = await userPasswordStore.GetPasswordHashAsync(user, HttpContext.RequestAborted);
        }

        showRemoveButton = passwordHash is not null || currentLogins.Count > 1;

        if (HttpMethods.IsGet(HttpContext.Request.Method) && Action == LinkLoginCallbackAction)
        {
            await OnGetLinkLoginCallbackAsync();
        }
    }

    private async Task OnSubmitAsync()
    {
        var result = await UserManager.RemoveLoginAsync(user, LoginProvider!, ProviderKey!);
        if (!result.Succeeded)
        {
            RedirectManager.RedirectToCurrentPageWithStatus("Error: The external login was not removed.", HttpContext);
        }

        await SignInManager.RefreshSignInAsync(user);
        RedirectManager.RedirectToCurrentPageWithStatus("The external login was removed.", HttpContext);
    }

    private async Task OnGetLinkLoginCallbackAsync()
    {
        var userId = await UserManager.GetUserIdAsync(user);
        var info = await SignInManager.GetExternalLoginInfoAsync(userId);
        if (info is null)
        {
            RedirectManager.RedirectToCurrentPageWithStatus("Error: Could not load external login info.", HttpContext);
        }

        var result = await UserManager.AddLoginAsync(user, info);
        if (!result.Succeeded)
        {
            RedirectManager.RedirectToCurrentPageWithStatus("Error: The external login was not added. External logins can only be associated with one account.", HttpContext);
        }

        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        RedirectManager.RedirectToCurrentPageWithStatus("The external login was added.", HttpContext);
    }
}

```

## File: GenerateRecoveryCodes.razor

```C#
@page "/Account/Manage/GenerateRecoveryCodes"

@using Microsoft.AspNetCore.Identity
@using BulkCarnageIQ.Core.Identity

@inject UserManager<ApplicationUser> UserManager
@inject IdentityUserAccessor UserAccessor
@inject IdentityRedirectManager RedirectManager
@inject ILogger<GenerateRecoveryCodes> Logger

<PageTitle>Generate two-factor authentication (2FA) recovery codes</PageTitle>

@if (recoveryCodes is not null)
{
    <ShowRecoveryCodes RecoveryCodes="recoveryCodes.ToArray()" StatusMessage="@message" />
}
else
{
    <h3>Generate two-factor authentication (2FA) recovery codes</h3>
    <div class="alert alert-warning" role="alert">
        <p>
            <span class="glyphicon glyphicon-warning-sign"></span>
            <strong>Put these codes in a safe place.</strong>
        </p>
        <p>
            If you lose your device and don't have the recovery codes you will lose access to your account.
        </p>
        <p>
            Generating new recovery codes does not change the keys used in authenticator apps. If you wish to change the key
            used in an authenticator app you should <a href="Account/Manage/ResetAuthenticator">reset your authenticator keys.</a>
        </p>
    </div>
    <div>
        <form @formname="generate-recovery-codes" @onsubmit="OnSubmitAsync" method="post">
            <AntiforgeryToken />
            <button class="btn btn-danger" type="submit">Generate Recovery Codes</button>
        </form>
    </div>
}

@code {
    private string? message;
    private ApplicationUser user = default!;
    private IEnumerable<string>? recoveryCodes;

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        user = await UserAccessor.GetRequiredUserAsync(HttpContext);

        var isTwoFactorEnabled = await UserManager.GetTwoFactorEnabledAsync(user);
        if (!isTwoFactorEnabled)
        {
            throw new InvalidOperationException("Cannot generate recovery codes for user because they do not have 2FA enabled.");
        }
    }

    private async Task OnSubmitAsync()
    {
        var userId = await UserManager.GetUserIdAsync(user);
        recoveryCodes = await UserManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
        message = "You have generated new recovery codes.";

        Logger.LogInformation("User with ID '{UserId}' has generated new 2FA recovery codes.", userId);
    }
}

```

## File: Index.razor

```C#
@page "/Account/Manage"

@using System.ComponentModel.DataAnnotations
@using Microsoft.AspNetCore.Identity
@using BulkCarnageIQ.Core.Identity

@inject UserManager<ApplicationUser> UserManager
@inject SignInManager<ApplicationUser> SignInManager
@inject IdentityUserAccessor UserAccessor
@inject IdentityRedirectManager RedirectManager

<PageTitle>Profile</PageTitle>

<h3>Profile</h3>
<StatusMessage />

<div class="row">
    <div class="col-xl-6">
        <EditForm Model="Input" FormName="profile" OnValidSubmit="OnValidSubmitAsync" method="post">
            <DataAnnotationsValidator />
            <ValidationSummary class="text-danger" role="alert" />
            <div class="form-floating mb-3">
                <input type="text" value="@username" id="username" class="form-control" placeholder="Choose your username." disabled />
                <label for="username" class="form-label">Username</label>
            </div>
            <div class="form-floating mb-3">
                <InputText @bind-Value="Input.PhoneNumber" id="Input.PhoneNumber" class="form-control" placeholder="Enter your phone number" />
                <label for="Input.PhoneNumber" class="form-label">Phone number</label>
                <ValidationMessage For="() => Input.PhoneNumber" class="text-danger" />
            </div>
            <button type="submit" class="w-100 btn btn-lg btn-primary">Save</button>
        </EditForm>
    </div>
</div>

@code {
    private ApplicationUser user = default!;
    private string? username;
    private string? phoneNumber;

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        user = await UserAccessor.GetRequiredUserAsync(HttpContext);
        username = await UserManager.GetUserNameAsync(user);
        phoneNumber = await UserManager.GetPhoneNumberAsync(user);

        Input.PhoneNumber ??= phoneNumber;
    }

    private async Task OnValidSubmitAsync()
    {
        if (Input.PhoneNumber != phoneNumber)
        {
            var setPhoneResult = await UserManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            if (!setPhoneResult.Succeeded)
            {
                RedirectManager.RedirectToCurrentPageWithStatus("Error: Failed to set phone number.", HttpContext);
            }
        }

        await SignInManager.RefreshSignInAsync(user);
        RedirectManager.RedirectToCurrentPageWithStatus("Your profile has been updated", HttpContext);
    }

    private sealed class InputModel
    {
        [Phone]
        [Display(Name = "Phone number")]
        public string? PhoneNumber { get; set; }
    }
}

```

## File: PersonalData.razor

```C#
@page "/Account/Manage/PersonalData"

@inject IdentityUserAccessor UserAccessor

<PageTitle>Personal Data</PageTitle>

<StatusMessage />
<h3>Personal Data</h3>

<div class="row">
    <div class="col-md-6">
        <p>Your account contains personal data that you have given us. This page allows you to download or delete that data.</p>
        <p>
            <strong>Deleting this data will permanently remove your account, and this cannot be recovered.</strong>
        </p>
        <form action="Account/Manage/DownloadPersonalData" method="post">
            <AntiforgeryToken />
            <button class="btn btn-primary" type="submit">Download</button>
        </form>
        <p>
            <a href="Account/Manage/DeletePersonalData" class="btn btn-danger">Delete</a>
        </p>
    </div>
</div>

@code {
    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        _ = await UserAccessor.GetRequiredUserAsync(HttpContext);
    }
}

```

## File: ResetAuthenticator.razor

```C#
@page "/Account/Manage/ResetAuthenticator"

@using Microsoft.AspNetCore.Identity
@using BulkCarnageIQ.Core.Identity

@inject UserManager<ApplicationUser> UserManager
@inject SignInManager<ApplicationUser> SignInManager
@inject IdentityUserAccessor UserAccessor
@inject IdentityRedirectManager RedirectManager
@inject ILogger<ResetAuthenticator> Logger

<PageTitle>Reset authenticator key</PageTitle>

<StatusMessage />
<h3>Reset authenticator key</h3>
<div class="alert alert-warning" role="alert">
    <p>
        <span class="glyphicon glyphicon-warning-sign"></span>
        <strong>If you reset your authenticator key your authenticator app will not work until you reconfigure it.</strong>
    </p>
    <p>
        This process disables 2FA until you verify your authenticator app.
        If you do not complete your authenticator app configuration you may lose access to your account.
    </p>
</div>
<div>
    <form @formname="reset-authenticator" @onsubmit="OnSubmitAsync" method="post">
        <AntiforgeryToken />
        <button class="btn btn-danger" type="submit">Reset authenticator key</button>
    </form>
</div>

@code {
    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    private async Task OnSubmitAsync()
    {
        var user = await UserAccessor.GetRequiredUserAsync(HttpContext);
        await UserManager.SetTwoFactorEnabledAsync(user, false);
        await UserManager.ResetAuthenticatorKeyAsync(user);
        var userId = await UserManager.GetUserIdAsync(user);
        Logger.LogInformation("User with ID '{UserId}' has reset their authentication app key.", userId);

        await SignInManager.RefreshSignInAsync(user);

        RedirectManager.RedirectToWithStatus(
            "Account/Manage/EnableAuthenticator",
            "Your authenticator app key has been reset, you will need to configure your authenticator app using the new key.",
            HttpContext);
    }
}

```

## File: SetPassword.razor

```C#
@page "/Account/Manage/SetPassword"

@using System.ComponentModel.DataAnnotations
@using Microsoft.AspNetCore.Identity
@using BulkCarnageIQ.Core.Identity

@inject UserManager<ApplicationUser> UserManager
@inject SignInManager<ApplicationUser> SignInManager
@inject IdentityUserAccessor UserAccessor
@inject IdentityRedirectManager RedirectManager

<PageTitle>Set password</PageTitle>

<h3>Set your password</h3>
<StatusMessage Message="@message" />
<p class="text-info">
    You do not have a local username/password for this site. Add a local
    account so you can log in without an external login.
</p>
<div class="row">
    <div class="col-xl-6">
        <EditForm Model="Input" FormName="set-password" OnValidSubmit="OnValidSubmitAsync" method="post">
            <DataAnnotationsValidator />
            <ValidationSummary class="text-danger" role="alert" />
            <div class="form-floating mb-3">
                <InputText type="password" @bind-Value="Input.NewPassword" id="Input.NewPassword" class="form-control" autocomplete="new-password" placeholder="Enter the new password" />
                <label for="Input.NewPassword" class="form-label">New password</label>
                <ValidationMessage For="() => Input.NewPassword" class="text-danger" />
            </div>
            <div class="form-floating mb-3">
                <InputText type="password" @bind-Value="Input.ConfirmPassword" id="Input.ConfirmPassword" class="form-control" autocomplete="new-password" placeholder="Enter the new password" />
                <label for="Input.ConfirmPassword" class="form-label">Confirm password</label>
                <ValidationMessage For="() => Input.ConfirmPassword" class="text-danger" />
            </div>
            <button type="submit" class="w-100 btn btn-lg btn-primary">Set password</button>
        </EditForm>
     </div>
</div>

@code {
    private string? message;
    private ApplicationUser user = default!;

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        user = await UserAccessor.GetRequiredUserAsync(HttpContext);

        var hasPassword = await UserManager.HasPasswordAsync(user);
        if (hasPassword)
        {
            RedirectManager.RedirectTo("Account/Manage/ChangePassword");
        }
    }

    private async Task OnValidSubmitAsync()
    {
        var addPasswordResult = await UserManager.AddPasswordAsync(user, Input.NewPassword!);
        if (!addPasswordResult.Succeeded)
        {
            message = $"Error: {string.Join(",", addPasswordResult.Errors.Select(error => error.Description))}";
            return;
        }

        await SignInManager.RefreshSignInAsync(user);
        RedirectManager.RedirectToCurrentPageWithStatus("Your password has been set.", HttpContext);
    }

    private sealed class InputModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}

```

## File: TwoFactorAuthentication.razor

```C#
@page "/Account/Manage/TwoFactorAuthentication"

@using Microsoft.AspNetCore.Http.Features
@using Microsoft.AspNetCore.Identity
@using BulkCarnageIQ.Core.Identity

@inject UserManager<ApplicationUser> UserManager
@inject SignInManager<ApplicationUser> SignInManager
@inject IdentityUserAccessor UserAccessor
@inject IdentityRedirectManager RedirectManager

<PageTitle>Two-factor authentication (2FA)</PageTitle>

<StatusMessage />
<h3>Two-factor authentication (2FA)</h3>
@if (canTrack)
{
    if (is2faEnabled)
    {
        if (recoveryCodesLeft == 0)
        {
            <div class="alert alert-danger">
                <strong>You have no recovery codes left.</strong>
                <p>You must <a href="Account/Manage/GenerateRecoveryCodes">generate a new set of recovery codes</a> before you can log in with a recovery code.</p>
            </div>
        }
        else if (recoveryCodesLeft == 1)
        {
            <div class="alert alert-danger">
                <strong>You have 1 recovery code left.</strong>
                <p>You can <a href="Account/Manage/GenerateRecoveryCodes">generate a new set of recovery codes</a>.</p>
            </div>
        }
        else if (recoveryCodesLeft <= 3)
        {
            <div class="alert alert-warning">
                <strong>You have @recoveryCodesLeft recovery codes left.</strong>
                <p>You should <a href="Account/Manage/GenerateRecoveryCodes">generate a new set of recovery codes</a>.</p>
            </div>
        }

        if (isMachineRemembered)
        {
            <form style="display: inline-block" @formname="forget-browser" @onsubmit="OnSubmitForgetBrowserAsync" method="post">
                <AntiforgeryToken />
                <button type="submit" class="btn btn-primary">Forget this browser</button>
            </form>
        }

        <a href="Account/Manage/Disable2fa" class="btn btn-primary">Disable 2FA</a>
        <a href="Account/Manage/GenerateRecoveryCodes" class="btn btn-primary">Reset recovery codes</a>
    }

    <h4>Authenticator app</h4>
    @if (!hasAuthenticator)
    {
        <a href="Account/Manage/EnableAuthenticator" class="btn btn-primary">Add authenticator app</a>
    }
    else
    {
        <a href="Account/Manage/EnableAuthenticator" class="btn btn-primary">Set up authenticator app</a>
        <a href="Account/Manage/ResetAuthenticator" class="btn btn-primary">Reset authenticator app</a>
    }
}
else
{
    <div class="alert alert-danger">
        <strong>Privacy and cookie policy have not been accepted.</strong>
        <p>You must accept the policy before you can enable two factor authentication.</p>
    </div>
}

@code {
    private bool canTrack;
    private bool hasAuthenticator;
    private int recoveryCodesLeft;
    private bool is2faEnabled;
    private bool isMachineRemembered;

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var user = await UserAccessor.GetRequiredUserAsync(HttpContext);
        canTrack = HttpContext.Features.Get<ITrackingConsentFeature>()?.CanTrack ?? true;
        hasAuthenticator = await UserManager.GetAuthenticatorKeyAsync(user) is not null;
        is2faEnabled = await UserManager.GetTwoFactorEnabledAsync(user);
        isMachineRemembered = await SignInManager.IsTwoFactorClientRememberedAsync(user);
        recoveryCodesLeft = await UserManager.CountRecoveryCodesAsync(user);
    }

    private async Task OnSubmitForgetBrowserAsync()
    {
        await SignInManager.ForgetTwoFactorClientAsync();

        RedirectManager.RedirectToCurrentPageWithStatus(
            "The current browser has been forgotten. When you login again from this browser you will be prompted for your 2fa code.",
            HttpContext);
    }
}

```

