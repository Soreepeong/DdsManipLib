using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

public interface IChannel : IEquatable<IChannel> {
    public int BitOffset { get; }
    public int BitCount { get; }
    public uint BitMask32 => (1u << BitCount) - 1;
    public uint BitMask32Shifted => BitMask32 << BitOffset;

    public void CopyPixelToSInt<T2>(ReadOnlySpan<byte> source, int sourceShift, IChannel<T2> targetChannel, Span<byte> target, int targetShift)
        where T2 : unmanaged, ISignedNumber<T2>, IBinaryInteger<T2>, IBinaryNumber<T2>, IMinMaxValue<T2>;
    
    public void CopyPixelToUInt<T2>(ReadOnlySpan<byte> source, int sourceShift, IChannel<T2> targetChannel, Span<byte> target, int targetShift)
        where T2 : unmanaged, IUnsignedNumber<T2>, IBinaryInteger<T2>, IBinaryNumber<T2>, IMinMaxValue<T2>;
    
    public void CopyPixelToSNorm<T2>(ReadOnlySpan<byte> source, int sourceShift, IChannel<T2> targetChannel, Span<byte> target, int targetShift)
        where T2 : unmanaged, ISignedNumber<T2>, IBinaryInteger<T2>, IBinaryNumber<T2>, IMinMaxValue<T2>;
    
    public void CopyPixelToUNorm<T2>(ReadOnlySpan<byte> source, int sourceShift, IChannel<T2> targetChannel, Span<byte> target, int targetShift)
        where T2 : unmanaged, IUnsignedNumber<T2>, IBinaryInteger<T2>, IBinaryNumber<T2>, IMinMaxValue<T2>;

    public void CopyPixelToFloat(ReadOnlySpan<byte> source, int sourceShift, IFloatChannel targetChannel, Span<byte> target, int targetShift);
    
    [return: NotNullIfNotNull(nameof(target))]
    public CopyPixelDelegate? GetCopyPixelDelegate(IChannel? target);
    
    public delegate void CopyPixelDelegate(ReadOnlySpan<byte> source, int sourceShift, Span<byte> target, int targetShift);
}