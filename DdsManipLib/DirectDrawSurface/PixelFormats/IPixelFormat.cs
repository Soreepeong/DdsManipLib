namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public interface IPixelFormat {
    public int Bpp { get; }
    public int CalculatePitch(int width);
    public int CalculateLinearSize(int width, int height);
}
