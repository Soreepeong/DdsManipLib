using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using DdsManipLib.DirectDrawSurface.PixelFormats;
using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;
using DdsManipLib.Utilities;

namespace DdsManipLib.DirectDrawSurface;

/// <summary>
/// Class that deals with DDS files.
/// </summary>
public class DdsFile : ICloneable {
    /// <summary>
    /// Header of this DDS file.
    /// </summary>
    public DdsHeader Header;

    /// <summary>
    /// DXT10 header of this file, if specified. <see cref="Header"/>.<see cref="DdsPixelFormat.FourCc"/> should be <see cref="DdsFourCc.Dx10"/>.
    /// </summary>
    public DdsHeaderDxt10 HeaderDxt10;

    /// <summary>
    /// Buffer for the images contained in this file.
    /// </summary>
    public byte[] Body;

    /// <summary>
    /// Buffer for the extraneous data after the pixel buffer.
    /// </summary>
    public byte[] Tail;

    /// <summary>
    /// Construct a new and empty instance of the class.
    ///
    /// All values are set to zero; correct values in the header must be set.
    /// </summary>
    public DdsFile() {
        Body = Array.Empty<byte>();
        Tail = Array.Empty<byte>();
    }

    /// <summary>
    /// Construct a new instance of the class from the given values, and allocate the buffers.
    /// </summary>
    public DdsFile(in DdsHeader header) {
        Header = header;
        Body = new byte[BodySize];
        Tail = Array.Empty<byte>();
    }

    /// <summary>
    /// Construct a new instance of the class from the given values, and allocate the buffers.
    /// </summary>
    public DdsFile(in DdsHeader header, in DdsHeaderDxt10 headerDxt10) {
        Header = header;
        HeaderDxt10 = headerDxt10;
        Body = new byte[BodySize];
        Tail = Array.Empty<byte>();
    }

    /// <summary>
    /// Construct a new instance of the class from the given values.
    /// </summary>
    /// <param name="header"></param>
    /// <param name="data">The pixel buffer.</param>
    /// <param name="tail">Extra data after the pixel buffer.</param>
    public DdsFile(in DdsHeader header, byte[] data, byte[] tail) {
        Header = header;
        Body = data;
        Tail = tail;
    }

    /// <summary>
    /// Construct a new instance of the class from the given values.
    /// </summary>
    /// <param name="header"></param>
    /// <param name="headerDxt10"></param>
    /// <param name="data">The pixel buffer.</param>
    /// <param name="tail">Extra data after the pixel buffer.</param>
    public DdsFile(in DdsHeader header, in DdsHeaderDxt10 headerDxt10, byte[] data, byte[] tail) {
        Header = header;
        HeaderDxt10 = headerDxt10;
        Body = data;
        Tail = tail;
    }

    /// <summary>
    /// Offset to the pixel buffer in this file, after the header(s).
    /// </summary>
    public int BodyOffset => Unsafe.SizeOf<DdsHeaderWithMagic>() +
        (UseDxt10Header ? Unsafe.SizeOf<DdsHeaderDxt10>() : 0);

    /// <summary>
    /// Offset to the tail in this file, after body.
    /// </summary>
    public int TailOffset => BodyOffset + BodySize;

    /// <summary>
    /// Whether to use DXT10 header. Pixel format may be invalidated on value change.
    /// Use <see cref="EnableDxt10Header"/> or <see cref="DisableDxt10Header"/> instead to determine whether the operation has resulted in pixel format invalidation.
    /// </summary>
    public bool UseDxt10Header {
        get => Header.PixelFormat.FourCc == DdsFourCc.Dx10;
        set {
            if (value)
                EnableDxt10Header();
            else
                DisableDxt10Header();
        }
    }

    /// <summary>
    /// Enable the use of DXT10 Header.
    /// </summary>
    /// <param name="pixelFormat">If supplied, the pixel format will be update to the supplied value.</param>
    /// <returns>Whether the field <see cref="DdsHeaderDxt10.DxgiFormat"/> contains a valid value.</returns>
    public bool EnableDxt10Header(PixelFormat? pixelFormat = null) {
        if (UseDxt10Header)
            return HeaderDxt10.DxgiFormat != DxgiFormat.Unknown;

        pixelFormat ??= PixelFormat;
        // Note: set Header.PixelFormat later so that the values are taken from non-DXT10 headers.
        HeaderDxt10 = new() {
            DxgiFormat = pixelFormat.ToDxgiFormat(),
            ResourceDimension = Is1D
                ? DdsHeaderDxt10ResourceDimension.Texture1D
                : Is3D
                    ? DdsHeaderDxt10ResourceDimension.Texture3D
                    : DdsHeaderDxt10ResourceDimension.Texture2D,
            MiscFlag = IsCubeMap ? DdsHeaderDxt10MiscFlags.TextureCube : 0,
            ArraySize = NumImages,
            MiscFlags2 = pixelFormat.Alpha switch {
                AlphaType.None => DdsHeaderDxt10MiscFlags2.AlphaModeOpaque,
                AlphaType.Straight => DdsHeaderDxt10MiscFlags2.AlphaModeStraight,
                AlphaType.Premultiplied => DdsHeaderDxt10MiscFlags2.AlphaModePremultiplied,
                AlphaType.Custom => DdsHeaderDxt10MiscFlags2.AlphaModeCustom,
                _ => DdsHeaderDxt10MiscFlags2.AlphaModeStraight,
            },
        };
        Header.PixelFormat = new() {
            Size = Unsafe.SizeOf<DdsPixelFormat>(),
            Flags = DdsPixelFormatFlags.FourCc,
            FourCc = DdsFourCc.Dx10,
        };

        return HeaderDxt10.DxgiFormat != DxgiFormat.Unknown;
    }

