using System;
using System.Buffers.Binary;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R32G32B32A32SIntPixelFormat : R32G32B32A32PixelFormat, IRawRgbPixelFormat<int>, IRawAPixelFormat<int> {
    public override DxgiFormat DxgiFormat => DxgiFormat.R32G32B32A32SInt;
    public override float GetRed(ReadOnlySpan<byte> pixel) => GetRedTyped(pixel);
    public override float GetGreen(ReadOnlySpan<byte> pixel) => GetGreenTyped(pixel);
    public override float GetBlue(ReadOnlySpan<byte> pixel) => GetBlueTyped(pixel);
    public override float GetAlpha(ReadOnlySpan<byte> pixel) => GetAlphaTyped(pixel);
    public int GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetR..]);
    public int GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetG..]);
    public int GetBlueTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetB..]);
    public int GetAlphaTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetA..]);
    public override void SetRed(Span<byte> pixel, float value) => SetRed(pixel, int.CreateTruncating(value));
    public override void SetGreen(Span<byte> pixel, float value) => SetGreen(pixel, int.CreateTruncating(value));
    public override void SetBlue(Span<byte> pixel, float value) => SetBlue(pixel, int.CreateTruncating(value));
    public override void SetAlpha(Span<byte> pixel, float value) => SetAlpha(pixel, int.CreateTruncating(value));
    public void SetRed(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetR..], value);
    public void SetGreen(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetG..], value);
    public void SetBlue(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetB..], value);
    public void SetAlpha(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetA..], value);
    public R32G32B32A32SIntPixelFormat(AlphaType alphaType) : base(alphaType) { }
}
