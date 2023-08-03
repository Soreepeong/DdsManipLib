using System;
using System.Buffers.Binary;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R32G32B32A32UIntPixelFormat : R32G32B32A32PixelFormat, IRawRgbPixelFormat<uint>, IRawAPixelFormat<uint> {
    public override DxgiFormat DxgiFormat => DxgiFormat.R32G32B32A32UInt;
    public override float GetRed(ReadOnlySpan<byte> pixel) => GetRedTyped(pixel);
    public override float GetGreen(ReadOnlySpan<byte> pixel) => GetGreenTyped(pixel);
    public override float GetBlue(ReadOnlySpan<byte> pixel) => GetBlueTyped(pixel);
    public override float GetAlpha(ReadOnlySpan<byte> pixel) => GetAlphaTyped(pixel);
    public uint GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetR..]);
    public uint GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetG..]);
    public uint GetBlueTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetB..]);
    public uint GetAlphaTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetA..]);
    public override void SetRed(Span<byte> pixel, float value) => SetRed(pixel, uint.CreateTruncating(value));
    public override void SetGreen(Span<byte> pixel, float value) => SetGreen(pixel, uint.CreateTruncating(value));
    public override void SetBlue(Span<byte> pixel, float value) => SetBlue(pixel, uint.CreateTruncating(value));
    public override void SetAlpha(Span<byte> pixel, float value) => SetAlpha(pixel, uint.CreateTruncating(value));
    public void SetRed(Span<byte> pixel, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetR..], value);
    public void SetGreen(Span<byte> pixel, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetG..], value);
    public void SetBlue(Span<byte> pixel, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetB..], value);
    public void SetAlpha(Span<byte> pixel, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetA..], value);
    public R32G32B32A32UIntPixelFormat(AlphaType alphaType) : base(alphaType) { }
}
