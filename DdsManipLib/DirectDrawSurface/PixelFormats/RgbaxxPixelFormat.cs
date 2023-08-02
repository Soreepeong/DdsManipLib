using System;
using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public class RgbaxxPixelFormat : RawPixelFormat, IRgbPixelFormat, IAlphaPixelFormat, IX2PixelFormat {
    public IChannel? Red { get; }
    public IChannel? Green { get; }
    public IChannel? Blue { get; }
    public IChannel? Alpha { get; }
    public IChannel? X1 { get; }
    public IChannel? X2 { get; }
    public AlphaType AlphaType { get; }

    public RgbaxxPixelFormat(
        IChannel? red = null,
        IChannel? green = null,
        IChannel? blue = null,
        IChannel? alpha = null,
        IChannel? x1 = null,
        IChannel? x2 = null,
        AlphaType alphaType = AlphaType.None) {
        Red = red;
        Green = green;
        Blue = blue;
        Alpha = alpha;
        X1 = x1;
        X2 = x2;
        AlphaType = alphaType;
        Bpp = (Red?.BitCount ?? 0) +
            (Green?.BitCount ?? 0) +
            (Blue?.BitCount ?? 0) +
            (Alpha?.BitCount ?? 0) +
            (X1?.BitCount ?? 0) +
            (X2?.BitCount ?? 0);
    }

    /// <inheritdoc/>
    public override int Bpp { get; }

    /// <inheritdoc/>
    public override bool Equals(PixelFormat? other)
        => other is RgbaxxPixelFormat r
            && Equals(Red, r.Red)
            && Equals(Green, r.Green)
            && Equals(Blue, r.Blue)
            && Equals(Alpha, r.Alpha)
            && Equals(X1, r.X1)
            && Equals(X2, r.X2)
            && Equals(AlphaType, r.AlphaType);

    public void Copy(ReadOnlySpan<byte> source, int width, int height, RgbaxxPixelFormat targetPixelFormat, Span<byte> target) {
        source = source[..CalculateLinearSize(width, height)];
        if (Equals(targetPixelFormat)) {
            source.CopyTo(target);
            return;
        }

        var sourcePitch = CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var rconv = Red?.GetCopyPixelDelegate(targetPixelFormat.Red);
        var gconv = Green?.GetCopyPixelDelegate(targetPixelFormat.Green);
        var bconv = Blue?.GetCopyPixelDelegate(targetPixelFormat.Blue);
        var aconv = Alpha?.GetCopyPixelDelegate(targetPixelFormat.Alpha);
        var x1conv = X1?.GetCopyPixelDelegate(targetPixelFormat.X1);
        var x2conv = X2?.GetCopyPixelDelegate(targetPixelFormat.X2);
        for (var y = 0; y < height; y++) {
            var sourceShift = 0;
            var targetShift = 0;
            for (var x = 0; x < width; x++) {
                rconv?.Invoke(source, sourceShift, target, targetShift);
                gconv?.Invoke(source, sourceShift, target, targetShift);
                bconv?.Invoke(source, sourceShift, target, targetShift);
                aconv?.Invoke(source, sourceShift, target, targetShift);
                x1conv?.Invoke(source, sourceShift, target, targetShift);
                x2conv?.Invoke(source, sourceShift, target, targetShift);
                sourceShift += Bpp;
                targetShift += targetPixelFormat.Bpp;
            }
            source = source[sourcePitch..];
            target = target[targetPitch..];
        }
    }

    public static RgbaxxPixelFormat FromRgbaMask(int nbits, uint rm, uint gm, uint bm, uint am, AlphaType alphaType = AlphaType.Straight) {
        if (nbits is < 0 or > 32)
            throw new ArgumentOutOfRangeException(nameof(nbits), nbits, null);
        
        var xm = ((1u << nbits) - 1u) & ~(am | rm | gm | bm);
        return new(
            red: UNormChannel.FromMask(rm),
            green: UNormChannel.FromMask(gm),
            blue: UNormChannel.FromMask(bm),
            alpha: UNormChannel.FromMask(am),
            alphaType: am == 0u ? AlphaType.None : alphaType,
            x1: TypelessChannel.FromMask(xm));
    }

    public static class Presets {
        public class R<T> : RgbaxxPixelFormat
            where T : unmanaged, IChannel {
            public R(int red) : base(red: Activator.CreateInstance(typeof(T), 0, red) as IChannel) { }
        }
        
        public class Rg<T> : RgbaxxPixelFormat
            where T : unmanaged, IChannel {
            public Rg(int red, int green) : base(
                red: Activator.CreateInstance(typeof(T), 0, red) as IChannel,
                green: Activator.CreateInstance(typeof(T), red, green) as IChannel) { }
        }
        
        public class Rx<T, TX1> : RgbaxxPixelFormat
            where T : unmanaged, IChannel
            where TX1 : unmanaged, IChannel {
            public Rx(int red, int x1) : base(
                red: Activator.CreateInstance(typeof(T), 0, red) as IChannel,
                x1: Activator.CreateInstance(typeof(TX1), red, x1) as IChannel) { }
        }
        
        public class Rxx<T, TX1, TX2> : RgbaxxPixelFormat
            where T : unmanaged, IChannel
            where TX1 : unmanaged, IChannel
            where TX2 : unmanaged, IChannel {
            public Rxx(int red, int x1, int x2) : base(
                red: Activator.CreateInstance(typeof(T), 0, red) as IChannel,
                x1: Activator.CreateInstance(typeof(TX1), red, x1) as IChannel,
                x2: Activator.CreateInstance(typeof(TX2), red, x2) as IChannel) { }
        }
        
        public class Xg<TX1, TG> : RgbaxxPixelFormat
            where TX1 : unmanaged, IChannel
            where TG : unmanaged, IChannel {
            public Xg(int x1, int green) : base(
                x1: Activator.CreateInstance(typeof(TX1), 0, x1) as IChannel,
                green: Activator.CreateInstance(typeof(TG), x1, green) as IChannel) { }
        }

        public class Xgx<TX1, TG, TX2> : RgbaxxPixelFormat
            where TX1 : unmanaged, IChannel
            where TG : unmanaged, IChannel
            where TX2 : unmanaged, IChannel {
            public Xgx(int x1, int green, int x2) : base(
                x1: Activator.CreateInstance(typeof(TX1), 0, x1) as IChannel,
                green: Activator.CreateInstance(typeof(TG), x1, green) as IChannel,
                x2: Activator.CreateInstance(typeof(TX2), x1 + green, x2) as IChannel) { }
        }

        public class Rgb<T> : RgbaxxPixelFormat
            where T : unmanaged, IChannel {
            public Rgb(int red, int green, int blue) : base(
                red: Activator.CreateInstance(typeof(T), 0, red) as IChannel,
                green: Activator.CreateInstance(typeof(T), red, green) as IChannel,
                blue: Activator.CreateInstance(typeof(T), red + green, blue) as IChannel) { }
        }
        
        public class Rgx<T, TX1> : RgbaxxPixelFormat
            where T : unmanaged, IChannel
            where TX1 : unmanaged, IChannel {
            public Rgx(int red, int green, int x1) : base(
                red: Activator.CreateInstance(typeof(T), 0, red) as IChannel,
                green: Activator.CreateInstance(typeof(T), red, green) as IChannel,
                x1: Activator.CreateInstance(typeof(TX1), red + green, x1) as IChannel) { }
        }
        
        public class Rgba<T, TAlpha> : RgbaxxPixelFormat
            where T : unmanaged, IChannel
            where TAlpha : unmanaged, IChannel {
            public Rgba(int red, int green, int blue, int alpha, AlphaType alphaType) : base(
                red: Activator.CreateInstance(typeof(T), 0, red) as IChannel,
                green: Activator.CreateInstance(typeof(T), red, green) as IChannel,
                blue: Activator.CreateInstance(typeof(T), red + green, blue) as IChannel,
                alpha: Activator.CreateInstance(typeof(TAlpha), red + green + blue, alpha) as IChannel,
                alphaType: alphaType) { }
        }
        
        public class Rgba<T> : RgbaxxPixelFormat
            where T : unmanaged, IChannel {
            public Rgba(int red, int green, int blue, int alpha, AlphaType alphaType) : base(
                red: Activator.CreateInstance(typeof(T), 0, red) as IChannel,
                green: Activator.CreateInstance(typeof(T), red, green) as IChannel,
                blue: Activator.CreateInstance(typeof(T), red + green, blue) as IChannel,
                alpha: Activator.CreateInstance(typeof(T), red + green + blue, alpha) as IChannel,
                alphaType: alphaType) { }
        }
        
        public class Abgr<T> : RgbaxxPixelFormat
            where T : unmanaged, IChannel {
            public Abgr(int alpha, AlphaType alphaType, int blue, int green, int red)
                : base(
                    alpha: Activator.CreateInstance(typeof(T), 0, alpha) as IChannel,
                    blue: Activator.CreateInstance(typeof(T), alpha, blue) as IChannel,
                    green: Activator.CreateInstance(typeof(T), alpha + blue, green) as IChannel,
                    red: Activator.CreateInstance(typeof(T), alpha + blue + green, red) as IChannel,
                    alphaType: alphaType) { }
        }

        public class Bgr<T> : RgbaxxPixelFormat
            where T : unmanaged, IChannel {
            public Bgr(int blue, int green, int red) : base(
                blue: Activator.CreateInstance(typeof(T), 0, blue) as IChannel,
                green: Activator.CreateInstance(typeof(T), blue, green) as IChannel,
                red: Activator.CreateInstance(typeof(T), blue + green, red) as IChannel) { }
        }

        public class Bgra<T, TAlpha> : RgbaxxPixelFormat
            where T : unmanaged, IChannel
            where TAlpha : unmanaged, IChannel {
            public Bgra(int blue, int green, int red, int alpha, AlphaType alphaType)
                : base(
                    blue: Activator.CreateInstance(typeof(T), 0, blue) as IChannel,
                    green: Activator.CreateInstance(typeof(T), blue, green) as IChannel,
                    red: Activator.CreateInstance(typeof(T), blue + green, red) as IChannel,
                    alpha: Activator.CreateInstance(typeof(TAlpha), blue + green + red, alpha) as IChannel,
                    alphaType: alphaType) { }
        }

        public class Bgra<T> : RgbaxxPixelFormat
            where T : unmanaged, IChannel {
            public Bgra(int blue, int green, int red, int alpha, AlphaType alphaType)
                : base(
                    blue: Activator.CreateInstance(typeof(T), 0, blue) as IChannel,
                    green: Activator.CreateInstance(typeof(T), blue, green) as IChannel,
                    red: Activator.CreateInstance(typeof(T), blue + green, red) as IChannel,
                    alpha: Activator.CreateInstance(typeof(T), blue + green + red, alpha) as IChannel,
                    alphaType: alphaType) { }
        }

        public class Bgrx<T> : RgbaxxPixelFormat
            where T : unmanaged, IChannel {
            public Bgrx(int blue, int green, int red, int x1)
                : base(
                    blue: Activator.CreateInstance(typeof(T), 0, blue) as IChannel,
                    green: Activator.CreateInstance(typeof(T), blue, green) as IChannel,
                    red: Activator.CreateInstance(typeof(T), blue + green, red) as IChannel,
                    x1: Activator.CreateInstance(typeof(T), blue + green + red, x1) as IChannel) { }
        }
    }
}
