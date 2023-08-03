using System;
using System.Buffers.Binary;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R16G16B16A16FloatPixelFormat : R16G16B16A16PixelFormat, IRawRgbPixelFormat<Half>, IRawAPixelFormat<Half> {
    public override DxgiFormat DxgiFormat => DxgiFormat.R16G16B16A16Float;
    public override float GetRed(ReadOnlySpan<byte> pixel) => (float) BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetR..]);
    public override float GetGreen(ReadOnlySpan<byte> pixel) => (float) BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetG..]);
    public override float GetBlue(ReadOnlySpan<byte> pixel) => (float) BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetB..]);
    public override float GetAlpha(ReadOnlySpan<byte> pixel) => (float) BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetA..]);
    public Half GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetR..]);
    public Half GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetG..]);
    public Half GetBlueTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetB..]);
    public Half GetAlphaTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetA..]);
    public override void SetRed(Span<byte> pixel, float value) => SetRed(pixel, Half.CreateTruncating(value));
    public override void SetGreen(Span<byte> pixel, float value) => SetGreen(pixel, Half.CreateTruncating(value));
    public override void SetBlue(Span<byte> pixel, float value) => SetBlue(pixel, Half.CreateTruncating(value));
    public override void SetAlpha(Span<byte> pixel, float value) => SetAlpha(pixel, Half.CreateTruncating(value));
    public void SetRed(Span<byte> pixel, Half value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetR..], value);
    public void SetGreen(Span<byte> pixel, Half value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetG..], value);
    public void SetBlue(Span<byte> pixel, Half value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetB..], value);
    public void SetAlpha(Span<byte> pixel, Half value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetA..], value);
    public R16G16B16A16FloatPixelFormat(AlphaType alphaType) : base(alphaType) { }
}
