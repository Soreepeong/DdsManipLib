namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public interface IRawRgbaAlignedBytePixelFormat : IRawRgbaPixelFormat, IRawRgbAlignedBytePixelFormat {
    public int OffsetA { get; }
}