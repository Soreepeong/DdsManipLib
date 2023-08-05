using System;
using System.Buffers.Binary;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R24G8TypelessPixelFormat : RawRgPixelFormat, IRawRgPixelFormat<uint>, IRawRgPixelFormat<int>, IRawRgPixelFormat<float> {
    public const int OffsetR = 0;
    public const int OffsetG = 3;

    public override DxgiFormat DxgiFormat => DxgiFormat.R24G8Typeless;
    public override int BitsPerPixel => 32;
    public override int BytesPerPixel => 4;
    public override float GetRed(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetR..]) & 0xFFFFFFu;
    public override float GetGreen(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
    float IRawRPixelFormat<float>.GetRedTyped(ReadOnlySpan<byte> pixel) => GetRedRaw(pixel);
    float IRawGPixelFormat<float>.GetGreenTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
    uint IRawRPixelFormat<uint>.GetRedTyped(ReadOnlySpan<byte> pixel) => GetRedRaw(pixel);
    uint IRawGPixelFormat<uint>.GetGreenTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
    int IRawRPixelFormat<int>.GetRedTyped(ReadOnlySpan<byte> pixel) => PixelFormatUtilities.RawToSInt((int) GetRedRaw(pixel), 24);
    int IRawGPixelFormat<int>.GetGreenTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
    public override void SetRed(Span<byte> pixel, float value) => SetRedRaw(pixel, PixelFormatUtilities.UIntToRaw(uint.CreateSaturating(value), 24));
    public override void SetGreen(Span<byte> pixel, float value) => pixel[OffsetG] = byte.CreateSaturating(value);
    public void SetRed(Span<byte> pixel, uint value) => SetRedRaw(pixel, PixelFormatUtilities.UIntToRaw(value, 24));
    public void SetGreen(Span<byte> pixel, uint value) => pixel[OffsetG] = byte.CreateSaturating(value);
    public void SetRed(Span<byte> pixel, int value) => SetRedRaw(pixel, (uint) PixelFormatUtilities.SIntToRaw(value, 24));
    public void SetGreen(Span<byte> pixel, int value) => pixel[OffsetG] = byte.CreateSaturating(value);

    private static uint GetRedRaw(ReadOnlySpan<byte> pixel) => (uint) (pixel[OffsetR] | (pixel[OffsetR + 1] << 8) | (pixel[OffsetR + 2] << 16));

    private static void SetRedRaw(Span<byte> pixel, uint value) {
        pixel[OffsetR] = (byte) value;
        pixel[OffsetR + 1] = (byte) (value >> 8);
        pixel[OffsetR + 2] = (byte) (value >> 16);
    }

    public R24G8TypelessPixelFormat() : base(AlphaType.None) { }
}
