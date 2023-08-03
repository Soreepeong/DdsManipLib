using System;
using System.Numerics;
using DdsManipLib.Utilities;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public sealed class B4G4R4A4UNormPixelFormat : RawRgbaPixelFormat, IRawRgbaPixelFormat<byte> {
    public override DxgiFormat DxgiFormat => DxgiFormat.B4G4R4A4UNorm;
    public override DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromRgba(16, 0xF00, 0xF0, 0xF, 0xF000);
    public override int BitsPerPixel => 16;
    public override int BytesPerPixel => 2;
    public override AlphaType AlphaType => AlphaType.Straight;
    public override float GetRed(ReadOnlySpan<byte> pixel) => (pixel[1] & 0x0F) / 15f;
    public override float GetGreen(ReadOnlySpan<byte> pixel) => (pixel[0] & 0xF0) / 240f;
    public override float GetBlue(ReadOnlySpan<byte> pixel) => (pixel[0] & 0x0F) / 15f;
    public override float GetAlpha(ReadOnlySpan<byte> pixel) => (pixel[1] & 0xF0) / 240f;
    public byte GetRedTyped(ReadOnlySpan<byte> pixel) => (byte) (pixel[1] & 0x0F);
    public byte GetGreenTyped(ReadOnlySpan<byte> pixel) => (byte) ((pixel[0] & 0xF0) >> 4);
    public byte GetBlueTyped(ReadOnlySpan<byte> pixel) => (byte) (pixel[0] & 0x0F);
    public byte GetAlphaTyped(ReadOnlySpan<byte> pixel) => (byte) ((pixel[1] & 0xF0) >> 4);
    public override void SetRed(Span<byte> pixel, float value) => pixel[1] = (byte) ((pixel[1] & ~0x0F) | (int) Math.Clamp(value, 0, 0xF));
    public override void SetGreen(Span<byte> pixel, float value) => pixel[0] = (byte) ((pixel[0] & ~0xF0) | ((int) Math.Clamp(value, 0, 0xF) << 4));
    public override void SetBlue(Span<byte> pixel, float value) => pixel[0] = (byte) ((pixel[0] & ~0x0F) | (int) Math.Clamp(value, 0, 0xF));
    public override void SetAlpha(Span<byte> pixel, float value) => pixel[1] = (byte) ((pixel[1] & ~0xF0) | ((int) Math.Clamp(value, 0, 0xF) << 4));
    public void SetRed(Span<byte> pixel, byte value) => pixel[1] = (byte) ((pixel[1] & ~0x0F) | byte.Clamp(value, 0, 0xF));
    public void SetGreen(Span<byte> pixel, byte value) => pixel[0] = (byte) ((pixel[0] & ~0xF0) | (byte.Clamp(value, 0, 0xF) << 4));
    public void SetBlue(Span<byte> pixel, byte value) => pixel[0] = (byte) ((pixel[0] & ~0x0F) | byte.Clamp(value, 0, 0xF));
    public void SetAlpha(Span<byte> pixel, byte value) => pixel[1] = (byte) ((pixel[1] & ~0xF0) | (byte.Clamp(value, 0, 0xF) << 4));

    public void SetRgba(Span<byte> pixel, Vector4<byte> rgba) {
        pixel[0] = (byte) (byte.Clamp(rgba.Z, 0, 15) | (byte.Clamp(rgba.Y, 0, 15) << 4));
        pixel[1] = (byte) (byte.Clamp(rgba.X, 0, 15) | (byte.Clamp(rgba.W, 0, 15) << 4));
    }

    public void SetRgba(Span<byte> pixel, Vector4 rgba) => SetRgba(
        pixel,
        new Vector4<byte>(
            byte.CreateTruncating(rgba.X * 16),
            byte.CreateTruncating(rgba.Y * 16),
            byte.CreateTruncating(rgba.Z * 16),
            byte.CreateTruncating(rgba.W * 16)));
}
