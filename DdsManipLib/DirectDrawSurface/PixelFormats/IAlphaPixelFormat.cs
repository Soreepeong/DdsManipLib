using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public interface IAlphaPixelFormat : IPixelFormat {
    public IChannel? Alpha { get; }
    public AlphaType AlphaType { get; }
}