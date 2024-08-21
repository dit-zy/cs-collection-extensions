using System;
using System.Collections.Generic;
using DitzyExtensions.Collection;

namespace DitzyExtensions.Functional {
	public class AccumulatedResults<T, E> : IAccumulatedResults<T, E> {
		private readonly IList<T> _value;
		private readonly IList<E> _errors;

		public IEnumerable<T> Values => _value;
		public IEnumerable<E> Errors => _errors;

		internal AccumulatedResults(IEnumerable<T> value) {
			_value = value.AsList();
			_errors = Array.Empty<E>();
		}

		internal AccumulatedResults(IEnumerable<E> errors) {
			_value = Array.Empty<T>();
			_errors = errors.AsList();
		}

		internal AccumulatedResults(IEnumerable<T> value, IEnumerable<E> errors) {
			_value = value.AsList();
			_errors = errors.AsList();
		}

		public static implicit operator AccumulatedResults<T, E>(T value) =>
			AccumulatedResults.From<T, E>(value);

		public static implicit operator AccumulatedResults<T, E>(E error) =>
			AccumulatedResults.From<T, E>(error);
	}

	public static class AccumulatedResults {
		public static AccumulatedResults<T, E> From<T, E>(T value) =>
			new AccumulatedResults<T, E>(value.AsSingletonList());

		public static AccumulatedResults<T, E> From<T, E>(E error) =>
			new AccumulatedResults<T, E>(error.AsSingletonList());

		public static AccumulatedResults<T, E> From<T, E>(IEnumerable<T> values) =>
			new AccumulatedResults<T, E>(values);

		public static AccumulatedResults<T, E> From<T, E>(IEnumerable<E> errors) =>
			new AccumulatedResults<T, E>(errors);

		public static AccumulatedResults<T, E> From<T, E>(IEnumerable<T> values, IEnumerable<E> errors) =>
			new AccumulatedResults<T, E>(values, errors);
	}
}
