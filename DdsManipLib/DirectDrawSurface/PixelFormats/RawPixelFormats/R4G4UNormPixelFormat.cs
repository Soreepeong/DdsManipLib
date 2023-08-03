using System;
using System.Numerics;
using DdsManipLib.Utilities;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R4G4UNormPixelFormat : RawRgPixelFormat, IRawRgPixelFormat<byte> {
    public override DxgiFormat DxgiFormat => DxgiFormat.R4G4UNorm;
    public override DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromRgba(8, 0xF, 0xF0, 0);
    public override int BitsPerPixel => 8;
    public override int BytesPerPixel => 1;
    public override float GetRed(ReadOnlySpan<byte> pixel) => GetRedTyped(pixel) / 255f;
    public override float GetGreen(ReadOnlySpan<byte> pixel) => GetGreenTyped(pixel) / 255f;
    public byte GetRedTyped(ReadOnlySpan<byte> pixel) => (byte) ((pixel[0] & 0xF) * 17);
    public byte GetGreenTyped(ReadOnlySpan<byte> pixel) => (byte) ((pixel[0] >> 4) * 17);
    public override void SetRed(Span<byte> pixel, float value) => SetRed(pixel, byte.CreateTruncating(value * 256));
    public override void SetGreen(Span<byte> pixel, float value) => SetGreen(pixel, byte.CreateTruncating(value * 256));
    public void SetRed(Span<byte> pixel, byte value) => pixel[0] = (byte) ((pixel[0] & ~0x0F) | (value >> 4));
    public void SetGreen(Span<byte> pixel, byte value) => pixel[0] = (byte) ((pixel[0] & ~0xF0) | (value & 0xF0));

    public void SetRg(Span<byte> pixel, Vector2<byte> rg) => pixel[0] = (byte) ((rg.X >> 4) | (rg.Y & 0xF0));
    public void SetRg(Span<byte> pixel, Vector2 rg) => SetRg(pixel, new Vector2<byte>(byte.CreateTruncating(rg.X * 256), byte.CreateTruncating(rg.Y * 256)));
    public R4G4UNormPixelFormat() : base(AlphaType.None) { }
}
