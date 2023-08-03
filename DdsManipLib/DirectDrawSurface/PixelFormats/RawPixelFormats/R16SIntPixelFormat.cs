using System;
using System.Buffers.Binary;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R16SIntPixelFormat : RawRPixelFormat, IRawRPixelFormat<short> {
    public const int OffsetR = 0;

    public override DxgiFormat DxgiFormat => DxgiFormat.R16SInt;
    public override int BitsPerPixel => 16;
    public override int BytesPerPixel => 2;
    public override float GetRed(ReadOnlySpan<byte> pixel) => GetRedTyped(pixel);
    public short GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetR..]);
    public override void SetRed(Span<byte> pixel, float value) => SetRed(pixel, short.CreateTruncating(value));
    public void SetRed(Span<byte> pixel, short value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetR..], value);
    public R16SIntPixelFormat() : base(AlphaType.None) { }
}
