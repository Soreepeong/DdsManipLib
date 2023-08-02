using System;
using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public class LaxPixelFormat : RawPixelFormat, IAlphaPixelFormat, IX1PixelFormat {
    public LaxPixelFormat(IChannel luminance, IChannel? alpha = null, IChannel? x1 = null, AlphaType alphaType = AlphaType.None) {
        Luminance = luminance;
        Alpha = alpha;
        AlphaType = alphaType;
        X1 = x1;
    }

    public IChannel Luminance { get; }
    public IChannel? Alpha { get; }
    public IChannel? X1 { get; }
    public AlphaType AlphaType { get; }

    /// <inheritdoc/>
    public override int Bpp => Luminance.BitCount + (Alpha?.BitCount ?? 0) + (X1?.BitCount ?? 0);

    /// <inheritdoc/>
    public override bool Equals(PixelFormat? other)
        => other is LaxPixelFormat r
            && Equals(Luminance, r.Luminance)
            && Equals(Alpha, r.Alpha)
            && Equals(X1, r.X1)
            && Equals(AlphaType, r.AlphaType);

    public static LaxPixelFormat FromLaMask(int nbits, uint lm, uint am, AlphaType alphaType = AlphaType.Straight) {
        if (nbits is < 0 or > 32)
            throw new ArgumentOutOfRangeException(nameof(nbits), nbits, null);

        var xm = ((1u << nbits) - 1u) & ~(am | lm);
        return new(
            luminance: UNormChannel.FromMask(lm) ?? throw new ArgumentOutOfRangeException(nameof(lm), lm, null),
            alpha: UNormChannel.FromMask(am),
            alphaType: am == 0u ? AlphaType.None : alphaType,
            x1: TypelessChannel.FromMask(xm));
    }
}
