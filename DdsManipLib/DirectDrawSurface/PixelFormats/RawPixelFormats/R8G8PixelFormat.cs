#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public abstract class R8G8PixelFormat : RawRgPixelFormat, IRawRgAlignedBytePixelFormat {
    public const int OffsetR = 0;
    public const int OffsetG = 1;

    public override int BitsPerPixel => 16;
    public override int BytesPerPixel => 2;
    protected R8G8PixelFormat() : base(AlphaType.None) { }

    int IRawRAlignedBytePixelFormat.OffsetR => OffsetR;
    int IRawRgAlignedBytePixelFormat.OffsetG => OffsetG;
}
