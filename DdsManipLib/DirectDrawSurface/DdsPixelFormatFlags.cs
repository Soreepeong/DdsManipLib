using System;

namespace DdsManipLib.DirectDrawSurface;

/// <summary>
/// Values which indicate what type of data is in the surface.
/// </summary>
[Flags]
public enum DdsPixelFormatFlags : uint {
    /// <summary>
    /// Texture contains alpha data.
    /// </summary>
    /// <remarks>
    /// <see cref="DdsPixelFormat.ABitMask"/> contains valid data.
    /// </remarks>
    AlphaPixels = 0x1,

    /// <summary>
    /// Used in some older DDS files for alpha channel only uncompressed data.
    /// </summary>
    /// <remarks>
    /// <see cref="DdsPixelFormat.RgbBitCount"/> contains the alpha channel bit count.<br />
    /// <see cref="DdsPixelFormat.ABitMask"/> contains valid data.
    /// </remarks>
    Alpha = 0x2,

    /// <summary>
    /// Texture contains compressed RGB data.
    /// </summary>
    /// <remarks>
    /// <see cref="DdsPixelFormat.FourCc"/> contains valid data.
    /// </remarks>
    FourCc = 0x4,
    
    /// <summary>
    /// Texture is stored in a palletizized format.
    /// </summary>
    /// <remarks>From https://ffmpeg.org/doxygen/trunk/dds_8c_source.html</remarks>
    Palette = 0x20,
    
    /// <summary>
    /// Texture contains uncompressed RGB data.
    /// </summary>
    /// <remarks>
    /// <see cref="DdsPixelFormat.RgbBitCount"/> and the RGB masks(<see cref="DdsPixelFormat.RBitMask"/>, <see cref="DdsPixelFormat.GBitMask"/>,
    /// <see cref="DdsPixelFormat.BBitMask"/>) contain valid data.
    /// </remarks>
    Rgb = 0x40,

    /// <summary>
    /// Used in some older DDS files for YUV uncompressed data.	
    /// </summary>
    /// <remarks>
    /// <see cref="DdsPixelFormat.RgbBitCount"/> contains the YUV bit count.<br />
    /// <see cref="DdsPixelFormat.RBitMask"/> contains the Y mask.<br />
    /// <see cref="DdsPixelFormat.GBitMask"/> contains the U mask.<br />
    /// <see cref="DdsPixelFormat.BBitMask"/> contains the V mask.
    /// </remarks>
    Yuv = 0x200,

    /// <summary>
    /// Used in some older DDS files for single channel color uncompressed data.
    /// </summary>
    /// <remarks>
    /// <see cref="DdsPixelFormat.RgbBitCount"/> contains the luminance channel bit count.<br />
    /// <see cref="DdsPixelFormat.RBitMask"/> contains the channel mask.<br />
    /// Can be combined with DDPF_ALPHAPIXELS for a two channel DDS file.
    /// </remarks>
    Luminance = 0x20000,

    /// <summary>
    /// Texture contains a normal map, and the Z (blue channel) value needs to be calculated.
    /// </summary>
    /// <remarks>From https://ffmpeg.org/doxygen/trunk/dds_8c_source.html</remarks>
    NormalMap = 0x80000000,

    /// <summary>
    /// Mask for all flags that will set <see cref="DdsPixelFormat.RgbBitCount"/> as a valid value.
    /// </summary>
    ValidRgbBitCountFieldMask = Alpha | Rgb | Yuv | Luminance,
}