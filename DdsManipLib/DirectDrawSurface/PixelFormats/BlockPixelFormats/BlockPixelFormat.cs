using System;
using DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.BlockPixelFormats;

public abstract class BlockPixelFormat : IBlockPixelFormat {
    public virtual DxgiFormat DxgiFormat => DxgiFormat.Unknown;
    public virtual DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
    public AlphaType AlphaType { get; }
    public abstract int BitsPerPixel { get; }
    public abstract int CalculatePitch(int width);
    public abstract int CalculateLinearSize(int width, int height);
    public abstract int BlockBytes { get; }
    public abstract int BlockWidth { get; }
    public abstract int BlockHeight { get; }
    public abstract IRawPixelFormat SuggestedRawPixelFormat { get; }
    public abstract bool SupportsRawPixelFormat(IRawPixelFormat rawpf);
    public abstract void Decompress(IRawPixelFormat rawPixelFormat, ReadOnlySpan<byte> sourceSpan, int width, int height, Span<byte> targetSpan);
    public abstract void Compress(IRawPixelFormat rawPixelFormat, ReadOnlySpan<byte> sourceSpan, int width, int height, Span<byte> targetSpan);

    protected BlockPixelFormat(AlphaType alphaType) => AlphaType = alphaType;
}