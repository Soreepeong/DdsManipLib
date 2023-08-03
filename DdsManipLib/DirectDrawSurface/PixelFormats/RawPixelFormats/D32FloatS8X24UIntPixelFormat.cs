using System;
using System.Buffers.Binary;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class D32FloatS8X24UIntPixelFormat : RawPixelFormat, IRawDsPixelFormat<float, byte> {
    public const int OffsetD = 0;
    public const int OffsetS = 4;

    public override DxgiFormat DxgiFormat => DxgiFormat.D32FloatS8X24UInt;
    public override int BitsPerPixel => 64;
    public override int BytesPerPixel => 8;
    public float GetDepth(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetD..]);
    public float GetStencil(ReadOnlySpan<byte> pixel) => pixel[OffsetS];
    public float GetDepthTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetD..]);
    public byte GetStencilTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetS];
    public void SetDepth(Span<byte> pixel, float value) => BinaryPrimitives.WriteSingleLittleEndian(pixel[OffsetD..], value);
    public void SetStencil(Span<byte> pixel, float value) => pixel[OffsetS] = byte.CreateTruncating(value);
    public void SetStencil(Span<byte> pixel, byte value) => pixel[OffsetS] = value;
    public D32FloatS8X24UIntPixelFormat() : base(AlphaType.None) { }
}
