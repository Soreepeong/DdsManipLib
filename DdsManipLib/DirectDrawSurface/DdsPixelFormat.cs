using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DdsManipLib.DirectDrawSurface.PixelFormats;
using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

namespace DdsManipLib.DirectDrawSurface;

/// <summary>
/// Surface pixel format, stored in <see cref="DdsHeader" />.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 32)]
public struct DdsPixelFormat {
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
    /// Set this object to indicate a FourCC.
    /// </summary>
    public void SetFourCc(DdsFourCc fourCc) {
        this = default;
        Size = Unsafe.SizeOf<DdsPixelFormat>();
        Flags = DdsPixelFormatFlags.FourCc;
        FourCc = fourCc;
    }

    /// <summary>
    /// Set this object to indicate a luminance channel, and optionally with an alpha channel.
    /// </summary>
    public void SetLuminance(int nbits, uint lmask, uint amask) {
        this = default;
        Size = Unsafe.SizeOf<DdsPixelFormat>();
        RgbBitCount = nbits;
        Flags = DdsPixelFormatFlags.Luminance | (amask == 0 ? 0 : DdsPixelFormatFlags.AlphaPixels);
        RBitMask = lmask;
        ABitMask = amask;
    }

    /// <summary>
    /// Set this object to indicate an alpha channel.
    /// </summary>
    public void SetAlpha(int nbits, uint amask) {
        this = default;
        Size = Unsafe.SizeOf<DdsPixelFormat>();
        RgbBitCount = nbits;
        Flags = DdsPixelFormatFlags.Alpha | DdsPixelFormatFlags.AlphaPixels;
        ABitMask = amask;
    }

    /// <summary>
    /// Set this object to indicate BGRA channels.
    /// </summary>
    public void SetBgra(int nbits, uint bmask, uint gmask, uint rmask, uint amask) {
        this = default;
        Size = Unsafe.SizeOf<DdsPixelFormat>();
        RgbBitCount = nbits;
        Flags = DdsPixelFormatFlags.Rgb | (amask == 0 ? 0 : DdsPixelFormatFlags.AlphaPixels);
        BBitMask = bmask;
        GBitMask = gmask;
        RBitMask = rmask;
        ABitMask = amask;
    }

    /// <summary>
    /// Attempt to deduce a corresponding <see cref="PixelFormat"/> from this <see cref="DdsPixelFormat"/>.
    /// </summary>
    /// <param name="pixelFormat">The resulting pixel format, or <see cref="UnknownPixelFormat"/> if not found.</param>
    /// <returns>Whether the corresponding format has been found.</returns>
    public bool TryGetPixelFormat(out PixelFormat pixelFormat) {
        pixelFormat = UnknownPixelFormat.Instance;

        if (!Flags.HasFlag(DdsPixelFormatFlags.FourCc)) {
            var alpha = new ChannelDefinition();
            if (Flags.HasFlag(DdsPixelFormatFlags.AlphaPixels))
                alpha = ChannelDefinition.FromMask(ChannelType.Unorm, ABitMask);
            if (Flags.HasFlag(DdsPixelFormatFlags.Rgb)) {
                var xbitmask =
                    unchecked((1u << RgbBitCount) - 1u) & ~(RBitMask | GBitMask | BBitMask) &
                    (Flags.HasFlag(DdsPixelFormatFlags.AlphaPixels) ? ~ABitMask : ~0u);
                pixelFormat = new RgbaPixelFormat(
                    alpha.IsEmpty ? AlphaType.None : AlphaType.Straight,
                    r: ChannelDefinition.FromMask(ChannelType.Unorm, RBitMask),
                    g: ChannelDefinition.FromMask(ChannelType.Unorm, GBitMask),
                    b: ChannelDefinition.FromMask(ChannelType.Unorm, BBitMask),
                    a: alpha,
                    x1: ChannelDefinition.FromMask(ChannelType.Typeless, xbitmask));
                return true;
            }

            if (Flags.HasFlag(DdsPixelFormatFlags.Yuv)) {
                var xbitmask =
                    unchecked((1u << RgbBitCount) - 1u) & ~(RBitMask | GBitMask | BBitMask) &
                    (Flags.HasFlag(DdsPixelFormatFlags.AlphaPixels) ? ~ABitMask : ~0u);
                pixelFormat = new YuvPixelFormat(
                    alpha.IsEmpty ? AlphaType.None : AlphaType.Straight,
                    y: ChannelDefinition.FromMask(ChannelType.Unorm, RBitMask),
                    u: ChannelDefinition.FromMask(ChannelType.Unorm, GBitMask),
                    v: ChannelDefinition.FromMask(ChannelType.Unorm, BBitMask),
                    a: alpha,
                    x: ChannelDefinition.FromMask(ChannelType.Typeless, xbitmask));
                return true;
            }

            if (Flags.HasFlag(DdsPixelFormatFlags.Luminance)) {
                var xbitmask =
                    unchecked((1u << RgbBitCount) - 1u) & ~RBitMask &
                    (Flags.HasFlag(DdsPixelFormatFlags.AlphaPixels) ? ~ABitMask : ~0u);
                pixelFormat = new LumiPixelFormat(
                    alpha.IsEmpty ? AlphaType.None : AlphaType.Straight,
                    l: ChannelDefinition.FromMask(ChannelType.Unorm, RBitMask),
                    a: alpha,
                    x: ChannelDefinition.FromMask(ChannelType.Typeless, xbitmask));
                return true;
            }

            if (Flags.HasFlag(DdsPixelFormatFlags.Alpha)) {
                var xbitmask = unchecked((1u << RgbBitCount) - 1u) & ~ABitMask;
                pixelFormat = new RgbaPixelFormat(
                    AlphaType.Straight,
                    a: alpha,
                    x1: ChannelDefinition.FromMask(ChannelType.Typeless, xbitmask));
                return true;
            }

            return false;
        }

        pixelFormat = FourCc.ToPixelFormat();
        return !Equals(pixelFormat, UnknownPixelFormat.Instance);
    }

