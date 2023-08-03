#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public abstract class B8G8R8A8PixelFormat : RawRgbaPixelFormat, IRawRgbaAlignedBytePixelFormat {
    public const int OffsetB = 0;
    public const int OffsetG = 1;
    public const int OffsetR = 2;
    public const int OffsetA = 3;

    public override int BitsPerPixel => 32;
    public override int BytesPerPixel => 4;
    protected B8G8R8A8PixelFormat(AlphaType alphaType) : base(alphaType) { }
    
    int IRawRAlignedBytePixelFormat.OffsetR => OffsetR;
    int IRawRgAlignedBytePixelFormat.OffsetG => OffsetG;
    int IRawRgbAlignedBytePixelFormat.OffsetB => OffsetB;
    int IRawRgbaAlignedBytePixelFormat.OffsetA => OffsetA;
}
