using System;
using System.Buffers.Binary;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public sealed class R24UNormX8TypelessPixelFormat : RawRPixelFormat, IRawRPixelFormat<uint>, IRawRPixelFormat<int>, IRawRPixelFormat<float> {
    public const int OffsetR = 0;

    public override DxgiFormat DxgiFormat => DxgiFormat.R24UNormX8Typeless;
    public override DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromRgba(32, 0xFFFFFFu, 0u, 0u);
    public override int BitsPerPixel => 32;
    public override int BytesPerPixel => 4;
    public override float GetRed(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetR..]) & 0xFFFFFFu;
    float IRawRPixelFormat<float>.GetRedTyped(ReadOnlySpan<byte> pixel) => GetRedRaw(pixel);
    uint IRawRPixelFormat<uint>.GetRedTyped(ReadOnlySpan<byte> pixel) => GetRedRaw(pixel);
    int IRawRPixelFormat<int>.GetRedTyped(ReadOnlySpan<byte> pixel) => PixelFormatUtilities.RawToSInt(GetRedRaw(pixel), 24);
    public override void SetRed(Span<byte> pixel, float value) => SetRedRaw(pixel, PixelFormatUtilities.UIntToRaw(uint.CreateSaturating(value), 24));
    public void SetRed(Span<byte> pixel, uint value) => SetRedRaw(pixel, PixelFormatUtilities.UIntToRaw(value, 24));
    public void SetRed(Span<byte> pixel, int value) => SetRedRaw(pixel, PixelFormatUtilities.SIntToRaw(value, 24));

    private static uint GetRedRaw(ReadOnlySpan<byte> pixel) => (uint) (pixel[OffsetR] | (pixel[OffsetR + 1] << 8) | (pixel[OffsetR + 2] << 16));

    private static void SetRedRaw(Span<byte> pixel, uint value) {
        pixel[OffsetR] = (byte) value;
        pixel[OffsetR + 1] = (byte) (value >> 8);
        pixel[OffsetR + 2] = (byte) (value >> 16);
    }
}
