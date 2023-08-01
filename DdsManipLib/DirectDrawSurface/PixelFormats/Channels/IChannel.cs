using System;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

public interface IChannel : IEquatable<IChannel> {
    public int BitOffset { get; }
    public int BitCount { get; }
    public uint BitMask32 => (1u << BitCount) - 1;
    public uint BitMask32Shifted => BitMask32 << BitOffset;
}