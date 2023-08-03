using System;
using System.Buffers.Binary;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R16UNormX8TypelessPixelFormat : RawRPixelFormat, IRawRPixelFormat<ushort> {
    public const int OffsetR = 0;

    public override DxgiFormat DxgiFormat => DxgiFormat.R16UNormX8Typeless;
    public override DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromRgba(24, 0xFFFFu, 0u, 0u);
    public override int BitsPerPixel => 24;
    public override int BytesPerPixel => 3;
    public override float GetRed(ReadOnlySpan<byte> pixel) => GetRedTyped(pixel) / 65535f;
    public ushort GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetR..]);
    public override void SetRed(Span<byte> pixel, float value) => SetRed(pixel, ushort.CreateTruncating(value * 65536f));
    public void SetRed(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetR..], value);
    public R16UNormX8TypelessPixelFormat() : base(AlphaType.None) { }
}