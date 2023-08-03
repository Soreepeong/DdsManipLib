using System;
using System.Buffers.Binary;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public sealed class R32G8X24TypelessPixelFormat : RawRgPixelFormat, IRawRgPixelFormat<uint>, IRawRgPixelFormat<int>, IRawRgPixelFormat<float> {
    public const int OffsetR = 0;
    public const int OffsetG = 4;

    public override DxgiFormat DxgiFormat => DxgiFormat.R32G8X24Typeless;
    public override int BitsPerPixel => 64;
    public override int BytesPerPixel => 8;
    public override float GetRed(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetR..]);
    public override float GetGreen(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
    float IRawRPixelFormat<float>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetR..]);
    float IRawGPixelFormat<float>.GetGreenTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
    uint IRawRPixelFormat<uint>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetR..]);
    uint IRawGPixelFormat<uint>.GetGreenTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
    int IRawRPixelFormat<int>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetR..]);
    int IRawGPixelFormat<int>.GetGreenTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
    public override void SetRed(Span<byte> pixel, float value) => BinaryPrimitives.WriteSingleLittleEndian(pixel[OffsetR..], value);
    public override void SetGreen(Span<byte> pixel, float value) => pixel[OffsetG] = byte.CreateTruncating(value);
    public void SetRed(Span<byte> pixel, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetR..], value);
    public void SetGreen(Span<byte> pixel, uint value) => pixel[OffsetG] = byte.CreateTruncating(value);
    public void SetRed(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetR..], value);
    public void SetGreen(Span<byte> pixel, int value) => pixel[OffsetG] = byte.CreateTruncating(value);
}
