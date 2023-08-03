using System;
using System.Numerics;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public interface IRawDsPixelFormat : IRawDPixelFormat {
    public float GetStencil(ReadOnlySpan<byte> pixel);
    public void SetStencil(Span<byte> pixel, float value);

    public Vector2 GetDs(ReadOnlySpan<byte> pixel) => new(GetDepth(pixel), GetStencil(pixel));

    public void SetDs(Span<byte> pixel, Vector2 ds) {
        SetDepth(pixel, ds.X);
        SetStencil(pixel, ds.Y);
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
                targetPixelFormat.SetDepth(targetSpan[(x * targetBpp)..], GetDepth(sourceSpan[(x * sourceBpp)..]));
            }
        }
    }

    public void CopyTo(ReadOnlySpan<byte> sourceSpan, int width, int height, IRawGPixelFormat targetPixelFormat, Span<byte> targetSpan) {
        var sourcePitch = CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = BitsPerPixel;
        var targetBpp = BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..]) {
            for (var x = 0; x < width; x++) {
                targetPixelFormat.SetGreen(targetSpan[(x * targetBpp)..], GetStencil(sourceSpan[(x * sourceBpp)..]));
            }
        }
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
                targetPixelFormat.SetRg(targetSpan[(x * targetBpp)..], GetDs(sourceSpan[(x * sourceBpp)..]));
            }
        }
    }
}

public interface IRawDsPixelFormat<TDepth, TStencil> : IRawDPixelFormat<TDepth>, IRawDsPixelFormat
    where TDepth : unmanaged
    where TStencil : unmanaged {
    public TStencil GetStencilTyped(ReadOnlySpan<byte> pixel);
    public void SetStencil(Span<byte> pixel, TStencil value);
}
