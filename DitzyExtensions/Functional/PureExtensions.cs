using System;
using System.Collections.Generic;
using DitzyExtensions.Collection;

namespace DitzyExtensions.Functional {
	public static class PureExtensions {
		public static ACC Reduce<T, ACC>(this IEnumerable<T> source, Func<ACC, T, ACC> reducer, ACC initial) {
			var acc = initial;
			source.ForEach(value => { acc = reducer.Invoke(acc, value); });
			return acc;
		}

#if !NET48
		public static IEnumerable<int> Sequence(this Range range) {
			for (int i = range.Start.Value; i < range.End.Value; i++) {
				yield return i;
			}
		}
#endif

		public static Func<Nothing> AsFunc(this Action action) =>
			() => {
				action();
				return Nothing.Value;
			};

		public struct Nothing {
			public static readonly Nothing Value = new Nothing();
		}
	}
}
