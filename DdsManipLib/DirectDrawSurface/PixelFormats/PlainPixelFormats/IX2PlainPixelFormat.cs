using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.PlainPixelFormats;

public interface IX2PlainPixelFormat : IX1PlainPixelFormat {
    public IChannel? X2 { get; }
}