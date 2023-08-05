using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R8G8B8A8UIntPixelFormat : R8G8B8A8PixelFormat, IRawRgbaPixelFormat<byte> {
    public override DxgiFormat DxgiFormat => DxgiFormat.R8G8B8A8UInt;
    public override float GetRed(ReadOnlySpan<byte> pixel) => pixel[OffsetR];
    public override float GetGreen(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
    public override float GetBlue(ReadOnlySpan<byte> pixel) => pixel[OffsetB];
    public override float GetAlpha(ReadOnlySpan<byte> pixel) => pixel[OffsetA];
    public byte GetRedTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetR];
    public byte GetGreenTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
    public byte GetBlueTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetB];
    public byte GetAlphaTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetA];
    public override void SetRed(Span<byte> pixel, float value) => pixel[OffsetR] = byte.CreateSaturating(value);
    public override void SetGreen(Span<byte> pixel, float value) => pixel[OffsetG] = byte.CreateSaturating(value);
    public override void SetBlue(Span<byte> pixel, float value) => pixel[OffsetB] = byte.CreateSaturating(value);
    public override void SetAlpha(Span<byte> pixel, float value) => pixel[OffsetA] = byte.CreateSaturating(value);
    public void SetRed(Span<byte> pixel, byte value) => pixel[OffsetR] = value;
    public void SetGreen(Span<byte> pixel, byte value) => pixel[OffsetG] = value;
    public void SetBlue(Span<byte> pixel, byte value) => pixel[OffsetB] = value;
    public void SetAlpha(Span<byte> pixel, byte value) => pixel[OffsetA] = value;
    public R8G8B8A8UIntPixelFormat(AlphaType alphaType) : base(alphaType) { }
}
