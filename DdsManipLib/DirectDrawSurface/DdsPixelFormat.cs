using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DdsManipLib.DirectDrawSurface;

/// <summary>
/// Surface pixel format, stored in <see cref="DdsHeader" />.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 32)]
public struct DdsPixelFormat : IEquatable<DdsPixelFormat> {
    /// <summary>
    /// Size of this structure. Must be 32;
    /// </summary>
    public int Size;

    /// <summary>
    /// Values which indicate what type of data is in the surface.
    /// </summary>
    public DdsPixelFormatFlags Flags;

    /// <summary>
    /// Four-character codes for specifying compressed or custom formats.
    /// 
    /// Possible values include: DXT1, DXT2, DXT3, DXT4, or DXT5.
    /// A FourCC of DX10 indicates the prescense of the <see cref="DdsHeaderDxt10"/> extended header,
    /// and the <see cref="DdsHeaderDxt10.DxgiFormat" /> member of that structure indicates the true format.
    /// When using a four-character code, <see cref="Flags"/> must include <see cref="DdsPixelFormatFlags.FourCc"/>.
    /// </summary>
    public DdsFourCc FourCc;

    /// <summary>
    /// Number of bits in an RGB (possibly including alpha) format.
    ///
    /// Valid when <see cref="Flags"/> includes <see cref="DdsPixelFormatFlags.Rgb"/>, <see cref="DdsPixelFormatFlags.Luminance"/>, or <see cref="DdsPixelFormatFlags.Yuv"/>.
    /// </summary>
    public int RgbBitCount;

    /// <summary>
    /// Red (or luminance or Y) mask for reading color data.
    ///
    /// For instance, given the A8R8G8B8 format, the red mask would be 0x00ff0000.
    /// </summary>
    public uint RBitMask;

    /// <summary>
    /// Green (or U) mask for reading color data.
    ///
    /// For instance, given the A8R8G8B8 format, the green mask would be 0x0000ff00.
    /// </summary>
    public uint GBitMask;

    /// <summary>
    /// Blue (or V) mask for reading color data.
    ///
    /// For instance, given the A8R8G8B8 format, the blue mask would be 0x000000ff.
    /// </summary>
    public uint BBitMask;

    /// <summary>
    /// Alpha mask for reading alpha data.
    ///
    /// <see cref="Flags"/> must include <see cref="DdsPixelFormatFlags.AlphaPixels"/> or <see cref="DdsPixelFormatFlags.Alpha"/>.
    /// For instance, given the A8R8G8B8 format, the alpha mask would be 0xff000000.
    /// </summary>
    public uint ABitMask;

    /// <summary>
    /// Whether this <see cref="DdsPixelFormat"/> contains a valid pixel format.
    /// </summary>
    public bool HasValidFormat => Flags != 0
        && (
            (Flags == DdsPixelFormatFlags.FourCc && FourCc != DdsFourCc.Unknown)
            || (Flags.HasFlag(DdsPixelFormatFlags.Rgb) && RgbBitCount is > 0 and <= 32)
            || (Flags.HasFlag(DdsPixelFormatFlags.Luminance) && RgbBitCount is > 0 and <= 32)
            || (Flags.HasFlag(DdsPixelFormatFlags.Alpha) && RgbBitCount is > 0 and <= 32)
            || (Flags.HasFlag(DdsPixelFormatFlags.Yuv) && RgbBitCount is > 0 and <= 32));

    /// <summary>
    /// Whether this <see cref="DdsPixelFormat"/> points to the usage of <see cref="DdsHeaderDxt10"/>.
    /// </summary>
    public bool UseDxt10Header => Flags.HasFlag(DdsPixelFormatFlags.FourCc) && FourCc == DdsFourCc.Dx10;

    /// <summary>
    /// Read the struct from the given BinaryReader.
    /// </summary>
    /// <param name="reader">Little-endian BinaryReader to read from.</param>
    public void ReadFrom(BinaryReader reader) {
        Size = reader.ReadInt32();
        Flags = (DdsPixelFormatFlags) reader.ReadInt32();
        FourCc = (DdsFourCc) reader.ReadInt32();
        RgbBitCount = reader.ReadInt32();
        RBitMask = reader.ReadUInt32();
        GBitMask = reader.ReadUInt32();
        BBitMask = reader.ReadUInt32();
        ABitMask = reader.ReadUInt32();
    }

    /// <summary>
    /// Write the struct to the given BinaryWriter.
    /// </summary>
    /// <param name="writer">Little-endian BinaryWriter to write to.</param>
    public readonly void WriteTo(BinaryWriter writer) {
        writer.Write(Size);
        writer.Write((int) Flags);
        writer.Write((int) FourCc);
        writer.Write(RgbBitCount);
        writer.Write(RBitMask);
        writer.Write(GBitMask);
        writer.Write(BBitMask);
        writer.Write(ABitMask);
    }

