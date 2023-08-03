namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public interface IRawRAlignedBytePixelFormat : IRawRPixelFormat {
    public int OffsetR { get; }
}

public interface IRawRgAlignedBytePixelFormat : IRawRgPixelFormat, IRawRAlignedBytePixelFormat {
    public int OffsetG { get; }
}

public interface IRawRgbAlignedBytePixelFormat : IRawRgbPixelFormat, IRawRgAlignedBytePixelFormat {
    public int OffsetB { get; }
}

public interface IRawRgbaAlignedBytePixelFormat : IRawRgbaPixelFormat, IRawRgbAlignedBytePixelFormat {
    public int OffsetA { get; }
}
