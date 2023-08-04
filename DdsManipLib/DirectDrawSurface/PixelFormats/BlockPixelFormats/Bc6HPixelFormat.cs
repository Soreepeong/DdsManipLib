using System;
using DdsManipLib.BcCodec;
using DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.BlockPixelFormats;

public abstract class Bc6HPixelFormat : BlockPixelFormat {
    public override int BitsPerPixel => 8;
    public override int BlockBytes => 16;
    public override int BlockWidth => 4;
    public override int BlockHeight => 4;

    public override IRawPixelFormat SuggestedRawPixelFormat => new R32G32B32FloatPixelFormat();

    public override int CalculatePitch(int width) => Math.Max((width + 3) / 4, 1) * 16;
    public override int CalculateLinearSize(int width, int height) => Math.Max((width + 3) / 4, 1) * Math.Max((height + 3) / 4, 1) * 16;
    public override bool SupportsRawPixelFormat(IRawPixelFormat rawpf) => rawpf is IRawRAlignedFloatPixelFormat;

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

    protected virtual SquishOptions2 GetSquishOptions2(IRawPixelFormat fmt, SquishOptions2? template = default) {
        template ??= new();
        template.Method = SquishMethod.Bc6U;
        template.NumBytesPerPixel = (byte) fmt.BytesPerPixel;
        template.OffsetR = (byte) (fmt is IRawRAlignedFloatPixelFormat r ? r.OffsetR : throw new ArgumentException(null, nameof(fmt)));
        template.OffsetG = (byte) (fmt is IRawRgAlignedFloatPixelFormat rg ? rg.OffsetG : 0xFF);
        template.OffsetB = (byte) (fmt is IRawRgbAlignedFloatPixelFormat rgb ? rgb.OffsetB : 0xFF);
        return template;
    }

    protected Bc6HPixelFormat() : base(AlphaType.None) { }
}

public sealed class Bc6HTypelessPixelFormat : Bc6HPixelFormat {
    public override DxgiFormat DxgiFormat => DxgiFormat.Bc6HTypeless;
}

public sealed class Bc6HUf16PixelFormat : Bc6HPixelFormat, IPixelFormat {
    public override DxgiFormat DxgiFormat => DxgiFormat.Bc6HUf16;
}

public sealed class Bc6HSf16PixelFormat : Bc6HPixelFormat {
    public override DxgiFormat DxgiFormat => DxgiFormat.Bc6HSf16;

    protected override SquishOptions2 GetSquishOptions2(IRawPixelFormat fmt, SquishOptions2? template = default) => 
        base.GetSquishOptions2(fmt, template) with {Method=SquishMethod.Bc6S};
}
