using System;
using System.Diagnostics.CodeAnalysis;
using DdsManipLib.DirectDrawSurface.PixelFormats;
using DdsManipLib.DirectDrawSurface.PixelFormats.Old;

namespace DdsManipLib.DirectDrawSurface;

public partial class DdsFile { 
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
    /// </summary>
    /// <param name="pixelFormat"></param>
    /// <param name="enableLegacyFormat">Enable the use of describing the pixel format using the fields in <see cref="DdsPixelFormat"/>.</param>
    /// <param name="enableDxt10HeaderFormat">Enable the use of describing the pixel format using the fields in <see cref="DdsHeaderDxt10"/>.</param>
    /// <returns>Whether the pixel format has been updated.</returns>
    /// <remarks>Number of images may be reset, in case the legacy format has been used.</remarks>
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
    /// Attempt to create a new <see cref="DdsFile"/> from this, using the specified target pixel format.
    ///
    /// Conversion across pixel formats with different number of channels may do anything.
    /// It is recommended to do that manually.
    /// </summary>
    /// <param name="targetPixelFormat"></param>
    /// <param name="target"></param>
    /// <returns>If false, the attempt was unsuccessful.</returns>
    public bool TryConvertTo(PixelFormat targetPixelFormat, [MaybeNullWhen(false)] out DdsFile target) {
        target = new(Header, HeaderDxt10, Array.Empty<byte>(), Array.Empty<byte>());
        if (!target.TryUpdatePixelFormat(targetPixelFormat, NumImages == 1, true))
            return false;

        target.InitializeBody();
        var sourcePixelFormat = PixelFormat;

        byte[]? tmp = null;
        foreach (var slice in EnumerateSlices()) {
            switch (sourcePixelFormat) {
                case var _ when sourcePixelFormat == targetPixelFormat:
                    SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice)
                        .CopyTo(target.SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice));
                    break;
                case RgbaPixelFormat rgbaS:
                    switch (targetPixelFormat) {
                        case RgbaPixelFormat rgbaT:
                            rgbaS.ConvertToRgba(
                                rgbaT,
                                target.SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                target.Pitch(slice.Mipmap),
                                SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                Pitch(slice.Mipmap),
                                slice.Width,
                                slice.Height);
                            break;
                        case BcPixelFormat bcT when bcT.Version != 6:
                            rgbaS.ConvertToBc(
                                bcT,
                                target.SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                Pitch(slice.Mipmap),
                                slice.Width,
                                slice.Height);
                            break;
                        case LumiPixelFormat lumiT:
                            rgbaS.ConvertToLumi(
                                lumiT,
                                target.SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                target.Pitch(slice.Mipmap),
                                SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                Pitch(slice.Mipmap),
                                slice.Width,
                                slice.Height);
                            break;
                        default:
                            return false;
                    }

                    break;
                case BcPixelFormat bcS:
                    switch (targetPixelFormat) {
                        case RgbaPixelFormat rgbaT:
                            bcS.ConvertToRgba(
                                rgbaT,
                                target.SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                target.Pitch(slice.Mipmap),
                                SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                slice.Width,
                                slice.Height);
                            break;
                        case BcPixelFormat bcT when bcT.Version != 6:
                            tmp ??= new byte[4 * Header.Width * Header.Height];
                            bcS.ConvertToRgba(
                                RgbaPixelFormat.NewBgra(8, 8, 8, 8, 0, 0, bcT.Type, bcT.Alpha),
                                tmp,
                                4 * slice.Width,
                                SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                slice.Width,
                                slice.Height);
                            RgbaPixelFormat.NewBgra(8, 8, 8, 8, 0, 0, bcT.Type, bcT.Alpha)
                                .ConvertToBc(
                                    bcT,
                                    target.SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                    tmp,
                                    4 * slice.Width,
                                    slice.Width,
                                    slice.Height);
                            break;
                        case LumiPixelFormat lumiT:
                            bcS.ConvertToLumi(
                                lumiT,
                                target.SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                target.Pitch(slice.Mipmap),
                                SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                slice.Width,
                                slice.Height);
                            break;
                        default:
                            return false;
                    }

                    break;
                case LumiPixelFormat lumiS:
                    switch (targetPixelFormat) {
                        case RgbaPixelFormat rgbaT:
                            lumiS.ConvertToRgba(
                                rgbaT,
                                target.SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                target.Pitch(slice.Mipmap),
                                SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                Pitch(slice.Mipmap),
                                slice.Width,
                                slice.Height);
                            break;
                        case BcPixelFormat bcT when bcT.Version != 6:
                            lumiS.ConvertToBc(
                                bcT,
                                target.SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                Pitch(slice.Mipmap),
                                slice.Width,
                                slice.Height);
                            break;
                        case LumiPixelFormat lumiT:
                            lumiS.ConvertToLumi(
                                lumiT,
                                target.SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                target.Pitch(slice.Mipmap),
                                SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                                Pitch(slice.Mipmap),
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

        return true;
    }

    /// <summary>
    /// Create a new <see cref="DdsFile"/> from this, using the specified target pixel format.
    ///
    /// Conversion across pixel formats with different number of channels may do anything.
    /// It is recommended to do that manually.
    /// </summary>
    public DdsFile ConvertTo(PixelFormat pixelFormat) => !TryConvertTo(pixelFormat, out var x) ? throw new InvalidOperationException() : x;
}
