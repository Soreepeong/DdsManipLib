using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public interface IX1PixelFormat : IPixelFormat {
    public IChannel? X1 { get; }
}