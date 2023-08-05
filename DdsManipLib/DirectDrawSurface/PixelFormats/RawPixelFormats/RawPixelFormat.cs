using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public abstract class RawPixelFormat : IRawPixelFormat {
    public virtual DxgiFormat DxgiFormat => DxgiFormat.Unknown;
    public virtual DdsFourCc FourCc => DdsFourCc.Unknown;
    public virtual DdsPixelFormat DdsPixelFormat => default;
    public AlphaType AlphaType { get; }
    public abstract int BitsPerPixel { get; }
    public abstract int BytesPerPixel { get; }
    public abstract void ClearPixel(Span<byte> pixel);
    public int CalculatePitch(int width) => (BitsPerPixel * width + 7) / 8;
    public int CalculateLinearSize(int width, int height) => (BitsPerPixel * width + 7) / 8 * height;

    protected RawPixelFormat(AlphaType alphaType) => AlphaType = alphaType;
}
