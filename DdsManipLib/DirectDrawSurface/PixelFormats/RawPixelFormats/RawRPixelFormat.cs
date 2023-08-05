using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public abstract class RawRPixelFormat : RawPixelFormat, IRawRPixelFormat {
    public abstract float GetRed(ReadOnlySpan<byte> pixel);
    public abstract void SetRed(Span<byte> pixel, float value);
    protected RawRPixelFormat(AlphaType alphaType) : base(alphaType) { }

    public override void ClearPixel(Span<byte> pixel) => pixel[..BytesPerPixel].Clear();
}
