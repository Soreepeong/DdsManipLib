namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public interface IRawRgAlignedFloatPixelFormat : IRawRgPixelFormat<float>, IRawRAlignedFloatPixelFormat {
    public int OffsetG { get; }
}