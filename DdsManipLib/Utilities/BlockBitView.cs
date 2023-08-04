using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace DdsManipLib.Utilities;

public readonly ref struct BlockBitView {
    private readonly ReadOnlySpan<byte> _spanReadOnly;
    private readonly Span<byte> _span;

    public BlockBitView(Span<byte> span) => _spanReadOnly = _span = span;
    public BlockBitView(ReadOnlySpan<byte> span) => _spanReadOnly = span;

    public byte this[int bitOffset] => (byte) ((_spanReadOnly[bitOffset / 8] >>> (bitOffset % 8)) & 1);

    public byte Get8(int bitOffset) {
        var span = _spanReadOnly[(bitOffset / 8)..];
        bitOffset %= 8;

        return span.Length switch {
            0 => 0,
            1 when bitOffset == 0 => span[0],
            1 => unchecked((byte) (span[0] >>> bitOffset)),
            _ => unchecked((byte) ((span[0] | (span[1] << 8)) >>> bitOffset)),
        };
    }

    public byte Get8(int bitOffset, int bitCount) => (byte) (Get8(bitOffset) & ((1u << bitCount) - 1));

    public ushort Get16(int bitOffset) {
        var span = _spanReadOnly[(bitOffset / 8)..];
        bitOffset %= 8;

        return unchecked((ushort) (bitOffset == 0
            ? span.Length switch {
                0 => 0,
                1 => span[0],
                _ => BinaryPrimitives.ReadUInt16LittleEndian(span),
            }
            : span.Length switch {
                0 => 0,
                1 => span[0] >>> bitOffset,
                2 => BinaryPrimitives.ReadUInt16LittleEndian(span) >>> bitOffset,
                _ => (BinaryPrimitives.ReadUInt16LittleEndian(span) | (span[2] << 16)) >>> bitOffset,
            }));
    }

    public ushort Get16(int bitOffset, int bitCount) => (ushort) (Get16(bitOffset) & ((1u << bitCount) - 1));

    public uint Get32(int bitOffset) {
        var span = _spanReadOnly[(bitOffset / 8)..];
        bitOffset %= 8;

        return unchecked(bitOffset == 0
            ? span.Length switch {
                0 => 0u,
                1 => span[0],
                2 => BinaryPrimitives.ReadUInt16LittleEndian(span),
                3 => BinaryPrimitives.ReadUInt16LittleEndian(span) | ((uint) span[2] << 16),
                _ => BinaryPrimitives.ReadUInt32LittleEndian(span),
            }
            : span.Length switch {
                0 => 0u,
                1 => (uint) span[0] >>> bitOffset,
                2 => (uint) BinaryPrimitives.ReadUInt16LittleEndian(span) >>> bitOffset,
                3 => (BinaryPrimitives.ReadUInt16LittleEndian(span) | ((uint) span[2] << 16)) >>> bitOffset,
                4 => BinaryPrimitives.ReadUInt32LittleEndian(span) >>> bitOffset,
                _ => (BinaryPrimitives.ReadUInt32LittleEndian(span) >>> bitOffset) | ((uint) span[4] << (32 - bitOffset)),
            });
    }

    public uint Get32(int bitOffset, int bitCount) => Get32(bitOffset) & ((1u << bitCount) - 1);

    public ulong Get64(int bitOffset) {
        var span = _spanReadOnly[(bitOffset / 8)..];
        bitOffset %= 8;

        return bitOffset == 0
            ? span.Length switch {
                0 => 0u,
                1 => span[0],
                2 => BinaryPrimitives.ReadUInt16LittleEndian(span),
                3 => BinaryPrimitives.ReadUInt16LittleEndian(span) | ((uint) span[2] << 16),
                4 => BinaryPrimitives.ReadUInt32LittleEndian(span),
                5 => BinaryPrimitives.ReadUInt32LittleEndian(span) | ((ulong) span[4] << 32),
                6 => BinaryPrimitives.ReadUInt32LittleEndian(span) | ((ulong) BinaryPrimitives.ReadUInt16LittleEndian(span) << 32),
                7 => BinaryPrimitives.ReadUInt32LittleEndian(span) | ((ulong) BinaryPrimitives.ReadUInt16LittleEndian(span) << 32) | ((ulong) span[6] << 48),
                _ => BinaryPrimitives.ReadUInt64LittleEndian(span),
            }
            : span.Length switch {
                0 => 0u,
                1 => (uint) span[0] >>> bitOffset,
                2 => (uint) BinaryPrimitives.ReadUInt16LittleEndian(span) >>> bitOffset,
                3 => (BinaryPrimitives.ReadUInt16LittleEndian(span) | ((uint) span[2] << 16)) >>> bitOffset,
                4 => BinaryPrimitives.ReadUInt32LittleEndian(span) >>> bitOffset,
                5 => (BinaryPrimitives.ReadUInt32LittleEndian(span) | ((ulong) span[4] << 32)) >>> bitOffset,
                6 => (BinaryPrimitives.ReadUInt32LittleEndian(span) | ((ulong) BinaryPrimitives.ReadUInt16LittleEndian(span) << 32)) >>> bitOffset,
                7 => (BinaryPrimitives.ReadUInt32LittleEndian(span) | ((ulong) BinaryPrimitives.ReadUInt16LittleEndian(span) << 32) | ((ulong) span[6] << 48))
                    >>> bitOffset,
                8 => BinaryPrimitives.ReadUInt64LittleEndian(span) >>> bitOffset,
                _ => (BinaryPrimitives.ReadUInt64LittleEndian(span) >>> bitOffset) | ((ulong) span[8] << (64 - bitOffset)),
            };
    }

    public ulong Get64(int bitOffset, int bitCount) => Get64(bitOffset) & ((1ul << bitCount) - 1);
}
