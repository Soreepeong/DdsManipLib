using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R8SIntPixelFormat : RawRPixelFormat, IRawRPixelFormat<sbyte>, IRawRAlignedBytePixelFormat {
    public const int OffsetR = 0;

    public override DxgiFormat DxgiFormat => DxgiFormat.R8SInt;
    public override int BitsPerPixel => 8;
    public override int BytesPerPixel => 1;
    public override float GetRed(ReadOnlySpan<byte> pixel) => GetRedTyped(pixel);
    public sbyte GetRedTyped(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetR];
    public override void SetRed(Span<byte> pixel, float value) => SetRed(pixel, sbyte.CreateTruncating(value));
    public void SetRed(Span<byte> pixel, sbyte value) => pixel[OffsetR] = (byte) value;
    public R8SIntPixelFormat() : base(AlphaType.None) { }
    
    int IRawRAlignedBytePixelFormat.OffsetR => OffsetR;
}
