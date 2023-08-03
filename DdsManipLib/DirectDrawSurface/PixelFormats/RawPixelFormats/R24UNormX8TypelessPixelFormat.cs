using System;
using System.Buffers.Binary;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R24UNormX8TypelessPixelFormat : RawRPixelFormat, IRawRPixelFormat<uint> {
    public const int OffsetR = 0;

    public override DxgiFormat DxgiFormat => DxgiFormat.R24UNormX8Typeless;
    public override DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromRgba(32, 0xFFFFFFu, 0u, 0u);
    public override int BitsPerPixel => 32;
    public override int BytesPerPixel => 4;
    public override float GetRed(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetR..]) & 0xFFFFFFu;
    public uint GetRedTyped(ReadOnlySpan<byte> pixel) => 
        (uint)(pixel[OffsetR + 2] | (pixel[OffsetR] << 8) | (pixel[OffsetR + 1] << 16) | (pixel[OffsetR + 2] << 24));

    public override void SetRed(Span<byte> pixel, float value) => SetRed(pixel, uint.CreateTruncating(value * uint.MaxValue));
    public void SetRed(Span<byte> pixel, uint value) {
        pixel[OffsetR] = (byte) (value >> 8);
        pixel[OffsetR + 1] = (byte) (value >> 16);
        pixel[OffsetR + 2] = (byte) (value >> 24);
    }

    public R24UNormX8TypelessPixelFormat() : base(AlphaType.None) { }
}
