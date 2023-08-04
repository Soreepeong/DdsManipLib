using System;
using System.Diagnostics;
using DdsManipLib.DirectDrawSurface.PixelFormats;
using DdsManipLib.Utilities;

namespace DdsManipLib.BcCodec.Bptc;

internal unsafe ref struct Bc6ParsedBlock {
    public const int MaxNumEndpoints = 4;
    public readonly Span<Vector3<int>> EndpointsFull;
    public Span<Vector3<int>> Endpoints;

    public Bc6Mode Mode;
    public byte PartitionIndex;
    public ulong Index;

    public Bc6ParsedBlock(Span<Vector3<int>> endpointsFull) {
        EndpointsFull = endpointsFull;
    }

    public bool ReadBlock(ReadOnlySpan<byte> block, bool signed) {
        var view = new BlockBitView(block);
        Mode = Bc6Mode.FromFirstByte(block[0]);
        if (Mode.ModeBits == 0)
            return false;

        Endpoints = EndpointsFull[..(Mode.Subsets * 2)];
        Endpoints[0] = new(view.Get16(5), view.Get16(15), view.Get16(25));
        switch (Mode.Mode) {
            case 2:
                Endpoints[0] = (Endpoints[0] & 0x3FF) | new Vector3<int>(block[40] << 10, block[49] << 10, block[59] << 10);
                break;
            case 6:
                Endpoints[0] = (Endpoints[0] & 0x3FF) | new Vector3<int>(block[39] << 10, block[50] << 10, block[59] << 10);
                break;
            case 10:
                Endpoints[0] = (Endpoints[0] & 0x3FF) | new Vector3<int>(block[39] << 10, block[49] << 10, block[60] << 10);
                break;
            case 7:
                Endpoints[0] = (Endpoints[0] & 0x3FF) | new Vector3<int>(block[44] << 10, block[54] << 10, block[64] << 10);
                break;
            case 11:
                Endpoints[0] = (Endpoints[0] & 0x3FF) | new Vector3<int>(
                    (block[44] << 10) | (block[43] << 11),
                    (block[54] << 10) | (block[53] << 11),
                    (block[64] << 10) | (block[63] << 11));
                break;
            case 15:
                Endpoints[0] = (Endpoints[0] & 0x3FF) | new Vector3<int>(
                    (block[44] << 10) | (block[43] << 11) | (block[42] << 12) | (block[41] << 13) | (block[40] << 14) | (block[39] << 15),
                    (block[54] << 10) | (block[53] << 11) | (block[52] << 12) | (block[51] << 13) | (block[50] << 14) | (block[49] << 15),
                    (block[64] << 10) | (block[63] << 11) | (block[62] << 12) | (block[61] << 13) | (block[60] << 14) | (block[59] << 15));
                break;
            default:
                Endpoints[0] &= (1 << Mode.EndpointBits) - 1;
                break;
        }

        if (signed)
            Endpoints[0] = RawToSInt(Endpoints[0], Mode.EndpointBits);

        Endpoints[1] = new(view.Get16(35), view.Get16(45), view.Get16(55));
        if (Mode.HasTransformedEndpoints) {
            Endpoints[1] &= new Vector3<int>((1 << Mode.DeltaBits.X) - 1, (1 << Mode.DeltaBits.Y) - 1, (1 << Mode.DeltaBits.Z) - 1);
            Endpoints[1] = RawToSInt(Endpoints[1], Mode.DeltaBits);
            Endpoints[1] = (Endpoints[0] + Endpoints[1]) & ((1 << Mode.EndpointBits) - 1);
        } else
            Endpoints[1] &= (1 << Mode.EndpointBits) - 1;

        if (signed)
            Endpoints[1] = RawToSInt(Endpoints[1], Mode.EndpointBits);

        if (Endpoints.Length >= 2) {
            Debug.Assert(Endpoints.Length == 4);
            Endpoints[2] = new(view.Get8(65, 5), view.Get8(41, 4), view.Get8(61, 4));
            switch (Mode.Mode) {
                case 0:
                    Endpoints[2] |= new Vector3<int>(0, view[2] << 4, view[3] << 4);
                    break;
                case 1:
                    Endpoints[2] |= new Vector3<int>(view[70] << 5, view[24] << 4 | view[2] << 5, view[14] << 4 | view[22] << 5);
                    break;
                case 2:
                    Endpoints[2] &= new Vector3<int>(0x1F, 0xF, 0xF);
                    break;
                case 6:
                    Endpoints[2].X &= 0xF;
                    Endpoints[2].Y |= view[75] << 4;
                    break;
                case 10:
                    Endpoints[2].X &= 0xF;
                    Endpoints[2].Z |= view[40] << 4;
                    break;
                case 14:
                    Endpoints[2] |= new Vector3<int>(0, view[24] << 4, view[14] << 4);
                    break;
                case 18:
                    Endpoints[2] |= new Vector3<int>(view[70] << 5, view[24] << 4, view[14] << 4);
                    break;
                case 22:
                    Endpoints[2] |= new Vector3<int>(0, view[24] << 4 | view[23] << 5, view[14] << 4);
                    break;
                case 26:
                    Endpoints[2] |= new Vector3<int>(0, view[24] << 4, view[14] << 4 | view[23] << 5);
                    break;
                case 30:
                    Endpoints[2].X = view.Get8(65, 6);
                    Endpoints[2].Y |= view[24] << 4 | view[21] << 5;
                    Endpoints[2].Z |= view[14] << 4 | view[22] << 5;
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }

            Endpoints[3] = new(view.Get8(71, 5), view.Get8(51, 4), 0);
            switch (Mode.Mode) {
                case 0:
                    Endpoints[3].Y |= view[40] << 4;
                    Endpoints[3].Z = view[50] | view[60] << 1 | view[70] << 2 | view[76] << 3 | view[4] << 4;
                    break;
                case 1:
                    Endpoints[3].X |= view[76] << 5;
                    Endpoints[3].Y |= view.Get8(3, 2) << 4;
                    Endpoints[3].Z = view.Get8(12, 2) | view[23] << 2 | view[32] << 3 | view[34] << 4 | view[33] << 5;
                    break;
                case 2:
                    Endpoints[3].Z = view[50] | view[60] << 1 | view[70] << 2 | view[76] << 3;
                    break;
                case 6:
                    Endpoints[3].X &= 0xF;
                    Endpoints[3].Y |= view[40] << 4;
                    Endpoints[3].Z = view[69] | view[60] << 1 | view[70] << 2 | view[76] << 3;
                    break;
                case 10:
                    Endpoints[3].X &= 0xF;
                    Endpoints[3].Z = view[50] | view[69] << 1 | view[70] << 2 | view[76] << 3 | view[75] << 4;
                    break;
                case 14:
                    Endpoints[3].Y |= view[40] << 4;
                    Endpoints[3].Z = view[50] | view[60] << 1 | view[70] << 2 | view[76] << 3 | view[34] << 4;
                    break;
                case 18:
                    Endpoints[3].X |= view[76] << 5;
                    Endpoints[3].Y |= view[13] << 4;
                    Endpoints[3].Z = view[50] | view[60] << 1 | view[23] << 2 | view[33] << 3 | view[34] << 4;
                    break;
                case 22:
                    Endpoints[3].Y |= view[40] << 4 | view[33] << 5;
                    Endpoints[3].Z = view[13] | view[60] << 1 | view[70] << 2 | view[76] << 3 | view[34] << 4;
                    break;
                case 26:
                    Endpoints[3].Y |= view[40] << 4;
                    Endpoints[3].Z = view[50] | view[13] << 1 | view[70] << 2 | view[76] << 3 | view[34] << 4 | view[33] << 5;
                    break;
                case 30:
                    Endpoints[3].X = view.Get8(71, 6);
                    Endpoints[3].Y |= view[11] << 4 | view[31] << 5;
                    Endpoints[3].Z = view.Get8(12, 2) | view[23] << 2 | view[32] << 3 | view[34] << 4 | view[33] << 5;
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }

            if (Mode.HasTransformedEndpoints) {
                Endpoints[2] = (Endpoints[0] + RawToSInt(Endpoints[2], Mode.DeltaBits)) & ((1 << Mode.EndpointBits) - 1);
                Endpoints[3] = (Endpoints[0] + RawToSInt(Endpoints[3], Mode.DeltaBits)) & ((1 << Mode.EndpointBits) - 1);
            }

            if (signed) {
                Endpoints[2] = RawToSInt(Endpoints[2], Mode.EndpointBits);
                Endpoints[3] = RawToSInt(Endpoints[3], Mode.EndpointBits);
            }
        }

        for (var i = 0; i < Endpoints.Length; i++)
            Endpoints[i] = Unquantize(signed, Endpoints[i], Mode.EndpointBits);

        PartitionIndex = Mode.PartitionBits == 0 ? (byte) 0 : view.Get8(77, 0);
        Index = view.Get64(Mode.Subsets == 2 ? 82 : 65);

        throw new NotImplementedException();
    }

    public int ColorIndexBitCount => Mode.Subsets == 1 ? 4 : 3;

    public int GetPartitionIndex(int itemIndex) =>
        Bc6Constants.PartitionTables[Mode.Subsets][PartitionIndex][itemIndex];

    public int GetIndexOffset(int bitCount, int index) {
        switch (Mode.Subsets) {
            case var _ when index is 0:
                return 0;
            case 1:
                return bitCount * index - 1;
            case 2:
                return index <= Bc6Constants.Subsets2AnchorIndices[PartitionIndex]
                    ? bitCount * index - 1
                    : bitCount * index - 2;
            default:
                Debug.Assert(false);
                return 0;
        }
    }

    /// <summary>
    /// Decrements bitCount by one if index is one of the anchor indices.
    /// </summary>
    public int GetIndexBitCount(int bitCount, int index) {
        switch (Mode.Subsets) {
            case var _ when index is 0:
                return bitCount - 1;
            case 2: {
                var anchorIndex = Bc6Constants.Subsets2AnchorIndices[PartitionIndex];
                if (index == anchorIndex)
                    return bitCount - 1;

                break;
            }
        }

        return bitCount;
    }

    public int GetColorIndex(int bitCount, int index) {
        if (bitCount == 0)
            return 0;
        var indexOffset = GetIndexOffset(bitCount, index);
        var indexBitCount = GetIndexBitCount(bitCount, index);
        return (int) ((Index >> indexOffset) & ((1ul << indexBitCount) - 1));
    }

    private static Vector3<int> RawToSInt(in Vector3<int> rawValue, int sourceBits) => new(
        PixelFormatUtilities.RawToSInt((uint) rawValue.X, sourceBits),
        PixelFormatUtilities.RawToSInt((uint) rawValue.Y, sourceBits),
        PixelFormatUtilities.RawToSInt((uint) rawValue.Z, sourceBits));

    private static Vector3<int> RawToSInt(in Vector3<int> rawValue, in Vector3<byte> sourceBits) => new(
        PixelFormatUtilities.RawToSInt((uint) rawValue.X, sourceBits.X),
        PixelFormatUtilities.RawToSInt((uint) rawValue.Y, sourceBits.Y),
        PixelFormatUtilities.RawToSInt((uint) rawValue.Z, sourceBits.Z));

    public static Vector3<int> Unquantize(bool signed, in Vector3<int> comp, int uBitsPerComp) => new(
        Unquantize(signed, comp.X, uBitsPerComp),
        Unquantize(signed, comp.Y, uBitsPerComp),
        Unquantize(signed, comp.Z, uBitsPerComp));

    public static int Unquantize(bool signed, int comp, int uBitsPerComp) {
        int unq;
        var sign = false;
        if (signed) {
            if (uBitsPerComp >= 16)
                unq = comp;
            else {
                if (comp < 0) {
                    sign = true;
                    comp = -comp;
                }

                if (comp == 0)
                    unq = 0;
                else if (comp >= (1 << (uBitsPerComp - 1)) - 1)
                    unq = 0x7FFF;
                else
                    unq = ((comp << 15) + 0x4000) >> (uBitsPerComp - 1);

                if (sign)
                    unq = -unq;
            }
        } else {
            if (uBitsPerComp >= 15)
                unq = comp;
            else if (comp == 0)
                unq = 0;
            else if (comp == (1 << uBitsPerComp) - 1)
                unq = 0xFFFF;
            else
                unq = ((comp << 16) + 0x8000) >> uBitsPerComp;
        }

        return unq;
    }

    public static float FinishUnquantize(bool signed, int comp) {
        if (signed) {
            comp = (comp < 0) ? -(((-comp) * 31) >> 5) : (comp * 31) >> 5; // scale the magnitude by 31/32
            var s = 0;
            if (comp < 0) {
                s = 0x8000;
                comp = -comp;
            }

            return (float) BitConverter.UInt16BitsToHalf((ushort) (s | comp));
        } else {
            // TODO: doesn't this branch drop a bit's worth of precision?
            comp = (comp * 31) >> 6; // scale the magnitude by 31/64
            return (float) BitConverter.UInt16BitsToHalf((ushort) comp);
        }
    }
}