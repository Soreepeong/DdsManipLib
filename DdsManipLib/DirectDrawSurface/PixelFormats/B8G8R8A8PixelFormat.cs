#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public abstract class B8G8R8A8PixelFormat : RawRgbaPixelFormat, IRawRgbaPixelFormat {
    public const int OffsetB = 0;
    public const int OffsetG = 1;
    public const int OffsetR = 2;
    public const int OffsetA = 3;

    public override int BitsPerPixel => 32;
    public override int BytesPerPixel => 4;
    public override AlphaType AlphaType => AlphaType.Straight;
}
