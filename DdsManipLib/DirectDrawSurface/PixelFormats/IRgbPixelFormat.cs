using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public interface IRgbPixelFormat : IPixelFormat {
    public IChannel? Red { get; }
    public IChannel? Green { get; }
    public IChannel? Blue { get; }
}