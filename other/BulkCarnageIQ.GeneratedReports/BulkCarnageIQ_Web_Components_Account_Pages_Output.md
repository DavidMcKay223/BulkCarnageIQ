# Directory: Components\Account\Pages

## File: _Imports.razor

```C#
@using BulkCarnageIQ.Web.Components.Account.Shared
@attribute [ExcludeFromInteractiveRouting]

```

## File: AccessDenied.razor

```C#
@page "/Account/AccessDenied"

<PageTitle>Access denied</PageTitle>

<header>
    <h1 class="text-danger">Access denied</h1>
    <p class="text-danger">You do not have access to this resource.</p>
</header>

```

## File: ConfirmEmail.razor

```C#
@page "/Account/ConfirmEmail"

@using System.Text
@using Microsoft.AspNetCore.Identity
@using Microsoft.AspNetCore.WebUtilities
@using BulkCarnageIQ.Core.Identity

@inject UserManager<ApplicationUser> UserManager
@inject IdentityRedirectManager RedirectManager

<PageTitle>Confirm email</PageTitle>

<h1>Confirm email</h1>
<StatusMessage Message="@statusMessage" />

@code {
    private string? statusMessage;

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    [SupplyParameterFromQuery]
    private string? UserId { get; set; }

    [SupplyParameterFromQuery]
    private string? Code { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (UserId is null || Code is null)
        {
            RedirectManager.RedirectTo("");
        }

        var user = await UserManager.FindByIdAsync(UserId);
        if (user is null)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            statusMessage = $"Error loading user with ID {UserId}";
        }
        else
        {
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Code));
            var result = await UserManager.ConfirmEmailAsync(user, code);
            statusMessage = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";
        }
    }
}

```

## File: ConfirmEmailChange.razor

```C#
@page "/Account/ConfirmEmailChange"

@using System.Text
@using Microsoft.AspNetCore.Identity
@using Microsoft.AspNetCore.WebUtilities
@using BulkCarnageIQ.Core.Identity

@inject UserManager<ApplicationUser> UserManager
@inject SignInManager<ApplicationUser> SignInManager
@inject IdentityRedirectManager RedirectManager

<PageTitle>Confirm email change</PageTitle>

<h1>Confirm email change</h1>

<StatusMessage Message="@message" />

@code {
    private string? message;

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    [SupplyParameterFromQuery]
    private string? UserId { get; set; }

    [SupplyParameterFromQuery]
    private string? Email { get; set; }

    [SupplyParameterFromQuery]
    private string? Code { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (UserId is null || Email is null || Code is null)
        {
            RedirectManager.RedirectToWithStatus(
                "Account/Login", "Error: Invalid email change confirmation link.", HttpContext);
        }

        var user = await UserManager.FindByIdAsync(UserId);
        if (user is null)
        {
            message = "Unable to find user with Id '{userId}'";
            return;
        }

        var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Code));
        var result = await UserManager.ChangeEmailAsync(user, Email, code);
        if (!result.Succeeded)
        {
            message = "Error changing email.";
            return;
        }

        // In our UI email and user name are one and the same, so when we update the email
        // we need to update the user name.
        var setUserNameResult = await UserManager.SetUserNameAsync(user, Email);
        if (!setUserNameResult.Succeeded)
        {
            message = "Error changing user name.";
            return;
        }

        await SignInManager.RefreshSignInAsync(user);
        message = "Thank you for confirming your email change.";
    }
}

```

## File: ExternalLogin.razor

