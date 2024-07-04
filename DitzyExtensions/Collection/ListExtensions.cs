using System;
using System.Collections.Generic;
using System.Linq;
#if !NET48
using System.Collections.Immutable;
#endif

namespace DitzyExtensions.Collection {
	public static class ListExtensions {
		public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action) =>
			source.ForEach((value, _) => action.Invoke(value));

		public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T, int> action) {
			var values = source as T[] ?? source.ToArray();
			for (var i = 0; i < values.Length; ++i) {
				action.Invoke(values[i], i);
			}
			return values;
		}

		public static IEnumerable<U> SelectWhere<T, U>(this IEnumerable<T> source, Func<T, (bool, U)> filteredSelector) =>
			source
				.Select(filteredSelector)
				.Where(result => result.Item1)
				.Select(result => result.Item2);

		public static IList<T> AsList<T>(this IEnumerable<T> source) =>
#if NET48
			source.ToList().AsReadOnly();
#else
			source.ToImmutableList();
#endif

		public static IList<T> AsMutableList<T>(this IEnumerable<T> source) =>
			source.ToList();

		public static IList<T> AsSingletonList<T>(this T value) =>
			new List<T>() { value }.AsList();

		public static bool IsEmpty<T>(this ICollection<T> source) =>
			source.Count == 0;

		public static bool IsNotEmpty<T>(this ICollection<T> source) =>
			0 < source.Count;
	}
}
