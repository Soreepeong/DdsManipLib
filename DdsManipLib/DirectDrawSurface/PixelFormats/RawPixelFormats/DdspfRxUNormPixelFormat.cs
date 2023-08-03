using System;
using System.Numerics;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class DdspfRxUNormPixelFormat<T> : DdspfUNormPixelFormat, IRawRPixelFormat<T>
    where T : unmanaged, IUnsignedNumber<T>, IBinaryInteger<T> {
    internal DdspfRxUNormPixelFormat(int nbits, int rshift, int rbits) : base(nbits, rshift, rbits, 0, 0, 0, 0, 0, 0) { }

    public float GetRed(ReadOnlySpan<byte> pixel) => float.CreateTruncating(GetRedTyped(pixel)) / RedMax;
    public void SetRed(Span<byte> pixel, float value) => SetRed(pixel, T.CreateTruncating(value * RedMax));
    public T GetRedTyped(ReadOnlySpan<byte> pixel) => T.CreateTruncating((GetRaw(pixel) >> RedShift) & RedMax);

    public void SetRed(Span<byte> pixel, T value) =>
        SetRaw(pixel, GetRaw(pixel) & ~(RedMax << RedShift) | (Math.Clamp(uint.CreateTruncating(value), 0, RedMax) << RedShift));
}
