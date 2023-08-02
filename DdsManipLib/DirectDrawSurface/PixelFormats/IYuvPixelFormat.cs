using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public interface IYuvPixelFormat : ILuminancePixelFormat {
    public IChannel? ChromaBlue { get; }
    public IChannel? ChromaRed { get; }
}