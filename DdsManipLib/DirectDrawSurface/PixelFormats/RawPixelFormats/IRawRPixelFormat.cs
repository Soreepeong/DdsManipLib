using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public interface IRawRPixelFormat : IRawPixelFormat {
    public float GetRed(ReadOnlySpan<byte> pixel);
    public void SetRed(Span<byte> pixel, float value);
}

public interface IRawRPixelFormat<T> : IRawRPixelFormat
    where T : unmanaged {
    public T GetRedTyped(ReadOnlySpan<byte> pixel);
    public void SetRed(Span<byte> pixel, T value);
}
