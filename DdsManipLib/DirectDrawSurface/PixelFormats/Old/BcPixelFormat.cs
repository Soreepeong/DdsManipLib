using System;
using DdsManipLib.BcCodec;
using DdsManipLib.BcCodec.SquishInternal;
using DdsManipLib.DirectDrawSurface.PixelFormats.Old.Channels;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.Old;

/// <summary>
/// Represent a pixel format containing block compressed data. 
/// </summary>
public class BcPixelFormat : PixelFormat, IEquatable<BcPixelFormat> {
    /// <summary>
    /// Channel type of each color channel.
    /// </summary>
    public readonly ChannelType Type;

    /// <summary>
    /// Block compression version.
    /// </summary>
    public readonly byte Version;

    /// <summary>
    /// Construct a new instance of this class.
    /// </summary>
    /// <param name="type">Type of each color channel.</param>
    /// <param name="alpha">Type of alpha channel.</param>
    /// <param name="version">Block compression version.</param>
    public BcPixelFormat(
        ChannelType type = ChannelType.Typeless,
        AlphaType alpha = AlphaType.Straight,
        byte version = 0) {
        if (version is < 1 or > 7)
            throw new ArgumentOutOfRangeException(nameof(version), version, null);

        Type = type;
        Alpha = alpha;
        Version = version;
        Bpp = version is 1 or 4 ? 4 : 8;
    }

    /// <summary>
    /// Number of bytes required to store one block containing 4x4 pixels. 
    /// </summary>
    public int BlockSize => Version is 1 or 4 ? 8 : 16;

    /// <inheritdoc/>
    public override int CalculatePitch(int width) => Math.Max(1, (width + 3) / 4) * BlockSize;

    /// <inheritdoc/>
    public override int CalculateLinearSize(int width, int height) =>
        Math.Max(1, (width + 3) / 4) * Math.Max(1, (height + 3) / 4) * BlockSize;

    /// <summary>
    /// <see cref="SquishMethod"/> corresponding to this <see cref="BcPixelFormat"/>.
    /// </summary>
    public SquishMethod SquishMethod => Version switch {
        1 when Type is ChannelType.Unorm or ChannelType.UnormSrgb or ChannelType.Typeless => SquishMethod.Bc1,
        2 when Type is ChannelType.Unorm or ChannelType.UnormSrgb or ChannelType.Typeless => SquishMethod.Bc2,
        3 when Type is ChannelType.Unorm or ChannelType.UnormSrgb or ChannelType.Typeless => SquishMethod.Bc3,
        4 when Type is ChannelType.Unorm or ChannelType.Typeless => SquishMethod.Bc4U,
        4 when Type is ChannelType.Snorm => SquishMethod.Bc4S,
        5 when Type is ChannelType.Unorm or ChannelType.Typeless => SquishMethod.Bc5U,
        5 when Type is ChannelType.Snorm => SquishMethod.Bc5S,
        7 => SquishMethod.Bc7,
        _ => throw new InvalidOperationException(),
    };

