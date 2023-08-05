using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R8G8SIntPixelFormat : R8G8PixelFormat, IRawRgPixelFormat<sbyte> {
    public override DxgiFormat DxgiFormat => DxgiFormat.R8G8SInt;
    public override float GetRed(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetR];
    public override float GetGreen(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetG];
    public sbyte GetRedTyped(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetR];
    public sbyte GetGreenTyped(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetG];
    public override void SetRed(Span<byte> pixel, float value) => pixel[OffsetR] = (byte) sbyte.CreateSaturating(value);
    public override void SetGreen(Span<byte> pixel, float value) => pixel[OffsetG] = (byte) sbyte.CreateSaturating(value);
    public void SetRed(Span<byte> pixel, sbyte value) => pixel[OffsetR] = (byte) value;
    public void SetGreen(Span<byte> pixel, sbyte value) => pixel[OffsetG] = (byte) value;
}
