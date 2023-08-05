﻿using System;
using System.Numerics;
using DdsManipLib.Utilities;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class B4G4R4A4UNormPixelFormat : RawRgbaPixelFormat, IRawRgbaPixelFormat<byte> {
    public override DdsFourCc FourCc => AlphaType != AlphaType.Straight ? DdsFourCc.Unknown : DdsFourCc.D3dFmtA4R4G4B4;
    public override DxgiFormat DxgiFormat => DxgiFormat.B4G4R4A4UNorm;

    public override DdsPixelFormat DdsPixelFormat => AlphaType != AlphaType.Straight
        ? default
        : DdsPixelFormat.FromRgba(16, 0xF00, 0xF0, 0xF, 0xF000);

    public override int BitsPerPixel => 16;
    public override int BytesPerPixel => 2;
    public override float GetRed(ReadOnlySpan<byte> pixel) => (pixel[1] & 0x0F) / 15f;
    public override float GetGreen(ReadOnlySpan<byte> pixel) => (pixel[0] >> 4) / 15f;
    public override float GetBlue(ReadOnlySpan<byte> pixel) => (pixel[0] & 0x0F) / 15f;
    public override float GetAlpha(ReadOnlySpan<byte> pixel) => (pixel[1] >> 4) / 15f;
    public byte GetRedTyped(ReadOnlySpan<byte> pixel) => (byte) ((pixel[1] & 0x0F) * 17);
    public byte GetGreenTyped(ReadOnlySpan<byte> pixel) => (byte) ((pixel[0] >> 4) * 17);
    public byte GetBlueTyped(ReadOnlySpan<byte> pixel) => (byte) ((pixel[0] & 0x0F) * 17);
    public byte GetAlphaTyped(ReadOnlySpan<byte> pixel) => (byte) ((pixel[1] >> 4) * 17);
    public override void SetRed(Span<byte> pixel, float value) => pixel[1] = (byte) ((pixel[1] & ~0x0F) | (int) Math.Clamp(value, 0, 0xF));
    public override void SetGreen(Span<byte> pixel, float value) => pixel[0] = (byte) ((pixel[0] & ~0xF0) | ((int) Math.Clamp(value, 0, 0xF) << 4));
    public override void SetBlue(Span<byte> pixel, float value) => pixel[0] = (byte) ((pixel[0] & ~0x0F) | (int) Math.Clamp(value, 0, 0xF));
    public override void SetAlpha(Span<byte> pixel, float value) => pixel[1] = (byte) ((pixel[1] & ~0xF0) | ((int) Math.Clamp(value, 0, 0xF) << 4));
    public void SetRed(Span<byte> pixel, byte value) => pixel[1] = (byte) ((pixel[1] & ~0x0F) | (value >> 4));
    public void SetGreen(Span<byte> pixel, byte value) => pixel[0] = (byte) ((pixel[0] & ~0xF0) | (value & 0xF0));
    public void SetBlue(Span<byte> pixel, byte value) => pixel[0] = (byte) ((pixel[0] & ~0x0F) | (value >> 4));
    public void SetAlpha(Span<byte> pixel, byte value) => pixel[1] = (byte) ((pixel[1] & ~0xF0) | (value & 0xF0));

    public void SetRgba(Span<byte> pixel, Vector4<byte> rgba) {
        pixel[0] = (byte) ((rgba.Z >> 4) | (rgba.Y & 0xF0));
        pixel[1] = (byte) ((rgba.X >> 4) | (rgba.W & 0xF0));
    }

    public void SetRgba(Span<byte> pixel, Vector4 rgba) => SetRgba(
        pixel,
        new Vector4<byte>(
            byte.CreateSaturating(rgba.X * 256),
            byte.CreateSaturating(rgba.Y * 256),
            byte.CreateSaturating(rgba.Z * 256),
            byte.CreateSaturating(rgba.W * 256)));

    public B4G4R4A4UNormPixelFormat(AlphaType alphaType) : base(alphaType) { }
}
