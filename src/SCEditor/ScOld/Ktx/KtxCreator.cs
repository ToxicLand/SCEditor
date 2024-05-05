using System;
using System.Collections.Generic;

namespace KtxSharp
{
	/// <summary>
	/// Create a new KtxStructure
	/// </summary>
	public static class KtxCreator
	{
		/// <summary>
		/// Create new 2d KtxStructure from existing data
		/// </summary>
		/// <param name="glDataType">GlDataType</param>
		/// <param name="glPixelFormat">GlPixelFormat</param>
		/// <param name="glInternalFormat">GlInternalFormat</param>
		/// <param name="width">Width</param>
		/// <param name="height">Height</param>
		/// <param name="textureDatas">Texture datas</param>
		/// <param name="metadata">metadata</param>
		/// <returns>KtxStructure</returns>
		public static KtxStructure Create(GlDataType glDataType, GlPixelFormat glPixelFormat, GlInternalFormat glInternalFormat, uint width, uint height, List<byte[]> textureDatas, Dictionary<string, MetadataValue> metadata)
		{
			KtxHeader header = new KtxHeader(glDataType, glPixelFormat, glInternalFormat, width, height, (uint)textureDatas.Count, metadata);
			KtxTextureData textureData = new KtxTextureData(textureDatas);

			return new KtxStructure(header, textureData);
		}
	}
}