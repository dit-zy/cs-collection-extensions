using System.Collections.Generic;

namespace DitzyExtensions.Functional {
	public interface IAccumulatedResults<out T, out E> {
		IEnumerable<T> Values { get; }
		IEnumerable<E> Errors { get; }
	}
}
