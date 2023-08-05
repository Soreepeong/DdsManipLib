using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using DdsManipLib.DirectDrawSurface.PixelFormats.BlockPixelFormats;
using DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

/// <summary>
/// Miscellaneous utility functions on <see cref="IPixelFormat"/>.
/// </summary>
public static class PixelFormatUtilities {
    /// <summary>
    /// Matrix for converting RGB to BT.601 YCbCr.
    /// </summary>
    public static readonly Matrix4x4 RgbToYuvBt601 =
        new(0.299f, 0.587f, 0.114f, 0f, -0.168736f, -0.331264f, 0f, 0f, 0f, -0.418688f, 0.081312f, 0f, 0f, 0f, 0f, 1f);

    /// <summary>
    /// Matrix for converting BT.601 YCbCr to RGB.
    /// </summary>
    public static readonly Matrix4x4 YuvToRgbBt601 =
        new(1f, 0f, 1.042f, 0f, 1f, -0.344136f, -0.714136f, 0f, 1f, 1.772f, 0f, 0f, 0f, 0f, 0f, 1f);

    /// <summary>
    /// Matrix for converting RGB to BT.709 YCbCr.
    /// </summary>
    public static readonly Matrix4x4 RgbToYuvBt709 =
        new(0.2126f, 0.7152f, 0.0722f, 0f, -0.1146f, -0.3854f, 0f, 0f, 0f, -0.4542f, -0.0458f, 0f, 0f, 0f, 0f, 1f);

    /// <summary>
    /// Matrix for converting BT.709 YCbCr to RGB.
    /// </summary>
    public static readonly Matrix4x4 YuvToRgbBt709 =
        new(1f, 0f, 1.5748f, 0f, 1f, -0.1873f, -0.4681f, 0f, 1f, 1.8556f, 0f, 0f, 0f, 0f, 0f, 1f);

    private static readonly IReadOnlyDictionary<Tuple<AlphaType, DxgiFormat>, IPixelFormat> DxgiToPixelFormatMap;
    private static readonly IReadOnlyDictionary<DdsFourCc, IPixelFormat> FourCcToPixelFormatMap;
    private static readonly IReadOnlyDictionary<DdsPixelFormat, IPixelFormat> DdspfToPixelFormatMap;

