// Class for storing texture data
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace KtxSharp
{
	/// <summary>
	/// Texture data class
	/// </summary>
	public sealed class KtxTextureData
	{
		/// <summary>
		/// How many bytes of texture data there is
		/// </summary>
		public readonly uint totalTextureDataLength;

		/// <summary>
		/// Texture data as raw bytes
		/// </summary>
		public readonly byte[] textureDataAsRawBytes;

		/// <summary>
		/// Texture type (basic)
		/// </summary>
		public readonly TextureTypeBasic textureType;

		/// <summary>
		/// Texture data for each mip map level
		/// </summary>
		public List<byte[]> textureDataOfMipmapLevel;

		/// <summary>
		/// Constructor for texture data
		/// </summary>
		/// <param name="textureDatas">List of 2d texture bytes, one for each mipmap level</param>
		public KtxTextureData(List<byte[]> textureDatas)
		{
			if (textureDatas == null || textureDatas.Count < 1)
			{
				throw new ArgumentException("Texturedatas must contain something");
			}

			this.textureDataOfMipmapLevel = textureDatas;

			this.textureType = this.textureDataOfMipmapLevel.Count > 1 ? TextureTypeBasic.Basic2DWithMipmaps : TextureTypeBasic.Basic2DNoMipmaps;
		}

		/// <summary>
		/// Constructor for texture data
		/// </summary>
		/// <param name="header">Header</param>
		/// <param name="stream">Stream for reading</param>
		public KtxTextureData(KtxHeader header, Stream stream)
		{
			//this.totalTextureDataLength = (uint)stream.Length;

			// Try to figure out texture type basic
			bool containsMipmaps = header.numberOfMipmapLevels > 1;

			if (header.numberOfArrayElements == 0 || header.numberOfArrayElements == 1)
			{
				// Is NOT texture array

				if (header.numberOfFaces == 0 || header.numberOfFaces == 1)
				{
					// Is NOT cube texture

					if (header.pixelDepth == 0 || header.pixelDepth == 1)
					{
						// Is not 3D texture

						if (header.pixelHeight == 0 || header.pixelHeight == 1)
						{
							// 1D texture
							this.textureType = containsMipmaps ? TextureTypeBasic.Basic1DWithMipmaps : TextureTypeBasic.Basic1DNoMipmaps;
						}
						else
						{
							// 2D texture
							this.textureType = containsMipmaps ? TextureTypeBasic.Basic2DWithMipmaps : TextureTypeBasic.Basic2DNoMipmaps;
						}
					}
					else
					{
						// Is 3D texture
						this.textureType = containsMipmaps ? TextureTypeBasic.Basic3DWithMipmaps : TextureTypeBasic.Basic3DNoMipmaps;
					}
				}
				else
				{
					// Is cube texture
				}
			}
			else
			{
				// Is Texture array
			}

			uint mipmapLevels = header.numberOfMipmapLevels;
			if (mipmapLevels == 0)
			{
				mipmapLevels = 1;
			}

			// Since we know how many mipmap levels there are, allocate the capacity
			this.textureDataOfMipmapLevel = new List<byte[]>((int)mipmapLevels);

			// Check if length reads should be endian swapped
			bool shouldSwapEndianness = (header.endiannessValue != Common.expectedEndianValue);

			using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true))
			{
				for (int i = 0; i < mipmapLevels; i++)
				{
					uint amountOfDataInThisMipmapLevel = shouldSwapEndianness ? KtxBitFiddling.SwapEndian(reader.ReadUInt32()) : reader.ReadUInt32();
					this.textureDataOfMipmapLevel.Add(reader.ReadBytes((int)amountOfDataInThisMipmapLevel));

					// Skip possible padding bytes
					while (amountOfDataInThisMipmapLevel % 4 != 0)
					{
						amountOfDataInThisMipmapLevel++;
						// Read but ignore values
						reader.ReadByte();
					}
				}
			}			
		}

		/// <summary>
		/// Write content to stream. Leaves stream open
		/// </summary>
		/// <param name="output">Output stream</param>
		/// <param name="writeLittleEndian">Write little endian output (default enabled)</param>
		public void WriteTo(Stream output, bool writeLittleEndian = true)
		{
			using (BinaryWriter writer = new BinaryWriter(output, Encoding.UTF8, leaveOpen: true))
			{
				Action<uint> writeUint = writer.Write;
				if (!writeLittleEndian)
				{
					writeUint = (uint u) => WriteUintAsBigEndian(writer, u);
				}

				GenericWrite(this.textureDataOfMipmapLevel, writeUint, writer.Write);
			}
		}

		private static void GenericWrite(List<byte[]> textureDataOfMipmapLevel, Action<uint> writeUint, Action<byte[]> writeByteArray)
		{
			foreach (byte[] level in textureDataOfMipmapLevel)
			{
				writeUint((uint)level.Length);
				writeByteArray(level);
			}
		}

		private static void WriteUintAsBigEndian(BinaryWriter writer, uint value)
		{
			writer.Write(KtxBitFiddling.SwapEndian(value));
		}
	}
}