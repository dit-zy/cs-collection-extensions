﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
#if !NET48
using System.Collections.Immutable;
#endif

namespace DitzyExtensions.Collection {
	public static class DictExtensions {
		public static IDictionary<K, V> AsDict<K, V>(this IEnumerable<KeyValuePair<K, V>> source)
#if NET48
			=>
#else
			where K : notnull =>
#endif
			source.Select(entry => (entry.Key, entry.Value)).AsDict();

		public static IDictionary<K, V> AsDict<K, V>(this IEnumerable<(K key, V value)> source)
#if NET48
			=>
				new ReadOnlyDictionary<K, V>(
					source
#else
			where K : notnull =>
			source
#endif
				.GroupBy(entry => entry.key)
				.Select(grouping => grouping.Last())
#if NET48
						.ToDictionary(entry => entry.key, entry => entry.value)
				);
#else
				.ToImmutableDictionary(entry => entry.key, entry => entry.value);
#endif

		public static IDictionary<K, V> AsMutableDict<K, V>(this IEnumerable<KeyValuePair<K, V>> source)
#if NET48
			=>
#else
			where K : notnull =>
#endif
			source.Select(entry => (entry.Key, entry.Value)).AsMutableDict();

		public static IDictionary<K, V> AsMutableDict<K, V>(this IEnumerable<(K, V)> source)
#if NET48
			=>
#else
			where K : notnull =>
#endif
			source
				.AsDict()
				.ToDictionary(entry => entry.Key, entry => entry.Value);

#if NET6_0_OR_GREATER
		public static V GetValueOrDefault<K, V>(this IDictionary<K, V> source, K key) =>
			source.GetValueOrDefault(key, default!);
#endif

#if NET6_0_OR_GREATER
		public static V GetValueOrDefault<K, V>(this IDictionary<K, V> source, K key, V defaultValue) =>
			source.TryGetValue(key, out var value) ? value : defaultValue;
#endif

		public static IDictionary<K, V> With<K, V>(this IDictionary<K, V> source, params (K, V)[] entries)
#if NET48
		{
#else
			where K : notnull {
#endif
			var dict = source.IsReadOnly ? source.AsMutableDict() : source;
			entries.ForEach(entry => dict[entry.Item1] = entry.Item2);
			return source.IsReadOnly ? dict.AsDict() : source;
		}

		public static IDictionary<K, V> Without<K, V>(this IDictionary<K, V> source, params K[] keys)
#if NET48
		{
#else
			where K : notnull {
#endif
			var dict = source.IsReadOnly ? source.AsMutableDict() : source;
			keys.ForEach(key => dict.Remove(key));
			return source.IsReadOnly ? dict.AsDict() : dict;
		}

		public static IDictionary<V, K> Flip<K, V>(this IDictionary<K, V> source)
#if NET48
			=>
#else
			where V : notnull =>
#endif
			source
				.Select(entry => (entry.Value, entry.Key))
				.AsDict();

		public static IDictionary<K, V> Update<K, V>(
			this IDictionary<K, V> source,
			IEnumerable<(K key, V value)> updateEntries
#if NET48
		) {
#else
		) where K : notnull {
#endif
			if (source.IsReadOnly)
				return source
					.AsPairs()
					.Concat(updateEntries)
					.AsDict();

			updateEntries.ForEach(entry => source[entry.key] = entry.value);
			return source;
		}

		public static IDictionary<K, V> UseToUpdate<K, V>(
			this IEnumerable<(K, V)> updateEntries,
			IDictionary<K, V> target
#if NET48
		) =>
#else
		) where K : notnull =>
#endif
			target.Update(updateEntries);

		public static IDictionary<K, V> VerifyEnumDictionary<K, V>(this IDictionary<K, V> enumDict)
			where K : struct, Enum {
#if NET7_0_OR_GREATER
			var allEnumsAreInDict = (Enum.GetValuesAsUnderlyingType<K>() as K[])!.All(enumDict.ContainsKey);
#else
			var allEnumsAreInDict = (Enum.GetValues(typeof(K)) as K[] ?? Array.Empty<K>()).All(enumDict.ContainsKey);
#endif
			if (!allEnumsAreInDict) {
				throw new Exception($"All values of enum [{typeof(K).Name}] must be in the dictionary.");
			}

			return enumDict.AsDict();
		}
	}
}
