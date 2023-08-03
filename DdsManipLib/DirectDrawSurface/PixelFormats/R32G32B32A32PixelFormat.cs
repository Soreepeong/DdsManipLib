#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public abstract class R32G32B32A32PixelFormat : RawRgbaPixelFormat, IRawAlphaPixelFormat {
    public const int OffsetR = 0;
    public const int OffsetG = 4;
    public const int OffsetB = 8;
    public const int OffsetA = 12;
    public override int BitsPerPixel => 128;
    public override int BytesPerPixel => 16;
    public override AlphaType AlphaType => AlphaType.Straight;
}
