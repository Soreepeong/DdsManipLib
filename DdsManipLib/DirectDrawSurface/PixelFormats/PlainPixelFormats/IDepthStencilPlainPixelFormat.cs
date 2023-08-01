using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.PlainPixelFormats;

public interface IDepthStencilPlainPixelFormat : IDepthPlainPixelFormat {
    public IChannel? Stencil { get; }
}