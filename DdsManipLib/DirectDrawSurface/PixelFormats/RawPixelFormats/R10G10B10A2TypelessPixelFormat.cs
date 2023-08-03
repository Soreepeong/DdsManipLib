using System;
using System.Buffers.Binary;
using System.Numerics;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R10G10B10A2TypelessPixelFormat : R10G10B10A2PixelFormat, IRawRgbaPixelFormat, IRawRgbPixelFormat<short>, IRawRgbPixelFormat<ushort>,
    IRawAPixelFormat<byte>, IRawAPixelFormat<sbyte> {
    public override DxgiFormat DxgiFormat => DxgiFormat.R10G10B10A2Typeless;
    public override float GetRed(ReadOnlySpan<byte> pixel) => GetRedRaw(pixel);
    public override float GetGreen(ReadOnlySpan<byte> pixel) => GetGreenRaw(pixel);
    public override float GetBlue(ReadOnlySpan<byte> pixel) => GetBlueRaw(pixel);
    public override float GetAlpha(ReadOnlySpan<byte> pixel) => GetAlphaRaw(pixel);
    ushort IRawRPixelFormat<ushort>.GetRedTyped(ReadOnlySpan<byte> pixel) => GetRedRaw(pixel);
    ushort IRawGPixelFormat<ushort>.GetGreenTyped(ReadOnlySpan<byte> pixel) => GetGreenRaw(pixel);
    ushort IRawRgbPixelFormat<ushort>.GetBlueTyped(ReadOnlySpan<byte> pixel) => GetBlueRaw(pixel);
    byte IRawAPixelFormat<byte>.GetAlphaTyped(ReadOnlySpan<byte> pixel) => GetAlphaRaw(pixel);
    short IRawRPixelFormat<short>.GetRedTyped(ReadOnlySpan<byte> pixel) => (short) GetRedRaw(pixel);
    short IRawGPixelFormat<short>.GetGreenTyped(ReadOnlySpan<byte> pixel) => (short) GetGreenRaw(pixel);
    short IRawRgbPixelFormat<short>.GetBlueTyped(ReadOnlySpan<byte> pixel) => (short) GetBlueRaw(pixel);
    sbyte IRawAPixelFormat<sbyte>.GetAlphaTyped(ReadOnlySpan<byte> pixel) => (sbyte) GetAlphaRaw(pixel);
    public override void SetRed(Span<byte> pixel, float value) => SetRedRaw(pixel, (ushort) Math.Clamp(value, 0, 0x3FF));
    public override void SetGreen(Span<byte> pixel, float value) => SetGreenRaw(pixel, (ushort) Math.Clamp(value, 0, 0x3FF));
    public override void SetBlue(Span<byte> pixel, float value) => SetBlueRaw(pixel, (ushort) Math.Clamp(value, 0, 0x3FF));
    public override void SetAlpha(Span<byte> pixel, float value) => SetAlphaRaw(pixel, (byte) Math.Clamp(value, 0, 3));
    public void SetRed(Span<byte> pixel, ushort value) => SetRedRaw(pixel, ushort.Clamp(value, 0, 0x3FF));
    public void SetGreen(Span<byte> pixel, ushort value) => SetGreenRaw(pixel, ushort.Clamp(value, 0, 0x3FF));
    public void SetBlue(Span<byte> pixel, ushort value) => SetBlueRaw(pixel, ushort.Clamp(value, 0, 0x3FF));
    public void SetAlpha(Span<byte> pixel, byte value) => SetAlphaRaw(pixel, byte.Clamp(value, 0, 3));
    public void SetRed(Span<byte> pixel, short value) => SetRedRaw(pixel, (ushort) short.Clamp(value, 0, 0x3FF));
    public void SetGreen(Span<byte> pixel, short value) => SetGreenRaw(pixel, (ushort) short.Clamp(value, 0, 0x3FF));
    public void SetBlue(Span<byte> pixel, short value) => SetBlueRaw(pixel, (ushort) short.Clamp(value, 0, 0x3FF));
    public void SetAlpha(Span<byte> pixel, sbyte value) => SetAlphaRaw(pixel, (byte) sbyte.Clamp(value, 0, 3));

    public Vector4 GetRgba(ReadOnlySpan<byte> pixel) {
        var v = BinaryPrimitives.ReadUInt32LittleEndian(pixel);
        return new(
            ((v >> 0) & 0x3FF),
            ((v >> 10) & 0x3FF),
            ((v >> 20) & 0x3FF),
            ((v >> 30) & 0x3FF));
    }

    public void SetRgba(Span<byte> pixel, Vector4 rgba) => BinaryPrimitives.WriteUInt32LittleEndian(
        pixel,
        ((uint) Math.Clamp(rgba.X, 0, 0x3FF) << 0) |
        ((uint) Math.Clamp(rgba.Y, 0, 0x3FF) << 10) |
        ((uint) Math.Clamp(rgba.Z, 0, 0x3FF) << 20) |
        ((uint) Math.Clamp(rgba.W, 0, 3) << 30));

    public R10G10B10A2TypelessPixelFormat(AlphaType alphaType) : base(alphaType) { }
}
