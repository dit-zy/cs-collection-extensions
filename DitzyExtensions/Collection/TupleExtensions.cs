using System;
using System.Collections.Generic;
using System.Linq;
using DitzyExtensions.Functional;

namespace DitzyExtensions.Collection {
	public static class TupleExtensions {
		public static IEnumerable<T> SelectFirst<T, U>(this IEnumerable<(T t, U u)> source) =>
			source.Select(tuple => tuple.t);

		public static IEnumerable<T> SelectFirst<T, U, V>(this IEnumerable<(T t, U u, V v)> source) =>
			source.Select(tuple => tuple.t);

		public static IEnumerable<T> SelectFirst<T, U, V, W>(this IEnumerable<(T t, U u, V v, W w)> source) =>
			source.Select(tuple => tuple.t);

		public static IEnumerable<U> SelectSecond<T, U>(this IEnumerable<(T t, U u)> source) =>
			source.Select(tuple => tuple.u);

		public static IEnumerable<U> SelectSecond<T, U, V>(this IEnumerable<(T t, U u, V v)> source) =>
			source.Select(tuple => tuple.u);

		public static IEnumerable<U> SelectSecond<T, U, V, W>(this IEnumerable<(T t, U u, V v, W w)> source) =>
			source.Select(tuple => tuple.u);

		public static IEnumerable<V> SelectThird<T, U, V>(this IEnumerable<(T t, U u, V v)> source) =>
			source.Select(tuple => tuple.v);

		public static IEnumerable<V> SelectThird<T, U, V, W>(this IEnumerable<(T t, U u, V v, W w)> source) =>
			source.Select(tuple => tuple.v);

		public static IEnumerable<W> SelectFourth<T, U, V, W>(this IEnumerable<(T t, U u, V v, W w)> source) =>
			source.Select(tuple => tuple.w);

		public static IEnumerable<(K key, V value)> AsPairs<K, V>(this IDictionary<K, V> source) =>
			source.Select(entry => (entry.Key, entry.Value));

		public static IEnumerable<(A, B)> WithDistinctFirst<A, B>(this IEnumerable<(A, B)> source)
#if N48_S2
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

		public static (IEnumerable<T> ts, IEnumerable<U> us, IEnumerable<V> vs) Unzip<T, U, V>(
			this IEnumerable<(T t, U u, V v)> source
		) =>
			source.Unzip((ts, us, vs) => (ts, us, vs));

		public static (IEnumerable<T> ts, IEnumerable<U> us, IEnumerable<V> vs, IEnumerable<W> ws) Unzip<T, U, V, W>(
			this IEnumerable<(T t, U u, V v, W w)> source
		) =>
			source.Unzip((ts, us, vs, ws) => (ts, us, vs, ws));

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

		public static R Unzip<T, U, V, R>(
			this IEnumerable<(T t, U u, V v)> source,
			Func<IEnumerable<T>, IEnumerable<U>, IEnumerable<V>, R> transform
		) {
			var (ts, us, vs) = source
				.Reduce(
					(acc, pair) => {
						acc.ts.Add(pair.t);
						acc.us.Add(pair.u);
						acc.vs.Add(pair.v);
						return acc;
					},
					(ts: new List<T>(), us: new List<U>(), vs: new List<V>())
				);
			return transform.Invoke(ts, us, vs);
		}

		public static R Unzip<T, U, V, W, R>(
			this IEnumerable<(T t, U u, V v, W w)> source,
			Func<IEnumerable<T>, IEnumerable<U>, IEnumerable<V>, IEnumerable<W>, R> transform
		) {
			var (ts, us, vs, ws) = source
				.Reduce(
					(acc, pair) => {
						acc.ts.Add(pair.t);
						acc.us.Add(pair.u);
						acc.vs.Add(pair.v);
						acc.ws.Add(pair.w);
						return acc;
					},
					(ts: new List<T>(), us: new List<U>(), vs: new List<V>(), ws: new List<W>())
				);
			return transform.Invoke(ts, us, vs, ws);
		}
	}
}
