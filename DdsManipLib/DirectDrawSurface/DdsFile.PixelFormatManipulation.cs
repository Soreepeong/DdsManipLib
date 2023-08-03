using System;
using System.Diagnostics.CodeAnalysis;
using DdsManipLib.DirectDrawSurface.PixelFormats;

namespace DdsManipLib.DirectDrawSurface;

public partial class DdsFile {
    /// <summary>
    /// Attempt to deduce a corresponding <see cref="PixelFormat"/>.
    /// </summary>
    /// <param name="pixelFormat">The resulting pixel format, or null if not found.</param>
    /// <returns>Whether the corresponding format has been found.</returns>
    public bool TryDeducePixelFormat([MaybeNullWhen(false)] out IPixelFormat pixelFormat) => UseDxt10Header
        ? HeaderDxt10.DxgiFormat.TryGetPixelFormat((HeaderDxt10.MiscFlags2 & DdsHeaderDxt10MiscFlags2.AlphaMask) switch {
                DdsHeaderDxt10MiscFlags2.AlphaModeUnknown => AlphaType.All,
                DdsHeaderDxt10MiscFlags2.AlphaModeStraight => AlphaType.Straight,
                DdsHeaderDxt10MiscFlags2.AlphaModePremultiplied => AlphaType.Premultiplied,
                DdsHeaderDxt10MiscFlags2.AlphaModeOpaque => AlphaType.None,
                DdsHeaderDxt10MiscFlags2.AlphaModeCustom => AlphaType.Custom,
                _ => AlphaType.All,
            },
            out pixelFormat)
        : Header.PixelFormat.TryGetPixelFormat(out pixelFormat);

    /// <summary>
    /// Attempt to update the pixel format.
    /// </summary>
    /// <param name="pixelFormat"></param>
    /// <param name="enableLegacyFormat">Enable the use of describing the pixel format using the fields in <see cref="DdsPixelFormat"/>.</param>
    /// <param name="enableDxt10HeaderFormat">Enable the use of describing the pixel format using the fields in <see cref="DdsHeaderDxt10"/>.</param>
    /// <returns>Whether the pixel format has been updated.</returns>
    /// <remarks>Number of images may be reset, in case the legacy format has been used.</remarks>
    public bool TryUpdatePixelFormat(IPixelFormat pixelFormat, bool enableLegacyFormat, bool enableDxt10HeaderFormat) {
        // If Pitch * Height == LinearSize, use Pitch; otherwise, use LinearSize.
        var newPitch = pixelFormat.CalculatePitch(Header.Width);
        var newLinearSize = pixelFormat.CalculateLinearSize(Header.Width, Header.Height);
        var newFlags = Header.Flags & ~(DdsHeaderFlags.Pitch | DdsHeaderFlags.LinearSize);
        newFlags |= newPitch * Header.Height == newLinearSize ? DdsHeaderFlags.Pitch : DdsHeaderFlags.LinearSize;
        var newPitchOrLinearSize = newFlags.HasFlag(DdsHeaderFlags.Pitch) ? newPitch : newLinearSize;

        if (enableLegacyFormat && pixelFormat.DdsPixelFormat is {HasValidFormat: true, UseDxt10Header: false} ddspf) {
            Header.PitchOrLinearSize = newPitchOrLinearSize;
            Header.Flags = newFlags;
            Header.PixelFormat = ddspf;
            HeaderDxt10 = default;
            return true;
        }

        if (enableDxt10HeaderFormat && pixelFormat.DxgiFormat != DxgiFormat.Unknown) {
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
    public IPixelFormat? PixelFormat => TryDeducePixelFormat(out var pf) ? pf : null;

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
    public bool TryConvertTo(IPixelFormat targetPixelFormat, [MaybeNullWhen(false)] out DdsFile target) {
        target = new(Header, HeaderDxt10, Array.Empty<byte>(), Array.Empty<byte>());
        if (!target.TryUpdatePixelFormat(targetPixelFormat, NumImages == 1, true))
            return false;

        target.InitializeBody();
        if (PixelFormat is not { } sourcePixelFormat)
            return false;

        foreach (var slice in EnumerateSlices()) {
            if (!sourcePixelFormat.ConvertToAuto(
                    SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice),
                    slice.Width,
                    slice.Height,
                    targetPixelFormat,
                    target.SliceSpan(slice.Image, slice.Face, slice.Mipmap, slice.Slice)))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Create a new <see cref="DdsFile"/> from this, using the specified target pixel format.
    ///
    /// Conversion across pixel formats with different number of channels may do anything.
    /// It is recommended to do that manually.
    /// </summary>
    public DdsFile ConvertTo(IPixelFormat pixelFormat) => !TryConvertTo(pixelFormat, out var x) ? throw new InvalidOperationException() : x;
}
