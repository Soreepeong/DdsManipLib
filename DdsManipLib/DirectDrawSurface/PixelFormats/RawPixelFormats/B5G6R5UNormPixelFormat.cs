using System;
using System.Buffers.Binary;
using System.Numerics;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class B5G6R5UNormPixelFormat : RawRgbPixelFormat, IRawRgbPixelFormat<byte> {
    public const ushort BlueMask = 0b00000_000000_11111;
    public const ushort GreenMask = 0b00000_111111_00000;
    public const ushort RedMask = 0b11111_000000_00000;
    public override DxgiFormat DxgiFormat => DxgiFormat.B5G6R5UNorm;

    public override DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromRgba(16, RedMask, GreenMask, BlueMask);

    public override int BitsPerPixel => 16;
    public override int BytesPerPixel => 2;

    public override float GetRed(ReadOnlySpan<byte> pixel) => GetRedRaw(pixel) / 31f;
    public override float GetGreen(ReadOnlySpan<byte> pixel) => GetGreenRaw(pixel) / 63f;
    public override float GetBlue(ReadOnlySpan<byte> pixel) => GetBlueRaw(pixel) / 31f;

    public byte GetRedTyped(ReadOnlySpan<byte> pixel) {
        var n = GetRedRaw(pixel);
        return (byte) ((n << 3) | (n >> 2));
    }

    public byte GetGreenTyped(ReadOnlySpan<byte> pixel) {
        var n = GetGreenRaw(pixel);
        return (byte) ((n << 2) | (n >> 4));
    }

    public byte GetBlueTyped(ReadOnlySpan<byte> pixel) {
        var n = GetBlueRaw(pixel);
        return (byte) ((n << 3) | (n >> 2));
    }

    public override void SetRed(Span<byte> pixel, float value) => SetRedRaw(pixel, (byte) Math.Clamp(value * 32, 0, 31));
    public override void SetGreen(Span<byte> pixel, float value) => SetGreenRaw(pixel, (byte) Math.Clamp(value * 64, 0, 63));
    public override void SetBlue(Span<byte> pixel, float value) => SetBlueRaw(pixel, (byte) Math.Clamp(value * 32, 0, 31));
    public void SetRed(Span<byte> pixel, byte value) => SetRedRaw(pixel, (byte) (value >> 3));
    public void SetGreen(Span<byte> pixel, byte value) => SetGreenRaw(pixel, (byte) (value >> 2));
    public void SetBlue(Span<byte> pixel, byte value) => SetBlueRaw(pixel, (byte) (value >> 3));

    public Vector3 GetRgb(ReadOnlySpan<byte> pixel) {
        var v = BinaryPrimitives.ReadUInt16LittleEndian(pixel);
        return new(
            ((v >> 11) & 0x1F) / 31f,
            ((v >> 5) & 0x3F) / 63f,
            ((v >> 0) & 0x1F) / 31f);
    }

    public void SetRgb(Span<byte> pixel, Vector3 rgb) => BinaryPrimitives.WriteUInt16LittleEndian(
        pixel,
        (ushort) (((ushort) Math.Clamp(rgb.X * 32f, 0, 31) << 11) |
            ((ushort) Math.Clamp(rgb.Y * 64f, 0, 63) << 5) |
            ((ushort) Math.Clamp(rgb.Z * 32f, 0, 31) << 0)));

    private static byte GetRedRaw(ReadOnlySpan<byte> pixel) => (byte) ((BinaryPrimitives.ReadUInt16LittleEndian(pixel) >> 11) & 0x1F);
    private static byte GetGreenRaw(ReadOnlySpan<byte> pixel) => (byte) ((BinaryPrimitives.ReadUInt16LittleEndian(pixel) >> 5) & 0x3F);
    private static byte GetBlueRaw(ReadOnlySpan<byte> pixel) => (byte) ((BinaryPrimitives.ReadUInt16LittleEndian(pixel) >> 0) & 0x1F);

    private static void SetRedRaw(Span<byte> pixel, byte value) =>
        BinaryPrimitives.WriteUInt16LittleEndian(pixel, (ushort) ((BinaryPrimitives.ReadUInt16LittleEndian(pixel) & ~RedMask) | (value << 11)));

    private static void SetGreenRaw(Span<byte> pixel, byte value) =>
        BinaryPrimitives.WriteUInt16LittleEndian(pixel, (ushort) ((BinaryPrimitives.ReadUInt16LittleEndian(pixel) & ~GreenMask) | (value << 5)));

    private static void SetBlueRaw(Span<byte> pixel, byte value) =>
        BinaryPrimitives.WriteUInt16LittleEndian(pixel, (ushort) ((BinaryPrimitives.ReadUInt16LittleEndian(pixel) & ~BlueMask) | (value << 0)));

    public B5G6R5UNormPixelFormat() : base(AlphaType.None) { }
}
