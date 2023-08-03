using System;
using System.Buffers.Binary;
using System.Numerics;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public sealed class R10G10B10A2UIntPixelFormat : R10G10B10A2PixelFormat, IRawRgbPixelFormat<ushort>, IRawAlphaPixelFormat<byte> {
    public override DxgiFormat DxgiFormat => DxgiFormat.R10G10B10A2UInt;
    public override float GetRed(ReadOnlySpan<byte> pixel) => GetRedRaw(pixel);
    public override float GetGreen(ReadOnlySpan<byte> pixel) => GetGreenRaw(pixel);
    public override float GetBlue(ReadOnlySpan<byte> pixel) => GetBlueRaw(pixel);
    public override float GetAlpha(ReadOnlySpan<byte> pixel) => GetAlphaRaw(pixel);
    public ushort GetRedTyped(ReadOnlySpan<byte> pixel) => GetRedRaw(pixel);
    public ushort GetGreenTyped(ReadOnlySpan<byte> pixel) => GetGreenRaw(pixel);
    public ushort GetBlueTyped(ReadOnlySpan<byte> pixel) => GetBlueRaw(pixel);
    public byte GetAlphaTyped(ReadOnlySpan<byte> pixel) => GetAlphaRaw(pixel);
    public override void SetRed(Span<byte> pixel, float value) => SetRedRaw(pixel, (ushort) Math.Clamp(value, 0, 0x3FF));
    public override void SetGreen(Span<byte> pixel, float value) => SetGreenRaw(pixel, (ushort) Math.Clamp(value, 0, 0x3FF));
    public override void SetBlue(Span<byte> pixel, float value) => SetBlueRaw(pixel, (ushort) Math.Clamp(value, 0, 0x3FF));
    public override void SetAlpha(Span<byte> pixel, float value) => SetAlphaRaw(pixel, (byte) Math.Clamp(value, 0, 3));
    public void SetRed(Span<byte> pixel, ushort value) => SetRedRaw(pixel, ushort.Clamp(value, 0, 0x3FF));
    public void SetGreen(Span<byte> pixel, ushort value) => SetGreenRaw(pixel, ushort.Clamp(value, 0, 0x3FF));
    public void SetBlue(Span<byte> pixel, ushort value) => SetBlueRaw(pixel, ushort.Clamp(value, 0, 0x3FF));
    public void SetAlpha(Span<byte> pixel, byte value) => SetAlphaRaw(pixel, byte.Clamp(value, 0, 3));

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
}
