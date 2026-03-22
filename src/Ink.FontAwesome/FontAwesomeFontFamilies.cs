using Avalonia.Media;

namespace Ink.FontAwesome;

/// <summary>
/// Consumer-configurable font family mapping for Font Awesome faces.
/// </summary>
public static class FontAwesomeFontFamilies
{
    /// <summary>
    /// Gets or sets the font family used for the classic regular face.
    /// </summary>
    public static FontFamily? ClassicRegular { get; set; }

    /// <summary>
    /// Gets or sets the font family used for the classic solid face.
    /// </summary>
    public static FontFamily? ClassicSolid { get; set; }

    /// <summary>
    /// Gets or sets the font family used for the classic light face.
    /// </summary>
    public static FontFamily? ClassicLight { get; set; }

    /// <summary>
    /// Gets or sets the font family used for the classic thin face.
    /// </summary>
    public static FontFamily? ClassicThin { get; set; }

    /// <summary>
    /// Gets or sets the font family used for the duotone regular face.
    /// </summary>
    public static FontFamily? DuotoneRegular { get; set; }

    /// <summary>
    /// Gets or sets the font family used for the duotone solid face.
    /// </summary>
    public static FontFamily? DuotoneSolid { get; set; }

    /// <summary>
    /// Gets or sets the font family used for the duotone light face.
    /// </summary>
    public static FontFamily? DuotoneLight { get; set; }

    /// <summary>
    /// Gets or sets the font family used for the duotone thin face.
    /// </summary>
    public static FontFamily? DuotoneThin { get; set; }

    /// <summary>
    /// Gets or sets the font family used for the sharp regular face.
    /// </summary>
    public static FontFamily? SharpRegular { get; set; }

    /// <summary>
    /// Gets or sets the font family used for the sharp solid face.
    /// </summary>
    public static FontFamily? SharpSolid { get; set; }

    /// <summary>
    /// Gets or sets the font family used for the sharp light face.
    /// </summary>
    public static FontFamily? SharpLight { get; set; }

    /// <summary>
    /// Gets or sets the font family used for the sharp thin face.
    /// </summary>
    public static FontFamily? SharpThin { get; set; }

    /// <summary>
    /// Gets or sets the font family used for the sharp duotone regular face.
    /// </summary>
    public static FontFamily? SharpDuotoneRegular { get; set; }

    /// <summary>
    /// Gets or sets the font family used for the sharp duotone solid face.
    /// </summary>
    public static FontFamily? SharpDuotoneSolid { get; set; }

    /// <summary>
    /// Gets or sets the font family used for the sharp duotone light face.
    /// </summary>
    public static FontFamily? SharpDuotoneLight { get; set; }

    /// <summary>
    /// Gets or sets the font family used for the sharp duotone thin face.
    /// </summary>
    public static FontFamily? SharpDuotoneThin { get; set; }

    /// <summary>
    /// Gets or sets the font family used for the brands face.
    /// </summary>
    public static FontFamily? Brands { get; set; }

    internal static FontFamily? GetFontFamily(Face face)
        => face switch
        {
            Face.ClassicRegular => ClassicRegular,
            Face.ClassicSolid => ClassicSolid,
            Face.ClassicLight => ClassicLight,
            Face.ClassicThin => ClassicThin,
            Face.DuotoneRegular => DuotoneRegular,
            Face.DuotoneSolid => DuotoneSolid,
            Face.DuotoneLight => DuotoneLight,
            Face.DuotoneThin => DuotoneThin,
            Face.SharpRegular => SharpRegular,
            Face.SharpSolid => SharpSolid,
            Face.SharpLight => SharpLight,
            Face.SharpThin => SharpThin,
            Face.SharpDuotoneRegular => SharpDuotoneRegular,
            Face.SharpDuotoneSolid => SharpDuotoneSolid,
            Face.SharpDuotoneLight => SharpDuotoneLight,
            Face.SharpDuotoneThin => SharpDuotoneThin,
            Face.Brands => Brands,
            _ => null,
        };
}
