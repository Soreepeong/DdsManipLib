using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R8UNormPixelFormat : RawRPixelFormat, IRawRPixelFormat<byte>, IRawRAlignedBytePixelFormat {
    public const int OffsetR = 0;

    public override DxgiFormat DxgiFormat => DxgiFormat.R8UNorm;
    public override DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromRgba(8, 0xFFu, 0u, 0u);
    public override int BitsPerPixel => 8;
    public override int BytesPerPixel => 1;
    public override float GetRed(ReadOnlySpan<byte> pixel) => pixel[OffsetR] / 255f;
    public byte GetRedTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetR];
    public override void SetRed(Span<byte> pixel, float value) => SetRed(pixel, byte.CreateTruncating(value * 256f));
    public void SetRed(Span<byte> pixel, byte value) => pixel[OffsetR] = value;
    public R8UNormPixelFormat() : base(AlphaType.None) { }

    int IRawRAlignedBytePixelFormat.OffsetR => OffsetR;
}
