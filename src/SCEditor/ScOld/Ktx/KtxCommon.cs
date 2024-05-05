using System.Text;
using System.Collections.Generic;

// Common global values
namespace KtxSharp
{
	/// <summary>
	/// Common values
	/// </summary>
	public static class Common
	{
		/// <summary>
		/// There is only one valid file identifier for KTX header, it is '«', 'K', 'T', 'X', ' ', '1', '1', '»', '\r', '\n', '\x1A', '\n'
		/// </summary>
		/// <value></value>
		public static readonly byte[] onlyValidIdentifier = new byte[] { 0xAB, 0x4B, 0x54, 0x58, 0x20, 0x31, 0x31, 0xBB, 0x0D, 0x0A, 0x1A, 0x0A };

		/// <summary>
		/// Expected Endian value
		/// </summary>
		public static readonly uint expectedEndianValue = 0x04030201;

		/// <summary>
		/// Other valid Endian value
		/// </summary>
		public static readonly uint otherValidEndianValue = 0x01020304;

		/// <summary>
		/// Big endian value as bytes
		/// </summary>
		/// <value></value>
		public static readonly byte[] bigEndianAsBytes = new byte[] { 0x04, 0x03, 0x02, 0x01 };

		/// <summary>
		/// Little endian value as bytes
		/// </summary>
		/// <value></value>
		public static readonly byte[] littleEndianAsBytes = new byte[] { 0x01, 0x02, 0x03, 0x04 };

		/// <summary>
		/// NUL is used to terminate UTF-8 strings, and padd metadata
		/// </summary>
		public static readonly byte nulByte = 0;

		/// <summary>
		/// Sizeof(uint)
		/// </summary>
		/// <returns></returns>
		public static readonly int sizeOfUint = sizeof(uint);

		/// <summary>
		/// Get length of UTF-8 string as bytes
		/// </summary>
		/// <param name="inputString">Input string</param>
		/// <returns>Length in bytes</returns>
		public static uint GetLengthOfUtf8StringAsBytes(string inputString)
		{
			return (uint)Encoding.UTF8.GetByteCount(inputString);
		}

		/// <summary>
		/// Get UTF-8 string as byte array
		/// </summary>
		/// <param name="inputString">Input string</param>
		/// <returns>Byte array</returns>
		public static byte[] GetUtf8StringAsBytes(string inputString)
		{
			return Encoding.UTF8.GetBytes(inputString);
		}

		/// <summary>
		/// GlType to size
		/// </summary>
		public static readonly Dictionary<GlDataType, uint> GlTypeToSize = new Dictionary<GlDataType, uint>()
		{
			{ GlDataType.Compressed, 1 },
			{ GlDataType.GL_BYTE, 1 },
			{ GlDataType.GL_UNSIGNED_BYTE, 1 },

			{ GlDataType.GL_SHORT, 2 },
			{ GlDataType.GL_UNSIGNED_SHORT, 2 },

			{ GlDataType.GL_FLOAT, 4 },
			{ GlDataType.GL_FIXED, 4 }
		};
	}

	/// <summary>
	/// Gl Data type
	/// </summary>
	public enum GlDataType : uint
	{
		/// <summary>
		/// Compressed
		/// </summary>
		Compressed = 0,

		/// <summary>
		/// GL_BYTE
		/// </summary>
		GL_BYTE = 0x1400,

		/// <summary>
		/// GL_UNSIGNED_BYTE
		/// </summary>
		GL_UNSIGNED_BYTE = 0x1401,

		/// <summary>
		/// GL_SHORT
		/// </summary>
		GL_SHORT = 0x1402,

		/// <summary>
		/// GL_UNSIGNED_SHORT
		/// </summary>
		GL_UNSIGNED_SHORT = 0x1403,

		/// <summary>
		/// GL_FLOAT
		/// </summary>
		GL_FLOAT = 0x1406,

