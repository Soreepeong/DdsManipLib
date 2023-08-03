#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public abstract class B8G8R8X8PixelFormat : RawRgbPixelFormat, IRawRgbPixelFormat {
    public const int OffsetB = 0;
    public const int OffsetG = 1;
    public const int OffsetR = 2;

    public override int BitsPerPixel => 32;
    public override int BytesPerPixel => 4;
}
