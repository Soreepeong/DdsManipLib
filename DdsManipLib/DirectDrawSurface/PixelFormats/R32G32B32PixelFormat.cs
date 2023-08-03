#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public abstract class R32G32B32PixelFormat : RawRgbPixelFormat, IRawRgbPixelFormat {
    public const int OffsetR = 0;
    public const int OffsetG = 4;
    public const int OffsetB = 8;
    public override int BitsPerPixel => 96;
    public override int BytesPerPixel => 12;
}