```C#
@page "/Account/ExternalLogin"

@using System.ComponentModel.DataAnnotations
@using System.Security.Claims
@using System.Text
@using System.Text.Encodings.Web
@using Microsoft.AspNetCore.Identity
@using Microsoft.AspNetCore.WebUtilities
@using BulkCarnageIQ.Core.Identity

@inject SignInManager<ApplicationUser> SignInManager
@inject UserManager<ApplicationUser> UserManager
@inject IUserStore<ApplicationUser> UserStore
@inject IEmailSender<ApplicationUser> EmailSender
@inject NavigationManager NavigationManager
@inject IdentityRedirectManager RedirectManager
@inject ILogger<ExternalLogin> Logger

<PageTitle>Register</PageTitle>

<StatusMessage Message="@message" />
<h1>Register</h1>
<h2>Associate your @ProviderDisplayName account.</h2>
<hr />

<div  class="alert alert-info">
    You've successfully authenticated with <strong>@ProviderDisplayName</strong>.
    Please enter an email address for this site below and click the Register button to finish
    logging in.
</div>

<div class="row">
    <div class="col-md-4">
        <EditForm Model="Input" OnValidSubmit="OnValidSubmitAsync" FormName="confirmation" method="post">
            <DataAnnotationsValidator />
            <ValidationSummary class="text-danger" role="alert" />
            <div class="form-floating mb-3">
                <InputText @bind-Value="Input.Email" id="Input.Email" class="form-control" autocomplete="email" placeholder="Please enter your email." />
                <label for="Input.Email" class="form-label">Email</label>
                <ValidationMessage For="() => Input.Email" />
            </div>
            <button type="submit" class="w-100 btn btn-lg btn-primary">Register</button>
        </EditForm>
    </div>
</div>

@code {
    public const string LoginCallbackAction = "LoginCallback";

    private string? message;
    private ExternalLoginInfo? externalLoginInfo;

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    [SupplyParameterFromQuery]
    private string? RemoteError { get; set; }

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    [SupplyParameterFromQuery]
    private string? Action { get; set; }

    private string? ProviderDisplayName => externalLoginInfo?.ProviderDisplayName;

    protected override async Task OnInitializedAsync()
    {
        if (RemoteError is not null)
        {
            RedirectManager.RedirectToWithStatus("Account/Login", $"Error from external provider: {RemoteError}", HttpContext);
        }

        var info = await SignInManager.GetExternalLoginInfoAsync();
        if (info is null)
        {
            RedirectManager.RedirectToWithStatus("Account/Login", "Error loading external login information.", HttpContext);
        }

        externalLoginInfo = info;

        if (HttpMethods.IsGet(HttpContext.Request.Method))
        {
            if (Action == LoginCallbackAction)
            {
                await OnLoginCallbackAsync();
                return;
            }

            // We should only reach this page via the login callback, so redirect back to
            // the login page if we get here some other way.
            RedirectManager.RedirectTo("Account/Login");
        }
    }

    private async Task OnLoginCallbackAsync()
    {
        if (externalLoginInfo is null)
        {
            RedirectManager.RedirectToWithStatus("Account/Login", "Error loading external login information.", HttpContext);
        }

        // Sign in the user with this external login provider if the user already has a login.
        var result = await SignInManager.ExternalLoginSignInAsync(
            externalLoginInfo.LoginProvider,
            externalLoginInfo.ProviderKey,
            isPersistent: false,
            bypassTwoFactor: true);

        if (result.Succeeded)
        {
            Logger.LogInformation(
                "{Name} logged in with {LoginProvider} provider.",
                externalLoginInfo.Principal.Identity?.Name,
                externalLoginInfo.LoginProvider);
            RedirectManager.RedirectTo(ReturnUrl);
        }
        else if (result.IsLockedOut)
        {
            RedirectManager.RedirectTo("Account/Lockout");
        }

        // If the user does not have an account, then ask the user to create an account.
        if (externalLoginInfo.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
        {
            Input.Email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email) ?? "";
        }
    }

    private async Task OnValidSubmitAsync()
    {
        if (externalLoginInfo is null)
        {
            RedirectManager.RedirectToWithStatus("Account/Login", "Error loading external login information during confirmation.", HttpContext);
        }

        var emailStore = GetEmailStore();
        var user = CreateUser();

        await UserStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
        await emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

        var result = await UserManager.CreateAsync(user);
        if (result.Succeeded)
        {
            result = await UserManager.AddLoginAsync(user, externalLoginInfo);
            if (result.Succeeded)
            {
                Logger.LogInformation("User created an account using {Name} provider.", externalLoginInfo.LoginProvider);

                var userId = await UserManager.GetUserIdAsync(user);
                var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                var callbackUrl = NavigationManager.GetUriWithQueryParameters(
                    NavigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri,
                    new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code });
                await EmailSender.SendConfirmationLinkAsync(user, Input.Email, HtmlEncoder.Default.Encode(callbackUrl));

                // If account confirmation is required, we need to show the link if we don't have a real email sender
                if (UserManager.Options.SignIn.RequireConfirmedAccount)
                {
                    RedirectManager.RedirectTo("Account/RegisterConfirmation", new() { ["email"] = Input.Email });
                }

                await SignInManager.SignInAsync(user, isPersistent: false, externalLoginInfo.LoginProvider);
                RedirectManager.RedirectTo(ReturnUrl);
            }
        }

        message = $"Error: {string.Join(",", result.Errors.Select(error => error.Description))}";
    }

    private ApplicationUser CreateUser()
    {
        try
        {
            return Activator.CreateInstance<ApplicationUser>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor");
        }
    }

    private IUserEmailStore<ApplicationUser> GetEmailStore()
    {
        if (!UserManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<ApplicationUser>)UserStore;
    }

    private sealed class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
    }
}

```

