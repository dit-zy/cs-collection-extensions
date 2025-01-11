using System.Collections.Generic;

namespace DitzyExtensions.Functional {
	public interface IAccumulatedResults<out T, E> {
		T Value { get; }

		ICollection<E> Errors { get; }

		bool HasValue { get; }

		bool HasErrors { get; }
	}
}
