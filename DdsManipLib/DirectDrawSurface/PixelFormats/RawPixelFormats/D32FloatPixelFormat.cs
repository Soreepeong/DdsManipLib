using System;
using System.Buffers.Binary;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class D32FloatPixelFormat : RawPixelFormat, IRawDPixelFormat<float> {
    public const int OffsetD = 0;

    public override DxgiFormat DxgiFormat => DxgiFormat.D32Float;
    public override int BitsPerPixel => 32;
    public override int BytesPerPixel => 4;
    public float GetDepth(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetD..]);
    public float GetDepthTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetD..]);
    public void SetDepth(Span<byte> pixel, float value) => BinaryPrimitives.WriteSingleLittleEndian(pixel[OffsetD..], value);
    public D32FloatPixelFormat() : base(AlphaType.None) { }

    public override void ClearPixel(Span<byte> pixel) => pixel[..BytesPerPixel].Clear();
}
