using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public interface IRawRPixelFormat : IRawPixelFormat {
    public float GetRed(ReadOnlySpan<byte> pixel);
    public void SetRed(Span<byte> pixel, float value);

    public void CopyTo(ReadOnlySpan<byte> sourceSpan, int width, int height, IRawRPixelFormat targetPixelFormat, Span<byte> targetSpan) {
        var sourcePitch = CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = BitsPerPixel;
        var targetBpp = BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..]) {
            for (var x = 0; x < width; x++) {
                targetPixelFormat.SetRed(targetSpan[(x * targetBpp)..], GetRed(sourceSpan[(x * sourceBpp)..]));
            }
        }
    }

    public void CopyTo(ReadOnlySpan<byte> sourceSpan, int width, int height, IRawDPixelFormat targetPixelFormat, Span<byte> targetSpan) {
        var sourcePitch = CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = BitsPerPixel;
        var targetBpp = BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..]) {
            for (var x = 0; x < width; x++) {
                targetPixelFormat.SetDepth(targetSpan[(x * targetBpp)..], GetRed(sourceSpan[(x * sourceBpp)..]));
            }
        }
    }
}

public interface IRawRPixelFormat<T> : IRawRPixelFormat
    where T : unmanaged {
    public T GetRedTyped(ReadOnlySpan<byte> pixel);
    public void SetRed(Span<byte> pixel, T value);

    public void CopyTo(ReadOnlySpan<byte> sourceSpan, int width, int height, IRawRPixelFormat<T> targetPixelFormat, Span<byte> targetSpan) {
        var sourcePitch = CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = BitsPerPixel;
        var targetBpp = BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..]) {
            for (var x = 0; x < width; x++) {
                targetPixelFormat.SetRed(targetSpan[(x * targetBpp)..], GetRedTyped(sourceSpan[(x * sourceBpp)..]));
            }
        }
    }

    public void CopyTo(ReadOnlySpan<byte> sourceSpan, int width, int height, IRawDPixelFormat<T> targetPixelFormat, Span<byte> targetSpan) {
        var sourcePitch = CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = BitsPerPixel;
        var targetBpp = BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..]) {
            for (var x = 0; x < width; x++) {
                targetPixelFormat.SetDepth(targetSpan[(x * targetBpp)..], GetRedTyped(sourceSpan[(x * sourceBpp)..]));
            }
        }
    }
}
