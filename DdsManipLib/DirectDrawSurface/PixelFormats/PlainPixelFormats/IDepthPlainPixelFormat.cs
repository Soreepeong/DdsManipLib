using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.PlainPixelFormats;

public interface IDepthPlainPixelFormat : IPlainPixelFormat {
    public IChannel? Depth { get; }
}