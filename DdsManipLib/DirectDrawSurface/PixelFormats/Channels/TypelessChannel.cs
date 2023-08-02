using System;
using System.Numerics;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

public readonly struct TypelessChannel : IChannel {
    public TypelessChannel(int bitOffset, int bitCount) {
        BitOffset = bitOffset;
        BitCount = bitCount;
    }

    public int BitOffset { get; }
    public int BitCount { get; }

    public bool Equals(TypelessChannel other) => BitOffset == other.BitOffset && BitCount == other.BitCount;

    public bool Equals(IChannel? other) => other is TypelessChannel r && Equals(r);

    public override bool Equals(object? obj) => obj is TypelessChannel other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(BitOffset, BitCount);

    public static bool operator ==(TypelessChannel left, TypelessChannel right) => left.Equals(right);

    public static bool operator !=(TypelessChannel left, TypelessChannel right) => !(left == right);
    
    public static TypelessChannel? FromMask(uint mask) {
        if (mask == 0)
            return null;
        
        var shift = BitOperations.TrailingZeroCount(mask);
        var bits = BitOperations.PopCount(mask);
        var mask2 = ((1u << bits) - 1u) << shift;
        if (mask != mask2)
            throw new NotSupportedException("Mask with a hole in the middle is not supported.");
        return new(shift, bits);
    }
}