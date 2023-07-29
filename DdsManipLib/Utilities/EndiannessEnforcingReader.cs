using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace DdsManipLib.Utilities;

/// <summary>
/// A subclass of <see cref="BinaryReader"/> that ensures that the values read are in the specified endianness.
/// </summary>
public class EndiannessEnforcingReader : BinaryReader {
    /// <inheritdoc/>
    public EndiannessEnforcingReader(Stream input) : base(input) { }

    /// <inheritdoc/>
    public EndiannessEnforcingReader(Stream input, Encoding encoding) : base(input, encoding) { }

    /// <inheritdoc/>
    public EndiannessEnforcingReader(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen) { }

    /// <summary>
    /// Interpret values in big endian.
    /// </summary>
    public bool IsBigEndian { get; set; }

    /// <summary>
    /// Alias of <see cref="BinaryReader.BaseStream" />.<see cref="Stream.Position"/>.
    /// </summary>
    public long Position {
        get => BaseStream.Position;
        set => BaseStream.Position = value;
    }

    /// <summary>
    /// Alias of <see cref="BinaryReader.BaseStream" />.<see cref="Stream.Position"/>, but in <see cref="int"/>.
    /// </summary>
    public int PositionI32 {
        get => checked((int) BaseStream.Position);
        set => BaseStream.Position = value;
    }

    /// <summary>
    /// Alias of <see cref="BinaryReader.BaseStream" />.<see cref="Stream.Length"/>.
    /// </summary>
    public long Length => BaseStream.Length;

    /// <summary>
    /// Alias of <see cref="BinaryReader.BaseStream" />.<see cref="Stream.Length"/>, but in <see cref="int"/>.
    /// </summary>
    public int LengthI32 => checked((int) BaseStream.Length);

    /// <inheritdoc/>
    public override decimal ReadDecimal() {
        Span<byte> buffer = stackalloc byte[sizeof(decimal)];
        BaseStream.ReadExactly(buffer);

        if (IsBigEndian == BitConverter.IsLittleEndian) {
            for (var i = 0; i < buffer.Length; i += sizeof(int))
                buffer.Slice(i, i + sizeof(int)).Reverse();
        }

        unsafe {
            fixed (byte* p = &buffer.GetPinnableReference()) {
                Span<int> bufferInts = new(p, buffer.Length);
                return new(bufferInts);
            }
        }
    }

    /// <inheritdoc />
    public override Half ReadHalf() {
        Span<byte> buffer = stackalloc byte[sizeof(ushort)];
        BaseStream.ReadExactly(buffer);
        return IsBigEndian
            ? BinaryPrimitives.ReadHalfBigEndian(buffer)
            : BinaryPrimitives.ReadHalfLittleEndian(buffer);
    }

    /// <inheritdoc/>
    public override float ReadSingle() {
        Span<byte> buffer = stackalloc byte[sizeof(float)];
        BaseStream.ReadExactly(buffer);
        return IsBigEndian
            ? BinaryPrimitives.ReadSingleBigEndian(buffer)
            : BinaryPrimitives.ReadSingleLittleEndian(buffer);
    }

    /// <inheritdoc/>
    public override double ReadDouble() {
        Span<byte> buffer = stackalloc byte[sizeof(double)];
        BaseStream.ReadExactly(buffer);
        return IsBigEndian
            ? BinaryPrimitives.ReadDoubleBigEndian(buffer)
            : BinaryPrimitives.ReadDoubleLittleEndian(buffer);
    }

    /// <inheritdoc/>
    public override short ReadInt16() {
        Span<byte> buffer = stackalloc byte[sizeof(short)];
        BaseStream.ReadExactly(buffer);
        return IsBigEndian
            ? BinaryPrimitives.ReadInt16BigEndian(buffer)
            : BinaryPrimitives.ReadInt16LittleEndian(buffer);
    }

    /// <inheritdoc/>
    public override ushort ReadUInt16() {
        Span<byte> buffer = stackalloc byte[sizeof(ushort)];
        BaseStream.ReadExactly(buffer);
        return IsBigEndian
            ? BinaryPrimitives.ReadUInt16BigEndian(buffer)
            : BinaryPrimitives.ReadUInt16LittleEndian(buffer);
    }

    /// <inheritdoc/>
    public override int ReadInt32() {
        Span<byte> buffer = stackalloc byte[sizeof(int)];
        BaseStream.ReadExactly(buffer);
        return IsBigEndian
            ? BinaryPrimitives.ReadInt32BigEndian(buffer)
            : BinaryPrimitives.ReadInt32LittleEndian(buffer);
    }

    /// <inheritdoc/>
    public override uint ReadUInt32() {
        Span<byte> buffer = stackalloc byte[sizeof(uint)];
        BaseStream.ReadExactly(buffer);
        return IsBigEndian
            ? BinaryPrimitives.ReadUInt32BigEndian(buffer)
            : BinaryPrimitives.ReadUInt32LittleEndian(buffer);
    }

    /// <inheritdoc/>
    public override long ReadInt64() {
        Span<byte> buffer = stackalloc byte[sizeof(long)];
        BaseStream.ReadExactly(buffer);
        return IsBigEndian
            ? BinaryPrimitives.ReadInt64BigEndian(buffer)
            : BinaryPrimitives.ReadInt64LittleEndian(buffer);
    }

    /// <inheritdoc/>
    public override ulong ReadUInt64() {
        Span<byte> buffer = stackalloc byte[sizeof(ulong)];
        BaseStream.ReadExactly(buffer);
        return IsBigEndian
            ? BinaryPrimitives.ReadUInt64BigEndian(buffer)
            : BinaryPrimitives.ReadUInt64LittleEndian(buffer);
    }

    /// <summary>
    /// Enforce endianness for the scope (until the disposure of the returned object).
    /// </summary>
    public IDisposable ScopedLittleEndian(bool useLittleEndian = true) {
        var b = new EndiannessRestorer(this);
        IsBigEndian = !useLittleEndian;
        return b;
    }

    /// <summary>
    /// Enforce endianness for the scope (until the disposure of the returned object).
    /// </summary>
    public IDisposable ScopedBigEndian(bool useBigEndian = true) => ScopedLittleEndian(!useBigEndian);

    /// <summary>
    /// Create a new instance of <see cref="EndiannessEnforcingReader"/> from the given array of bytes.
    /// </summary>
    public static EndiannessEnforcingReader FromBytes(byte[] b, bool useBigEndian = false) =>
        new(new MemoryStream(b, false)) {IsBigEndian = useBigEndian};

    internal sealed class EndiannessRestorer : IDisposable {
        private readonly bool _previousBigEndian;
        private readonly EndiannessEnforcingReader _reader;
        private bool _isDisposed;

        public EndiannessRestorer(EndiannessEnforcingReader reader) {
            _reader = reader;
            _previousBigEndian = reader.IsBigEndian;
        }

        public void Dispose() {
            if (!_isDisposed)
                _reader.IsBigEndian = _previousBigEndian;
            _isDisposed = true;
        }
    }
}
