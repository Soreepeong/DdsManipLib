using System;
using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public abstract class BlockCompressionPixelFormat : PixelFormat {
    protected BlockCompressionPixelFormat(int version, int blockByteCount, int bpp, RgbaxxPixelFormat pixelFormat) {
        AlphaType = pixelFormat is IAlphaPixelFormat iacs ? iacs.AlphaType : AlphaType.None;
        Version = version;
        BlockByteCount = blockByteCount;
        Bpp = bpp;
        DecompressedPixelFormat = pixelFormat;
    }

    public AlphaType AlphaType { get; }
    public int Version { get; }
    public int BlockByteCount { get; }
    public RgbaxxPixelFormat DecompressedPixelFormat { get; }
    public override int Bpp { get; }

    /// <inheritdoc/>
    public override int CalculatePitch(int width) => Math.Max(1, (width + 3) / 4) * BlockByteCount / 2;

    /// <inheritdoc/>
    public override int CalculateLinearSize(int width, int height) =>
        Math.Max(1, (width + 3) / 4) * Math.Max(1, (height + 3) / 4) * BlockByteCount;

    public override bool Equals(PixelFormat? other) =>
        GetType() == other?.GetType()
        && other is BlockCompressionPixelFormat r
        && AlphaType == r.AlphaType
        && Version == r.Version
        && BlockByteCount == r.BlockByteCount
        && DecompressedPixelFormat.Equals(r.DecompressedPixelFormat)
        && Bpp == r.Bpp;

    public override int GetHashCode() => HashCode.Combine(
        base.GetHashCode(),
        AlphaType.GetHashCode(),
        Version.GetHashCode(),
        BlockByteCount.GetHashCode(),
        DecompressedPixelFormat.GetHashCode(),
        Bpp.GetHashCode());

    public static class Presets {
        public class R<T>
            : BlockCompressionPixelFormat
            where T : unmanaged, IChannel {
            public R(int version, int blockByteCount, int bpp, int red)
                : base(version, blockByteCount, bpp, new RgbaxxPixelFormat.Presets.R<T>(red)) { }
        }
        public class RgAttribute<T>
            : BlockCompressionPixelFormat
            where T : unmanaged, IChannel {
            public RgAttribute(int version, int blockByteCount, int bpp, int red, int green)
                : base(version, blockByteCount, bpp, new RgbaxxPixelFormat.Presets.Rg<T>(red, green)) { }
        }
        public class Rgba<T>
            : BlockCompressionPixelFormat
            where T : unmanaged, IChannel {
            public Rgba(int version, int blockByteCount, int bpp, int red, int green, int blue, int alpha, AlphaType alphaType)
                : base(version, blockByteCount, bpp, new RgbaxxPixelFormat.Presets.Rgba<T>(red, green, blue, alpha, alphaType)) { }
        }
        public class RgbAttribute<T>
            : BlockCompressionPixelFormat
            where T : unmanaged, IChannel {
            public RgbAttribute(int version, int blockByteCount, int bpp, int red, int green, int blue)
                : base(version, blockByteCount, bpp, new RgbaxxPixelFormat.Presets.Rgb<T>(red, green, blue)) { }
        }
    }
}