## File: ForgotPassword.razor

```C#
@page "/Account/ForgotPassword"

@using System.ComponentModel.DataAnnotations
@using System.Text
@using System.Text.Encodings.Web
@using Microsoft.AspNetCore.Identity
@using Microsoft.AspNetCore.WebUtilities
@using BulkCarnageIQ.Core.Identity

@inject UserManager<ApplicationUser> UserManager
@inject IEmailSender<ApplicationUser> EmailSender
@inject NavigationManager NavigationManager
@inject IdentityRedirectManager RedirectManager

<PageTitle>Forgot your password?</PageTitle>

<h1>Forgot your password?</h1>
<h2>Enter your email.</h2>
<hr />
<div class="row">
    <div class="col-md-4">
        <EditForm Model="Input" FormName="forgot-password" OnValidSubmit="OnValidSubmitAsync" method="post">
            <DataAnnotationsValidator />
            <ValidationSummary class="text-danger" role="alert" />

            <div class="form-floating mb-3">
                <InputText @bind-Value="Input.Email" id="Input.Email" class="form-control" autocomplete="username" aria-required="true" placeholder="name@example.com" />
                <label for="Input.Email" class="form-label">Email</label>
                <ValidationMessage For="() => Input.Email" class="text-danger" />
            </div>
            <button type="submit" class="w-100 btn btn-lg btn-primary">Reset password</button>
        </EditForm>
     </div>
</div>

@code {
    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    private async Task OnValidSubmitAsync()
    {
        var user = await UserManager.FindByEmailAsync(Input.Email);
        if (user is null || !(await UserManager.IsEmailConfirmedAsync(user)))
        {
            // Don't reveal that the user does not exist or is not confirmed
            RedirectManager.RedirectTo("Account/ForgotPasswordConfirmation");
        }

        // For more information on how to enable account confirmation and password reset please
        // visit https://go.microsoft.com/fwlink/?LinkID=532713
        var code = await UserManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = NavigationManager.GetUriWithQueryParameters(
            NavigationManager.ToAbsoluteUri("Account/ResetPassword").AbsoluteUri,
            new Dictionary<string, object?> { ["code"] = code });

        await EmailSender.SendPasswordResetLinkAsync(user, Input.Email, HtmlEncoder.Default.Encode(callbackUrl));

        RedirectManager.RedirectTo("Account/ForgotPasswordConfirmation");
    }

    private sealed class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
    }
}

```

## File: ForgotPasswordConfirmation.razor

```C#
@page "/Account/ForgotPasswordConfirmation"

<PageTitle>Forgot password confirmation</PageTitle>

<h1>Forgot password confirmation</h1>
<p role="alert">
    Please check your email to reset your password.
</p>

```

## File: InvalidPasswordReset.razor

```C#
@page "/Account/InvalidPasswordReset"

<PageTitle>Invalid password reset</PageTitle>

<h1>Invalid password reset</h1>
<p role="alert">
    The password reset link is invalid.
</p>

```

## File: InvalidUser.razor

```C#
@page "/Account/InvalidUser"

<PageTitle>Invalid user</PageTitle>

<h3>Invalid user</h3>

<StatusMessage />

```

## File: Lockout.razor

```C#
@page "/Account/Lockout"

<PageTitle>Locked out</PageTitle>

<header>
    <h1 class="text-danger">Locked out</h1>
    <p class="text-danger" role="alert">This account has been locked out, please try again later.</p>
</header>

```

## File: Login.razor

