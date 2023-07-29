using System.IO;
using System.Runtime.InteropServices;
using DdsManipLib.DirectDrawSurface.PixelFormats;
using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

namespace DdsManipLib.DirectDrawSurface;

/// <summary>
/// DDS header extension to handle resource arrays, DXGI pixel formats that don't map to the legacy Microsoft DirectDraw pixel format structures, and additional metadata.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct DdsHeaderDxt10 {
    /// <summary>The surface pixel format.</summary>
    public DxgiFormat DxgiFormat;

    /// <summary>Identifies the type of resource.</summary>
    public DdsHeaderDxt10ResourceDimension ResourceDimension;

    /// <summary>Identifies other, less common options for resources.</summary>
    public DdsHeaderDxt10MiscFlags MiscFlag;

    /// <summary>
    /// The number of elements in the array.
    /// 
    /// For a 2D texture that is also a cube-map texture, this number represents the number of cubes. This number is
    /// the same as the number in the NumCubes member of D3D10_TEXCUBE_ARRAY_SRV1 or D3D11_TEXCUBE_ARRAY_SRV). In this
    /// case, the DDS file contains arraySize*6 2D textures. For more information about this case, see the miscFlag
    /// description.
    /// 
    /// For a 3D texture, you must set this number to 1.
    /// </summary>
    public int ArraySize;

    /// <summary>
    /// Contains additional metadata (formerly was reserved). The lower 3 bits indicate the alpha mode of the
    /// associated resource. The upper 29 bits are reserved and are typically 0.
    /// </summary>
    public DdsHeaderDxt10MiscFlags2 MiscFlags2;

    /// <summary>
    /// Read the struct from the given BinaryReader.
    /// </summary>
    /// <param name="reader">Little-endian BinaryReader to read from.</param>
    public void ReadFrom(BinaryReader reader) {
        DxgiFormat = (DxgiFormat) reader.ReadInt32();
        ResourceDimension = (DdsHeaderDxt10ResourceDimension) reader.ReadInt32();
        MiscFlag = (DdsHeaderDxt10MiscFlags) reader.ReadInt32();
        ArraySize = reader.ReadInt32();
        MiscFlags2 = (DdsHeaderDxt10MiscFlags2) reader.ReadInt32();
    }

    /// <summary>
    /// Write the struct to the given BinaryWriter.
    /// </summary>
    /// <param name="writer">Little-endian BinaryWriter to write to.</param>
    public readonly void WriteTo(BinaryWriter writer) {
        writer.Write((int) DxgiFormat);
        writer.Write((int) ResourceDimension);
        writer.Write((int) MiscFlag);
        writer.Write(ArraySize);
        writer.Write((int) MiscFlags2);
    }

    /// <summary>
    /// Attempt to deduce a corresponding <see cref="PixelFormat"/>.
    /// </summary>
    /// <param name="pixelFormat">The resulting pixel format, or <see cref="UnknownPixelFormat"/> if not found.</param>
    /// <returns>Whether the corresponding format has been found.</returns>
    public bool TryToPixelFormat(out PixelFormat pixelFormat) {
        pixelFormat = UnknownPixelFormat.Instance;
        var dxt10AlphaMode = MiscFlags2 & DdsHeaderDxt10MiscFlags2.AlphaMask;
        switch (dxt10AlphaMode) {
            case DdsHeaderDxt10MiscFlags2.AlphaModeUnknown:
            case DdsHeaderDxt10MiscFlags2.AlphaModeStraight:
                pixelFormat = DxgiFormat.ToPixelFormat(AlphaType.Straight);
                return true;
            case DdsHeaderDxt10MiscFlags2.AlphaModePremultiplied:
                pixelFormat = DxgiFormat.ToPixelFormat(AlphaType.Premultiplied);
                return true;
            case DdsHeaderDxt10MiscFlags2.AlphaModeOpaque:
                pixelFormat = DxgiFormat.ToPixelFormat(AlphaType.None);
                return true;
            case DdsHeaderDxt10MiscFlags2.AlphaModeCustom:
                pixelFormat = DxgiFormat.ToPixelFormat(AlphaType.Custom);
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Attempt to deduce the fields of this object from a <see cref="PixelFormat"/>.
    /// </summary>
    /// <param name="value">The value to convert from.</param>
    /// <returns>Whether the corresponding format has been found.</returns>
    public bool TryUpdateFromPixelFormat(PixelFormat value) {
        var dxgi = value.ToDxgiFormat();
        if (dxgi == DxgiFormat.Unknown)
            return false;

        DxgiFormat = dxgi;
        MiscFlags2 = value.Alpha switch {
            AlphaType.None => DdsHeaderDxt10MiscFlags2.AlphaModeOpaque,
            AlphaType.Straight => DdsHeaderDxt10MiscFlags2.AlphaModeStraight,
            AlphaType.Premultiplied => DdsHeaderDxt10MiscFlags2.AlphaModePremultiplied,
            AlphaType.Custom => DdsHeaderDxt10MiscFlags2.AlphaModeCustom,
            _ => DdsHeaderDxt10MiscFlags2.AlphaModeStraight,
        };
        return true;
    }
}
