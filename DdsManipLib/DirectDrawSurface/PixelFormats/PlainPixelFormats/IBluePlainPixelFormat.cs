using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.PlainPixelFormats;

public interface IBluePlainPixelFormat : IPlainPixelFormat {
    public IChannel? Blue { get; }
}