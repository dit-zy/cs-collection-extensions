using System.Collections.Generic;

namespace DitzyExtensions {
	public static class StringExtensions {
		public static string AsLower(this string str) =>
			str.ToLowerInvariant();

		public static string AsUpper(this string str) =>
			str.ToUpperInvariant();

#if NET6_0_OR_GREATER
		public static string Join(this IEnumerable<string> source, string? separator) =>
#else
		public static string Join(this IEnumerable<string> source, string separator) =>
#endif
			string.Join(separator, source);
	}
}
