namespace DdsManipLib.DirectDrawSurface.PixelFormats.PlainPixelFormats;

public abstract class PlainPixelFormat : PixelFormat {
    public override int CalculatePitch(int width) => (Bpp * width + 7) / 8;
    public override int CalculateLinearSize(int width, int height) => (Bpp * width + 7) / 8 * height;
}