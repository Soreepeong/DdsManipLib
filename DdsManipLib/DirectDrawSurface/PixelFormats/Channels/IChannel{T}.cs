using System;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

public interface IChannel<T> : IChannel {
    public T ReadValue(ReadOnlySpan<byte> span, int shift);
    public void WriteValue(Span<byte> span, int shift, T value);

    public sealed uint ReadRawUInt32(ReadOnlySpan<byte> span, int shift) {
        shift += BitOffset;
        span = span[(shift / 8)..];
        shift %= 8;

        var tmp = 0ul;
        for (var i = 0; i < shift + BitCount; i += 8)
            tmp |= (ulong) span[i] << (i * 8);

        return (uint) (tmp >> shift) & ((1u << BitCount) - 1);
    }

    public sealed void WriteRawUInt32(Span<byte> span, int shift, uint value) {
        shift += BitOffset;
        span = span[(shift / 8)..];
        shift %= 8;

        var tmp = 0ul;
        for (var i = 0; i < shift + BitCount; i += 8)
            tmp |= (ulong) span[i] << (i * 8);

        tmp &= ~(((1ul << BitCount) - 1) << shift);
        tmp |= value << shift;
        for (var i = 0; i < shift + BitCount; i += 8)
            span[i] = (byte) (tmp >> (i * 8));
    }
}