using System;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

/// <summary>
/// How a channel is stored.
/// </summary>
[Flags]
public enum ChannelType : byte {
    /// <summary>
    /// The channel is not typed.
    /// </summary>
    /// <remarks>
    /// When using this format as pixel format conversion source or target, the resulting values are for arbitrary visual representation only.
    /// </remarks>
    Typeless = 0,

    /// <summary>
    /// The channel is signed.
    /// </summary>
    Signed = 1,

    /// <summary>
    /// The channel is unsigned.
    /// </summary>
    Unsigned = 2,

    /// <summary>
    /// The channel has an integer value.
    /// </summary>
    Integer = 4,

    /// <summary>
    /// The channel has a float value.
    /// </summary>
    FloatingPoint = 8,

    /// <summary>
    /// The channel value is normalized.
    /// </summary>
    Normalized = 16,

    /// <summary>
    /// The channel value should be interpreted in sRGB colorspace.
    /// </summary>
    Srgb = 32,

    /// <summary>
    /// Signed normalized integer format.
    /// </summary>
    /// <remarks>
    /// The format uses 2's complement to store values.<br />
    /// F32 values [-1f, 1f] linearly (evenly-spacedly) maps to [-(2<sup>n</sup> - 1), 2<sup>n</sup> - 1].<br />
    /// The minimum value -(1 &lt;&lt; n) is also interpreted as -1f.
    /// </remarks>
    Snorm = Signed | Normalized | Integer,

    /// <summary>
    /// Unsigned normalized integer format.
    /// </summary>
    /// <remarks>
    /// The format uses 2's complement to store values.<br />
    /// F32 values [0f, 1f] linearly (evenly-spacedly) maps to [0, 2<sup>n</sup> - 1].<br />
    /// </remarks>
    Unorm = Unsigned | Normalized | Integer,

    /// <summary>
    /// Unsigned normalized integer format in sRGB colorspace.
    /// </summary>
    /// <remarks>
    /// Same remarks from <see cref="Unorm"/> applies, but the spacing is not linear.<br />
    /// For complete detail, refer to the SRGB color standard, IEC 61996-2-1.
    /// </remarks>
    UnormSrgb = Unorm | Srgb,

    /// <summary>
    /// Signed integer format.
    /// </summary>
    /// <remarks>
    /// The format uses 2's complement to store values.<br />
    /// The values map directly to float values; out-of-range values will be clamped.
    /// </remarks>
    Sint = Signed | Integer,

    /// <summary>
    /// Unsigned integer format.
    /// </summary>
    /// <remarks>
    /// The format uses 2's complement to store values.<br />
    /// The values map directly to float values; out-of-range values will be clamped.
    /// </remarks>
    Uint = Unsigned | Integer,

    /// <summary>
    /// Standard FP32 format.
    /// </summary>
    F32 = FloatingPoint | Normalized,

    /// <summary>
    /// Standard FP16 format.
    /// </summary>
    F16 = Signed | FloatingPoint,

    /// <summary>
    /// Signed FP16 format: 1 sign bit + 5 exponent bits + 10 mantissa bits
    /// 
    /// See: https://learn.microsoft.com/en-us/windows/win32/direct3d11/bc6h-format
    /// </summary> 
    Sf16 = F16,

    /// <summary>
    /// Custom unsigned FP16 format: 5 exponent bits + 11 mantissa bits
    /// 
    /// See: https://learn.microsoft.com/en-us/windows/win32/direct3d11/bc6h-format
    /// </summary> 
    Uf16 = Unsigned | FloatingPoint,
}
