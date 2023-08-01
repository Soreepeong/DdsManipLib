using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.PlainPixelFormats;

public interface ILuminancePlainPixelFormat : IPlainPixelFormat {
    public IChannel? Luminance { get; }
}