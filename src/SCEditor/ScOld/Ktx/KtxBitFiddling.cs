// Bit fiddling

namespace KtxSharp
{
	/// <summary>
	/// Static class for bit fiddling operations
	/// </summary>
	public static class KtxBitFiddling
	{
		/// <summary>
		/// Swap endianness of uint
		/// </summary>
		/// <remark>https://stackoverflow.com/a/19560621/4886769</remark>
		/// <param name="inputValue">Input value</param>
		/// <returns>Endianness swapped value</returns>
		public static uint SwapEndian(uint inputValue)
		{
			// swap adjacent 16-bit blocks
			inputValue = (inputValue >> 16) | (inputValue << 16);
			// swap adjacent 8-bit blocks
			return ((inputValue & 0xFF00FF00) >> 8) | ((inputValue & 0x00FF00FF) << 8);
		}
	}
}