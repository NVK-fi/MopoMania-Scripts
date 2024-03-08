namespace Tools
{
	using System.Linq;
	using UnityEngine;

	/// <summary>
	/// Extension methods for various functionalities related to audio, color, and mathematical operations.
	/// </summary>
	public static class ExtensionMethods
	{
		/// <summary>
		/// Selects a random AudioClip from the provided array and assigns it to the AudioSource for playback.
		/// </summary>
		/// <param name="audioSource">The AudioSource to assign the selected AudioClip to.</param>
		/// <param name="audioClips">Array of AudioClips to select from.</param>
		/// <param name="useNewAudioClipEachTime">If true, ensures that the same AudioClip is not selected consecutively.</param>
		public static void SelectRandomClip(this AudioSource audioSource, AudioClip[] audioClips, bool useNewAudioClipEachTime = false)
		{
			if (audioClips.Length == 0) return;
			
			var availableClips = audioClips;
			if (useNewAudioClipEachTime && availableClips.Length > 1)
			{
				availableClips = audioClips.Except(new[] { audioSource.clip }).ToArray();
			}
			
			audioSource.clip = availableClips[Random.Range(0, audioClips.Length - 1)];
			audioSource.Play();
		}

		/// <summary>
		/// Sets the pitch of the AudioSource to a random value within the specified range.
		/// </summary>
		/// <param name="audioSource">The AudioSource whose pitch will be set.</param>
		/// <param name="minPitch">The minimum pitch value.</param>
		/// <param name="maxPitch">The maximum pitch value.</param>
		public static void SetRandomPitch(this AudioSource audioSource, float minPitch = .75f, float maxPitch = 1.25f) 
			=> audioSource.pitch = Random.Range(minPitch, maxPitch);

		/// <summary>
		/// Plays the AudioSource with a random pitch within the specified range.
		/// </summary>
		/// <param name="audioSource">The AudioSource to play with a random pitch.</param>
		/// <param name="minPitch">The minimum pitch value.</param>
		/// <param name="maxPitch">The maximum pitch value.</param>
		public static void PlayWithRandomPitch(this AudioSource audioSource, float minPitch = .75f, float maxPitch = 1.25f)
		{
			audioSource.SetRandomPitch(minPitch, maxPitch);
			audioSource.Play();
		}

		/// <summary>
		/// Changes the alpha value of the Color.
		/// </summary>
		/// <param name="color">The Color to modify.</param>
		/// <param name="alpha">The new alpha value.</param>
		/// <returns>A new Color with the modified alpha value.</returns>
		public static Color ChangeAlpha(this Color color, float alpha)
		{
			return new Color(color.r, color.g, color.b, alpha);
		}

		/// <summary>
		/// Returns the absolute value of the specified float.
		/// </summary>
		/// <param name="value">The float value.</param>
		/// <returns>The absolute value of the float.</returns>
		public static float Abs(this float value) => Mathf.Abs(value);
	}
}
