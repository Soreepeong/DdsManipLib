using System;
using System.Buffers.Binary;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public sealed class D24UNormS8UIntPixelFormat : RawPixelFormat, IRawDsPixelFormat<uint, byte> {
    public const int OffsetD = 0;
    public const int OffsetS = 3;

    public override DxgiFormat DxgiFormat => DxgiFormat.D24UNormS8UInt;
    public override int BitsPerPixel => 32;
    public override int BytesPerPixel => 4;
    public float GetDepth(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetD..]) & 0xFFFFFFu;
    public float GetStencil(ReadOnlySpan<byte> pixel) => pixel[OffsetS];
    uint IRawDPixelFormat<uint>.GetDepthTyped(ReadOnlySpan<byte> pixel) => GetDepthRaw(pixel);
    byte IRawDsPixelFormat<uint, byte>.GetStencilTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetS];
    public void SetDepth(Span<byte> pixel, float value) => SetDepthRaw(pixel, PixelFormatUtilities.UIntToRaw(uint.CreateSaturating(value), 24));
    public void SetStencil(Span<byte> pixel, float value) => pixel[OffsetS] = byte.CreateTruncating(value);
    public void SetDepth(Span<byte> pixel, uint value) => SetDepthRaw(pixel, PixelFormatUtilities.UIntToRaw(value, 24));
    public void SetStencil(Span<byte> pixel, byte value) => pixel[OffsetS] = value;

    private static uint GetDepthRaw(ReadOnlySpan<byte> pixel) => (uint) (pixel[OffsetD] | (pixel[OffsetD + 1] << 8) | (pixel[OffsetD + 2] << 16));

    private static void SetDepthRaw(Span<byte> pixel, uint value) {
        pixel[OffsetD] = (byte) value;
        pixel[OffsetD + 1] = (byte) (value >> 8);
        pixel[OffsetD + 2] = (byte) (value >> 16);
    }
}
