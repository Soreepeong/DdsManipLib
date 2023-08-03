using System;
using System.Buffers.Binary;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R32G32B32A32TypelessPixelFormat : R32G32B32A32PixelFormat, IRawRgbaPixelFormat<uint>, IRawRgbaPixelFormat<int>,
    IRawRgbaPixelFormat<float> {
    public override DxgiFormat DxgiFormat => DxgiFormat.R32G32B32A32Typeless;
    public override float GetRed(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetR..]);
    public override float GetGreen(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetG..]);
    public override float GetBlue(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetB..]);
    public override float GetAlpha(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetA..]);
    float IRawRPixelFormat<float>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetR..]);
    float IRawGPixelFormat<float>.GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetG..]);
    float IRawRgbPixelFormat<float>.GetBlueTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetB..]);
    float IRawAPixelFormat<float>.GetAlphaTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetA..]);
    uint IRawRPixelFormat<uint>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetR..]);
    uint IRawGPixelFormat<uint>.GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetG..]);
    uint IRawRgbPixelFormat<uint>.GetBlueTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetB..]);
    uint IRawAPixelFormat<uint>.GetAlphaTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetA..]);
    int IRawRPixelFormat<int>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetR..]);
    int IRawGPixelFormat<int>.GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetG..]);
    int IRawRgbPixelFormat<int>.GetBlueTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetB..]);
    int IRawAPixelFormat<int>.GetAlphaTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetA..]);
    public override void SetRed(Span<byte> pixel, float value) => BinaryPrimitives.WriteSingleLittleEndian(pixel[OffsetR..], value);
    public override void SetGreen(Span<byte> pixel, float value) => BinaryPrimitives.WriteSingleLittleEndian(pixel[OffsetG..], value);
    public override void SetBlue(Span<byte> pixel, float value) => BinaryPrimitives.WriteSingleLittleEndian(pixel[OffsetB..], value);
    public override void SetAlpha(Span<byte> pixel, float value) => BinaryPrimitives.WriteSingleLittleEndian(pixel[OffsetA..], value);
    public void SetRed(Span<byte> pixel, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetR..], value);
    public void SetGreen(Span<byte> pixel, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetG..], value);
    public void SetBlue(Span<byte> pixel, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetB..], value);
    public void SetAlpha(Span<byte> pixel, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetA..], value);
    public void SetRed(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetR..], value);
    public void SetGreen(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetG..], value);
    public void SetBlue(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetB..], value);
    public void SetAlpha(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetA..], value);
    public R32G32B32A32TypelessPixelFormat(AlphaType alphaType) : base(alphaType) { }
}
