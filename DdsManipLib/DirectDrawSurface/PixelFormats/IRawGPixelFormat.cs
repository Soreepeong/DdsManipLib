using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public interface IRawGPixelFormat : IRawPixelFormat {
    public float GetGreen(ReadOnlySpan<byte> pixel);
    public void SetGreen(Span<byte> pixel, float value);

    public void CopyTo(ReadOnlySpan<byte> sourceSpan, int width, int height, IRawGPixelFormat targetPixelFormat, Span<byte> targetSpan) {
        var sourcePitch = CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = BitsPerPixel;
        var targetBpp = BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..]) {
            for (var x = 0; x < width; x++) {
                targetPixelFormat.SetGreen(targetSpan[(x * targetBpp)..], GetGreen(sourceSpan[(x * sourceBpp)..]));
            }
        }
    }

    public void CopyTo(ReadOnlySpan<byte> sourceSpan, int width, int height, IRawDsPixelFormat targetPixelFormat, Span<byte> targetSpan) {
        var sourcePitch = CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = BitsPerPixel;
        var targetBpp = BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..]) {
            for (var x = 0; x < width; x++) {
                targetPixelFormat.SetStencil(targetSpan[(x * targetBpp)..], GetGreen(sourceSpan[(x * sourceBpp)..]));
            }
        }
    }
}

public interface IRawGPixelFormat<T> : IRawGPixelFormat
    where T : unmanaged {
    public T GetGreenTyped(ReadOnlySpan<byte> pixel);
    public void SetGreen(Span<byte> pixel, T value);

    public void CopyTo(ReadOnlySpan<byte> sourceSpan, int width, int height, IRawGPixelFormat<T> targetPixelFormat, Span<byte> targetSpan) {
        var sourcePitch = CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = BitsPerPixel;
        var targetBpp = BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..]) {
            for (var x = 0; x < width; x++) {
                targetPixelFormat.SetGreen(targetSpan[(x * targetBpp)..], GetGreenTyped(sourceSpan[(x * sourceBpp)..]));
            }
        }
    }

    public void CopyTo<TDepth>(ReadOnlySpan<byte> sourceSpan, int width, int height, IRawDsPixelFormat<TDepth, T> targetPixelFormat, Span<byte> targetSpan)
        where TDepth : unmanaged {
        var sourcePitch = CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = BitsPerPixel;
        var targetBpp = BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..]) {
            for (var x = 0; x < width; x++) {
                targetPixelFormat.SetStencil(targetSpan[(x * targetBpp)..], GetGreenTyped(sourceSpan[(x * sourceBpp)..]));
            }
        }
    }
}
