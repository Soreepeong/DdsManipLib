#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public abstract class R16G16PixelFormat : RawRgPixelFormat, IRawRgPixelFormat {
    public const int OffsetR = 0;
    public const int OffsetG = 2;

    public override int BitsPerPixel => 32;
    public override int BytesPerPixel => 4;
}
