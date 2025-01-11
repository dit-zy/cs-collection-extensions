using System;
using System.Collections;
using System.Collections.Generic;

namespace DitzyExtensions.Collection {
#if NET6_0_OR_GREATER
	public class MultiDict<K, V> : IEnumerable<(K Key, V Value)> where K : notnull {
#else
	public class MultiDict<K, V> : IEnumerable<(K Key, V Value)> {
#endif

		private readonly Dictionary<K, IList<V>> _contents = new Dictionary<K, IList<V>>();

		public ICollection<K> Keys => _contents.Keys;

		public ICollection<V> Values => _contents.Values.Flatten().AsList();

		public int Count { get; private set; } = 0;

		public IEnumerator<(K Key, V Value)> GetEnumerator() {
			foreach (var kv in _contents) {
				foreach (var value in kv.Value) {
					yield return (kv.Key, value);
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public void Add(K key, V value) {
			if (!_contents.TryGetValue(key, out var valueList)) {
				valueList = new List<V>();
				_contents.Add(key, valueList);
			}

			valueList.Add(value);
			Count++;
		}

		public void AddRange(K key, IEnumerable<V> values) {
			if (!_contents.TryGetValue(key, out var valueList)) {
				valueList = new List<V>();
				_contents.Add(key, valueList);
			}

			values.ForEach(
				value => {
					valueList.Add(value);
					Count++;
				}
			);
		}

		public void Clear() {
			_contents.Clear();
		}

		public bool ContainsKey(K key) => _contents.ContainsKey(key);

		public bool Remove(K key) {
			var result = _contents.Remove(key);
			if (result) Count--;
			return result;
		}

		public bool Remove(K key, V value) {
			var result = _contents.TryGetValue(key, out var values);
			if (!result) return false;
#if NET6_0_OR_GREATER
			result = values!.Remove(value);
#else
			result = values.Remove(value);
#endif
			if (result) Count--;
			return result;
		}

#if NET6_0_OR_GREATER
		public bool TryGetValues(K key, out ICollection<V>? values) {
#else
		public bool TryGetValues(K key, out ICollection<V> values) {
#endif
			var result = _contents.TryGetValue(key, out var rawValues);
			values = rawValues;
			return result;
		}

		public ICollection<V> this[K key] {
#if NET6_0_OR_GREATER
			get => TryGetValues(key, out var values) ? values! : Array.Empty<V>();
#else
			get => TryGetValues(key, out var values) ? values : Array.Empty<V>();
#endif
			set {
				_contents.Remove(key);
				AddRange(key, value);
			}
		}
	}
}
