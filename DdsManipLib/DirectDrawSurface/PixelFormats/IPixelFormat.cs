namespace DdsManipLib.DirectDrawSurface.PixelFormats;

/// <summary>
/// An interface for pixel formats.
/// </summary>
public interface IPixelFormat {
    /// <summary>
    /// Corresponding <see cref="DxgiFormat"/> for this <see cref="IPixelFormat"/>.
    /// </summary>
    /// <remarks>
    /// Contains <see cref="DirectDrawSurface.DxgiFormat.Unknown"/> if not representable.
    /// </remarks>
    public DxgiFormat DxgiFormat { get; }
    
    /// <summary>
    /// Corresponding <see cref="DdsPixelFormat"/> for this <see cref="IPixelFormat"/>.
    /// </summary>
    /// <remarks>
    /// Contains <see cref="DdsFourCc.Dx10"/> if not representable solely via <see cref="DdsPixelFormat"/>.
    /// Note that <see cref="DdsHeaderDxt10"/> still may indicate that the pixel format has no corresponding <see cref="DxgiFormat"/>.
    /// </remarks>
    public DdsPixelFormat DdsPixelFormat { get; }

    /// <summary>
    /// How to interpret alpha values.
    /// </summary>
    public AlphaType AlphaType { get; }
    
    /// <summary>
    /// Number of bits per pixel (bpp).
    /// </summary>
    public int BitsPerPixel { get; }

    /// <summary>
    /// Calculate the pitch(stride) for the given dimension.
    /// </summary>
    /// <param name="width">Width of the image.</param>
    /// <returns>Corresponding pitch(stride) in bytes.</returns>
    public int CalculatePitch(int width);

    /// <summary>
    /// Calculate the linear size for the given dimensions of a 2D image.
    /// </summary>
    /// <param name="width">Width of the image.</param>
    /// <param name="height">Height of the image.</param>
    /// <returns>Corresponding linear size in bytes.</returns>
    public int CalculateLinearSize(int width, int height);

    /// <summary>
    /// Determine if the <see cref="DdsPixelFormat"/> can be represented by the current <see cref="IPixelFormat"/>.
    /// </summary>
    /// <param name="ddspf">The DDS pixel format to determine.</param>
    /// <returns>Whether it supports the pixel format.</returns>
    public bool IsDdsPixelFormat(in DdsPixelFormat ddspf) => ddspf == DdsPixelFormat;
}
