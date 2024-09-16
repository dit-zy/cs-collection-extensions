using System.Collections.Generic;

namespace DitzyExtensions.Functional {
	public interface IAccumulatedResults<out T, out E> {
		T Value { get; }
		IEnumerable<E> Errors { get; }
		bool HasValue { get; }
	}
}
