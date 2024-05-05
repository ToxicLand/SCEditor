using System;
using System.IO;

namespace KtxSharp
{
	/// <summary>
	/// Write Ktx output static class
	/// </summary>
	public static class KtxWriter
	{
		/// <summary>
		/// Should writer override endianness
		/// </summary>
		public enum OverrideEndianness
		{
			/// <summary>
			/// Keep same endianness as input had
			/// </summary>
			KeepSame = 0,

			/// <summary>
			/// Write little endian
			/// </summary>
			WriteLittleEndian,

			/// <summary>
			/// Write big endian
			/// </summary>
			WriteBigEndian
		}

		/// <summary>
		/// Write KtxStructure to output stream
		/// </summary>
		/// <param name="structure">KtxStructure</param>
		/// <param name="output">Output stream</param>
		/// <param name="overrideEndianness">Override endianness (optional)</param>
		public static void WriteTo(KtxStructure structure, Stream output, OverrideEndianness overrideEndianness = OverrideEndianness.KeepSame)
		{
			if (structure == null)
			{
				throw new NullReferenceException("Structure cannot be null");
			}

			if (output == null)
			{
				throw new NullReferenceException("Output stream cannot be null");
			}
			else if (!output.CanWrite)
			{
				throw new ArgumentException("Output stream must be writable");
			}

			bool writeLittleEndian = true;
			if (overrideEndianness == OverrideEndianness.KeepSame)
			{
				writeLittleEndian = structure.header.isInputLittleEndian;
			}
			else if (overrideEndianness == OverrideEndianness.WriteBigEndian)
			{
				writeLittleEndian = false;
			}

			output.Write(Common.onlyValidIdentifier, 0, Common.onlyValidIdentifier.Length);
			structure.header.WriteTo(output, writeLittleEndian);
			structure.textureData.WriteTo(output, writeLittleEndian);
		}
	}
}