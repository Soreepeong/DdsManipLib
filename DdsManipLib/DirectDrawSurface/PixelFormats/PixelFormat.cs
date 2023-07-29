using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

/// <summary>
/// Represent a pixel format.
/// </summary>
public abstract class PixelFormat {
    /// <summary>
    /// How the pixel values are represented, with respect to the alpha value.
    /// </summary>
    public AlphaType Alpha { get; protected init; }

    /// <summary>
    /// Number of bits per pixel.
    /// </summary>
    public int Bpp { get; protected init; }

    /// <summary>
    /// Associated <see cref="DxgiFormat"/>.
    /// </summary>
    public virtual DxgiFormat DxgiFormat => this.ToDxgiFormat();

    /// <summary>
    /// Associated <see cref="DdsFourCc"/>.
    /// </summary>
    public virtual DdsFourCc FourCc => this.ToFourCc();

    /// <summary>
    /// Whether this object stands for B8G8R8A8Unorm pixel format. 
    /// </summary>
    public bool IsB8G8R8A8Unorm => this is RgbaPixelFormat {
        B: {Mask: 0x000000FFu, Type: ChannelType.Unorm},
        G: {Mask: 0x0000FF00u, Type: ChannelType.Unorm},
        R: {Mask: 0x00FF0000u, Type: ChannelType.Unorm},
        A: {Mask: 0xFF000000u, Type: ChannelType.Unorm},
        X1.IsEmpty: true,
        X2.IsEmpty: true,
    };

    /// <summary>
    /// Calculate the pitch of image in bytes from the given width.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <returns>Pitch, in bytes.</returns>
    public virtual int CalculatePitch(int width) => (Bpp * width + 7) / 8;

    /// <summary>
    /// Calculate the size of image in bytes of given dimensions.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <returns>Number of bytes required to fully store an image of given dimensions in current pixel format.</returns>
    public virtual int CalculateLinearSize(int width, int height) => CalculatePitch(width) * height;
}