```C#
@page "/Account/Login"

@using System.ComponentModel.DataAnnotations
@using Microsoft.AspNetCore.Authentication
@using Microsoft.AspNetCore.Identity
@using BulkCarnageIQ.Core.Identity

@inject SignInManager<ApplicationUser> SignInManager
@inject ILogger<Login> Logger
@inject NavigationManager NavigationManager
@inject IdentityRedirectManager RedirectManager

<PageTitle>Log in</PageTitle>

<h1>Log in</h1>
<div class="row">
    <div class="col-lg-6">
        <section>
            <StatusMessage Message="@errorMessage" />
            <EditForm Model="Input" method="post" OnValidSubmit="LoginUser" FormName="login">
                <DataAnnotationsValidator />
                <h2>Use a local account to log in.</h2>
                <hr />
                <ValidationSummary class="text-danger" role="alert" />
                <div class="form-floating mb-3">
                    <InputText @bind-Value="Input.Email" id="Input.Email" class="form-control" autocomplete="username" aria-required="true" placeholder="name@example.com" />
                    <label for="Input.Email" class="form-label">Email</label>
                    <ValidationMessage For="() => Input.Email" class="text-danger" />
                </div>
                <div class="form-floating mb-3">
                    <InputText type="password" @bind-Value="Input.Password" id="Input.Password" class="form-control" autocomplete="current-password" aria-required="true" placeholder="password" />
                    <label for="Input.Password" class="form-label">Password</label>
                    <ValidationMessage For="() => Input.Password" class="text-danger" />
                </div>
                <div class="checkbox mb-3">
                    <label class="form-label">
                        <InputCheckbox @bind-Value="Input.RememberMe" class="darker-border-checkbox form-check-input" />
                        Remember me
                    </label>
                </div>
                <div>
                    <button type="submit" class="w-100 btn btn-lg btn-primary">Log in</button>
                </div>
                <div>
                    <p>
                        <a href="Account/ForgotPassword">Forgot your password?</a>
                    </p>
                    <p>
                        <a href="@(NavigationManager.GetUriWithQueryParameters("Account/Register", new Dictionary<string, object?> { ["ReturnUrl"] = ReturnUrl }))">Register as a new user</a>
                    </p>
                    <p>
                        <a href="Account/ResendEmailConfirmation">Resend email confirmation</a>
                    </p>
                </div>
            </EditForm>
        </section>
    </div>
    <div class="col-lg-4 col-lg-offset-2">
        <section>
            <h3>Use another service to log in.</h3>
            <hr />
            <ExternalLoginPicker />
        </section>
    </div>
</div>

@code {
    private string? errorMessage;

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (HttpMethods.IsGet(HttpContext.Request.Method))
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        }
    }

    public async Task LoginUser()
    {
        // This doesn't count login failures towards account lockout
        // To enable password failures to trigger account lockout, set lockoutOnFailure: true
        var result = await SignInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            Logger.LogInformation("User logged in.");
            RedirectManager.RedirectTo(ReturnUrl);
        }
        else if (result.RequiresTwoFactor)
        {
            RedirectManager.RedirectTo(
                "Account/LoginWith2fa",
                new() { ["returnUrl"] = ReturnUrl, ["rememberMe"] = Input.RememberMe });
        }
        else if (result.IsLockedOut)
        {
            Logger.LogWarning("User account locked out.");
            RedirectManager.RedirectTo("Account/Lockout");
        }
        else
        {
            errorMessage = "Error: Invalid login attempt.";
        }
    }

    private sealed class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}

```

## File: LoginWith2fa.razor

