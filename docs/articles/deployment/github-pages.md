# Deploying to GitHub Pages

GitHub Pages is a static CDN with no server-side request handling. It cannot rewrite URLs to `index.html`, so `BrowserHistoryRouter` — which relies on the History API and path-based URLs — does not fully work when a user manually enters a URL or refreshes the page.

## Limitation

When navigating within the app, `BrowserHistoryRouter` works correctly: the URL updates and the router state stays in sync. The limitation only affects:

- Manually typing a URL into the address bar
- Refreshing the page on any route other than `/`
- Sharing a deep link

In these cases, GitHub Pages returns a 404 because the path does not correspond to a real file.

## Workaround: 404.html with path restoration

GitHub Pages serves a single `404.html` from the **site root** when any path is not found. The trick is to encode the original path in a query parameter, redirect to the app root, and then restore the path via `history.replaceState` before the WASM runtime starts.

**`404.html`** (place at the site root):

```html
<script>
    var target = '/?_p=' + encodeURIComponent(
        window.location.pathname + window.location.search + window.location.hash
    );
    window.location.replace(target);
</script>
```

**`index.html`** (add before the WASM module script):

```html
<script>
    var p = new URLSearchParams(window.location.search).get('_p');
    if (p) window.history.replaceState(null, '', p);
</script>
```

Because the restore script is a regular `<script>` tag (not a module), it runs synchronously before the WASM module loads. `BrowserHistoryRouter` then reads the correct URL from `window.location.href` during startup.

For a production B2B application, a platform with native URL rewriting such as [Azure App Service](azure-app-service.md) or [nginx](nginx.md) is still preferred.

## The Ink demo

The live Ink demo is hosted on GitHub Pages using `BrowserHistoryRouter` with the `404.html` path-restoration workaround described above.
