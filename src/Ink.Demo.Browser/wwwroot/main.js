import { dotnet } from './_framework/dotnet.js'

const is_browser = typeof window != "undefined";
if (!is_browser) throw new Error(`Expected to be running in a browser`);

// Ink router helpers — must be set up before the .NET runtime starts
globalThis.ink = {
    router: {
        getCurrentUrl: () => window.location.href,
        pushState: (path) => window.history.pushState(null, '', path),
        replaceState: (path) => window.history.replaceState(null, '', path),
        back: () => window.history.back(),
        forward: () => window.history.forward(),
        registerPopState: (callback) => {
            window.addEventListener('popstate', () => callback());
        }
    }
};

function showError(message, detail) {
    const banner = document.getElementById('error-banner');
    if (!banner) return;
    banner.style.display = 'block';
    banner.textContent = '[Ink] ' + message + (detail ? '\n\n' + detail : '');
    console.error('[Ink]', message, detail ?? '');
}

let dotnetRuntime;
try {
    dotnetRuntime = await dotnet
        .withDiagnosticTracing(false)
        .withApplicationArgumentsFromQuery()
        .create();
} catch (e) {
    showError('Failed to initialise the .NET runtime.', e?.message ?? String(e));
    throw e;
}

const config = dotnetRuntime.getConfig();

try {
    await dotnetRuntime.runMain(config.mainAssemblyName, [globalThis.location.href]);
} catch (e) {
    showError('The application crashed during startup.', e?.message ?? String(e));
    throw e;
}
