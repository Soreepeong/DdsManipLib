using System;
using System.Buffers.Binary;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R16G16B16A16UNormPixelFormat : R16G16B16A16PixelFormat, IRawRgbPixelFormat<ushort>, IRawAPixelFormat<ushort> {
    public override DdsFourCc FourCc => AlphaType != AlphaType.Straight ? DdsFourCc.Unknown : DdsFourCc.D3dFmtA16B16G16R16;
    public override DxgiFormat DxgiFormat => DxgiFormat.R16G16B16A16UNorm;
    public override float GetRed(ReadOnlySpan<byte> pixel) => GetRedTyped(pixel) / 65535f;
    public override float GetGreen(ReadOnlySpan<byte> pixel) => GetGreenTyped(pixel) / 65535f;
    public override float GetBlue(ReadOnlySpan<byte> pixel) => GetBlueTyped(pixel) / 65535f;
    public override float GetAlpha(ReadOnlySpan<byte> pixel) => GetAlphaTyped(pixel) / 65535f;
    public ushort GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetR..]);
    public ushort GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetG..]);
    public ushort GetBlueTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetB..]);
    public ushort GetAlphaTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetA..]);
    public override void SetRed(Span<byte> pixel, float value) => SetRed(pixel, ushort.CreateSaturating(65536 * value));
    public override void SetGreen(Span<byte> pixel, float value) => SetGreen(pixel, ushort.CreateSaturating(65536 * value));
    public override void SetBlue(Span<byte> pixel, float value) => SetBlue(pixel, ushort.CreateSaturating(65536 * value));
    public override void SetAlpha(Span<byte> pixel, float value) => SetAlpha(pixel, ushort.CreateSaturating(65536 * value));
    public void SetRed(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetR..], value);
    public void SetGreen(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetG..], value);
    public void SetBlue(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetB..], value);
    public void SetAlpha(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetA..], value);
    public R16G16B16A16UNormPixelFormat(AlphaType alphaType) : base(alphaType) { }
}
