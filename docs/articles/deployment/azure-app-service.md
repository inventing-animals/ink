# Deploying to Azure App Service

Azure App Service (Windows) uses IIS, which supports URL rewriting via `web.config`. This is required for `BrowserHistoryRouter` to work correctly — without it, manually entered URLs and page refreshes return a 404 because IIS looks for a file at that path instead of serving `index.html`.

## URL rewriting

Add a `web.config` to your app's `wwwroot` folder:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <rewrite>
      <rules>
        <rule name="SPA" stopProcessing="true">
          <match url=".*" />
          <conditions logicalGrouping="MatchAll">
            <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true" />
          </conditions>
          <action type="Rewrite" url="/index.html" />
        </rule>
      </rules>
    </rewrite>
  </system.webServer>
</configuration>
```

This rule rewrites all requests that do not match an existing file to `index.html`, letting the Ink router handle the path client-side.

> [!NOTE]
> The URL Rewrite module must be installed on the App Service instance. It is available by default on most App Service plans.

## Sub-path hosting

If the app is served under a sub-path (e.g. the WASM files are served by an ASP.NET Core API at `/app/`), update the rewrite rule to match only that prefix and set `<base href="/app/" />` in `index.html`.

The base href can also be injected at publish time rather than hard-coded:

```bash
dotnet publish src/MyApp.Browser -p:BaseHref=/app/
```

`BrowserHistoryRouter` reads the base href at startup and strips it from all paths automatically — no changes to view model routing logic are required. See [Base URL / sub-path hosting](../platform.md#base-url--sub-path-hosting) for details.

## Linux App Service

If you are running on a Linux App Service (Nginx), see the [nginx deployment guide](nginx.md) instead.
