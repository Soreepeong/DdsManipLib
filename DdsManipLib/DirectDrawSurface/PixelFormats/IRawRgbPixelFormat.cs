using System;
using System.Numerics;
using DdsManipLib.Utilities;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public interface IRawRgbPixelFormat : IRawRgPixelFormat {
    public float GetBlue(ReadOnlySpan<byte> pixel);
    public void SetBlue(Span<byte> pixel, float value);

    void IRawRgPixelFormat.SetRg(Span<byte> pixel, Vector2 rg) => SetRgb(pixel, new(rg, float.MaxValue));

    public Vector3 GetRgb(ReadOnlySpan<byte> pixel) => new(GetRed(pixel), GetGreen(pixel), GetBlue(pixel));

    public void SetRgb(Span<byte> pixel, Vector3 rgb) {
        SetRed(pixel, rgb.X);
        SetGreen(pixel, rgb.Y);
        SetBlue(pixel, rgb.Z);
    }

    public void CopyTo(ReadOnlySpan<byte> sourceSpan, int width, int height, IRawRgbPixelFormat targetPixelFormat, Span<byte> targetSpan) {
        var sourcePitch = CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = BitsPerPixel;
        var targetBpp = BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..]) {
            for (var x = 0; x < width; x++) {
                targetPixelFormat.SetRgb(targetSpan[(x * targetBpp)..], GetRgb(sourceSpan[(x * sourceBpp)..]));
            }
        }
    }
}

public interface IRawRgbPixelFormat<T> : IRawRgbPixelFormat, IRawRgPixelFormat<T>
    where T : unmanaged, IMinMaxValue<T> {
    public T GetBlueTyped(ReadOnlySpan<byte> pixel);
    public void SetBlue(Span<byte> pixel, T value);

    void IRawRgPixelFormat<T>.SetRg(Span<byte> pixel, Vector2<T> rg) => SetRgb(pixel, new(rg, T.MaxValue));

    public Vector3<T> GetRgbTyped(ReadOnlySpan<byte> pixel) => new(GetRedTyped(pixel), GetGreenTyped(pixel), GetBlueTyped(pixel));

    public void SetRgb(Span<byte> pixel, Vector3<T> rgb) {
        SetRed(pixel, rgb.X);
        SetGreen(pixel, rgb.Y);
        SetBlue(pixel, rgb.Z);
    }

    public void CopyTo(ReadOnlySpan<byte> sourceSpan, int width, int height, IRawRgbPixelFormat<T> targetPixelFormat, Span<byte> targetSpan) {
        var sourcePitch = CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = BitsPerPixel;
        var targetBpp = BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..]) {
            for (var x = 0; x < width; x++) {
                targetPixelFormat.SetRgb(targetSpan[(x * targetBpp)..], GetRgbTyped(sourceSpan[(x * sourceBpp)..]));
            }
        }
    }
}
