using System;
using System.Buffers.Binary;
using System.Numerics;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class B5G5R5A1UNormPixelFormat : RawRgbaPixelFormat, IRawRgbaPixelFormat, IRawRgbPixelFormat<byte>, IRawAPixelFormat<byte> {
    public const ushort BlueMask = 0b0_00000_00000_11111;
    public const ushort GreenMask = 0b0_00000_11111_00000;
    public const ushort RedMask = 0b0_11111_00000_00000;
    public const ushort AlphaMask = 0b1_00000_00000_00000;
    public override DxgiFormat DxgiFormat => DxgiFormat.B5G5R5A1UNorm;

    public override DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromRgba(16, RedMask, GreenMask, BlueMask, AlphaMask);

    public override int BitsPerPixel => 16;
    public override int BytesPerPixel => 2;

    public override float GetRed(ReadOnlySpan<byte> pixel) => GetRedRaw(pixel) / 31f;
    public override float GetGreen(ReadOnlySpan<byte> pixel) => GetGreenRaw(pixel) / 31f;
    public override float GetBlue(ReadOnlySpan<byte> pixel) => GetBlueRaw(pixel) / 31f;
    public override float GetAlpha(ReadOnlySpan<byte> pixel) => GetAlphaRaw(pixel);

    public byte GetRedTyped(ReadOnlySpan<byte> pixel) {
        var n = GetRedRaw(pixel);
        return (byte) ((n << 3) | (n >> 2));
    }

    public byte GetGreenTyped(ReadOnlySpan<byte> pixel) {
        var n = GetGreenRaw(pixel);
        return (byte) ((n << 3) | (n >> 2));
    }

    public byte GetBlueTyped(ReadOnlySpan<byte> pixel) {
        var n = GetBlueRaw(pixel);
        return (byte) ((n << 3) | (n >> 2));
    }

    public byte GetAlphaTyped(ReadOnlySpan<byte> pixel) => GetAlphaRaw(pixel) == 0 ? byte.MinValue : byte.MaxValue;
    public override void SetRed(Span<byte> pixel, float value) => SetRedRaw(pixel, (byte) Math.Clamp(value * 32, 0, 31));
    public override void SetGreen(Span<byte> pixel, float value) => SetGreenRaw(pixel, (byte) Math.Clamp(value * 32, 0, 31));
    public override void SetBlue(Span<byte> pixel, float value) => SetBlueRaw(pixel, (byte) Math.Clamp(value * 32, 0, 31));
    public override void SetAlpha(Span<byte> pixel, float value) => SetAlphaRaw(pixel, value <= 0 ? byte.MinValue : byte.MaxValue);
    public void SetRed(Span<byte> pixel, byte value) => SetRedRaw(pixel, (byte) (value >> 3));
    public void SetGreen(Span<byte> pixel, byte value) => SetGreenRaw(pixel, (byte) (value >> 3));
    public void SetBlue(Span<byte> pixel, byte value) => SetBlueRaw(pixel, (byte) (value >> 3));
    public void SetAlpha(Span<byte> pixel, byte value) => SetAlphaRaw(pixel, value == 0 ? byte.MinValue : byte.MaxValue);

    public Vector4 GetRgba(ReadOnlySpan<byte> pixel) {
        var v = BinaryPrimitives.ReadUInt16LittleEndian(pixel);
        return new(
            ((v >> 10) & 0x1F) / 31f,
            ((v >> 5) & 0x1F) / 31f,
            ((v >> 0) & 0x1F) / 31f,
            v >> 15);
    }

    public void SetRgba(Span<byte> pixel, Vector4 rgba) => BinaryPrimitives.WriteUInt16LittleEndian(
        pixel,
        (ushort) (((ushort) Math.Clamp(rgba.X * 32f, 0, 31) << 10) |
            ((ushort) Math.Clamp(rgba.Y * 32f, 0, 31) << 5) |
            ((ushort) Math.Clamp(rgba.Z * 32f, 0, 31) << 0) |
            (ushort) (rgba.W <= 0 ? 0 : AlphaMask)));

    private static byte GetRedRaw(ReadOnlySpan<byte> pixel) => (byte) ((BinaryPrimitives.ReadUInt16LittleEndian(pixel) >> 10) & 0x1F);
    private static byte GetGreenRaw(ReadOnlySpan<byte> pixel) => (byte) ((BinaryPrimitives.ReadUInt16LittleEndian(pixel) >> 5) & 0x1F);
    private static byte GetBlueRaw(ReadOnlySpan<byte> pixel) => (byte) ((BinaryPrimitives.ReadUInt16LittleEndian(pixel) >> 0) & 0x1F);
    private static byte GetAlphaRaw(ReadOnlySpan<byte> pixel) => (byte) (BinaryPrimitives.ReadUInt16LittleEndian(pixel) >> 15);

    private static void SetRedRaw(Span<byte> pixel, byte value) =>
        BinaryPrimitives.WriteUInt16LittleEndian(pixel, (ushort) ((BinaryPrimitives.ReadUInt16LittleEndian(pixel) & ~RedMask) | (value << 10)));

    private static void SetGreenRaw(Span<byte> pixel, byte value) =>
        BinaryPrimitives.WriteUInt16LittleEndian(pixel, (ushort) ((BinaryPrimitives.ReadUInt16LittleEndian(pixel) & ~GreenMask) | (value << 5)));

    private static void SetBlueRaw(Span<byte> pixel, byte value) =>
        BinaryPrimitives.WriteUInt16LittleEndian(pixel, (ushort) ((BinaryPrimitives.ReadUInt16LittleEndian(pixel) & ~BlueMask) | (value << 0)));

    private static void SetAlphaRaw(Span<byte> pixel, byte value) =>
        BinaryPrimitives.WriteUInt16LittleEndian(pixel, (ushort) ((BinaryPrimitives.ReadUInt16LittleEndian(pixel) & ~AlphaMask) | (value << 15)));

    public B5G5R5A1UNormPixelFormat(AlphaType alphaType) : base(alphaType) { }
}
