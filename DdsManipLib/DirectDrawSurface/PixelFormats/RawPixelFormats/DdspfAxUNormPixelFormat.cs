using System;
using System.Numerics;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class DdspfAxUNormPixelFormat<T> : DdspfUNormPixelFormat, IRawAPixelFormat<T>
    where T : unmanaged, IUnsignedNumber<T>, IBinaryInteger<T> {
    internal DdspfAxUNormPixelFormat(int nbits, int rshift, int rbits) : base(nbits, rshift, rbits, 0, 0, 0, 0, 0, 0) { }

    public float GetAlpha(ReadOnlySpan<byte> pixel) => float.CreateTruncating(GetAlphaTyped(pixel)) / AlphaMax;
    public void SetAlpha(Span<byte> pixel, float value) => SetAlpha(pixel, T.CreateTruncating(value * AlphaMax));
    public T GetAlphaTyped(ReadOnlySpan<byte> pixel) => T.CreateTruncating((GetRaw(pixel) >> AlphaShift) & AlphaMax);
    public void SetAlpha(Span<byte> pixel, T value) =>
        SetRaw(pixel, GetRaw(pixel) & ~(AlphaMax << AlphaShift) | (Math.Clamp(uint.CreateTruncating(value), 0, AlphaMax) << AlphaShift));
}