#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public interface IRawPixelFormat : IPixelFormat {
    public int BytesPerPixel { get; }
}
