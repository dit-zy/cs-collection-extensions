using System;
using System.Collections.Generic;
using System.Linq;

namespace DitzyExtensions.Functional {
	public class AccumulationHasNoValueException<E> : Exception {
		internal AccumulationHasNoValueException(IList<E> errors) : base() { }

		private static string GetMessage(IList<E> errors) {
			var errorsStr =
				errors
					.Select(error => $"- {error?.ToString()}")
					.Join("\n");
			return
				$"you attempted to access the Value of an accumulated result with no Value. The accumulated errors are:\n{errorsStr}";
		}
	}
}
