using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public sealed class B8G8R8X8UNormPixelFormat : B8G8R8X8PixelFormat, IRawRgbPixelFormat<byte> {
    public override DxgiFormat DxgiFormat => DxgiFormat.B8G8R8X8UNorm;
    public override DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromRgba(32, 0xFF0000, 0xFF00, 0xFF);
    public override float GetRed(ReadOnlySpan<byte> pixel) => pixel[OffsetR] / 255f;
    public override float GetGreen(ReadOnlySpan<byte> pixel) => pixel[OffsetG] / 255f;
    public override float GetBlue(ReadOnlySpan<byte> pixel) => pixel[OffsetB] / 255f;
    public byte GetRedTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetR];
    public byte GetGreenTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
    public byte GetBlueTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetB];
    public override void SetRed(Span<byte> pixel, float value) => pixel[OffsetR] = byte.CreateTruncating(value * 256f);
    public override void SetGreen(Span<byte> pixel, float value) => pixel[OffsetG] = byte.CreateTruncating(value * 256f);
    public override void SetBlue(Span<byte> pixel, float value) => pixel[OffsetB] = byte.CreateTruncating(value * 256f);
    public void SetRed(Span<byte> pixel, byte value) => pixel[OffsetR] = value;
    public void SetGreen(Span<byte> pixel, byte value) => pixel[OffsetG] = value;
    public void SetBlue(Span<byte> pixel, byte value) => pixel[OffsetB] = value;
}