```C#
@page "/Account/LoginWith2fa"

@using System.ComponentModel.DataAnnotations
@using Microsoft.AspNetCore.Identity
@using BulkCarnageIQ.Core.Identity

@inject SignInManager<ApplicationUser> SignInManager
@inject UserManager<ApplicationUser> UserManager
@inject IdentityRedirectManager RedirectManager
@inject ILogger<LoginWith2fa> Logger

<PageTitle>Two-factor authentication</PageTitle>

<h1>Two-factor authentication</h1>
<hr />
<StatusMessage Message="@message" />
<p>Your login is protected with an authenticator app. Enter your authenticator code below.</p>
<div class="row">
    <div class="col-md-4">
        <EditForm Model="Input" FormName="login-with-2fa" OnValidSubmit="OnValidSubmitAsync" method="post">
            <input type="hidden" name="ReturnUrl" value="@ReturnUrl" />
            <input type="hidden" name="RememberMe" value="@RememberMe" />
            <DataAnnotationsValidator />
            <ValidationSummary class="text-danger" role="alert" />
            <div class="form-floating mb-3">
                <InputText @bind-Value="Input.TwoFactorCode" id="Input.TwoFactorCode" class="form-control" autocomplete="off" />
                <label for="Input.TwoFactorCode" class="form-label">Authenticator code</label>
                <ValidationMessage For="() => Input.TwoFactorCode" class="text-danger" />
            </div>
            <div class="checkbox mb-3">
                <label for="remember-machine" class="form-label">
                    <InputCheckbox @bind-Value="Input.RememberMachine" />
                    Remember this machine
                </label>
            </div>
            <div>
                <button type="submit" class="w-100 btn btn-lg btn-primary">Log in</button>
            </div>
        </EditForm>
    </div>
</div>
<p>
    Don't have access to your authenticator device? You can
    <a href="Account/LoginWithRecoveryCode?ReturnUrl=@ReturnUrl">log in with a recovery code</a>.
</p>

@code {
    private string? message;
    private ApplicationUser user = default!;

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    [SupplyParameterFromQuery]
    private bool RememberMe { get; set; }

    protected override async Task OnInitializedAsync()
    {
        // Ensure the user has gone through the username & password screen first
        user = await SignInManager.GetTwoFactorAuthenticationUserAsync() ??
            throw new InvalidOperationException("Unable to load two-factor authentication user.");
    }

    private async Task OnValidSubmitAsync()
    {
        var authenticatorCode = Input.TwoFactorCode!.Replace(" ", string.Empty).Replace("-", string.Empty);
        var result = await SignInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, RememberMe, Input.RememberMachine);
        var userId = await UserManager.GetUserIdAsync(user);

        if (result.Succeeded)
        {
            Logger.LogInformation("User with ID '{UserId}' logged in with 2fa.", userId);
            RedirectManager.RedirectTo(ReturnUrl);
        }
        else if (result.IsLockedOut)
        {
            Logger.LogWarning("User with ID '{UserId}' account locked out.", userId);
            RedirectManager.RedirectTo("Account/Lockout");
        }
        else
        {
            Logger.LogWarning("Invalid authenticator code entered for user with ID '{UserId}'.", userId);
            message = "Error: Invalid authenticator code.";
        }
    }

    private sealed class InputModel
    {
        [Required]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Authenticator code")]
        public string? TwoFactorCode { get; set; }

        [Display(Name = "Remember this machine")]
        public bool RememberMachine { get; set; }
    }
}

```

## File: LoginWithRecoveryCode.razor

```C#
@page "/Account/LoginWithRecoveryCode"

@using System.ComponentModel.DataAnnotations
@using Microsoft.AspNetCore.Identity
@using BulkCarnageIQ.Core.Identity

@inject SignInManager<ApplicationUser> SignInManager
@inject UserManager<ApplicationUser> UserManager
@inject IdentityRedirectManager RedirectManager
@inject ILogger<LoginWithRecoveryCode> Logger

<PageTitle>Recovery code verification</PageTitle>

<h1>Recovery code verification</h1>
<hr />
<StatusMessage Message="@message" />
<p>
    You have requested to log in with a recovery code. This login will not be remembered until you provide
    an authenticator app code at log in or disable 2FA and log in again.
</p>
<div class="row">
    <div class="col-md-4">
        <EditForm Model="Input" FormName="login-with-recovery-code" OnValidSubmit="OnValidSubmitAsync" method="post">
            <DataAnnotationsValidator />
            <ValidationSummary class="text-danger" role="alert" />
            <div class="form-floating mb-3">
                <InputText @bind-Value="Input.RecoveryCode" id="Input.RecoveryCode" class="form-control" autocomplete="off" placeholder="RecoveryCode" />
                <label for="Input.RecoveryCode" class="form-label">Recovery Code</label>
                <ValidationMessage For="() => Input.RecoveryCode" class="text-danger" />
            </div>
            <button type="submit" class="w-100 btn btn-lg btn-primary">Log in</button>
        </EditForm>
    </div>
</div>

@code {
    private string? message;
    private ApplicationUser user = default!;

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    protected override async Task OnInitializedAsync()
    {
        // Ensure the user has gone through the username & password screen first
        user = await SignInManager.GetTwoFactorAuthenticationUserAsync() ??
            throw new InvalidOperationException("Unable to load two-factor authentication user.");
    }

    private async Task OnValidSubmitAsync()
    {
        var recoveryCode = Input.RecoveryCode.Replace(" ", string.Empty);

        var result = await SignInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

        var userId = await UserManager.GetUserIdAsync(user);

        if (result.Succeeded)
        {
            Logger.LogInformation("User with ID '{UserId}' logged in with a recovery code.", userId);
            RedirectManager.RedirectTo(ReturnUrl);
        }
        else if (result.IsLockedOut)
        {
            Logger.LogWarning("User account locked out.");
            RedirectManager.RedirectTo("Account/Lockout");
        }
        else
        {
            Logger.LogWarning("Invalid recovery code entered for user with ID '{UserId}' ", userId);
            message = "Error: Invalid recovery code entered.";
        }
    }

    private sealed class InputModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Recovery Code")]
        public string RecoveryCode { get; set; } = "";
    }
}

```

