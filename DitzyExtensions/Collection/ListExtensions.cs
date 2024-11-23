using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
#if !N48_S2
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
#if N48_S2
			source.ToList().AsReadOnly();
#else
			source.ToImmutableList();
#endif

		public static IList<T> AsMutableList<T>(this IEnumerable<T> source) =>
			source.ToList();

		public static IList<T> AsSingletonList<T>(this T value) =>
			new[] { value }.AsList();

		public static bool IsEmpty<T>(this ICollection<T> source) =>
			source.Count == 0;

		public static bool IsNotEmpty<T>(this ICollection<T> source) =>
			0 < source.Count;

		public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> source) =>
			source.SelectMany(x => x);

#if N48_S2
		public static Maybe<T> MinBy<T, U>(
			this IEnumerable<T> source,
			Func<T, int, U> keySelector
		) =>
			source.MinBy(keySelector, null);

		public static Maybe<T> MinBy<T, U>(
			this IEnumerable<T> source,
			Func<T, int, U> keySelector,
			IComparer<U> comparer
		) {
			if (comparer is null) {
				comparer = Comparer<U>.Default;
			}
			
			T minEntry = default;
			U minKey = default;
			var started = false;

			source.ForEach(
				(t, i) => {
					var newKey = keySelector(t, i);

					if (!started) {
						minEntry = t;
						minKey = newKey;
						started = true;
						return;
					}

					if (comparer.Compare(newKey, minKey) < 0) {
						minEntry = t;
						minKey = newKey;
					}
				}
			);

			return started ? minEntry : Maybe<T>.None;
		}
#endif
	}
}