		/// <summary>
		/// GL_FIXED
		/// </summary>
		GL_FIXED = 0x140C,

		/// <summary>
		/// Custom value for situation where parser cannot identify format
		/// </summary>
		NotKnown = 0xFFFF,
	}

	/// <summary>
	/// GlPixelFormat
	/// </summary>
	public enum GlPixelFormat : uint
	{
		/// <summary>
		/// GL_COLOR_INDEX
		/// </summary>
		GL_COLOR_INDEX = 0x1900,

		/// <summary>
		/// GL_STENCIL_INDEX
		/// </summary>
		GL_STENCIL_INDEX = 0x1901,

		/// <summary>
		/// GL_DEPTH_COMPONENT
		/// </summary>
		GL_DEPTH_COMPONENT = 0x1902,

		/// <summary>
		/// GL_RED
		/// </summary>
		GL_RED = 0x1903,

		/// <summary>
		/// GL_GREEN
		/// </summary>
		GL_GREEN = 0x1904,

		/// <summary>
		/// GL_BLUE
		/// </summary>
		GL_BLUE = 0x1905,

		/// <summary>
		/// GL_ALPHA
		/// </summary>
		GL_ALPHA = 0x1906,

		/// <summary>
		/// GL_RGB
		/// </summary>
		GL_RGB = 0x1907,

		/// <summary>
		/// GL_RGBA
		/// </summary>
		GL_RGBA = 0x1908,

		/// <summary>
		/// GL_LUMINANCE
		/// </summary>
		GL_LUMINANCE = 0x1909,

		/// <summary>
		/// GL_LUMINANCE_ALPHA
		/// </summary>
		GL_LUMINANCE_ALPHA = 0x190A,

		/// <summary>
		/// Custom value for situation where parser cannot identify format
		/// </summary>
		NotKnown = 0xFFFF,
	}

	/// <summary>
	/// GlInternalFormat
	/// </summary>
	public enum GlInternalFormat : uint
	{
		/// <summary>
		/// RGB8
		/// </summary>
		GL_RGB8_OES = 0x8051,

		/// <summary>
		/// RGBA8
		/// </summary>
		GL_RGBA8_OES = 0x8058,

		/// <summary>
		/// RGB S3TC DXT1
		/// </summary>
		GL_COMPRESSED_RGB_S3TC_DXT1_EXT = 0x83F0,

		/// <summary>
		/// RGBA S3TC DXT1
		/// </summary>
		GL_COMPRESSED_RGBA_S3TC_DXT1_EXT = 0x83F1,

		/// <summary>
		/// RGBA S3TC DXT3
		/// </summary>
		GL_COMPRESSED_RGBA_S3TC_DXT3_EXT = 0x83F2,

		/// <summary>
		/// RGBA S3TC DXT5
		/// </summary>
		GL_COMPRESSED_RGBA_S3TC_DXT5_EXT = 0x83F3,

		/// <summary>
		/// ETC1 RGB8
		/// </summary>
		GL_ETC1_RGB8_OES = 0x8D64,

		/// <summary>
		/// R11 EAC
		/// </summary>
		GL_COMPRESSED_R11_EAC = 0x9270,

		/// <summary>
		/// SIGNED R11 EAC
		/// </summary>
		GL_COMPRESSED_SIGNED_R11_EAC = 0x9271,

		/// <summary>
		/// RG11 EAC
		/// </summary>
		GL_COMPRESSED_RG11_EAC = 0x9272,

		/// <summary>
		/// SIGNED RG11 EAC
		/// </summary>
		GL_COMPRESSED_SIGNED_RG11_EAC = 0x9273,

		/// <summary>
		/// RGB8 ETC2
		/// </summary>
		GL_COMPRESSED_RGB8_ETC2 = 0x9274,

		/// <summary>
		/// SRGB8 ETC2
		/// </summary>
		GL_COMPRESSED_SRGB8_ETC2 = 0x9275,

