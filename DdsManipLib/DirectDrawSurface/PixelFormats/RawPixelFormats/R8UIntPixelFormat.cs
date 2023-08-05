using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R8UIntPixelFormat : RawRPixelFormat, IRawRPixelFormat<byte>, IRawRAlignedBytePixelFormat {
    public const int OffsetR = 0;

    public override DxgiFormat DxgiFormat => DxgiFormat.R8UInt;
    public override int BitsPerPixel => 8;
    public override int BytesPerPixel => 1;
    public override float GetRed(ReadOnlySpan<byte> pixel) => pixel[OffsetR];
    public byte GetRedTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetR];
    public override void SetRed(Span<byte> pixel, float value) => SetRed(pixel, byte.CreateSaturating(value));
    public void SetRed(Span<byte> pixel, byte value) => pixel[OffsetR] = value;
    public R8UIntPixelFormat() : base(AlphaType.None) { }

    int IRawRAlignedBytePixelFormat.OffsetR => OffsetR;
}
