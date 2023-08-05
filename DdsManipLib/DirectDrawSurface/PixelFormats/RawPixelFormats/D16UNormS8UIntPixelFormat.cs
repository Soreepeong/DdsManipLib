using System;
using System.Buffers.Binary;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class D16UNormS8UIntPixelFormat : RawPixelFormat, IRawDsPixelFormat<ushort, byte> {
    public const int OffsetD = 0;
    public const int OffsetS = 2;

    public override DxgiFormat DxgiFormat => DxgiFormat.D16UNormS8UInt;
    public override int BitsPerPixel => 24;
    public override int BytesPerPixel => 3;
    public float GetDepth(ReadOnlySpan<byte> pixel) => GetDepthTyped(pixel);
    public float GetStencil(ReadOnlySpan<byte> pixel) => pixel[OffsetS];
    public ushort GetDepthTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetD..]);
    public byte GetStencilTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetS];
    public void SetDepth(Span<byte> pixel, float value) => SetDepth(pixel, ushort.CreateSaturating(value * ushort.MaxValue));
    public void SetStencil(Span<byte> pixel, float value) => pixel[OffsetS] = byte.CreateSaturating(value);
    public void SetDepth(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetD..], value);
    public void SetStencil(Span<byte> pixel, byte value) => pixel[OffsetS] = value;

    public D16UNormS8UIntPixelFormat() : base(AlphaType.None) { }

    public override void ClearPixel(Span<byte> pixel) => pixel[..BytesPerPixel].Clear();
}
