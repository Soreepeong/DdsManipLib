using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class B8G8R8X8TypelessPixelFormat : B8G8R8X8PixelFormat, IRawRgbPixelFormat<byte>, IRawRgbPixelFormat<sbyte> {
    public override DxgiFormat DxgiFormat => DxgiFormat.B8G8R8X8Typeless;
    public override float GetRed(ReadOnlySpan<byte> pixel) => pixel[OffsetR];
    public override float GetGreen(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
    public override float GetBlue(ReadOnlySpan<byte> pixel) => pixel[OffsetB];
    byte IRawRPixelFormat<byte>.GetRedTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetR];
    byte IRawGPixelFormat<byte>.GetGreenTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
    byte IRawRgbPixelFormat<byte>.GetBlueTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetB];
    sbyte IRawRPixelFormat<sbyte>.GetRedTyped(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetR];
    sbyte IRawGPixelFormat<sbyte>.GetGreenTyped(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetG];
    sbyte IRawRgbPixelFormat<sbyte>.GetBlueTyped(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetB];
    public override void SetRed(Span<byte> pixel, float value) => pixel[OffsetR] = byte.CreateTruncating(value);
    public override void SetGreen(Span<byte> pixel, float value) => pixel[OffsetG] = byte.CreateTruncating(value);
    public override void SetBlue(Span<byte> pixel, float value) => pixel[OffsetB] = byte.CreateTruncating(value);
    public void SetRed(Span<byte> pixel, byte value) => pixel[OffsetR] = value;
    public void SetGreen(Span<byte> pixel, byte value) => pixel[OffsetG] = value;
    public void SetBlue(Span<byte> pixel, byte value) => pixel[OffsetB] = value;
    public void SetRed(Span<byte> pixel, sbyte value) => pixel[OffsetR] = byte.CreateTruncating(value);
    public void SetGreen(Span<byte> pixel, sbyte value) => pixel[OffsetG] = byte.CreateTruncating(value);
    public void SetBlue(Span<byte> pixel, sbyte value) => pixel[OffsetB] = byte.CreateTruncating(value);
}
