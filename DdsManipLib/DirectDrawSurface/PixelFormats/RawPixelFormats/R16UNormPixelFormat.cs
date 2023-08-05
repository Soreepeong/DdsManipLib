using System;
using System.Buffers.Binary;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class R16UNormPixelFormat : RawRPixelFormat, IRawRPixelFormat<ushort> {
    public const int OffsetR = 0;

    public override DxgiFormat DxgiFormat => DxgiFormat.R16UNorm;
    public override DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromRgba(16, 0xFFFFu, 0u, 0u);
    public override int BitsPerPixel => 16;
    public override int BytesPerPixel => 2;
    public override float GetRed(ReadOnlySpan<byte> pixel) => GetRedTyped(pixel) / 65535f;
    public ushort GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetR..]);
    public override void SetRed(Span<byte> pixel, float value) => SetRed(pixel, ushort.CreateSaturating(value * 65536f));
    public void SetRed(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetR..], value);
    public R16UNormPixelFormat() : base(AlphaType.None) { }
}