using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public class B8G8R8UNormPixelFormat : RawRgbPixelFormat, IRawRgbAlignedBytePixelFormat, IRawRgbPixelFormat<byte> {
    public const int OffsetB = 0;
    public const int OffsetG = 1;
    public const int OffsetR = 2;

    public override DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromRgba(24, 0xFF, 0xFF00, 0xFF0000);
    public override DdsFourCc FourCc => DdsFourCc.D3dFmtR8G8B8;

    public override int BitsPerPixel => 32;
    public override int BytesPerPixel => 4;
    
    public override float GetRed(ReadOnlySpan<byte> pixel) => pixel[OffsetR] / 255f;
    public override float GetGreen(ReadOnlySpan<byte> pixel) => pixel[OffsetG] / 255f;
    public override float GetBlue(ReadOnlySpan<byte> pixel) => pixel[OffsetB] / 255f;
    public byte GetRedTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetR];
    public byte GetGreenTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
    public byte GetBlueTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetB];
    public override void SetRed(Span<byte> pixel, float value) => pixel[OffsetR] = byte.CreateSaturating(value * 256f);
    public override void SetGreen(Span<byte> pixel, float value) => pixel[OffsetG] = byte.CreateSaturating(value * 256f);
    public override void SetBlue(Span<byte> pixel, float value) => pixel[OffsetB] = byte.CreateSaturating(value * 256f);
    public void SetRed(Span<byte> pixel, byte value) => pixel[OffsetR] = value;
    public void SetGreen(Span<byte> pixel, byte value) => pixel[OffsetG] = value;
    public void SetBlue(Span<byte> pixel, byte value) => pixel[OffsetB] = value;
    
    protected B8G8R8UNormPixelFormat() : base(AlphaType.None) { }

    int IRawRAlignedBytePixelFormat.OffsetR => OffsetR;
    int IRawRgAlignedBytePixelFormat.OffsetG => OffsetG;
    int IRawRgbAlignedBytePixelFormat.OffsetB => OffsetB;
}
