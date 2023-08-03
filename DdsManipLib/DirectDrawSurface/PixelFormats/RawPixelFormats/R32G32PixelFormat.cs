#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public abstract class R32G32PixelFormat : RawRgPixelFormat, IRawRgPixelFormat {
    public const int OffsetR = 0;
    public const int OffsetG = 4;

    public override int BitsPerPixel => 64;
    public override int BytesPerPixel => 8;
    protected R32G32PixelFormat() : base(AlphaType.None) { }
}
