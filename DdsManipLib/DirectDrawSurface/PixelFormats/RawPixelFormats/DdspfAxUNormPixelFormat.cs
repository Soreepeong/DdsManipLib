using System;
using System.Numerics;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class DdspfAxUNormPixelFormat<T> : DdspfPixelFormat, IRawAPixelFormat<T>
    where T : unmanaged, IUnsignedNumber<T>, IBinaryInteger<T> {
    internal DdspfAxUNormPixelFormat(int nbits, int rshift, int rbits) : base(nbits, rshift, rbits, 0, 0, 0, 0, 0, 0) { }

    public float GetAlpha(ReadOnlySpan<byte> pixel) => float.CreateSaturating(GetAlphaTyped(pixel)) / AlphaMax;
    public void SetAlpha(Span<byte> pixel, float value) => SetAlpha(pixel, T.CreateSaturating(value * AlphaMax));
    public T GetAlphaTyped(ReadOnlySpan<byte> pixel) => T.CreateSaturating((GetRaw(pixel) >> AlphaShift) & AlphaMax);

    public void SetAlpha(Span<byte> pixel, T value) =>
        SetRaw(pixel, GetRaw(pixel) & ~(AlphaMax << AlphaShift) | (Math.Clamp(uint.CreateSaturating(value), 0, AlphaMax) << AlphaShift));

    public override void ClearPixel(Span<byte> pixel) => SetRaw(pixel, AlphaMax << AlphaShift);
}
