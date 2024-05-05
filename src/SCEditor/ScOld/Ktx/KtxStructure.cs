

namespace KtxSharp
{
	/// <summary>
	/// KtxStructure that has header and texture data
	/// </summary>
	public sealed class KtxStructure
	{
		/// <summary>
		/// Header
		/// </summary>
		public readonly KtxHeader header;
		
		/// <summary>
		/// Texture data
		/// </summary>
		public readonly KtxTextureData textureData;

		/// <summary>
		/// Constuctor for KtxStructure
		/// </summary>
		/// <param name="ktxHeader">Header</param>
		/// <param name="texData">Texture data</param>
		public KtxStructure(KtxHeader ktxHeader, KtxTextureData texData)
		{
			this.header = ktxHeader;
			this.textureData = texData;
		}
	}
}