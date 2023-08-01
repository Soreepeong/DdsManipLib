using System.Diagnostics.CodeAnalysis;
using DdsManipLib.DirectDrawSurface.PixelFormats;
using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

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

    [RgbaPixelFormat<uint, TypelessChannel<uint>>(32, 32, 32, 32, AlphaType.All)]
    R32G32B32A32Typeless = 1,

    [RgbaPixelFormat<float, F32Channel>(32, 32, 32, 32, AlphaType.All)]
    R32G32B32A32Float = 2,

    [RgbaPixelFormat<uint, UIntChannel<uint>>(32, 32, 32, 32, AlphaType.All)]
    R32G32B32A32Uint = 3,

    [RgbaPixelFormat<int, SIntChannel<int>>(32, 32, 32, 32, AlphaType.All)]
    R32G32B32A32Sint = 4,

    [RgbPixelFormat<uint, TypelessChannel<uint>>(32, 32, 32)]
    R32G32B32Typeless = 5,

    [RgbPixelFormat<float, F32Channel>(32, 32, 32)]
    R32G32B32Float = 6,

    [RgbPixelFormat<uint, UIntChannel<uint>>(32, 32, 32)]
    R32G32B32Uint = 7,

    [RgbPixelFormat<int, SIntChannel<int>>(32, 32, 32)]
    R32G32B32Sint = 8,

    [RgbaPixelFormat<uint, TypelessChannel<uint>>(16, 16, 16, 16, AlphaType.All)]
    R16G16B16A16Typeless = 9,

    [RgbaPixelFormat<float, F32Channel>(16, 16, 16, 16, AlphaType.All)]
    R16G16B16A16Float = 10, // 0x0000000A

    [RgbaPixelFormat<ushort, UNormChannel<ushort>>(16, 16, 16, 16, AlphaType.All)]
    R16G16B16A16Unorm = 11, // 0x0000000B

    [RgbaPixelFormat<ushort, UIntChannel<ushort>>(16, 16, 16, 16, AlphaType.All)]
    R16G16B16A16Uint = 12, // 0x0000000C

    [RgbaPixelFormat<short, SNormChannel<short>>(16, 16, 16, 16, AlphaType.All)]
    R16G16B16A16Snorm = 13, // 0x0000000D

    [RgbaPixelFormat<short, SIntChannel<short>>(16, 16, 16, 16, AlphaType.All)]
    R16G16B16A16Sint = 14, // 0x0000000E

    [RedGreenPixelFormat<uint, TypelessChannel<uint>>(32, 32)]
    R32G32Typeless = 15, // 0x0000000F

    [RedGreenPixelFormat<float, F32Channel>(32, 32)]
    R32G32Float = 16, // 0x00000010

    [RedGreenPixelFormat<uint, UIntChannel<uint>>(32, 32)]
    R32G32Uint = 17, // 0x00000011

    [RedGreenPixelFormat<int, SIntChannel<int>>(32, 32)]
    R32G32Sint = 18, // 0x00000012

    [RgxPixelFormat<uint, TypelessChannel<uint>, uint>(32, 8, 24)]
    R32G8X24Typeless = 19, // 0x00000013
    
    [DsxPixelFormatAttribute<float, F32Channel, byte, UIntChannel<byte>, uint, UIntChannel<uint>>(32, 8, 24)]
    D32FloatS8X24Uint = 20, // 0x00000014
    
    [RxxPixelFormat<float, F32Channel, byte, uint>(32, 8, 24)]
    R32FloatX8X24Typeless = 21, // 0x00000015
    
    [XgxPixelFormat<byte, UIntChannel<byte>, uint, uint>(32, 8, 24)]
    X32TypelessG8X24Uint = 22, // 0x00000016

    [RgbaPixelFormat<ushort, TypelessChannel<ushort>>(10, 10, 10, 2, AlphaType.All)]
    R10G10B10A2Typeless = 23, // 0x00000017

    [RgbaPixelFormat<ushort, UNormChannel<ushort>>(10, 10, 10, 2, AlphaType.All)]
    R10G10B10A2Unorm = 24, // 0x00000018

    [RgbaPixelFormat<ushort, UIntChannel<ushort>>(10, 10, 10, 2, AlphaType.All)]
    R10G10B10A2Uint = 25, // 0x00000019

    R11G11B10Float = 26, // 0x0000001A

    [RgbaPixelFormat<byte, TypelessChannel<byte>>(8, 8, 8, 8, AlphaType.All)]
    R8G8B8A8Typeless = 27, // 0x0000001B

    [RgbaPixelFormat<byte, UNormChannel<byte>>(8, 8, 8, 8, AlphaType.All)]
    R8G8B8A8Unorm = 28, // 0x0000001C

    [RgbaPixelFormat<byte, UNormSrgbChannel<byte>>(8, 8, 8, 8, AlphaType.All)]
    R8G8B8A8UnormSrgb = 29, // 0x0000001D

    [RgbaPixelFormat<byte, UIntChannel<byte>>(8, 8, 8, 8, AlphaType.All)]
    R8G8B8A8Uint = 30, // 0x0000001E

    [RgbaPixelFormat<sbyte, SNormChannel<sbyte>>(8, 8, 8, 8, AlphaType.All)]
    R8G8B8A8Snorm = 31, // 0x0000001F

    [RgbaPixelFormat<sbyte, SIntChannel<sbyte>>(8, 8, 8, 8, AlphaType.All)]
    R8G8B8A8Sint = 32, // 0x00000020

    [RedGreenPixelFormat<ushort, TypelessChannel<ushort>>(16, 16)]
    R16G16Typeless = 33, // 0x00000021

    [RedGreenPixelFormat<float, F16Channel>(16, 16)]
    R16G16Float = 34, // 0x00000022

    [RedGreenPixelFormat<ushort, UNormChannel<ushort>>(16, 16)]
    R16G16Unorm = 35, // 0x00000023

    [RedGreenPixelFormat<ushort, UIntChannel<ushort>>(16, 16)]
    R16G16Uint = 36, // 0x00000024

    [RedGreenPixelFormat<short, SNormChannel<short>>(16, 16)]
    R16G16Snorm = 37, // 0x00000025

    [RedGreenPixelFormat<short, SIntChannel<short>>(16, 16)]
    R16G16Sint = 38, // 0x00000026

    [RedPixelFormat<uint, TypelessChannel<uint>>(32)]
    R32Typeless = 39, // 0x00000027
    
    [DepthPixelFormat<float, F32Channel>(32)]
    D32Float = 40, // 0x00000028

    [RedPixelFormat<float, F32Channel>(32)]
    R32Float = 41, // 0x00000029

    [RedPixelFormat<uint, UIntChannel<uint>>(32)]
    R32Uint = 42, // 0x0000002A

    [RedPixelFormat<int, SIntChannel<int>>(32)]
    R32Sint = 43, // 0x0000002B

    [RedGreenPixelFormat<uint, TypelessChannel<uint>>(24, 8)]
    R24G8Typeless = 44, // 0x0000002C
    
    [DepthStencilPixelFormat<uint, UNormChannel<uint>, byte, UIntChannel<byte>>(24, 8)]
    D24UnormS8Uint = 45, // 0x0000002D
    
    [RxPixelFormat<uint, UNormChannel<uint>, byte>(24, 8)]
    R24UnormX8Typeless = 46, // 0x0000002E
    
    [XgPixelFormat<uint, UIntChannel<uint>, byte>(24, 8)]
    X24TypelessG8Uint = 47, // 0x0000002F

    [RedGreenPixelFormat<byte, TypelessChannel<byte>>(8, 8)]
    R8G8Typeless = 48, // 0x00000030

    [RedGreenPixelFormat<byte, UNormChannel<byte>>(8, 8)]
    R8G8Unorm = 49, // 0x00000031

    [RedGreenPixelFormat<byte, UIntChannel<byte>>(8, 8)]
    R8G8Uint = 50, // 0x00000032

    [RedGreenPixelFormat<sbyte, SNormChannel<sbyte>>(8, 8)]
    R8G8Snorm = 51, // 0x00000033

    [RedGreenPixelFormat<sbyte, SIntChannel<sbyte>>(8, 8)]
    R8G8Sint = 52, // 0x00000034

    [RedPixelFormat<ushort, TypelessChannel<ushort>>(16)]
    R16Typeless = 53, // 0x00000035

    [RedPixelFormat<float, F32Channel>(16)]
    R16Float = 54, // 0x00000036
    
    [DepthPixelFormat<ushort, UNormChannel<ushort>>(16)]
    D16Unorm = 55, // 0x00000037

    [RedPixelFormat<ushort, UNormChannel<ushort>>(16)]
    R16Unorm = 56, // 0x00000038

    [RedPixelFormat<ushort, UIntChannel<ushort>>(16)]
    R16Uint = 57, // 0x00000039

    [RedPixelFormat<short, SNormChannel<short>>(16)]
    R16Snorm = 58, // 0x0000003A

    [RedPixelFormat<short, SIntChannel<short>>(16)]
    R16Sint = 59, // 0x0000003B

    [RedPixelFormat<byte, TypelessChannel<byte>>(8)]
    R8Typeless = 60, // 0x0000003C

    [RedPixelFormat<byte, UNormChannel<byte>>(8)]
    R8Unorm = 61, // 0x0000003D

    [RedPixelFormat<byte, UIntChannel<byte>>(8)]
    R8Uint = 62, // 0x0000003E

    [RedPixelFormat<sbyte, SNormChannel<sbyte>>(8)]
    R8Snorm = 63, // 0x0000003F

    [RedPixelFormat<sbyte, SNormChannel<sbyte>>(8)]
    R8Sint = 64, // 0x00000040

    [AlphaPixelFormat<byte, UNormChannel<byte>>(8, AlphaType.All)]
    A8Unorm = 65, // 0x00000041

    [RedPixelFormat<byte, UNormChannel<byte>>(1)]
    R1Unorm = 66, // 0x00000042

    R9G9B9E5Sharedexp = 67, // 0x00000043
    R8G8B8G8Unorm = 68, // 0x00000044
    G8R8G8B8Unorm = 69, // 0x00000045

    [RgbBcPixelFormat<byte, TypelessChannel<byte>>(1, 8, 4, 8, 8, 8)]
    Bc1Typeless = 70, // 0x00000046

    [RgbBcPixelFormat<byte, UNormChannel<byte>>(1, 8, 4, 8, 8, 8)]
    Bc1Unorm = 71, // 0x00000047

    [RgbBcPixelFormat<byte, UNormSrgbChannel<byte>>(1, 8, 4, 8, 8, 8)]
    Bc1UnormSrgb = 72, // 0x00000048

    [RgbaBcPixelFormat<byte, TypelessChannel<byte>>(2, 16, 8, 8, 8, 8, 8, AlphaType.All)]
    Bc2Typeless = 73, // 0x00000049

    [RgbaBcPixelFormat<byte, UNormChannel<byte>>(2, 16, 8, 8, 8, 8, 8, AlphaType.All)]
    Bc2Unorm = 74, // 0x0000004A

    [RgbaBcPixelFormat<byte, UNormSrgbChannel<byte>>(2, 16, 8, 8, 8, 8, 8, AlphaType.All)]
    Bc2UnormSrgb = 75, // 0x0000004B

    [RgbaBcPixelFormat<byte, TypelessChannel<byte>>(3, 16, 8, 8, 8, 8, 8, AlphaType.All)]
    Bc3Typeless = 76, // 0x0000004C

    [RgbaBcPixelFormat<byte, UNormChannel<byte>>(3, 16, 8, 8, 8, 8, 8, AlphaType.All)]
    Bc3Unorm = 77, // 0x0000004D

    [RgbaBcPixelFormat<byte, UNormSrgbChannel<byte>>(3, 16, 8, 8, 8, 8, 8, AlphaType.All)]
    Bc3UnormSrgb = 78, // 0x0000004E

    [RedBcPixelFormat<byte, TypelessChannel<byte>>(4, 8, 4, 8)]
    Bc4Typeless = 79, // 0x0000004F

    [RedBcPixelFormat<byte, UNormChannel<byte>>(4, 8, 4, 8)]
    Bc4Unorm = 80, // 0x00000050

    [RedBcPixelFormat<sbyte, SNormChannel<sbyte>>(4, 8, 4, 8)]
    Bc4Snorm = 81, // 0x00000051

    [RedGreenBcPixelFormat<byte, TypelessChannel<byte>>(5, 16, 8, 8, 8)]
    Bc5Typeless = 82, // 0x00000052

    [RedGreenBcPixelFormat<byte, UNormChannel<byte>>(5, 16, 8, 8, 8)]
    Bc5Unorm = 83, // 0x00000053

    [RedGreenBcPixelFormat<sbyte, SNormChannel<sbyte>>(5, 16, 8, 8, 8)]
    Bc5Snorm = 84, // 0x00000054

    [BgrPixelFormat<byte, UNormChannel<byte>>(5, 6, 5)]
    B5G6R5Unorm = 85, // 0x00000055

    [BgraPixelFormat<byte, UNormChannel<byte>>(5, 5, 5, 1, AlphaType.All)]
    B5G5R5A1Unorm = 86, // 0x00000056

    [BgraPixelFormat<byte, UNormChannel<byte>>(8, 8, 8, 8, AlphaType.All)]
    B8G8R8A8Unorm = 87, // 0x00000057
    
    [BgrxPixelFormat<byte, UNormChannel<byte>>(8, 8, 8, 8)]
    B8G8R8X8Unorm = 88, // 0x00000058
    
    R10G10B10XrBiasA2Unorm = 89, // 0x00000059

    [BgraPixelFormat<byte, TypelessChannel<byte>>(8, 8, 8, 8, AlphaType.All)]
    B8G8R8A8Typeless = 90, // 0x0000005A

    [BgraPixelFormat<byte, UNormSrgbChannel<byte>>(8, 8, 8, 8, AlphaType.All)]
    B8G8R8A8UnormSrgb = 91, // 0x0000005B
    
    [BgrxPixelFormat<byte, TypelessChannel<byte>>(8, 8, 8, 8)]
    B8G8R8X8Typeless = 92, // 0x0000005C
    
    [BgrxPixelFormat<byte, UNormSrgbChannel<byte>>(8, 8, 8, 8)]
    B8G8R8X8UnormSrgb = 93, // 0x0000005D

    [RgbaBcPixelFormat<ushort, TypelessChannel<ushort>>(6, 16, 8, 32, 32, 32, 32, AlphaType.All)]
    Bc6HTypeless = 94, // 0x0000005E

    [RgbaBcPixelFormat<float, F16Channel>(6, 16, 8, 32, 32, 32, 32, AlphaType.All)]
    Bc6HUf16 = 95, // 0x0000005F

    [RgbaBcPixelFormat<float, F16UnsignedChannel>(6, 16, 8, 32, 32, 32, 32, AlphaType.All)]
    Bc6HSf16 = 96, // 0x00000060

    [RgbaBcPixelFormat<byte, TypelessChannel<byte>>(7, 16, 8, 8, 8, 8, 8, AlphaType.All)]
    Bc7Typeless = 97, // 0x00000061

    [RgbaBcPixelFormat<byte, UNormChannel<byte>>(7, 16, 8, 8, 8, 8, 8, AlphaType.All)]
    Bc7Unorm = 98, // 0x00000062

    [RgbaBcPixelFormat<byte, UNormSrgbChannel<byte>>(7, 16, 8, 8, 8, 8, 8, AlphaType.All)]
    Bc7UnormSrgb = 99, // 0x00000063
    Ayuv = 100, // 0x00000064
    Y410 = 101, // 0x00000065
    Y416 = 102, // 0x00000066
    Nv12 = 103, // 0x00000067
    P010 = 104, // 0x00000068
    P016 = 105, // 0x00000069
    YUV420_OPAQUE = 106, // 0x0000006A
    Yuy2 = 107, // 0x0000006B
    Y210 = 108, // 0x0000006C
    Y216 = 109, // 0x0000006D
    Nv11 = 110, // 0x0000006E
    Ai44 = 111, // 0x0000006F
    Ia44 = 112, // 0x00000070
    P8 = 113, // 0x00000071
    A8P8 = 114, // 0x00000072

    [BgraPixelFormat<byte, UNormChannel<byte>>(4, 4, 4, 4, AlphaType.All)]
    B4G4R4A4Unorm = 115, // 0x00000073
    R10G10B10_7E3_A2Float = 116,
    R10G10B10_6E4_A2Float = 117,
    
    [DepthStencilPixelFormat<ushort, UNormChannel<ushort>, byte, UIntChannel<byte>>(16, 8)]
    D16UnormS8Uint = 118,
    
    [RxPixelFormat<ushort, UNormChannel<ushort>, byte>(16, 8)]
    R16UnormX8Typeless = 119,
    
    [XgPixelFormat<byte, UIntChannel<byte>, ushort>(16, 8)]
    X16TypelessG8Uint = 120,
    P208 = 130, // 0x00000082
    V208 = 131, // 0x00000083
    V408 = 132, // 0x00000084
    R10G10B10SnormA2Unorm = 189, // 0x000000BD

    [RedGreenPixelFormat<byte, UNormChannel<byte>>(4, 4)]
    R4G4Unorm = 190, // 0x000000BE
    A4B4G4R4Unorm = 191, // 0x000000BF
#pragma warning restore CS1591
}
