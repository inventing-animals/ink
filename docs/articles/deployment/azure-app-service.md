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

## Linux App Service

If you are running on a Linux App Service (Nginx), see the [nginx deployment guide](nginx.md) instead.
