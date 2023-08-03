using System;
using DdsManipLib.BcCodec;
using DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.BlockPixelFormats;

public abstract class Bc3PixelFormat : BlockPixelFormat {
    public override int BitsPerPixel => 8;
    public override int BlockBytes => 16;
    public override int BlockWidth => 4;
    public override int BlockHeight => 4;

    public override int CalculatePitch(int width) => Math.Max((width + 3) / 4, 1) * 16;
    public override int CalculateLinearSize(int width, int height) => Math.Max((width + 3) / 4, 1) * Math.Max((height + 3) / 4, 1) * 16;
    public override bool SupportsRawPixelFormat(IRawPixelFormat rawpf) => rawpf is IRawRAlignedBytePixelFormat;

    public override void Decompress(IRawPixelFormat rawPixelFormat, ReadOnlySpan<byte> sourceSpan, int width, int height, Span<byte> targetSpan) =>
        Squish.DecompressImage(
            targetSpan,
            rawPixelFormat.CalculatePitch(width),
            width,
            height,
            sourceSpan,
            GetSquishOptions2(rawPixelFormat));

    public override void Compress(IRawPixelFormat rawPixelFormat, ReadOnlySpan<byte> sourceSpan, int width, int height, Span<byte> targetSpan) =>
        Squish.CompressImage(
            sourceSpan,
            rawPixelFormat.CalculatePitch(width),
            width,
            height,
            targetSpan,
            GetSquishOptions2(rawPixelFormat));

    public void Compress(IRawPixelFormat rawPixelFormat, ReadOnlySpan<byte> sourceSpan, int width, int height, Span<byte> targetSpan, SquishOptions2 options) =>
        Squish.CompressImage(
            sourceSpan,
            rawPixelFormat.CalculatePitch(width),
            width,
            height,
            targetSpan,
            GetSquishOptions2(rawPixelFormat, options));

    private static SquishOptions2 GetSquishOptions2(IRawPixelFormat fmt, SquishOptions2? template = default) {
        template ??= new();
        template.Method = SquishMethod.Bc3;
        template.NumBytesPerPixel = (byte) fmt.BytesPerPixel;
        template.OffsetR = (byte) (fmt is IRawRAlignedBytePixelFormat r ? r.OffsetR : throw new ArgumentException(null, nameof(fmt)));
        template.OffsetG = (byte) (fmt is IRawRgAlignedBytePixelFormat rg ? rg.OffsetG : 0xFF);
        template.OffsetB = (byte) (fmt is IRawRgbAlignedBytePixelFormat rgb ? rgb.OffsetB : 0xFF);
        template.OffsetA = (byte) (fmt is IRawRgbaAlignedBytePixelFormat rgba ? rgba.OffsetA : 0xFF);
        return template;
    }

    protected Bc3PixelFormat(AlphaType alphaType) : base(alphaType) { }
}

public sealed class Bc3TypelessPixelFormat : Bc3PixelFormat {
    public override DxgiFormat DxgiFormat => DxgiFormat.Bc3Typeless;

    public override IRawPixelFormat SuggestedRawPixelFormat => AlphaType == AlphaType.None
        ? new B8G8R8X8TypelessPixelFormat()
        : new B8G8R8A8TypelessPixelFormat(AlphaType);

    public Bc3TypelessPixelFormat(AlphaType alphaType) : base(alphaType) { }
}

public sealed class Bc3UNormPixelFormat : Bc3PixelFormat {
    public override DxgiFormat DxgiFormat => DxgiFormat.Bc3UNorm;

    public override DdsPixelFormat DdsPixelFormat =>
        DdsPixelFormat.FromFourCc(AlphaType switch {
            AlphaType.Premultiplied => DdsFourCc.Dxt4,
            AlphaType.Straight => DdsFourCc.Dxt5,
            _ => DdsFourCc.Dx10,
        });

    public override IRawPixelFormat SuggestedRawPixelFormat => AlphaType == AlphaType.None
        ? new B8G8R8X8UNormPixelFormat()
        : new B8G8R8A8UNormPixelFormat(AlphaType);

    public Bc3UNormPixelFormat(AlphaType alphaType) : base(alphaType) { }
}

public sealed class Bc3UNormSrgbPixelFormat : Bc3PixelFormat {
    public override DxgiFormat DxgiFormat => DxgiFormat.Bc3UNormSrgb;

    public override IRawPixelFormat SuggestedRawPixelFormat => AlphaType == AlphaType.None
        ? new B8G8R8X8UNormSrgbPixelFormat()
        : new B8G8R8A8UNormSrgbPixelFormat(AlphaType);

    public Bc3UNormSrgbPixelFormat(AlphaType alphaType) : base(alphaType) { }
}
