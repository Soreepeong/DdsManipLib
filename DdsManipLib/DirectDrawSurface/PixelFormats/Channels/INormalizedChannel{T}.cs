namespace DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

public interface INormalizedChannel<T> : INormalizedChannel, IChannel<T> {
    public float ToNormalizedValue(T value);
    public T FromNormalizedValue(float value);
}
