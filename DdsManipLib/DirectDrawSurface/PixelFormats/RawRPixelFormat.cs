using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public abstract class RawRPixelFormat : RawPixelFormat, IRawRPixelFormat {
    public abstract float GetRed(ReadOnlySpan<byte> pixel);
    public abstract void SetRed(Span<byte> pixel, float value);
}
