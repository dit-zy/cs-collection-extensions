using System;
using System.Collections.Generic;
using System.Linq;
using DitzyExtensions.Functional;

namespace DitzyExtensions.Collection {
	public static class PairExtensions {
		public static IEnumerable<(K key, V value)> AsPairs<K, V>(this IDictionary<K, V> source) =>
			source.Select(entry => (entry.Key, entry.Value));

		public static IEnumerable<(A, B)> WithDistinctFirst<A, B>(this IEnumerable<(A, B)> source)
#if NET48
			=>
#else
			where A : notnull =>
#endif
				source
					.AsDict()
					.AsPairs();

		public static IEnumerable<(U, T)> Flip<T, U>(this IEnumerable<(T t, U u)> source) =>
			source.Select(entry => (entry.u, entry.t));

		public static (IEnumerable<T> ts, IEnumerable<U> us) Unzip<T, U>(this IEnumerable<(T t, U u)> source) =>
			source.Unzip((ts, us) => (ts, us));

		public static R Unzip<T, U, R>(
			this IEnumerable<(T t, U u)> source,
			Func<IEnumerable<T>, IEnumerable<U>, R> transform
		) {
			var (ts, us) = source
				.Reduce(
					(acc, pair) => {
						acc.ts.Add(pair.t);
						acc.us.Add(pair.u);
						return acc;
					},
					(ts: new List<T>(), us: new List<U>())
				);
			return transform.Invoke(ts, us);
		}
	}
}
