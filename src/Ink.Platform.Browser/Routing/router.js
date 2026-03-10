// Ink router helpers — wired to the browser History API.
// Import this module and call ink.router.registerPopState() once on startup.

export const router = {
    pushState(path) {
        window.history.pushState(null, '', path);
    },

    replaceState(path) {
        window.history.replaceState(null, '', path);
    },

    back() {
        window.history.back();
    },

    forward() {
        window.history.forward();
    },

    registerPopState() {
        window.addEventListener('popstate', () => {
            globalThis.DotNet?.invokeMethodAsync('Ink.Platform.Browser', 'OnPopState');
        });
    }
};

globalThis.ink = { ...globalThis.ink, router };
