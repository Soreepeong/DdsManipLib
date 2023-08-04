namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public interface IRawRAlignedFloatPixelFormat : IRawRPixelFormat<float> {
    public int OffsetR { get; }
}