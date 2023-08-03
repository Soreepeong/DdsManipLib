﻿using System;
using System.Buffers.Binary;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R32G32TypelessPixelFormat : R32G32PixelFormat, IRawRgPixelFormat<uint>, IRawRgPixelFormat<int>, IRawRgPixelFormat<float> {
    public override DxgiFormat DxgiFormat => DxgiFormat.R32G32Typeless;
    public override float GetRed(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetR..]);
    public override float GetGreen(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetG..]);
    float IRawRPixelFormat<float>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetR..]);
    float IRawGPixelFormat<float>.GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetG..]);
    uint IRawRPixelFormat<uint>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetR..]);
    uint IRawGPixelFormat<uint>.GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetG..]);
    int IRawRPixelFormat<int>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetR..]);
    int IRawGPixelFormat<int>.GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetG..]);
    public override void SetRed(Span<byte> pixel, float value) => BinaryPrimitives.WriteSingleLittleEndian(pixel[OffsetR..], value);
    public override void SetGreen(Span<byte> pixel, float value) => BinaryPrimitives.WriteSingleLittleEndian(pixel[OffsetG..], value);
    public void SetRed(Span<byte> pixel, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetR..], value);
    public void SetGreen(Span<byte> pixel, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetG..], value);
    public void SetRed(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetR..], value);
    public void SetGreen(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetG..], value);
}
