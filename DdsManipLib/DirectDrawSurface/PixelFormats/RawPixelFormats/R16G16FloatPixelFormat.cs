using System;
using System.Buffers.Binary;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R16G16FloatPixelFormat : R16G16PixelFormat, IRawRgPixelFormat<Half> {
    public override DxgiFormat DxgiFormat => DxgiFormat.R16G16Float;
    public override float GetRed(ReadOnlySpan<byte> pixel) => (float) GetRedTyped(pixel);
    public override float GetGreen(ReadOnlySpan<byte> pixel) => (float) GetGreenTyped(pixel);
    public Half GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetR..]);
    public Half GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetG..]);
    public override void SetRed(Span<byte> pixel, float value) => SetRed(pixel, Half.CreateSaturating(value));
    public override void SetGreen(Span<byte> pixel, float value) => SetGreen(pixel, Half.CreateSaturating(value));
    public void SetRed(Span<byte> pixel, Half value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetR..], value);
    public void SetGreen(Span<byte> pixel, Half value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetG..], value);
}
