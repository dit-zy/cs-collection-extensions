using System;
using System.Collections.Generic;
using System.Configuration;
using DitzyExtensions.Collection;

namespace DitzyExtensions.Functional {
	public static class PureExtensions {
		public static ACC Reduce<T, ACC>(this IEnumerable<T> source, Func<ACC, T, ACC> reducer, ACC initial) {
			var acc = initial;
			source.ForEach(value => { acc = reducer.Invoke(acc, value); });
			return acc;
		}

#if !N48_S2
		public static IEnumerable<int> Sequence(this Range range) {
			for (int i = range.Start.Value; i < range.End.Value; i++) {
				yield return i;
			}
		}
#endif
		
#if N48_S2
		public static IEnumerable<int> SequenceTo(this int startInclusive, int endExclusive, int step = 1) {
			if (step == 0) throw new ArgumentException("step cannot be 0.");
			if (Math.Sign(endExclusive - startInclusive) != Math.Sign(step))
				throw new ArgumentException(
					$"end [{endExclusive}] is not reachable from start [{startInclusive}] with step [{step}]."
				);
			
			for (int i = startInclusive; (0 < step && i < endExclusive) || (step < 0 && endExclusive < i); i += step) {
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
