using System;
using System.Buffers.Binary;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public abstract class R10G10B10A2PixelFormat : RawRgbaPixelFormat, IRawRgbaPixelFormat {
    public override int BitsPerPixel => 32;
    public override int BytesPerPixel => 4;
    public override AlphaType AlphaType => AlphaType.Straight;

    protected static ushort GetRedRaw(ReadOnlySpan<byte> pixel) => (ushort) ((BinaryPrimitives.ReadUInt32LittleEndian(pixel) >> 0) & 0x3FF);
    protected static ushort GetGreenRaw(ReadOnlySpan<byte> pixel) => (ushort) ((BinaryPrimitives.ReadUInt32LittleEndian(pixel) >> 10) & 0x3FF);
    protected static ushort GetBlueRaw(ReadOnlySpan<byte> pixel) => (ushort) ((BinaryPrimitives.ReadUInt32LittleEndian(pixel) >> 20) & 0x3FF);
    protected static byte GetAlphaRaw(ReadOnlySpan<byte> pixel) => (byte) ((BinaryPrimitives.ReadUInt32LittleEndian(pixel) >> 30) & 0x3FF);

    protected static void SetRedRaw(Span<byte> pixel, ushort value) =>
        BinaryPrimitives.WriteUInt32LittleEndian(pixel, (BinaryPrimitives.ReadUInt32LittleEndian(pixel) & ~0x3FFu) | ((uint) value << 0));

    protected static void SetGreenRaw(Span<byte> pixel, ushort value) =>
        BinaryPrimitives.WriteUInt32LittleEndian(pixel, (BinaryPrimitives.ReadUInt32LittleEndian(pixel) & ~0xFFC00u) | ((uint) value << 10));

    protected static void SetBlueRaw(Span<byte> pixel, ushort value) =>
        BinaryPrimitives.WriteUInt32LittleEndian(pixel, (BinaryPrimitives.ReadUInt32LittleEndian(pixel) & ~0x3FF00000u) | ((uint) value << 20));

    protected static void SetAlphaRaw(Span<byte> pixel, byte value) =>
        BinaryPrimitives.WriteUInt32LittleEndian(pixel, (BinaryPrimitives.ReadUInt32LittleEndian(pixel) & ~0xC0000000u) | ((uint) value << 30));
}
