using System;
using System.Buffers.Binary;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public sealed class R32UIntPixelFormat : RawRPixelFormat, IRawRPixelFormat<uint> {
    public const int OffsetR = 0;

    public override DxgiFormat DxgiFormat => DxgiFormat.R32UInt;
    public override int BitsPerPixel => 32;
    public override int BytesPerPixel => 4;
    public override float GetRed(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetR..]);
    public uint GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetR..]);
    public override void SetRed(Span<byte> pixel, float value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetR..], uint.CreateTruncating(value));
    public void SetRed(Span<byte> pixel, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetR..], value);
}
