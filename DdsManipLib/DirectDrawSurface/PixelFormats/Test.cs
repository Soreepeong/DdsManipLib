using System;
using System.Buffers.Binary;
using System.Numerics;
using DdsManipLib.BcCodec.SquishInternal;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public static class Test {
    public interface IPixelFormat {
        public DxgiFormat DxgiFormat { get; }
        public DdsPixelFormat DdsPixelFormat { get; }
    }

    public interface IRawPixelFormat : IPixelFormat {
        public int BitsPerPixel { get; }
        public int BytesPerPixel { get; }
    }

    public interface IRawRPixelFormat : IRawPixelFormat {
        public float GetRed(ReadOnlySpan<byte> pixel);
        public void SetRed(Span<byte> pixel, float value);
    }

    public interface IRawRPixelFormat<T> : IRawRPixelFormat
        where T : unmanaged {
        public T GetRedTyped(ReadOnlySpan<byte> pixel);
        public void SetRed(Span<byte> pixel, T value);
    }

    public interface IRawGPixelFormat : IRawPixelFormat {
        public float GetGreen(ReadOnlySpan<byte> pixel);
        public void SetGreen(Span<byte> pixel, float value);
    }

    public interface IRawGPixelFormat<T> : IRawGPixelFormat
        where T : unmanaged {
        public T GetGreenTyped(ReadOnlySpan<byte> pixel);
        public void SetGreen(Span<byte> pixel, T value);
    }

    public interface IRawRgPixelFormat : IRawRPixelFormat, IRawGPixelFormat { }

    public interface IRawRgPixelFormat<T> : IRawRgPixelFormat, IRawRPixelFormat<T>, IRawGPixelFormat<T>
        where T : unmanaged { }

    public interface IRawRgbPixelFormat : IRawRgPixelFormat {
        public float GetBlue(ReadOnlySpan<byte> pixel);
        public void SetBlue(Span<byte> pixel, float value);
        public Vector3 GetRgb(ReadOnlySpan<byte> pixel) => new(GetRed(pixel), GetGreen(pixel), GetBlue(pixel));

        public void SetRgb(Span<byte> pixel, Vector3 rgb) {
            SetRed(pixel, rgb.X);
            SetGreen(pixel, rgb.Y);
            SetBlue(pixel, rgb.Z);
        }
    }

    public interface IRawRgbPixelFormat<T> : IRawRgbPixelFormat, IRawRgPixelFormat<T>
        where T : unmanaged {
        public T GetBlueTyped(ReadOnlySpan<byte> pixel);
        public void SetBlue(Span<byte> pixel, T value);
    }

    public interface IRawAlphaPixelFormat : IRawPixelFormat {
        public AlphaType AlphaType { get; }
        public float GetAlpha(ReadOnlySpan<byte> pixel);
        public void SetAlpha(Span<byte> pixel, float value);
    }

    public interface IRawAlphaPixelFormat<T> : IRawAlphaPixelFormat {
        public T GetAlphaTyped(ReadOnlySpan<byte> pixel);
        public void SetAlpha(Span<byte> pixel, T value);
    }

    public interface IRawRgbaPixelFormat : IRawRgbPixelFormat, IRawAlphaPixelFormat {
        public Vector4 GetRgba(ReadOnlySpan<byte> pixel) => new(GetRed(pixel), GetGreen(pixel), GetBlue(pixel), GetAlpha(pixel));

        public void SetRgba(Span<byte> pixel, Vector4 rgba) {
            SetRed(pixel, rgba.X);
            SetGreen(pixel, rgba.Y);
            SetBlue(pixel, rgba.Z);
            SetAlpha(pixel, rgba.W);
        }

        Vector3 IRawRgbPixelFormat.GetRgb(ReadOnlySpan<byte> pixel) => GetRgba(pixel).DropW();

        void IRawRgbPixelFormat.SetRgb(Span<byte> pixel, Vector3 rgb) => SetRgba(pixel, new(rgb, 1f));
    }

    public interface IRawDPixelFormat : IRawPixelFormat {
        public float GetDepth(ReadOnlySpan<byte> pixel);
        public void SetDepth(Span<byte> pixel, float value);
    }

    public interface IRawDPixelFormat<T> : IRawDPixelFormat
        where T : unmanaged {
        public T GetDepthTyped(ReadOnlySpan<byte> pixel);
        public void SetDepth(Span<byte> pixel, T value);
    }

    public interface IRawSPixelFormat : IRawPixelFormat {
        public float GetStencil(ReadOnlySpan<byte> pixel);
        public void SetStencil(Span<byte> pixel, float value);
    }

    public interface IRawSPixelFormat<T> : IRawSPixelFormat
        where T : unmanaged {
        public T GetStencilTyped(ReadOnlySpan<byte> pixel);
        public void SetStencil(Span<byte> pixel, T value);
    }

    public sealed class R32G32B32A32TypelessPixelFormat
        : IRawRgbaPixelFormat
            , IRawRgbPixelFormat<uint>, IRawRgbPixelFormat<int>, IRawRgbPixelFormat<float>
            , IRawAlphaPixelFormat<uint>, IRawAlphaPixelFormat<int>, IRawAlphaPixelFormat<float> {
        public const int OffsetR = 0;
        public const int OffsetG = 4;
        public const int OffsetB = 8;
        public const int OffsetA = 12;

        public DxgiFormat DxgiFormat => DxgiFormat.R32G32B32A32Typeless;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 128;
        public int BytesPerPixel => 32;
        public AlphaType AlphaType => AlphaType.Straight;
        public float GetRed(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetR..]);
        public float GetGreen(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetG..]);
        public float GetBlue(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetB..]);
        public float GetAlpha(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetA..]);
        float IRawRPixelFormat<float>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetR..]);
        float IRawGPixelFormat<float>.GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetG..]);
        float IRawRgbPixelFormat<float>.GetBlueTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetB..]);
        float IRawAlphaPixelFormat<float>.GetAlphaTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetA..]);
        uint IRawRPixelFormat<uint>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetR..]);
        uint IRawGPixelFormat<uint>.GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetG..]);
        uint IRawRgbPixelFormat<uint>.GetBlueTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetB..]);
        uint IRawAlphaPixelFormat<uint>.GetAlphaTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetA..]);
        int IRawRPixelFormat<int>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetR..]);
        int IRawGPixelFormat<int>.GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetG..]);
        int IRawRgbPixelFormat<int>.GetBlueTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetB..]);
        int IRawAlphaPixelFormat<int>.GetAlphaTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetA..]);
        public void SetRed(Span<byte> pixel, float value) => BinaryPrimitives.WriteSingleLittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, float value) => BinaryPrimitives.WriteSingleLittleEndian(pixel[OffsetG..], value);
        public void SetBlue(Span<byte> pixel, float value) => BinaryPrimitives.WriteSingleLittleEndian(pixel[OffsetB..], value);
        public void SetAlpha(Span<byte> pixel, float value) => BinaryPrimitives.WriteSingleLittleEndian(pixel[OffsetA..], value);
        public void SetRed(Span<byte> pixel, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetG..], value);
        public void SetBlue(Span<byte> pixel, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetB..], value);
        public void SetAlpha(Span<byte> pixel, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetA..], value);
        public void SetRed(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetG..], value);
        public void SetBlue(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetB..], value);
        public void SetAlpha(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetA..], value);
    }

    public sealed class R32G32B32A32FloatPixelFormat
        : IRawRgbaPixelFormat
            , IRawRgbPixelFormat<float>, IRawAlphaPixelFormat<float> {
        public const int OffsetR = 0;
        public const int OffsetG = 4;
        public const int OffsetB = 8;
        public const int OffsetA = 12;

        public DxgiFormat DxgiFormat => DxgiFormat.R32G32B32A32Float;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 128;
        public int BytesPerPixel => 32;
        public AlphaType AlphaType => AlphaType.Straight;
        public float GetRed(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetR..]);
        public float GetGreen(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetG..]);
        public float GetBlue(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetB..]);
        public float GetAlpha(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetA..]);
        public float GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetR..]);
        public float GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetG..]);
        public float GetBlueTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetB..]);
        public float GetAlphaTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetA..]);
        public void SetRed(Span<byte> pixel, float value) => BinaryPrimitives.WriteSingleLittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, float value) => BinaryPrimitives.WriteSingleLittleEndian(pixel[OffsetG..], value);
        public void SetBlue(Span<byte> pixel, float value) => BinaryPrimitives.WriteSingleLittleEndian(pixel[OffsetB..], value);
        public void SetAlpha(Span<byte> pixel, float value) => BinaryPrimitives.WriteSingleLittleEndian(pixel[OffsetA..], value);
    }

    public sealed class R32G32B32A32UIntPixelFormat
        : IRawRgbaPixelFormat
            , IRawRgbPixelFormat<uint>, IRawAlphaPixelFormat<uint> {
        public const int OffsetR = 0;
        public const int OffsetG = 4;
        public const int OffsetB = 8;
        public const int OffsetA = 12;

        public DxgiFormat DxgiFormat => DxgiFormat.R32G32B32A32UInt;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 128;
        public int BytesPerPixel => 32;
        public AlphaType AlphaType => AlphaType.Straight;
        public float GetRed(ReadOnlySpan<byte> pixel) => GetRedTyped(pixel);
        public float GetGreen(ReadOnlySpan<byte> pixel) => GetGreenTyped(pixel);
        public float GetBlue(ReadOnlySpan<byte> pixel) => GetBlueTyped(pixel);
        public float GetAlpha(ReadOnlySpan<byte> pixel) => GetAlphaTyped(pixel);
        public uint GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetR..]);
        public uint GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetG..]);
        public uint GetBlueTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetB..]);
        public uint GetAlphaTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetA..]);
        public void SetRed(Span<byte> pixel, float value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetR..], uint.CreateTruncating(value));
        public void SetGreen(Span<byte> pixel, float value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetG..], uint.CreateTruncating(value));
        public void SetBlue(Span<byte> pixel, float value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetB..], uint.CreateTruncating(value));
        public void SetAlpha(Span<byte> pixel, float value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetA..], uint.CreateTruncating(value));
        public void SetRed(Span<byte> pixel, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetG..], value);
        public void SetBlue(Span<byte> pixel, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetB..], value);
        public void SetAlpha(Span<byte> pixel, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetA..], value);
    }

    public sealed class R32G32B32A32SIntPixelFormat
        : IRawRgbaPixelFormat
            , IRawRgbPixelFormat<int>, IRawAlphaPixelFormat<int> {
        public const int OffsetR = 0;
        public const int OffsetG = 4;
        public const int OffsetB = 8;
        public const int OffsetA = 12;

        public DxgiFormat DxgiFormat => DxgiFormat.R32G32B32A32SInt;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 128;
        public int BytesPerPixel => 32;
        public AlphaType AlphaType => AlphaType.Straight;
        public float GetRed(ReadOnlySpan<byte> pixel) => GetRedTyped(pixel);
        public float GetGreen(ReadOnlySpan<byte> pixel) => GetGreenTyped(pixel);
        public float GetBlue(ReadOnlySpan<byte> pixel) => GetBlueTyped(pixel);
        public float GetAlpha(ReadOnlySpan<byte> pixel) => GetAlphaTyped(pixel);
        public int GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetR..]);
        public int GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetG..]);
        public int GetBlueTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetB..]);
        public int GetAlphaTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetA..]);
        public void SetRed(Span<byte> pixel, float value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetR..], int.CreateTruncating(value));
        public void SetGreen(Span<byte> pixel, float value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetG..], int.CreateTruncating(value));
        public void SetBlue(Span<byte> pixel, float value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetB..], int.CreateTruncating(value));
        public void SetAlpha(Span<byte> pixel, float value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetA..], int.CreateTruncating(value));
        public void SetRed(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetG..], value);
        public void SetBlue(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetB..], value);
        public void SetAlpha(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetA..], value);
    }

    public sealed class R32G32B32TypelessPixelFormat
        : IRawRgbPixelFormat<uint>, IRawRgbPixelFormat<int>, IRawRgbPixelFormat<float> {
        public const int OffsetR = 0;
        public const int OffsetG = 4;
        public const int OffsetB = 8;

        public DxgiFormat DxgiFormat => DxgiFormat.R32G32B32Typeless;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 96;
        public int BytesPerPixel => 24;
        public float GetRed(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetR..]);
        public float GetGreen(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetG..]);
        public float GetBlue(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetB..]);
        float IRawRPixelFormat<float>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetR..]);
        float IRawGPixelFormat<float>.GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetG..]);
        float IRawRgbPixelFormat<float>.GetBlueTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetB..]);
        uint IRawRPixelFormat<uint>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetR..]);
        uint IRawGPixelFormat<uint>.GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetG..]);
        uint IRawRgbPixelFormat<uint>.GetBlueTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetB..]);
        int IRawRPixelFormat<int>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetR..]);
        int IRawGPixelFormat<int>.GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetG..]);
        int IRawRgbPixelFormat<int>.GetBlueTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetB..]);
        public void SetRed(Span<byte> pixel, float value) => BinaryPrimitives.WriteSingleLittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, float value) => BinaryPrimitives.WriteSingleLittleEndian(pixel[OffsetG..], value);
        public void SetBlue(Span<byte> pixel, float value) => BinaryPrimitives.WriteSingleLittleEndian(pixel[OffsetB..], value);
        public void SetRed(Span<byte> pixel, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetG..], value);
        public void SetBlue(Span<byte> pixel, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetB..], value);
        public void SetRed(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetG..], value);
        public void SetBlue(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetB..], value);
    }

    public sealed class R32G32B32FloatPixelFormat
        : IRawRgbPixelFormat<float> {
        public const int OffsetR = 0;
        public const int OffsetG = 4;
        public const int OffsetB = 8;

        public DxgiFormat DxgiFormat => DxgiFormat.R32G32B32Float;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 96;
        public int BytesPerPixel => 24;
        public float GetRed(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetR..]);
        public float GetGreen(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetG..]);
        public float GetBlue(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetB..]);
        public float GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetR..]);
        public float GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetG..]);
        public float GetBlueTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetG..]);
        public void SetRed(Span<byte> pixel, float value) => BinaryPrimitives.WriteSingleLittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, float value) => BinaryPrimitives.WriteSingleLittleEndian(pixel[OffsetG..], value);
        public void SetBlue(Span<byte> pixel, float value) => BinaryPrimitives.WriteSingleLittleEndian(pixel[OffsetB..], value);
    }

    public sealed class R32G32B32UIntPixelFormat
        : IRawRgbPixelFormat<uint> {
        public const int OffsetR = 0;
        public const int OffsetG = 4;
        public const int OffsetB = 8;

        public DxgiFormat DxgiFormat => DxgiFormat.R32G32B32UInt;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 96;
        public int BytesPerPixel => 24;
        public float GetRed(ReadOnlySpan<byte> pixel) => GetRedTyped(pixel);
        public float GetGreen(ReadOnlySpan<byte> pixel) => GetGreenTyped(pixel);
        public float GetBlue(ReadOnlySpan<byte> pixel) => GetBlueTyped(pixel);
        public uint GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetR..]);
        public uint GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetG..]);
        public uint GetBlueTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetB..]);
        public void SetRed(Span<byte> pixel, float value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetR..], uint.CreateTruncating(value));
        public void SetGreen(Span<byte> pixel, float value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetG..], uint.CreateTruncating(value));
        public void SetBlue(Span<byte> pixel, float value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetB..], uint.CreateTruncating(value));
        public void SetRed(Span<byte> pixel, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetG..], value);
        public void SetBlue(Span<byte> pixel, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetB..], value);
    }

    public sealed class R32G32B32SIntPixelFormat
        : IRawRgbPixelFormat<int> {
        public const int OffsetR = 0;
        public const int OffsetG = 4;
        public const int OffsetB = 8;

        public DxgiFormat DxgiFormat => DxgiFormat.R32G32B32SInt;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 96;
        public int BytesPerPixel => 24;
        public float GetRed(ReadOnlySpan<byte> pixel) => GetRedTyped(pixel);
        public float GetGreen(ReadOnlySpan<byte> pixel) => GetGreenTyped(pixel);
        public float GetBlue(ReadOnlySpan<byte> pixel) => GetBlueTyped(pixel);
        public int GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetR..]);
        public int GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetG..]);
        public int GetBlueTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetB..]);
        public void SetRed(Span<byte> pixel, float value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetR..], int.CreateTruncating(value));
        public void SetGreen(Span<byte> pixel, float value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetG..], int.CreateTruncating(value));
        public void SetBlue(Span<byte> pixel, float value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetB..], int.CreateTruncating(value));
        public void SetRed(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetG..], value);
        public void SetBlue(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetB..], value);
    }

    public sealed class R16G16B16A16TypelessPixelFormat
        : IRawRgbaPixelFormat
            , IRawRgbPixelFormat<ushort>, IRawRgbPixelFormat<short>, IRawRgbPixelFormat<Half>
            , IRawAlphaPixelFormat<ushort>, IRawAlphaPixelFormat<short>, IRawAlphaPixelFormat<Half> {
        public const int OffsetR = 0;
        public const int OffsetG = 2;
        public const int OffsetB = 4;
        public const int OffsetA = 6;

        public DxgiFormat DxgiFormat => DxgiFormat.R16G16B16A16Typeless;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 64;
        public int BytesPerPixel => 8;
        public AlphaType AlphaType => AlphaType.Straight;
        public float GetRed(ReadOnlySpan<byte> pixel) => (float) BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetR..]);
        public float GetGreen(ReadOnlySpan<byte> pixel) => (float) BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetG..]);
        public float GetBlue(ReadOnlySpan<byte> pixel) => (float) BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetB..]);
        public float GetAlpha(ReadOnlySpan<byte> pixel) => (float) BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetA..]);
        Half IRawRPixelFormat<Half>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetR..]);
        Half IRawGPixelFormat<Half>.GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetG..]);
        Half IRawRgbPixelFormat<Half>.GetBlueTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetB..]);
        Half IRawAlphaPixelFormat<Half>.GetAlphaTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetA..]);
        ushort IRawRPixelFormat<ushort>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetR..]);
        ushort IRawGPixelFormat<ushort>.GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetG..]);
        ushort IRawRgbPixelFormat<ushort>.GetBlueTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetB..]);
        ushort IRawAlphaPixelFormat<ushort>.GetAlphaTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetA..]);
        short IRawRPixelFormat<short>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetR..]);
        short IRawGPixelFormat<short>.GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetG..]);
        short IRawRgbPixelFormat<short>.GetBlueTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetB..]);
        short IRawAlphaPixelFormat<short>.GetAlphaTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetA..]);
        public void SetRed(Span<byte> pixel, float value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetR..], Half.CreateTruncating(value));
        public void SetGreen(Span<byte> pixel, float value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetG..], Half.CreateTruncating(value));
        public void SetBlue(Span<byte> pixel, float value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetB..], Half.CreateTruncating(value));
        public void SetAlpha(Span<byte> pixel, float value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetA..], Half.CreateTruncating(value));
        public void SetRed(Span<byte> pixel, Half value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, Half value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetG..], value);
        public void SetBlue(Span<byte> pixel, Half value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetB..], value);
        public void SetAlpha(Span<byte> pixel, Half value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetA..], value);
        public void SetRed(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetG..], value);
        public void SetBlue(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetB..], value);
        public void SetAlpha(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetA..], value);
        public void SetRed(Span<byte> pixel, short value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, short value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetG..], value);
        public void SetBlue(Span<byte> pixel, short value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetB..], value);
        public void SetAlpha(Span<byte> pixel, short value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetA..], value);
    }

    public sealed class R16G16B16A16FloatPixelFormat
        : IRawRgbaPixelFormat
            , IRawRgbPixelFormat<Half>, IRawAlphaPixelFormat<Half> {
        public const int OffsetR = 0;
        public const int OffsetG = 2;
        public const int OffsetB = 4;
        public const int OffsetA = 6;

        public DxgiFormat DxgiFormat => DxgiFormat.R16G16B16A16Float;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 64;
        public int BytesPerPixel => 8;
        public AlphaType AlphaType => AlphaType.Straight;
        public float GetRed(ReadOnlySpan<byte> pixel) => (float) BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetR..]);
        public float GetGreen(ReadOnlySpan<byte> pixel) => (float) BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetG..]);
        public float GetBlue(ReadOnlySpan<byte> pixel) => (float) BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetB..]);
        public float GetAlpha(ReadOnlySpan<byte> pixel) => (float) BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetA..]);
        public Half GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetR..]);
        public Half GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetG..]);
        public Half GetBlueTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetB..]);
        public Half GetAlphaTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetA..]);
        public void SetRed(Span<byte> pixel, float value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetR..], Half.CreateTruncating(value));
        public void SetGreen(Span<byte> pixel, float value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetG..], Half.CreateTruncating(value));
        public void SetBlue(Span<byte> pixel, float value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetB..], Half.CreateTruncating(value));
        public void SetAlpha(Span<byte> pixel, float value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetA..], Half.CreateTruncating(value));
        public void SetRed(Span<byte> pixel, Half value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, Half value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetG..], value);
        public void SetBlue(Span<byte> pixel, Half value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetB..], value);
        public void SetAlpha(Span<byte> pixel, Half value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetA..], value);
    }

    public sealed class R16G16B16A16UNormPixelFormat
        : IRawRgbaPixelFormat
            , IRawRgbPixelFormat<ushort>, IRawAlphaPixelFormat<ushort> {
        public const int OffsetR = 0;
        public const int OffsetG = 2;
        public const int OffsetB = 4;
        public const int OffsetA = 6;

        public DxgiFormat DxgiFormat => DxgiFormat.R16G16B16A16UNorm;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 64;
        public int BytesPerPixel => 8;
        public AlphaType AlphaType => AlphaType.Straight;
        public float GetRed(ReadOnlySpan<byte> pixel) => GetRedTyped(pixel) / 65535f;
        public float GetGreen(ReadOnlySpan<byte> pixel) => GetGreenTyped(pixel) / 65535f;
        public float GetBlue(ReadOnlySpan<byte> pixel) => GetBlueTyped(pixel) / 65535f;
        public float GetAlpha(ReadOnlySpan<byte> pixel) => GetAlphaTyped(pixel) / 65535f;
        public ushort GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetR..]);
        public ushort GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetG..]);
        public ushort GetBlueTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetB..]);
        public ushort GetAlphaTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetA..]);
        public void SetRed(Span<byte> pixel, float value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetR..], ushort.CreateTruncating(65536 * value));

        public void SetGreen(Span<byte> pixel, float value) =>
            BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetG..], ushort.CreateTruncating(65536 * value));

        public void SetBlue(Span<byte> pixel, float value) =>
            BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetB..], ushort.CreateTruncating(65536 * value));

        public void SetAlpha(Span<byte> pixel, float value) =>
            BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetA..], ushort.CreateTruncating(65536 * value));

        public void SetRed(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetG..], value);
        public void SetBlue(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetB..], value);
        public void SetAlpha(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetA..], value);
    }

    public sealed class R16G16B16A16UIntPixelFormat
        : IRawRgbaPixelFormat
            , IRawRgbPixelFormat<ushort>, IRawAlphaPixelFormat<ushort> {
        public const int OffsetR = 0;
        public const int OffsetG = 2;
        public const int OffsetB = 4;
        public const int OffsetA = 6;

        public DxgiFormat DxgiFormat => DxgiFormat.R16G16B16A16UInt;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 64;
        public int BytesPerPixel => 8;
        public AlphaType AlphaType => AlphaType.Straight;
        public float GetRed(ReadOnlySpan<byte> pixel) => GetRedTyped(pixel);
        public float GetGreen(ReadOnlySpan<byte> pixel) => GetGreenTyped(pixel);
        public float GetBlue(ReadOnlySpan<byte> pixel) => GetBlueTyped(pixel);
        public float GetAlpha(ReadOnlySpan<byte> pixel) => GetAlphaTyped(pixel);
        public ushort GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetR..]);
        public ushort GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetG..]);
        public ushort GetBlueTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetB..]);
        public ushort GetAlphaTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetA..]);
        public void SetRed(Span<byte> pixel, float value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetR..], ushort.CreateTruncating(value));
        public void SetGreen(Span<byte> pixel, float value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetG..], ushort.CreateTruncating(value));
        public void SetBlue(Span<byte> pixel, float value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetB..], ushort.CreateTruncating(value));
        public void SetAlpha(Span<byte> pixel, float value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetA..], ushort.CreateTruncating(value));
        public void SetRed(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetG..], value);
        public void SetBlue(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetB..], value);
        public void SetAlpha(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetA..], value);
    }

    public sealed class R16G16B16A16SNormPixelFormat
        : IRawRgbaPixelFormat
            , IRawRgbPixelFormat<short>, IRawAlphaPixelFormat<short> {
        public const int OffsetR = 0;
        public const int OffsetG = 2;
        public const int OffsetB = 4;
        public const int OffsetA = 6;

        public DxgiFormat DxgiFormat => DxgiFormat.R16G16B16A16SNorm;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 64;
        public int BytesPerPixel => 8;
        public AlphaType AlphaType => AlphaType.Straight;
        public float GetRed(ReadOnlySpan<byte> pixel) => Math.Max(-1f, GetRedTyped(pixel) / 32767f);
        public float GetGreen(ReadOnlySpan<byte> pixel) => Math.Max(-1f, GetGreenTyped(pixel) / 32767f);
        public float GetBlue(ReadOnlySpan<byte> pixel) => Math.Max(-1f, GetBlueTyped(pixel) / 32767f);
        public float GetAlpha(ReadOnlySpan<byte> pixel) => Math.Max(-1f, GetAlphaTyped(pixel) / 32767f);
        public short GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetR..]);
        public short GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetG..]);
        public short GetBlueTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetB..]);
        public short GetAlphaTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetA..]);
        public void SetRed(Span<byte> pixel, float value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetR..], short.CreateTruncating(32767 * value));
        public void SetGreen(Span<byte> pixel, float value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetG..], short.CreateTruncating(32767 * value));
        public void SetBlue(Span<byte> pixel, float value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetB..], short.CreateTruncating(32767 * value));
        public void SetAlpha(Span<byte> pixel, float value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetA..], short.CreateTruncating(32767 * value));
        public void SetRed(Span<byte> pixel, short value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, short value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetG..], value);
        public void SetBlue(Span<byte> pixel, short value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetB..], value);
        public void SetAlpha(Span<byte> pixel, short value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetA..], value);
    }

    public sealed class R16G16B16A16SIntPixelFormat
        : IRawRgbaPixelFormat
            , IRawRgbPixelFormat<short>, IRawAlphaPixelFormat<short> {
        public const int OffsetR = 0;
        public const int OffsetG = 2;
        public const int OffsetB = 4;
        public const int OffsetA = 6;

        public DxgiFormat DxgiFormat => DxgiFormat.R16G16B16A16SInt;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 64;
        public int BytesPerPixel => 8;
        public AlphaType AlphaType => AlphaType.Straight;
        public float GetRed(ReadOnlySpan<byte> pixel) => GetRedTyped(pixel);
        public float GetGreen(ReadOnlySpan<byte> pixel) => GetGreenTyped(pixel);
        public float GetBlue(ReadOnlySpan<byte> pixel) => GetBlueTyped(pixel);
        public float GetAlpha(ReadOnlySpan<byte> pixel) => GetAlphaTyped(pixel);
        public short GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetR..]);
        public short GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetG..]);
        public short GetBlueTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetB..]);
        public short GetAlphaTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetA..]);
        public void SetRed(Span<byte> pixel, float value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetR..], short.CreateTruncating(value));
        public void SetGreen(Span<byte> pixel, float value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetG..], short.CreateTruncating(value));
        public void SetBlue(Span<byte> pixel, float value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetB..], short.CreateTruncating(value));
        public void SetAlpha(Span<byte> pixel, float value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetA..], short.CreateTruncating(value));
        public void SetRed(Span<byte> pixel, short value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, short value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetG..], value);
        public void SetBlue(Span<byte> pixel, short value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetB..], value);
        public void SetAlpha(Span<byte> pixel, short value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetA..], value);
    }

    public sealed class R32G32TypelessPixelFormat
        : IRawRgPixelFormat<uint>, IRawRgPixelFormat<int>, IRawRgPixelFormat<float> {
        public const int OffsetR = 0;
        public const int OffsetG = 4;

        public DxgiFormat DxgiFormat => DxgiFormat.R32G32Typeless;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 64;
        public int BytesPerPixel => 8;
        public float GetRed(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetR..]);
        public float GetGreen(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetG..]);
        float IRawRPixelFormat<float>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetR..]);
        float IRawGPixelFormat<float>.GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetG..]);
        uint IRawRPixelFormat<uint>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetR..]);
        uint IRawGPixelFormat<uint>.GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetG..]);
        int IRawRPixelFormat<int>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetR..]);
        int IRawGPixelFormat<int>.GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetG..]);
        public void SetRed(Span<byte> pixel, float value) => BinaryPrimitives.WriteSingleLittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, float value) => BinaryPrimitives.WriteSingleLittleEndian(pixel[OffsetG..], value);
        public void SetRed(Span<byte> pixel, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetG..], value);
        public void SetRed(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetG..], value);
    }

    public sealed class R32G32FloatPixelFormat
        : IRawRgPixelFormat<float> {
        public const int OffsetR = 0;
        public const int OffsetG = 4;

        public DxgiFormat DxgiFormat => DxgiFormat.R32G32Float;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 64;
        public int BytesPerPixel => 8;
        public float GetRed(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetR..]);
        public float GetGreen(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetG..]);
        public float GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetR..]);
        public float GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetG..]);
        public void SetRed(Span<byte> pixel, float value) => BinaryPrimitives.WriteSingleLittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, float value) => BinaryPrimitives.WriteSingleLittleEndian(pixel[OffsetG..], value);
    }

    public sealed class R32G32UIntPixelFormat
        : IRawRgPixelFormat<uint> {
        public const int OffsetR = 0;
        public const int OffsetG = 4;

        public DxgiFormat DxgiFormat => DxgiFormat.R32G32UInt;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 64;
        public int BytesPerPixel => 8;
        public float GetRed(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetR..]);
        public float GetGreen(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetG..]);
        public uint GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetR..]);
        public uint GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetG..]);
        public void SetRed(Span<byte> pixel, float value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetR..], uint.CreateTruncating(value));
        public void SetGreen(Span<byte> pixel, float value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetG..], uint.CreateTruncating(value));
        public void SetRed(Span<byte> pixel, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetG..], value);
    }

    public sealed class R32G32SIntPixelFormat
        : IRawRgPixelFormat<int> {
        public const int OffsetR = 0;
        public const int OffsetG = 4;

        public DxgiFormat DxgiFormat => DxgiFormat.R32G32SInt;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 64;
        public int BytesPerPixel => 8;
        public float GetRed(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetR..]);
        public float GetGreen(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetG..]);
        public int GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetR..]);
        public int GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetG..]);
        public void SetRed(Span<byte> pixel, float value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetR..], int.CreateTruncating(value));
        public void SetGreen(Span<byte> pixel, float value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetG..], int.CreateTruncating(value));
        public void SetRed(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetG..], value);
    }

    public sealed class R32G8X24TypelessPixelFormat
        : IRawRgPixelFormat<uint>, IRawRgPixelFormat<int>, IRawRgPixelFormat<float> {
        public const int OffsetR = 0;
        public const int OffsetG = 4;

        public DxgiFormat DxgiFormat => DxgiFormat.R32G8X24Typeless;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 64;
        public int BytesPerPixel => 8;
        public float GetRed(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetR..]);
        public float GetGreen(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
        float IRawRPixelFormat<float>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetR..]);
        float IRawGPixelFormat<float>.GetGreenTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
        uint IRawRPixelFormat<uint>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt32LittleEndian(pixel[OffsetR..]);
        uint IRawGPixelFormat<uint>.GetGreenTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
        int IRawRPixelFormat<int>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt32LittleEndian(pixel[OffsetR..]);
        int IRawGPixelFormat<int>.GetGreenTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
        public void SetRed(Span<byte> pixel, float value) => BinaryPrimitives.WriteSingleLittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, float value) => pixel[OffsetG] = byte.CreateTruncating(value);
        public void SetRed(Span<byte> pixel, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, uint value) => pixel[OffsetG] = byte.CreateTruncating(value);
        public void SetRed(Span<byte> pixel, int value) => BinaryPrimitives.WriteInt32LittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, int value) => pixel[OffsetG] = byte.CreateTruncating(value);
    }

    public sealed class D32FloatS8X24UIntPixelFormat
        : IRawDPixelFormat<float>, IRawSPixelFormat<byte> {
        public const int OffsetD = 0;
        public const int OffsetS = 4;

        public DxgiFormat DxgiFormat => DxgiFormat.D32FloatS8X24UInt;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 64;
        public int BytesPerPixel => 8;
        public float GetDepth(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetD..]);
        public float GetStencil(ReadOnlySpan<byte> pixel) => pixel[OffsetS];
        public float GetDepthTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetD..]);
        public byte GetStencilTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetS];
        public void SetDepth(Span<byte> pixel, float value) => BinaryPrimitives.WriteSingleLittleEndian(pixel[OffsetD..], value);
        public void SetStencil(Span<byte> pixel, float value) => pixel[OffsetS] = byte.CreateTruncating(value);
        public void SetStencil(Span<byte> pixel, byte value) => pixel[OffsetS] = value;
    }

    public sealed class R32FloatX8X24TypelessPixelFormat
        : IRawRPixelFormat<float> {
        public const int OffsetR = 0;

        public DxgiFormat DxgiFormat => DxgiFormat.R32FloatX8X24Typeless;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 64;
        public int BytesPerPixel => 8;
        public float GetRed(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetR..]);
        public float GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadSingleLittleEndian(pixel[OffsetR..]);
        public void SetRed(Span<byte> pixel, float value) => BinaryPrimitives.WriteSingleLittleEndian(pixel[OffsetR..], value);
    }

    public sealed class X32TypelessG8X24UIntPixelFormat
        : IRawGPixelFormat<float> {
        public const int OffsetG = 4;

        public DxgiFormat DxgiFormat => DxgiFormat.X32TypelessG8X24UInt;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 64;
        public int BytesPerPixel => 8;
        public float GetGreen(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
        float IRawGPixelFormat<float>.GetGreenTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
        public void SetGreen(Span<byte> pixel, float value) => pixel[OffsetG] = byte.CreateTruncating(value);
    }

    public sealed class R10G10B10A2TypelessPixelFormat
        : IRawRgbaPixelFormat
            , IRawRgbPixelFormat<short>, IRawRgbPixelFormat<ushort>
            , IRawAlphaPixelFormat<byte>, IRawAlphaPixelFormat<sbyte> {
        public DxgiFormat DxgiFormat => DxgiFormat.R10G10B10A2Typeless;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 32;
        public int BytesPerPixel => 4;
        public AlphaType AlphaType => AlphaType.Straight;
        public float GetRed(ReadOnlySpan<byte> pixel) => GetRedRaw(pixel);
        public float GetGreen(ReadOnlySpan<byte> pixel) => GetGreenRaw(pixel);
        public float GetBlue(ReadOnlySpan<byte> pixel) => GetBlueRaw(pixel);
        public float GetAlpha(ReadOnlySpan<byte> pixel) => GetAlphaRaw(pixel);
        ushort IRawRPixelFormat<ushort>.GetRedTyped(ReadOnlySpan<byte> pixel) => GetRedRaw(pixel);
        ushort IRawGPixelFormat<ushort>.GetGreenTyped(ReadOnlySpan<byte> pixel) => GetGreenRaw(pixel);
        ushort IRawRgbPixelFormat<ushort>.GetBlueTyped(ReadOnlySpan<byte> pixel) => GetBlueRaw(pixel);
        byte IRawAlphaPixelFormat<byte>.GetAlphaTyped(ReadOnlySpan<byte> pixel) => GetAlphaRaw(pixel);
        short IRawRPixelFormat<short>.GetRedTyped(ReadOnlySpan<byte> pixel) => (short) GetRedRaw(pixel);
        short IRawGPixelFormat<short>.GetGreenTyped(ReadOnlySpan<byte> pixel) => (short) GetGreenRaw(pixel);
        short IRawRgbPixelFormat<short>.GetBlueTyped(ReadOnlySpan<byte> pixel) => (short) GetBlueRaw(pixel);
        sbyte IRawAlphaPixelFormat<sbyte>.GetAlphaTyped(ReadOnlySpan<byte> pixel) => (sbyte) GetAlphaRaw(pixel);
        public void SetRed(Span<byte> pixel, float value) => SetRedRaw(pixel, (ushort) Math.Clamp(value, 0, 0x3FF));
        public void SetGreen(Span<byte> pixel, float value) => SetGreenRaw(pixel, (ushort) Math.Clamp(value, 0, 0x3FF));
        public void SetBlue(Span<byte> pixel, float value) => SetBlueRaw(pixel, (ushort) Math.Clamp(value, 0, 0x3FF));
        public void SetAlpha(Span<byte> pixel, float value) => SetAlphaRaw(pixel, (byte) Math.Clamp(value, 0, 3));
        public void SetRed(Span<byte> pixel, ushort value) => SetRedRaw(pixel, ushort.Clamp(value, 0, 0x3FF));
        public void SetGreen(Span<byte> pixel, ushort value) => SetGreenRaw(pixel, ushort.Clamp(value, 0, 0x3FF));
        public void SetBlue(Span<byte> pixel, ushort value) => SetBlueRaw(pixel, ushort.Clamp(value, 0, 0x3FF));
        public void SetAlpha(Span<byte> pixel, byte value) => SetAlphaRaw(pixel, byte.Clamp(value, 0, 3));
        public void SetRed(Span<byte> pixel, short value) => SetRedRaw(pixel, (ushort) short.Clamp(value, 0, 0x3FF));
        public void SetGreen(Span<byte> pixel, short value) => SetGreenRaw(pixel, (ushort) short.Clamp(value, 0, 0x3FF));
        public void SetBlue(Span<byte> pixel, short value) => SetBlueRaw(pixel, (ushort) short.Clamp(value, 0, 0x3FF));
        public void SetAlpha(Span<byte> pixel, sbyte value) => SetAlphaRaw(pixel, (byte) sbyte.Clamp(value, 0, 3));

        public Vector4 GetRgba(ReadOnlySpan<byte> pixel) {
            var v = BinaryPrimitives.ReadUInt32LittleEndian(pixel);
            return new(
                ((v >> 0) & 0x3FF),
                ((v >> 10) & 0x3FF),
                ((v >> 20) & 0x3FF),
                ((v >> 30) & 0x3FF));
        }

        public void SetRgba(Span<byte> pixel, Vector4 rgba) => BinaryPrimitives.WriteUInt32LittleEndian(
            pixel,
            ((uint) Math.Clamp(rgba.X, 0, 0x3FF) << 0) |
            ((uint) Math.Clamp(rgba.Y, 0, 0x3FF) << 10) |
            ((uint) Math.Clamp(rgba.Z, 0, 0x3FF) << 20) |
            ((uint) Math.Clamp(rgba.W, 0, 3) << 30));

        private static ushort GetRedRaw(ReadOnlySpan<byte> pixel) => (ushort) ((BinaryPrimitives.ReadUInt32LittleEndian(pixel) >> 0) & 0x3FF);
        private static ushort GetGreenRaw(ReadOnlySpan<byte> pixel) => (ushort) ((BinaryPrimitives.ReadUInt32LittleEndian(pixel) >> 10) & 0x3FF);
        private static ushort GetBlueRaw(ReadOnlySpan<byte> pixel) => (ushort) ((BinaryPrimitives.ReadUInt32LittleEndian(pixel) >> 20) & 0x3FF);
        private static byte GetAlphaRaw(ReadOnlySpan<byte> pixel) => (byte) ((BinaryPrimitives.ReadUInt32LittleEndian(pixel) >> 30) & 0x3FF);

        private static void SetRedRaw(Span<byte> pixel, ushort value) =>
            BinaryPrimitives.WriteUInt32LittleEndian(pixel, (BinaryPrimitives.ReadUInt32LittleEndian(pixel) & ~0x3FFu) | ((uint) value << 0));

        private static void SetGreenRaw(Span<byte> pixel, ushort value) =>
            BinaryPrimitives.WriteUInt32LittleEndian(pixel, (BinaryPrimitives.ReadUInt32LittleEndian(pixel) & ~0xFFC00u) | ((uint) value << 10));

        private static void SetBlueRaw(Span<byte> pixel, ushort value) =>
            BinaryPrimitives.WriteUInt32LittleEndian(pixel, (BinaryPrimitives.ReadUInt32LittleEndian(pixel) & ~0x3FF00000u) | ((uint) value << 20));

        private static void SetAlphaRaw(Span<byte> pixel, byte value) =>
            BinaryPrimitives.WriteUInt32LittleEndian(pixel, (BinaryPrimitives.ReadUInt32LittleEndian(pixel) & ~0xC0000000u) | ((uint) value << 30));
    }

    public sealed class R10G10B10A2UNormPixelFormat
        : IRawRgbaPixelFormat
            , IRawRgbPixelFormat<ushort>, IRawAlphaPixelFormat<byte> {
        public DxgiFormat DxgiFormat => DxgiFormat.R10G10B10A2UNorm;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromRgba(32, 0x3FF, 0xFFC00, 0x3FF00000, 0xC0000000);
        public int BitsPerPixel => 32;
        public int BytesPerPixel => 4;
        public AlphaType AlphaType => AlphaType.Straight;
        public float GetRed(ReadOnlySpan<byte> pixel) => GetRedRaw(pixel) / 1023f;
        public float GetGreen(ReadOnlySpan<byte> pixel) => GetGreenRaw(pixel) / 1023f;
        public float GetBlue(ReadOnlySpan<byte> pixel) => GetBlueRaw(pixel) / 1023f;
        public float GetAlpha(ReadOnlySpan<byte> pixel) => GetAlphaRaw(pixel) / 3f;
        public ushort GetRedTyped(ReadOnlySpan<byte> pixel) => GetRedRaw(pixel);
        public ushort GetGreenTyped(ReadOnlySpan<byte> pixel) => GetGreenRaw(pixel);
        public ushort GetBlueTyped(ReadOnlySpan<byte> pixel) => GetBlueRaw(pixel);
        public byte GetAlphaTyped(ReadOnlySpan<byte> pixel) => GetAlphaRaw(pixel);
        public void SetRed(Span<byte> pixel, float value) => SetRedRaw(pixel, (ushort) Math.Clamp(value * 0x400, 0, 0x3FF));
        public void SetGreen(Span<byte> pixel, float value) => SetGreenRaw(pixel, (ushort) Math.Clamp(value * 0x400, 0, 0x3FF));
        public void SetBlue(Span<byte> pixel, float value) => SetBlueRaw(pixel, (ushort) Math.Clamp(value * 0x400, 0, 0x3FF));
        public void SetAlpha(Span<byte> pixel, float value) => SetAlphaRaw(pixel, (byte) Math.Clamp(value * 0x4, 0, 3));
        public void SetRed(Span<byte> pixel, ushort value) => SetRedRaw(pixel, ushort.Clamp(value, 0, 0x3FF));
        public void SetGreen(Span<byte> pixel, ushort value) => SetGreenRaw(pixel, ushort.Clamp(value, 0, 0x3FF));
        public void SetBlue(Span<byte> pixel, ushort value) => SetBlueRaw(pixel, ushort.Clamp(value, 0, 0x3FF));
        public void SetAlpha(Span<byte> pixel, byte value) => SetAlphaRaw(pixel, byte.Clamp(value, 0, 3));

        public Vector4 GetRgba(ReadOnlySpan<byte> pixel) {
            var v = BinaryPrimitives.ReadUInt32LittleEndian(pixel);
            return new(
                ((v >> 0) & 0x3FF) / 1023f,
                ((v >> 10) & 0x3FF) / 1023f,
                ((v >> 20) & 0x3FF) / 1023f,
                ((v >> 30) & 0x3FF) / 3f);
        }

        public void SetRgba(Span<byte> pixel, Vector4 rgba) => BinaryPrimitives.WriteUInt32LittleEndian(
            pixel,
            ((uint) Math.Clamp(rgba.X * 1024f, 0, 0x3FF) << 0) |
            ((uint) Math.Clamp(rgba.Y * 1024f, 0, 0x3FF) << 10) |
            ((uint) Math.Clamp(rgba.Z * 1024f, 0, 0x3FF) << 20) |
            ((uint) Math.Clamp(rgba.W * 4f, 0, 3) << 30));

        private static ushort GetRedRaw(ReadOnlySpan<byte> pixel) => (ushort) ((BinaryPrimitives.ReadUInt32LittleEndian(pixel) >> 0) & 0x3FF);
        private static ushort GetGreenRaw(ReadOnlySpan<byte> pixel) => (ushort) ((BinaryPrimitives.ReadUInt32LittleEndian(pixel) >> 10) & 0x3FF);
        private static ushort GetBlueRaw(ReadOnlySpan<byte> pixel) => (ushort) ((BinaryPrimitives.ReadUInt32LittleEndian(pixel) >> 20) & 0x3FF);
        private static byte GetAlphaRaw(ReadOnlySpan<byte> pixel) => (byte) ((BinaryPrimitives.ReadUInt32LittleEndian(pixel) >> 30) & 0x3FF);

        private static void SetRedRaw(Span<byte> pixel, ushort value) =>
            BinaryPrimitives.WriteUInt32LittleEndian(pixel, (BinaryPrimitives.ReadUInt32LittleEndian(pixel) & ~0x3FFu) | ((uint) value << 0));

        private static void SetGreenRaw(Span<byte> pixel, ushort value) =>
            BinaryPrimitives.WriteUInt32LittleEndian(pixel, (BinaryPrimitives.ReadUInt32LittleEndian(pixel) & ~0xFFC00u) | ((uint) value << 10));

        private static void SetBlueRaw(Span<byte> pixel, ushort value) =>
            BinaryPrimitives.WriteUInt32LittleEndian(pixel, (BinaryPrimitives.ReadUInt32LittleEndian(pixel) & ~0x3FF00000u) | ((uint) value << 20));

        private static void SetAlphaRaw(Span<byte> pixel, byte value) =>
            BinaryPrimitives.WriteUInt32LittleEndian(pixel, (BinaryPrimitives.ReadUInt32LittleEndian(pixel) & ~0xC0000000u) | ((uint) value << 30));
    }

    public sealed class R10G10B10A2UIntPixelFormat
        : IRawRgbaPixelFormat
            , IRawRgbPixelFormat<ushort>, IRawAlphaPixelFormat<byte> {
        public DxgiFormat DxgiFormat => DxgiFormat.R10G10B10A2UInt;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 32;
        public int BytesPerPixel => 4;
        public AlphaType AlphaType => AlphaType.Straight;
        public float GetRed(ReadOnlySpan<byte> pixel) => GetRedRaw(pixel);
        public float GetGreen(ReadOnlySpan<byte> pixel) => GetGreenRaw(pixel);
        public float GetBlue(ReadOnlySpan<byte> pixel) => GetBlueRaw(pixel);
        public float GetAlpha(ReadOnlySpan<byte> pixel) => GetAlphaRaw(pixel);
        public ushort GetRedTyped(ReadOnlySpan<byte> pixel) => GetRedRaw(pixel);
        public ushort GetGreenTyped(ReadOnlySpan<byte> pixel) => GetGreenRaw(pixel);
        public ushort GetBlueTyped(ReadOnlySpan<byte> pixel) => GetBlueRaw(pixel);
        public byte GetAlphaTyped(ReadOnlySpan<byte> pixel) => GetAlphaRaw(pixel);
        public void SetRed(Span<byte> pixel, float value) => SetRedRaw(pixel, (ushort) Math.Clamp(value, 0, 0x3FF));
        public void SetGreen(Span<byte> pixel, float value) => SetGreenRaw(pixel, (ushort) Math.Clamp(value, 0, 0x3FF));
        public void SetBlue(Span<byte> pixel, float value) => SetBlueRaw(pixel, (ushort) Math.Clamp(value, 0, 0x3FF));
        public void SetAlpha(Span<byte> pixel, float value) => SetAlphaRaw(pixel, (byte) Math.Clamp(value, 0, 3));
        public void SetRed(Span<byte> pixel, ushort value) => SetRedRaw(pixel, ushort.Clamp(value, 0, 0x3FF));
        public void SetGreen(Span<byte> pixel, ushort value) => SetGreenRaw(pixel, ushort.Clamp(value, 0, 0x3FF));
        public void SetBlue(Span<byte> pixel, ushort value) => SetBlueRaw(pixel, ushort.Clamp(value, 0, 0x3FF));
        public void SetAlpha(Span<byte> pixel, byte value) => SetAlphaRaw(pixel, byte.Clamp(value, 0, 3));

        public Vector4 GetRgba(ReadOnlySpan<byte> pixel) {
            var v = BinaryPrimitives.ReadUInt32LittleEndian(pixel);
            return new(
                ((v >> 0) & 0x3FF),
                ((v >> 10) & 0x3FF),
                ((v >> 20) & 0x3FF),
                ((v >> 30) & 0x3FF));
        }

        public void SetRgba(Span<byte> pixel, Vector4 rgba) => BinaryPrimitives.WriteUInt32LittleEndian(
            pixel,
            ((uint) Math.Clamp(rgba.X, 0, 0x3FF) << 0) |
            ((uint) Math.Clamp(rgba.Y, 0, 0x3FF) << 10) |
            ((uint) Math.Clamp(rgba.Z, 0, 0x3FF) << 20) |
            ((uint) Math.Clamp(rgba.W, 0, 3) << 30));

        private static ushort GetRedRaw(ReadOnlySpan<byte> pixel) => (ushort) ((BinaryPrimitives.ReadUInt32LittleEndian(pixel) >> 0) & 0x3FF);
        private static ushort GetGreenRaw(ReadOnlySpan<byte> pixel) => (ushort) ((BinaryPrimitives.ReadUInt32LittleEndian(pixel) >> 10) & 0x3FF);
        private static ushort GetBlueRaw(ReadOnlySpan<byte> pixel) => (ushort) ((BinaryPrimitives.ReadUInt32LittleEndian(pixel) >> 20) & 0x3FF);
        private static byte GetAlphaRaw(ReadOnlySpan<byte> pixel) => (byte) ((BinaryPrimitives.ReadUInt32LittleEndian(pixel) >> 30) & 0x3FF);

        private static void SetRedRaw(Span<byte> pixel, ushort value) =>
            BinaryPrimitives.WriteUInt32LittleEndian(pixel, (BinaryPrimitives.ReadUInt32LittleEndian(pixel) & ~0x3FFu) | ((uint) value << 0));

        private static void SetGreenRaw(Span<byte> pixel, ushort value) =>
            BinaryPrimitives.WriteUInt32LittleEndian(pixel, (BinaryPrimitives.ReadUInt32LittleEndian(pixel) & ~0xFFC00u) | ((uint) value << 10));

        private static void SetBlueRaw(Span<byte> pixel, ushort value) =>
            BinaryPrimitives.WriteUInt32LittleEndian(pixel, (BinaryPrimitives.ReadUInt32LittleEndian(pixel) & ~0x3FF00000u) | ((uint) value << 20));

        private static void SetAlphaRaw(Span<byte> pixel, byte value) =>
            BinaryPrimitives.WriteUInt32LittleEndian(pixel, (BinaryPrimitives.ReadUInt32LittleEndian(pixel) & ~0xC0000000u) | ((uint) value << 30));
    }

    public sealed class R8G8B8A8TypelessPixelFormat
        : IRawRgbPixelFormat<byte>, IRawRgbPixelFormat<sbyte>
            , IRawAlphaPixelFormat<byte>, IRawAlphaPixelFormat<sbyte> {
        public const int OffsetR = 0;
        public const int OffsetG = 1;
        public const int OffsetB = 2;
        public const int OffsetA = 3;

        public DxgiFormat DxgiFormat => DxgiFormat.R8G8B8A8Typeless;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 32;
        public int BytesPerPixel => 4;
        public AlphaType AlphaType => AlphaType.Straight;
        public float GetRed(ReadOnlySpan<byte> pixel) => pixel[OffsetR];
        public float GetGreen(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
        public float GetBlue(ReadOnlySpan<byte> pixel) => pixel[OffsetB];
        public float GetAlpha(ReadOnlySpan<byte> pixel) => pixel[OffsetA];
        byte IRawRPixelFormat<byte>.GetRedTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetR];
        byte IRawGPixelFormat<byte>.GetGreenTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
        byte IRawRgbPixelFormat<byte>.GetBlueTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetB];
        byte IRawAlphaPixelFormat<byte>.GetAlphaTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetA];
        sbyte IRawRPixelFormat<sbyte>.GetRedTyped(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetR];
        sbyte IRawGPixelFormat<sbyte>.GetGreenTyped(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetG];
        sbyte IRawRgbPixelFormat<sbyte>.GetBlueTyped(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetB];
        sbyte IRawAlphaPixelFormat<sbyte>.GetAlphaTyped(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetA];
        public void SetRed(Span<byte> pixel, float value) => pixel[OffsetR] = byte.CreateTruncating(value);
        public void SetGreen(Span<byte> pixel, float value) => pixel[OffsetG] = byte.CreateTruncating(value);
        public void SetBlue(Span<byte> pixel, float value) => pixel[OffsetB] = byte.CreateTruncating(value);
        public void SetAlpha(Span<byte> pixel, float value) => pixel[OffsetA] = byte.CreateTruncating(value);
        public void SetRed(Span<byte> pixel, byte value) => pixel[OffsetR] = value;
        public void SetGreen(Span<byte> pixel, byte value) => pixel[OffsetG] = value;
        public void SetBlue(Span<byte> pixel, byte value) => pixel[OffsetB] = value;
        public void SetAlpha(Span<byte> pixel, byte value) => pixel[OffsetA] = value;
        public void SetRed(Span<byte> pixel, sbyte value) => pixel[OffsetR] = byte.CreateTruncating(value);
        public void SetGreen(Span<byte> pixel, sbyte value) => pixel[OffsetG] = byte.CreateTruncating(value);
        public void SetBlue(Span<byte> pixel, sbyte value) => pixel[OffsetB] = byte.CreateTruncating(value);
        public void SetAlpha(Span<byte> pixel, sbyte value) => pixel[OffsetA] = byte.CreateTruncating(value);
    }

    public sealed class R8G8B8A8UNormPixelFormat
        : IRawRgbPixelFormat<byte>, IRawAlphaPixelFormat<byte> {
        public const int OffsetR = 0;
        public const int OffsetG = 1;
        public const int OffsetB = 2;
        public const int OffsetA = 3;

        public DxgiFormat DxgiFormat => DxgiFormat.R8G8B8A8UNorm;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromRgba(32, 0xFF, 0xFF00, 0xFF0000, 0xFF000000);
        public int BitsPerPixel => 32;
        public int BytesPerPixel => 4;
        public AlphaType AlphaType => AlphaType.Straight;
        public float GetRed(ReadOnlySpan<byte> pixel) => pixel[OffsetR] / 255f;
        public float GetGreen(ReadOnlySpan<byte> pixel) => pixel[OffsetG] / 255f;
        public float GetBlue(ReadOnlySpan<byte> pixel) => pixel[OffsetB] / 255f;
        public float GetAlpha(ReadOnlySpan<byte> pixel) => pixel[OffsetA] / 255f;
        public byte GetRedTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetR];
        public byte GetGreenTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
        public byte GetBlueTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetB];
        public byte GetAlphaTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetA];
        public void SetRed(Span<byte> pixel, float value) => pixel[OffsetR] = byte.CreateTruncating(value * 256f);
        public void SetGreen(Span<byte> pixel, float value) => pixel[OffsetG] = byte.CreateTruncating(value * 256f);
        public void SetBlue(Span<byte> pixel, float value) => pixel[OffsetB] = byte.CreateTruncating(value * 256f);
        public void SetAlpha(Span<byte> pixel, float value) => pixel[OffsetA] = byte.CreateTruncating(value * 256f);
        public void SetRed(Span<byte> pixel, byte value) => pixel[OffsetR] = value;
        public void SetGreen(Span<byte> pixel, byte value) => pixel[OffsetG] = value;
        public void SetBlue(Span<byte> pixel, byte value) => pixel[OffsetB] = value;
        public void SetAlpha(Span<byte> pixel, byte value) => pixel[OffsetA] = value;
    }

    public sealed class R8G8B8A8UNormSrgbPixelFormat
        : IRawRgbPixelFormat<byte>, IRawAlphaPixelFormat<byte> {
        public const int OffsetR = 0;
        public const int OffsetG = 1;
        public const int OffsetB = 2;
        public const int OffsetA = 3;

        public DxgiFormat DxgiFormat => DxgiFormat.R8G8B8A8UNormSrgb;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 32;
        public int BytesPerPixel => 4;
        public AlphaType AlphaType => AlphaType.Straight;
        public float GetRed(ReadOnlySpan<byte> pixel) => pixel[OffsetR] / 255f;
        public float GetGreen(ReadOnlySpan<byte> pixel) => pixel[OffsetG] / 255f;
        public float GetBlue(ReadOnlySpan<byte> pixel) => pixel[OffsetB] / 255f;
        public float GetAlpha(ReadOnlySpan<byte> pixel) => pixel[OffsetA] / 255f;
        public byte GetRedTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetR];
        public byte GetGreenTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
        public byte GetBlueTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetB];
        public byte GetAlphaTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetA];
        public void SetRed(Span<byte> pixel, float value) => pixel[OffsetR] = byte.CreateTruncating(value * 256f);
        public void SetGreen(Span<byte> pixel, float value) => pixel[OffsetG] = byte.CreateTruncating(value * 256f);
        public void SetBlue(Span<byte> pixel, float value) => pixel[OffsetB] = byte.CreateTruncating(value * 256f);
        public void SetAlpha(Span<byte> pixel, float value) => pixel[OffsetA] = byte.CreateTruncating(value * 256f);
        public void SetRed(Span<byte> pixel, byte value) => pixel[OffsetR] = value;
        public void SetGreen(Span<byte> pixel, byte value) => pixel[OffsetG] = value;
        public void SetBlue(Span<byte> pixel, byte value) => pixel[OffsetB] = value;
        public void SetAlpha(Span<byte> pixel, byte value) => pixel[OffsetA] = value;
    }

    public sealed class R8G8B8A8UIntPixelFormat
        : IRawRgbPixelFormat<byte>, IRawAlphaPixelFormat<byte> {
        public const int OffsetR = 0;
        public const int OffsetG = 1;
        public const int OffsetB = 2;
        public const int OffsetA = 3;

        public DxgiFormat DxgiFormat => DxgiFormat.R8G8B8A8UInt;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 32;
        public int BytesPerPixel => 4;
        public AlphaType AlphaType => AlphaType.Straight;
        public float GetRed(ReadOnlySpan<byte> pixel) => pixel[OffsetR];
        public float GetGreen(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
        public float GetBlue(ReadOnlySpan<byte> pixel) => pixel[OffsetB];
        public float GetAlpha(ReadOnlySpan<byte> pixel) => pixel[OffsetA];
        public byte GetRedTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetR];
        public byte GetGreenTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetG];
        public byte GetBlueTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetB];
        public byte GetAlphaTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetA];
        public void SetRed(Span<byte> pixel, float value) => pixel[OffsetR] = byte.CreateTruncating(value);
        public void SetGreen(Span<byte> pixel, float value) => pixel[OffsetG] = byte.CreateTruncating(value);
        public void SetBlue(Span<byte> pixel, float value) => pixel[OffsetB] = byte.CreateTruncating(value);
        public void SetAlpha(Span<byte> pixel, float value) => pixel[OffsetA] = byte.CreateTruncating(value);
        public void SetRed(Span<byte> pixel, byte value) => pixel[OffsetR] = value;
        public void SetGreen(Span<byte> pixel, byte value) => pixel[OffsetG] = value;
        public void SetBlue(Span<byte> pixel, byte value) => pixel[OffsetB] = value;
        public void SetAlpha(Span<byte> pixel, byte value) => pixel[OffsetA] = value;
    }

    public sealed class R8G8B8A8SNormPixelFormat
        : IRawRgbPixelFormat<sbyte>, IRawAlphaPixelFormat<sbyte> {
        public const int OffsetR = 0;
        public const int OffsetG = 1;
        public const int OffsetB = 2;
        public const int OffsetA = 3;

        public DxgiFormat DxgiFormat => DxgiFormat.R8G8B8A8SNorm;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 32;
        public int BytesPerPixel => 4;
        public AlphaType AlphaType => AlphaType.Straight;
        public float GetRed(ReadOnlySpan<byte> pixel) => Math.Clamp((sbyte) pixel[OffsetR] / 127f, -1f, 1f);
        public float GetGreen(ReadOnlySpan<byte> pixel) => Math.Clamp((sbyte) pixel[OffsetG] / 127f, -1f, 1f);
        public float GetBlue(ReadOnlySpan<byte> pixel) => Math.Clamp((sbyte) pixel[OffsetB] / 127f, -1f, 1f);
        public float GetAlpha(ReadOnlySpan<byte> pixel) => Math.Clamp((sbyte) pixel[OffsetA] / 127f, -1f, 1f);
        public sbyte GetRedTyped(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetR];
        public sbyte GetGreenTyped(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetG];
        public sbyte GetBlueTyped(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetB];
        public sbyte GetAlphaTyped(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetA];
        public void SetRed(Span<byte> pixel, float value) => pixel[OffsetR] = (byte) sbyte.CreateTruncating(Math.Clamp(value, -1f, 1f) * 127f);
        public void SetGreen(Span<byte> pixel, float value) => pixel[OffsetG] = (byte) sbyte.CreateTruncating(Math.Clamp(value, -1f, 1f) * 127f);
        public void SetBlue(Span<byte> pixel, float value) => pixel[OffsetB] = (byte) sbyte.CreateTruncating(Math.Clamp(value, -1f, 1f) * 127f);
        public void SetAlpha(Span<byte> pixel, float value) => pixel[OffsetA] = (byte) sbyte.CreateTruncating(Math.Clamp(value, -1f, 1f) * 127f);
        public void SetRed(Span<byte> pixel, sbyte value) => pixel[OffsetR] = (byte) value;
        public void SetGreen(Span<byte> pixel, sbyte value) => pixel[OffsetG] = (byte) value;
        public void SetBlue(Span<byte> pixel, sbyte value) => pixel[OffsetB] = (byte) value;
        public void SetAlpha(Span<byte> pixel, sbyte value) => pixel[OffsetA] = (byte) value;
    }

    public sealed class R8G8B8A8SIntPixelFormat
        : IRawRgbPixelFormat<sbyte>, IRawAlphaPixelFormat<sbyte> {
        public const int OffsetR = 0;
        public const int OffsetG = 1;
        public const int OffsetB = 2;
        public const int OffsetA = 3;

        public DxgiFormat DxgiFormat => DxgiFormat.R8G8B8A8SInt;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 32;
        public int BytesPerPixel => 4;
        public AlphaType AlphaType => AlphaType.Straight;
        public float GetRed(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetR];
        public float GetGreen(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetG];
        public float GetBlue(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetB];
        public float GetAlpha(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetA];
        public sbyte GetRedTyped(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetR];
        public sbyte GetGreenTyped(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetG];
        public sbyte GetBlueTyped(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetB];
        public sbyte GetAlphaTyped(ReadOnlySpan<byte> pixel) => (sbyte) pixel[OffsetA];
        public void SetRed(Span<byte> pixel, float value) => pixel[OffsetR] = (byte) sbyte.CreateTruncating(value);
        public void SetGreen(Span<byte> pixel, float value) => pixel[OffsetG] = (byte) sbyte.CreateTruncating(value);
        public void SetBlue(Span<byte> pixel, float value) => pixel[OffsetB] = (byte) sbyte.CreateTruncating(value);
        public void SetAlpha(Span<byte> pixel, float value) => pixel[OffsetA] = (byte) sbyte.CreateTruncating(value);
        public void SetRed(Span<byte> pixel, sbyte value) => pixel[OffsetR] = (byte) value;
        public void SetGreen(Span<byte> pixel, sbyte value) => pixel[OffsetG] = (byte) value;
        public void SetBlue(Span<byte> pixel, sbyte value) => pixel[OffsetB] = (byte) value;
        public void SetAlpha(Span<byte> pixel, sbyte value) => pixel[OffsetA] = (byte) value;
    }

    public sealed class R16G16TypelessPixelFormat
        : IRawRgPixelFormat<ushort>, IRawRgPixelFormat<short>, IRawRgPixelFormat<Half> {
        public const int OffsetR = 0;
        public const int OffsetG = 2;

        public DxgiFormat DxgiFormat => DxgiFormat.R16G16Typeless;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 32;
        public int BytesPerPixel => 4;
        public float GetRed(ReadOnlySpan<byte> pixel) => (float) BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetR..]);
        public float GetGreen(ReadOnlySpan<byte> pixel) => (float) BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetG..]);
        Half IRawRPixelFormat<Half>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetR..]);
        Half IRawGPixelFormat<Half>.GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetG..]);
        ushort IRawRPixelFormat<ushort>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetR..]);
        ushort IRawGPixelFormat<ushort>.GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetG..]);
        short IRawRPixelFormat<short>.GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetR..]);
        short IRawGPixelFormat<short>.GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetG..]);
        public void SetRed(Span<byte> pixel, float value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetR..], Half.CreateTruncating(value));
        public void SetGreen(Span<byte> pixel, float value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetG..], Half.CreateTruncating(value));
        public void SetRed(Span<byte> pixel, Half value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, Half value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetG..], value);
        public void SetRed(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetG..], value);
        public void SetRed(Span<byte> pixel, short value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, short value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetG..], value);
    }

    public sealed class R16G16FloatPixelFormat
        : IRawRgPixelFormat<Half> {
        public const int OffsetR = 0;
        public const int OffsetG = 2;

        public DxgiFormat DxgiFormat => DxgiFormat.R16G16Float;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 32;
        public int BytesPerPixel => 4;
        public float GetRed(ReadOnlySpan<byte> pixel) => (float) BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetR..]);
        public float GetGreen(ReadOnlySpan<byte> pixel) => (float) BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetG..]);
        public Half GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetR..]);
        public Half GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadHalfLittleEndian(pixel[OffsetG..]);
        public void SetRed(Span<byte> pixel, float value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetR..], Half.CreateTruncating(value));
        public void SetGreen(Span<byte> pixel, float value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetG..], Half.CreateTruncating(value));
        public void SetRed(Span<byte> pixel, Half value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, Half value) => BinaryPrimitives.WriteHalfLittleEndian(pixel[OffsetG..], value);
    }

    public sealed class R16G16UNormPixelFormat
        : IRawRgPixelFormat<ushort> {
        public const int OffsetR = 0;
        public const int OffsetG = 2;

        public DxgiFormat DxgiFormat => DxgiFormat.R16G16UNorm;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromRgba(32, 0xFFFF, 0xFFFF0000, 0);
        public int BitsPerPixel => 32;
        public int BytesPerPixel => 4;
        public float GetRed(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetR..]) / 65535f;
        public float GetGreen(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetG..]) / 65535f;
        public ushort GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetR..]);
        public ushort GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetG..]);

        public void SetRed(Span<byte> pixel, float value) =>
            BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetR..], ushort.CreateTruncating(value * 65536f));

        public void SetGreen(Span<byte> pixel, float value) =>
            BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetG..], ushort.CreateTruncating(value * 65536f));

        public void SetRed(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetG..], value);
    }

    public sealed class R16G16UIntPixelFormat
        : IRawRgPixelFormat<ushort> {
        public const int OffsetR = 0;
        public const int OffsetG = 2;

        public DxgiFormat DxgiFormat => DxgiFormat.R16G16UInt;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 32;
        public int BytesPerPixel => 4;
        public float GetRed(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetR..]);
        public float GetGreen(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetG..]);
        public ushort GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetR..]);
        public ushort GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadUInt16LittleEndian(pixel[OffsetG..]);
        public void SetRed(Span<byte> pixel, float value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetR..], ushort.CreateTruncating(value));
        public void SetGreen(Span<byte> pixel, float value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetG..], ushort.CreateTruncating(value));
        public void SetRed(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(pixel[OffsetG..], value);
    }

    public sealed class R16G16SNormPixelFormat
        : IRawRgPixelFormat<short> {
        public const int OffsetR = 0;
        public const int OffsetG = 2;

        public DxgiFormat DxgiFormat => DxgiFormat.R16G16SNorm;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 32;
        public int BytesPerPixel => 4;
        public float GetRed(ReadOnlySpan<byte> pixel) => Math.Clamp(BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetR..]) / 32767f, -1f, 1f);
        public float GetGreen(ReadOnlySpan<byte> pixel) => Math.Clamp(BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetG..]) / 32767f, -1f, 1f);
        public short GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetR..]);
        public short GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetG..]);

        public void SetRed(Span<byte> pixel, float value) =>
            BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetR..], short.CreateTruncating(Math.Clamp(value, -1f, 1f) * 32767f));

        public void SetGreen(Span<byte> pixel, float value) =>
            BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetG..], short.CreateTruncating(Math.Clamp(value, -1f, 1f) * 32767f));

        public void SetRed(Span<byte> pixel, short value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, short value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetG..], value);
    }

    public sealed class R16G16SIntPixelFormat
        : IRawRgPixelFormat<short> {
        public const int OffsetR = 0;
        public const int OffsetG = 2;

        public DxgiFormat DxgiFormat => DxgiFormat.R16G16SInt;
        public DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromFourCc(DdsFourCc.Dx10);
        public int BitsPerPixel => 32;
        public int BytesPerPixel => 4;
        public float GetRed(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetR..]);
        public float GetGreen(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetG..]);
        public short GetRedTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetR..]);
        public short GetGreenTyped(ReadOnlySpan<byte> pixel) => BinaryPrimitives.ReadInt16LittleEndian(pixel[OffsetG..]);
        public void SetRed(Span<byte> pixel, float value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetR..], short.CreateTruncating(value);
        public void SetGreen(Span<byte> pixel, float value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetG..], short.CreateTruncating(value);
        public void SetRed(Span<byte> pixel, short value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetR..], value);
        public void SetGreen(Span<byte> pixel, short value) => BinaryPrimitives.WriteInt16LittleEndian(pixel[OffsetG..], value);
    }
}
