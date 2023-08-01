using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.PlainPixelFormats;

public interface IYuvPlainPixelFormat : ILuminancePlainPixelFormat {
    public IChannel? ChromaBlue { get; }
    public IChannel? ChromaRed { get; }
}