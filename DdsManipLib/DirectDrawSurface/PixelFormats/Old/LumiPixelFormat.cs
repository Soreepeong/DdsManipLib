using System;
using System.Linq;
using DdsManipLib.BcCodec;
using DdsManipLib.BcCodec.SquishInternal;
using DdsManipLib.DirectDrawSurface.PixelFormats.Old.Channels;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.Old;

/// <summary>
/// Represent a pixel format containing luminous values instead of colors without compression.
/// </summary>
public class LumiPixelFormat : PixelFormat, IEquatable<LumiPixelFormat> {
    /// <summary>
    /// Number of bits for the luminousity channel. 
    /// </summary>
    public readonly ChannelDefinition L;

    /// <summary>
    /// Number of bits for the alpha channel.
    /// </summary>
    public readonly ChannelDefinition A;

    /// <summary>
    /// Number of bits for the extra channel.
    /// </summary>
    public readonly ChannelDefinition X;

    /// <summary>
    /// Construct a new instance of the class.
    /// </summary>
    /// <param name="alphaType"></param>
    /// <param name="l">Number of bits for the luminousity channel.</param>
    /// <param name="a">Number of bits for the alpha channel.</param>
    /// <param name="x">Number of bits for the extra channel.</param>
    public LumiPixelFormat(
        AlphaType alphaType,
        ChannelDefinition? l = null,
        ChannelDefinition? a = null,
        ChannelDefinition? x = null) {
        L = l ?? new();
        A = a ?? new();
        X = x ?? new();
        Alpha = alphaType;

        Bpp = new[] {L.Bits + L.Shift, A.Bits + A.Shift, X.Bits + X.Shift}.Max();
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
    /// <param name="useX2">Use X2 instead of X1, if X channel exists in the pixel format.</param>
    public void ConvertToRgba(
        RgbaPixelFormat targetPixelFormat,
        Span<byte> target,
        int targetStride,
        ReadOnlySpan<byte> source,
        int sourceStride,
        int width,
        int height,
        bool useX2 = false) {
        var channelX = useX2 ? targetPixelFormat.X2 : targetPixelFormat.X1;
        for (var y = 0; y < height; y++) {
            var sourceRow = source[(y * sourceStride)..];
            var targetRow = target[(y * targetStride)..];
            for (var x = 0; x < width; x++) {
                var sourceOffset = Bpp * x;
                var targetOffset = targetPixelFormat.Bpp * x;
                var l = L.DecodeFloat(sourceRow, sourceOffset);
                targetPixelFormat.R.EncodeFloat(targetRow, targetOffset, l);
                targetPixelFormat.G.EncodeFloat(targetRow, targetOffset, l);
                targetPixelFormat.B.EncodeFloat(targetRow, targetOffset, l);
                targetPixelFormat.A.EncodeFloat(targetRow, targetOffset, A.DecodeFloat(sourceRow, sourceOffset, 1f));
                channelX.EncodeFloat(targetRow, targetOffset, X.DecodeFloat(sourceRow, sourceOffset));
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
    /// <remarks>
    /// L/A will be mapped to R/G.
    /// </remarks>
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
                    for (var bx = 0; bx < bw; bx++) {
                        var span = source[(sourceStride * (y + by))..];
                        buf[by * 16 + bx * 4 + 0] = L.DecodeByte(span, Bpp * (x + bx));
                        buf[by * 16 + bx * 4 + 1] = A.DecodeByte(span, Bpp * (x + bx), byte.MaxValue);
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
    public void ConvertToLumi(
        LumiPixelFormat targetPixelFormat,
        Span<byte> target,
        int targetStride,
        ReadOnlySpan<byte> source,
        int sourceStride,
        int width,
        int height) {
        for (var y = 0; y < height; y++) {
            var sourceRow = source[(y * sourceStride)..];
            var targetRow = target[(y * targetStride)..];
            for (var x = 0; x < width; x++) {
                var sourceOffset = Bpp * x;
                var targetOffset = targetPixelFormat.Bpp * x;
                targetPixelFormat.L.EncodeFloat(targetRow, targetOffset, L.DecodeFloat(sourceRow, sourceOffset));
                targetPixelFormat.A.EncodeFloat(targetRow, targetOffset, A.DecodeFloat(sourceRow, sourceOffset, 1f));
                targetPixelFormat.X.EncodeFloat(targetRow, targetOffset, X.DecodeFloat(sourceRow, sourceOffset));
            }
        }
    }

    /// <inheritdoc/>
    public bool Equals(LumiPixelFormat? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return L.Equals(other.L) && A.Equals(other.A) && X.Equals(other.X) && Alpha == other.Alpha;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as LumiPixelFormat);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(L, A, X, (int) Alpha);

    /// <summary>
    /// Create a new instance of <see cref="LumiPixelFormat"/>.
    /// </summary>
    public static LumiPixelFormat NewL(int lbits, int xbits, ChannelType type) =>
        new(
            alphaType: AlphaType.None,
            l: new(type, 0, lbits),
            x: new(ChannelType.Typeless, lbits, xbits));

    /// <summary>
    /// Create a new instance of <see cref="LumiPixelFormat"/>.
    /// </summary>
    public static LumiPixelFormat NewLa(int lbits, int abits, int xbits, ChannelType type, AlphaType alphaType) =>
        new(
            alphaType: alphaType,
            l: new(type, 0, lbits),
            a: new(type, lbits, abits),
            x: new(ChannelType.Typeless, lbits + abits, xbits));
}
