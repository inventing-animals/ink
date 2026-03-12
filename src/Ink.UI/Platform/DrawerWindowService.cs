using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

namespace Ink.UI.Platform;

/// <summary>
/// Mobile / web implementation: displays content as a bottom-sheet drawer
/// using Avalonia's <see cref="OverlayLayer"/>, without spawning a new OS window.
/// </summary>
public sealed class DrawerWindowService : IWindowService
{
    private readonly Func<TopLevel?> _topLevelProvider;

    /// <param name="topLevelProvider">
    /// Resolves the current <see cref="TopLevel"/> at call time.
    /// Typically: <c>() => TopLevel.GetTopLevel(rootView)</c>
    /// </param>
    public DrawerWindowService(Func<TopLevel?> topLevelProvider)
    {
        _topLevelProvider = topLevelProvider;
    }

    public Task<IWindowHandle> OpenAsync(Func<Control> content, WindowOptions? options = null)
    {
        var topLevel     = _topLevelProvider();
        var overlayLayer = topLevel is not null ? OverlayLayer.GetOverlayLayer(topLevel) : null;

        if (overlayLayer is null)
            throw new InvalidOperationException(
                "DrawerWindowService: no OverlayLayer found. " +
                "Ensure the topLevelProvider returns an attached TopLevel.");

        var tcs     = new TaskCompletionSource();
        Control? overlay = null;

        void Close()
        {
            if (overlay is not null)
                overlayLayer.Children.Remove(overlay);
            tcs.TrySetResult();
        }

        overlay = BuildDrawer(content(), options?.Title, Close);
        overlayLayer.Children.Add(overlay);

        return Task.FromResult<IWindowHandle>(new LambdaWindowHandle(Close, tcs.Task));
    }

    private static Control BuildDrawer(Control content, string? title, Action onClose)
    {
        // ── Backdrop ─────────────────────────────────────────────────
        var backdrop = new Border
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment   = VerticalAlignment.Stretch,
            Background          = new SolidColorBrush(Color.FromArgb(0x80, 0, 0, 0)),
        };
        backdrop.PointerPressed += (_, e) =>
        {
            e.Handled = true;
            onClose();
        };

        // ── Drag handle pill ─────────────────────────────────────────
        var handle = new Border
        {
            Width       = 32,
            Height      = 4,
            CornerRadius = new CornerRadius(2),
            Background  = GetThemeBrush("Ink.Brush.Border.Subtle", Brushes.Gray),
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin      = new Thickness(0, 0, 0, 8),
        };

        // ── Card body ────────────────────────────────────────────────
        var stack = new StackPanel { Margin = new Thickness(16, 12, 16, 16) };
        stack.Children.Add(handle);

        if (title is not null)
        {
            stack.Children.Add(new TextBlock
            {
                Text       = title,
                FontWeight = FontWeight.SemiBold,
                Margin     = new Thickness(0, 0, 0, 12),
            });
        }

        stack.Children.Add(content);

        var card = new Border
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment   = VerticalAlignment.Bottom,
            CornerRadius        = new CornerRadius(12, 12, 0, 0),
            Background          = GetThemeBrush("Ink.Brush.Surface.Panel", Brushes.White),
            Child               = stack,
        };

        // ── Overlay grid (backdrop + card share the same cell) ───────
        var grid = new Grid
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment   = VerticalAlignment.Stretch,
        };
        grid.Children.Add(backdrop);
        grid.Children.Add(card);

        return grid;
    }

    private static IBrush GetThemeBrush(string key, IBrush fallback)
    {
        if (Application.Current?.TryFindResource(
                key, Application.Current.ActualThemeVariant, out var resource) == true
            && resource is IBrush brush)
            return brush;

        return fallback;
    }
}
