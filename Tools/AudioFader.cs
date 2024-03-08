namespace Tools
{
	using System.Collections;
	using UnityEngine;

	/// <summary>
	/// A static class providing methods to fade audio volume over time.
	/// </summary>
	public static class AudioFader
	{
		public static Coroutine GlobalVolumeFaderCoroutine;

		/// <summary>
		/// Updates the global AudioListener volume over time.
		/// </summary>
		public static IEnumerator UpdateGlobalVolumeOverTime(float startVolume, float targetVolume, float time = 1f)
		{
			var elapsedTime = 0f;

			while (elapsedTime < time)
			{
				AudioListener.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / time);
				elapsedTime += Time.deltaTime;
				yield return null;
			}
			
			AudioListener.volume = targetVolume;
		}
		
		/// <summary>
		/// Changes the volume of an AudioSource over time.
		/// </summary>
		public static IEnumerator ChangeVolumeOverTime(this AudioSource audioSource, float targetVolume, float time = 1f)
		{
			var elapsedTime = 0f;
			var startVolume = audioSource.volume;
			
			while (elapsedTime < time)
			{
				audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / time);
				elapsedTime += Time.unscaledDeltaTime;
				yield return null;
			}
			
			audioSource.volume = targetVolume;
		}
	}
}
