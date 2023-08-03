#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public abstract class R32G32B32A32PixelFormat : RawRgbaPixelFormat, IRawAPixelFormat {
    public const int OffsetR = 0;
    public const int OffsetG = 4;
    public const int OffsetB = 8;
    public const int OffsetA = 12;
    public override int BitsPerPixel => 128;
    public override int BytesPerPixel => 16;
    protected R32G32B32A32PixelFormat(AlphaType alphaType) : base(alphaType) { }
}
