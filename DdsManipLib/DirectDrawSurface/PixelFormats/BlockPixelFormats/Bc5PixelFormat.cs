using System;
using DdsManipLib.BcCodec;
using DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.BlockPixelFormats;

public abstract class Bc5PixelFormat : BlockPixelFormat {
    public override int BitsPerPixel => 8;
    public override int BlockBytes => 16;
    public override int BlockWidth => 4;
    public override int BlockHeight => 4;

    public override int CalculatePitch(int width) => Math.Max((width + 3) / 4, 1) * 16;
    public override int CalculateLinearSize(int width, int height) => Math.Max((width + 3) / 4, 1) * Math.Max((height + 3) / 4, 1) * 16;
    public override bool SupportsRawPixelFormat(IRawPixelFormat rawpf) => rawpf is IRawRAlignedBytePixelFormat;

    public override void Decompress(IRawPixelFormat rawPixelFormat, ReadOnlySpan<byte> sourceSpan, int width, int height, Span<byte> targetSpan) => Squish.DecompressImage(
        targetSpan,
        rawPixelFormat.CalculatePitch(width),
        width,
        height,
        sourceSpan,
        GetSquishOptions2(rawPixelFormat));

    public override void Compress(IRawPixelFormat rawPixelFormat, ReadOnlySpan<byte> sourceSpan, int width, int height, Span<byte> targetSpan) => Squish.CompressImage(
        sourceSpan,
        rawPixelFormat.CalculatePitch(width),
        width,
        height,
        targetSpan,
        GetSquishOptions2(rawPixelFormat));

    public void Compress(IRawPixelFormat rawPixelFormat, ReadOnlySpan<byte> sourceSpan, int width, int height, Span<byte> targetSpan, SquishOptions2 options) => Squish.CompressImage(
        sourceSpan,
        rawPixelFormat.CalculatePitch(width),
        width,
        height,
        targetSpan,
        GetSquishOptions2(rawPixelFormat, options));

    private static SquishOptions2 GetSquishOptions2(IRawPixelFormat fmt, SquishOptions2? template = default) {
        template ??= new();
        template.Method = SquishMethod.Bc5;
        template.NumBytesPerPixel = (byte) fmt.BytesPerPixel;
        template.OffsetR = (byte) (fmt is IRawRAlignedBytePixelFormat r ? r.OffsetR : throw new ArgumentException(null, nameof(fmt)));
        template.OffsetG = (byte) (fmt is IRawRgAlignedBytePixelFormat rg ? rg.OffsetG : 0xFF);
        return template;
    }

    protected Bc5PixelFormat() : base(AlphaType.None) { }
}

public sealed class Bc5TypelessPixelFormat : Bc5PixelFormat {
    public override DxgiFormat DxgiFormat => DxgiFormat.Bc5Typeless;
    public override IRawPixelFormat SuggestedRawPixelFormat => new R8G8SNormPixelFormat();
}

public sealed class Bc5UNormPixelFormat : Bc5PixelFormat, IPixelFormat {
    public override DxgiFormat DxgiFormat => DxgiFormat.Bc5UNorm;
    public override DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Bc5U);
    public override IRawPixelFormat SuggestedRawPixelFormat => new R8G8SNormPixelFormat();

    public bool IsDdsPixelFormat(in DdsPixelFormat ddspf)
        => ddspf.Flags.HasFlag(DdsPixelFormatFlags.FourCc) && ddspf.FourCc is DdsFourCc.Bc5 or DdsFourCc.Bc5U;
}

public sealed class Bc5SNormPixelFormat : Bc5PixelFormat {
    public override DxgiFormat DxgiFormat => DxgiFormat.Bc5SNorm;
    public override DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Bc5S);
    public override IRawPixelFormat SuggestedRawPixelFormat => new R8G8SNormPixelFormat();
}
