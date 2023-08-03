using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public abstract class RawRgbaPixelFormat : RawRgbPixelFormat, IRawRgbaPixelFormat {
    public abstract AlphaType AlphaType { get; }
    public abstract float GetAlpha(ReadOnlySpan<byte> pixel);
    public abstract void SetAlpha(Span<byte> pixel, float value);
}
