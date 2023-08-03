using System;
using System.Buffers.Binary;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public sealed class D16UNormPixelFormat : RawPixelFormat, IRawDPixelFormat<ushort> {
    public const int OffsetD = 0;

    public override DxgiFormat DxgiFormat => DxgiFormat.D16UNorm;
    public override int BitsPerPixel => 16;
    public override int BytesPerPixel => 2;
    public float GetDepth(ReadOnlySpan<byte> pixel) => GetDepthTyped(pixel) / 65535f;
    public ushort GetDepthTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetD..]);
    public void SetDepth(Span<byte> pixel, float value) => SetDepth(pixel, ushort.CreateTruncating(value * 65536f));
    public void SetDepth(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetD..], value);
}
