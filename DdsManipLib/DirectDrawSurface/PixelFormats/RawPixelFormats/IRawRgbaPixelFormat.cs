using System;
using System.Numerics;
using DdsManipLib.Utilities;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public interface IRawRgbaPixelFormat : IRawRgbPixelFormat, IRawAPixelFormat {
    public Vector4 GetRgba(ReadOnlySpan<byte> pixel) => new(GetRed(pixel), GetGreen(pixel), GetBlue(pixel), GetAlpha(pixel));

    public void SetRgba(Span<byte> pixel, Vector4 rgba) {
        SetRed(pixel, rgba.X);
        SetGreen(pixel, rgba.Y);
        SetBlue(pixel, rgba.Z);
        SetAlpha(pixel, rgba.W);
    }

    void IRawRgPixelFormat.SetRg(Span<byte> pixel, Vector2 rg) => SetRgba(pixel, new(rg, float.MaxValue, float.MaxValue));

    void IRawRgbPixelFormat.SetRgb(Span<byte> pixel, Vector3 rgb) => SetRgba(pixel, new(rgb, float.MaxValue));
}

public interface IRawRgbaPixelFormat<T> : IRawRgbaPixelFormat, IRawRgbPixelFormat<T>, IRawAPixelFormat<T>
    where T : unmanaged, IMinMaxValue<T>, IBinaryNumber<T> {
    public Vector4<T> GetRgbaTyped(ReadOnlySpan<byte> pixel) => new(GetRedTyped(pixel), GetGreenTyped(pixel), GetBlueTyped(pixel), GetAlphaTyped(pixel));

    public void SetRgba(Span<byte> pixel, Vector4<T> rgba) {
        SetRed(pixel, rgba.X);
        SetGreen(pixel, rgba.Y);
        SetBlue(pixel, rgba.Z);
        SetAlpha(pixel, rgba.W);
    }

    void IRawRgPixelFormat<T>.SetRg(Span<byte> pixel, Vector2<T> rg) => SetRgba(pixel, new(rg, T.MaxValue, T.MaxValue));

    void IRawRgbPixelFormat<T>.SetRgb(Span<byte> pixel, Vector3<T> rgba) => SetRgba(pixel, new(rgba, T.MaxValue));
}
