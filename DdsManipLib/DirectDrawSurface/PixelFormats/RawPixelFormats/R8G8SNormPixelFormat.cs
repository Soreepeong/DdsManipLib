using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R8G8SNormPixelFormat : R8G8PixelFormat, IRawRgPixelFormat<sbyte> {
    public override DxgiFormat DxgiFormat => DxgiFormat.R8G8SNorm;
    public override float GetRed(ReadOnlySpan<byte> pixel) => Math.Clamp((sbyte) pixel[OffsetR] / 127f, -1f, 1f);
    public override float GetGreen(ReadOnlySpan<byte> pixel) => Math.Clamp((sbyte) pixel[OffsetG] / 127f, -1f, 1f);
    public sbyte GetRedTyped(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetR];
    public sbyte GetGreenTyped(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetG];
    public override void SetRed(Span<byte> pixel, float value) => pixel[OffsetR] = (byte) sbyte.CreateTruncating(Math.Clamp(value, -1, 1) * 128);
    public override void SetGreen(Span<byte> pixel, float value) => pixel[OffsetG] = (byte) sbyte.CreateTruncating(Math.Clamp(value, -1, 1) * 128);
    public void SetRed(Span<byte> pixel, sbyte value) => pixel[OffsetR] = (byte) value;
    public void SetGreen(Span<byte> pixel, sbyte value) => pixel[OffsetG] = (byte) value;
}