    /// <summary>
    /// Attempt to deduce the fields of this object from a <see cref="PixelFormat"/>.
    /// </summary>
    /// <param name="value">The value to convert from.</param>
    /// <returns>Whether the corresponding format has been found.</returns>
    public bool TryUpdateFromPixelFormat(PixelFormat value) {
        if (value.Alpha is AlphaType.Straight or AlphaType.None) {
            var alpha = value.Alpha is AlphaType.Straight;
            switch (value) {
                case RgbaPixelFormat {X2.IsEmpty: true} rgba
                    when rgba.R.Bits + rgba.G.Bits + rgba.B.Bits + rgba.A.Bits + rgba.X1.Bits <= 32
                    && (rgba.R.IsEmpty || rgba.R.Type is ChannelType.Unorm)
                    && (rgba.G.IsEmpty || rgba.G.Type is ChannelType.Unorm)
                    && (rgba.B.IsEmpty || rgba.B.Type is ChannelType.Unorm)
                    && (rgba.A.IsEmpty || rgba.A.Type is ChannelType.Unorm): {
                    Size = Unsafe.SizeOf<DdsPixelFormat>();
                    Flags = (alpha ? DdsPixelFormatFlags.AlphaPixels : 0)
                        | (rgba.R.IsEmpty && rgba.G.IsEmpty && rgba.B.IsEmpty
                            ? DdsPixelFormatFlags.Alpha
                            : DdsPixelFormatFlags.Rgb);
                    FourCc = 0;
                    RgbBitCount = rgba.R.Bits + rgba.G.Bits + rgba.B.Bits + rgba.A.Bits + rgba.X1.Bits;
                    RBitMask = rgba.R.Mask << rgba.R.Shift;
                    GBitMask = rgba.G.Mask << rgba.G.Shift;
                    BBitMask = rgba.B.Mask << rgba.B.Shift;
                    ABitMask = rgba.A.Mask << rgba.A.Shift;
                    return true;
                }
                case YuvPixelFormat {X.IsEmpty: true} yuv
                    when yuv.Y.Bits + yuv.U.Bits + yuv.V.Bits + yuv.A.Bits + yuv.X.Bits <= 32
                    && (yuv.Y.IsEmpty || yuv.Y.Type is ChannelType.Unorm)
                    && (yuv.U.IsEmpty || yuv.U.Type is ChannelType.Unorm)
                    && (yuv.V.IsEmpty || yuv.V.Type is ChannelType.Unorm)
                    && (yuv.A.IsEmpty || yuv.A.Type is ChannelType.Unorm): {
                    Size = Unsafe.SizeOf<DdsPixelFormat>();
                    Flags = (alpha ? DdsPixelFormatFlags.AlphaPixels : 0)
                        | (yuv.Y.IsEmpty && yuv.U.IsEmpty && yuv.V.IsEmpty
                            ? DdsPixelFormatFlags.Alpha
                            : DdsPixelFormatFlags.Yuv);
                    FourCc = 0;
                    RgbBitCount = yuv.Y.Bits + yuv.U.Bits + yuv.V.Bits + yuv.A.Bits + yuv.X.Bits;
                    RBitMask = yuv.Y.Mask << yuv.Y.Shift;
                    GBitMask = yuv.U.Mask << yuv.U.Shift;
                    BBitMask = yuv.V.Mask << yuv.V.Shift;
                    ABitMask = yuv.A.Mask << yuv.A.Shift;
                    return true;
                }
                case LumiPixelFormat {X.IsEmpty: true} lumi
                    when lumi.L.Bits + lumi.A.Bits + lumi.X.Bits <= 32
                    && (lumi.L.IsEmpty || lumi.L.Type is ChannelType.Unorm)
                    && (lumi.A.IsEmpty || lumi.A.Type is ChannelType.Unorm): {
                    Size = Unsafe.SizeOf<DdsPixelFormat>();
                    Flags = (alpha ? DdsPixelFormatFlags.AlphaPixels : 0)
                        | (lumi.L.IsEmpty
                            ? DdsPixelFormatFlags.Alpha
                            : DdsPixelFormatFlags.Luminance);
                    FourCc = 0;
                    RgbBitCount = lumi.L.Bits + lumi.A.Bits + lumi.X.Bits;
                    RBitMask = lumi.L.Mask << lumi.L.Shift;
                    ABitMask = lumi.A.Mask << lumi.A.Shift;
                    GBitMask = BBitMask = 0;
                    return true;
                }
            }
        }

        var fourCc = value.ToFourCc();
        if (fourCc != DdsFourCc.Unknown) {
            Size = Unsafe.SizeOf<DdsPixelFormat>();
            Flags = DdsPixelFormatFlags.FourCc;
            FourCc = fourCc;
            RgbBitCount = 0;
            RBitMask = GBitMask = BBitMask = ABitMask = 0;
            return true;
        }

        return false;
    }
}
