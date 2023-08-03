using System;
using System.Buffers.Binary;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public sealed class R16G16B16A16SNormPixelFormat : R16G16B16A16PixelFormat, IRawRgbPixelFormat<short>, IRawAlphaPixelFormat<short> {
    public override DxgiFormat DxgiFormat => DxgiFormat.R16G16B16A16SNorm;
    public override float GetRed(ReadOnlySpan<byte> pixel) => Math.Max(-1f, GetRedTyped(pixel) / 32767f);
    public override float GetGreen(ReadOnlySpan<byte> pixel) => Math.Max(-1f, GetGreenTyped(pixel) / 32767f);
    public override float GetBlue(ReadOnlySpan<byte> pixel) => Math.Max(-1f, GetBlueTyped(pixel) / 32767f);
    public override float GetAlpha(ReadOnlySpan<byte> pixel) => Math.Max(-1f, GetAlphaTyped(pixel) / 32767f);
    public short GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetR..]);
    public short GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetG..]);
    public short GetBlueTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetB..]);
    public short GetAlphaTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetA..]);
    public override void SetRed(Span<byte> pixel, float value) => SetRed(pixel, short.CreateTruncating(32768f * Math.Clamp(value, -1, 1)));
    public override void SetGreen(Span<byte> pixel, float value) => SetGreen(pixel, short.CreateTruncating(32768f * Math.Clamp(value, -1, 1)));
    public override void SetBlue(Span<byte> pixel, float value) => SetBlue(pixel, short.CreateTruncating(32768f * Math.Clamp(value, -1, 1)));
    public override void SetAlpha(Span<byte> pixel, float value) => SetAlpha(pixel, short.CreateTruncating(32768f * Math.Clamp(value, -1, 1)));
    public void SetRed(Span<byte> pixel, short value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetR..], value);
    public void SetGreen(Span<byte> pixel, short value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetG..], value);
    public void SetBlue(Span<byte> pixel, short value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetB..], value);
    public void SetAlpha(Span<byte> pixel, short value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetA..], value);
}
