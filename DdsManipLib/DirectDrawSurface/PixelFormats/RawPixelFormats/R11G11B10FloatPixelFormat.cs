using System;
using System.Buffers.Binary;
using System.Numerics;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R11G11B10FloatPixelFormat : RawRgbPixelFormat, IRawRgbPixelFormat {
    public override DxgiFormat DxgiFormat => DxgiFormat.R11G11B10Float;
    public override int BitsPerPixel => 32;
    public override int BytesPerPixel => 4;

    public override float GetRed(ReadOnlySpan<byte> pixel) {
        var v = BinaryPrimitives.ReadUInt32LittleEndian(pixel);
        var e = ((v >>> 6) & 0x1F) - 15 + 127;
        var m = v & 0x3F;
        return BitConverter.UInt32BitsToSingle((e << 23) | (m << 17) | (m << 11) | (m << 5) | (m >> 1));
    }

    public override float GetGreen(ReadOnlySpan<byte> pixel) {
        var v = BinaryPrimitives.ReadUInt32LittleEndian(pixel);
        var e = ((v >>> 17) & 0x1F) - 15 + 127;
        var m = (v >>> 11) & 0x3F;
        return BitConverter.UInt32BitsToSingle((e << 23) | (m << 17) | (m << 11) | (m << 5) | (m >> 1));
    }

    public override float GetBlue(ReadOnlySpan<byte> pixel) {
        var v = BinaryPrimitives.ReadUInt32LittleEndian(pixel);
        var e = ((v >>> 27) & 0x1F) - 15 + 127;
        var m = (v >>> 22) & 0x1F;
        return BitConverter.UInt32BitsToSingle((e << 23) | (m << 17) | (m << 12) | (m << 7) | (m << 2) | (m >> 2));
    }

    public override void SetRed(Span<byte> pixel, float value) {
        var (e, m) = TranslateExponentMantissa(BitConverter.SingleToUInt32Bits(value), 6);
        BinaryPrimitives.WriteUInt32LittleEndian(pixel, BinaryPrimitives.ReadUInt32LittleEndian(pixel) & ~0x7FFu | (e << 6) | m);
    }

    public override void SetGreen(Span<byte> pixel, float value) {
        var (e, m) = TranslateExponentMantissa(BitConverter.SingleToUInt32Bits(value), 6);
        BinaryPrimitives.WriteUInt32LittleEndian(pixel, BinaryPrimitives.ReadUInt32LittleEndian(pixel) & ~0x3FF800u | (e << 17) | (m << 11));
    }

    public override void SetBlue(Span<byte> pixel, float value) {
        var (e, m) = TranslateExponentMantissa(BitConverter.SingleToUInt32Bits(value), 5);
        BinaryPrimitives.WriteUInt32LittleEndian(pixel, BinaryPrimitives.ReadUInt32LittleEndian(pixel) & ~0x3FF800u | (e << 27) | (m << 22));
    }

    public Vector3 GetRgb(ReadOnlySpan<byte> pixel) {
        var v = BinaryPrimitives.ReadUInt32LittleEndian(pixel);
        var re = ((v >>> 6) & 0x1F) - 15 + 127;
        var rm = v & 0x3F;
        var ge = ((v >>> 17) & 0x1F) - 15 + 127;
        var gm = (v >>> 11) & 0x3F;
        var be = ((v >>> 27) & 0x1F) - 15 + 127;
        var bm = (v >>> 22) & 0x1F;
        return new(
            BitConverter.UInt32BitsToSingle((re << 23) | (rm << 17) | (rm << 11) | (rm << 5) | (rm >> 1)),
            BitConverter.UInt32BitsToSingle((ge << 23) | (gm << 17) | (gm << 11) | (gm << 5) | (gm >> 1)),
            BitConverter.UInt32BitsToSingle((be << 23) | (bm << 17) | (bm << 12) | (bm << 7) | (bm << 2) | (bm >> 2)));
    }

    public void SetRgb(Span<byte> pixel, Vector3 rgb) {
        var (re, rm) = TranslateExponentMantissa(BitConverter.SingleToUInt32Bits(rgb.X), 6);
        var (ge, gm) = TranslateExponentMantissa(BitConverter.SingleToUInt32Bits(rgb.Y), 6);
        var (be, bm) = TranslateExponentMantissa(BitConverter.SingleToUInt32Bits(rgb.Z), 5);
        BinaryPrimitives.WriteUInt32LittleEndian(pixel, (be << 27) | (bm << 22) | (ge << 17) | (gm << 11) | (re << 6) | rm);
    }

    private static (uint e, uint m) TranslateExponentMantissa(float v, int mbits) {
        var u = BitConverter.SingleToUInt32Bits(v);
        var ei = (int) ((u >> 23) & 0xFF) - 127;
        if (ei < -16)
            return (0, 0);
        if (ei > 15)
            return (0x1F, (1u << mbits) - 1u);
        var e = ((u >> 23) & 0xFF) - 127 + 15;
        var m = (u >> (23 - mbits)) & ((1u << mbits) - 1u);
        return (e, m);
    }

    public R11G11B10FloatPixelFormat() : base(AlphaType.None) { }
}
