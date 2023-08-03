using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public abstract class RawRgPixelFormat : RawRPixelFormat, IRawRgPixelFormat {
    public abstract float GetGreen(ReadOnlySpan<byte> pixel);
    public abstract void SetGreen(Span<byte> pixel, float value);
    protected RawRgPixelFormat(AlphaType alphaType) : base(alphaType) { }
}
