using System;
using System.Buffers.Binary;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R32FloatX8X24TypelessPixelFormat : RawRPixelFormat, IRawRPixelFormat<float> {
    public const int OffsetR = 0;

    public override DxgiFormat DxgiFormat => DxgiFormat.R32FloatX8X24Typeless;
    public override int BitsPerPixel => 64;
    public override int BytesPerPixel => 8;
    public override float GetRed(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetR..]);
    public float GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetR..]);
    public override void SetRed(Span<byte> pixel, float value) => BinaryPrimitives.WriteSingleLittleEndian(pixel[OffsetR..], value);
    public R32FloatX8X24TypelessPixelFormat() : base(AlphaType.None) { }
}
