using System;
using System.Collections.Generic;
using DitzyExtensions.Collection;

namespace DitzyExtensions.Functional {
	public class AccumulatedResults<T, E> : IAccumulatedResults<T, E> {
#if NET6_0_OR_GREATER
		private readonly T? _value;
#else
		private readonly T _value;
#endif
		private readonly IList<E> _errors;

		public T Value {
			get {
				if (!HasValue) {
					throw new AccumulationHasNoValueException<E>(_errors);
				}
#if NET6_0_OR_GREATER
				return _value!;
#else
				return _value;
#endif
			}
		}

		public IEnumerable<E> Errors => _errors;

		public bool HasValue { get; }

		internal AccumulatedResults(T value) {
			_value = value;
			_errors = Array.Empty<E>();
			HasValue = true;
		}

		internal AccumulatedResults(IEnumerable<E> errors) {
			_value = default;
			_errors = errors.AsList();
			HasValue = false;
		}

		internal AccumulatedResults(T value, IEnumerable<E> errors) {
			_value = value;
			_errors = errors.AsList();
			HasValue = true;
		}

		public static implicit operator AccumulatedResults<T, E>(T value) =>
			AccumulatedResults.From<T, E>(value);

		public static implicit operator AccumulatedResults<T, E>(E error) =>
			AccumulatedResults.From<T, E>(error);
	}

	public static class AccumulatedResults {
		public static AccumulatedResults<T, E> From<T, E>(T value) =>
			new AccumulatedResults<T, E>(value);

		public static AccumulatedResults<T, E> From<T, E>(params E[] errors) =>
			new AccumulatedResults<T, E>(errors);
		public static AccumulatedResults<T, E> From<T, E>(IEnumerable<E> errors) =>
			new AccumulatedResults<T, E>(errors);

		public static AccumulatedResults<T, E> From<T, E>(T value, params E[] errors) =>
			new AccumulatedResults<T, E>(value, errors);
		public static AccumulatedResults<T, E> From<T, E>(T value, IEnumerable<E> errors) =>
			new AccumulatedResults<T, E>(value, errors);
	}
}
