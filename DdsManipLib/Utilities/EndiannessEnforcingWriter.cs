using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace DdsManipLib.Utilities;

/// <summary>
/// A subclass of <see cref="BinaryWriter"/> that ensures that the values written are in the specified endianness.
/// </summary>
public class EndiannessEnforcingWriter : BinaryWriter {
    
    /// <inheritdoc/>
    public EndiannessEnforcingWriter(Stream inStream)
        : base(inStream) { }

    /// <inheritdoc/>
    public EndiannessEnforcingWriter(Stream inStream, Encoding encoding)
        : base(inStream, encoding) { }

    /// <inheritdoc/>
    public EndiannessEnforcingWriter(Stream inStream, Encoding encoding, bool leaveOpen)
        : base(inStream, encoding, leaveOpen) { }

    /// <summary>
    /// Write values in big endian.
    /// </summary>
    public bool IsBigEndian { get; set; }

    /// <summary>
    /// Alias of <see cref="BinaryWriter.BaseStream" />.<see cref="Stream.Position"/>.
    /// </summary>
    public long Position {
        get => BaseStream.Position;
        set => BaseStream.Position = value;
    }

    /// <summary>
    /// Alias of <see cref="BinaryWriter.BaseStream" />.<see cref="Stream.Position"/>, but in <see cref="int"/>.
    /// </summary>
    public int PositionI32 {
        get => checked((int) BaseStream.Position);
        set => BaseStream.Position = value;
    }

    /// <summary>
    /// Alias of <see cref="BinaryWriter.BaseStream" />.<see cref="Stream.Length"/>.
    /// </summary>
    public long Length => BaseStream.Length;

    /// <summary>
    /// Alias of <see cref="BinaryWriter.BaseStream" />.<see cref="Stream.Length"/>, but in <see cref="int"/>.
    /// </summary>
    public int LengthI32 => checked((int) BaseStream.Length);

    /// <inheritdoc/>
    public override void Write(decimal value) {
        Span<byte> buffer = stackalloc byte[sizeof(decimal)];
        unsafe {
            fixed (byte* p = &buffer.GetPinnableReference()) {
                Span<int> bufferInts = new(p, buffer.Length / sizeof(int));
                decimal.GetBits(value, bufferInts);
            }
        }

        if (IsBigEndian == BitConverter.IsLittleEndian) {
            for (var i = 0; i < buffer.Length; i += sizeof(int))
                buffer.Slice(i, i + sizeof(int)).Reverse();
        }

        OutStream.Write(buffer);
    }

    /// <inheritdoc/>
    public override void Write(Half value) {
        Span<byte> buffer = stackalloc byte[sizeof(ushort)];
        if (IsBigEndian)
            BinaryPrimitives.WriteHalfBigEndian(buffer, value);
        else
            BinaryPrimitives.WriteHalfLittleEndian(buffer, value);
        OutStream.Write(buffer);
    }

    /// <inheritdoc/>
    public override void Write(float value) {
        Span<byte> buffer = stackalloc byte[sizeof(float)];
        if (IsBigEndian)
            BinaryPrimitives.WriteSingleBigEndian(buffer, value);
        else
            BinaryPrimitives.WriteSingleLittleEndian(buffer, value);
        OutStream.Write(buffer);
    }

    /// <inheritdoc/>
    public override void Write(double value) {
        Span<byte> buffer = stackalloc byte[sizeof(double)];
        if (IsBigEndian)
            BinaryPrimitives.WriteDoubleBigEndian(buffer, value);
        else
            BinaryPrimitives.WriteDoubleLittleEndian(buffer, value);
        OutStream.Write(buffer);
    }

    /// <inheritdoc/>
    public override void Write(short value) {
        Span<byte> buffer = stackalloc byte[sizeof(short)];
        if (IsBigEndian)
            BinaryPrimitives.WriteInt16BigEndian(buffer, value);
        else
            BinaryPrimitives.WriteInt16LittleEndian(buffer, value);
        OutStream.Write(buffer);
    }

    /// <inheritdoc/>
    public override void Write(ushort value) {
        Span<byte> buffer = stackalloc byte[sizeof(ushort)];
        if (IsBigEndian)
            BinaryPrimitives.WriteUInt16BigEndian(buffer, value);
        else
            BinaryPrimitives.WriteUInt16LittleEndian(buffer, value);
        OutStream.Write(buffer);
    }

    /// <inheritdoc/>
    public override void Write(int value) {
        Span<byte> buffer = stackalloc byte[sizeof(int)];
        if (IsBigEndian)
            BinaryPrimitives.WriteInt32BigEndian(buffer, value);
        else
            BinaryPrimitives.WriteInt32LittleEndian(buffer, value);
        OutStream.Write(buffer);
    }

    /// <inheritdoc/>
    public override void Write(uint value) {
        Span<byte> buffer = stackalloc byte[sizeof(uint)];
        if (IsBigEndian)
            BinaryPrimitives.WriteUInt32BigEndian(buffer, value);
        else
            BinaryPrimitives.WriteUInt32LittleEndian(buffer, value);
        OutStream.Write(buffer);
    }

    /// <inheritdoc/>
    public override void Write(long value) {
        Span<byte> buffer = stackalloc byte[sizeof(long)];
        if (IsBigEndian)
            BinaryPrimitives.WriteInt64BigEndian(buffer, value);
        else
            BinaryPrimitives.WriteInt64LittleEndian(buffer, value);
        OutStream.Write(buffer);
    }

    /// <inheritdoc/>
    public override void Write(ulong value) {
        Span<byte> buffer = stackalloc byte[sizeof(ulong)];
        if (IsBigEndian)
            BinaryPrimitives.WriteUInt64BigEndian(buffer, value);
        else
            BinaryPrimitives.WriteUInt64LittleEndian(buffer, value);
        OutStream.Write(buffer);
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

    internal sealed class EndiannessRestorer : IDisposable {
        private readonly bool _previousBigEndian;
        private readonly EndiannessEnforcingWriter _writer;
        private bool _isDisposed = false;

        public EndiannessRestorer(EndiannessEnforcingWriter writer) {
            _writer = writer;
            _previousBigEndian = writer.IsBigEndian;
        }

        public void Dispose() {
            if(!_isDisposed)
                _writer.IsBigEndian = _previousBigEndian;
            _isDisposed = true;
        }
    }
}
