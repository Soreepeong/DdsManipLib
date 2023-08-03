using System;
using System.Buffers.Binary;
using System.Numerics;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R10G10B10A2UNormPixelFormat : R10G10B10A2PixelFormat, IRawRgbaPixelFormat, IRawRgbPixelFormat<ushort>, IRawAPixelFormat<byte> {
    public override DxgiFormat DxgiFormat => DxgiFormat.R10G10B10A2UNorm;
    public override float GetRed(ReadOnlySpan<byte> pixel) => GetRedRaw(pixel) / 1023f;
    public override float GetGreen(ReadOnlySpan<byte> pixel) => GetGreenRaw(pixel) / 1023f;
    public override float GetBlue(ReadOnlySpan<byte> pixel) => GetBlueRaw(pixel) / 1023f;
    public override float GetAlpha(ReadOnlySpan<byte> pixel) => GetAlphaRaw(pixel) / 3f;

    public ushort GetRedTyped(ReadOnlySpan<byte> pixel) {
        var n = GetRedRaw(pixel);
        return (ushort) ((n << 6) | (n >> 4));
    }

    public ushort GetGreenTyped(ReadOnlySpan<byte> pixel) {
        var n = GetGreenRaw(pixel);
        return (ushort) ((n << 6) | (n >> 4));
    }

    public ushort GetBlueTyped(ReadOnlySpan<byte> pixel) {
        var n = GetBlueRaw(pixel);
        return (ushort) ((n << 6) | (n >> 4));
    }

    public byte GetAlphaTyped(ReadOnlySpan<byte> pixel) {
        var n = GetAlphaRaw(pixel);
        return (byte) (n * 0b01010101);
    }

    public override void SetRed(Span<byte> pixel, float value) => SetRedRaw(pixel, (ushort) Math.Clamp(value * 0x400, 0, 0x3FF));
    public override void SetGreen(Span<byte> pixel, float value) => SetGreenRaw(pixel, (ushort) Math.Clamp(value * 0x400, 0, 0x3FF));
    public override void SetBlue(Span<byte> pixel, float value) => SetBlueRaw(pixel, (ushort) Math.Clamp(value * 0x400, 0, 0x3FF));
    public override void SetAlpha(Span<byte> pixel, float value) => SetAlphaRaw(pixel, (byte) Math.Clamp(value * 0x4, 0, 3));
    public void SetRed(Span<byte> pixel, ushort value) => SetRedRaw(pixel, (ushort) (value >> 6));
    public void SetGreen(Span<byte> pixel, ushort value) => SetGreenRaw(pixel, (ushort) (value >> 6));
    public void SetBlue(Span<byte> pixel, ushort value) => SetBlueRaw(pixel, (ushort) (value >> 6));
    public void SetAlpha(Span<byte> pixel, byte value) => SetAlphaRaw(pixel, (byte) (value >> 6));

    public Vector4 GetRgba(ReadOnlySpan<byte> pixel) {
        var v = BinaryPrimitives.ReadUInt32LittleEndian(pixel);
        return new(
            ((v >> 0) & 0x3FF) / 1023f,
            ((v >> 10) & 0x3FF) / 1023f,
            ((v >> 20) & 0x3FF) / 1023f,
            ((v >> 30) & 0x3FF) / 3f);
    }

    public void SetRgba(Span<byte> pixel, Vector4 rgba) => BinaryPrimitives.WriteUInt32LittleEndian(
        pixel,
        ((uint) Math.Clamp(rgba.X * 1024f, 0, 0x3FF) << 0) |
        ((uint) Math.Clamp(rgba.Y * 1024f, 0, 0x3FF) << 10) |
        ((uint) Math.Clamp(rgba.Z * 1024f, 0, 0x3FF) << 20) |
        ((uint) Math.Clamp(rgba.W * 4f, 0, 3) << 30));

    public R10G10B10A2UNormPixelFormat(AlphaType alphaType) : base(alphaType) { }
}
