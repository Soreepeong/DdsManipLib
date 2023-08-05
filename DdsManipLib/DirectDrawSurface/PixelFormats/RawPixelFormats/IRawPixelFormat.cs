using System;
using System.Numerics;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public interface IRawPixelFormat : IPixelFormat {
    public int BytesPerPixel { get; }

    public void ClearPixel(Span<byte> pixel);
}

public interface IRawPixelFormat<T> : IRawPixelFormat
    where T : unmanaged, IBinaryNumber<T> {
}