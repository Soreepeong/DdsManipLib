using System;
using System.Buffers.Binary;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public sealed class R16G16UNormPixelFormat : R16G16PixelFormat, IRawRgPixelFormat<ushort> {
    public override DxgiFormat DxgiFormat => DxgiFormat.R16G16UNorm;
    public override DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromRgba(32, 0xFFFF, 0xFFFF0000, 0);
    public override float GetRed(ReadOnlySpan<byte> pixel) => GetRedTyped(pixel) / 65535f;
    public override float GetGreen(ReadOnlySpan<byte> pixel) => GetGreenTyped(pixel) / 65535f;
    public ushort GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetR..]);
    public ushort GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetG..]);
    public override void SetRed(Span<byte> pixel, float value) => SetRed(pixel, ushort.CreateTruncating(value * 65536f));
    public override void SetGreen(Span<byte> pixel, float value) => SetGreen(pixel, ushort.CreateTruncating(value * 65536f));
    public void SetRed(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetR..], value);
    public void SetGreen(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetG..], value);
}
