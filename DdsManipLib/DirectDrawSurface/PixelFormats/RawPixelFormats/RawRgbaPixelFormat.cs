using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public abstract class RawRgbaPixelFormat : RawRgbPixelFormat, IRawRgbaPixelFormat {
    public abstract float GetAlpha(ReadOnlySpan<byte> pixel);
    public abstract void SetAlpha(Span<byte> pixel, float value);
    protected RawRgbaPixelFormat(AlphaType alphaType) : base(alphaType) { }
}
