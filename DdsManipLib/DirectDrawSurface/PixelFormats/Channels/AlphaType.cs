namespace DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

/// <summary>
/// How the transparency is encoded in a pixel.
/// </summary>
public enum AlphaType : byte {
    /// <summary>
    /// Transparency is not used.
    /// </summary>
    None,

    /// <summary>
    /// Color values and alpha value are not associated. 
    /// </summary>
    Straight,

    /// <summary>
    /// Color values are premultiplied with alpha value.
    /// </summary>
    Premultiplied,

    /// <summary>
    /// Nonstandard?
    /// </summary>
    Custom,
}
