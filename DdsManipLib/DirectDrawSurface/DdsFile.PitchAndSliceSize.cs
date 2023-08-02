using System;

namespace DdsManipLib.DirectDrawSurface;

public partial class DdsFile
{
    /// <summary>
    /// Get the pitch(stride) and slice size of the specified mipmap in this file.
    /// </summary>
    /// <param name="mipmapIndex">Index of the mipmap.</param>
    /// <param name="pitch">Calculated pitch.</param>
    /// <param name="sliceSize">Calculated slice size.</param>
    /// <returns>Whether the operation has succeeded.</returns>
    public bool PitchAndSliceSize( int mipmapIndex, out int pitch, out int sliceSize )
    {
        pitch = sliceSize = 0;

        var w = Width( mipmapIndex );
        var h = Height( mipmapIndex );
        if( !Header.PixelFormat.Flags.HasFlag( DdsPixelFormatFlags.FourCc ) )
        {
            if( 0 == ( Header.PixelFormat.Flags & DdsPixelFormatFlags.ValidRgbBitCountFieldMask ) )
                return false;
            pitch = ( w * Header.PixelFormat.RgbBitCount + 7 ) / 8;
            sliceSize = pitch * h;
            return true;
        }

        switch( Header.PixelFormat.FourCc )
        {
            case DdsFourCc.Dxt1:
            case DdsFourCc.Bc4S:
            case DdsFourCc.Bc4U:
            {
                var nbw = Math.Max( 1, ( w + 3 ) / 4 );
                var nbh = Math.Max( 1, ( h + 3 ) / 4 );
                pitch = nbw * 8;
                sliceSize = pitch * nbh;
                return true;
            }
            case DdsFourCc.Dxt2:
            case DdsFourCc.Dxt3:
            case DdsFourCc.Dxt4:
            case DdsFourCc.Dxt5:
            case DdsFourCc.Ati1:
            case DdsFourCc.Ati2:
            case DdsFourCc.Atc:
            case DdsFourCc.AtcI:
            case DdsFourCc.AtcA:
            case DdsFourCc.Bc5S:
            case DdsFourCc.Bc5U:
            {
                var nbw = Math.Max( 1, ( w + 3 ) / 4 );
                var nbh = Math.Max( 1, ( h + 3 ) / 4 );
                pitch = nbw * 16;
                sliceSize = pitch * nbh;
                return true;
            }
            case DdsFourCc.D3dFmtR8G8B8G8:
            case DdsFourCc.D3dFmtG8R8G8B8:
            case DdsFourCc.D3dFmtUyvy:
            case DdsFourCc.D3dFmtYuy2:
                pitch = ( ( w + 1 ) >> 1 ) * 4;
                sliceSize = pitch * h;
                return true;
            case DdsFourCc.Dx10:

                // https://github.com/microsoft/DirectXTex/blob/4d9d7a8ceba6d6a121cd1aae160a0b856ef03d89/DirectXTex/DirectXTexUtil.cpp#L919
                int bpp;
                switch( HeaderDxt10.DxgiFormat )
                {
                    case DxgiFormat.Bc1Typeless:
                    case DxgiFormat.Bc1UNorm:
                    case DxgiFormat.Bc1UNormSrgb:
                    case DxgiFormat.Bc4Typeless:
                    case DxgiFormat.Bc4UNorm:
                    case DxgiFormat.Bc4SNorm:
                    {
                        var nbw = Math.Max( 1, ( w + 3 ) / 4 );
                        var nbh = Math.Max( 1, ( h + 3 ) / 4 );
                        pitch = nbw * 8;
                        sliceSize = pitch * nbh;
                        return true;
                    }
                    case DxgiFormat.Bc2Typeless:
                    case DxgiFormat.Bc2UNorm:
                    case DxgiFormat.Bc2UNormSrgb:
                    case DxgiFormat.Bc3Typeless:
                    case DxgiFormat.Bc3UNorm:
                    case DxgiFormat.Bc3UNormSrgb:
                    case DxgiFormat.Bc5Typeless:
                    case DxgiFormat.Bc5UNorm:
                    case DxgiFormat.Bc5SNorm:
                    case DxgiFormat.Bc6HTypeless:
                    case DxgiFormat.Bc6HUf16:
                    case DxgiFormat.Bc6HSf16:
                    case DxgiFormat.Bc7Typeless:
                    case DxgiFormat.Bc7UNorm:
                    case DxgiFormat.Bc7UNormSrgb:
                    {
                        var nbw = Math.Max( 1, ( w + 3 ) / 4 );
                        var nbh = Math.Max( 1, ( h + 3 ) / 4 );
                        pitch = nbw * 16;
                        sliceSize = pitch * nbh;
                        return true;
                    }
                    case DxgiFormat.R8G8B8G8UNorm:
                    case DxgiFormat.G8R8G8B8UNorm:
                    case DxgiFormat.Yuy2:
                        pitch = ( ( w + 1 ) >> 1 ) * 4;
                        sliceSize = pitch * h;
                        return true;
                    case DxgiFormat.Y210:
                    case DxgiFormat.Y216:
                        pitch = ( ( w + 1 ) >> 1 ) * 8;
                        sliceSize = pitch * h;
                        return true;
                    case DxgiFormat.Nv12:
                    case DxgiFormat.Yuv420Opaque:
                        if( h % 2 != 0 )
                            return false;
                        pitch = ( ( w + 1 ) >> 1 ) * 2;
                        sliceSize = pitch * ( h + ( ( h + 1 ) >> 1 ) );
                        return true;
                    case DxgiFormat.P010:
                    case DxgiFormat.P016:
                        if( h % 2 != 0 )
                            return false;
                        goto case DxgiFormat.D16UNormS8UInt;
                    case DxgiFormat.D16UNormS8UInt:
                    case DxgiFormat.R16UNormX8Typeless:
                    case DxgiFormat.X16TypelessG8UInt:
                        pitch = ( ( w + 1 ) >> 1 ) * 4;
                        sliceSize = pitch * ( h + ( ( h + 1 ) >> 1 ) );
                        return true;
                    case DxgiFormat.Nv11:
                        pitch = ( ( w + 3 ) >> 2 ) * 4;
                        sliceSize = pitch * h * 2;
                        return true;
                    case DxgiFormat.P208:
                        pitch = ( ( w + 1 ) >> 1 ) * 2;
                        sliceSize = pitch * h * 2;
                        return true;
                    case DxgiFormat.V208:
                        if( h % 2 != 0 )
                            return false;
                        pitch = w;
                        sliceSize = pitch * ( h + ( ( h + 1 ) >> 1 ) * 2 );
                        return true;
                    case DxgiFormat.V408:
                        pitch = w;
                        sliceSize = pitch * ( h + ( h >> 1 ) * 4 );
                        return true;
                    case DxgiFormat.R32G32B32A32Typeless:
                    case DxgiFormat.R32G32B32A32Float:
                    case DxgiFormat.R32G32B32A32UInt:
                    case DxgiFormat.R32G32B32A32SInt:
                        bpp = 128;
                        break;
                    case DxgiFormat.R32G32B32Typeless:
                    case DxgiFormat.R32G32B32Float:
                    case DxgiFormat.R32G32B32UInt:
                    case DxgiFormat.R32G32B32SInt:
                        bpp = 96;
                        break;
                    case DxgiFormat.R16G16B16A16Typeless:
                    case DxgiFormat.R16G16B16A16Float:
                    case DxgiFormat.R16G16B16A16UNorm:
                    case DxgiFormat.R16G16B16A16UInt:
                    case DxgiFormat.R16G16B16A16SNorm:
                    case DxgiFormat.R16G16B16A16SInt:
                    case DxgiFormat.R32G32Typeless:
                    case DxgiFormat.R32G32Float:
                    case DxgiFormat.R32G32UInt:
                    case DxgiFormat.R32G32SInt:
                    case DxgiFormat.R32G8X24Typeless:
                    case DxgiFormat.D32FloatS8X24UInt:
                    case DxgiFormat.R32FloatX8X24Typeless:
                    case DxgiFormat.X32TypelessG8X24UInt:
                    case DxgiFormat.Y416:
                        bpp = 64;
                        break;
                    case DxgiFormat.R10G10B10A2Typeless:
                    case DxgiFormat.R10G10B10A2UNorm:
                    case DxgiFormat.R10G10B10A2UInt:
                    case DxgiFormat.R11G11B10Float:
                    case DxgiFormat.R8G8B8A8Typeless:
                    case DxgiFormat.R8G8B8A8UNorm:
                    case DxgiFormat.R8G8B8A8UNormSrgb:
                    case DxgiFormat.R8G8B8A8UInt:
                    case DxgiFormat.R8G8B8A8SNorm:
                    case DxgiFormat.R8G8B8A8SInt:
                    case DxgiFormat.R16G16Typeless:
                    case DxgiFormat.R16G16Float:
                    case DxgiFormat.R16G16UNorm:
                    case DxgiFormat.R16G16UInt:
                    case DxgiFormat.R16G16SNorm:
                    case DxgiFormat.R16G16SInt:
                    case DxgiFormat.R32Typeless:
                    case DxgiFormat.D32Float:
                    case DxgiFormat.R32Float:
                    case DxgiFormat.R32UInt:
                    case DxgiFormat.R32SInt:
                    case DxgiFormat.R24G8Typeless:
                    case DxgiFormat.D24UNormS8UInt:
                    case DxgiFormat.R24UNormX8Typeless:
                    case DxgiFormat.X24TypelessG8UInt:
                    case DxgiFormat.R9G9B9E5Sharedexp:
                    case DxgiFormat.B8G8R8A8UNorm:
                    case DxgiFormat.B8G8R8X8UNorm:
                    case DxgiFormat.R10G10B10XrBiasA2UNorm:
                    case DxgiFormat.B8G8R8A8Typeless:
                    case DxgiFormat.B8G8R8A8UNormSrgb:
                    case DxgiFormat.B8G8R8X8Typeless:
                    case DxgiFormat.B8G8R8X8UNormSrgb:
                    case DxgiFormat.Ayuv:
                    case DxgiFormat.Y410:
                    case DxgiFormat.R10G10B10_7E3_A2Float:
                    case DxgiFormat.R10G10B10_6E4_A2Float:
                    case DxgiFormat.R10G10B10SNormA2UNorm:
                        bpp = 32;
                        break;
                    case DxgiFormat.R8G8Typeless:
                    case DxgiFormat.R8G8UNorm:
                    case DxgiFormat.R8G8UInt:
                    case DxgiFormat.R8G8SNorm:
                    case DxgiFormat.R8G8SInt:
                    case DxgiFormat.R16Typeless:
                    case DxgiFormat.R16Float:
                    case DxgiFormat.D16UNorm:
                    case DxgiFormat.R16UNorm:
                    case DxgiFormat.R16UInt:
                    case DxgiFormat.R16SNorm:
                    case DxgiFormat.R16SInt:
                    case DxgiFormat.B5G6R5UNorm:
                    case DxgiFormat.B5G5R5A1UNorm:
                    case DxgiFormat.A8P8:
                    case DxgiFormat.B4G4R4A4UNorm:
                    case DxgiFormat.A4B4G4R4UNorm:
                        bpp = 16;
                        break;
                    case DxgiFormat.R8Typeless:
                    case DxgiFormat.R8UNorm:
                    case DxgiFormat.R8UInt:
                    case DxgiFormat.R8SNorm:
                    case DxgiFormat.R8SInt:
                    case DxgiFormat.A8UNorm:
                    case DxgiFormat.Ai44:
                    case DxgiFormat.Ia44:
                    case DxgiFormat.P8:
                    case DxgiFormat.R4G4UNorm:
                        bpp = 8;
                        break;
                    case DxgiFormat.R1UNorm:
                        bpp = 1;
                        break;
                    default:
                        return false;
                }

                pitch = bpp * ( w + 7 ) / 8;
                sliceSize = pitch * h;
                return true;
            default:
                return false;
        }
    }
}