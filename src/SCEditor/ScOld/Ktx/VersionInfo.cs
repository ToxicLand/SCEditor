using System.Reflection;

namespace KtxSharp
{
	/// <summary>
	/// Version info static class
	/// </summary>
	public static class VersionInfo
	{
		/// <summary>
		/// Get version
		/// </summary>
		/// <returns>Version string</returns>
		public static string GetVersion()
		{
			var version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
			return version;
		}
	}
}