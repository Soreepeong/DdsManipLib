using System;
using System.Numerics;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class DdspfRxUNormPixelFormat<T> : DdspfPixelFormat, IRawRPixelFormat<T>
    where T : unmanaged, IUnsignedNumber<T>, IBinaryInteger<T>, IMinMaxValue<T> {
    internal DdspfRxUNormPixelFormat(int nbits, int rshift, int rbits) : base(nbits, rshift, rbits, 0, 0, 0, 0, 0, 0) { }

    public float GetRed(ReadOnlySpan<byte> pixel) => GetFloatFromUNorm<T>(pixel, RedShift, RedBits);
    public void SetRed(Span<byte> pixel, float value) => UpdateUNorm<T>(pixel, RedShift, RedBits, value);
    public T GetRedTyped(ReadOnlySpan<byte> pixel) => GetUNorm<T>(pixel, RedShift, RedBits);
    public void SetRed(Span<byte> pixel, T value) => UpdateUNorm(pixel, RedShift, RedBits, value);

    public override void ClearPixel(Span<byte> pixel) => SetRaw(pixel, 0u);
}
