namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public interface IRawRgAlignedBytePixelFormat : IRawRgPixelFormat, IRawRAlignedBytePixelFormat {
    public int OffsetG { get; }
}