## File: Register.razor

```C#
@page "/Account/Register"

@using System.ComponentModel.DataAnnotations
@using System.Text
@using System.Text.Encodings.Web
@using Microsoft.AspNetCore.Identity
@using Microsoft.AspNetCore.WebUtilities
@using BulkCarnageIQ.Core.Identity

@inject UserManager<ApplicationUser> UserManager
@inject IUserStore<ApplicationUser> UserStore
@inject SignInManager<ApplicationUser> SignInManager
@inject IEmailSender<ApplicationUser> EmailSender
@inject ILogger<Register> Logger
@inject NavigationManager NavigationManager
@inject IdentityRedirectManager RedirectManager

<PageTitle>Register</PageTitle>

<h1>Register</h1>

<div class="row">
    <div class="col-lg-6">
        <StatusMessage Message="@Message" />
        <EditForm Model="Input" asp-route-returnUrl="@ReturnUrl" method="post" OnValidSubmit="RegisterUser" FormName="register">
            <DataAnnotationsValidator />
            <h2>Create a new account.</h2>
            <hr />
            <ValidationSummary class="text-danger" role="alert" />
            <div class="form-floating mb-3">
                <InputText @bind-Value="Input.Email" id="Input.Email" class="form-control" autocomplete="username" aria-required="true" placeholder="name@example.com" />
                <label for="Input.Email">Email</label>
                <ValidationMessage For="() => Input.Email" class="text-danger" />
            </div>
            <div class="form-floating mb-3">
                <InputText type="password" @bind-Value="Input.Password" id="Input.Password" class="form-control" autocomplete="new-password" aria-required="true" placeholder="password" />
                <label for="Input.Password">Password</label>
                <ValidationMessage For="() => Input.Password" class="text-danger" />
            </div>
            <div class="form-floating mb-3">
                <InputText type="password" @bind-Value="Input.ConfirmPassword" id="Input.ConfirmPassword" class="form-control" autocomplete="new-password" aria-required="true" placeholder="password" />
                <label for="Input.ConfirmPassword">Confirm Password</label>
                <ValidationMessage For="() => Input.ConfirmPassword" class="text-danger" />
            </div>
            <button type="submit" class="w-100 btn btn-lg btn-primary">Register</button>
        </EditForm>
    </div>
    <div class="col-lg-4 col-lg-offset-2">
        <section>
            <h3>Use another service to register.</h3>
            <hr />
            <ExternalLoginPicker />
        </section>
    </div>
</div>

@code {
    private IEnumerable<IdentityError>? identityErrors;

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    private string? Message => identityErrors is null ? null : $"Error: {string.Join(", ", identityErrors.Select(error => error.Description))}";

    public async Task RegisterUser(EditContext editContext)
    {
        var user = CreateUser();

        await UserStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
        var emailStore = GetEmailStore();
        await emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
        var result = await UserManager.CreateAsync(user, Input.Password);

        if (!result.Succeeded)
        {
            identityErrors = result.Errors;
            return;
        }

        Logger.LogInformation("User created a new account with password.");

        var userId = await UserManager.GetUserIdAsync(user);
        var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = NavigationManager.GetUriWithQueryParameters(
            NavigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri,
            new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code, ["returnUrl"] = ReturnUrl });

        await EmailSender.SendConfirmationLinkAsync(user, Input.Email, HtmlEncoder.Default.Encode(callbackUrl));

        if (UserManager.Options.SignIn.RequireConfirmedAccount)
        {
            RedirectManager.RedirectTo(
                "Account/RegisterConfirmation",
                new() { ["email"] = Input.Email, ["returnUrl"] = ReturnUrl });
        }

        await SignInManager.SignInAsync(user, isPersistent: false);
        RedirectManager.RedirectTo(ReturnUrl);
    }

    private ApplicationUser CreateUser()
    {
        try
        {
            return Activator.CreateInstance<ApplicationUser>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor.");
        }
    }

    private IUserEmailStore<ApplicationUser> GetEmailStore()
    {
        if (!UserManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<ApplicationUser>)UserStore;
    }

    private sealed class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = "";

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = "";
    }
}

```

