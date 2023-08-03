using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public interface IRawAlphaPixelFormat : IRawPixelFormat {
    public AlphaType AlphaType { get; }
    public float GetAlpha(ReadOnlySpan<byte> pixel);
    public void SetAlpha(Span<byte> pixel, float value);

    public void CopyTo(ReadOnlySpan<byte> sourceSpan, int width, int height, IRawAlphaPixelFormat targetPixelFormat, Span<byte> targetSpan) {
        var sourcePitch = CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = BitsPerPixel;
        var targetBpp = BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..]) {
            for (var x = 0; x < width; x++) {
                targetPixelFormat.SetAlpha(targetSpan[(x * targetBpp)..], GetAlpha(sourceSpan[(x * sourceBpp)..]));
            }
        }
    }
}

public interface IRawAlphaPixelFormat<T> : IRawAlphaPixelFormat {
    public T GetAlphaTyped(ReadOnlySpan<byte> pixel);
    public void SetAlpha(Span<byte> pixel, T value);

    public void CopyTo(ReadOnlySpan<byte> sourceSpan, int width, int height, IRawAlphaPixelFormat<T> targetPixelFormat, Span<byte> targetSpan) {
        var sourcePitch = CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = BitsPerPixel;
        var targetBpp = BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..]) {
            for (var x = 0; x < width; x++) {
                targetPixelFormat.SetAlpha(targetSpan[(x * targetBpp)..], GetAlphaTyped(sourceSpan[(x * sourceBpp)..]));
            }
        }
    }
}
