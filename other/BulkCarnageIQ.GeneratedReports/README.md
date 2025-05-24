# Project Directory Structure Overview

```
├── 📁 other/
│   └── 📁 BulkCarnageIQ.ConsoleApp/
│       ├── 📁 Data/
│       ├── 📁 Utility/
│       │   ├── 🟣 DataSeeder.cs
│       │   ├── 🟣 FileUtility.cs
│       │   └── 🟣 SeedData.cs
│       └── 🟣 Program.cs
└── 📁 src/
    ├── 📁 BulkCarnageIQ.Application/
    ├── 📁 BulkCarnageIQ.Core/
    │   ├── 📁 Carnage/
    │   │   ├── 📁 Enums/
    │   │   │   └── 🟣 Sex.cs
    │   │   ├── 📁 Report/
    │   │   │   ├── 🟣 FoodItemFilter.cs
    │   │   │   ├── 🟣 MacroSummary.cs
    │   │   │   └── 🟣 WeightProjection.cs
    │   │   ├── 🟣 FoodItem.cs
    │   │   ├── 🟣 GroceryListItem.cs
    │   │   ├── 🟣 GroupFoodItem.cs
    │   │   ├── 🟣 GroupFoodItemEntry.cs
    │   │   ├── 🟣 MealEntry.cs
    │   │   ├── 🟣 UserProfile.cs
    │   │   └── 🟣 WeightLog.cs
    │   ├── 📁 Contracts/
    │   │   ├── 🟣 IFoodItemService.cs
    │   │   ├── 🟣 IGroceryListService.cs
    │   │   ├── 🟣 IGroupFoodService.cs
    │   │   ├── 🟣 IMealEntryService.cs
    │   │   ├── 🟣 IUserProfileService.cs
    │   │   └── 🟣 IWeightLogService.cs
    │   └── 📁 Identity/
    │       └── 🟣 ApplicationUser.cs
    ├── 📁 BulkCarnageIQ.Infrastructure/
    │   ├── 📁 Migrations/
    │   │   ├── 🟣 20250417034438_InitDatabase1.cs
    │   │   ├── 🟣 20250417034438_InitDatabase1.Designer.cs
    │   │   ├── 🟣 20250418231948_MealEntry-FoodItem.cs
    │   │   ├── 🟣 20250418231948_MealEntry-FoodItem.Designer.cs
    │   │   ├── 🟣 20250425141901_MealEntryMeasurementType.cs
    │   │   ├── 🟣 20250425141901_MealEntryMeasurementType.Designer.cs
    │   │   ├── 🟣 20250426002429_GroceryListItem.cs
    │   │   ├── 🟣 20250426002429_GroceryListItem.Designer.cs
    │   │   ├── 🟣 20250426145205_FoodItem-GroupName.cs
    │   │   ├── 🟣 20250426145205_FoodItem-GroupName.Designer.cs
    │   │   ├── 🟣 20250427170611_GroceryList-GroupName.cs
    │   │   ├── 🟣 20250427170611_GroceryList-GroupName.Designer.cs
    │   │   ├── 🟣 20250428221716_GroupFoodItem.cs
    │   │   ├── 🟣 20250428221716_GroupFoodItem.Designer.cs
    │   │   ├── 🟣 20250504043624_WeightLog.cs
    │   │   ├── 🟣 20250504043624_WeightLog.Designer.cs
    │   │   ├── 🟣 20250510171940_UserProfile.cs
    │   │   ├── 🟣 20250510171940_UserProfile.Designer.cs
    │   │   ├── 🟣 20250510172758_UserProfileUpdate.cs
    │   │   ├── 🟣 20250510172758_UserProfileUpdate.Designer.cs
    │   │   ├── 🟣 20250510174627_UserProfileUpdate2.cs
    │   │   ├── 🟣 20250510174627_UserProfileUpdate2.Designer.cs
    │   │   ├── 🟣 20250512160246_MealEntry-GroupName.cs
    │   │   ├── 🟣 20250512160246_MealEntry-GroupName.Designer.cs
    │   │   ├── 🟣 20250517201044_UserProfile-Sex.cs
    │   │   ├── 🟣 20250517201044_UserProfile-Sex.Designer.cs
    │   │   └── 🟣 AppDbContextModelSnapshot.cs
    │   ├── 📁 Persistence/
    │   │   └── 🟣 AppDbContext.cs
    │   └── 📁 Repositories/
    │       ├── 🟣 FoodItemService.cs
    │       ├── 🟣 GroceryListService.cs
    │       ├── 🟣 GroupFoodService.cs
    │       ├── 🟣 MealEntryService.cs
    │       ├── 🟣 UserProfileService.cs
    │       └── 🟣 WeightLogService.cs
    └── 📁 BulkCarnageIQ.Web/
        ├── 📁 Components/
        │   ├── 📁 Account/
        │   │   ├── 📁 Pages/
        │   │   │   ├── 📁 Manage/
        │   │   │   │   ├── 🌀 _Imports.razor
        │   │   │   │   ├── 🌀 ChangePassword.razor
        │   │   │   │   ├── 🌀 DeletePersonalData.razor
        │   │   │   │   ├── 🌀 Disable2fa.razor
        │   │   │   │   ├── 🌀 Email.razor
        │   │   │   │   ├── 🌀 EnableAuthenticator.razor
        │   │   │   │   ├── 🌀 ExternalLogins.razor
        │   │   │   │   ├── 🌀 GenerateRecoveryCodes.razor
        │   │   │   │   ├── 🌀 Index.razor
        │   │   │   │   ├── 🌀 PersonalData.razor
        │   │   │   │   ├── 🌀 ResetAuthenticator.razor
        │   │   │   │   ├── 🌀 SetPassword.razor
        │   │   │   │   └── 🌀 TwoFactorAuthentication.razor
        │   │   │   ├── 🌀 _Imports.razor
        │   │   │   ├── 🌀 AccessDenied.razor
        │   │   │   ├── 🌀 ConfirmEmail.razor
        │   │   │   ├── 🌀 ConfirmEmailChange.razor
        │   │   │   ├── 🌀 ExternalLogin.razor
        │   │   │   ├── 🌀 ForgotPassword.razor
        │   │   │   ├── 🌀 ForgotPasswordConfirmation.razor
        │   │   │   ├── 🌀 InvalidPasswordReset.razor
        │   │   │   ├── 🌀 InvalidUser.razor
        │   │   │   ├── 🌀 Lockout.razor
        │   │   │   ├── 🌀 Login.razor
        │   │   │   ├── 🌀 LoginWith2fa.razor
        │   │   │   ├── 🌀 LoginWithRecoveryCode.razor
        │   │   │   ├── 🌀 Register.razor
        │   │   │   ├── 🌀 RegisterConfirmation.razor
        │   │   │   ├── 🌀 ResendEmailConfirmation.razor
        │   │   │   ├── 🌀 ResetPassword.razor
        │   │   │   └── 🌀 ResetPasswordConfirmation.razor
        │   │   ├── 📁 Shared/
        │   │   │   ├── 🌀 ExternalLoginPicker.razor
        │   │   │   ├── 🌀 ManageLayout.razor
        │   │   │   ├── 🌀 ManageNavMenu.razor
        │   │   │   ├── 🌀 RedirectToLogin.razor
        │   │   │   ├── 🌀 ShowRecoveryCodes.razor
        │   │   │   └── 🌀 StatusMessage.razor
        │   │   ├── 🟣 IdentityComponentsEndpointRouteBuilderExtensions.cs
        │   │   ├── 🟣 IdentityNoOpEmailSender.cs
        │   │   ├── 🟣 IdentityRedirectManager.cs
        │   │   ├── 🟣 IdentityRevalidatingAuthenticationStateProvider.cs
        │   │   └── 🟣 IdentityUserAccessor.cs
        │   ├── 📁 Carnage/
        │   │   ├── 🌀 CalorieBarChart.razor
        │   │   ├── 🌀 GroupMealQuickAdd.razor
        │   │   ├── 🌀 MacroAlertPanel.razor
        │   │   └── 🌀 WeeklyMacroHeatmap.razor
        │   ├── 📁 Charts/
        │   │   └── 🌀 LineChart.razor
        │   ├── 📁 Layout/
        │   │   ├── 🌀 MainLayout.razor
        │   │   └── 🌀 NavMenu.razor
        │   ├── 📁 Pages/
        │   │   ├── 🌀 Auth.razor
        │   │   ├── 🌀 Error.razor
        │   │   ├── 🌀 FoodList.razor
        │   │   ├── 🌀 GroceryList.razor
        │   │   ├── 🌀 Home.razor
        │   │   ├── 🌀 MealTracker.razor
        │   │   ├── 🌀 MealTrackerReport.razor
        │   │   ├── 🌀 MealTrackerReportMacro.razor
        │   │   ├── 🌀 MealTrackerTopFoods.razor
        │   │   ├── 🌀 MealTrackerTopGroupNames.razor
        │   │   ├── 🌀 UserProfilePage.razor
        │   │   └── 🌀 WeightTracker.razor
        │   ├── 🌀 _Imports.razor
        │   ├── 🌀 App.razor
        │   └── 🌀 Routes.razor
        ├── 📁 Properties/
        ├── 📁 wwwroot/
        │   ├── 📁 lib/
        │   │   ├── 📁 bootstrap/
        │   │   │   └── 📁 dist/
        │   │   │       ├── 📁 css/
        │   │   │       └── 📁 js/
        │   │   └── 📁 bootstrap-icons/
        │   │       └── 📁 fonts/
        │   └── 📁 photos/
        └── 🟣 Program.cs
```
