using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public sealed class R8G8TypelessPixelFormat : R8G8PixelFormat, IRawRgPixelFormat<byte>, IRawRgPixelFormat<sbyte> {
    public override DxgiFormat DxgiFormat => DxgiFormat.R8G8Typeless;
    public override float GetRed(ReadOnlySpan<byte> pixel) => pixel[OffsetR];
    public override float GetGreen(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
    byte IRawRPixelFormat<byte>.GetRedTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetR];
    byte IRawGPixelFormat<byte>.GetGreenTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
    sbyte IRawRPixelFormat<sbyte>.GetRedTyped(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetR];
    sbyte IRawGPixelFormat<sbyte>.GetGreenTyped(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetG];
    public override void SetRed(Span<byte> pixel, float value) => pixel[OffsetR] = byte.CreateTruncating(value);
    public override void SetGreen(Span<byte> pixel, float value) => pixel[OffsetG] = byte.CreateTruncating(value);
    public void SetRed(Span<byte> pixel, sbyte value) => pixel[OffsetR] = (byte) value;
    public void SetGreen(Span<byte> pixel, sbyte value) => pixel[OffsetG] = (byte) value;
    public void SetRed(Span<byte> pixel, byte value) => pixel[OffsetR] = value;
    public void SetGreen(Span<byte> pixel, byte value) => pixel[OffsetG] = value;
}
