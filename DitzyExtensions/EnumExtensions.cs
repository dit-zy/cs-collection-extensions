using System;
using CSharpFunctionalExtensions;

namespace DitzyExtensions {
	public static class EnumExtensions {
		public static T[] GetEnumValues<T>() where T : struct, Enum =>
#if NET7_0_OR_GREATER
			Enum.GetValuesAsUnderlyingType<T>() as T[] ?? Array.Empty<T>();
#else
			Enum.GetValues(typeof(T)) as T[] ?? Array.Empty<T>();
#endif

		public static Result<T, string> AsEnum<T>(this string enumName) where T : struct =>
			Enum.TryParse(enumName, true, out T result)
				? Result.Success<T, string>(result)
				: $"";
	}
}
