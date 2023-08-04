using System;
using System.Buffers.Binary;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R32FloatPixelFormat : RawRPixelFormat, IRawRAlignedFloatPixelFormat {
    public const int OffsetR = 0;

    public override DxgiFormat DxgiFormat => DxgiFormat.R32Float;
    public override int BitsPerPixel => 32;
    public override int BytesPerPixel => 4;
    public override float GetRed(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetR..]);
    public float GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetR..]);
    public override void SetRed(Span<byte> pixel, float value) => BinaryPrimitives.WriteSingleLittleEndian(pixel[OffsetR..], value);
    public R32FloatPixelFormat() : base(AlphaType.None) { }
    
    int IRawRAlignedFloatPixelFormat.OffsetR => OffsetR;
}
