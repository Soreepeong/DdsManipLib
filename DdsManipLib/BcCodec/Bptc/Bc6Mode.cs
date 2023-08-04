using DdsManipLib.Utilities;

namespace DdsManipLib.BcCodec.Bptc;

internal readonly struct Bc6Mode {
    // See:
    // https://registry.khronos.org/DataFormat/specs/1.3/dataformat.1.3.html#bptc_bc6h
    // https://learn.microsoft.com/en-us/windows/win32/direct3d11/bc6h-format
    public readonly byte Mode;
    public readonly byte ModeBits;
    public readonly byte PartitionBits;
    public readonly byte EndpointBits;
    public readonly Vector3<byte> DeltaBits;

    public Bc6Mode(byte mode, byte modeBits, byte partitionBits, byte endpointBits, Vector3<byte> deltaBits) {
        Mode = mode;
        ModeBits = modeBits;
        PartitionBits = partitionBits;
        EndpointBits = endpointBits;
        DeltaBits = deltaBits;
    }

    public byte Subsets => PartitionBits == 0 ? (byte) 1 : (byte) 2;

    public bool HasTransformedEndpoints => DeltaBits.X != 0;

    public static Bc6Mode FromModeIndex(int modeIndex) => modeIndex switch {
        0 => new(0, 2, 5, 10, new(5, 5, 5)),
        1 => new(1, 2, 5, 7, new(6, 6, 6)),
        2 => new(2, 5, 5, 11, new(5, 4, 4)),
        6 => new(6, 5, 5, 11, new(4, 5, 4)),
        10 => new(10, 5, 5, 11, new(4, 4, 5)),
        14 => new(14, 5, 5, 9, new(5, 5, 5)),
        18 => new(18, 5, 5, 8, new(6, 5, 5)),
        22 => new(22, 5, 5, 8, new(5, 6, 5)),
        26 => new(26, 5, 5, 8, new(5, 5, 6)),
        30 => new(30, 5, 5, 6, new()),
        3 => new(3, 5, 0, 10, new()),
        7 => new(7, 5, 0, 11, new(9, 9, 9)),
        11 => new(11, 5, 0, 12, new(8, 8, 8)),
        15 => new(15, 5, 0, 16, new(4, 4, 4)),
        _ => default,
    };

    public static Bc6Mode FromFirstByte(byte firstByte) => (firstByte & 0b11) is 0 or 1
        ? FromModeIndex(firstByte & 0b11)
        : FromModeIndex(firstByte & 0b11111);
}