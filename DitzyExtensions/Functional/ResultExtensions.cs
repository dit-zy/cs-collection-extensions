using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using DitzyExtensions.Collection;

namespace DitzyExtensions.Functional {
	public static class ResultExtensions {
		public static AccumulatedResults<U, E> Select<T, U, E>(this AccumulatedResults<T, E> source, Func<T, U> selector) =>
			source.HasValue
				? AccumulatedResults.From(selector(source.Value), source.Errors)
				: AccumulatedResults.From<U, E>(source.Errors);

		public static AccumulatedResults<IEnumerable<U>, E> Select<T, U, E>(
			this AccumulatedResults<IEnumerable<T>, E> source,
			Func<T, U> selector
		) =>
			AccumulatedResults.From(source.Value.Select(selector), source.Errors);

		public static AccumulatedResults<IEnumerable<U>, E> SelectMany<T, U, E>(
			this IEnumerable<T> source,
			Func<T, AccumulatedResults<U, E>> selector
		) {
			return source.Select(selector)
				.Reduce(
					(acc, results) => {
						acc.values.Add(results.Value);
						acc.errors.AddRange(results.Errors);
						return acc;
					},
					(values: new List<U>(), errors: new List<E>())
				)
				.AsAccumulatedResults<U, E>();
		}

		public static AccumulatedResults<U, E> SelectMany<T, U, E>(
			this AccumulatedResults<T, E> source,
			Func<T, AccumulatedResults<U, E>> selector
		) {
			if (!source.HasValue) return AccumulatedResults.From<U, E>(source.Errors);

			var selectedResults = selector(source.Value);
			var errors = source.Errors.Concat(selectedResults.Errors);
			return selectedResults.HasValue
				? AccumulatedResults.From(selectedResults.Value, errors)
				: AccumulatedResults.From<U, E>(errors);
		}

		public static AccumulatedResults<IEnumerable<U>, E> SelectResults<T, U, E>(
			this AccumulatedResults<IEnumerable<T>, E> source,
			Func<T, Result<U, E>> selector
		) {
			var accumulatedResults = source.Value.SelectResults(selector);
			return AccumulatedResults.From(
				accumulatedResults.Value,
				source.Errors.Concat(accumulatedResults.Errors)
			);
		}

		public static AccumulatedResults<IEnumerable<T>, string> SelectResults<T>(this IEnumerable<Result<T>> source) =>
			source.SelectResults(result => result);

		public static AccumulatedResults<IEnumerable<U>, string> SelectResults<T, U>(
			this IEnumerable<T> source,
			Func<T, Result<U>> selector
		) =>
			source.SelectResults(
				value => selector(value).Match(
					Result.Success<U, string>,
					Result.Failure<U, string>
				)
			);

		public static AccumulatedResults<IEnumerable<T>, E> SelectResults<T, E>(this IEnumerable<Result<T, E>> source) =>
			source.SelectResults(result => result);

		public static AccumulatedResults<IEnumerable<U>, E> SelectResults<T, U, E>(
			this IEnumerable<T> source,
			Func<T, Result<U, E>> selector
		) =>
			source.BindResults(result => selector(result).Map(AccumulatedResults.From<U, E>));

		public static AccumulatedResults<IEnumerable<U>, E> BindResults<T, U, E>(
			this IEnumerable<T> source,
			Func<T, Result<AccumulatedResults<U, E>, E>> selector
		) =>
			source.Reduce(
					(acc, value) => {
						selector(value).Match(
							success => {
								acc.results.Add(success.Value);
								acc.errors.AddRange(success.Errors);
							},
							error => acc.errors.Add(error)
						);
						return acc;
					},
					(results: new List<U>(), errors: new List<E>())
				)
				.AsAccumulatedResults<U, E>();

