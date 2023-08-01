using System;
using System.Linq;
using DdsManipLib.BcCodec;
using DdsManipLib.BcCodec.SquishInternal;
using DdsManipLib.DirectDrawSurface.PixelFormats.Old.Channels;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.Old;

/// <summary>
/// Represent a pixel format containing color values without compression. 
/// </summary>
public class RgbaPixelFormat : PixelFormat, IEquatable<RgbaPixelFormat> {
    /// <summary>
    /// Number of bits for the red channel.
    /// </summary>
    public readonly ChannelDefinition R;

    /// <summary>
    /// Number of bits for the green channel.
    /// </summary>
    public readonly ChannelDefinition G;

    /// <summary>
    /// Number of bits for the blue channel.
    /// </summary>
    public readonly ChannelDefinition B;

    /// <summary>
    /// Number of bits for the alpha channel.
    /// </summary>
    public readonly ChannelDefinition A;

    /// <summary>
    /// Number of bits for the extra channel 1.
    /// </summary>
    public readonly ChannelDefinition X1;

    /// <summary>
    /// Number of bits for the extra channel 2.
    /// </summary>
    public readonly ChannelDefinition X2;

    /// <summary>
    /// Construct a new instance of the class.
    ///
    /// Assuming that there are no lol values like r.mask=0b0101 g.mask=0b1010. 
    /// </summary>
    /// <param name="alphaType"></param>
    /// <param name="r">Number of bits for the red channel.</param>
    /// <param name="g">Number of bits for the green channel.</param>
    /// <param name="b">Number of bits for the blue channel.</param>
    /// <param name="a">Number of bits for the alpha channel.</param>
    /// <param name="x1">Number of bits for the extra channel 1.</param>
    /// <param name="x2">Number of bits for the extra channel 2.</param>
    public RgbaPixelFormat(
        AlphaType alphaType,
        ChannelDefinition? r = null,
        ChannelDefinition? g = null,
        ChannelDefinition? b = null,
        ChannelDefinition? a = null,
        ChannelDefinition? x1 = null,
        ChannelDefinition? x2 = null) {
        Alpha = alphaType;
        R = r ?? new();
        G = g ?? new();
        B = b ?? new();
        A = a ?? new();
        X1 = x1 ?? new();
        X2 = x2 ?? new();
        Bpp = new[] {
            R.Bits + R.Shift,
            G.Bits + G.Shift,
            B.Bits + B.Shift,
            A.Bits + A.Shift,
            X1.Bits + X1.Shift,
            X2.Bits + X2.Shift,
        }.Max();
    }

