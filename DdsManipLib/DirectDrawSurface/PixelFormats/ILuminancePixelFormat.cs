using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public interface ILuminancePixelFormat : IPixelFormat {
    public IChannel? Luminance { get; }
}