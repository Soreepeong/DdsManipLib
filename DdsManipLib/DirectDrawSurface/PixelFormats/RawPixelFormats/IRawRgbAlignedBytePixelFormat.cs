namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public interface IRawRgbAlignedBytePixelFormat : IRawRgbPixelFormat, IRawRgAlignedBytePixelFormat {
    public int OffsetB { get; }
}