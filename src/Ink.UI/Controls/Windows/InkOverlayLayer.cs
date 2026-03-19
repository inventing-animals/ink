using System;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;

namespace Ink.UI.Controls;

/// <summary>
/// Owns the root Panel that every Ink window type sets as its Content.
/// The panel stacks an optional main-content slot beneath a dim backdrop overlay.
/// </summary>
internal sealed class InkOverlayLayer
{
    private readonly Border _overlay;
    private Control? _content;
    private int _count;

    public Panel Root { get; }

    /// <summary>The control occupying the main content slot (beneath the overlay).</summary>
    public Control? Content
    {
        get => _content;
        set
        {
            if (_content != null) Root.Children.Remove(_content);
            _content = value;
            if (value != null) Root.Children.Insert(0, value);
        }
    }

    // BlurEffect applied to the content element (not the overlay) when the overlay is visible.
    // Null on browsers where GPU effects may be unavailable.
    private static readonly BlurEffect? ContentBlur =
        OperatingSystem.IsBrowser() ? null : new BlurEffect { Radius = 8 };

    public InkOverlayLayer()
    {
        _overlay = new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(0x90, 0, 0, 0)),
            Opacity = 0,
            IsHitTestVisible = false,
            ZIndex = 1000,
            Transitions = new Transitions
            {
                new DoubleTransition
                {
                    Property = Visual.OpacityProperty,
                    Duration = TimeSpan.FromMilliseconds(200),
                    Easing = new SineEaseInOut(),
                },
            },
        };
        Root = new Panel();
        Root.Children.Add(_overlay);
    }

    public void Show()
    {
        if (++_count == 1)
        {
            if (_content is not null) _content.Effect = ContentBlur;
            _overlay.IsHitTestVisible = true;
            _overlay.Opacity = 1;
        }
    }

    public void Hide()
    {
        if (_count > 0 && --_count == 0)
        {
            if (_content is not null) _content.Effect = null;
            _overlay.IsHitTestVisible = false;
            _overlay.Opacity = 0;
        }
    }
}
