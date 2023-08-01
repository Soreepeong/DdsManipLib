using System;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

/// <summary>
/// How the transparency is encoded in a pixel.
/// </summary>
[Flags]
public enum AlphaType : byte {
    /// <summary>
    /// Transparency is not used.
    /// </summary>
    None = 0,

    /// <summary>
    /// Color values and alpha value are not associated. 
    /// </summary>
    Straight = 1,

    /// <summary>
    /// Color values are premultiplied with alpha value.
    /// </summary>
    Premultiplied = 2,

    /// <summary>
    /// Nonstandard?
    /// </summary>
    Custom = 4,
    
    /// <summary>
    /// May contain any alpha type.
    /// </summary>
    All = Straight | Premultiplied | Custom,
}