## File: RegisterConfirmation.razor

```C#
@page "/Account/RegisterConfirmation"

@using System.Text
@using Microsoft.AspNetCore.Identity
@using Microsoft.AspNetCore.WebUtilities
@using BulkCarnageIQ.Core.Identity

@inject UserManager<ApplicationUser> UserManager
@inject IEmailSender<ApplicationUser> EmailSender
@inject NavigationManager NavigationManager
@inject IdentityRedirectManager RedirectManager

<PageTitle>Register confirmation</PageTitle>

<h1>Register confirmation</h1>

<StatusMessage Message="@statusMessage" />

@if (emailConfirmationLink is not null)
{
    <p>
        This app does not currently have a real email sender registered, see <a href="https://aka.ms/aspaccountconf">these docs</a> for how to configure a real email sender.
        Normally this would be emailed: <a href="@emailConfirmationLink">Click here to confirm your account</a>
    </p>
}
else
{
    <p role="alert">Please check your email to confirm your account.</p>
}

@code {
    private string? emailConfirmationLink;
    private string? statusMessage;

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    [SupplyParameterFromQuery]
    private string? Email { get; set; }

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (Email is null)
        {
            RedirectManager.RedirectTo("");
        }

        var user = await UserManager.FindByEmailAsync(Email);
        if (user is null)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            statusMessage = "Error finding user for unspecified email";
        }
        else if (EmailSender is IdentityNoOpEmailSender)
        {
            // Once you add a real email sender, you should remove this code that lets you confirm the account
            var userId = await UserManager.GetUserIdAsync(user);
            var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            emailConfirmationLink = NavigationManager.GetUriWithQueryParameters(
                NavigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri,
                new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code, ["returnUrl"] = ReturnUrl });
        }
    }
}

```

## File: ResendEmailConfirmation.razor

```C#
@page "/Account/ResendEmailConfirmation"

@using System.ComponentModel.DataAnnotations
@using System.Text
@using System.Text.Encodings.Web
@using Microsoft.AspNetCore.Identity
@using Microsoft.AspNetCore.WebUtilities
@using BulkCarnageIQ.Core.Identity

@inject UserManager<ApplicationUser> UserManager
@inject IEmailSender<ApplicationUser> EmailSender
@inject NavigationManager NavigationManager
@inject IdentityRedirectManager RedirectManager

<PageTitle>Resend email confirmation</PageTitle>

<h1>Resend email confirmation</h1>
<h2>Enter your email.</h2>
<hr />
<StatusMessage Message="@message" />
<div class="row">
    <div class="col-md-4">
        <EditForm Model="Input" FormName="resend-email-confirmation" OnValidSubmit="OnValidSubmitAsync" method="post">
            <DataAnnotationsValidator />
            <ValidationSummary class="text-danger" role="alert" />
            <div class="form-floating mb-3">
                <InputText @bind-Value="Input.Email" id="Input.Email" class="form-control" aria-required="true" placeholder="name@example.com" />
                <label for="Input.Email" class="form-label">Email</label>
                <ValidationMessage For="() => Input.Email" class="text-danger" />
            </div>
            <button type="submit" class="w-100 btn btn-lg btn-primary">Resend</button>
        </EditForm>
    </div>
</div>

@code {
    private string? message;

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    private async Task OnValidSubmitAsync()
    {
        var user = await UserManager.FindByEmailAsync(Input.Email!);
        if (user is null)
        {
            message = "Verification email sent. Please check your email.";
            return;
        }

        var userId = await UserManager.GetUserIdAsync(user);
        var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = NavigationManager.GetUriWithQueryParameters(
            NavigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri,
            new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code });
        await EmailSender.SendConfirmationLinkAsync(user, Input.Email, HtmlEncoder.Default.Encode(callbackUrl));

        message = "Verification email sent. Please check your email.";
    }

    private sealed class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
    }
}

```

