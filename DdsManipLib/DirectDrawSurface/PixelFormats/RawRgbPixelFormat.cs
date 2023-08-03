using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public abstract class RawRgbPixelFormat : RawRgPixelFormat, IRawRgbPixelFormat {
    public abstract float GetBlue(ReadOnlySpan<byte> pixel);
    public abstract void SetBlue(Span<byte> pixel, float value);
}
