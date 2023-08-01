using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.PlainPixelFormats;

public interface IRedPlainPixelFormat : IPlainPixelFormat {
    public IChannel? Red { get; }
}