# Deploying with nginx

nginx is commonly used on Linux App Service, self-hosted VMs, and container deployments. The `try_files` directive serves `index.html` as a fallback for any path that does not match a real file.

## URL rewriting

In your `server` block:

```nginx
server {
    listen 80;
    root /var/www/app;
    index index.html;

    location / {
        try_files $uri $uri/ /index.html;
    }
}
```

The `try_files` directive first checks if the request matches a file (`$uri`) or directory (`$uri/`), and falls back to `/index.html` if neither exists. This lets `BrowserHistoryRouter` handle the path client-side.

## Sub-path hosting

If the app lives at a sub-path (e.g. `/app/`), scope the `location` block and set the fallback accordingly:

```nginx
location /app/ {
    try_files $uri $uri/ /app/index.html;
}
```

Then set `<base href="/app/" />` in `index.html`, or inject it at publish time:

```bash
dotnet publish src/MyApp.Browser -p:BaseHref=/app/
```

`BrowserHistoryRouter` reads the base href at startup and strips it from all paths automatically. See [Base URL / sub-path hosting](../platform.md#base-url--sub-path-hosting) for details.

## WASM MIME types

Ensure nginx serves `.wasm` files with the correct MIME type. Add to your `http` or `server` block:

```nginx
types {
    application/wasm wasm;
}
```

Or confirm it is included via `mime.types` (most default nginx installations already include this).
