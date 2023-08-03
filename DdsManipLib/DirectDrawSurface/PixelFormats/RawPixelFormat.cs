#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public abstract class RawPixelFormat : IRawPixelFormat {
    public abstract DxgiFormat DxgiFormat { get; }
    public virtual DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
    public abstract int BitsPerPixel { get; }
    public abstract int BytesPerPixel { get; }
    public int CalculatePitch(int width) => (BitsPerPixel * width + 7) / 8;
    public int CalculateLinearSize(int width, int height) => (BitsPerPixel * width + 7) / 8 * height;
}
