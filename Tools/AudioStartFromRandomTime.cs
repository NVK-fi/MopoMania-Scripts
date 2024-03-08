using UnityEngine;

namespace Tools
{
	/// <summary>
	/// Sets the playback time of audio sources to a random position within their clip length upon starting.
	/// </summary>
	public class AudioStartFromRandomTime : MonoBehaviour
	{
		[SerializeField] private AudioSource[] audioSources;

		private void Start()
		{
			foreach (var audioSource in audioSources)
			{
				audioSource.time = Random.Range(0f, audioSource.clip.length);
			}
		}
	}
}
