using System;
using DdsManipLib.BcCodec;
using DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.BlockPixelFormats;

public abstract class Bc4PixelFormat : BlockPixelFormat {
    public override int BitsPerPixel => 4;
    public override int BlockBytes => 8;
    public override int BlockWidth => 4;
    public override int BlockHeight => 4;

    public override int CalculatePitch(int width) => Math.Max((width + 3) / 4, 1) * 8;
    public override int CalculateLinearSize(int width, int height) => Math.Max((width + 3) / 4, 1) * Math.Max((height + 3) / 4, 1) * 8;
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

    protected virtual SquishOptions2 GetSquishOptions2(IRawPixelFormat fmt, SquishOptions2? template = default) {
        template ??= new();
        template.Method = SquishMethod.Bc4;
        template.NumBytesPerPixel = (byte) fmt.BytesPerPixel;
        template.OffsetR = (byte) (fmt is IRawRAlignedBytePixelFormat r ? r.OffsetR : throw new ArgumentException(null, nameof(fmt)));
        return template;
    }

    protected Bc4PixelFormat() : base(AlphaType.None) { }
}

public sealed class Bc4TypelessPixelFormat : Bc4PixelFormat {
    public override DxgiFormat DxgiFormat => DxgiFormat.Bc4Typeless;
    public override IRawPixelFormat SuggestedRawPixelFormat => new R8SNormPixelFormat();
}

public sealed class Bc4UNormPixelFormat : Bc4PixelFormat, IPixelFormat {
    public override DxgiFormat DxgiFormat => DxgiFormat.Bc4UNorm;
    public override DdsFourCc FourCc => DdsFourCc.Bc4U;
    public override IRawPixelFormat SuggestedRawPixelFormat => new R8SNormPixelFormat();

    public bool IsDdsPixelFormat(in DdsPixelFormat ddspf)
        => ddspf.Flags.HasFlag(DdsPixelFormatFlags.FourCc) && ddspf.FourCc is DdsFourCc.Bc4 or DdsFourCc.Bc4U;
}

public sealed class Bc4SNormPixelFormat : Bc4PixelFormat {
    public override DxgiFormat DxgiFormat => DxgiFormat.Bc4SNorm;
    public override DdsFourCc FourCc => DdsFourCc.Bc4S;
    public override IRawPixelFormat SuggestedRawPixelFormat => new R8SNormPixelFormat();

    protected override SquishOptions2 GetSquishOptions2(IRawPixelFormat fmt, SquishOptions2? template = default) =>
        base.GetSquishOptions2(fmt, template) with {Method = SquishMethod.Bc4S};
}
