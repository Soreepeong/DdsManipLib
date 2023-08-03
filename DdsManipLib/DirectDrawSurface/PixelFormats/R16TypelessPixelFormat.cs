using System;
using System.Buffers.Binary;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public sealed class R16TypelessPixelFormat : RawRPixelFormat, IRawRPixelFormat<ushort>, IRawRPixelFormat<short>, IRawRPixelFormat<Half> {
    public const int OffsetR = 0;

    public override DxgiFormat DxgiFormat => DxgiFormat.R16Typeless;
    public override int BitsPerPixel => 16;
    public override int BytesPerPixel => 2;
    public override float GetRed(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetR..]);
    Half IRawRPixelFormat<Half>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetR..]);
    ushort IRawRPixelFormat<ushort>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetR..]);
    short IRawRPixelFormat<short>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetR..]);
    public override void SetRed(Span<byte> pixel, float value) => SetRed(pixel, Half.CreateTruncating(value));
    public void SetRed(Span<byte> pixel, Half value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetR..], value);
    public void SetRed(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetR..], value);
    public void SetRed(Span<byte> pixel, short value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetR..], value);
}
