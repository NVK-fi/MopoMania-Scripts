using UnityEngine;

namespace Tools
{
	/// <summary>
	/// Provides utility methods for various validation checks.
	/// </summary>
	public static class Check
	{
		/// <summary>
		/// Checks if a given reference is null. Does not halt the execution.
		/// </summary>
		/// <param name="reference">The reference to check.</param>
		/// <param name="unityObject">The UnityEngine Object that contains the reference. Used for error logging.</param>
		/// <param name="logError">Optional flag to control if an error should be logged when the reference is null.</param>
		/// <returns>True if the reference is null; otherwise, false.</returns>
		public static bool IsReferenceNull<T>(this Object unityObject, T reference, bool logError = true)
		{
			if (reference != null && !reference.Equals(null)) return false;

			if (!logError) return true;
			
			var errorMessage = unityObject != null
				? $"A {typeof(T).Name} property is null in {unityObject}."
				: $"A {typeof(T).Name} property is null. (Containing object is null.)";
			
			Debug.LogError(errorMessage, unityObject);

			return true;
		}

		/// <summary>
		/// Determines if a value is between the specified limits.
		/// The check is exclusive of the lower limit and inclusive of the upper limit.
		/// </summary>
		/// <param name="value">Value to check.</param>
		/// <param name="lower">Exclusive lower limit.</param>
		/// <param name="upper">Inclusive upper limit.</param>
		/// <returns>True if the value is between the limits; otherwise, false.</returns>
		public static bool IsBetween(this float value, float lower, float upper) => value > lower && value <= upper;
	}
}