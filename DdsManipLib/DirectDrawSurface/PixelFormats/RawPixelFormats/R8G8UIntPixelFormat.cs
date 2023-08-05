using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R8G8UIntPixelFormat : R8G8PixelFormat, IRawRgPixelFormat<byte> {
    public override DxgiFormat DxgiFormat => DxgiFormat.R8G8UInt;
    public override float GetRed(ReadOnlySpan<byte> pixel) => pixel[OffsetR];
    public override float GetGreen(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
    public byte GetRedTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetR];
    public byte GetGreenTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
    public override void SetRed(Span<byte> pixel, float value) => pixel[OffsetR] = byte.CreateSaturating(value);
    public override void SetGreen(Span<byte> pixel, float value) => pixel[OffsetG] = byte.CreateSaturating(value);
    public void SetRed(Span<byte> pixel, byte value) => pixel[OffsetR] = value;
    public void SetGreen(Span<byte> pixel, byte value) => pixel[OffsetG] = value;
}
