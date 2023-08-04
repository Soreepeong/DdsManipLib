#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public interface IRawPixelFormat : IPixelFormat {
    public int BytesPerPixel { get; }
}

public interface IRawPixelFormat<T> : IRawPixelFormat { }