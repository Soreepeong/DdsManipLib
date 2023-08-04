using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public interface IRawGPixelFormat : IRawPixelFormat {
    public float GetGreen(ReadOnlySpan<byte> pixel);
    public void SetGreen(Span<byte> pixel, float value);
}

public interface IRawGPixelFormat<T> : IRawPixelFormat<T>, IRawGPixelFormat
    where T : unmanaged {
    public T GetGreenTyped(ReadOnlySpan<byte> pixel);
    public void SetGreen(Span<byte> pixel, T value);
}
