using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public sealed class X24TypelessG8UIntPixelFormat : RawPixelFormat, IRawGPixelFormat<byte>, IRawGPixelFormat<sbyte> {
    public const int OffsetG = 3;

    public override DxgiFormat DxgiFormat => DxgiFormat.X24TypelessG8UInt;
    public override int BitsPerPixel => 32;
    public override int BytesPerPixel => 4;
    public float GetGreen(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
    byte IRawGPixelFormat<byte>.GetGreenTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
    sbyte IRawGPixelFormat<sbyte>.GetGreenTyped(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetG];
    public void SetGreen(Span<byte> pixel, float value) => pixel[OffsetG] = byte.CreateTruncating(value);
    public void SetGreen(Span<byte> pixel, byte value) => pixel[OffsetG] = byte.CreateTruncating(value);
    public void SetGreen(Span<byte> pixel, sbyte value) => pixel[OffsetG] = byte.CreateTruncating(value);
}
