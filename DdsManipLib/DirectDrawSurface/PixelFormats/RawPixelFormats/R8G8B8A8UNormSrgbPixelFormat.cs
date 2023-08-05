using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R8G8B8A8UNormSrgbPixelFormat : R8G8B8A8PixelFormat, IRawRgbaPixelFormat<byte> {
    public override DxgiFormat DxgiFormat => DxgiFormat.R8G8B8A8UNormSrgb;
    public override float GetRed(ReadOnlySpan<byte> pixel) => pixel[OffsetR] / 255f;
    public override float GetGreen(ReadOnlySpan<byte> pixel) => pixel[OffsetG] / 255f;
    public override float GetBlue(ReadOnlySpan<byte> pixel) => pixel[OffsetB] / 255f;
    public override float GetAlpha(ReadOnlySpan<byte> pixel) => pixel[OffsetA] / 255f;
    public byte GetRedTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetR];
    public byte GetGreenTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
    public byte GetBlueTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetB];
    public byte GetAlphaTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetA];
    public override void SetRed(Span<byte> pixel, float value) => pixel[OffsetR] = byte.CreateSaturating(value * 256f);
    public override void SetGreen(Span<byte> pixel, float value) => pixel[OffsetG] = byte.CreateSaturating(value * 256f);
    public override void SetBlue(Span<byte> pixel, float value) => pixel[OffsetB] = byte.CreateSaturating(value * 256f);
    public override void SetAlpha(Span<byte> pixel, float value) => pixel[OffsetA] = byte.CreateSaturating(value * 256f);
    public void SetRed(Span<byte> pixel, byte value) => pixel[OffsetR] = value;
    public void SetGreen(Span<byte> pixel, byte value) => pixel[OffsetG] = value;
    public void SetBlue(Span<byte> pixel, byte value) => pixel[OffsetB] = value;
    public void SetAlpha(Span<byte> pixel, byte value) => pixel[OffsetA] = value;
    public R8G8B8A8UNormSrgbPixelFormat(AlphaType alphaType) : base(alphaType) { }
}
