using System;
using System.Numerics;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public interface IRawDsPixelFormat : IRawDPixelFormat {
    public float GetStencil(ReadOnlySpan<byte> pixel);
    public void SetStencil(Span<byte> pixel, float value);

    public Vector2 GetDs(ReadOnlySpan<byte> pixel) => new(GetDepth(pixel), GetStencil(pixel));

    public void SetDs(Span<byte> pixel, Vector2 ds) {
        SetDepth(pixel, ds.X);
        SetStencil(pixel, ds.Y);
    }
}

public interface IRawDsPixelFormat<TDepth, TStencil> : IRawDPixelFormat<TDepth>, IRawDsPixelFormat
    where TDepth : unmanaged, IBinaryNumber<TDepth>
    where TStencil : unmanaged, IBinaryNumber<TStencil> {
    public TStencil GetStencilTyped(ReadOnlySpan<byte> pixel);
    public void SetStencil(Span<byte> pixel, TStencil value);
}
