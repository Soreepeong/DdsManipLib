namespace DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

public interface IFloatChannel : IChannel<float> {
    public bool HasSignBit { get; }
    public int ExponentBitCount { get; }
    public int MantissaBitCount { get; }
}