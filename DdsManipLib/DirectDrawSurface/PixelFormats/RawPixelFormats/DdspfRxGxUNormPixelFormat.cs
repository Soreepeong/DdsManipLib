using System;
using System.Numerics;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class DdspfRxGxUNormPixelFormat<T> : DdspfUNormPixelFormat, IRawRgPixelFormat<T>
    where T : unmanaged, IUnsignedNumber<T>, IBinaryInteger<T> {
    internal DdspfRxGxUNormPixelFormat(int nbits, int rshift, int rbits, int gshift, int gbits) : base(nbits, rshift, rbits, gshift, gbits, 0, 0, 0, 0) { }

    public float GetRed(ReadOnlySpan<byte> pixel) => float.CreateTruncating(GetRedTyped(pixel)) / RedMax;
    public void SetRed(Span<byte> pixel, float value) => SetRed(pixel, T.CreateTruncating(value * RedMax));
    public T GetRedTyped(ReadOnlySpan<byte> pixel) => T.CreateTruncating((GetRaw(pixel) >> RedShift) & RedMax);
    public void SetRed(Span<byte> pixel, T value) =>
        SetRaw(pixel, GetRaw(pixel) & ~(RedMax << RedShift) | (Math.Clamp(uint.CreateTruncating(value), 0, RedMax) << RedShift));
    
    public float GetGreen(ReadOnlySpan<byte> pixel) => float.CreateTruncating(GetGreenTyped(pixel)) / GreenMax;
    public void SetGreen(Span<byte> pixel, float value) => SetGreen(pixel, T.CreateTruncating(value * GreenMax));
    public T GetGreenTyped(ReadOnlySpan<byte> pixel) => T.CreateTruncating((GetRaw(pixel) >> GreenShift) & GreenMax);
    public void SetGreen(Span<byte> pixel, T value) =>
        SetRaw(pixel, GetRaw(pixel) & ~(GreenMax << GreenShift) | (Math.Clamp(uint.CreateTruncating(value), 0, GreenMax) << GreenShift));
}