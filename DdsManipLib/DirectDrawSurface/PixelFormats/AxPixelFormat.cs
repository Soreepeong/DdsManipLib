using System;
using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public class AxPixelFormat : RawPixelFormat, IAlphaPixelFormat, IX1PixelFormat {
    public AxPixelFormat(IChannel alpha, IChannel? x1, AlphaType alphaType) {
        Alpha = alpha;
        AlphaType = alphaType;
        X1 = x1;
    }

    public IChannel Alpha { get; }
    public IChannel? X1 { get; }
    public AlphaType AlphaType { get; }
    public override int Bpp => Alpha.BitCount;

    public override bool Equals(PixelFormat? other) =>
        GetType() == other?.GetType()
        && other is AxPixelFormat r
        && Alpha.Equals(r.Alpha)
        && AlphaType == r.AlphaType;

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Alpha.GetHashCode(), Bpp);

    public static AxPixelFormat FromAMask(int nbits, uint am, AlphaType alphaType = AlphaType.Straight) {
        if (nbits is < 0 or > 32)
            throw new ArgumentOutOfRangeException(nameof(nbits), nbits, null);

        var xm = ((1u << nbits) - 1u) & ~am;
        return new(
            alpha: UNormChannel.FromMask(am) ?? throw new ArgumentOutOfRangeException(nameof(am), am, null),
            x1: TypelessChannel.FromMask(xm),
            alphaType: am == 0u ? AlphaType.None : alphaType);
    }

    public static class Presets {
        public class A<T> : AxPixelFormat
            where T : unmanaged, IChannel {
            public A(int alpha, AlphaType alphaType) : base(
                Activator.CreateInstance(typeof(T), 0, alpha) as IChannel ?? throw new ArgumentOutOfRangeException(nameof(alpha), alpha, null),
                null,
                alphaType) { }
        }
    }
}
