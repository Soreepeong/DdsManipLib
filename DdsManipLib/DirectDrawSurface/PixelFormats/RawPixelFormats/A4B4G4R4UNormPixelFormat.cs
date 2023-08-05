using System;
using System.Numerics;
using DdsManipLib.Utilities;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class A4B4G4R4UNormPixelFormat : RawRgbaPixelFormat, IRawRgbaPixelFormat<byte> {
    public override DxgiFormat DxgiFormat => DxgiFormat.A4B4G4R4UNorm;
    public override DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromRgba(16, 0xF000, 0xF00, 0xF0, 0xF);
    public override int BitsPerPixel => 16;
    public override int BytesPerPixel => 2;
    public override float GetRed(ReadOnlySpan<byte> pixel) => (pixel[1] >> 4) / 15f;
    public override float GetGreen(ReadOnlySpan<byte> pixel) => (pixel[1] & 0x0F) / 15f;
    public override float GetBlue(ReadOnlySpan<byte> pixel) => (pixel[0] >> 4) / 15f;
    public override float GetAlpha(ReadOnlySpan<byte> pixel) => (pixel[0] & 0x0F) / 15f;
    public byte GetRedTyped(ReadOnlySpan<byte> pixel) => (byte) ((pixel[1] >> 4) * 17);
    public byte GetGreenTyped(ReadOnlySpan<byte> pixel) => (byte) ((pixel[1] & 0x0F) * 17);
    public byte GetBlueTyped(ReadOnlySpan<byte> pixel) => (byte) ((pixel[0] >> 4) * 17);
    public byte GetAlphaTyped(ReadOnlySpan<byte> pixel) => (byte) ((pixel[0] & 0x0F) * 17);
    public override void SetRed(Span<byte> pixel, float value) => pixel[1] = (byte) ((pixel[1] & ~0xF0) | ((int) Math.Clamp(value, 0, 0xF) << 4));
    public override void SetGreen(Span<byte> pixel, float value) => pixel[0] = (byte) ((pixel[1] & ~0x0F) | ((int) Math.Clamp(value, 0, 0xF) << 0));
    public override void SetBlue(Span<byte> pixel, float value) => pixel[0] = (byte) ((pixel[0] & ~0xF0) | ((int) Math.Clamp(value, 0, 0xF) << 4));
    public override void SetAlpha(Span<byte> pixel, float value) => pixel[1] = (byte) ((pixel[0] & ~0x0F) | ((int) Math.Clamp(value, 0, 0xF) << 0));
    public void SetRed(Span<byte> pixel, byte value) => pixel[1] = (byte) ((pixel[1] & ~0xF0) | (value & 0xF0));
    public void SetGreen(Span<byte> pixel, byte value) => pixel[0] = (byte) ((pixel[1] & ~0x0F) | (value >> 4));
    public void SetBlue(Span<byte> pixel, byte value) => pixel[0] = (byte) ((pixel[0] & ~0xF0) | (value & 0xF0));
    public void SetAlpha(Span<byte> pixel, byte value) => pixel[1] = (byte) ((pixel[0] & ~0x0F) | (value >> 4));

    public void SetRgba(Span<byte> pixel, Vector4<byte> rgba) {
        pixel[0] = (byte) ((rgba.W >> 4) | (rgba.Z & 0xF0));
        pixel[1] = (byte) ((rgba.Y >> 4) | (rgba.X & 0xF0));
    }

    public void SetRgba(Span<byte> pixel, Vector4 rgba) => SetRgba(
        pixel,
        new Vector4<byte>(
            byte.CreateSaturating(rgba.X * 256),
            byte.CreateSaturating(rgba.Y * 256),
            byte.CreateSaturating(rgba.Z * 256),
            byte.CreateSaturating(rgba.W * 256)));

    public A4B4G4R4UNormPixelFormat(AlphaType alphaType) : base(alphaType) { }
}
