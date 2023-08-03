#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public abstract class R16G16B16A16PixelFormat : RawRgbaPixelFormat, IRawRgbaPixelFormat {
    public const int OffsetR = 0;
    public const int OffsetG = 2;
    public const int OffsetB = 4;
    public const int OffsetA = 6;

    public override int BitsPerPixel => 64;
    public override int BytesPerPixel => 8;
    public override AlphaType AlphaType => AlphaType.Straight;
}
