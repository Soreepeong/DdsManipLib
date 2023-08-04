using System;
using System.Numerics;
using DdsManipLib.Utilities;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public interface IRawLaPixelFormat : IRawLPixelFormat, IRawAPixelFormat {
    public Vector2 GetLa(ReadOnlySpan<byte> span) => new(GetLuminance(span), GetAlpha(span));

    public void SetLa(Span<byte> span, Vector2 la) {
        SetLuminance(span, la.X);
        SetAlpha(span, la.Y);
    }
}

public interface IRawLaPixelFormat<T> : IRawLaPixelFormat, IRawLPixelFormat<T>, IRawAPixelFormat<T>
    where T : unmanaged, IBinaryNumber<T> { 
    public Vector2<T> GetLaTyped(ReadOnlySpan<byte> span) => new(GetLuminanceTyped(span), GetAlphaTyped(span));

    public void SetLa(Span<byte> span, Vector2<T> la) {
        SetLuminance(span, la.X);
        SetAlpha(span, la.Y);
    }
}
