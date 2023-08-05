using System;
using System.Numerics;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public interface IRawDPixelFormat : IRawPixelFormat {
    public float GetDepth(ReadOnlySpan<byte> pixel);
    public void SetDepth(Span<byte> pixel, float value);
}

public interface IRawDPixelFormat<T> : IRawPixelFormat<T>, IRawDPixelFormat
    where T : unmanaged, IBinaryNumber<T> {
    public T GetDepthTyped(ReadOnlySpan<byte> pixel);
    public void SetDepth(Span<byte> pixel, T value);
}