    static PixelFormatUtilities() {
        const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        var type = typeof(IPixelFormat);
        var opaquePixelFormats = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p) && p is {IsClass: true, IsAbstract: false})
            .Select(p => p.GetConstructor(bindingFlags, Array.Empty<Type>())?.Invoke(null) as IPixelFormat)
            .Where(p => p is not null)
            .Select(p => p!)
            .ToArray();
        var alphaPixelFormats = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p) && p is {IsClass: true, IsAbstract: false})
            .SelectMany(p => new[] {
                p.GetConstructor(bindingFlags, new[] {typeof(AlphaType)})?.Invoke(new object[] {AlphaType.Straight}) as IPixelFormat,
                p.GetConstructor(bindingFlags, new[] {typeof(AlphaType)})?.Invoke(new object[] {AlphaType.Premultiplied}) as IPixelFormat,
                p.GetConstructor(bindingFlags, new[] {typeof(AlphaType)})?.Invoke(new object[] {AlphaType.Custom}) as IPixelFormat
            })
            .Where(p => p is not null)
            .Select(p => p!)
            .ToArray();
        DxgiToPixelFormatMap = opaquePixelFormats
            .Concat(alphaPixelFormats)
            .Where(x => x.DxgiFormat != DxgiFormat.Unknown)
            .ToImmutableDictionary(x => Tuple.Create(x.AlphaType, x.DxgiFormat), x => x);
        FourCcToPixelFormatMap = opaquePixelFormats
            .Concat(alphaPixelFormats.Where(x => x.AlphaType == AlphaType.Straight))
            .Where(x => x.FourCc != DdsFourCc.Unknown)
            .ToImmutableDictionary(x => x.FourCc, x => x);
        DdspfToPixelFormatMap = opaquePixelFormats
            .Concat(alphaPixelFormats.Where(x => x.AlphaType == AlphaType.Straight))
            .Where(x => x.DdsPixelFormat is {HasValidFormat: true, UseDxt10Header: false})
            .ToImmutableDictionary(x => x.DdsPixelFormat, x => x);
    }

    /// <summary>
    /// Attempt to translate the given <see cref="DxgiFormat"/> to a corresponding <see cref="IPixelFormat"/>.
    /// </summary>
    /// <param name="dxgiFormat">The DXGI format.</param>
    /// <param name="alphaType">Alpha type to query. Multiple values can be set, in which case, the enum value with the least value will be tested first.</param>
    /// <param name="pixelFormat">Corresponding <see cref="IPixelFormat"/> if found. Otherwise, the value is undefined.</param>
    /// <returns>True if the corresponding <see cref="IPixelFormat"/> was found.</returns>
    public static bool TryGetPixelFormat(this DxgiFormat dxgiFormat, AlphaType alphaType, [MaybeNullWhen(false)] out IPixelFormat pixelFormat) {
        if (alphaType is AlphaType.None)
            return DxgiToPixelFormatMap.TryGetValue(Tuple.Create(alphaType, dxgiFormat), out pixelFormat);

        foreach (var e in Enum.GetValues<AlphaType>()) {
            if ((e & alphaType) != 0 && DxgiToPixelFormatMap.TryGetValue(Tuple.Create(e, dxgiFormat), out pixelFormat))
                return true;
        }

        pixelFormat = default;
        return false;
    }

    /// <summary>
    /// Attempt to translate the given <see cref="DdsPixelFormat"/> to a corresponding <see cref="IPixelFormat"/>.
    /// </summary>
    /// <param name="ddspf">The DDS pixel format.</param>
    /// <param name="pixelFormat">Corresponding <see cref="IPixelFormat"/> if found. Otherwise, the value is undefined.</param>
    /// <returns>True if the corresponding <see cref="IPixelFormat"/> was found.</returns>
    public static bool TryGetPixelFormat(this DdsPixelFormat ddspf, [MaybeNullWhen(false)] out IPixelFormat pixelFormat) {
        if (DdspfToPixelFormatMap.TryGetValue(ddspf, out pixelFormat))
            return true;

        if (ddspf.Flags.HasFlag(DdsPixelFormatFlags.FourCc))
            return FourCcToPixelFormatMap.TryGetValue(ddspf.FourCc, out pixelFormat);

        if (ddspf.RgbBitCount is <= 0 or > 32)
            return false;

        if (!DeconstructMask(ddspf.RBitMask, out var rshift, out var rbits))
            return false;
        if (!DeconstructMask(ddspf.GBitMask, out var gshift, out var gbits))
            return false;
        if (!DeconstructMask(ddspf.BBitMask, out var bshift, out var bbits))
            return false;
        if (!DeconstructMask(ddspf.Flags.HasFlag(DdsPixelFormatFlags.AlphaPixels) ? ddspf.ABitMask : 0u, out var ashift, out var abits))
            return false;

        var maxBits = Math.Max(rbits, Math.Max(gbits, Math.Max(bbits, abits)));

        if (ddspf.Flags.HasFlag(DdsPixelFormatFlags.Rgb)) {
            if (abits != 0 && (rbits | gbits | bbits) != 0)
                pixelFormat = maxBits switch {
                    <= 8 => new DdspfRxGxBxAxUNormPixelFormat<byte>(ddspf.RgbBitCount, rshift, rbits, gshift, gbits, bshift, bbits, ashift, abits),
                    <= 16 => new DdspfRxGxBxAxUNormPixelFormat<ushort>(ddspf.RgbBitCount, rshift, rbits, gshift, gbits, bshift, bbits, ashift, abits),
                    <= 32 => new DdspfRxGxBxAxUNormPixelFormat<uint>(ddspf.RgbBitCount, rshift, rbits, gshift, gbits, bshift, bbits, ashift, abits),
                    _ => throw new InvalidOperationException(),
                };
            else if (abits != 0)
                pixelFormat = maxBits switch {
                    <= 8 => new DdspfAxUNormPixelFormat<byte>(ddspf.RgbBitCount, ashift, abits),
                    <= 16 => new DdspfAxUNormPixelFormat<ushort>(ddspf.RgbBitCount, ashift, abits),
                    <= 32 => new DdspfAxUNormPixelFormat<uint>(ddspf.RgbBitCount, ashift, abits),
                    _ => throw new InvalidOperationException(),
                };
            else if (bbits != 0)
                pixelFormat = maxBits switch {
                    <= 8 => new DdspfRxGxBxUNormPixelFormat<byte>(ddspf.RgbBitCount, rshift, rbits, gshift, gbits, bshift, bbits),
                    <= 16 => new DdspfRxGxBxUNormPixelFormat<ushort>(ddspf.RgbBitCount, rshift, rbits, gshift, gbits, bshift, bbits),
                    <= 32 => new DdspfRxGxBxUNormPixelFormat<uint>(ddspf.RgbBitCount, rshift, rbits, gshift, gbits, bshift, bbits),
                    _ => throw new InvalidOperationException(),
                };
            else if (rbits != 0 && gbits != 0)
                pixelFormat = maxBits switch {
                    <= 8 => new DdspfRxGxUNormPixelFormat<byte>(ddspf.RgbBitCount, rshift, rbits, gshift, gbits),
                    <= 16 => new DdspfRxGxUNormPixelFormat<ushort>(ddspf.RgbBitCount, rshift, rbits, gshift, gbits),
                    <= 32 => new DdspfRxGxUNormPixelFormat<uint>(ddspf.RgbBitCount, rshift, rbits, gshift, gbits),
                    _ => throw new InvalidOperationException(),
                };
            else if (gbits != 0)
                pixelFormat = maxBits switch {
                    <= 8 => new DdspfGxUNormPixelFormat<byte>(ddspf.RgbBitCount, gshift, gbits),
                    <= 16 => new DdspfGxUNormPixelFormat<ushort>(ddspf.RgbBitCount, gshift, gbits),
                    <= 32 => new DdspfGxUNormPixelFormat<uint>(ddspf.RgbBitCount, gshift, gbits),
                    _ => throw new InvalidOperationException(),
                };
            else if (rbits != 0)
                pixelFormat = maxBits switch {
                    <= 8 => new DdspfGxUNormPixelFormat<byte>(ddspf.RgbBitCount, gshift, gbits),
                    <= 16 => new DdspfGxUNormPixelFormat<ushort>(ddspf.RgbBitCount, gshift, gbits),
                    <= 32 => new DdspfGxUNormPixelFormat<uint>(ddspf.RgbBitCount, gshift, gbits),
                    _ => throw new InvalidOperationException(),
                };
            else
                return false;
            return true;
        }

        if (ddspf.Flags.HasFlag(DdsPixelFormatFlags.Yuv)) {
            if (abits != 0 && (rbits | gbits | bbits) != 0)
                pixelFormat = maxBits switch {
                    <= 8 => new DdspfYxUxVxAxPixelFormat<byte>(ddspf.RgbBitCount, rshift, rbits, gshift, gbits, bshift, bbits, ashift, abits),
                    <= 16 => new DdspfYxUxVxAxPixelFormat<ushort>(ddspf.RgbBitCount, rshift, rbits, gshift, gbits, bshift, bbits, ashift, abits),
                    <= 32 => new DdspfYxUxVxAxPixelFormat<uint>(ddspf.RgbBitCount, rshift, rbits, gshift, gbits, bshift, bbits, ashift, abits),
                    _ => throw new InvalidOperationException(),
                };
            else if (abits != 0)
                pixelFormat = maxBits switch {
                    <= 8 => new DdspfAxUNormPixelFormat<byte>(ddspf.RgbBitCount, ashift, abits),
                    <= 16 => new DdspfAxUNormPixelFormat<ushort>(ddspf.RgbBitCount, ashift, abits),
                    <= 32 => new DdspfAxUNormPixelFormat<uint>(ddspf.RgbBitCount, ashift, abits),
                    _ => throw new InvalidOperationException(),
                };
            else if ((rbits | gbits | bbits) != 0)
                pixelFormat = maxBits switch {
                    <= 8 => new DdspfYxUxVxPixelFormat<byte>(ddspf.RgbBitCount, rshift, rbits, gshift, gbits, bshift, bbits),
                    <= 16 => new DdspfYxUxVxPixelFormat<ushort>(ddspf.RgbBitCount, rshift, rbits, gshift, gbits, bshift, bbits),
                    <= 32 => new DdspfYxUxVxPixelFormat<uint>(ddspf.RgbBitCount, rshift, rbits, gshift, gbits, bshift, bbits),
                    _ => throw new InvalidOperationException(),
                };
            else
                return false;
            return true;
        }

        if (ddspf.Flags.HasFlag(DdsPixelFormatFlags.Luminance)) {
            if (abits != 0 && rbits != 0)
                pixelFormat = maxBits switch {
                    <= 8 => new DdspfLxAxUNormPixelFormat<byte>(ddspf.RgbBitCount, rshift, rbits, ashift, abits),
                    <= 16 => new DdspfLxAxUNormPixelFormat<ushort>(ddspf.RgbBitCount, rshift, rbits, ashift, abits),
                    <= 32 => new DdspfLxAxUNormPixelFormat<uint>(ddspf.RgbBitCount, rshift, rbits, ashift, abits),
                    _ => throw new InvalidOperationException(),
                };
            else if (abits != 0)
                pixelFormat = maxBits switch {
                    <= 8 => new DdspfAxUNormPixelFormat<byte>(ddspf.RgbBitCount, ashift, abits),
                    <= 16 => new DdspfAxUNormPixelFormat<ushort>(ddspf.RgbBitCount, ashift, abits),
                    <= 32 => new DdspfAxUNormPixelFormat<uint>(ddspf.RgbBitCount, ashift, abits),
                    _ => throw new InvalidOperationException(),
                };
            else if (rbits != 0)
                pixelFormat = maxBits switch {
                    <= 8 => new DdspfLxUNormPixelFormat<byte>(ddspf.RgbBitCount, rshift, rbits),
                    <= 16 => new DdspfLxUNormPixelFormat<ushort>(ddspf.RgbBitCount, rshift, rbits),
                    <= 32 => new DdspfLxUNormPixelFormat<uint>(ddspf.RgbBitCount, rshift, rbits),
                    _ => throw new InvalidOperationException(),
                };
            else
                return false;
            return true;
        }

        if (ddspf.Flags.HasFlag(DdsPixelFormatFlags.Alpha)) {
            pixelFormat = maxBits switch {
                <= 8 => new DdspfAxUNormPixelFormat<byte>(ddspf.RgbBitCount, ashift, abits),
                <= 16 => new DdspfAxUNormPixelFormat<ushort>(ddspf.RgbBitCount, ashift, abits),
                <= 32 => new DdspfAxUNormPixelFormat<uint>(ddspf.RgbBitCount, ashift, abits),
                _ => throw new InvalidOperationException(),
            };
            return true;
        }

        return false;
    }

    /// <summary>
    /// Deconstruct a bitmask into bit shift offset and number of bits.
    /// </summary>
    /// <param name="mask">The bitmask to deconstruct.</param>
    /// <param name="shift">Determined bit shift offset.</param>
    /// <param name="bits">Determined number of bits.</param>
    /// <returns>True if the mask can be reconstructed from the determined bit shift offset and number of bits.</returns>
    public static bool DeconstructMask(uint mask, out int shift, out int bits) {
        shift = BitOperations.TrailingZeroCount(mask);
        bits = BitOperations.PopCount(mask);
        var mask2 = ((1u << bits) - 1u) << shift;
        return mask == mask2;
    }

    /// <summary>
    /// Translate a raw value into SInt value. 
    /// </summary>
    /// <param name="rawValue">The raw value.</param>
    /// <param name="bits">Number of bits used for representing the SInt value.</param>
    /// <returns>The translated SInt value, fully bit-extended.</returns>
    public static T RawToUInt<T>(T rawValue, int bits) where T : unmanaged, IBinaryInteger<T> =>
        rawValue & ((T.One << bits) - T.One);

    /// <summary>
    /// Translate a raw value into SInt value. 
    /// </summary>
    /// <param name="rawValue">The raw value.</param>
    /// <param name="bits">Number of bits used for representing the SInt value.</param>
    /// <returns>The translated SInt value, fully bit-extended.</returns>
    public static T RawToSInt<T>(T rawValue, int bits) where T : unmanaged, IBinaryInteger<T> {
        var signMask = T.One << (bits - 1);
        var numberMask = signMask - T.One;
        return (rawValue & signMask) == T.Zero
            ? rawValue & numberMask
            : ~numberMask | (rawValue & numberMask);
    }

    /// <summary>
    /// Translate a raw value into UNorm value. 
    /// </summary>
    /// <param name="rawValue">The raw value.</param>
    /// <param name="bits">Number of bits used for representing the UNorm value.</param>
    /// <returns>The translated UNorm value, fully bit-extended.</returns>
    public static T RawToUNorm<T>(T rawValue, int bits) where T : unmanaged, IBinaryInteger<T> {
        var tbits = Unsafe.SizeOf<T>() * 8;
        rawValue <<= tbits - bits;
        for (; bits < tbits; bits *= 2)
            rawValue |= rawValue >>> bits;
        return T.CreateSaturating(rawValue);
    }

    /// <summary>
    /// Translate a raw value into SNorm value. 
    /// </summary>
    /// <param name="rawValue">The raw value.</param>
    /// <param name="bits">Number of bits used for representing the SNorm value.</param>
    /// <returns>The translated SNorm value, fully bit-extended.</returns>
    public static T RawToSNorm<T>(T rawValue, int bits) where T : unmanaged, IBinaryInteger<T> {
        var tbits = Unsafe.SizeOf<T>() * 8;
        var signMask = T.One << (bits - 1);
        var numberUnorm = RawToUNorm(rawValue, bits - 1);
        return (rawValue & signMask) == T.Zero
            ? numberUnorm >>> 1
            : T.One << (tbits - 1) | (numberUnorm >>> 1);
    }

    /// <summary>
    /// Translate a float value into UNorm value. 
    /// </summary>
    /// <param name="floatValue">The float value.</param>
    /// <param name="bits">Number of bits used for representing the UNorm value.</param>
    /// <returns>The translated UNorm value, fully bit-extended.</returns>
    public static T FloatToUNormRaw<T>(float floatValue, int bits) where T : unmanaged, IBinaryInteger<T> {
        if (floatValue < 0 || float.IsNaN(floatValue) || float.IsNegativeInfinity(floatValue))
            return default;
        if (floatValue >= 1 || float.IsPositiveInfinity(floatValue))
            return (T.One << bits) - T.One;
        return T.Clamp(T.CreateTruncating(floatValue * MathF.Pow(2, bits)), T.Zero, (T.One << bits) - T.One);
    }

    /// <summary>
    /// Translate a float value into SNorm value. 
    /// </summary>
    /// <param name="floatValue">The float value.</param>
    /// <param name="bits">Number of bits used for representing the SNorm value.</param>
    /// <returns>The translated SNorm value, fully bit-extended.</returns>
    public static T FloatToSNormRaw<T>(float floatValue, int bits) where T : unmanaged, IBinaryInteger<T> {
        if (float.IsNaN(floatValue))
            return default;
        if (floatValue <= -1 || float.IsNegativeInfinity(floatValue))
            return T.One << (bits - 1);
        if (floatValue >= 1 || float.IsPositiveInfinity(floatValue))
            return (T.One << (bits - 1)) - T.One;
        if (floatValue > 0)
            return T.Clamp(T.CreateTruncating(floatValue * MathF.Pow(2, bits - 1)), T.Zero, (T.One << (bits - 1)) - T.One);
        if (floatValue < 0)
            return T.Clamp(T.CreateTruncating(-floatValue * MathF.Pow(2, bits - 1)), T.Zero, (T.One << (bits - 1)) - T.One) | (T.One << (bits - 1));
        return default;
    }

    /// <summary>
    /// Translate a UInt value into a raw value.
    /// </summary>
    /// <param name="value">The UInt value.</param>
    /// <param name="bits">Number of bits to use for UInt representation.</param>
    /// <returns>The translated raw value.</returns>
    /// <remarks>The value will be clamped.</remarks>
    public static T UIntToRaw<T>(T value, int bits) where T : unmanaged, IBinaryInteger<T> {
        var numberMask = (T.One << bits) - T.One;
        return (value & ~numberMask) != T.Zero // above maximum value?
            ? numberMask
            : value;
    }

    /// <summary>
    /// Translate a SInt value into a raw value.
    /// </summary>
    /// <param name="value">The SInt value.</param>
    /// <param name="bits">Number of bits to use for SInt representation.</param>
    /// <returns>The translated raw value.</returns>
    /// <remarks>The value will be clamped.</remarks>
    public static T SIntToRaw<T>(T value, int bits) where T : unmanaged, IBinaryInteger<T> {
        var tbits = Unsafe.SizeOf<T>() * 8;
        var sign = value >>> tbits;
        var overflow = value << 1 >>> 1 >>> bits;
        var numberMask = (T.One << (bits - 1)) - T.One;
        var n = value & numberMask;

        return sign == T.Zero // is it positive?
            ? overflow != T.Zero // above maximum value?
                ? numberMask
                : n
            : overflow != (T.One << (tbits - 1 - bits)) - T.One // below minimum value?
                ? (T.One << bits) - T.One
                : (T.One << (bits - 1)) | n;
    }

    /// <summary>
    /// Translate a UNorm value into a raw value.
    /// </summary>
    /// <param name="value">The UNorm value.</param>
    /// <param name="bits">Number of bits to use for UNorm representation.</param>
    /// <returns>The translated raw value.</returns>
    /// <remarks>The value will be clamped.</remarks>
    public static T UNormToRaw<T>(T value, int bits) where T : unmanaged, IBinaryInteger<T> {
        var tbits = Unsafe.SizeOf<T>() * 8;
        return value >> (tbits - bits);
    }

    /// <summary>
    /// Translate a SNorm value into a raw value.
    /// </summary>
    /// <param name="value">The SNorm value.</param>
    /// <param name="bits">Number of bits to use for SNorm representation.</param>
    /// <returns>The translated raw value.</returns>
    /// <remarks>The value will be clamped.</remarks>
    public static T SNormToRaw<T>(T value, int bits) where T : unmanaged, IBinaryInteger<T> {
        var tbits = Unsafe.SizeOf<T>() * 8;
        var uv = value << 1 >> 1 >> (tbits - bits);
        return (value & (T.One << (tbits - 1))) == T.Zero
            ? uv
            : uv | (T.One << (bits - 1));
    }

    /// <summary>
    /// Translate the given byte span into another, from a pixel format to another.
    /// </summary>
    /// <param name="sourcePixelFormat">Source pixel format.</param>
    /// <param name="sourceSpan">Byte span containing source data.</param>
    /// <param name="width">Width of the image.</param>
    /// <param name="height">Height of the image.</param>
    /// <param name="targetPixelFormat">Target pixel format.</param>
    /// <param name="targetSpan">Byte span for translated data.</param>
    /// <param name="scratchBuffer">If a temporary byte array is required, the given byte array may be used as a scratch buffer.</param>
    /// <returns>Whether the operation was supported and successful.</returns>
    public static bool ConvertToAuto(
        this IPixelFormat sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IPixelFormat targetPixelFormat,
        Span<byte> targetSpan,
        ref byte[]? scratchBuffer) {
        switch (sourcePixelFormat) {
            case var _ when Equals(sourcePixelFormat, targetPixelFormat):
                sourceSpan = sourceSpan[..sourcePixelFormat.CalculateLinearSize(width, height)];
                targetSpan = targetSpan[..targetPixelFormat.CalculateLinearSize(width, height)];
                sourceSpan.CopyTo(targetSpan);
                return true;
            case IRawPixelFormat rawS: {
                switch (targetPixelFormat) {
                    case IRawPixelFormat rawT:
                        return ConvertTo(rawS, sourceSpan, width, height, rawT, targetSpan);
                    case IBlockPixelFormat blockT:
                        if (blockT.SupportsRawPixelFormat(rawS)) {
                            blockT.Compress(rawS, sourceSpan, width, height, targetSpan);
                        } else {
                            var rawpf = blockT.SuggestedRawPixelFormat;
                            var tmplen = rawpf.CalculateLinearSize(width, height);
                            if (scratchBuffer is null || scratchBuffer.Length < tmplen)
                                scratchBuffer = new byte[tmplen];
                            if (!ConvertTo(rawS, sourceSpan, width, height, rawpf, scratchBuffer))
                                return false;
                            blockT.Compress(rawpf, scratchBuffer, width, height, targetSpan);
                        }

                        return true;
                    default:
                        return false;
                }
            }
            case IBlockPixelFormat blockS when targetPixelFormat is IRawPixelFormat rawT && blockS.SupportsRawPixelFormat(rawT):
                blockS.Decompress(rawT, sourceSpan, width, height, targetSpan);
                return true;
            case IBlockPixelFormat blockS: {
                switch (targetPixelFormat) {
                    case IRawPixelFormat rawT:
                        if (blockS.SupportsRawPixelFormat(rawT)) {
                            blockS.Decompress(rawT, sourceSpan, width, height, targetSpan);
                        } else {
                            var rawpf = blockS.SuggestedRawPixelFormat;
                            var tmplen = rawpf.CalculateLinearSize(width, height);
                            if (scratchBuffer is null || scratchBuffer.Length < tmplen)
                                scratchBuffer = new byte[tmplen];

                            blockS.Decompress(rawpf, sourceSpan, width, height, scratchBuffer);
                            return ConvertTo(rawpf, scratchBuffer, width, height, rawT, targetSpan);
                        }

                        return true;
                    case IBlockPixelFormat blockT: {
                        var rawpf = blockS.SuggestedRawPixelFormat;
                        if (!blockT.SupportsRawPixelFormat(rawpf))
                            return false;

                        var tmplen = rawpf.CalculateLinearSize(width, height);
                        if (scratchBuffer is null || scratchBuffer.Length < tmplen)
                            scratchBuffer = new byte[tmplen];

                        blockS.Decompress(rawpf, sourceSpan, width, height, scratchBuffer);
                        blockT.Compress(rawpf, scratchBuffer, width, height, targetSpan);

                        return true;
                    }
                    default:
                        return false;
                }
            }
            default:
                return false;
        }
    }

    /// <summary>
    /// Translate the given byte span into another, from a pixel format to another.
    /// </summary>
    /// <param name="sourcePixelFormat">Source pixel format.</param>
    /// <param name="sourceSpan">Byte span containing source data.</param>
    /// <param name="width">Width of the image.</param>
    /// <param name="height">Height of the image.</param>
    /// <param name="targetPixelFormat">Target pixel format.</param>
    /// <param name="targetSpan">Byte span for translated data.</param>
    /// <returns>Whether the operation was supported and successful.</returns>
    /// <remarks>Consider using <see cref="ConvertToAuto(DdsManipLib.DirectDrawSurface.PixelFormats.IPixelFormat,System.ReadOnlySpan{byte},int,int,DdsManipLib.DirectDrawSurface.PixelFormats.IPixelFormat,System.Span{byte},ref byte[])"/> if there are multiple images to convert.</remarks>
    public static bool ConvertToAuto(
        this IPixelFormat sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IPixelFormat targetPixelFormat,
        Span<byte> targetSpan) {
        byte[]? tmp = null;
        return ConvertToAuto(sourcePixelFormat, sourceSpan, width, height, targetPixelFormat, targetSpan, ref tmp);
    }

    /// <summary>
    /// Reset the target buffer.
    /// </summary>
    /// <param name="pixelFormat">Pixel format.</param>
    /// <param name="width">Width of the image.</param>
    /// <param name="height">Height of the image.</param>
    /// <param name="span">Byte span for the data.</param>
    public static void Clear(IRawPixelFormat pixelFormat, int width, int height, Span<byte> span) {
        var pitch = pixelFormat.CalculatePitch(width);
        var bpp = pixelFormat.BytesPerPixel;
        span = span[..(pitch * height)];
        for (; !span.IsEmpty; span = span[pitch..])
        for (var x = 0; x < width; x++)
            pixelFormat.ClearPixel(span[(x * bpp)..]);
    }

    private static bool ConvertTo(
        IRawPixelFormat sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawPixelFormat targetPixelFormat,
        Span<byte> targetSpan) {
        Clear(targetPixelFormat, width, height, targetSpan);
        
        switch (sourcePixelFormat) {
            case IRawRgbaPixelFormat rgba1 when targetPixelFormat is IRawRgbaPixelFormat rgba2:
                ConvertTo(rgba1, sourceSpan, width, height, rgba2, targetSpan);
                return true;
            case IRawRgbaPixelFormat rgba1 when targetPixelFormat is IRawYuvaPixelFormat yuva2:
                ConvertToAny(rgba1, sourceSpan, width, height, yuva2, targetSpan, (s, t) => yuva2.SetRgbaBt601(t, rgba1.GetRgba(s)));
                return true;
            case IRawRgbaPixelFormat rgba1 when targetPixelFormat is IRawLaPixelFormat la2:
                ConvertToAny(rgba1, sourceSpan, width, height, la2, targetSpan, (s, t) => la2.SetLa(t, new(rgba1.GetRgb(s).Average(), rgba1.GetAlpha(s))));
                return true;

            case IRawRgbPixelFormat rgb1 when targetPixelFormat is IRawRgbPixelFormat rgb2:
                ConvertTo(rgb1, sourceSpan, width, height, rgb2, targetSpan);
                return true;
            case IRawRgbPixelFormat rgb1 when targetPixelFormat is IRawYuvPixelFormat yuv2:
                ConvertToAny(rgb1, sourceSpan, width, height, yuv2, targetSpan, (s, t) => yuv2.SetRgbBt601(t, rgb1.GetRgb(s)));
                return true;
            case IRawRgbPixelFormat rgb1 when targetPixelFormat is IRawLPixelFormat l2:
                ConvertToAny(rgb1, sourceSpan, width, height, l2, targetSpan, (s, t) => l2.SetLuminance(t, rgb1.GetRgb(s).Average()));
                return true;

            case IRawRgPixelFormat rg1 when targetPixelFormat is IRawRgPixelFormat rg2:
                ConvertTo(rg1, sourceSpan, width, height, rg2, targetSpan);
                return true;
            case IRawRgPixelFormat rg1 when targetPixelFormat is IRawYuvPixelFormat l2:
                ConvertToAny(rg1, sourceSpan, width, height, l2, targetSpan, (s, t) => l2.SetYuv(t, new(rg1.GetRg(s).Average(), 0f, 0f)));
                return true;
            case IRawRgPixelFormat rg1 when targetPixelFormat is IRawLaPixelFormat la2:
                ConvertToAny(rg1, sourceSpan, width, height, la2, targetSpan, (s, t) => la2.SetLa(t, rg1.GetRg(s)));
                return true;
            case IRawRgPixelFormat rg1 when targetPixelFormat is IRawLPixelFormat l2:
                ConvertToAny(rg1, sourceSpan, width, height, l2, targetSpan, (s, t) => l2.SetLuminance(t, rg1.GetRg(s).Average()));
                return true;

            case IRawRPixelFormat r1 when targetPixelFormat is IRawRPixelFormat r2:
                ConvertTo(r1, sourceSpan, width, height, r2, targetSpan);
                return true;
            case IRawRPixelFormat r1 when targetPixelFormat is IRawGPixelFormat g2:
                ConvertToAny(r1, sourceSpan, width, height, g2, targetSpan, (s, t) => g2.SetGreen(t, r1.GetRed(s)));
                return true;
            case IRawRPixelFormat r1 when targetPixelFormat is IRawLPixelFormat l2:
                ConvertToAny(r1, sourceSpan, width, height, l2, targetSpan, (s, t) => l2.SetLuminance(t, r1.GetRed(s)));
                return true;
            case IRawRPixelFormat r1 when targetPixelFormat is IRawAPixelFormat a2:
                ConvertToAny(r1, sourceSpan, width, height, a2, targetSpan, (s, t) => a2.SetAlpha(t, r1.GetRed(s)));
                return true;

            case IRawGPixelFormat g1 when targetPixelFormat is IRawGPixelFormat g2:
                ConvertTo(g1, sourceSpan, width, height, g2, targetSpan);
                return true;
            case IRawGPixelFormat g1 when targetPixelFormat is IRawRPixelFormat r2:
                ConvertToAny(g1, sourceSpan, width, height, r2, targetSpan, (s, t) => r2.SetRed(t, g1.GetGreen(s)));
                return true;
            case IRawGPixelFormat g1 when targetPixelFormat is IRawLPixelFormat l2:
                ConvertToAny(g1, sourceSpan, width, height, l2, targetSpan, (s, t) => l2.SetLuminance(t, g1.GetGreen(s)));
                return true;
            case IRawGPixelFormat g1 when targetPixelFormat is IRawAPixelFormat a2:
                ConvertToAny(g1, sourceSpan, width, height, a2, targetSpan, (s, t) => a2.SetAlpha(t, g1.GetGreen(s)));
                return true;

            case IRawDsPixelFormat ds1 when targetPixelFormat is IRawDsPixelFormat ds2:
                ConvertTo(ds1, sourceSpan, width, height, ds2, targetSpan);
                return true;
            case IRawDsPixelFormat ds1 when targetPixelFormat is IRawRgPixelFormat rg2:
                ConvertTo(ds1, sourceSpan, width, height, rg2, targetSpan);
                return true;
            case IRawDsPixelFormat ds1 when targetPixelFormat is IRawGPixelFormat g2:
                ConvertTo(ds1, sourceSpan, width, height, g2, targetSpan);
                return true;
            case IRawDsPixelFormat ds1 when targetPixelFormat is IRawRPixelFormat r2:
                ConvertTo(ds1, sourceSpan, width, height, r2, targetSpan);
                return true;
            case IRawDsPixelFormat ds1 when targetPixelFormat is IRawLaPixelFormat la2:
                ConvertToAny(ds1, sourceSpan, width, height, la2, targetSpan, (s, t) => la2.SetLa(t, ds1.GetDs(s)));
                return true;

            case IRawDPixelFormat d1 when targetPixelFormat is IRawDPixelFormat d2:
                ConvertTo(d1, sourceSpan, width, height, d2, targetSpan);
                return true;
            case IRawDPixelFormat d1 when targetPixelFormat is IRawRPixelFormat r2:
                ConvertTo(d1, sourceSpan, width, height, r2, targetSpan);
                return true;
            case IRawDPixelFormat d1 when targetPixelFormat is IRawGPixelFormat g2:
                ConvertToAny(d1, sourceSpan, width, height, g2, targetSpan, (s, t) => g2.SetGreen(t, d1.GetDepth(s)));
                return true;
            case IRawDPixelFormat d1 when targetPixelFormat is IRawLPixelFormat l2:
                ConvertToAny(d1, sourceSpan, width, height, l2, targetSpan, (s, t) => l2.SetLuminance(t, d1.GetDepth(s)));
                return true;

            case IRawYuvaPixelFormat yuva1 when targetPixelFormat is IRawYuvaPixelFormat yuva2:
                ConvertToAny(yuva1, sourceSpan, width, height, yuva2, targetSpan, (s, t) => yuva2.SetYuva(t, yuva1.GetYuva(s)));
                return true;
            case IRawYuvaPixelFormat yuva1 when targetPixelFormat is IRawRgbaPixelFormat rgba2:
                ConvertToAny(yuva1, sourceSpan, width, height, rgba2, targetSpan, (s, t) => rgba2.SetRgba(t, yuva1.GetRgbaBt601(s)));
                return true;

            case IRawYuvPixelFormat yuv1 when targetPixelFormat is IRawYuvPixelFormat yuv2:
                ConvertToAny(yuv1, sourceSpan, width, height, yuv2, targetSpan, (s, t) => yuv2.SetYuv(t, yuv1.GetYuv(s)));
                return true;
            case IRawYuvPixelFormat yuv1 when targetPixelFormat is IRawRgbPixelFormat rgb2:
                ConvertToAny(yuv1, sourceSpan, width, height, rgb2, targetSpan, (s, t) => rgb2.SetRgb(t, yuv1.GetRgbBt601(s)));
                return true;

            case IRawLaPixelFormat la1 when targetPixelFormat is IRawRgbaPixelFormat rgba2:
                ConvertToAny(la1, sourceSpan, width, height, rgba2, targetSpan, (s, t) => rgba2.SetRgba(t, new(new(la1.GetLuminance(s)), la1.GetAlpha(s))));
                return true;
            case IRawLaPixelFormat la1 when targetPixelFormat is IRawYuvaPixelFormat yuva2:
                ConvertToAny(la1,
                    sourceSpan,
                    width,
                    height,
                    yuva2,
                    targetSpan,
                    (s, t) => yuva2.SetYuva(t, new(la1.GetLuminance(s), 0f, 0f, la1.GetAlpha(s))));
                return true;
            case IRawLaPixelFormat la1 when targetPixelFormat is IRawDsPixelFormat ds2:
                ConvertToAny(la1, sourceSpan, width, height, ds2, targetSpan, (s, t) => ds2.SetDs(t, la1.GetLa(s)));
                return true;

            case IRawLPixelFormat l1 when targetPixelFormat is IRawRgbPixelFormat rgb2:
                ConvertToAny(l1, sourceSpan, width, height, rgb2, targetSpan, (s, t) => rgb2.SetRgb(t, new(l1.GetLuminance(s))));
                return true;
            case IRawLPixelFormat l1 when targetPixelFormat is IRawRgPixelFormat rg2:
                ConvertToAny(l1, sourceSpan, width, height, rg2, targetSpan, (s, t) => rg2.SetRg(t, new(l1.GetLuminance(s))));
                return true;
            case IRawLPixelFormat l1 when targetPixelFormat is IRawRPixelFormat r2:
                ConvertToAny(l1, sourceSpan, width, height, r2, targetSpan, (s, t) => r2.SetRed(t, l1.GetLuminance(s)));
                return true;
            case IRawLPixelFormat l1 when targetPixelFormat is IRawGPixelFormat g2:
                ConvertToAny(l1, sourceSpan, width, height, g2, targetSpan, (s, t) => g2.SetGreen(t, l1.GetLuminance(s)));
                return true;
            case IRawLPixelFormat l1 when targetPixelFormat is IRawYuvPixelFormat yuv2:
                ConvertToAny(l1, sourceSpan, width, height, yuv2, targetSpan, (s, t) => yuv2.SetYuv(t, new(l1.GetLuminance(s), 0f, 0f)));
                return true;
            case IRawLPixelFormat l1 when targetPixelFormat is IRawDPixelFormat d2:
                ConvertToAny(l1, sourceSpan, width, height, d2, targetSpan, (s, t) => d2.SetDepth(t, l1.GetLuminance(s)));
                return true;
            case IRawLPixelFormat l1 when targetPixelFormat is IRawAPixelFormat a2:
                ConvertToAny(l1, sourceSpan, width, height, a2, targetSpan, (s, t) => a2.SetAlpha(t, l1.GetLuminance(s)));
                return true;

            case IRawAPixelFormat a1 when targetPixelFormat is IRawYuvaPixelFormat yuva2:
                ConvertToAny(a1, sourceSpan, width, height, yuva2, targetSpan, (s, t) => yuva2.SetYuva(t, new(1f, 0f, 0f, a1.GetAlpha(s))));
                return true;
            case IRawAPixelFormat a1 when targetPixelFormat is IRawYuvPixelFormat yuv2:
                ConvertToAny(a1, sourceSpan, width, height, yuv2, targetSpan, (s, t) => yuv2.SetYuv(t, new(a1.GetAlpha(s), 0f, 0f)));
                return true;
            case IRawAPixelFormat a1 when targetPixelFormat is IRawRgbaPixelFormat rgba2:
                ConvertToAny(a1, sourceSpan, width, height, rgba2, targetSpan, (s, t) => rgba2.SetRgba(t, new(1f, 1f, 1f, a1.GetAlpha(s))));
                return true;
            case IRawAPixelFormat a1 when targetPixelFormat is IRawRgbPixelFormat rgb2:
                ConvertToAny(a1, sourceSpan, width, height, rgb2, targetSpan, (s, t) => rgb2.SetRgb(t, new(a1.GetAlpha(s))));
                return true;
            case IRawAPixelFormat a1 when targetPixelFormat is IRawRgPixelFormat rg2:
                ConvertToAny(a1, sourceSpan, width, height, rg2, targetSpan, (s, t) => rg2.SetRg(t, new(a1.GetAlpha(s))));
                return true;
            case IRawAPixelFormat a1 when targetPixelFormat is IRawRPixelFormat r2:
                ConvertToAny(a1, sourceSpan, width, height, r2, targetSpan, (s, t) => r2.SetRed(t, a1.GetAlpha(s)));
                return true;
            case IRawAPixelFormat a1 when targetPixelFormat is IRawGPixelFormat g2:
                ConvertToAny(a1, sourceSpan, width, height, g2, targetSpan, (s, t) => g2.SetGreen(t, a1.GetAlpha(s)));
                return true;
            case IRawAPixelFormat a1 when targetPixelFormat is IRawLaPixelFormat la2:
                ConvertToAny(a1, sourceSpan, width, height, la2, targetSpan, (s, t) => la2.SetLa(t, new(1f, a1.GetAlpha(s))));
                return true;
            case IRawAPixelFormat a1 when targetPixelFormat is IRawLPixelFormat l2:
                ConvertToAny(a1, sourceSpan, width, height, l2, targetSpan, (s, t) => l2.SetLuminance(t, a1.GetAlpha(s)));
                return true;
            case IRawAPixelFormat a1 when targetPixelFormat is IRawAPixelFormat a2:
                ConvertTo(a1, sourceSpan, width, height, a2, targetSpan);
                return true;

            default:
                return false;
        }
    }

    private static void ConvertTo(
        IRawRPixelFormat sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawRPixelFormat targetPixelFormat,
        Span<byte> targetSpan) {
        switch (sourcePixelFormat) {
            case IRawRPixelFormat<byte> r1 when targetPixelFormat is IRawRPixelFormat<byte> r2:
                ConvertTo(r1, sourceSpan, width, height, r2, targetSpan);
                return;
            case IRawRPixelFormat<sbyte> r1 when targetPixelFormat is IRawRPixelFormat<sbyte> r2:
                ConvertTo(r1, sourceSpan, width, height, r2, targetSpan);
                return;
            case IRawRPixelFormat<ushort> r1 when targetPixelFormat is IRawRPixelFormat<ushort> r2:
                ConvertTo(r1, sourceSpan, width, height, r2, targetSpan);
                return;
            case IRawRPixelFormat<short> r1 when targetPixelFormat is IRawRPixelFormat<short> r2:
                ConvertTo(r1, sourceSpan, width, height, r2, targetSpan);
                return;
            case IRawRPixelFormat<uint> r1 when targetPixelFormat is IRawRPixelFormat<uint> r2:
                ConvertTo(r1, sourceSpan, width, height, r2, targetSpan);
                return;
            case IRawRPixelFormat<int> r1 when targetPixelFormat is IRawRPixelFormat<int> r2:
                ConvertTo(r1, sourceSpan, width, height, r2, targetSpan);
                return;
            case IRawRPixelFormat<Half> r1 when targetPixelFormat is IRawRPixelFormat<Half> r2:
                ConvertTo(r1, sourceSpan, width, height, r2, targetSpan);
                return;
            case IRawRPixelFormat<float> r1 when targetPixelFormat is IRawRPixelFormat<float> r2:
                ConvertTo(r1, sourceSpan, width, height, r2, targetSpan);
                return;
        }

        var sourcePitch = sourcePixelFormat.CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = sourcePixelFormat.BytesPerPixel;
        var targetBpp = targetPixelFormat.BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..])
        for (var x = 0; x < width; x++)
            targetPixelFormat.SetRed(targetSpan[(x * targetBpp)..], sourcePixelFormat.GetRed(sourceSpan[(x * sourceBpp)..]));
    }

    private static void ConvertTo(
        IRawRPixelFormat sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawDsPixelFormat targetPixelFormat,
        Span<byte> targetSpan) {
        var sourcePitch = sourcePixelFormat.CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = sourcePixelFormat.BytesPerPixel;
        var targetBpp = targetPixelFormat.BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..])
        for (var x = 0; x < width; x++)
            targetPixelFormat.SetStencil(targetSpan[(x * targetBpp)..], sourcePixelFormat.GetRed(sourceSpan[(x * sourceBpp)..]));
    }

    private static void ConvertTo<T>(
        IRawRPixelFormat<T> sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawRPixelFormat<T> targetPixelFormat,
        Span<byte> targetSpan)
        where T : unmanaged, IBinaryNumber<T> {
        var sourcePitch = sourcePixelFormat.CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = sourcePixelFormat.BytesPerPixel;
        var targetBpp = targetPixelFormat.BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..])
        for (var x = 0; x < width; x++)
            targetPixelFormat.SetRed(targetSpan[(x * targetBpp)..], sourcePixelFormat.GetRedTyped(sourceSpan[(x * sourceBpp)..]));
    }

    private static void ConvertTo<T, TDepth>(
        IRawGPixelFormat<T> sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawDsPixelFormat<TDepth, T> targetPixelFormat,
        Span<byte> targetSpan)
        where T : unmanaged, IBinaryNumber<T>
        where TDepth : unmanaged, IBinaryNumber<TDepth> {
        var sourcePitch = sourcePixelFormat.CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = sourcePixelFormat.BytesPerPixel;
        var targetBpp = targetPixelFormat.BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..])
        for (var x = 0; x < width; x++)
            targetPixelFormat.SetStencil(targetSpan[(x * targetBpp)..], sourcePixelFormat.GetGreenTyped(sourceSpan[(x * sourceBpp)..]));
    }

    private static void ConvertTo(
        IRawGPixelFormat sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawGPixelFormat targetPixelFormat,
        Span<byte> targetSpan) {
        var sourcePitch = sourcePixelFormat.CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = sourcePixelFormat.BytesPerPixel;
        var targetBpp = targetPixelFormat.BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..])
        for (var x = 0; x < width; x++)
            targetPixelFormat.SetGreen(targetSpan[(x * targetBpp)..], sourcePixelFormat.GetGreen(sourceSpan[(x * sourceBpp)..]));
    }

    private static void ConvertTo(
        IRawGPixelFormat sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawDPixelFormat targetPixelFormat,
        Span<byte> targetSpan) {
        var sourcePitch = sourcePixelFormat.CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = sourcePixelFormat.BytesPerPixel;
        var targetBpp = targetPixelFormat.BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..])
        for (var x = 0; x < width; x++)
            targetPixelFormat.SetDepth(targetSpan[(x * targetBpp)..], sourcePixelFormat.GetGreen(sourceSpan[(x * sourceBpp)..]));
    }

    private static void ConvertTo<T>(
        IRawGPixelFormat<T> sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawGPixelFormat<T> targetPixelFormat,
        Span<byte> targetSpan)
        where T : unmanaged, IBinaryNumber<T> {
        switch (sourcePixelFormat) {
            case IRawGPixelFormat<byte> g1 when targetPixelFormat is IRawGPixelFormat<byte> g2:
                ConvertTo(g1, sourceSpan, width, height, g2, targetSpan);
                return;
            case IRawGPixelFormat<sbyte> g1 when targetPixelFormat is IRawGPixelFormat<sbyte> g2:
                ConvertTo(g1, sourceSpan, width, height, g2, targetSpan);
                return;
            case IRawGPixelFormat<ushort> g1 when targetPixelFormat is IRawGPixelFormat<ushort> g2:
                ConvertTo(g1, sourceSpan, width, height, g2, targetSpan);
                return;
            case IRawGPixelFormat<short> g1 when targetPixelFormat is IRawGPixelFormat<short> g2:
                ConvertTo(g1, sourceSpan, width, height, g2, targetSpan);
                return;
            case IRawGPixelFormat<uint> g1 when targetPixelFormat is IRawGPixelFormat<uint> g2:
                ConvertTo(g1, sourceSpan, width, height, g2, targetSpan);
                return;
            case IRawGPixelFormat<int> g1 when targetPixelFormat is IRawGPixelFormat<int> g2:
                ConvertTo(g1, sourceSpan, width, height, g2, targetSpan);
                return;
            case IRawGPixelFormat<Half> g1 when targetPixelFormat is IRawGPixelFormat<Half> g2:
                ConvertTo(g1, sourceSpan, width, height, g2, targetSpan);
                return;
            case IRawGPixelFormat<float> g1 when targetPixelFormat is IRawGPixelFormat<float> g2:
                ConvertTo(g1, sourceSpan, width, height, g2, targetSpan);
                return;
        }

        var sourcePitch = sourcePixelFormat.CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = sourcePixelFormat.BytesPerPixel;
        var targetBpp = targetPixelFormat.BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..])
        for (var x = 0; x < width; x++)
            targetPixelFormat.SetGreen(targetSpan[(x * targetBpp)..], sourcePixelFormat.GetGreenTyped(sourceSpan[(x * sourceBpp)..]));
    }

    private static void ConvertTo<T>(
        IRawGPixelFormat<T> sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawDPixelFormat<T> targetPixelFormat,
        Span<byte> targetSpan)
        where T : unmanaged, IBinaryNumber<T> {
        var sourcePitch = sourcePixelFormat.CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = sourcePixelFormat.BytesPerPixel;
        var targetBpp = targetPixelFormat.BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..])
        for (var x = 0; x < width; x++)
            targetPixelFormat.SetDepth(targetSpan[(x * targetBpp)..], sourcePixelFormat.GetGreenTyped(sourceSpan[(x * sourceBpp)..]));
    }

    private static void ConvertTo(
        IRawRgPixelFormat sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawRgPixelFormat targetPixelFormat,
        Span<byte> targetSpan) {
        switch (sourcePixelFormat) {
            case IRawRgPixelFormat<byte> rg1 when targetPixelFormat is IRawRgPixelFormat<byte> rg2:
                ConvertTo(rg1, sourceSpan, width, height, rg2, targetSpan);
                return;
            case IRawRgPixelFormat<sbyte> rg1 when targetPixelFormat is IRawRgPixelFormat<sbyte> rg2:
                ConvertTo(rg1, sourceSpan, width, height, rg2, targetSpan);
                return;
            case IRawRgPixelFormat<ushort> rg1 when targetPixelFormat is IRawRgPixelFormat<ushort> rg2:
                ConvertTo(rg1, sourceSpan, width, height, rg2, targetSpan);
                return;
            case IRawRgPixelFormat<short> rg1 when targetPixelFormat is IRawRgPixelFormat<short> rg2:
                ConvertTo(rg1, sourceSpan, width, height, rg2, targetSpan);
                return;
            case IRawRgPixelFormat<uint> rg1 when targetPixelFormat is IRawRgPixelFormat<uint> rg2:
                ConvertTo(rg1, sourceSpan, width, height, rg2, targetSpan);
                return;
            case IRawRgPixelFormat<int> rg1 when targetPixelFormat is IRawRgPixelFormat<int> rg2:
                ConvertTo(rg1, sourceSpan, width, height, rg2, targetSpan);
                return;
            case IRawRgPixelFormat<Half> rg1 when targetPixelFormat is IRawRgPixelFormat<Half> rg2:
                ConvertTo(rg1, sourceSpan, width, height, rg2, targetSpan);
                return;
            case IRawRgPixelFormat<float> rg1 when targetPixelFormat is IRawRgPixelFormat<float> rg2:
                ConvertTo(rg1, sourceSpan, width, height, rg2, targetSpan);
                return;
        }

        var sourcePitch = sourcePixelFormat.CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = sourcePixelFormat.BytesPerPixel;
        var targetBpp = targetPixelFormat.BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..])
        for (var x = 0; x < width; x++)
            targetPixelFormat.SetRg(targetSpan[(x * targetBpp)..], sourcePixelFormat.GetRg(sourceSpan[(x * sourceBpp)..]));
    }

    private static void ConvertTo(
        IRawRgPixelFormat sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawDsPixelFormat targetPixelFormat,
        Span<byte> targetSpan) {
        var sourcePitch = sourcePixelFormat.CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = sourcePixelFormat.BytesPerPixel;
        var targetBpp = targetPixelFormat.BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..])
        for (var x = 0; x < width; x++)
            targetPixelFormat.SetDs(targetSpan[(x * targetBpp)..], sourcePixelFormat.GetRg(sourceSpan[(x * sourceBpp)..]));
    }

    private static void ConvertTo<T>(
        IRawRgPixelFormat<T> sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawRgPixelFormat<T> targetPixelFormat,
        Span<byte> targetSpan)
        where T : unmanaged, IBinaryNumber<T> {
        var sourcePitch = sourcePixelFormat.CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = sourcePixelFormat.BytesPerPixel;
        var targetBpp = targetPixelFormat.BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..])
        for (var x = 0; x < width; x++)
            targetPixelFormat.SetRg(targetSpan[(x * targetBpp)..], sourcePixelFormat.GetRgTyped(sourceSpan[(x * sourceBpp)..]));
    }

    private static void ConvertTo(
        IRawRgbPixelFormat sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawRgbPixelFormat targetPixelFormat,
        Span<byte> targetSpan) {
        switch (sourcePixelFormat) {
            case IRawRgbPixelFormat<byte> rgb1 when targetPixelFormat is IRawRgbPixelFormat<byte> rgb2:
                ConvertTo(rgb1, sourceSpan, width, height, rgb2, targetSpan);
                return;
            case IRawRgbPixelFormat<sbyte> rgb1 when targetPixelFormat is IRawRgbPixelFormat<sbyte> rgb2:
                ConvertTo(rgb1, sourceSpan, width, height, rgb2, targetSpan);
                return;
            case IRawRgbPixelFormat<ushort> rgb1 when targetPixelFormat is IRawRgbPixelFormat<ushort> rgb2:
                ConvertTo(rgb1, sourceSpan, width, height, rgb2, targetSpan);
                return;
            case IRawRgbPixelFormat<short> rgb1 when targetPixelFormat is IRawRgbPixelFormat<short> rgb2:
                ConvertTo(rgb1, sourceSpan, width, height, rgb2, targetSpan);
                return;
            case IRawRgbPixelFormat<uint> rgb1 when targetPixelFormat is IRawRgbPixelFormat<uint> rgb2:
                ConvertTo(rgb1, sourceSpan, width, height, rgb2, targetSpan);
                return;
            case IRawRgbPixelFormat<int> rgb1 when targetPixelFormat is IRawRgbPixelFormat<int> rgb2:
                ConvertTo(rgb1, sourceSpan, width, height, rgb2, targetSpan);
                return;
            case IRawRgbPixelFormat<Half> rgb1 when targetPixelFormat is IRawRgbPixelFormat<Half> rgb2:
                ConvertTo(rgb1, sourceSpan, width, height, rgb2, targetSpan);
                return;
            case IRawRgbPixelFormat<float> rgb1 when targetPixelFormat is IRawRgbPixelFormat<float> rgb2:
                ConvertTo(rgb1, sourceSpan, width, height, rgb2, targetSpan);
                return;
        }

        var sourcePitch = sourcePixelFormat.CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = sourcePixelFormat.BytesPerPixel;
        var targetBpp = targetPixelFormat.BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..])
        for (var x = 0; x < width; x++)
            targetPixelFormat.SetRgb(targetSpan[(x * targetBpp)..], sourcePixelFormat.GetRgb(sourceSpan[(x * sourceBpp)..]));
    }

    private static void ConvertTo<T>(
        IRawRgbPixelFormat<T> sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawRgbPixelFormat<T> targetPixelFormat,
        Span<byte> targetSpan)
        where T : unmanaged, IMinMaxValue<T>, IBinaryNumber<T> {
        var sourcePitch = sourcePixelFormat.CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = sourcePixelFormat.BytesPerPixel;
        var targetBpp = targetPixelFormat.BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..])
        for (var x = 0; x < width; x++)
            targetPixelFormat.SetRgb(targetSpan[(x * targetBpp)..], sourcePixelFormat.GetRgbTyped(sourceSpan[(x * sourceBpp)..]));
    }

    private static void ConvertTo(
        IRawAPixelFormat sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawAPixelFormat targetPixelFormat,
        Span<byte> targetSpan) {
        switch (sourcePixelFormat) {
            case IRawAPixelFormat<byte> a1 when targetPixelFormat is IRawAPixelFormat<byte> a2:
                ConvertTo(a1, sourceSpan, width, height, a2, targetSpan);
                return;
            case IRawAPixelFormat<sbyte> a1 when targetPixelFormat is IRawAPixelFormat<sbyte> a2:
                ConvertTo(a1, sourceSpan, width, height, a2, targetSpan);
                return;
            case IRawAPixelFormat<ushort> a1 when targetPixelFormat is IRawAPixelFormat<ushort> a2:
                ConvertTo(a1, sourceSpan, width, height, a2, targetSpan);
                return;
            case IRawAPixelFormat<short> a1 when targetPixelFormat is IRawAPixelFormat<short> a2:
                ConvertTo(a1, sourceSpan, width, height, a2, targetSpan);
                return;
            case IRawAPixelFormat<uint> a1 when targetPixelFormat is IRawAPixelFormat<uint> a2:
                ConvertTo(a1, sourceSpan, width, height, a2, targetSpan);
                return;
            case IRawAPixelFormat<int> a1 when targetPixelFormat is IRawAPixelFormat<int> a2:
                ConvertTo(a1, sourceSpan, width, height, a2, targetSpan);
                return;
            case IRawAPixelFormat<Half> a1 when targetPixelFormat is IRawAPixelFormat<Half> a2:
                ConvertTo(a1, sourceSpan, width, height, a2, targetSpan);
                return;
            case IRawAPixelFormat<float> a1 when targetPixelFormat is IRawAPixelFormat<float> a2:
                ConvertTo(a1, sourceSpan, width, height, a2, targetSpan);
                return;
        }

        var sourcePitch = sourcePixelFormat.CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = sourcePixelFormat.BytesPerPixel;
        var targetBpp = targetPixelFormat.BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..])
        for (var x = 0; x < width; x++)
            targetPixelFormat.SetAlpha(targetSpan[(x * targetBpp)..], sourcePixelFormat.GetAlpha(sourceSpan[(x * sourceBpp)..]));
    }

    private static void ConvertTo<T>(
        IRawAPixelFormat<T> sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawAPixelFormat<T> targetPixelFormat,
        Span<byte> targetSpan)
        where T : unmanaged, IBinaryNumber<T> {
        var sourcePitch = sourcePixelFormat.CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = sourcePixelFormat.BytesPerPixel;
        var targetBpp = targetPixelFormat.BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..])
        for (var x = 0; x < width; x++)
            targetPixelFormat.SetAlpha(targetSpan[(x * targetBpp)..], sourcePixelFormat.GetAlphaTyped(sourceSpan[(x * sourceBpp)..]));
    }

    private static void ConvertTo(
        IRawRgbaPixelFormat sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawRgbaPixelFormat targetPixelFormat,
        Span<byte> targetSpan) {
        switch (sourcePixelFormat) {
            case IRawRgbaPixelFormat<byte> rgba1 when targetPixelFormat is IRawRgbaPixelFormat<byte> rgba2:
                ConvertTo(rgba1, sourceSpan, width, height, rgba2, targetSpan);
                return;
            case IRawRgbaPixelFormat<sbyte> rgba1 when targetPixelFormat is IRawRgbaPixelFormat<sbyte> rgba2:
                ConvertTo(rgba1, sourceSpan, width, height, rgba2, targetSpan);
                return;
            case IRawRgbaPixelFormat<ushort> rgba1 when targetPixelFormat is IRawRgbaPixelFormat<ushort> rgba2:
                ConvertTo(rgba1, sourceSpan, width, height, rgba2, targetSpan);
                return;
            case IRawRgbaPixelFormat<short> rgba1 when targetPixelFormat is IRawRgbaPixelFormat<short> rgba2:
                ConvertTo(rgba1, sourceSpan, width, height, rgba2, targetSpan);
                return;
            case IRawRgbaPixelFormat<uint> rgba1 when targetPixelFormat is IRawRgbaPixelFormat<uint> rgba2:
                ConvertTo(rgba1, sourceSpan, width, height, rgba2, targetSpan);
                return;
            case IRawRgbaPixelFormat<int> rgba1 when targetPixelFormat is IRawRgbaPixelFormat<int> rgba2:
                ConvertTo(rgba1, sourceSpan, width, height, rgba2, targetSpan);
                return;
            case IRawRgbaPixelFormat<Half> rgba1 when targetPixelFormat is IRawRgbaPixelFormat<Half> rgba2:
                ConvertTo(rgba1, sourceSpan, width, height, rgba2, targetSpan);
                return;
            case IRawRgbaPixelFormat<float> rgba1 when targetPixelFormat is IRawRgbaPixelFormat<float> rgba2:
                ConvertTo(rgba1, sourceSpan, width, height, rgba2, targetSpan);
                return;
        }

        var sourcePitch = sourcePixelFormat.CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = sourcePixelFormat.BytesPerPixel;
        var targetBpp = targetPixelFormat.BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..])
        for (var x = 0; x < width; x++)
            targetPixelFormat.SetRgba(targetSpan[(x * targetBpp)..], sourcePixelFormat.GetRgba(sourceSpan[(x * sourceBpp)..]));
    }

    private static void ConvertTo<T>(
        IRawRgbaPixelFormat<T> sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawRgbaPixelFormat<T> targetPixelFormat,
        Span<byte> targetSpan)
        where T : unmanaged, IMinMaxValue<T>, IBinaryNumber<T> {
        var sourcePitch = sourcePixelFormat.CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = sourcePixelFormat.BytesPerPixel;
        var targetBpp = targetPixelFormat.BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..])
        for (var x = 0; x < width; x++)
            targetPixelFormat.SetRgba(targetSpan[(x * targetBpp)..], sourcePixelFormat.GetRgbaTyped(sourceSpan[(x * sourceBpp)..]));
    }

    private static void ConvertTo(
        IRawDPixelFormat sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawDPixelFormat targetPixelFormat,
        Span<byte> targetSpan) {
        switch (sourcePixelFormat) {
            case IRawDPixelFormat<byte> d1 when targetPixelFormat is IRawDPixelFormat<byte> d2:
                ConvertTo(d1, sourceSpan, width, height, d2, targetSpan);
                return;
            case IRawDPixelFormat<sbyte> d1 when targetPixelFormat is IRawDPixelFormat<sbyte> d2:
                ConvertTo(d1, sourceSpan, width, height, d2, targetSpan);
                return;
            case IRawDPixelFormat<ushort> d1 when targetPixelFormat is IRawDPixelFormat<ushort> d2:
                ConvertTo(d1, sourceSpan, width, height, d2, targetSpan);
                return;
            case IRawDPixelFormat<short> d1 when targetPixelFormat is IRawDPixelFormat<short> d2:
                ConvertTo(d1, sourceSpan, width, height, d2, targetSpan);
                return;
            case IRawDPixelFormat<uint> d1 when targetPixelFormat is IRawDPixelFormat<uint> d2:
                ConvertTo(d1, sourceSpan, width, height, d2, targetSpan);
                return;
            case IRawDPixelFormat<int> d1 when targetPixelFormat is IRawDPixelFormat<int> d2:
                ConvertTo(d1, sourceSpan, width, height, d2, targetSpan);
                return;
            case IRawDPixelFormat<Half> d1 when targetPixelFormat is IRawDPixelFormat<Half> d2:
                ConvertTo(d1, sourceSpan, width, height, d2, targetSpan);
                return;
            case IRawDPixelFormat<float> d1 when targetPixelFormat is IRawDPixelFormat<float> d2:
                ConvertTo(d1, sourceSpan, width, height, d2, targetSpan);
                return;
        }

        var sourcePitch = sourcePixelFormat.CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = sourcePixelFormat.BytesPerPixel;
        var targetBpp = targetPixelFormat.BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..])
        for (var x = 0; x < width; x++)
            targetPixelFormat.SetDepth(targetSpan[(x * targetBpp)..], sourcePixelFormat.GetDepth(sourceSpan[(x * sourceBpp)..]));
    }

    private static void ConvertTo<T>(
        IRawDPixelFormat<T> sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawDPixelFormat<T> targetPixelFormat,
        Span<byte> targetSpan)
        where T : unmanaged, IBinaryNumber<T> {
        var sourcePitch = sourcePixelFormat.CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = sourcePixelFormat.BytesPerPixel;
        var targetBpp = targetPixelFormat.BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..])
        for (var x = 0; x < width; x++)
            targetPixelFormat.SetDepth(targetSpan[(x * targetBpp)..], sourcePixelFormat.GetDepthTyped(sourceSpan[(x * sourceBpp)..]));
    }

    private static void ConvertTo(
        IRawDPixelFormat sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawRPixelFormat targetPixelFormat,
        Span<byte> targetSpan) {
        switch (sourcePixelFormat) {
            case IRawDPixelFormat<byte> r1 when targetPixelFormat is IRawRPixelFormat<byte> r2:
                ConvertTo(r1, sourceSpan, width, height, r2, targetSpan);
                return;
            case IRawDPixelFormat<sbyte> r1 when targetPixelFormat is IRawRPixelFormat<sbyte> r2:
                ConvertTo(r1, sourceSpan, width, height, r2, targetSpan);
                return;
            case IRawDPixelFormat<ushort> r1 when targetPixelFormat is IRawRPixelFormat<ushort> r2:
                ConvertTo(r1, sourceSpan, width, height, r2, targetSpan);
                return;
            case IRawDPixelFormat<short> r1 when targetPixelFormat is IRawRPixelFormat<short> r2:
                ConvertTo(r1, sourceSpan, width, height, r2, targetSpan);
                return;
            case IRawDPixelFormat<uint> r1 when targetPixelFormat is IRawRPixelFormat<uint> r2:
                ConvertTo(r1, sourceSpan, width, height, r2, targetSpan);
                return;
            case IRawDPixelFormat<int> r1 when targetPixelFormat is IRawRPixelFormat<int> r2:
                ConvertTo(r1, sourceSpan, width, height, r2, targetSpan);
                return;
            case IRawDPixelFormat<Half> r1 when targetPixelFormat is IRawRPixelFormat<Half> r2:
                ConvertTo(r1, sourceSpan, width, height, r2, targetSpan);
                return;
            case IRawDPixelFormat<float> r1 when targetPixelFormat is IRawRPixelFormat<float> r2:
                ConvertTo(r1, sourceSpan, width, height, r2, targetSpan);
                return;
        }

        var sourcePitch = sourcePixelFormat.CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = sourcePixelFormat.BytesPerPixel;
        var targetBpp = targetPixelFormat.BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..])
        for (var x = 0; x < width; x++)
            targetPixelFormat.SetRed(targetSpan[(x * targetBpp)..], sourcePixelFormat.GetDepth(sourceSpan[(x * sourceBpp)..]));
    }

    private static void ConvertTo<T>(
        IRawDPixelFormat<T> sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawRPixelFormat<T> targetPixelFormat,
        Span<byte> targetSpan)
        where T : unmanaged, IBinaryNumber<T> {
        var sourcePitch = sourcePixelFormat.CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = sourcePixelFormat.BytesPerPixel;
        var targetBpp = targetPixelFormat.BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..])
        for (var x = 0; x < width; x++)
            targetPixelFormat.SetRed(targetSpan[(x * targetBpp)..], sourcePixelFormat.GetDepthTyped(sourceSpan[(x * sourceBpp)..]));
    }

    private static void ConvertTo(
        IRawDsPixelFormat sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawDsPixelFormat targetPixelFormat,
        Span<byte> targetSpan) {
        var sourcePitch = sourcePixelFormat.CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = sourcePixelFormat.BytesPerPixel;
        var targetBpp = targetPixelFormat.BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..])
        for (var x = 0; x < width; x++)
            targetPixelFormat.SetDs(targetSpan[(x * targetBpp)..], sourcePixelFormat.GetDs(sourceSpan[(x * sourceBpp)..]));
    }

    private static void ConvertTo(
        IRawDsPixelFormat sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawGPixelFormat targetPixelFormat,
        Span<byte> targetSpan) {
        var sourcePitch = sourcePixelFormat.CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = sourcePixelFormat.BytesPerPixel;
        var targetBpp = targetPixelFormat.BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..])
        for (var x = 0; x < width; x++)
            targetPixelFormat.SetGreen(targetSpan[(x * targetBpp)..], sourcePixelFormat.GetStencil(sourceSpan[(x * sourceBpp)..]));
    }

    private static void ConvertTo(
        IRawDsPixelFormat sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawRgPixelFormat targetPixelFormat,
        Span<byte> targetSpan) {
        var sourcePitch = sourcePixelFormat.CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = sourcePixelFormat.BytesPerPixel;
        var targetBpp = targetPixelFormat.BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..])
        for (var x = 0; x < width; x++)
            targetPixelFormat.SetRg(targetSpan[(x * targetBpp)..], sourcePixelFormat.GetDs(sourceSpan[(x * sourceBpp)..]));
    }

    private static float Average(this Vector2 v3) => (v3.X + v3.Y) / 3;

    private static float Average(this Vector3 v3) => (v3.X + v3.Y + v3.Z) / 3;

    /// <summary>
    /// Translate the given byte span into another, from a pixel format to another, using a converter delegate.
    /// </summary>
    /// <param name="sourcePixelFormat">Source pixel format.</param>
    /// <param name="sourceSpan">Byte span containing source data.</param>
    /// <param name="width">Width of the image.</param>
    /// <param name="height">Height of the image.</param>
    /// <param name="targetPixelFormat">Target pixel format.</param>
    /// <param name="targetSpan">Byte span for translated data.</param>
    /// <param name="convertPixelDelegate">Converter delegate.</param>
    /// <typeparam name="TSource">Source raw pixel format.</typeparam>
    /// <typeparam name="TTarget">Target raw pixel format.</typeparam>
    /// <returns>Whether the operation was supported and successful.</returns>
    public static void ConvertToAny<TSource, TTarget>(
        TSource sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        TTarget targetPixelFormat,
        Span<byte> targetSpan,
        ConvertPixelDelegate<TSource, TTarget> convertPixelDelegate)
        where TSource : IRawPixelFormat
        where TTarget : IRawPixelFormat {
        var sourcePitch = sourcePixelFormat.CalculatePitch(width);
        var targetPitch = targetPixelFormat.CalculatePitch(width);
        var sourceBpp = sourcePixelFormat.BytesPerPixel;
        var targetBpp = targetPixelFormat.BytesPerPixel;
        sourceSpan = sourceSpan[..(sourcePitch * height)];
        targetSpan = targetSpan[..(targetPitch * height)];
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..])
        for (var x = 0; x < width; x++)
            convertPixelDelegate(sourceSpan[(x * sourceBpp)..], targetSpan[(x * targetBpp)..]);
    }

    /// <summary>
    /// A delegate for converting a pixel.
    /// </summary>
    /// <typeparam name="TSource">Source raw pixel format.</typeparam>
    /// <typeparam name="TTarget">Target raw pixel format.</typeparam>
    public delegate void ConvertPixelDelegate<in TSource, in TTarget>(
        ReadOnlySpan<byte> sourceSpan,
        Span<byte> targetSpan);
}
