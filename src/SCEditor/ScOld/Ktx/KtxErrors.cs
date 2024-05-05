
namespace KtxSharp
{
	/// <summary>
	/// Generic error generator
	/// </summary>
	public static class ErrorGen
	{
		/// <summary>
		/// Modulo 4 error
		/// </summary>
		/// <param name="variableName">Variable Name</param>
		/// <param name="value">Value</param>
		/// <returns>Error message</returns>
		public static string Modulo4Error(string variableName, uint value)
		{
			return $"{variableName} value is {value}, but it should be modulo 4!";
		}
	}
}