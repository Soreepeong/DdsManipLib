using System;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats; 

public interface IRawLPixelFormat : IRawPixelFormat{
    public float GetLuminance(ReadOnlySpan<byte> pixel);
    public void SetLuminance(Span<byte> pixel, float value);
}

public interface IRawLPixelFormat<T> : IRawPixelFormat<T>, IRawLPixelFormat where T : unmanaged {
    public T GetLuminanceTyped(ReadOnlySpan<byte> pixel);
    public void SetLuminance(Span<byte> pixel, T value);
}
