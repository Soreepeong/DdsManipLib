using System;
using System.Buffers.Binary;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R32G32SIntPixelFormat : R32G32PixelFormat, IRawRgPixelFormat<int> {
    public override DxgiFormat DxgiFormat => DxgiFormat.R32G32SInt;
    public override float GetRed(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetR..]);
    public override float GetGreen(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetG..]);
    public int GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetR..]);
    public int GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetG..]);
    public override void SetRed(Span<byte> pixel, float value) => SetRed(pixel, int.CreateTruncating(value));
    public override void SetGreen(Span<byte> pixel, float value) => SetGreen(pixel, int.CreateTruncating(value));
    public void SetRed(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetR..], value);
    public void SetGreen(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetG..], value);
}
