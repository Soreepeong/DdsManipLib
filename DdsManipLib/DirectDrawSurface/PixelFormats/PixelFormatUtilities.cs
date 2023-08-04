using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Reflection;
using DdsManipLib.DirectDrawSurface.PixelFormats.BlockPixelFormats;
using DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public static class PixelFormatUtilities {
    private static readonly IReadOnlyDictionary<Tuple<AlphaType, DxgiFormat>, IPixelFormat> DxgiToPixelFormatMap;
    private static readonly IReadOnlyDictionary<DdsPixelFormat, IPixelFormat> DdspfToPixelFormatMap;

    static PixelFormatUtilities() {
        var type = typeof(IPixelFormat);

        var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
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
        DdspfToPixelFormatMap = opaquePixelFormats
            .Concat(alphaPixelFormats.Where(x => x.AlphaType == AlphaType.Straight))
            .Where(x => x.DdsPixelFormat is {HasValidFormat: true, UseDxt10Header: false})
            .ToImmutableDictionary(x => x.DdsPixelFormat, x => x);
    }

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

    public static bool TryGetPixelFormat(this DdsPixelFormat ddspf, [MaybeNullWhen(false)] out IPixelFormat pixelFormat) {
        if (DdspfToPixelFormatMap.TryGetValue(ddspf, out pixelFormat))
            return true;

        // FourCC should have been dealt from the above dictionary access; skip it.

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
            if (abits != 0)
                pixelFormat = maxBits switch {
                    <= 8 => new DdspfRxGxBxAxUNormPixelFormat<byte>(ddspf.RgbBitCount, rshift, rbits, gshift, gbits, bshift, bbits, ashift, abits),
                    <= 16 => new DdspfRxGxBxAxUNormPixelFormat<ushort>(ddspf.RgbBitCount, rshift, rbits, gshift, gbits, bshift, bbits, ashift, abits),
                    <= 32 => new DdspfRxGxBxAxUNormPixelFormat<uint>(ddspf.RgbBitCount, rshift, rbits, gshift, gbits, bshift, bbits, ashift, abits),
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
            return false;
            // TODO: return true;
        }

        if (ddspf.Flags.HasFlag(DdsPixelFormatFlags.Luminance)) {
            return false;
            // TODO: return true;
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

    public static bool DeconstructMask(uint mask, out int shift, out int bits) {
        shift = BitOperations.TrailingZeroCount(mask);
        bits = BitOperations.PopCount(mask);
        var mask2 = ((1u << bits) - 1u) << shift;
        return mask == mask2;
    }

    public static int RawToSInt(uint rawValue, int sourceBits) {
        var signMask = 1u << (sourceBits - 1);
        var numberMask = signMask - 1;
        if ((rawValue & signMask) == 0)
            return (int) (rawValue & numberMask);
        return (int) (~numberMask | (rawValue & numberMask));
    }

    public static uint SIntToRaw(int intValue, int sourceBits) {
        var maxValue = (1 << (sourceBits - 1)) - 1;
        var minValue = -(1 << (sourceBits - 1));
        if (intValue > maxValue)
            intValue = maxValue;
        else if (intValue < minValue)
            intValue = minValue;
        return (uint) intValue & ((1u << sourceBits) - 1u);
    }

    public static uint UIntToRaw(uint uintValue, int sourceBits) {
        var maxValue = (1u << sourceBits) - 1;
        return uintValue > maxValue ? maxValue : uintValue;
    }

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
                        ConvertTo(rawS, sourceSpan, width, height, rawT, targetSpan);
                        return true;
                    case IBlockPixelFormat blockT:
                        if (blockT.SupportsRawPixelFormat(rawS)) {
                            blockT.Compress(rawS, sourceSpan, width, height, targetSpan);
                        } else {
                            var rawpf = blockT.SuggestedRawPixelFormat;
                            var tmplen = rawpf.CalculateLinearSize(width, height);
                            if (scratchBuffer is null || scratchBuffer.Length < tmplen)
                                scratchBuffer = new byte[tmplen];
                            ConvertTo(rawS, sourceSpan, width, height, rawpf, scratchBuffer);
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
                        if (blockS.SupportsRawPixelFormat(rawT)) { } else {
                            var rawpf = blockS.SuggestedRawPixelFormat;
                            var tmplen = rawpf.CalculateLinearSize(width, height);
                            if (scratchBuffer is null || scratchBuffer.Length < tmplen)
                                scratchBuffer = new byte[tmplen];

                            blockS.Decompress(rawpf, sourceSpan, width, height, scratchBuffer);
                            ConvertTo(rawpf, scratchBuffer, width, height, rawT, targetSpan);
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

    public static void ConvertTo(
        IRawPixelFormat sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawPixelFormat targetPixelFormat,
        Span<byte> targetSpan) {
        switch (sourcePixelFormat) {
            case IRawRgbaPixelFormat rgba1 when targetPixelFormat is IRawRgbaPixelFormat rgba2:
                ConvertTo(rgba1, sourceSpan, width, height, rgba2, targetSpan);
                break;
            case IRawRgbPixelFormat rgb1 when targetPixelFormat is IRawRgbPixelFormat rgb2:
                ConvertTo(rgb1, sourceSpan, width, height, rgb2, targetSpan);
                break;
            case IRawRgPixelFormat rg1 when targetPixelFormat is IRawRgPixelFormat rg2:
                ConvertTo(rg1, sourceSpan, width, height, rg2, targetSpan);
                break;
            case IRawGPixelFormat g1 when targetPixelFormat is IRawGPixelFormat g2:
                ConvertTo(g1, sourceSpan, width, height, g2, targetSpan);
                break;
            case IRawRPixelFormat r1 when targetPixelFormat is IRawRPixelFormat r2:
                ConvertTo(r1, sourceSpan, width, height, r2, targetSpan);
                break;
            case IRawAPixelFormat a1 when targetPixelFormat is IRawAPixelFormat a2:
                ConvertTo(a1, sourceSpan, width, height, a2, targetSpan);
                break;
            case IRawDPixelFormat d1 when targetPixelFormat is IRawDPixelFormat d2:
                ConvertTo(d1, sourceSpan, width, height, d2, targetSpan);
                break;
            case IRawDsPixelFormat d1 when targetPixelFormat is IRawDsPixelFormat d2:
                ConvertTo(d1, sourceSpan, width, height, d2, targetSpan);
                break;
        }
    }

    public static void ConvertTo(
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

    public static void ConvertTo(
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

    public static void ConvertTo<T>(
        IRawRPixelFormat<T> sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawRPixelFormat<T> targetPixelFormat,
        Span<byte> targetSpan)
        where T : unmanaged {
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

    public static void ConvertTo<T, TDepth>(
        IRawGPixelFormat<T> sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawDsPixelFormat<TDepth, T> targetPixelFormat,
        Span<byte> targetSpan)
        where T : unmanaged
        where TDepth : unmanaged {
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

    public static void ConvertTo(
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

    public static void ConvertTo(
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

    public static void ConvertTo<T>(
        IRawGPixelFormat<T> sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawGPixelFormat<T> targetPixelFormat,
        Span<byte> targetSpan)
        where T : unmanaged {
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

    public static void ConvertTo<T>(
        IRawGPixelFormat<T> sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawDPixelFormat<T> targetPixelFormat,
        Span<byte> targetSpan)
        where T : unmanaged {
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

    public static void ConvertTo(
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

    public static void ConvertTo(
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

    public static void ConvertTo<T>(
        IRawRgPixelFormat<T> sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawRgPixelFormat<T> targetPixelFormat,
        Span<byte> targetSpan)
        where T : unmanaged {
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

    public static void ConvertTo(
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

    public static void ConvertTo<T>(
        IRawRgbPixelFormat<T> sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawRgbPixelFormat<T> targetPixelFormat,
        Span<byte> targetSpan)
        where T : unmanaged, IMinMaxValue<T> {
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

    public static void ConvertTo(
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

    public static void ConvertTo<T>(
        IRawAPixelFormat<T> sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawAPixelFormat<T> targetPixelFormat,
        Span<byte> targetSpan)
        where T : unmanaged {
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

    public static void ConvertTo(
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

    public static void ConvertTo<T>(
        IRawRgbaPixelFormat<T> sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawRgbaPixelFormat<T> targetPixelFormat,
        Span<byte> targetSpan)
        where T : unmanaged, IMinMaxValue<T> {
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

    public static void ConvertTo(
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

    public static void ConvertTo(
        IRawDPixelFormat sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawRPixelFormat targetPixelFormat,
        Span<byte> targetSpan) {
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

    public static void ConvertTo<T>(
        IRawDPixelFormat<T> sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawDPixelFormat<T> targetPixelFormat,
        Span<byte> targetSpan)
        where T : unmanaged {
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

    public static void ConvertTo<T>(
        IRawDPixelFormat<T> sourcePixelFormat,
        ReadOnlySpan<byte> sourceSpan,
        int width,
        int height,
        IRawRPixelFormat<T> targetPixelFormat,
        Span<byte> targetSpan)
        where T : unmanaged {
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

    public static void ConvertTo(
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
            targetPixelFormat.SetDepth(targetSpan[(x * targetBpp)..], sourcePixelFormat.GetDepth(sourceSpan[(x * sourceBpp)..]));
    }

    public static void ConvertTo(
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

    public static void ConvertTo(
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
}
