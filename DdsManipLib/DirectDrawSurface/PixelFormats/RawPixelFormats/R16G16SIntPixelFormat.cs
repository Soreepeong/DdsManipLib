using System;
using System.Buffers.Binary;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R16G16SIntPixelFormat : R16G16PixelFormat, IRawRgPixelFormat<short> {
    public override DxgiFormat DxgiFormat => DxgiFormat.R16G16SInt;
    public override float GetRed(ReadOnlySpan<byte> pixel) => GetRedTyped(pixel);
    public override float GetGreen(ReadOnlySpan<byte> pixel) => GetGreenTyped(pixel);
    public short GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetR..]);
    public short GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetG..]);
    public override void SetRed(Span<byte> pixel, float value) => SetRed(pixel, short.CreateTruncating(value));
    public override void SetGreen(Span<byte> pixel, float value) => SetGreen(pixel, short.CreateTruncating(value));
    public void SetRed(Span<byte> pixel, short value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetR..], value);
    public void SetGreen(Span<byte> pixel, short value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetG..], value);
}
