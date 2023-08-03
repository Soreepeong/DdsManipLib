using System;
using System.Buffers.Binary;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R16G16B16A16TypelessPixelFormat : R16G16B16A16PixelFormat, IRawRgbaPixelFormat<ushort>, IRawRgbaPixelFormat<short>,
    IRawRgbaPixelFormat<Half> {
    public override DxgiFormat DxgiFormat => DxgiFormat.R16G16B16A16Typeless;
    public override float GetRed(ReadOnlySpan<byte> pixel) => (float) BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetR..]);
    public override float GetGreen(ReadOnlySpan<byte> pixel) => (float) BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetG..]);
    public override float GetBlue(ReadOnlySpan<byte> pixel) => (float) BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetB..]);
    public override float GetAlpha(ReadOnlySpan<byte> pixel) => (float) BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetA..]);
    Half IRawRPixelFormat<Half>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetR..]);
    Half IRawGPixelFormat<Half>.GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetG..]);
    Half IRawRgbPixelFormat<Half>.GetBlueTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetB..]);
    Half IRawAPixelFormat<Half>.GetAlphaTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetA..]);
    ushort IRawRPixelFormat<ushort>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetR..]);
    ushort IRawGPixelFormat<ushort>.GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetG..]);
    ushort IRawRgbPixelFormat<ushort>.GetBlueTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetB..]);
    ushort IRawAPixelFormat<ushort>.GetAlphaTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetA..]);
    short IRawRPixelFormat<short>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetR..]);
    short IRawGPixelFormat<short>.GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetG..]);
    short IRawRgbPixelFormat<short>.GetBlueTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetB..]);
    short IRawAPixelFormat<short>.GetAlphaTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetA..]);
    public override void SetRed(Span<byte> pixel, float value) => SetRed(pixel, ushort.CreateTruncating(65536 * value));
    public override void SetGreen(Span<byte> pixel, float value) => SetGreen(pixel, ushort.CreateTruncating(65536 * value));
    public override void SetBlue(Span<byte> pixel, float value) => SetBlue(pixel, ushort.CreateTruncating(65536 * value));
    public override void SetAlpha(Span<byte> pixel, float value) => SetAlpha(pixel, ushort.CreateTruncating(65536 * value));
    public void SetRed(Span<byte> pixel, Half value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetR..], value);
    public void SetGreen(Span<byte> pixel, Half value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetG..], value);
    public void SetBlue(Span<byte> pixel, Half value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetB..], value);
    public void SetAlpha(Span<byte> pixel, Half value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetA..], value);
    public void SetRed(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetR..], value);
    public void SetGreen(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetG..], value);
    public void SetBlue(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetB..], value);
    public void SetAlpha(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetA..], value);
    public void SetRed(Span<byte> pixel, short value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetR..], value);
    public void SetGreen(Span<byte> pixel, short value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetG..], value);
    public void SetBlue(Span<byte> pixel, short value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetB..], value);
    public void SetAlpha(Span<byte> pixel, short value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetA..], value);
    public R16G16B16A16TypelessPixelFormat(AlphaType alphaType) : base(alphaType) { }
}
