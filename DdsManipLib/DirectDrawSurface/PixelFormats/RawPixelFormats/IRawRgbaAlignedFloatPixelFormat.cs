namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public interface IRawRgbaAlignedFloatPixelFormat : IRawRgbaPixelFormat<float>, IRawRgbAlignedFloatPixelFormat {
    public int OffsetA { get; }
}