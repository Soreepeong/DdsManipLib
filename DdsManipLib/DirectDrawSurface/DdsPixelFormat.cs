using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DdsManipLib.DirectDrawSurface.PixelFormats;
using DdsManipLib.DirectDrawSurface.PixelFormats.Old;
using DdsManipLib.DirectDrawSurface.PixelFormats.Old.Channels;
using DdsManipLib.DirectDrawSurface.PixelFormats.PlainPixelFormats;

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
    /// Set this object to indicate an alpha channel.
    /// </summary>
    public void SetAlpha(int nbits, uint amask) {
        if (nbits is < 0 or > 32)
            throw new ArgumentOutOfRangeException(nameof(nbits), nbits, null);
        this = default;
        Size = Unsafe.SizeOf<DdsPixelFormat>();
        RgbBitCount = nbits;
        Flags = DdsPixelFormatFlags.Alpha | DdsPixelFormatFlags.AlphaPixels;
        ABitMask = amask;
    }

    /// <summary>
    /// Set this object to indicate RGB(A) channels.
    /// </summary>
    public void SetBgra(int nbits, uint bmask, uint gmask, uint rmask, uint amask) {
        if (nbits is < 0 or > 32)
            throw new ArgumentOutOfRangeException(nameof(nbits), nbits, null);
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
    /// Set this object to indicate a luminance channel, and optionally with an alpha channel.
    /// </summary>
    public void SetLuminance(int nbits, uint lmask, uint amask) {
        if (nbits is < 0 or > 32)
            throw new ArgumentOutOfRangeException(nameof(nbits), nbits, null);
        this = default;
        Size = Unsafe.SizeOf<DdsPixelFormat>();
        RgbBitCount = nbits;
        Flags = DdsPixelFormatFlags.Luminance | (amask == 0 ? 0 : DdsPixelFormatFlags.AlphaPixels);
        RBitMask = lmask;
        ABitMask = amask;
    }

    /// <summary>
    /// Set this object to indicate YUV(A) channels.
    /// </summary>
    public void SetYuv(int nbits, uint ymask, uint umask, uint vmask, uint amask) {
        if (nbits is < 0 or > 32)
            throw new ArgumentOutOfRangeException(nameof(nbits), nbits, null);
        this = default;
        Size = Unsafe.SizeOf<DdsPixelFormat>();
        RgbBitCount = nbits;
        Flags = DdsPixelFormatFlags.Yuv | (amask == 0 ? 0 : DdsPixelFormatFlags.AlphaPixels);
        BBitMask = vmask;
        GBitMask = umask;
        RBitMask = ymask;
        ABitMask = amask;
    }

    /// <summary>
    /// Attempt to deduce the fields of this object from a <see cref="IPixelFormat"/>.
    /// </summary>
    /// <param name="value">The value to convert from.</param>
    /// <returns>Whether the corresponding format has been found.</returns>
    public bool TryUpdateFromPixelFormat(IPixelFormat value) {
        if (Enum.GetNames<DdsFourCc>()
                .FirstOrDefault(x => value.Equals(typeof(DdsFourCc).GetField(x)?.GetCustomAttribute<PixelFormat>())) is { } fourCcName) {
            SetFourCc(Enum.Parse<DdsFourCc>(fourCcName));
            return true;
        }
        
        if (value.Bpp is < 0 or > 32)
            return false;

        if (value is RgbaxxPixelFormat rgbaxx) {
            if (rgbaxx.X2 is not null)
                return false;
            SetBgra(
                value.Bpp,
                rgbaxx.Red?.BitMask32Shifted ?? 0u,
                rgbaxx.Green?.BitMask32Shifted ?? 0u,
                rgbaxx.Blue?.BitMask32Shifted ?? 0u, 
                rgbaxx.Alpha?.BitMask32Shifted ?? 0u);
            return true;
        }
        
        if (value is not IPlainPixelFormat)
            return false;

        if (value is IX2PlainPixelFormat)
            return false;

        var r = value as IRedPlainPixelFormat;
        var g = value as IGreenPlainPixelFormat;
        var b = value as IBluePlainPixelFormat;
        var a = value as IAlphaPlainPixelFormat;
        var l = value as ILuminancePlainPixelFormat;
        var yuv = value as IYuvPlainPixelFormat;
        var x1 = value as IX1PlainPixelFormat;

        if (r is not null || g is not null || b is not null) {
            if (l is not null || yuv is not null)
                return false;
            SetBgra(
                value.Bpp,
                r?.Red.BitMask32Shifted ?? 0u,
                g?.Green.BitMask32Shifted ?? 0u,
                b?.Blue.BitMask32Shifted ?? 0u, 
                a?.Alpha.BitMask32Shifted ?? 0u);
            return true;
        }

        if (yuv is not null) {
            SetYuv(
                value.Bpp,
                yuv.Luminance.BitMask32Shifted,
                yuv.ChromaBlue.BitMask32Shifted,
                yuv.ChromaRed.BitMask32Shifted, 
                a?.Alpha.BitMask32Shifted ?? 0u);
            return true;
        }

        if (l is not null) {
            SetLuminance(value.Bpp, l.Luminance.BitMask32Shifted, a?.Alpha.BitMask32Shifted ?? 0u);
            return true;
        }

        if (a is not null) {
            SetAlpha(value.Bpp, a.Alpha.BitMask32Shifted);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Attempt to deduce a corresponding <see cref="IPixelFormat"/> from this <see cref="DdsPixelFormat"/>.
    /// </summary>
    /// <param name="pixelFormat">The resulting pixel format, or null if not found.</param>
    /// <returns>Whether the corresponding format has been found.</returns>
    public bool TryGetPixelFormat([MaybeNullWhen(false)] out IPixelFormat pixelFormat) {
        pixelFormat = null;

        if (!Flags.HasFlag(DdsPixelFormatFlags.FourCc)) {
            var alpha = new ChannelDefinition();
            
            var am = Flags.HasFlag(DdsPixelFormatFlags.AlphaPixels) ? ABitMask : 0u;
            if (Flags.HasFlag(DdsPixelFormatFlags.Rgb)) {
                var xbitmask =
                    unchecked((1u << RgbBitCount) - 1u) & ~(RBitMask | GBitMask | BBitMask) &
                    (Flags.HasFlag(DdsPixelFormatFlags.AlphaPixels) ? ~ABitMask : ~0u);

                var rm = RBitMask;
                var gm = GBitMask;
                var bm = BBitMask;
                var xm = 0xFFFFFFFFu & ~(am | rm | gm | bm);

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
}
