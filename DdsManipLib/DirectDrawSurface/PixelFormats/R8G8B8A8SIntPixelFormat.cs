using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public sealed class R8G8B8A8SIntPixelFormat : R8G8B8A8PixelFormat, IRawRgbaPixelFormat<sbyte> {
    public override DxgiFormat DxgiFormat => DxgiFormat.R8G8B8A8SInt;
    public override float GetRed(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetR];
    public override float GetGreen(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetG];
    public override float GetBlue(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetB];
    public override float GetAlpha(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetA];
    public sbyte GetRedTyped(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetR];
    public sbyte GetGreenTyped(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetG];
    public sbyte GetBlueTyped(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetB];
    public sbyte GetAlphaTyped(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetA];
    public override void SetRed(Span<byte> pixel, float value) => pixel[OffsetR] = (byte) sbyte.CreateTruncating(value);
    public override void SetGreen(Span<byte> pixel, float value) => pixel[OffsetG] = (byte) sbyte.CreateTruncating(value);
    public override void SetBlue(Span<byte> pixel, float value) => pixel[OffsetB] = (byte) sbyte.CreateTruncating(value);
    public override void SetAlpha(Span<byte> pixel, float value) => pixel[OffsetA] = (byte) sbyte.CreateTruncating(value);
    public void SetRed(Span<byte> pixel, sbyte value) => pixel[OffsetR] = (byte) value;
    public void SetGreen(Span<byte> pixel, sbyte value) => pixel[OffsetG] = (byte) value;
    public void SetBlue(Span<byte> pixel, sbyte value) => pixel[OffsetB] = (byte) value;
    public void SetAlpha(Span<byte> pixel, sbyte value) => pixel[OffsetA] = (byte) value;
}
