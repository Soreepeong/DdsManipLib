using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public interface IX2PixelFormat : IX1PixelFormat {
    public IChannel? X2 { get; }
}