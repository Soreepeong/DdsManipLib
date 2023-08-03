using System;
using System.Numerics;
using DdsManipLib.Utilities;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public interface IRawRgPixelFormat : IRawRPixelFormat, IRawGPixelFormat {
    public Vector2 GetRg(ReadOnlySpan<byte> pixel) => new(GetRed(pixel), GetGreen(pixel));

    public void SetRg(Span<byte> pixel, Vector2 rg) {
        SetRed(pixel, rg.X);
        SetGreen(pixel, rg.Y);
    }

    public void CopyTo(ReadOnlySpan<byte> sourceSpan, int width, int height, IRawRgPixelFormat targetPixelFormat, Span<byte> targetSpan) {
        var sourcePitch = CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = BitsPerPixel;
        var targetBpp = BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..]) {
            for (var x = 0; x < width; x++) {
                targetPixelFormat.SetRg(targetSpan[(x * targetBpp)..], GetRg(sourceSpan[(x * sourceBpp)..]));
            }
        }
    }

    void IRawGPixelFormat.CopyTo(ReadOnlySpan<byte> sourceSpan, int width, int height, IRawDsPixelFormat targetPixelFormat, Span<byte> targetSpan) {
        var sourcePitch = CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = BitsPerPixel;
        var targetBpp = BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..]) {
            for (var x = 0; x < width; x++) {
                targetPixelFormat.SetDs(targetSpan[(x * targetBpp)..], GetRg(sourceSpan[(x * sourceBpp)..]));
            }
        }
    }
}

public interface IRawRgPixelFormat<T> : IRawRgPixelFormat, IRawRPixelFormat<T>, IRawGPixelFormat<T>
    where T : unmanaged {
    public Vector2<T> GetRgTyped(ReadOnlySpan<byte> pixel) => new(GetRedTyped(pixel), GetGreenTyped(pixel));

    public void SetRg(Span<byte> pixel, Vector2<T> rg) {
        SetRed(pixel, rg.X);
        SetGreen(pixel, rg.Y);
    }

    public void CopyTo(ReadOnlySpan<byte> sourceSpan, int width, int height, IRawRgPixelFormat<T> targetPixelFormat, Span<byte> targetSpan) {
        var sourcePitch = CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = BitsPerPixel;
        var targetBpp = BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..]) {
            for (var x = 0; x < width; x++) {
                targetPixelFormat.SetRg(targetSpan[(x * targetBpp)..], GetRgTyped(sourceSpan[(x * sourceBpp)..]));
            }
        }
    }
}
