using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using DdsManipLib.DirectDrawSurface.PixelFormats;
using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;
using DdsManipLib.DirectDrawSurface.PixelFormats.DepthPixelFormats;

namespace DdsManipLib.DirectDrawSurface;

/// <summary>
/// Resource data formats, including fully-typed and typeless formats.
///
/// See: https://learn.microsoft.com/en-us/windows/win32/api/dxgiformat/ne-dxgiformat-dxgi_format
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum DxgiFormat {
#pragma warning disable CS1591
    Unknown = 0,
    
    // 128 bits
    R32G32B32A32Typeless = 1,
    R32G32B32A32Float = 2,
    R32G32B32A32UInt = 3,
    R32G32B32A32SInt = 4,

    // 96 bits
    R32G32B32Typeless = 5,
    R32G32B32Float = 6,
    R32G32B32UInt = 7,
    R32G32B32SInt = 8,

    // 64 bits
    R16G16B16A16Typeless = 9,
    R16G16B16A16Float = 10, // 0x0000000A
    R16G16B16A16UNorm = 11, // 0x0000000B
    R16G16B16A16UInt = 12, // 0x0000000C
    R16G16B16A16SNorm = 13, // 0x0000000D
    R16G16B16A16SInt = 14, // 0x0000000E

    R32G32Typeless = 15, // 0x0000000F
    R32G32Float = 16, // 0x00000010
    R32G32UInt = 17, // 0x00000011
    R32G32SInt = 18, // 0x00000012

    R32G8X24Typeless = 19, // 0x00000013
    D32FloatS8X24UInt = 20, // 0x00000014
    R32FloatX8X24Typeless = 21, // 0x00000015
    X32TypelessG8X24UInt = 22, // 0x00000016

    // 32 bits
    R10G10B10A2Typeless = 23, // 0x00000017
    R10G10B10A2UNorm = 24, // 0x00000018
    R10G10B10A2UInt = 25, // 0x00000019

    // Todo
    R11G11B10Float = 26, // 0x0000001A

    R8G8B8A8Typeless = 27, // 0x0000001B
    R8G8B8A8UNorm = 28, // 0x0000001C
    R8G8B8A8UNormSrgb = 29, // 0x0000001D
    R8G8B8A8UInt = 30, // 0x0000001E
    R8G8B8A8SNorm = 31, // 0x0000001F
    R8G8B8A8SInt = 32, // 0x00000020

    R16G16Typeless = 33, // 0x00000021
    R16G16Float = 34, // 0x00000022
    R16G16UNorm = 35, // 0x00000023
    R16G16UInt = 36, // 0x00000024
    R16G16SNorm = 37, // 0x0000002
    R16G16SInt = 38, // 0x00000026

    [RgbaxxPixelFormat.Presets.R<TypelessChannel>(32)]
    R32Typeless = 39, // 0x00000027

    [DepthPixelFormat<F32Channel>(32)]
    D32Float = 40, // 0x00000028

    [RgbaxxPixelFormat.Presets.R<F32Channel>(32)]
    R32Float = 41, // 0x00000029

    [RgbaxxPixelFormat.Presets.R<UIntChannel<uint>>(32)]
    R32UInt = 42, // 0x0000002A

    [RgbaxxPixelFormat.Presets.R<SIntChannel<int>>(32)]
    R32SInt = 43, // 0x0000002B

    [RgbaxxPixelFormat.Presets.Rg<TypelessChannel>(24, 8)]
    R24G8Typeless = 44, // 0x0000002C

    [DepthStencilPixelFormat<UNormChannel<uint>, UIntChannel<byte>>(24, 8)]
    D24UNormS8UInt = 45, // 0x0000002D

    [RgbaxxPixelFormat.Presets.Rx<UNormChannel<uint>, TypelessChannel>(24, 8)]
    R24UNormX8Typeless = 46, // 0x0000002E

    [RgbaxxPixelFormat.Presets.Xg<TypelessChannel, UIntChannel<byte>>(24, 8)]
    X24TypelessG8UInt = 47, // 0x0000002F

    [RgbaxxPixelFormat.Presets.Rg<TypelessChannel>(8, 8)]
    R8G8Typeless = 48, // 0x00000030

    [RgbaxxPixelFormat.Presets.Rg<UNormChannel<byte>>(8, 8)]
    R8G8UNorm = 49, // 0x00000031

    [RgbaxxPixelFormat.Presets.Rg<UIntChannel<byte>>(8, 8)]
    R8G8UInt = 50, // 0x00000032

    [RgbaxxPixelFormat.Presets.Rg<SNormChannel<sbyte>>(8, 8)]
    R8G8SNorm = 51, // 0x00000033

    [RgbaxxPixelFormat.Presets.Rg<SIntChannel<sbyte>>(8, 8)]
    R8G8SInt = 52, // 0x00000034

    [RgbaxxPixelFormat.Presets.R<TypelessChannel>(16)]
    R16Typeless = 53, // 0x00000035

    [RgbaxxPixelFormat.Presets.R<F32Channel>(16)]
    R16Float = 54, // 0x00000036

    [DepthPixelFormat<UNormChannel<ushort>>(16)]
    D16UNorm = 55, // 0x00000037

    [RgbaxxPixelFormat.Presets.R<UNormChannel<ushort>>(16)]
    R16UNorm = 56, // 0x00000038

    [RgbaxxPixelFormat.Presets.R<UIntChannel<ushort>>(16)]
    R16UInt = 57, // 0x00000039

    [RgbaxxPixelFormat.Presets.R<SNormChannel<short>>(16)]
    R16SNorm = 58, // 0x0000003A

    [RgbaxxPixelFormat.Presets.R<SIntChannel<short>>(16)]
    R16SInt = 59, // 0x0000003B

    [RgbaxxPixelFormat.Presets.R<TypelessChannel>(8)]
    R8Typeless = 60, // 0x0000003C

    [RgbaxxPixelFormat.Presets.R<UNormChannel<byte>>(8)]
    R8UNorm = 61, // 0x0000003D

    [RgbaxxPixelFormat.Presets.R<UIntChannel<byte>>(8)]
    R8UInt = 62, // 0x0000003E

    [RgbaxxPixelFormat.Presets.R<SNormChannel<sbyte>>(8)]
    R8SNorm = 63, // 0x0000003F

    [RgbaxxPixelFormat.Presets.R<SNormChannel<sbyte>>(8)]
    R8SInt = 64, // 0x00000040

    [AxPixelFormat.Presets.A<UNormChannel<byte>>(8, AlphaType.All)]
    A8UNorm = 65, // 0x00000041

    [RgbaxxPixelFormat.Presets.R<UNormChannel<byte>>(1)]
    R1UNorm = 66, // 0x00000042

    R9G9B9E5Sharedexp = 67, // 0x00000043
    R8G8B8G8UNorm = 68, // 0x00000044
    G8R8G8B8UNorm = 69, // 0x00000045

    [BlockCompressionPixelFormat.Presets.Rgb<TypelessChannel>(1, 8, 4, 8, 8, 8)]
    Bc1Typeless = 70, // 0x00000046

    [BlockCompressionPixelFormat.Presets.Rgb<UNormChannel<byte>>(1, 8, 4, 8, 8, 8)]
    Bc1UNorm = 71, // 0x00000047

    [BlockCompressionPixelFormat.Presets.Rgb<UNormSrgbChannel<byte>>(1, 8, 4, 8, 8, 8)]
    Bc1UNormSrgb = 72, // 0x00000048

    [BlockCompressionPixelFormat.Presets.Rgba<TypelessChannel>(2, 16, 8, 8, 8, 8, 8, AlphaType.All)]
    Bc2Typeless = 73, // 0x00000049

    [BlockCompressionPixelFormat.Presets.Rgba<UNormChannel<byte>>(2, 16, 8, 8, 8, 8, 8, AlphaType.All)]
    Bc2UNorm = 74, // 0x0000004A

    [BlockCompressionPixelFormat.Presets.Rgba<UNormSrgbChannel<byte>>(2, 16, 8, 8, 8, 8, 8, AlphaType.All)]
    Bc2UNormSrgb = 75, // 0x0000004B

    [BlockCompressionPixelFormat.Presets.Rgba<TypelessChannel>(3, 16, 8, 8, 8, 8, 8, AlphaType.All)]
    Bc3Typeless = 76, // 0x0000004C

    [BlockCompressionPixelFormat.Presets.Rgba<UNormChannel<byte>>(3, 16, 8, 8, 8, 8, 8, AlphaType.All)]
    Bc3UNorm = 77, // 0x0000004D

    [BlockCompressionPixelFormat.Presets.Rgba<UNormSrgbChannel<byte>>(3, 16, 8, 8, 8, 8, 8, AlphaType.All)]
    Bc3UNormSrgb = 78, // 0x0000004E

    [BlockCompressionPixelFormat.Presets.R<TypelessChannel>(4, 8, 4, 8)]
    Bc4Typeless = 79, // 0x0000004F

    [BlockCompressionPixelFormat.Presets.R<UNormChannel<byte>>(4, 8, 4, 8)]
    Bc4UNorm = 80, // 0x00000050

    [BlockCompressionPixelFormat.Presets.R<SNormChannel<sbyte>>(4, 8, 4, 8)]
    Bc4SNorm = 81, // 0x00000051

    [BlockCompressionPixelFormat.Presets.Rg<TypelessChannel>(5, 16, 8, 8, 8)]
    Bc5Typeless = 82, // 0x00000052

    [BlockCompressionPixelFormat.Presets.Rg<UNormChannel<byte>>(5, 16, 8, 8, 8)]
    Bc5UNorm = 83, // 0x00000053

    [BlockCompressionPixelFormat.Presets.Rg<SNormChannel<sbyte>>(5, 16, 8, 8, 8)]
    Bc5SNorm = 84, // 0x00000054

    [RgbaxxPixelFormat.Presets.Bgr<UNormChannel<byte>>(5, 6, 5)]
    B5G6R5UNorm = 85, // 0x00000055

    [RgbaxxPixelFormat.Presets.Bgra<UNormChannel<byte>>(5, 5, 5, 1, AlphaType.All)]
    B5G5R5A1UNorm = 86, // 0x00000056

    [RgbaxxPixelFormat.Presets.Bgra<UNormChannel<byte>>(8, 8, 8, 8, AlphaType.All)]
    B8G8R8A8UNorm = 87, // 0x00000057

    [RgbaxxPixelFormat.Presets.Bgrx<UNormChannel<byte>>(8, 8, 8, 8)]
    B8G8R8X8UNorm = 88, // 0x00000058

    R10G10B10XrBiasA2UNorm = 89, // 0x00000059

    [RgbaxxPixelFormat.Presets.Bgra<TypelessChannel>(8, 8, 8, 8, AlphaType.All)]
    B8G8R8A8Typeless = 90, // 0x0000005A

    [RgbaxxPixelFormat.Presets.Bgra<UNormSrgbChannel<byte>>(8, 8, 8, 8, AlphaType.All)]
    B8G8R8A8UNormSrgb = 91, // 0x0000005B

    [RgbaxxPixelFormat.Presets.Bgrx<TypelessChannel>(8, 8, 8, 8)]
    B8G8R8X8Typeless = 92, // 0x0000005C

    [RgbaxxPixelFormat.Presets.Bgrx<UNormSrgbChannel<byte>>(8, 8, 8, 8)]
    B8G8R8X8UNormSrgb = 93, // 0x0000005D

    [BlockCompressionPixelFormat.Presets.Rgba<TypelessChannel>(6, 16, 8, 32, 32, 32, 32, AlphaType.All)]
    Bc6HTypeless = 94, // 0x0000005E

    [BlockCompressionPixelFormat.Presets.Rgba<F16Channel>(6, 16, 8, 32, 32, 32, 32, AlphaType.All)]
    Bc6HUf16 = 95, // 0x0000005F

    [BlockCompressionPixelFormat.Presets.Rgba<F16UnsignedChannel>(6, 16, 8, 32, 32, 32, 32, AlphaType.All)]
    Bc6HSf16 = 96, // 0x00000060

    [BlockCompressionPixelFormat.Presets.Rgba<TypelessChannel>(7, 16, 8, 8, 8, 8, 8, AlphaType.All)]
    Bc7Typeless = 97, // 0x00000061

    [BlockCompressionPixelFormat.Presets.Rgba<UNormChannel<byte>>(7, 16, 8, 8, 8, 8, 8, AlphaType.All)]
    Bc7UNorm = 98, // 0x00000062

    [BlockCompressionPixelFormat.Presets.Rgba<UNormSrgbChannel<byte>>(7, 16, 8, 8, 8, 8, 8, AlphaType.All)]
    Bc7UNormSrgb = 99, // 0x00000063
    Ayuv = 100, // 0x00000064
    Y410 = 101, // 0x00000065
    Y416 = 102, // 0x00000066
    Nv12 = 103, // 0x00000067
    P010 = 104, // 0x00000068
    P016 = 105, // 0x00000069
    Yuv420Opaque = 106, // 0x0000006A
    Yuy2 = 107, // 0x0000006B
    Y210 = 108, // 0x0000006C
    Y216 = 109, // 0x0000006D
    Nv11 = 110, // 0x0000006E
    Ai44 = 111, // 0x0000006F
    Ia44 = 112, // 0x00000070
    P8 = 113, // 0x00000071
    A8P8 = 114, // 0x00000072

    [RgbaxxPixelFormat.Presets.Bgra<UNormChannel<byte>>(4, 4, 4, 4, AlphaType.All)]
    B4G4R4A4UNorm = 115, // 0x00000073
    R10G10B10_7E3_A2Float = 116,
    R10G10B10_6E4_A2Float = 117,

    [DepthStencilPixelFormat<UNormChannel<ushort>, UIntChannel<byte>>(16, 8)]
    D16UNormS8UInt = 118,

    [RgbaxxPixelFormat.Presets.Rx<UNormChannel<ushort>, TypelessChannel>(16, 8)]
    R16UNormX8Typeless = 119,

    [RgbaxxPixelFormat.Presets.Xg<TypelessChannel, UIntChannel<byte>>(16, 8)]
    X16TypelessG8UInt = 120,
    P208 = 130, // 0x00000082
    V208 = 131, // 0x00000083
    V408 = 132, // 0x00000084

    [RgbaxxPixelFormat.Presets.Rgba<SNormChannel<short>, UNormChannel<byte>>(10, 10, 10, 2, AlphaType.All)]
    R10G10B10SNormA2UNorm = 189, // 0x000000BD

    [RgbaxxPixelFormat.Presets.Rg<UNormChannel<byte>>(4, 4)]
    R4G4UNorm = 190, // 0x000000BE

    [RgbaxxPixelFormat.Presets.Abgr<UNormChannel<byte>>(4, AlphaType.All, 4, 4, 4)]
    A4B4G4R4UNorm = 191, // 0x000000BF
#pragma warning restore CS1591
}

public static class DxgiFormatExtensions {
    public static bool TryGetDxgiFormat(this IPixelFormat value, out DxgiFormat dxgiFormat) {
        dxgiFormat = DxgiFormat.Unknown;
        
        if (Enum.GetNames<DxgiFormat>()
                .FirstOrDefault(x => value.Equals(typeof(DxgiFormat).GetField(x)?.GetCustomAttribute<PixelFormat>())) is not { } dxgiFormatName)
            return false;
        
        dxgiFormat = Enum.Parse<DxgiFormat>(dxgiFormatName);
        return true;
    }

    public static bool TryGetPixelFormat(this DxgiFormat dxgiFormat, [MaybeNullWhen(false)] out IPixelFormat pixelFormat) {
        pixelFormat = null;

        if (Enum.GetName(dxgiFormat) is not { } name)
            return false;
        if (typeof(DxgiFormat).GetField(name)?.GetCustomAttribute<PixelFormat>() is not { } pf)
            return false;
        pixelFormat = pf;
        return true;
    }
}