		/// <summary>
		/// RGB8 PUNCHTHROUGH ALPHA1 ETC2
		/// </summary>
		GL_COMPRESSED_RGB8_PUNCHTHROUGH_ALPHA1_ETC2 = 0x9276,

		/// <summary>
		/// SRGB8 PUNCHTHROUGH ALPHA1 ETC2
		/// </summary>
		GL_COMPRESSED_SRGB8_PUNCHTHROUGH_ALPHA1_ETC2 = 0x9277,

		/// <summary>
		/// RGBA8 ETC2 EAC
		/// </summary>
		GL_COMPRESSED_RGBA8_ETC2_EAC = 0x9278,

		/// <summary>
		/// SRGB8 ALPHA8 ETC2 EAC
		/// </summary>
		GL_COMPRESSED_SRGB8_ALPHA8_ETC2_EAC = 0x9279,

		/// <summary>
		/// RGBA ASTC 4x4 KHR
		/// </summary>
		GL_COMPRESSED_RGBA_ASTC_4x4_KHR = 0x93B0,

		/// <summary>
		/// RGBA ASTC 5x4 KHR
		/// </summary>
		GL_COMPRESSED_RGBA_ASTC_5x4_KHR = 0x93B1,

		/// <summary>
		/// RGBA ASTC 5x5 KHR
		/// </summary>
		GL_COMPRESSED_RGBA_ASTC_5x5_KHR = 0x93B2,

		/// <summary>
		/// RGBA ASTC 6x5 KHR
		/// </summary>
		GL_COMPRESSED_RGBA_ASTC_6x5_KHR = 0x93B3,

		/// <summary>
		/// RGBA ASTC 6x6 KHR
		/// </summary>
		GL_COMPRESSED_RGBA_ASTC_6x6_KHR = 0x93B4,

		/// <summary>
		/// RGBA ASTC 8x5 KHR
		/// </summary>
		GL_COMPRESSED_RGBA_ASTC_8x5_KHR = 0x93B5,

		/// <summary>
		/// RGBA ASTC 8x6 KHR
		/// </summary>
		GL_COMPRESSED_RGBA_ASTC_8x6_KHR = 0x93B6,

		/// <summary>
		/// RGBA ASTC 8x8 KHR
		/// </summary>
		GL_COMPRESSED_RGBA_ASTC_8x8_KHR = 0x93B7,

		/// <summary>
		/// RGBA ASTC 10x5 KHR
		/// </summary>
		GL_COMPRESSED_RGBA_ASTC_10x5_KHR = 0x93B8,

		/// <summary>
		/// RGBA ASTC 10x6 KHR
		/// </summary>
		GL_COMPRESSED_RGBA_ASTC_10x6_KHR = 0x93B9,

		/// <summary>
		/// RGBA ASTC 10x8 KHR
		/// </summary>
		GL_COMPRESSED_RGBA_ASTC_10x8_KHR = 0x93BA,

		/// <summary>
		/// RGBA ASTC 10x10 KHR
		/// </summary>
		GL_COMPRESSED_RGBA_ASTC_10x10_KHR = 0x93BB,

		/// <summary>
		/// RGBA ASTC 12x10 KHR
		/// </summary>
		GL_COMPRESSED_RGBA_ASTC_12x10_KHR = 0x93BC,

		/// <summary>
		/// RGBA ASTC 12x12 KHR
		/// </summary>
		GL_COMPRESSED_RGBA_ASTC_12x12_KHR = 0x93BD,


		/// <summary>
		/// SRGB8 ALPHA8 ASTC 4x4 KHR
		/// </summary>
		GL_COMPRESSED_SRGB8_ALPHA8_ASTC_4x4_KHR = 0x93D0,

		/// <summary>
		/// SRGB8 ALPHA8 ASTC 5x4 KHR
		/// </summary>
		GL_COMPRESSED_SRGB8_ALPHA8_ASTC_5x4_KHR = 0x93D1,

