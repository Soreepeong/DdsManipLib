using System;
using System.Numerics;
using DdsManipLib.Utilities;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public sealed class R4G4UNormPixelFormat : RawRgPixelFormat, IRawRgPixelFormat<byte> {
    public override DxgiFormat DxgiFormat => DxgiFormat.R4G4UNorm;
    public override DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromRgba(8, 0xF, 0xF0, 0);
    public override int BitsPerPixel => 8;
    public override int BytesPerPixel => 1;
    public override float GetRed(ReadOnlySpan<byte> pixel) => (pixel[0] & 0x0F) / 15f;
    public override float GetGreen(ReadOnlySpan<byte> pixel) => (pixel[0] & 0xF0) / 240f;
    public byte GetRedTyped(ReadOnlySpan<byte> pixel) => (byte) (pixel[0] & 0xF);
    public byte GetGreenTyped(ReadOnlySpan<byte> pixel) => (byte) (pixel[0] >> 8);
    public override void SetRed(Span<byte> pixel, float value) => pixel[0] = (byte) ((pixel[0] & ~0x0F) | ((int) Math.Clamp(value * 16, 0, 15) << 0));
    public override void SetGreen(Span<byte> pixel, float value) => pixel[0] = (byte) ((pixel[0] & ~0xF0) | ((int) Math.Clamp(value * 16, 0, 15) << 8));
    public void SetRed(Span<byte> pixel, byte value) => pixel[0] = (byte) ((pixel[0] & ~0x0F) | (byte.Clamp(value, 0, 15) << 0));
    public void SetGreen(Span<byte> pixel, byte value) => pixel[0] = (byte) ((pixel[0] & ~0xF0) | (byte.Clamp(value, 0, 15) << 8));

    public void SetRg(Span<byte> pixel, Vector2<byte> rg) => pixel[0] = (byte) (byte.Clamp(rg.X, 0, 15) | (byte.Clamp(rg.Y, 0, 15) << 4));
    public void SetRg(Span<byte> pixel, Vector2 rg) => SetRg(pixel, new Vector2<byte>(byte.CreateTruncating(rg.X * 16), byte.CreateTruncating(rg.Y * 16)));
}