    /// <summary>
    /// Disable the use of DXT10 Header.
    /// </summary>
    /// <param name="pixelFormat">If supplied, the pixel format will be update to the supplied value.</param>
    /// <returns>Whether the field <see cref="DdsHeader.PixelFormat"/>.<see cref="DdsPixelFormat.Flags"/> contains a valid value.</returns>
    public bool DisableDxt10Header(PixelFormat? pixelFormat = null) {
        if (!UseDxt10Header)
            return Header.PixelFormat.Flags != 0;

        pixelFormat ??= PixelFormat;
        if (!TryUpdatePixelFormat(pixelFormat, true, false)) {
            Header.PixelFormat = default;
            HeaderDxt10 = default;
        }

        return Header.PixelFormat.Flags != 0;
    }

    /// <summary>
    /// Number of images contained in this file.
    /// </summary>
    public int NumImages {
        get => UseDxt10Header ? HeaderDxt10.ArraySize : 1;
        set {
            if (value == 1) {
                if (UseDxt10Header)
                    HeaderDxt10.ArraySize = 1;
            } else {
                if (!UseDxt10Header)
                    throw new NotSupportedException("Enable DXT10 header to use texture array.");
                HeaderDxt10.ArraySize = value;
            }
        }
    }

    /// <summary>
    /// Number of mipmaps contained in this file.
    /// </summary>
    public int NumMipmaps {
        get => Header.Flags.HasFlag(DdsHeaderFlags.MipmapCount) ? Header.MipMapCount : 1;
        set {
            switch (value) {
                case 1:
                    Header.MipMapCount = 0;
                    Header.Flags &= ~DdsHeaderFlags.MipmapCount;
                    break;
                case > 1:
                    Header.MipMapCount = value;
                    Header.Flags |= DdsHeaderFlags.MipmapCount;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
    }

    /// <summary>
    /// Number of bits per pixel, for the pixel format used in this file.
    /// </summary>
    public int Bpp => PixelFormat.Bpp;

    /// <summary>
    /// Get whether this file represents an one-dimensional texture.
    ///
    /// Explicitly setting this to false will make this DDS file invalid until you set the appropriate values back.
    /// </summary>
    public bool Is1D {
        get => !Header.Caps2.HasFlag(DdsCaps2.Cubemap) &&
            (Header.Flags & DdsHeaderFlags.DimensionMask) == DdsHeaderFlags.Dimension1;
        set {
            if (value) {
                Header.Caps2 &= ~DdsCaps2.Cubemap;
                Header.Flags = (Header.Flags & ~DdsHeaderFlags.DimensionMask) | DdsHeaderFlags.Dimension1;
            } else {
                Header.Caps2 &= ~DdsCaps2.Cubemap;
                Header.Flags &= ~DdsHeaderFlags.DimensionMask;
            }
        }
    }

    /// <summary>
    /// Get whether this file represents a two-dimensional texture.
    ///
    /// Explicitly setting this to false will make this DDS file invalid until you set the appropriate values back.
    /// </summary>
    public bool Is2D {
        get => !Header.Caps2.HasFlag(DdsCaps2.Cubemap) &&
            (Header.Flags & DdsHeaderFlags.DimensionMask) == DdsHeaderFlags.Dimension2;
        set {
            if (value) {
                Header.Caps2 &= ~DdsCaps2.Cubemap;
                Header.Flags = (Header.Flags & ~DdsHeaderFlags.DimensionMask) | DdsHeaderFlags.Dimension2;
            } else {
                Header.Caps2 &= ~DdsCaps2.Cubemap;
                Header.Flags &= ~DdsHeaderFlags.DimensionMask;
            }
        }
    }

    /// <summary>
    /// Get whether this file represents a three-dimensional texture.
    ///
    /// Explicitly setting this to false will make this DDS file invalid until you set the appropriate values back.
    /// </summary>
    public bool Is3D {
        get => !Header.Caps2.HasFlag(DdsCaps2.Cubemap) &&
            (Header.Flags & DdsHeaderFlags.DimensionMask) == DdsHeaderFlags.Dimension3;
        set {
            if (value) {
                Header.Caps2 &= ~DdsCaps2.Cubemap;
                Header.Flags = (Header.Flags & ~DdsHeaderFlags.DimensionMask) | DdsHeaderFlags.Dimension3;
            } else {
                Header.Caps2 &= ~DdsCaps2.Cubemap;
                Header.Flags &= ~DdsHeaderFlags.DimensionMask;
            }
        }
    }

    /// <summary>
    /// Get or set whether this file represents a cube map texture.
    ///
    /// Explicitly setting this to false will make this DDS file invalid until you set the appropriate values back.
    /// </summary>
    public bool IsCubeMap {
        get => Header.Caps2.HasFlag(DdsCaps2.Cubemap) &&
            (Header.Flags & DdsHeaderFlags.DimensionMask) == DdsHeaderFlags.Dimension2;
        set {
            if (value) {
                Header.Caps2 |= DdsCaps2.Cubemap;
                Header.Flags = (Header.Flags & ~DdsHeaderFlags.DimensionMask) | DdsHeaderFlags.Dimension2;
            } else {
                Header.Caps2 &= ~DdsCaps2.Cubemap;
                Header.Flags &= ~DdsHeaderFlags.DimensionMask;
            }
        }
    }

    /// <summary>
    /// Enumerate mipmap sizes.
    /// </summary>
    public IEnumerable<int> MipmapSizes => Enumerable.Range(0, NumMipmaps).Select(MipmapSize);

    /// <summary>
    /// Get the width of the specified mipmap in this file.
    /// </summary>
    /// <param name="mipmapIndex">Index of the mipmap.</param>
    /// <returns>Width of the mipmap.</returns>
    public int Width(int mipmapIndex) =>
        0 <= mipmapIndex && mipmapIndex < NumMipmaps
            ? Header.Flags.HasFlag(DdsHeaderFlags.Width) ? Math.Max(1, Header.Width >> mipmapIndex) : 1
            : throw new ArgumentOutOfRangeException(nameof(mipmapIndex), mipmapIndex, null);

    /// <summary>
    /// Get the pitch(stride) of the specified mipmap in this file.
    /// </summary>
    /// <param name="mipmapIndex">Index of the mipmap.</param>
    /// <returns>Pitch(stride) of the mipmap.</returns>
    public int Pitch(int mipmapIndex) {
        var pf = PixelFormat;
        if (pf is BcPixelFormat bcPixelFormat)
            return Math.Max(1, (Width(mipmapIndex) + 3) / 4) * 4 * bcPixelFormat.Bpp;

        // For R8G8_B8G8, G8R8_G8B8, legacy UYVY-packed, and legacy YUY2-packed formats, compute the pitch as:
        switch (Header.PixelFormat.FourCc) {
            case DdsFourCc.D3dFmtR8G8B8G8:
            case DdsFourCc.D3dFmtG8R8G8B8:
            case DdsFourCc.D3dFmtUyvy:
            case DdsFourCc.D3dFmtYuy2:
                return ((Width(mipmapIndex) + 1) >> 1) * 4;
        }

        switch (pf.DxgiFormat) {
            case DxgiFormat.R8G8B8G8Unorm:
            case DxgiFormat.G8R8G8B8Unorm:
            case DxgiFormat.Yuy2:
                return ((Width(mipmapIndex) + 1) >> 1) * 4;
        }

        return (Width(mipmapIndex) * pf.Bpp + 7) / 8;
    }

    /// <summary>
    /// Get the height of the specified mipmap in this file.
    /// </summary>
    /// <param name="mipmapIndex">Index of the mipmap.</param>
    /// <returns>Height of the mipmap.</returns>
    public int Height(int mipmapIndex) =>
        0 <= mipmapIndex && mipmapIndex < NumMipmaps
            ? Header.Flags.HasFlag(DdsHeaderFlags.Height) ? Math.Max(1, Header.Height >> mipmapIndex) : 1
            : throw new ArgumentOutOfRangeException(nameof(mipmapIndex), mipmapIndex, null);

    /// <summary>
    /// Get the depth of the specified mipmap in this file.
    /// </summary>
    /// <param name="mipmapIndex">Index of the mipmap.</param>
    /// <returns>Depth of the mipmap.</returns>
    public int Depth(int mipmapIndex) => 0 <= mipmapIndex && mipmapIndex < NumMipmaps
        ? Header.Flags.HasFlag(DdsHeaderFlags.Depth) ? Math.Max(1, Header.Depth >> mipmapIndex) : 1
        : throw new ArgumentOutOfRangeException(nameof(mipmapIndex), mipmapIndex, null);

    /// <summary>
    /// Get the number of faces in this file.
    /// </summary>
    public int NumFaces => !IsCubeMap
        ? 1
        : (Header.Caps2.HasFlag(DdsCaps2.CubemapNegativeX) ? 1 : 0)
        + (Header.Caps2.HasFlag(DdsCaps2.CubemapPositiveX) ? 1 : 0)
        + (Header.Caps2.HasFlag(DdsCaps2.CubemapNegativeY) ? 1 : 0)
        + (Header.Caps2.HasFlag(DdsCaps2.CubemapPositiveY) ? 1 : 0)
        + (Header.Caps2.HasFlag(DdsCaps2.CubemapNegativeZ) ? 1 : 0)
        + (Header.Caps2.HasFlag(DdsCaps2.CubemapPositiveZ) ? 1 : 0);

    /// <summary>
    /// Get the depth or number of faces of the specified mipmap in this file.
    /// </summary>
    /// <param name="mipmapIndex">Index of the mipmap.</param>
    /// <returns></returns>
    public int DepthOrNumFaces(int mipmapIndex) => IsCubeMap ? NumFaces : Depth(mipmapIndex);

    /// <summary>
    /// Get the number of bytes occupied by a slice(1D or 2D) in the specified mipmap in this file.
    /// </summary>
    /// <param name="mipmapIndex">Index of the mipmap.</param>
    /// <returns>Number of bytes.</returns>
    public int SliceSize(int mipmapIndex) {
        var pf = PixelFormat;
        if (pf is BcPixelFormat bcPixelFormat) {
            return Math.Max(1, (Width(mipmapIndex) + 3) / 4) *
                Math.Max(1, (Height(mipmapIndex) + 3) / 4) *
                bcPixelFormat.BlockSize;
        }

        return Pitch(mipmapIndex) * Height(mipmapIndex);
    }

    /// <summary>
    /// Get the number of bytes occupied by the specified mipmap in this file.
    /// </summary>
    /// <param name="mipmapIndex">Index of the mipmap.</param>
    /// <returns></returns>
    public int MipmapSize(int mipmapIndex) => SliceSize(mipmapIndex) * Depth(mipmapIndex);

    /// <summary>
    /// Get the number of bytes occupied by a face in this file.
    /// </summary>
    public int FaceSize => Enumerable.Range(0, NumMipmaps).Sum(MipmapSize);

    /// <summary>
    /// Get the number of bytes occupied by one image in this file.
    /// </summary>
    public int ImageSize => FaceSize * NumFaces;

    /// <summary>
    /// Get the number of bytes occupied by all the images in this file.
    /// </summary>
    public int BodySize => ImageSize * NumImages;

    /// <summary>
    /// Get the offset of the specified image in bytes.
    /// </summary>
    /// <param name="imageIndex">Index of the image.</param>
    /// <param name="size">Number of bytes of the image.</param>
    /// <returns>Offset to the image in bytes.</returns>
    public int ImageBodyOffset(int imageIndex, out int size) {
        if (imageIndex < 0 || imageIndex >= NumImages)
            throw new ArgumentOutOfRangeException(nameof(imageIndex), imageIndex, null);
        size = ImageSize;
        return size * imageIndex;
    }

    /// <summary>
    /// Get the offset of the specified face in bytes.
    /// </summary>
    /// <param name="imageIndex">Index of the image.</param>
    /// <param name="faceIndex">Index of the face.</param>
    /// <param name="size">Number of bytes of the face.</param>
    /// <returns>Offset to the face in bytes.</returns>
    public int FaceBodyOffset(int imageIndex, int faceIndex, out int size) {
        var offset = ImageBodyOffset(imageIndex, out _);
        size = FaceSize;
        return offset + size * faceIndex;
    }

    /// <summary>
    /// Get the offset of the specified mipmap in bytes.
    /// </summary>
    /// <param name="imageIndex">Index of the image.</param>
    /// <param name="faceIndex">Index of the face.</param>
    /// <param name="mipmapIndex">Index of the mipmap.</param>
    /// <param name="size">Number of bytes of the mipmap.</param>
    /// <returns>Offset to the mipmap in bytes.</returns>
    public int MipmapBodyOffset(int imageIndex, int faceIndex, int mipmapIndex, out int size) {
        var baseOffset = FaceBodyOffset(imageIndex, faceIndex, out _);
        var mipOffset = Enumerable.Range(0, mipmapIndex).Sum(MipmapSize);
        size = MipmapSize(mipmapIndex);
        return baseOffset + mipOffset;
    }

    /// <summary>
    /// Get the offset of the specified slice in bytes.
    /// </summary>
    /// <param name="imageIndex">Index of the image.</param>
    /// <param name="faceIndex">Index of the face.</param>
    /// <param name="mipmapIndex">Index of the mipmap.</param>
    /// <param name="sliceIndex">Index of the slice.</param>
    /// <param name="size">Number of bytes of the slice.</param>
    /// <returns>Offset to the slice in bytes.</returns>
    public int SliceBodyOffset(int imageIndex, int faceIndex, int mipmapIndex, int sliceIndex, out int size) {
        var offset = MipmapBodyOffset(imageIndex, faceIndex, mipmapIndex, out _);
        size = SliceSize(mipmapIndex);
        return offset + size * sliceIndex;
    }

    /// <summary>
    /// Get the offset of the specified slice or face in bytes.
    ///
    /// If it's a cube map, it will assume the first slice.
    /// Otherwise, it will assume the first face.
    /// </summary>
    /// <param name="imageIndex">Index of the image.</param>
    /// <param name="mipmapIndex">Index of the mipmap.</param>
    /// <param name="sliceOrFaceIndex">Index of the slice or face.</param>
    /// <param name="size">Number of bytes of a slice or face.</param>
    /// <returns>Offset to the slice or face in bytes.</returns>
    public int SliceOrFaceBodyOffset(int imageIndex, int mipmapIndex, int sliceOrFaceIndex, out int size) => IsCubeMap
        ? SliceBodyOffset(imageIndex, sliceOrFaceIndex, mipmapIndex, 0, out size)
        : SliceBodyOffset(imageIndex, 0, mipmapIndex, sliceOrFaceIndex, out size);

    /// <summary>
    /// Get a span view of the specified image contained in <see cref="Body"/>.
    /// </summary>
    /// <param name="imageIndex">Index of the image.</param>
    /// <returns>The span.</returns>
    public Span<byte> ImageSpan(int imageIndex) {
        var offset = ImageBodyOffset(imageIndex, out var size);
        return new(Body, offset, size);
    }

    /// <summary>
    /// Get a span view of the specified face contained in <see cref="Body"/>.
    /// </summary>
    /// <param name="imageIndex">Index of the image.</param>
    /// <param name="faceIndex">Index of the face.</param>
    /// <returns>The span.</returns>
    public Span<byte> FaceSpan(int imageIndex, int faceIndex) {
        var offset = FaceBodyOffset(imageIndex, faceIndex, out var size);
        return new(Body, offset, size);
    }

    /// <summary>
    /// Get a span view of the specified mipmap contained in <see cref="Body"/>.
    /// </summary>
    /// <param name="imageIndex">Index of the image.</param>
    /// <param name="faceIndex">Index of the face.</param>
    /// <param name="mipmapIndex">Index of the mipmap.</param>
    /// <returns>The span.</returns>
    public Span<byte> MipmapSpan(int imageIndex, int faceIndex, int mipmapIndex) {
        var offset = MipmapBodyOffset(imageIndex, faceIndex, mipmapIndex, out var size);
        return new(Body, offset, size);
    }

    /// <summary>
    /// Get a span view of the specified slice contained in <see cref="Body"/>.
    /// </summary>
    /// <param name="imageIndex">Index of the image.</param>
    /// <param name="faceIndex">Index of the face.</param>
    /// <param name="mipmapIndex">Index of the mipmap.</param>
    /// <param name="sliceIndex">Index of the slice.</param>
    /// <returns>The span.</returns>
    public Span<byte> SliceSpan(int imageIndex, int faceIndex, int mipmapIndex, int sliceIndex) {
        var offset = SliceBodyOffset(imageIndex, faceIndex, mipmapIndex, sliceIndex, out var size);
        return new(Body, offset, size);
    }

    /// <summary>
    /// Get a span view of the specified slice or face contained in <see cref="Body"/>.
    ///
    /// If it's a cube map, it will assume the first slice.
    /// Otherwise, it will assume the first face.
    /// </summary>
    /// <param name="imageIndex">Index of the image.</param>
    /// <param name="mipmapIndex">Index of the mipmap.</param>
    /// <param name="sliceOrFaceIndex">Index of the slice or face.</param>
    /// <returns>The span.</returns>
    public Span<byte> SliceOrFaceSpan(int imageIndex, int mipmapIndex, int sliceOrFaceIndex) => IsCubeMap
        ? SliceSpan(imageIndex, sliceOrFaceIndex, mipmapIndex, 0)
        : SliceSpan(imageIndex, 0, mipmapIndex, sliceOrFaceIndex);

    /// <summary>
    /// Get a memory view of the specified image contained in <see cref="Body"/>.
    /// </summary>
    /// <param name="imageIndex">Index of the image.</param>
    /// <returns>The memory.</returns>
    public Memory<byte> ImageMemory(int imageIndex) {
        var offset = ImageBodyOffset(imageIndex, out var size);
        return new(Body, offset, size);
    }

    /// <summary>
    /// Get a memory view of the specified face contained in <see cref="Body"/>.
    /// </summary>
    /// <param name="imageIndex">Index of the image.</param>
    /// <param name="faceIndex">Index of the face.</param>
    /// <returns>The memory.</returns>
    public Memory<byte> FaceMemory(int imageIndex, int faceIndex) {
        var offset = FaceBodyOffset(imageIndex, faceIndex, out var size);
        return new(Body, offset, size);
    }

    /// <summary>
    /// Get a memory view of the specified mipmap contained in <see cref="Body"/>.
    /// </summary>
    /// <param name="imageIndex">Index of the image.</param>
    /// <param name="faceIndex">Index of the face.</param>
    /// <param name="mipmapIndex">Index of the mipmap.</param>
    /// <returns>The memory.</returns>
    public Memory<byte> MipmapMemory(int imageIndex, int faceIndex, int mipmapIndex) {
        var offset = MipmapBodyOffset(imageIndex, faceIndex, mipmapIndex, out var size);
        return new(Body, offset, size);
    }

    /// <summary>
    /// Get a memory view of the specified slice contained in <see cref="Body"/>.
    /// </summary>
    /// <param name="imageIndex">Index of the image.</param>
    /// <param name="faceIndex">Index of the face.</param>
    /// <param name="mipmapIndex">Index of the mipmap.</param>
    /// <param name="sliceIndex">Index of the slice.</param>
    /// <returns>The memory.</returns>
    public Memory<byte> SliceMemory(int imageIndex, int faceIndex, int mipmapIndex, int sliceIndex) {
        var offset = SliceBodyOffset(imageIndex, faceIndex, mipmapIndex, sliceIndex, out var size);
        return new(Body, offset, size);
    }

    /// <summary>
    /// Get a memory view of the specified slice or face contained in <see cref="Body"/>.
    ///
    /// If it's a cube map, it will assume the first slice.
    /// Otherwise, it will assume the first face.
    /// </summary>
    /// <param name="imageIndex">Index of the image.</param>
    /// <param name="mipmapIndex">Index of the mipmap.</param>
    /// <param name="sliceOrFaceIndex">Index of the slice or face.</param>
    /// <returns>The memory.</returns>
    public Memory<byte> SliceOrFaceMemory(int imageIndex, int mipmapIndex, int sliceOrFaceIndex) => IsCubeMap
        ? SliceMemory(imageIndex, sliceOrFaceIndex, mipmapIndex, 0)
        : SliceMemory(imageIndex, 0, mipmapIndex, sliceOrFaceIndex);

    /// <summary>
    /// Attempt to deduce a corresponding <see cref="PixelFormat"/>.
    /// </summary>
    /// <param name="pixelFormat">The resulting pixel format, or <see cref="UnknownPixelFormat"/> if not found.</param>
    /// <returns>Whether the corresponding format has been found.</returns>
    public bool TryDeducePixelFormat(out PixelFormat pixelFormat) {
        if (Header.PixelFormat.TryGetPixelFormat(out pixelFormat))
            return true;

        return UseDxt10Header && HeaderDxt10.TryToPixelFormat(out pixelFormat);
    }

    /// <summary>
    /// Attempt to update the pixel format.
    ///
    /// Number of images may be reset, in case the legacy format has been used.
    /// </summary>
    /// <param name="pixelFormat"></param>
    /// <param name="enableLegacyFormat">Enable the use of describing the pixel format using the fields in <see cref="DdsPixelFormat"/>.</param>
    /// <param name="enableDxt10HeaderFormat">Enable the use of describing the pixel format using the fields in <see cref="DdsHeaderDxt10"/>.</param>
    /// <returns>Whether the pixel format has been updated.</returns>
    public bool TryUpdatePixelFormat(PixelFormat pixelFormat, bool enableLegacyFormat, bool enableDxt10HeaderFormat) {
        // If Pitch * Height == LinearSize, use Pitch; otherwise, use LinearSize.
        var newPitch = pixelFormat.CalculatePitch(Header.Width);
        var newLinearSize = pixelFormat.CalculateLinearSize(Header.Width, Header.Height);
        var newFlags = Header.Flags & ~(DdsHeaderFlags.Pitch | DdsHeaderFlags.LinearSize);
        newFlags |= newPitch * Header.Height == newLinearSize ? DdsHeaderFlags.Pitch : DdsHeaderFlags.LinearSize;
        var newPitchOrLinearSize = newFlags.HasFlag(DdsHeaderFlags.Pitch) ? newPitch : newLinearSize;

        if (enableLegacyFormat && Header.PixelFormat.TryUpdateFromPixelFormat(pixelFormat)) {
            Header.PitchOrLinearSize = newPitchOrLinearSize;
            Header.Flags = newFlags;
            HeaderDxt10 = default;
            return true;
        }

        if (enableDxt10HeaderFormat && pixelFormat.ToDxgiFormat() != DxgiFormat.Unknown) {
            Header.PitchOrLinearSize = newPitchOrLinearSize;
            Header.Flags = newFlags;
            EnableDxt10Header(pixelFormat);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Get or set the associated <see cref="PixelFormat"/> for this file.
    /// </summary>
    public PixelFormat PixelFormat => TryDeducePixelFormat(out var pf) ? pf : UnknownPixelFormat.Instance;

    /// <summary>
    /// Whether the pixel format indicates empty value. 
    /// </summary>
    public bool IsPixelFormatEmpty => Header.PixelFormat.Flags == 0 ||
        (Header.PixelFormat.Flags.HasFlag(DdsPixelFormatFlags.FourCc) && Header.PixelFormat.FourCc == 0) ||
        (Header.PixelFormat.Flags.HasFlag(DdsPixelFormatFlags.FourCc) &&
            Header.PixelFormat.FourCc == DdsFourCc.Dx10 && HeaderDxt10.DxgiFormat == 0);

    /// <summary>
    /// Write this DDS file to the given BinaryWriter.
    /// 
    /// Zeroes will be added or extra data will be discarded in Body to fit BodySize.
    /// </summary>
    /// <param name="stream">A <see cref="Stream"/> to write to.</param>
    /// <param name="leaveOpen">Whether to keep the stream open.</param>
    public void WriteTo(Stream stream, bool leaveOpen = false) {
        using var writer = new EndiannessEnforcingWriter(stream, Encoding.UTF8, leaveOpen);
        writer.Write(DdsHeaderWithMagic.MagicValue);
        Header.WriteTo(writer);
        if (UseDxt10Header)
            HeaderDxt10.WriteTo(writer);

        var bodySize = BodySize;
        if (Body.Length < bodySize) {
            writer.Write(Body);
            Span<byte> zeroes = stackalloc byte[Math.Min(4096, bodySize - Body.Length)];
            for (var remaining = bodySize - Body.Length; remaining > 0; remaining -= zeroes.Length)
                writer.Write(zeroes[..Math.Min(zeroes.Length, remaining)]);
        } else if (Body.Length > bodySize)
            writer.Write(Body.AsSpan(0, bodySize));
        else
            writer.Write(Body);

        writer.Write(Tail);
    }

    /// <summary>
    /// Write this DDS file to the specified path.
    /// </summary>
    public void WriteToFile(string path) => WriteTo(File.Create(path));

    /// <summary>
    /// Enumerate all slices' indices.
    /// </summary>
    public IEnumerable<SliceInfo> EnumerateSlices(
        Range? images = default,
        Range? faces = default,
        Range? mipmaps = default,
        Range? slices = default) {
        var imageStart = (images ?? Range.All).Start.GetOffset(NumImages);
        var imageEnd = (images ?? Range.All).End.GetOffset(NumImages);
        var faceStart = (faces ?? Range.All).Start.GetOffset(NumFaces);
        var faceEnd = (faces ?? Range.All).End.GetOffset(NumFaces);
        var mipmapStart = (mipmaps ?? Range.All).Start.GetOffset(NumMipmaps);
        var mipmapEnd = (mipmaps ?? Range.All).End.GetOffset(NumMipmaps);
        var si = new SliceInfo();
        for (si.Image = imageStart; si.Image < imageEnd; si.Image++) {
            for (si.Face = faceStart; si.Face < faceEnd; si.Face++) {
                for (si.Mipmap = mipmapStart; si.Mipmap < mipmapEnd; si.Mipmap++) {
                    si.Depth = Depth(si.Mipmap);
                    si.Width = Width(si.Mipmap);
                    si.Height = Height(si.Mipmap);
                    var sliceStart = (slices ?? Range.All).Start.GetOffset(si.Depth);
                    var sliceEnd = (slices ?? Range.All).End.GetOffset(si.Depth);
                    for (si.Slice = sliceStart; si.Slice < sliceEnd; si.Slice++)
                        yield return si;
                }
            }
        }
    }

    /// <summary>
    /// Initialize <see cref="Body" />.
    /// </summary>
    /// <param name="alwaysReallocate">If false, and the size is correct, the buffer will not be reallocated, and instead cleared.</param>
    /// <returns>Whether the buffer has been newly allocated.</returns>
    public bool InitializeBody(bool alwaysReallocate = false) {
        if (Body.Length != BodySize || alwaysReallocate) {
            Body = new byte[BodySize];
            return true;
        }

        Array.Clear(Body);
        return false;
    }

    /// <summary>
    /// Attempt to initialize this object from the source file, using the specified pixel format.
    ///
    /// Conversion across pixel formats with different number of channels may do anything.
    /// It is recommended to do that manually.
    /// </summary>
    /// <param name="pixelFormat"></param>
    /// <param name="source"></param>
    /// <param name="convertBody">Whether to convert the body.</param>
    /// <returns>If false, the attempt was unsuccessful, and the object is in indeterminate state.</returns>
    public bool TryInitializeFrom(PixelFormat pixelFormat, DdsFile source, bool convertBody = true) {
        Header = source.Header;
        HeaderDxt10 = source.HeaderDxt10;
        if (!TryUpdatePixelFormat(pixelFormat, NumImages == 1, true))
            return false;

        if (convertBody) {
            InitializeBody();
            var sourcePixelFormat = source.PixelFormat;

            byte[]? tmp = null;
            foreach (var slice in EnumerateSlices()) {
                switch (sourcePixelFormat) {
                    case var _ when sourcePixelFormat == pixelFormat:
                        source.SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice)
                            .CopyTo(SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice));
                        break;
                    case RgbaPixelFormat rgbaS:
                        switch (pixelFormat) {
                            case RgbaPixelFormat rgbaT:
                                rgbaS.ConvertToRgba(
                                    rgbaT,
                                    SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                    Pitch(slice.Mipmap),
                                    source.SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                    source.Pitch(slice.Mipmap),
                                    slice.Width,
                                    slice.Height);
                                break;
                            case BcPixelFormat bcT when bcT.Version != 6:
                                rgbaS.ConvertToBc(
                                    bcT,
                                    SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                    source.SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                    source.Pitch(slice.Mipmap),
                                    slice.Width,
                                    slice.Height);
                                break;
                            case LumiPixelFormat lumiT:
                                rgbaS.ConvertToLumi(
                                    lumiT,
                                    SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                    Pitch(slice.Mipmap),
                                    source.SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                    source.Pitch(slice.Mipmap),
                                    slice.Width,
                                    slice.Height);
                                break;
                            default:
                                return false;
                        }

                        break;
                    case BcPixelFormat bcS:
                        switch (pixelFormat) {
                            case RgbaPixelFormat rgbaT:
                                bcS.ConvertToRgba(
                                    rgbaT,
                                    SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                    Pitch(slice.Mipmap),
                                    source.SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                    slice.Width,
                                    slice.Height);
                                break;
                            case BcPixelFormat bcT when bcT.Version != 6:
                                tmp ??= new byte[4 * Header.Width * Header.Height];
                                bcS.ConvertToRgba(
                                    RgbaPixelFormat.NewBgra(8, 8, 8, 8, 0, 0, bcT.Type, bcT.Alpha),
                                    tmp,
                                    4 * slice.Width,
                                    source.SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                    slice.Width,
                                    slice.Height);
                                RgbaPixelFormat.NewBgra(8, 8, 8, 8, 0, 0, bcT.Type, bcT.Alpha)
                                    .ConvertToBc(
                                        bcT,
                                        SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                        tmp,
                                        4 * slice.Width,
                                        slice.Width,
                                        slice.Height);
                                break;
                            case LumiPixelFormat lumiT:
                                bcS.ConvertToLumi(
                                    lumiT,
                                    SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                    Pitch(slice.Mipmap),
                                    source.SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                    slice.Width,
                                    slice.Height);
                                break;
                            default:
                                return false;
                        }

                        break;
                    case LumiPixelFormat lumiS:
                        switch (pixelFormat) {
                            case RgbaPixelFormat rgbaT:
                                lumiS.ConvertToRgba(
                                    rgbaT,
                                    SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                    Pitch(slice.Mipmap),
                                    source.SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                    source.Pitch(slice.Mipmap),
                                    slice.Width,
                                    slice.Height);
                                break;
                            case BcPixelFormat bcT when bcT.Version != 6:
                                lumiS.ConvertToBc(
                                    bcT,
                                    SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                    source.SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                    source.Pitch(slice.Mipmap),
                                    slice.Width,
                                    slice.Height);
                                break;
                            case LumiPixelFormat lumiT:
                                lumiS.ConvertToLumi(
                                    lumiT,
                                    SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                    Pitch(slice.Mipmap),
                                    source.SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                    source.Pitch(slice.Mipmap),
                                    slice.Width,
                                    slice.Height);
                                break;
                            default:
                                return false;
                        }

                        break;
                    default:
                        return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Attempt to initialize this object for an one-dimensional DDS file.
    /// </summary>
    /// <param name="pixelFormat">The pixel format to be contained in this DDS file.</param>
    /// <param name="width">Width of the first mipmap.</param>
    /// <param name="mipmaps">Number of mipmaps.</param>
    /// <param name="images">Number of images, in case of texture arrays.</param>
    /// <param name="initializeBody">Whether to allocate byte array for the body.</param>
    /// <returns>If false, the attempt was unsuccessful, and the object is in indeterminate state.</returns>
    public bool TryInitialize1D(
        PixelFormat pixelFormat,
        int width,
        int mipmaps = 1,
        int images = 1,
        bool initializeBody = true) {
        if (width <= 0)
            throw new ArgumentOutOfRangeException(nameof(width), width, "Width must be a positive integer.");
        Header = new() {
            Size = Unsafe.SizeOf<DdsHeader>(),
            Flags = DdsHeaderFlags.Caps | DdsHeaderFlags.PixelFormat | DdsHeaderFlags.Width,
            Width = width,
            Caps = DdsCaps1.Texture,
        };

        if (!TryUpdatePixelFormat(pixelFormat, images == 1, true))
            return false;

        NumMipmaps = mipmaps;
        NumImages = images;
        if (initializeBody)
            InitializeBody();

        return true;
    }

    /// <summary>
    /// Attempt to initialize this object for a two-dimensional DDS file.
    /// </summary>
    /// <param name="pixelFormat">The pixel format to be contained in this DDS file.</param>
    /// <param name="width">Width of the first mipmap.</param>
    /// <param name="height">Height of the first mipmap.</param>
    /// <param name="mipmaps">Number of mipmaps.</param>
    /// <param name="images">Number of images, in case of texture arrays.</param>
    /// <param name="initializeBody">Whether to allocate byte array for the body.</param>
    /// <returns>If false, the attempt was unsuccessful, and the object is in indeterminate state.</returns>
    public bool TryInitialize2D(
        PixelFormat pixelFormat,
        int width,
        int height,
        int mipmaps = 1,
        int images = 1,
        bool initializeBody = true) {
        if (width <= 0)
            throw new ArgumentOutOfRangeException(nameof(width), width, "Width must be a positive integer.");
        if (height <= 0)
            throw new ArgumentOutOfRangeException(nameof(height), height, "Height must be a positive integer.");
        Header = new() {
            Size = Unsafe.SizeOf<DdsHeader>(),
            Flags = DdsHeaderFlags.Caps | DdsHeaderFlags.PixelFormat | DdsHeaderFlags.Width | DdsHeaderFlags.Height,
            Width = width,
            Height = height,
            Caps = DdsCaps1.Texture,
        };

        if (!TryUpdatePixelFormat(pixelFormat, images == 1, true))
            return false;

        NumMipmaps = mipmaps;
        NumImages = images;
        if (initializeBody)
            InitializeBody();

        return true;
    }

    /// <summary>
    /// Attempt to initialize this object for a three-dimensional DDS file.
    /// Note that texture array is unsupported for 3D textures, and thus images parameter is unavailable.
    /// </summary>
    /// <param name="pixelFormat">The pixel format to be contained in this DDS file.</param>
    /// <param name="width">Width of the first mipmap.</param>
    /// <param name="height">Height of the first mipmap.</param>
    /// <param name="depth">Depth of the first mipmap.</param>
    /// <param name="mipmaps">Number of mipmaps.</param>
    /// <param name="initializeBody">Whether to allocate byte array for the body.</param>
    /// <returns>If false, the attempt was unsuccessful, and the object is in indeterminate state.</returns>
    public bool TryInitialize3D(
        PixelFormat pixelFormat,
        int width,
        int height,
        int depth,
        int mipmaps = 1,
        bool initializeBody = true) {
        if (width <= 0)
            throw new ArgumentOutOfRangeException(nameof(width), width, "Width must be a positive integer.");
        if (height <= 0)
            throw new ArgumentOutOfRangeException(nameof(height), height, "Height must be a positive integer.");
        if (depth <= 0)
            throw new ArgumentOutOfRangeException(nameof(depth), depth, "Height must be a positive integer.");
        Header = new() {
            Size = Unsafe.SizeOf<DdsHeader>(),
            Flags = DdsHeaderFlags.Caps | DdsHeaderFlags.PixelFormat | DdsHeaderFlags.Width | DdsHeaderFlags.Height |
                DdsHeaderFlags.Depth,
            Width = width,
            Height = height,
            Depth = depth,
            Caps = DdsCaps1.Texture,
        };

        if (!TryUpdatePixelFormat(pixelFormat, true, true))
            return false;

        NumMipmaps = mipmaps;
        if (initializeBody)
            InitializeBody();

        return true;
    }

    /// <summary>
    /// Attempt to initialize this object for a cube map with all six faces defined.
    /// </summary>
    /// <param name="pixelFormat">The pixel format to be contained in this DDS file.</param>
    /// <param name="width">Width of the first mipmap.</param>
    /// <param name="height">Height of the first mipmap.</param>
    /// <param name="mipmaps">Number of mipmaps.</param>
    /// <param name="images">Number of images, in case of texture arrays.</param>
    /// <param name="initializeBody">Whether to allocate byte array for the body.</param>
    /// <returns>If false, the attempt was unsuccessful, and the object is in indeterminate state.</returns>
    public bool TryInitializeCubeMap(
        PixelFormat pixelFormat,
        int width,
        int height,
        int mipmaps = 1,
        int images = 1,
        bool initializeBody = true) {
        if (width <= 0)
            throw new ArgumentOutOfRangeException(nameof(width), width, "Width must be a positive integer.");
        if (height <= 0)
            throw new ArgumentOutOfRangeException(nameof(height), height, "Height must be a positive integer.");
        Header = new() {
            Size = Unsafe.SizeOf<DdsHeader>(),
            Flags = DdsHeaderFlags.Caps | DdsHeaderFlags.PixelFormat | DdsHeaderFlags.Width | DdsHeaderFlags.Height,
            Width = width,
            Height = height,
            Caps = DdsCaps1.Texture | DdsCaps1.Complex,
            Caps2 = DdsCaps2.AllFaces,
        };

        if (!TryUpdatePixelFormat(pixelFormat, images == 1, true))
            return false;

        NumMipmaps = mipmaps;
        NumImages = images;
        if (initializeBody)
            InitializeBody();

        return true;
    }

    /// <inheritdoc/>
    public object Clone() {
        var r = (DdsFile) MemberwiseClone();
        r.Body = (byte[]) r.Body.Clone();
        r.Tail = (byte[]) r.Tail.Clone();
        return r;
    }

    /// <summary>
    /// Construct a new instance of the class from a stream.
    ///
    /// If the file is shorter than the calculated <see cref="BodySize"/>, the unfilled data will be kept as zeroes.
    /// If the file is longer than the calculated <see cref="BodySize"/>, the rest will be used as <see cref="Tail"/>
    /// </summary>
    public static DdsFile FromStream(Stream stream, bool leaveOpen = false) {
        using var br = new EndiannessEnforcingReader(stream, Encoding.UTF8, leaveOpen) {IsBigEndian = false};
        if (br.ReadUInt32() != DdsHeaderWithMagic.MagicValue)
            throw new InvalidDataException("Invalid header value.");
        var header = new DdsHeader();
        header.ReadFrom(br);
        var headerDxt10 = new DdsHeaderDxt10();
        if (header.PixelFormat.Flags.HasFlag(DdsPixelFormatFlags.FourCc) && header.PixelFormat.FourCc == DdsFourCc.Dx10)
            headerDxt10.ReadFrom(br);

        var res = new DdsFile(header, headerDxt10);
        _ = br.Read(res.Body);

        res.Tail = new byte[br.BaseStream.Length - br.BaseStream.Position];
        if (br.Read(res.Tail) != res.Tail.Length)
            throw new IOException("Failed to completely read the stream.");
        return res;
    }
    
    /// <summary>
    /// Construct a new instance of the class from a file.
    /// </summary>
    public static DdsFile FromFile(string path) => FromStream(File.OpenRead(path));

    /// <summary>
    /// Collection of slice index information.
    /// </summary>
    public struct SliceInfo {
        /// <summary>
        /// Index of image.
        /// </summary>
        public int Image;

        /// <summary>
        /// Index of face.
        /// </summary>
        public int Face;

        /// <summary>
        /// Index of mipmap.
        /// </summary>
        public int Mipmap;

        /// <summary>
        /// Index of slice.
        /// </summary>
        public int Slice;

        /// <summary>
        /// Depth of mipmap.
        /// </summary>
        public int Depth;

        /// <summary>
        /// Width of mipmap.
        /// </summary>
        public int Width;

        /// <summary>
        /// Height of mipmap.
        /// </summary>
        public int Height;
    }
}
