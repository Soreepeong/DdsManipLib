using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.PlainPixelFormats;

public interface IGreenPlainPixelFormat : IPlainPixelFormat {
    public IChannel? Green { get; }
}