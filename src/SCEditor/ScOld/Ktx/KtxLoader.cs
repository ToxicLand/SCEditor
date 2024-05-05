using System;
using System.IO;

namespace KtxSharp
{
	/// <summary>
	/// Load Ktx input static class
	/// </summary>
	public static class KtxLoader
	{
		
		/// <summary>
		/// Check if input is valid
		/// </summary>
		/// <param name="stream">Stream to check</param>
		/// <returns>Tuple that tells if input is valid, and possible error message</returns>
		public static (bool isValid, string possibleError) CheckIfInputIsValid(Stream stream)
		{
			// Currently only header and metadata are validated properly, so texture data can still contain invalid values
			(bool isStreamValid, string possibleStreamError) = KtxValidators.GenericStreamValidation(stream);
			if (!isStreamValid)
			{
				return (isValid: false, possibleError: possibleStreamError);
			}

			// We have to duplicate the data, since we have to both validate it and keep it for texture data validation step
			long streamPos = stream.Position;

			(bool isIdentifierValid, string possibleIdentifierError) = KtxValidators.ValidateIdentifier(stream);
			if (!isIdentifierValid)
			{
				return (isValid: false, possibleError: possibleIdentifierError);
			}

			(bool isHeaderValid, string possibleHeaderError) = KtxValidators.ValidateHeaderData(stream);
			if (!isHeaderValid)
			{
				return (isValid: false, possibleError: possibleHeaderError);
			}

			stream.Position = streamPos;
			KtxHeader tempHeader = new KtxHeader(stream);

			(bool isTextureDataValid, string possibleTextureDataError) = KtxValidators.ValidateTextureData(stream, tempHeader, (uint)(stream.Length - stream.Position));

			return (isValid: isTextureDataValid, possibleError: possibleTextureDataError);
		}

		/// <summary>
		/// Load KtxStructure from stream
		/// </summary>
		/// <param name="stream">Stream to read</param>
		/// <returns>KtxStructure</returns>
		public static KtxStructure LoadInput(Stream stream)
		{
			// First we read the header
			KtxHeader header = new KtxHeader(stream);
			// Then texture data
			KtxTextureData textureData = new KtxTextureData(header, stream);
			// And combine those to one structure
			return new KtxStructure(header, textureData);
		}
	}
}
