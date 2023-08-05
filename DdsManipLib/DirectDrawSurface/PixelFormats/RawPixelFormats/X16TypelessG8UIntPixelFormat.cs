using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class X16TypelessG8UIntPixelFormat : RawPixelFormat, IRawGPixelFormat<byte>, IRawGPixelFormat<sbyte> {
    public const int OffsetG = 2;

    public override DxgiFormat DxgiFormat => DxgiFormat.X16TypelessG8UInt;
    public override int BitsPerPixel => 24;
    public override int BytesPerPixel => 3;
    public float GetGreen(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
    byte IRawGPixelFormat<byte>.GetGreenTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
    sbyte IRawGPixelFormat<sbyte>.GetGreenTyped(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetG];
    public void SetGreen(Span<byte> pixel, float value) => pixel[OffsetG] = byte.CreateSaturating(value);
    public void SetGreen(Span<byte> pixel, byte value) => pixel[OffsetG] = byte.CreateSaturating(value);
    public void SetGreen(Span<byte> pixel, sbyte value) => pixel[OffsetG] = byte.CreateSaturating(value);
    public X16TypelessG8UIntPixelFormat() : base(AlphaType.None) { }

    public override void ClearPixel(Span<byte> pixel) => pixel[..BytesPerPixel].Clear();
}
