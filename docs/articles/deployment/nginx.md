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

## WASM MIME types

Ensure nginx serves `.wasm` files with the correct MIME type. Add to your `http` or `server` block:

```nginx
types {
    application/wasm wasm;
}
```

Or confirm it is included via `mime.types` (most default nginx installations already include this).
