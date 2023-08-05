using System;
using System.Numerics;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public interface IRawAPixelFormat : IRawPixelFormat {
    public float GetAlpha(ReadOnlySpan<byte> pixel);
    public void SetAlpha(Span<byte> pixel, float value);
}

public interface IRawAPixelFormat<T> : IRawPixelFormat<T>, IRawAPixelFormat where T : unmanaged, IBinaryNumber<T> {
    public T GetAlphaTyped(ReadOnlySpan<byte> pixel);
    public void SetAlpha(Span<byte> pixel, T value);
}
