using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public sealed class R8TypelessPixelFormat : RawRPixelFormat, IRawRPixelFormat<byte>, IRawRPixelFormat<sbyte> {
    public const int OffsetR = 0;

    public override DxgiFormat DxgiFormat => DxgiFormat.R8Typeless;
    public override int BitsPerPixel => 8;
    public override int BytesPerPixel => 1;
    public override float GetRed(ReadOnlySpan<byte> pixel) => pixel[OffsetR];
    byte IRawRPixelFormat<byte>.GetRedTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetR];
    sbyte IRawRPixelFormat<sbyte>.GetRedTyped(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetR];
    public override void SetRed(Span<byte> pixel, float value) => pixel[OffsetR] = byte.CreateTruncating(value);
    public void SetRed(Span<byte> pixel, byte value) => pixel[OffsetR] = value;
    public void SetRed(Span<byte> pixel, sbyte value) => pixel[OffsetR] = (byte) value;
}
