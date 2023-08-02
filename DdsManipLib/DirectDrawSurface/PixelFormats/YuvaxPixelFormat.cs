using System;
using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public abstract class YuvaxPixelFormat : RawPixelFormat, IAlphaPixelFormat, IX1PixelFormat {
    protected YuvaxPixelFormat(
        IChannel luminance,
        IChannel chromaBlue,
        IChannel chromaRed,
        IChannel? x1 = null,
        IChannel? alpha = null,
        AlphaType alphaType = AlphaType.None) {
        Luminance = luminance;
        ChromaBlue = chromaBlue;
        ChromaRed = chromaRed;
        X1 = x1;
        Alpha = alpha;
        AlphaType = alphaType;
    }

    public static YuvaxPixelFormat FromYuvaMask(int nbits, uint ym, uint um, uint vm, uint am, AlphaType alphaType = AlphaType.Straight) {
        throw new NotImplementedException();
    }

    public override int Bpp => throw new NotImplementedException();
    public override bool Equals(PixelFormat? other) => throw new NotImplementedException();

    public IChannel Luminance { get; }
    public IChannel ChromaBlue { get; }
    public IChannel ChromaRed { get; }
    public IChannel? X1 { get; }
    public IChannel? Alpha { get; }
    public AlphaType AlphaType { get; }
}

// see
// https://learn.microsoft.com/en-us/windows/win32/medfound/recommended-8-bit-yuv-formats-for-video-rendering
// https://learn.microsoft.com/en-us/windows/win32/medfound/10-bit-and-16-bit-yuv-video-formats