		/// <summary>
		/// SRGB8 ALPHA8 ASTC 5x5 KHR
		/// </summary>
		GL_COMPRESSED_SRGB8_ALPHA8_ASTC_5x5_KHR = 0x93D2,

		/// <summary>
		/// SRGB8 ALPHA8 ASTC 6x5 KHR
		/// </summary>
		GL_COMPRESSED_SRGB8_ALPHA8_ASTC_6x5_KHR = 0x93D3,

		/// <summary>
		/// SRGB8 ALPHA8 ASTC 6x6 KHR
		/// </summary>
		GL_COMPRESSED_SRGB8_ALPHA8_ASTC_6x6_KHR = 0x93D4,

		/// <summary>
		/// SRGB8 ALPHA8 ASTC 8x5 KHR
		/// </summary>
		GL_COMPRESSED_SRGB8_ALPHA8_ASTC_8x5_KHR = 0x93D5,

		/// <summary>
		/// SRGB8 ALPHA8 ASTC 8x6 KHR
		/// </summary>
		GL_COMPRESSED_SRGB8_ALPHA8_ASTC_8x6_KHR = 0x93D6,

		/// <summary>
		/// SRGB8 ALPHA8 ASTC 8x8 KHR
		/// </summary>
		GL_COMPRESSED_SRGB8_ALPHA8_ASTC_8x8_KHR = 0x93D7,

		/// <summary>
		/// SRGB8 ALPHA8 ASTC 10x5 KHR
		/// </summary>
		GL_COMPRESSED_SRGB8_ALPHA8_ASTC_10x5_KHR = 0x93D8,

		/// <summary>
		/// SRGB8 ALPHA8 ASTC 10x6 KHR
		/// </summary>
		GL_COMPRESSED_SRGB8_ALPHA8_ASTC_10x6_KHR = 0x93D9,

		/// <summary>
		/// SRGB8 ALPHA8 ASTC 10x8 KHR
		/// </summary>
		GL_COMPRESSED_SRGB8_ALPHA8_ASTC_10x8_KHR = 0x93DA,

		/// <summary>
		/// SRGB8 ALPHA8 ASTC 10x10 KHR
		/// </summary>
		GL_COMPRESSED_SRGB8_ALPHA8_ASTC_10x10_KHR = 0x93DB,

		/// <summary>
		/// SRGB8 ALPHA8 ASTC 12x10 KHR
		/// </summary>
		GL_COMPRESSED_SRGB8_ALPHA8_ASTC_12x10_KHR = 0x93DC,

		/// <summary>
		/// SRGB8 ALPHA8 ASTC 12x12 KHR
		/// </summary>
		GL_COMPRESSED_SRGB8_ALPHA8_ASTC_12x12_KHR = 0x93DD,

		/// <summary>
		/// Custom value for situation where parser cannot identify format
		/// </summary>
		NotKnown = 0xFFFF,
	}

	/// <summary>
	/// Texture type (basic)
	/// </summary>
	public enum TextureTypeBasic
	{
		/// <summary>
		/// Basic 2d no mip maps
		/// </summary>
		Basic2DNoMipmaps = 1,

		/// <summary>
		/// Basic 2d with mip maps
		/// </summary>
		Basic2DWithMipmaps = 2,

		/// <summary>
		/// Basic 3d no mip maps
		/// </summary>
		Basic3DNoMipmaps = 3,

		/// <summary>
		/// Basic 3d with mip maps
		/// </summary>
		Basic3DWithMipmaps = 4,

		/// <summary>
		/// Basic 1d no mip maps
		/// </summary>
		Basic1DNoMipmaps = 5,

		/// <summary>
		/// Basic 1d with mip maps
		/// </summary>
		Basic1DWithMipmaps = 6,
	}
	
}