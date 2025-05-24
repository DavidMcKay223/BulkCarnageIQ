# Directory: Components

## File: _Imports.razor

```C#
@using System.Net.Http
@using System.Net.Http.Json
@using Microsoft.AspNetCore.Components.Authorization
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using static Microsoft.AspNetCore.Components.Web.RenderMode
@using Microsoft.AspNetCore.Components.Web.Virtualization
@using Microsoft.JSInterop
@using BulkCarnageIQ.Web
@using BulkCarnageIQ.Web.Components

```

## File: App.razor

```C#
<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <base href="/" />
    <link rel="stylesheet" href="@Assets["lib/bootstrap/dist/css/bootstrap.min.css"]" />
    <link rel="stylesheet" href="@Assets["lib/bootstrap-icons/bootstrap-icons.css"]" />
    <link rel="stylesheet" href="@Assets["app.css"]" />
    <link rel="stylesheet" href="@Assets["BulkCarnageIQ.Web.styles.css"]" />
    <ImportMap />
    <link rel="icon" type="image/png" href="favicon.png" />
    <HeadOutlet />
</head>

<body>
    <Routes />
    <script src="_framework/blazor.web.js"></script>
    <script src="@Assets["lib/bootstrap/dist/js/bootstrap.bundle.min.js"]"></script>
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script src="@Assets["chartInterop.js"]"></script>

    <script>
        window.downloadFile = (fileName, contentType, byteBase64) => {
          const link = document.createElement('a');
          link.href = `data:${contentType};base64,${byteBase64}`;
          link.download = fileName;
          document.body.appendChild(link);
          link.click();
          document.body.removeChild(link);
        };
    </script>
</body>

</html>

```

## File: Routes.razor

```C#
@using BulkCarnageIQ.Web.Components.Account.Shared
<Router AppAssembly="typeof(Program).Assembly">
    <Found Context="routeData">
        <AuthorizeRouteView RouteData="routeData" DefaultLayout="typeof(Layout.MainLayout)">
            <NotAuthorized>
                <RedirectToLogin />
            </NotAuthorized>
        </AuthorizeRouteView>
        <FocusOnNavigate RouteData="routeData" Selector="h1" />
    </Found>
</Router>

```

