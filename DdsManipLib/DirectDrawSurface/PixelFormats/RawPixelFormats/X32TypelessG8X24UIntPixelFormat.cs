using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class X32TypelessG8X24UIntPixelFormat : RawPixelFormat, IRawGPixelFormat<float> {
    public const int OffsetG = 4;

    public override DxgiFormat DxgiFormat => DxgiFormat.X32TypelessG8X24UInt;
    public override int BitsPerPixel => 64;
    public override int BytesPerPixel => 8;
    public float GetGreen(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
    float IRawGPixelFormat<float>.GetGreenTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
    public void SetGreen(Span<byte> pixel, float value) => pixel[OffsetG] = byte.CreateTruncating(value);
    public X32TypelessG8X24UIntPixelFormat() : base(AlphaType.None) { }
}