## File: ResetPassword.razor

```C#
@page "/Account/ResetPassword"

@using System.ComponentModel.DataAnnotations
@using System.Text
@using Microsoft.AspNetCore.Identity
@using Microsoft.AspNetCore.WebUtilities
@using BulkCarnageIQ.Core.Identity

@inject IdentityRedirectManager RedirectManager
@inject UserManager<ApplicationUser> UserManager

<PageTitle>Reset password</PageTitle>

<h1>Reset password</h1>
<h2>Reset your password.</h2>
<hr />
<div class="row">
    <div class="col-md-4">
        <StatusMessage Message="@Message" />
        <EditForm Model="Input" FormName="reset-password" OnValidSubmit="OnValidSubmitAsync" method="post">
            <DataAnnotationsValidator />
            <ValidationSummary class="text-danger" role="alert" />

            <input type="hidden" name="Input.Code" value="@Input.Code" />
            <div class="form-floating mb-3">
                <InputText @bind-Value="Input.Email" id="Input.Email" class="form-control" autocomplete="username" aria-required="true" placeholder="name@example.com" />
                <label for="Input.Email" class="form-label">Email</label>
                <ValidationMessage For="() => Input.Email" class="text-danger" />
            </div>
            <div class="form-floating mb-3">
                <InputText type="password" @bind-Value="Input.Password" id="Input.Password" class="form-control" autocomplete="new-password" aria-required="true" placeholder="Please enter your password." />
                <label for="Input.Password" class="form-label">Password</label>
                <ValidationMessage For="() => Input.Password" class="text-danger" />
            </div>
            <div class="form-floating mb-3">
                <InputText type="password" @bind-Value="Input.ConfirmPassword" id="Input.ConfirmPassword" class="form-control" autocomplete="new-password" aria-required="true" placeholder="Please confirm your password." />
                <label for="Input.ConfirmPassword" class="form-label">Confirm password</label>
                <ValidationMessage For="() => Input.ConfirmPassword" class="text-danger" />
            </div>
            <button type="submit" class="w-100 btn btn-lg btn-primary">Reset</button>
        </EditForm>
    </div>
</div>

@code {
    private IEnumerable<IdentityError>? identityErrors;

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    [SupplyParameterFromQuery]
    private string? Code { get; set; }

    private string? Message => identityErrors is null ? null : $"Error: {string.Join(", ", identityErrors.Select(error => error.Description))}";

    protected override void OnInitialized()
    {
        if (Code is null)
        {
            RedirectManager.RedirectTo("Account/InvalidPasswordReset");
        }

        Input.Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Code));
    }

    private async Task OnValidSubmitAsync()
    {
        var user = await UserManager.FindByEmailAsync(Input.Email);
        if (user is null)
        {
            // Don't reveal that the user does not exist
            RedirectManager.RedirectTo("Account/ResetPasswordConfirmation");
        }

        var result = await UserManager.ResetPasswordAsync(user, Input.Code, Input.Password);
        if (result.Succeeded)
        {
            RedirectManager.RedirectTo("Account/ResetPasswordConfirmation");
        }

        identityErrors = result.Errors;
    }

    private sealed class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = "";

        [Required]
        public string Code { get; set; } = "";
    }
}

```

## File: ResetPasswordConfirmation.razor

```C#
@page "/Account/ResetPasswordConfirmation"
<PageTitle>Reset password confirmation</PageTitle>

<h1>Reset password confirmation</h1>
<p role="alert">
    Your password has been reset. Please <a href="Account/Login">click here to log in</a>.
</p>

```

