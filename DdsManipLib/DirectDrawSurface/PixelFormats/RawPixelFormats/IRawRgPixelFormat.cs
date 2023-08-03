using System;
using System.Numerics;
using DdsManipLib.Utilities;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public interface IRawRgPixelFormat : IRawRPixelFormat, IRawGPixelFormat {
    public Vector2 GetRg(ReadOnlySpan<byte> pixel) => new(GetRed(pixel), GetGreen(pixel));

    public void SetRg(Span<byte> pixel, Vector2 rg) {
        SetRed(pixel, rg.X);
        SetGreen(pixel, rg.Y);
    }
}

public interface IRawRgPixelFormat<T> : IRawRgPixelFormat, IRawRPixelFormat<T>, IRawGPixelFormat<T>
    where T : unmanaged {
    public Vector2<T> GetRgTyped(ReadOnlySpan<byte> pixel) => new(GetRedTyped(pixel), GetGreenTyped(pixel));

    public void SetRg(Span<byte> pixel, Vector2<T> rg) {
        SetRed(pixel, rg.X);
        SetGreen(pixel, rg.Y);
    }
}
