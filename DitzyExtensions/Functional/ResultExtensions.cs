using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using DitzyExtensions.Collection;

namespace DitzyExtensions.Functional {
	public static class ResultExtensions {
		public static AccumulatedResults<U, E> Select<T, U, E>(
			this AccumulatedResults<T, E> source,
			Func<T, U> selector
		) =>
			AccumulatedResults.From(source.Values.Select(selector), source.Errors);

		public static AccumulatedResults<U, E> SelectMany<T, U, E>(
			this IEnumerable<T> source,
			Func<T, AccumulatedResults<U, E>> selector
		) {
			return source.Select(selector)
				.Reduce(
					(acc, results) => {
						acc.values.AddRange(results.Values);
						acc.errors.AddRange(results.Errors);
						return acc;
					},
					(values: new List<U>(), errors: new List<E>())
				)
				.ToAccumulatedResult();
		}

		public static AccumulatedResults<U, E> SelectResults<T, U, E>(
			this AccumulatedResults<T, E> source,
			Func<T, Result<U, E>> selector
		) =>
			AccumulatedResults.From(
				source.Values.SelectResults(selector).Values,
				source.Errors.Concat(source.Values.SelectResults(selector).Errors)
			);

		public static AccumulatedResults<T, string> SelectResults<T>(this IEnumerable<Result<T>> source) =>
			source.SelectResults(result => result);

		public static AccumulatedResults<U, string> SelectResults<T, U>(
			this IEnumerable<T> source,
			Func<T, Result<U>> selector
		) =>
			source.SelectResults(
				value => selector(value).Match(
					Result.Success<U, string>,
					Result.Failure<U, string>
				)
			);

		public static AccumulatedResults<T, E> SelectResults<T, E>(this IEnumerable<Result<T, E>> source) =>
			source.SelectResults(result => result);

		public static AccumulatedResults<U, E> SelectResults<T, U, E>(
			this IEnumerable<T> source,
			Func<T, Result<U, E>> selector
		) =>
			source.BindResults(result => selector(result).Map(AccumulatedResults.From<U, E>));

		public static AccumulatedResults<U, E> BindResults<T, U, E>(
			this IEnumerable<T> source,
			Func<T, Result<AccumulatedResults<U, E>, E>> selector
		) =>
			source.Reduce(
					(acc, value) => {
						selector(value).Match(
							success => {
								acc.results.AddRange(success.Values);
								acc.errors.AddRange(success.Errors);
							},
							error => acc.errors.Add(error)
						);
						return acc;
					},
					(results: new List<U>(), errors: new List<E>())
				)
				.ToAccumulatedResult();

		public static AccumulatedResults<T, E> Concat<T, E>(
			this AccumulatedResults<T, E> source,
			AccumulatedResults<T, E> secondSource
		) =>
			source.Concat(secondSource, (ns, ms) => ns.Concat(ms));

		public static AccumulatedResults<V, E> Concat<T, U, V, E>(
			this AccumulatedResults<T, E> source,
			AccumulatedResults<U, E> secondSource,
			Func<IEnumerable<T>, IEnumerable<U>, IEnumerable<V>> combiner
		) =>
			AccumulatedResults.From(
				combiner(source.Values, secondSource.Values),
				source.Errors.Concat(secondSource.Errors)
			);

		public static AccumulatedResults<U, E> WithValues<T, U, E>(
			this AccumulatedResults<T, E> source,
			Func<IEnumerable<T>, IEnumerable<U>> valueModifier
		) =>
			AccumulatedResults.From(valueModifier(source.Values), source.Errors);

		public static AccumulatedResults<T, E> ForEachValue<T, E>(this AccumulatedResults<T, E> source, Action<T> action) {
			source.Values.ForEach(action);
			return source;
		}

		public static AccumulatedResults<T, E> ForEachError<T, E>(this AccumulatedResults<T, E> source, Action<E> action) {
			source.Errors.ForEach(action);
			return source;
		}

		public static (IEnumerable<T>, IEnumerable<E>) AsPair<T, E>(this AccumulatedResults<T, E> source) {
			return (source.Values, source.Errors);
		}

		public static AccumulatedResults<T, E> ToAccumulatedResult<T, E>(this (IEnumerable<T>, IEnumerable<E>) pair) =>
			AccumulatedResults.From(pair.Item1, pair.Item2);

		public static AccumulatedResults<U, E> ToAccumulatedResult<T, U, E>(
			this (IEnumerable<T>, IEnumerable<E>) pair,
			Func<IEnumerable<T>, IEnumerable<U>> selector
		) =>
			AccumulatedResults.From(selector(pair.Item1), pair.Item2);

		public static AccumulatedResults<T, E> ToAccumulatedResult<T, E>(this T value) =>
			AccumulatedResults.From<T, E>(value);
	}
}
