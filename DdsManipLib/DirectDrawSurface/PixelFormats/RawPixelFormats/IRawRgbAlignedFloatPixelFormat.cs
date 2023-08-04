namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public interface IRawRgbAlignedFloatPixelFormat : IRawRgbPixelFormat<float>, IRawRgAlignedFloatPixelFormat {
    public int OffsetB { get; }
}