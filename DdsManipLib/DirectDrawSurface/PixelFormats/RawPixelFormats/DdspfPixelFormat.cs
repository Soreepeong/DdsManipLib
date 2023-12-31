﻿using System;
using System.Buffers.Binary;
using System.Numerics;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public abstract class DdspfPixelFormat : RawPixelFormat, IEquatable<DdspfPixelFormat> {
    internal DdspfPixelFormat(int nbits, int rshift, int rbits, int gshift, int gbits, int bshift, int bbits, int ashift, int abits) :
        base(abits == 0 ? AlphaType.None : AlphaType.Straight) {
        BitsPerPixel = nbits;
        BytesPerPixel = (nbits + 7) / 8;
        RedShift = rshift;
        RedBits = rbits;
        RedMax = (1u << rbits) - 1u;
        GreenShift = gshift;
        GreenBits = gbits;
        GreenMax = (1u << gbits) - 1u;
        BlueShift = bshift;
        BlueBits = bbits;
        BlueMax = (1u << bbits) - 1u;
        AlphaShift = ashift;
        AlphaBits = abits;
        AlphaMax = (1u << abits) - 1u;
    }

    public int RedShift { get; }
    public int RedBits { get; }
    public uint RedMax { get; }
    public int GreenShift { get; }
    public int GreenBits { get; }
    public uint GreenMax { get; }
    public int BlueShift { get; }
    public int BlueBits { get; }
    public uint BlueMax { get; }
    public int AlphaShift { get; }
    public int AlphaBits { get; }
    public uint AlphaMax { get; }

    public override int BitsPerPixel { get; }
    public override int BytesPerPixel { get; }

    public override DdsPixelFormat DdsPixelFormat =>
        DdsPixelFormat.FromRgba(BitsPerPixel, RedMax << RedShift, GreenMax << GreenShift, BlueMax << BlueShift, AlphaMax << AlphaShift);

    public uint GetRaw(ReadOnlySpan<byte> pixel) => BytesPerPixel switch {
        0 => 0u,
        1 => pixel[0],
        2 => BinaryPrimitives.ReadUInt16LittleEndian(pixel),
        3 => BinaryPrimitives.ReadUInt16LittleEndian(pixel) | (uint) (pixel[2] << 16),
        4 => BinaryPrimitives.ReadUInt32LittleEndian(pixel),
        _ => throw new NotSupportedException(),
    };

    public void SetRaw(Span<byte> pixel, uint value) {
        switch (BytesPerPixel) {
            case 0:
                break;
            case 4:
                pixel[3] = (byte) (value >> 24);
                goto case 3;
            case 3:
                pixel[2] = (byte) (value >> 16);
                goto case 2;
            case 2:
                pixel[1] = (byte) (value >> 8);
                goto case 1;
            case 1:
                pixel[0] = (byte) value;
                break;
        }
    }

    public T GetRaw<T>(ReadOnlySpan<byte> pixel, int shift) where T : unmanaged, IBinaryInteger<T> => T.CreateTruncating(GetRaw(pixel) >> shift);

    public void UpdateRaw<T>(Span<byte> pixel, int shift, int bits, T value) where T : unmanaged, IBinaryInteger<T> =>
        SetRaw(pixel, GetRaw(pixel) & ~(((1u << bits) - 1u) << shift) | (uint.CreateTruncating(value) << shift));

    public T GetUNorm<T>(ReadOnlySpan<byte> pixel, int shift, int bits) where T : unmanaged, IBinaryInteger<T>, IUnsignedNumber<T> =>
        PixelFormatUtilities.RawToUNorm(GetRaw<T>(pixel, shift), bits);

    public float GetFloatFromUNorm<T>(ReadOnlySpan<byte> pixel, int shift, int bits)
        where T : unmanaged, IBinaryInteger<T>, IUnsignedNumber<T>, IMinMaxValue<T> =>
        float.CreateSaturating(GetUNorm<T>(pixel, shift, bits)) / float.CreateTruncating(T.MaxValue);

    public void UpdateUNorm<T>(Span<byte> pixel, int shift, int bits, T value) where T : unmanaged, IBinaryInteger<T>, IUnsignedNumber<T>
        => UpdateRaw(pixel, shift, bits, PixelFormatUtilities.UNormToRaw(value, bits));

    public void UpdateUNorm<T>(Span<byte> pixel, int shift, int bits, float value) where T : unmanaged, IBinaryInteger<T>, IUnsignedNumber<T>
        => UpdateRaw(pixel, shift, bits, PixelFormatUtilities.FloatToUNormRaw<T>(value, bits));

    /// <inheritdoc/>
    public bool Equals(DdspfPixelFormat? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (other.GetType() != GetType()) return false;
        return RedShift == other.RedShift && RedBits == other.RedBits && GreenShift == other.GreenShift && GreenBits == other.GreenBits &&
            BlueShift == other.BlueShift && BlueBits == other.BlueBits && AlphaShift == other.AlphaShift && AlphaBits == other.AlphaBits &&
            BitsPerPixel == other.BitsPerPixel;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as DdspfPixelFormat);

    /// <inheritdoc/>
    public override int GetHashCode() {
        var hashCode = new HashCode();
        hashCode.Add(RedShift);
        hashCode.Add(RedBits);
        hashCode.Add(GreenShift);
        hashCode.Add(GreenBits);
        hashCode.Add(BlueShift);
        hashCode.Add(BlueBits);
        hashCode.Add(AlphaShift);
        hashCode.Add(AlphaBits);
        hashCode.Add(BitsPerPixel);
        return hashCode.ToHashCode();
    }

    public static bool operator ==(DdspfPixelFormat? left, DdspfPixelFormat? right) => Equals(left, right);

    public static bool operator !=(DdspfPixelFormat? left, DdspfPixelFormat? right) => !Equals(left, right);

    public override void ClearPixel(Span<byte> pixel) {
        if (AlphaBits == 0)
            pixel[..BytesPerPixel].Clear();
        else
            SetRaw(pixel, AlphaMax << AlphaShift);
    }
}
