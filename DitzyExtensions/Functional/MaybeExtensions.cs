using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using DitzyExtensions.Collection;

namespace DitzyExtensions.Functional {
	public static class MaybeExtensions {
		public static IEnumerable<T> SelectMaybe<T>(this IEnumerable<Maybe<T>> source) =>
			source.SelectMaybe(value => value);

		public static IEnumerable<U> SelectMaybe<T, U>(this IEnumerable<T> source, Func<T, Maybe<U>> selector) =>
			source.SelectWhere(
					value => {
						var result = selector.Invoke(value);
						return (result.HasValue, result);
					}
				)
				.Select(result => result.Value);

		public static IEnumerable<Maybe<U>> SelectOverMaybe<T, U>(
			this IEnumerable<Maybe<T>> source,
			Func<T, U> transform
		) =>
			source.Select(maybeValue => maybeValue.Select(transform));

		public static IEnumerable<Maybe<U>> SelectManyOverMaybe<T, U>(
			this IEnumerable<Maybe<T>> source,
			Func<T, Maybe<U>> transform
		) =>
			source.Select(maybeValue => maybeValue.SelectMany(transform));

		public static Maybe<V> MaybeGet<K, V>(this IDictionary<K, V> source, K key) {
			if (source.TryGetValue(key, out var value)) {
				return value;
			}
			return Maybe.None;
		}
	}
}