    /// <summary>
    /// Convert across pixel formats.
    /// </summary>
    /// <param name="targetPixelFormat">Target pixel format.</param>
    /// <param name="target">Target byte span.</param>
    /// <param name="targetStride">Number of bytes occupied per horizontal row in the target.</param>
    /// <param name="source">Source byte span.</param>
    /// <param name="width">Width of the source image.</param>
    /// <param name="height">Height of the source image.</param>
    public void ConvertToRgba(
        RgbaPixelFormat targetPixelFormat,
        Span<byte> target,
        int targetStride,
        ReadOnlySpan<byte> source,
        int width,
        int height) {
        var decomp = new BlockDecompresser(new(SquishMethod) {ChannelOffsets = SquishOptions2.OffsetRgba});

        for (var y = 0; y < height; y += 4) {
            var bh = Math.Min(4, height - y);
            for (var x = 0; x < width; x += 4) {
                var bw = Math.Min(4, width - x);
                decomp.DecompressFrom(source);
                for (var by = 0; by < bh; by++) {
                    for (var bx = 0; bx < bw; bx++) {
                        var pixel = decomp[by, bx];
                        
                        if (targetPixelFormat.R.Type is not (ChannelType.F32 or ChannelType.Sf16 or ChannelType.Uf16))
                            targetPixelFormat.R.EncodeByte(target[(targetStride * (y + by))..], targetPixelFormat.Bpp * (x + bx), pixel.X);
                        else if (Type == ChannelType.Snorm)
                            targetPixelFormat.R.EncodeFloat(target[(targetStride * (y + by))..], targetPixelFormat.Bpp * (x + bx), (sbyte) pixel.X / 127f);
                        else
                            targetPixelFormat.R.EncodeFloat(target[(targetStride * (y + by))..], targetPixelFormat.Bpp * (x + bx), pixel.X / 255f);

                        if (targetPixelFormat.G.Type is not (ChannelType.F32 or ChannelType.Sf16 or ChannelType.Uf16))
                            targetPixelFormat.G.EncodeByte(target[(targetStride * (y + by))..], targetPixelFormat.Bpp * (x + bx), pixel.Y);
                        else if (Type == ChannelType.Snorm)
                            targetPixelFormat.G.EncodeFloat(target[(targetStride * (y + by))..], targetPixelFormat.Bpp * (x + bx), (sbyte) pixel.Y / 127f);
                        else
                            targetPixelFormat.G.EncodeFloat(target[(targetStride * (y + by))..], targetPixelFormat.Bpp * (x + bx), pixel.Y / 255f);

                        if (targetPixelFormat.B.Type is not (ChannelType.F32 or ChannelType.Sf16 or ChannelType.Uf16))
                            targetPixelFormat.B.EncodeByte(target[(targetStride * (y + by))..], targetPixelFormat.Bpp * (x + bx), pixel.Z);
                        else if (Type == ChannelType.Snorm)
                            targetPixelFormat.B.EncodeFloat(target[(targetStride * (y + by))..], targetPixelFormat.Bpp * (x + bx), (sbyte) pixel.Z / 127f);
                        else
                            targetPixelFormat.B.EncodeFloat(target[(targetStride * (y + by))..], targetPixelFormat.Bpp * (x + bx), pixel.Z / 255f);

                        if (targetPixelFormat.A.Type is not (ChannelType.F32 or ChannelType.Sf16 or ChannelType.Uf16))
                            targetPixelFormat.A.EncodeByte(target[(targetStride * (y + by))..], targetPixelFormat.Bpp * (x + bx), pixel.W);
                        else if (Type == ChannelType.Snorm)
                            targetPixelFormat.A.EncodeFloat(target[(targetStride * (y + by))..], targetPixelFormat.Bpp * (x + bx), (sbyte) pixel.W / 127f);
                        else
                            targetPixelFormat.A.EncodeFloat(target[(targetStride * (y + by))..], targetPixelFormat.Bpp * (x + bx), pixel.W / 255f);
                    }
                }

                source = source[BlockSize..];
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
    /// <param name="width">Width of the source image.</param>
    /// <param name="height">Height of the source image.</param>
    /// <remarks>
    /// R/G will be mapped to L/A.
    /// </remarks>
    public void ConvertToLumi(
        LumiPixelFormat targetPixelFormat,
        Span<byte> target,
        int targetStride,
        ReadOnlySpan<byte> source,
        int width,
        int height) {
        var decomp = new BlockDecompresser(new(SquishMethod) {ChannelOffsets = SquishOptions2.OffsetRgba});

        for (var y = 0; y < height; y += 4) {
            var bh = Math.Min(4, height - y);
            for (var x = 0; x < width; x += 4) {
                var bw = Math.Min(4, width - x);
                decomp.DecompressFrom(source);
                for (var by = 0; by < bh; by++) {
                    for (var bx = 0; bx < bw; bx++) {
                        var pixel = decomp[by, bx];
                        if (targetPixelFormat.L.Type is not (ChannelType.F32 or ChannelType.Sf16 or ChannelType.Uf16))
                            targetPixelFormat.L.EncodeByte(target[(targetStride * (y + by))..], targetPixelFormat.Bpp * (x + bx), pixel.X);
                        else if (Type == ChannelType.Snorm)
                            targetPixelFormat.L.EncodeFloat(target[(targetStride * (y + by))..], targetPixelFormat.Bpp * (x + bx), (sbyte) pixel.X / 127f);
                        else
                            targetPixelFormat.L.EncodeFloat(target[(targetStride * (y + by))..], targetPixelFormat.Bpp * (x + bx), pixel.X / 255f);
                        
                        if (targetPixelFormat.A.Type is not (ChannelType.F32 or ChannelType.Sf16 or ChannelType.Uf16))
                            targetPixelFormat.A.EncodeByte(target[(targetStride * (y + by))..], targetPixelFormat.Bpp * (x + bx), pixel.Y);
                        else if (Type == ChannelType.Snorm)
                            targetPixelFormat.A.EncodeFloat(target[(targetStride * (y + by))..], targetPixelFormat.Bpp * (x + bx), (sbyte) pixel.Y / 127f);
                        else
                            targetPixelFormat.A.EncodeFloat(target[(targetStride * (y + by))..], targetPixelFormat.Bpp * (x + bx), pixel.Y / 255f);
                    }
                }

                source = source[BlockSize..];
            }
        }
    }

    /// <inheritdoc/>
    public bool Equals(BcPixelFormat? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Type == other.Type && Version == other.Version && Alpha == other.Alpha;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as BcPixelFormat);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine((int) Type, Version, (int) Alpha);
}
