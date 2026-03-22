using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace Ink.FontAwesome;

/// <summary>
/// Displays a Font Awesome glyph using a strongly typed icon identifier and face.
/// </summary>
public class FontAwesomeIcon : TextBlock
{
    /// <summary>
    /// Identifies the <see cref="Icon"/> property.
    /// </summary>
    public static readonly StyledProperty<Icon> IconProperty =
        AvaloniaProperty.Register<FontAwesomeIcon, Icon>(nameof(Icon));

    /// <summary>
    /// Identifies the <see cref="Face"/> property.
    /// </summary>
    public static readonly StyledProperty<Face> FaceProperty =
        AvaloniaProperty.Register<FontAwesomeIcon, Face>(nameof(Face), Face.ClassicSolid);

    static FontAwesomeIcon()
    {
        IconProperty.Changed.AddClassHandler<FontAwesomeIcon>(OnIconChanged);
        FaceProperty.Changed.AddClassHandler<FontAwesomeIcon>(OnFaceChanged);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FontAwesomeIcon"/> class.
    /// </summary>
    public FontAwesomeIcon()
    {
        HorizontalAlignment = HorizontalAlignment.Center;
        VerticalAlignment = VerticalAlignment.Center;
        TextAlignment = TextAlignment.Center;
        UpdatePresentation();
    }

    /// <summary>
    /// Gets or sets the icon identifier.
    /// </summary>
    public Icon Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets or sets the requested icon face.
    /// </summary>
    public Face Face
    {
        get => GetValue(FaceProperty);
        set => SetValue(FaceProperty, value);
    }

    private static void OnIconChanged(FontAwesomeIcon icon, AvaloniaPropertyChangedEventArgs e)
        => icon.UpdatePresentation();

    private static void OnFaceChanged(FontAwesomeIcon icon, AvaloniaPropertyChangedEventArgs e)
        => icon.UpdatePresentation();

    private void UpdatePresentation()
    {
        if (Icon == Icon.None)
        {
            Text = string.Empty;
            return;
        }

        var resolvedFace = FontAwesomeMetadata.ResolveFace(Icon, Face);
        Text = FontAwesomeMetadata.GetGlyph(Icon);

        var configuredFontFamily = FontAwesomeFontFamilies.GetFontFamily(resolvedFace);
        if (configuredFontFamily is not null)
            FontFamily = configuredFontFamily;
    }
}
