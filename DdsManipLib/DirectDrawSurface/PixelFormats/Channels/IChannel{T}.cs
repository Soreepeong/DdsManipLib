using System;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

public interface IChannel<T> : IChannel {
    public T ReadValue(ReadOnlySpan<byte> span, int shift);
    public void WriteValue(Span<byte> span, int shift, T value);
}

public static class ChannelUtilities {
    public static uint ReadRawUInt32(ReadOnlySpan<byte> span, int bitOffset, int bitCount) {
        span = span[(bitOffset / 8)..];
        bitOffset %= 8;

        var tmp = 0ul;
        for (var i = 0; i < bitOffset + bitCount; i += 8)
            tmp |= (ulong) span[i] << (i * 8);

        return (uint) (tmp >> bitOffset) & ((1u << bitCount) - 1);
    }

    public static void WriteRawUInt32(Span<byte> span, int bitOffset, int bitCount, uint value) {
        span = span[(bitOffset / 8)..];
        bitOffset %= 8;

        var tmp = 0ul;
        for (var i = 0; i < bitOffset + bitCount; i += 8)
            tmp |= (ulong) span[i] << (i * 8);

        tmp &= ~(((1ul << bitCount) - 1) << bitOffset);
        tmp |= value << bitOffset;
        for (var i = 0; i < bitOffset + bitCount; i += 8)
            span[i] = (byte) (tmp >> (i * 8));
    }
}
