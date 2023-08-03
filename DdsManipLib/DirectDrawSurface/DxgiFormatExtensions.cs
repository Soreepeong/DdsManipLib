using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DdsManipLib.DirectDrawSurface.PixelFormats;

namespace DdsManipLib.DirectDrawSurface;

public static class DxgiFormatExtensions {
    public static bool TryGetDxgiFormat(this IPixelFormat value, out DxgiFormat dxgiFormat) {
        dxgiFormat = DxgiFormat.Unknown;

        if (Enum.GetNames<DxgiFormat>()
                .FirstOrDefault(x => value.Equals(typeof(DxgiFormat).GetField(x)?.GetCustomAttribute<PixelFormat>())) is not { } dxgiFormatName)
            return false;

        dxgiFormat = Enum.Parse<DxgiFormat>(dxgiFormatName);
        return true;
    }

    public static bool TryGetPixelFormat(this DxgiFormat dxgiFormat, [MaybeNullWhen(false)] out IPixelFormat pixelFormat) {
        pixelFormat = null;

        if (Enum.GetName(dxgiFormat) is not { } name)
            return false;
        if (typeof(DxgiFormat).GetField(name)?.GetCustomAttribute<PixelFormat>() is not { } pf)
            return false;
        pixelFormat = pf;
        return true;
    }
}
