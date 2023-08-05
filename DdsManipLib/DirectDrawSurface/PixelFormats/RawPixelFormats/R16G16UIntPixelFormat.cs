using System;
using System.Buffers.Binary;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R16G16UIntPixelFormat : R16G16PixelFormat, IRawRgPixelFormat<ushort> {
    public override DxgiFormat DxgiFormat => DxgiFormat.R16G16UInt;
    public override float GetRed(ReadOnlySpan<byte> pixel) => GetRedTyped(pixel);
    public override float GetGreen(ReadOnlySpan<byte> pixel) => GetGreenTyped(pixel);
    public ushort GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetR..]);
    public ushort GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetG..]);
    public override void SetRed(Span<byte> pixel, float value) => SetRed(pixel, ushort.CreateSaturating(value));
    public override void SetGreen(Span<byte> pixel, float value) => SetGreen(pixel, ushort.CreateSaturating(value));
    public void SetRed(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetR..], value);
    public void SetGreen(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetG..], value);
}
