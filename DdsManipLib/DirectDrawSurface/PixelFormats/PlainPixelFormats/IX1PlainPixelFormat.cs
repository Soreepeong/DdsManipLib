using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.PlainPixelFormats;

public interface IX1PlainPixelFormat : IPlainPixelFormat {
    public IChannel? X1 { get; }
}