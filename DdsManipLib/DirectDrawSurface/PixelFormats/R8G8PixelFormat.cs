#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public abstract class R8G8PixelFormat : RawRgPixelFormat, IRawRgPixelFormat {
    public const int OffsetR = 0;
    public const int OffsetG = 1;

    public override int BitsPerPixel => 16;
    public override int BytesPerPixel => 2;
}
