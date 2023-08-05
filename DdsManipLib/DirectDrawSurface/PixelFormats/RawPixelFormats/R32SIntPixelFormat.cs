using System;
using System.Buffers.Binary;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R32SIntPixelFormat : RawRPixelFormat, IRawRPixelFormat<int> {
    public const int OffsetR = 0;

    public override DxgiFormat DxgiFormat => DxgiFormat.R32SInt;
    public override int BitsPerPixel => 32;
    public override int BytesPerPixel => 4;
    public override float GetRed(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetR..]);
    public int GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetR..]);
    public override void SetRed(Span<byte> pixel, float value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetR..], int.CreateSaturating(value));
    public void SetRed(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetR..], value);
    public R32SIntPixelFormat() : base(AlphaType.None) { }
}