		public static AccumulatedResults<IEnumerable<T>, E> Concat<T, E>(
			this AccumulatedResults<IEnumerable<T>, E> source,
			AccumulatedResults<IEnumerable<T>, E> secondSource
		) =>
			source.Concat(secondSource, (ns, ms) => ns.Concat(ms));

		public static AccumulatedResults<IEnumerable<V>, E> Concat<T, U, V, E>(
			this AccumulatedResults<IEnumerable<T>, E> source,
			AccumulatedResults<IEnumerable<U>, E> secondSource,
			Func<IEnumerable<T>, IEnumerable<U>, IEnumerable<V>> combiner
		) =>
			AccumulatedResults.From(
				combiner(source.Value, secondSource.Value),
				source.Errors.Concat(secondSource.Errors)
			);

		public static AccumulatedResults<U, E> WithValue<T, U, E>(
			this AccumulatedResults<T, E> source,
			Func<T, U> valueModifier
		) =>
			AccumulatedResults.From(valueModifier(source.Value), source.Errors);

		public static AccumulatedResults<IEnumerable<T>, E> ForEachValue<T, E>(
			this AccumulatedResults<IEnumerable<T>, E> source,
			Action<T> action
		) {
			source.Value.ForEach(action);
			return source;
		}

		public static AccumulatedResults<T, E> ForEachError<T, E>(this AccumulatedResults<T, E> source, Action<E> action) {
			source.Errors.ForEach(action);
			return source;
		}

		public static (T, IEnumerable<E>) AsPair<T, E>(this AccumulatedResults<T, E> source) {
			return (source.Value, source.Errors);
		}

		public static AccumulatedResults<T, E> AsAccumulatedResults<T, E>(this (T, IEnumerable<E>) pair) =>
			AccumulatedResults.From(pair.Item1, pair.Item2);

		public static AccumulatedResults<IEnumerable<T>, E> AsAccumulatedResults<T, E>(
			this (IEnumerable<T>, IEnumerable<E>) pair
		) =>
			AccumulatedResults.From(pair.Item1, pair.Item2);

		public static AccumulatedResults<U, E> AsAccumulatedResults<T, U, E>(
			this (T, IEnumerable<E>) pair,
			Func<T, U> selector
		) =>
			AccumulatedResults.From(selector(pair.Item1), pair.Item2);

		public static AccumulatedResults<T, E> AsAccumulatedResults<T, E>(this T value) =>
			AccumulatedResults.From<T, E>(value);

		public static AccumulatedResults<T, E> AsAccumulatedResults<T, E>(this Result<T, E> result) =>
			result.Match(
				t => t.AsAccumulatedResults<T, E>(),
				e => e
			);

		public static AccumulatedResults<(T, U), E> PairWith<T, U, E>(
			this Result<T, E> firstResult,
			Result<U, E> secondResult
		) =>
			firstResult.JoinWith(secondResult, (t, u) => (t, u));

		public static AccumulatedResults<(T, U), E> PairWith<T, U, E>(
			this AccumulatedResults<T, E> firstResult,
			AccumulatedResults<U, E> secondResult
		) =>
			firstResult.JoinWith(secondResult, (t, u) => (t, u));

		public static AccumulatedResults<V, E> JoinWith<T, U, V, E>(
			this Result<T, E> firstResult,
			Result<U, E> secondResult,
			Func<T, U, V> transform
		) =>
			firstResult.AsAccumulatedResults().JoinWith(secondResult.AsAccumulatedResults(), transform);

		public static AccumulatedResults<V, E> JoinWith<T, U, V, E>(
			this AccumulatedResults<T, E> firstResults,
			AccumulatedResults<U, E> secondResults,
			Func<T, U, V> transform
		) {
			var errors = firstResults.Errors.Concat(secondResults.Errors);
			return firstResults.HasValue && secondResults.HasValue
				? AccumulatedResults.From(transform(firstResults.Value, secondResults.Value), errors)
				: AccumulatedResults.From<V, E>(errors);
		}
	}
}
