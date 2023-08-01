using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.PlainPixelFormats;

public interface IAlphaPlainPixelFormat : IPlainPixelFormat {
    public IChannel? Alpha { get; }
    public AlphaType AlphaType { get; }
}