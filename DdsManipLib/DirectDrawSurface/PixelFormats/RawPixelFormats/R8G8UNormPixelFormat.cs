using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R8G8UNormPixelFormat : R8G8PixelFormat, IRawRgPixelFormat<byte> {
    public override DxgiFormat DxgiFormat => DxgiFormat.R8G8UNorm;
    public override DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromRgba(16, 0xFF, 0xFF00, 0);
    public override float GetRed(ReadOnlySpan<byte> pixel) => pixel[OffsetR] / 255f;
    public override float GetGreen(ReadOnlySpan<byte> pixel) => pixel[OffsetG] / 255f;
    public byte GetRedTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetR];
    public byte GetGreenTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
    public override void SetRed(Span<byte> pixel, float value) => pixel[OffsetR] = byte.CreateSaturating(Math.Clamp(value, 0, 1) * 256);
    public override void SetGreen(Span<byte> pixel, float value) => pixel[OffsetG] = byte.CreateSaturating(Math.Clamp(value, 0, 1) * 256);
    public void SetRed(Span<byte> pixel, byte value) => pixel[OffsetR] = value;
    public void SetGreen(Span<byte> pixel, byte value) => pixel[OffsetG] = value;
}
