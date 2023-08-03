using System;
using System.Runtime.CompilerServices;
using DdsManipLib.DirectDrawSurface.PixelFormats;

namespace DdsManipLib.DirectDrawSurface;

public partial class DdsFile {
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
    /// Attempt to initialize this object for an one-dimensional DDS file.
    /// </summary>
    /// <param name="pixelFormat">The pixel format to be contained in this DDS file.</param>
    /// <param name="width">Width of the first mipmap.</param>
    /// <param name="mipmaps">Number of mipmaps.</param>
    /// <param name="images">Number of images, in case of texture arrays.</param>
    /// <param name="initializeBody">Whether to allocate byte array for the body.</param>
    /// <returns>If false, the attempt was unsuccessful, and the object is in indeterminate state.</returns>
    public bool TryInitialize1D(
        IPixelFormat pixelFormat,
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
        IPixelFormat pixelFormat,
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
        IPixelFormat pixelFormat,
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
        IPixelFormat pixelFormat,
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
}