    /// <summary>
    /// Construct a new instance of <see cref="DdsPixelFormat" /> containing a FourCC.
    /// </summary>
    public static DdsPixelFormat FromFourCc(DdsFourCc fourCc) => new() {
        Size = Unsafe.SizeOf<DdsPixelFormat>(),
        Flags = DdsPixelFormatFlags.FourCc,
        FourCc = fourCc,
    };

    /// <summary>
    /// Construct a new instance of <see cref="DdsPixelFormat" /> containing an alpha channel.
    /// </summary>
    public static DdsPixelFormat FromAlpha(int nbits, uint amask = 0u) => new() {
        Size = Unsafe.SizeOf<DdsPixelFormat>(),
        RgbBitCount = nbits is > 0 and <= 32 ? nbits : throw new ArgumentOutOfRangeException(nameof(nbits), nbits, null),
        Flags = DdsPixelFormatFlags.Alpha | DdsPixelFormatFlags.AlphaPixels,
        ABitMask = amask != 0u ? amask : throw new ArgumentOutOfRangeException(nameof(amask), amask, null),
    };

    /// <summary>
    /// Construct a new instance of <see cref="DdsPixelFormat" /> containing RGB channels, optionally with an alpha channel.
    /// </summary>
    public static DdsPixelFormat FromRgba(int nbits, uint rmask, uint gmask, uint bmask, uint amask = 0u) => new() {
        Size = Unsafe.SizeOf<DdsPixelFormat>(),
        RgbBitCount = nbits is > 0 and <= 32 ? nbits : throw new ArgumentOutOfRangeException(nameof(nbits), nbits, null),
        Flags = DdsPixelFormatFlags.Rgb | (amask == 0 ? 0 : DdsPixelFormatFlags.AlphaPixels),
        RBitMask = rmask,
        GBitMask = gmask,
        BBitMask = bmask,
        ABitMask = amask,
    };

    /// <summary>
    /// Construct a new instance of <see cref="DdsPixelFormat" /> containing a luminance channel, optionally with an alpha channel.
    /// </summary>
    public static DdsPixelFormat FromLuminance(int nbits, uint lmask, uint amask = 0u) => new() {
        Size = Unsafe.SizeOf<DdsPixelFormat>(),
        RgbBitCount = nbits is > 0 and <= 32 ? nbits : throw new ArgumentOutOfRangeException(nameof(nbits), nbits, null),
        Flags = DdsPixelFormatFlags.Luminance | (amask == 0 ? 0 : DdsPixelFormatFlags.AlphaPixels),
        RBitMask = lmask != 0u ? lmask : throw new ArgumentOutOfRangeException(nameof(lmask), lmask, null),
        ABitMask = amask,
    };

    /// <summary>
    /// Construct a new instance of <see cref="DdsPixelFormat" /> containing YUV channels, optionally with an alpha channel.
    /// </summary>
    public static DdsPixelFormat FromYuv(int nbits, uint ymask, uint umask, uint vmask, uint amask = 0u) => new() {
        Size = Unsafe.SizeOf<DdsPixelFormat>(),
        RgbBitCount = nbits is > 0 and <= 32 ? nbits : throw new ArgumentOutOfRangeException(nameof(nbits), nbits, null),
        Flags = DdsPixelFormatFlags.Yuv | (amask == 0 ? 0 : DdsPixelFormatFlags.AlphaPixels),
        RBitMask = ymask,
        GBitMask = umask,
        BBitMask = vmask,
        ABitMask = amask,
    };

    /// <inheritdoc/>
    public bool Equals(DdsPixelFormat other) => Size == other.Size && Flags == other.Flags && FourCc == other.FourCc && RgbBitCount == other.RgbBitCount &&
        RBitMask == other.RBitMask && GBitMask == other.GBitMask && BBitMask == other.BBitMask && ABitMask == other.ABitMask;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is DdsPixelFormat other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Size, (int) Flags, (int) FourCc, RgbBitCount, RBitMask, GBitMask, BBitMask, ABitMask);

    /// <summary>
    /// Test for equality.
    /// </summary>
    /// <param name="left">First item.</param>
    /// <param name="right">Second item.</param>
    /// <returns>Whether they equal.</returns>
    public static bool operator ==(DdsPixelFormat? left, DdsPixelFormat? right) => left?.Equals(right) ?? right is null;

    /// <summary>
    /// Test for inequality.
    /// </summary>
    /// <param name="left">First item.</param>
    /// <param name="right">Second item.</param>
    /// <returns>Whether they does not equal.</returns>
    public static bool operator !=(DdsPixelFormat? left, DdsPixelFormat? right) => !(left?.Equals(right) ?? right is null);
}
