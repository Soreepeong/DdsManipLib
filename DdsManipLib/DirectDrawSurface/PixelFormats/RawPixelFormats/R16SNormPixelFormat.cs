using System;
using System.Buffers.Binary;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R16SNormPixelFormat : RawRPixelFormat, IRawRPixelFormat<short> {
    public const int OffsetR = 0;

    public override DxgiFormat DxgiFormat => DxgiFormat.R16SNorm;
    public override int BitsPerPixel => 16;
    public override int BytesPerPixel => 2;
    public override float GetRed(ReadOnlySpan<byte> pixel) => Math.Clamp(GetRedTyped(pixel) / 32767f, -1f, 1f);
    public short GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetR..]);
    public override void SetRed(Span<byte> pixel, float value) => SetRed(pixel, short.CreateTruncating(Math.Clamp(value, -1f, 1f) * 32768f));
    public void SetRed(Span<byte> pixel, short value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetR..], value);
    public R16SNormPixelFormat() : base(AlphaType.None) { }
}
