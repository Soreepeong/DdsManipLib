using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class D24UNormS8UIntPixelFormat : RawPixelFormat, IRawDsPixelFormat<uint, byte> {
    public const int OffsetD = 0;
    public const int OffsetS = 3;

    public override DxgiFormat DxgiFormat => DxgiFormat.D24UNormS8UInt;
    public override int BitsPerPixel => 32;
    public override int BytesPerPixel => 4;
    public float GetDepth(ReadOnlySpan<byte> pixel) => GetDepthTyped(pixel);
    public float GetStencil(ReadOnlySpan<byte> pixel) => pixel[OffsetS];

    public uint GetDepthTyped(ReadOnlySpan<byte> pixel) =>
        (uint) (pixel[OffsetD + 2] | (pixel[OffsetD] << 8) | (pixel[OffsetD + 1] << 16) | (pixel[OffsetD + 2] << 24));

    public byte GetStencilTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetS];
    public void SetDepth(Span<byte> pixel, float value) => SetDepth(pixel, uint.CreateSaturating(value * uint.MaxValue));
    public void SetStencil(Span<byte> pixel, float value) => pixel[OffsetS] = byte.CreateSaturating(value);

    public void SetDepth(Span<byte> pixel, uint value) {
        pixel[OffsetD] = (byte) (value >> 8);
        pixel[OffsetD + 1] = (byte) (value >> 16);
        pixel[OffsetD + 2] = (byte) (value >> 24);
    }

    public void SetStencil(Span<byte> pixel, byte value) => pixel[OffsetS] = value;

    public D24UNormS8UIntPixelFormat() : base(AlphaType.None) { }

    public override void ClearPixel(Span<byte> pixel) => pixel[..BytesPerPixel].Clear();
}