    /// <summary>
    /// Convert across pixel formats.
    /// </summary>
    /// <param name="targetPixelFormat">Target pixel format.</param>
    /// <param name="target">Target byte span.</param>
    /// <param name="targetStride">Number of bytes occupied per horizontal row in the target.</param>
    /// <param name="source">Source byte span.</param>
    /// <param name="sourceStride">Number of bytes occupied per horizontal row in the source.</param>
    /// <param name="width">Width of the source image.</param>
    /// <param name="height">Height of the source image.</param>
    public void ConvertToRgba(
        RgbaPixelFormat targetPixelFormat,
        Span<byte> target,
        int targetStride,
        ReadOnlySpan<byte> source,
        int sourceStride,
        int width,
        int height) {
        var modeR = DetermineConversionMode(R, targetPixelFormat.R);
        var modeG = DetermineConversionMode(G, targetPixelFormat.G);
        var modeB = DetermineConversionMode(B, targetPixelFormat.B);
        var modeA = DetermineConversionMode(A, targetPixelFormat.A);
        var modeX1 = DetermineConversionMode(X1, targetPixelFormat.X1);
        var modeX2 = DetermineConversionMode(X2, targetPixelFormat.X2);
        for (var y = 0; y < height; y++) {
            var sourceRow = source[(y * sourceStride)..];
            var targetRow = target[(y * targetStride)..];
            for (var x = 0; x < width; x++) {
                var sourceOffset = Bpp * x;
                var targetOffset = targetPixelFormat.Bpp * x;
                switch (modeR) {
                    case 0:
                        targetPixelFormat.R.EncodeFloat(targetRow, targetOffset, R.DecodeFloat(sourceRow, sourceOffset));
                        break;
                    case 1:
                        targetPixelFormat.R.EncodeSByte(targetRow, targetOffset, R.DecodeSByte(sourceRow, sourceOffset));
                        break;
                    default:
                        targetPixelFormat.R.EncodeByte(targetRow, targetOffset, R.DecodeByte(sourceRow, sourceOffset));
                        break;
                }

                switch (modeG) {
                    case 0:
                        targetPixelFormat.G.EncodeFloat(targetRow, targetOffset, G.DecodeFloat(sourceRow, sourceOffset));
                        break;
                    case 1:
                        targetPixelFormat.G.EncodeSByte(targetRow, targetOffset, G.DecodeSByte(sourceRow, sourceOffset));
                        break;
                    default:
                        targetPixelFormat.G.EncodeByte(targetRow, targetOffset, G.DecodeByte(sourceRow, sourceOffset));
                        break;
                }

                switch (modeB) {
                    case 0:
                        targetPixelFormat.B.EncodeFloat(targetRow, targetOffset, B.DecodeFloat(sourceRow, sourceOffset));
                        break;
                    case 1:
                        targetPixelFormat.B.EncodeSByte(targetRow, targetOffset, B.DecodeSByte(sourceRow, sourceOffset));
                        break;
                    default:
                        targetPixelFormat.B.EncodeByte(targetRow, targetOffset, B.DecodeByte(sourceRow, sourceOffset));
                        break;
                }

                switch (modeA) {
                    case 0:
                        targetPixelFormat.A.EncodeFloat(targetRow, targetOffset, A.DecodeFloat(sourceRow, sourceOffset, 1f));
                        break;
                    case 1:
                        targetPixelFormat.A.EncodeSByte(targetRow, targetOffset, A.DecodeSByte(sourceRow, sourceOffset, sbyte.MaxValue));
                        break;
                    default:
                        targetPixelFormat.A.EncodeByte(targetRow, targetOffset, A.DecodeByte(sourceRow, sourceOffset, byte.MaxValue));
                        break;
                }

                switch (modeX1) {
                    case 0:
                        targetPixelFormat.X1.EncodeFloat(targetRow, targetOffset, X1.DecodeFloat(sourceRow, sourceOffset));
                        break;
                    case 1:
                        targetPixelFormat.X1.EncodeSByte(targetRow, targetOffset, X1.DecodeSByte(sourceRow, sourceOffset));
                        break;
                    default:
                        targetPixelFormat.X1.EncodeByte(targetRow, targetOffset, X1.DecodeByte(sourceRow, sourceOffset));
                        break;
                }

                switch (modeX2) {
                    case 0:
                        targetPixelFormat.X2.EncodeFloat(targetRow, targetOffset, X2.DecodeFloat(sourceRow, sourceOffset));
                        break;
                    case 1:
                        targetPixelFormat.X2.EncodeSByte(targetRow, targetOffset, X2.DecodeSByte(sourceRow, sourceOffset));
                        break;
                    default:
                        targetPixelFormat.X2.EncodeByte(targetRow, targetOffset, X2.DecodeByte(sourceRow, sourceOffset));
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Convert across pixel formats.
    /// </summary>
    /// <param name="targetPixelFormat">Target pixel format.</param>
    /// <param name="target">Target byte span.</param>
    /// <param name="source">Source byte span.</param>
    /// <param name="sourceStride">Number of bytes occupied per horizontal row in the source.</param>
    /// <param name="width">Width of the source image.</param>
    /// <param name="height">Height of the source image.</param>
    public void ConvertToBc(
        BcPixelFormat targetPixelFormat,
        Span<byte> target,
        ReadOnlySpan<byte> source,
        int sourceStride,
        int width,
        int height) {
        var options = new SquishOptions2(targetPixelFormat.SquishMethod) {ChannelOffsets = SquishOptions2.OffsetRgba};
        var comp = new BlockCompresser(options);

        Span<byte> buf = stackalloc byte[64];
        for (var y = 0; y < height; y += 4) {
            var bh = Math.Min(4, height - y);
            for (var x = 0; x < width; x += 4) {
                var bw = Math.Min(4, width - x);

                var mask = 0;
                for (var by = 0; by < bh; by++) {
                    var span = source[(sourceStride * (y + by))..];
                    for (var bx = 0; bx < bw; bx++) {
                        buf[by * 16 + bx * 4 + 0] = R.DecodeByte(span, Bpp * (x + bx));
                        buf[by * 16 + bx * 4 + 1] = G.DecodeByte(span, Bpp * (x + bx));
                        buf[by * 16 + bx * 4 + 2] = B.DecodeByte(span, Bpp * (x + bx));
                        buf[by * 16 + bx * 4 + 3] = A.DecodeByte(span, Bpp * (x + bx), byte.MaxValue);
                        mask |= 1 << (by * 4 + bx);
                    }
                }

                comp.RemapChannelsFromMasked(buf, mask);
                comp.CompressMaskedInto(target);
                target = target[targetPixelFormat.BlockSize..];
            }
        }
    }

    /// <summary>
    /// Convert across pixel formats.
    /// </summary>
    /// <param name="targetPixelFormat">Target pixel format.</param>
    /// <param name="target">Target byte span.</param>
    /// <param name="targetStride">Number of bytes occupied per horizontal row in the target.</param>
    /// <param name="source">Source byte span.</param>
    /// <param name="sourceStride">Number of bytes occupied per horizontal row in the source.</param>
    /// <param name="width">Width of the source image.</param>
    /// <param name="height">Height of the source image.</param>
    /// <param name="redWeight">Weight of red channel for desaturation.</param>
    /// <param name="greenWeight">Weight of green channel for desaturation.</param>
    /// <param name="blueWeight">Weight of blue channel for desaturation.</param>
    /// <param name="useX2">Use X2 instead of X1, if X channel exists in the target pixel format.</param>
    public void ConvertToLumi(
        LumiPixelFormat targetPixelFormat,
        Span<byte> target,
        int targetStride,
        ReadOnlySpan<byte> source,
        int sourceStride,
        int width,
        int height,
        float redWeight = 1,
        float greenWeight = 1,
        float blueWeight = 1,
        bool useX2 = false) {
        var channelX = useX2 ? X2 : X1;
        var modeA = DetermineConversionMode(A, targetPixelFormat.A);
        var modeX = DetermineConversionMode(channelX, targetPixelFormat.X);
        for (var y = 0; y < height; y++) {
            var sourceRow = source[(y * sourceStride)..];
            var targetRow = target[(y * targetStride)..];
            for (var x = 0; x < width; x++) {
                var sourceOffset = Bpp * x;
                var targetOffset = targetPixelFormat.Bpp * x;

                var weightSum = 0f;
                var weightMax = 0f;
                if (!R.IsEmpty) {
                    weightSum += redWeight * R.DecodeNormalizedFloat(sourceRow, sourceOffset);
                    weightMax += redWeight;
                }

                if (!G.IsEmpty) {
                    weightSum += greenWeight * G.DecodeNormalizedFloat(sourceRow, sourceOffset);
                    weightMax += greenWeight;
                }

                if (!B.IsEmpty) {
                    weightSum += blueWeight * B.DecodeNormalizedFloat(sourceRow, sourceOffset);
                    weightMax += blueWeight;
                }

                targetPixelFormat.L.EncodeFloat(targetRow, targetOffset, weightSum / weightMax);

                switch (modeA) {
                    case 0:
                        targetPixelFormat.A.EncodeFloat(targetRow, targetOffset, A.DecodeFloat(sourceRow, sourceOffset, 1f));
                        break;
                    case 1:
                        targetPixelFormat.A.EncodeSByte(targetRow, targetOffset, A.DecodeSByte(sourceRow, sourceOffset, sbyte.MaxValue));
                        break;
                    default:
                        targetPixelFormat.A.EncodeByte(targetRow, targetOffset, A.DecodeByte(sourceRow, sourceOffset, byte.MaxValue));
                        break;
                }

                switch (modeX) {
                    case 0:
                        targetPixelFormat.X.EncodeFloat(targetRow, targetOffset, channelX.DecodeFloat(sourceRow, sourceOffset));
                        break;
                    case 1:
                        targetPixelFormat.X.EncodeSByte(targetRow, targetOffset, channelX.DecodeSByte(sourceRow, sourceOffset));
                        break;
                    default:
                        targetPixelFormat.X.EncodeByte(targetRow, targetOffset, channelX.DecodeByte(sourceRow, sourceOffset));
                        break;
                }
            }
        }
    }

    private static int DetermineConversionMode(in ChannelDefinition cd1, in ChannelDefinition cd2) {
        if (cd1.Type != cd2.Type || cd1.Bits > 8 || cd2.Bits > 8)
            return 0;
        if (cd1.Type.HasFlag(ChannelType.Integer))
            return cd1.Type.HasFlag(ChannelType.Signed) ? 1 : 2;
        return 0;
    }

    /// <inheritdoc/>
    public bool Equals(RgbaPixelFormat? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return R.Equals(other.R) && G.Equals(other.G) && B.Equals(other.B) && A.Equals(other.A) &&
            X1.Equals(other.X1) && X2.Equals(other.X2) && Alpha == other.Alpha;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as RgbaPixelFormat);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(R, G, B, A, X1, X2, (int) Alpha);

    /// <summary>
    /// Create a new instance of <see cref="RgbaPixelFormat"/>.
    /// </summary>
    public static RgbaPixelFormat NewR(int rbits, int xbits1, int xbits2, ChannelType type) => new(
        alphaType: AlphaType.None,
        r: new(type, 0, rbits),
        x1: new(ChannelType.Typeless, rbits, xbits1),
        x2: new(ChannelType.Typeless, rbits + xbits1, xbits2));

    /// <summary>
    /// Create a new instance of <see cref="RgbaPixelFormat"/>.
    /// </summary>
    public static RgbaPixelFormat NewA(int abits, int xbits1, int xbits2, ChannelType type, AlphaType alphaType) =>
        new(
            alphaType: alphaType,
            a: new(type, 0, abits),
            x1: new(ChannelType.Typeless, abits, xbits1),
            x2: new(ChannelType.Typeless, abits + xbits1, xbits2));

    /// <summary>
    /// Create a new instance of <see cref="RgbaPixelFormat"/>.
    /// </summary>
    public static RgbaPixelFormat NewRg(int rbits, int gbits, int xbits1, int xbits2, ChannelType type) => new(
        alphaType: AlphaType.None,
        r: new(type, 0, rbits),
        g: new(type, rbits, gbits),
        x1: new(ChannelType.Typeless, rbits + gbits, xbits1),
        x2: new(ChannelType.Typeless, rbits + gbits + xbits1, xbits2));

    /// <summary>
    /// Create a new instance of <see cref="RgbaPixelFormat"/>.
    /// </summary>
    public static RgbaPixelFormat NewRgb(int rbits, int gbits, int bbits, int xbits1, int xbits2, ChannelType type) =>
        new(
            alphaType: AlphaType.None,
            r: new(type, 0, rbits),
            g: new(type, rbits, gbits),
            b: new(type, rbits + gbits, bbits),
            x1: new(ChannelType.Typeless, rbits + gbits + bbits, xbits1),
            x2: new(ChannelType.Typeless, rbits + gbits + bbits + xbits1, xbits2));

    /// <summary>
    /// Create a new instance of <see cref="RgbaPixelFormat"/>.
    /// </summary>
    public static RgbaPixelFormat NewRgba(
        int rbits,
        int gbits,
        int bbits,
        int abits,
        int xbits1,
        int xbits2,
        ChannelType type,
        AlphaType alphaType) =>
        new(
            alphaType: alphaType,
            r: new(type, 0, rbits),
            g: new(type, rbits, gbits),
            b: new(type, rbits + gbits, bbits),
            a: new(type, rbits + bbits + bbits, abits),
            x1: new(ChannelType.Typeless, rbits + bbits + bbits + abits, xbits1),
            x2: new(ChannelType.Typeless, rbits + bbits + bbits + abits + xbits1, xbits2));

    /// <summary>
    /// Create a new instance of <see cref="RgbaPixelFormat"/>.
    /// </summary>
    public static RgbaPixelFormat NewBgr(int rbits, int gbits, int bbits, int xbits1, int xbits2, ChannelType type) =>
        new(
            alphaType: AlphaType.None,
            b: new(type, 0, bbits),
            g: new(type, bbits, gbits),
            r: new(type, bbits + gbits, rbits),
            x1: new(ChannelType.Typeless, bbits + gbits + rbits, xbits1),
            x2: new(ChannelType.Typeless, bbits + gbits + rbits + xbits1, xbits2));

    /// <summary>
    /// Create a new instance of <see cref="RgbaPixelFormat"/>.
    /// </summary>
    public static RgbaPixelFormat NewBgra(
        int rbits,
        int gbits,
        int bbits,
        int abits,
        int xbits1,
        int xbits2,
        ChannelType type,
        AlphaType alphaType) =>
        new(
            alphaType: alphaType,
            b: new(type, 0, bbits),
            g: new(type, bbits, gbits),
            r: new(type, bbits + gbits, rbits),
            a: new(type, bbits + gbits + rbits, abits, (1u << abits) - 1u),
            x1: new(ChannelType.Typeless, bbits + gbits + rbits + abits, xbits1),
            x2: new(ChannelType.Typeless, bbits + gbits + rbits + abits + xbits1, xbits2));
}
