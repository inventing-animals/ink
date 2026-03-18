using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Ink.UI.Platform;

namespace Ink.UI.Controls;

/// <summary>
/// Root window for desktop applications that want secondary surfaces opened as tabs
/// rather than spawning separate OS windows.
/// The tab strip is hidden when there are no secondary tabs. When at least one secondary
/// tab is open, the strip shows the main window as its first (non-closeable) tab followed
/// by the secondary tabs.
/// </summary>
/// <remarks>
/// Use as the top-level <see cref="Window"/> in your desktop entry point and pair it
/// with <see cref="DesktopTabbedWindowService"/> to redirect <see cref="IWindowService"/>
/// calls into tabs.
/// </remarks>
public class DesktopTabbedWindow : DesktopWindow
{
    // _selectedIndex == -1  →  home tab (MainContent) is active
    // _selectedIndex >= 0   →  that secondary tab is active
    private sealed record TabEntry(string Title, Control Content)
    {
        public TaskCompletionSource Tcs { get; } = new();
    }

    private readonly List<TabEntry> _tabs = [];
    private readonly StackPanel _tabStrip;
    private readonly Border _tabStripContainer;
    private readonly Border _contentArea;

    private int _selectedIndex = -1;
    private Control? _mainContent;

    /// <summary>
    /// Gets or sets the persistent content shown when no secondary tab is active.
    /// Set this to the application's main view; do not set
    /// <see cref="Avalonia.Controls.ContentControl.Content"/> directly, as it is
    /// managed internally by this window.
    /// </summary>
    public new Control? MainContent
    {
        get => _mainContent;
        set
        {
            _mainContent = value;
            if (_selectedIndex < 0)
                _contentArea.Child = value;
        }
    }

    /// <inheritdoc />
    public DesktopTabbedWindow()
    {
        _tabStrip = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 1,
        };

        _tabStripContainer = new Border
        {
            IsVisible = false,
            BorderThickness = new Thickness(0, 0, 0, 1),
            Padding = new Thickness(8, 6, 8, 0),
            Child = _tabStrip,
        };

        _contentArea = new Border
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
        };

        var grid = new Grid();
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
        Grid.SetRow(_tabStripContainer, 0);
        Grid.SetRow(_contentArea, 1);
        grid.Children.Add(_tabStripContainer);
        grid.Children.Add(_contentArea);

        OverlayLayer.Content = grid;
    }

    private static IBrush GetThemeBrush(string key, IBrush fallback)
    {
        if (Application.Current?.TryFindResource(
                key, Application.Current.ActualThemeVariant, out var resource) == true
            && resource is IBrush brush)
            return brush;

        return fallback;
    }

    internal IWindowHandle AddTab(string? title, Control content)
    {
        var entry = new TabEntry(title ?? string.Empty, content);
        _tabs.Add(entry);
        _selectedIndex = _tabs.Count - 1;
        _contentArea.Child = content;
        _tabStripContainer.IsVisible = true;
        RebuildTabStrip();
        return new LambdaWindowHandle(() => RemoveTab(entry), entry.Tcs.Task);
    }

    // index == -1 selects the home tab
    private void SelectTab(int index)
    {
        _selectedIndex = index;
        _contentArea.Child = index < 0 ? _mainContent : _tabs[index].Content;
        RebuildTabStrip();
    }

    private void RemoveTab(TabEntry entry)
    {
        var idx = _tabs.IndexOf(entry);
        if (idx < 0) return;

        _tabs.RemoveAt(idx);
        entry.Tcs.TrySetResult();

        if (_tabs.Count == 0)
        {
            _selectedIndex = -1;
            _tabStripContainer.IsVisible = false;
            _contentArea.Child = _mainContent;
            return;
        }

        // Keep selection valid; fall back to home if removed tab was selected
        if (_selectedIndex >= _tabs.Count)
            _selectedIndex = _tabs.Count - 1;

        _contentArea.Child = _selectedIndex < 0 ? _mainContent : _tabs[_selectedIndex].Content;
        RebuildTabStrip();
    }

    private void RebuildTabStrip()
    {
        var selectedBg = GetThemeBrush("Ink.Brush.Surface.Raised", Brushes.White);
        var borderBrush = GetThemeBrush("Ink.Brush.Border.Subtle", Brushes.LightGray);
        var fgPrimary = GetThemeBrush("Ink.Brush.Content.Primary", Brushes.Black);
        var fgSecondary = GetThemeBrush("Ink.Brush.Content.Secondary", Brushes.Gray);

        _tabStripContainer.BorderBrush = borderBrush;
        _tabStrip.Children.Clear();

        // ── Home tab (always first, no close button) ──────────────────
        var homeSelected = _selectedIndex < 0;
        var homeButton = new Button
        {
            Content = Title ?? string.Empty,
            Padding = new Thickness(10, 4),
            MinHeight = 0,
            CornerRadius = new CornerRadius(4, 4, 0, 0),
            Background = homeSelected ? selectedBg : Brushes.Transparent,
            BorderBrush = homeSelected ? borderBrush : Brushes.Transparent,
            BorderThickness = homeSelected ? new Thickness(1, 1, 1, 0) : new Thickness(0),
            Foreground = fgPrimary,
        };
        homeButton.Click += (_, _) => SelectTab(-1);
        _tabStrip.Children.Add(homeButton);

        // ── Secondary tabs ────────────────────────────────────────────
        for (var i = 0; i < _tabs.Count; i++)
        {
            var entry = _tabs[i];
            var isSelected = i == _selectedIndex;
            var capturedIdx = i;
            var capturedEntry = entry;

            var titleButton = new Button
            {
                Content = entry.Title,
                Padding = new Thickness(10, 4),
                MinHeight = 0,
                CornerRadius = new CornerRadius(4, 4, 0, 0),
                Background = isSelected ? selectedBg : Brushes.Transparent,
                BorderBrush = isSelected ? borderBrush : Brushes.Transparent,
                BorderThickness = isSelected ? new Thickness(1, 1, 1, 0) : new Thickness(0),
                Foreground = fgPrimary,
            };
            titleButton.Click += (_, _) => SelectTab(capturedIdx);

            var closeButton = new Button
            {
                Content = "×",
                Padding = new Thickness(6, 4),
                MinHeight = 0,
                CornerRadius = new CornerRadius(4),
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Foreground = fgSecondary,
                FontSize = 14,
            };
            closeButton.Click += (_, _) => RemoveTab(capturedEntry);

            var tabItem = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 2, 0),
            };
            tabItem.Children.Add(titleButton);
            tabItem.Children.Add(closeButton);

            _tabStrip.Children.Add(tabItem);
        }
    }
}
