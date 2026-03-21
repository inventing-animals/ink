using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;

namespace Ink.UI.Controls;

/// <summary>
/// A titled header with an optional description and a close button, designed to sit at
/// the top of ink popup surfaces such as <see cref="Flyout"/> dropdown content or
/// an <see cref="ComboBox"/> dropdown. The close button dismisses the nearest enclosing
/// <see cref="Popup"/> in the logical tree.
/// </summary>
public class PopupHeader : TemplatedControl
{
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<PopupHeader, string?>(nameof(Title));

    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<PopupHeader, string?>(nameof(Description));

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        var closeButton = e.NameScope.Find<Avalonia.Controls.Button>("PART_CloseButton");
        if (closeButton is not null)
            closeButton.Click += OnCloseClick;
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        var popup = ((ILogical)this).GetSelfAndLogicalAncestors().OfType<Popup>().FirstOrDefault();
        if (popup is not null)
            popup.IsOpen = false;
    }
}
