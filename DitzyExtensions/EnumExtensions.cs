using System;

namespace DitzyExtensions {
	public static class EnumExtensions {
		public static T[] GetEnumValues<T>() where T : struct, Enum =>
#if NET7_0_OR_GREATER
			Enum.GetValuesAsUnderlyingType<T>() as T[] ?? Array.Empty<T>();
#else
			Enum.GetValues(typeof(T)) as T[] ?? Array.Empty<T>();
#endif
	}
}
