using System;
using DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.BlockPixelFormats;

public interface IBlockPixelFormat : IPixelFormat {
    public int BlockBytes { get; }
    public int BlockWidth { get; }
    public int BlockHeight { get; }
    public IRawPixelFormat SuggestedRawPixelFormat { get; }
    public bool SupportsRawPixelFormat(IRawPixelFormat rawpf);
    public void Decompress(IRawPixelFormat rawPixelFormat, ReadOnlySpan<byte> sourceSpan, int width, int height, Span<byte> targetSpan);
    public void Compress(IRawPixelFormat rawPixelFormat, ReadOnlySpan<byte> sourceSpan, int width, int height, Span<byte> targetSpan);
}
