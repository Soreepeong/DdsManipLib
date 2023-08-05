using System;
using System.Numerics;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class DdspfGxUNormPixelFormat<T> : DdspfPixelFormat, IRawGPixelFormat<T>
    where T : unmanaged, IUnsignedNumber<T>, IBinaryInteger<T> {
    internal DdspfGxUNormPixelFormat(int nbits, int gshift, int gbits) : base(nbits, 0, 0, gshift, gbits, 0, 0, 0, 0) { }
    
    public float GetGreen(ReadOnlySpan<byte> pixel) => float.CreateSaturating(GetGreenTyped(pixel)) / GreenMax;
    public void SetGreen(Span<byte> pixel, float value) => SetGreen(pixel, T.CreateSaturating(value * GreenMax));
    public T GetGreenTyped(ReadOnlySpan<byte> pixel) => T.CreateSaturating((GetRaw(pixel) >> GreenShift) & GreenMax);

    public void SetGreen(Span<byte> pixel, T value) =>
        SetRaw(pixel, GetRaw(pixel) & ~(GreenMax << GreenShift) | (Math.Clamp(uint.CreateSaturating(value), 0, GreenMax) << GreenShift));

    public override void ClearPixel(Span<byte> pixel) => SetRaw(pixel, 0u);
}
