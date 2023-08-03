using System;
using System.Numerics;
using DdsManipLib.Utilities;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public interface IRawRgbaPixelFormat : IRawRgbPixelFormat, IRawAlphaPixelFormat {
    public Vector4 GetRgba(ReadOnlySpan<byte> pixel) => new(GetRed(pixel), GetGreen(pixel), GetBlue(pixel), GetAlpha(pixel));

    public void SetRgba(Span<byte> pixel, Vector4 rgba) {
        SetRed(pixel, rgba.X);
        SetGreen(pixel, rgba.Y);
        SetBlue(pixel, rgba.Z);
        SetAlpha(pixel, rgba.W);
    }

    void IRawRgPixelFormat.SetRg(Span<byte> pixel, Vector2 rg) => SetRgba(pixel, new(rg, float.MaxValue, float.MaxValue));

    void IRawRgbPixelFormat.SetRgb(Span<byte> pixel, Vector3 rgb) => SetRgba(pixel, new(rgb, float.MaxValue));

    public void CopyTo(ReadOnlySpan<byte> sourceSpan, int width, int height, IRawRgbaPixelFormat targetPixelFormat, Span<byte> targetSpan) {
        var sourcePitch = CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = BitsPerPixel;
        var targetBpp = BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..]) {
            for (var x = 0; x < width; x++) {
                targetPixelFormat.SetRgba(targetSpan[(x * targetBpp)..], GetRgba(sourceSpan[(x * sourceBpp)..]));
            }
        }
    }
}

public interface IRawRgbaPixelFormat<T> : IRawRgbaPixelFormat, IRawRgbPixelFormat<T>, IRawAlphaPixelFormat<T>
    where T : unmanaged, IMinMaxValue<T> {
    public Vector4<T> GetRgbaTyped(ReadOnlySpan<byte> pixel) => new(GetRedTyped(pixel), GetGreenTyped(pixel), GetBlueTyped(pixel), GetAlphaTyped(pixel));

    public void SetRgba(Span<byte> pixel, Vector4<T> rgba) {
        SetRed(pixel, rgba.X);
        SetGreen(pixel, rgba.Y);
        SetBlue(pixel, rgba.Z);
        SetAlpha(pixel, rgba.W);
    }

    void IRawRgPixelFormat<T>.SetRg(Span<byte> pixel, Vector2<T> rg) => SetRgba(pixel, new(rg, T.MaxValue, T.MaxValue));

    void IRawRgbPixelFormat<T>.SetRgb(Span<byte> pixel, Vector3<T> rgba) => SetRgba(pixel, new(rgba, T.MaxValue));

    public void CopyTo(ReadOnlySpan<byte> sourceSpan, int width, int height, IRawRgbaPixelFormat<T> targetPixelFormat, Span<byte> targetSpan) {
        var sourcePitch = CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = BitsPerPixel;
        var targetBpp = BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..]) {
            for (var x = 0; x < width; x++) {
                targetPixelFormat.SetRgba(targetSpan[(x * targetBpp)..], GetRgbaTyped(sourceSpan[(x * sourceBpp)..]));
            }
        }
    }
}
