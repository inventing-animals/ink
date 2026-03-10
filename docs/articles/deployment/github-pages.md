# Deploying to GitHub Pages

GitHub Pages is a static CDN with no server-side request handling. It cannot rewrite URLs to `index.html`, so `BrowserHistoryRouter` — which relies on the History API and path-based URLs — does not fully work when a user manually enters a URL or refreshes the page.

## Limitation

When navigating within the app, `BrowserHistoryRouter` works correctly: the URL updates and the router state stays in sync. The limitation only affects:

- Manually typing a URL into the address bar
- Refreshing the page on any route other than `/`
- Sharing a deep link

In these cases, GitHub Pages returns a 404 because the path does not correspond to a real file.

## Workaround: 404.html

GitHub Pages serves `404.html` when a path does not match a real file. Since the Ink WASM app is entirely client-side, simply copying `index.html` to `404.html` is enough — the app loads, and `BrowserHistoryRouter` reads the path from the address bar and routes correctly.

In your deployment pipeline, copy the file after publishing:

```bash
cp wwwroot/index.html wwwroot/404.html
```

Or in a GitHub Actions workflow:

```yaml
- name: Copy index.html to 404.html
  run: cp ./publish/wwwroot/index.html ./publish/wwwroot/404.html
```

For a production B2B application, a platform with native URL rewriting such as [Azure App Service](azure-app-service.md) or [nginx](nginx.md) is still preferred.

## The Ink demo

The live Ink demo is hosted on GitHub Pages with a `404.html` copy of `index.html` and uses `BrowserHistoryRouter`, so the URL bar reflects navigation state. The `404.html` workaround ensures that refreshing or sharing a deep link still loads the app correctly.
