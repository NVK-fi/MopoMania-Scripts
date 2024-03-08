using UnityEngine;

namespace Items
{
	using Tools;

	/// <summary>
	/// A singleton, which manages audio effects for egg collecting and saving actions.
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	public class EggCollectingAudio : MonoBehaviour
	{
		public static EggCollectingAudio Instance;

		[SerializeField] private AudioClip[] pickupAudioClips;
		[SerializeField] private AudioClip[] savingAudioClips;
		private AudioSource _audioSource;
		
		private void Awake()
		{
			// Singleton
			if (Instance != null) Destroy(gameObject);
			else Instance = this;

			_audioSource = GetComponent<AudioSource>();
		}

		/// <summary>
		/// Plays a random pickup audio clip.
		/// </summary>
		public void PlayPickupEffects()
		{
			_audioSource.SelectRandomClip(pickupAudioClips);
			_audioSource.Play();
		}

		/// <summary>
		/// Plays a random saving audio clip. Used at checkpoints.
		/// </summary>
		public void PlaySavingEffects()
		{
			_audioSource.SelectRandomClip(savingAudioClips);
			_audioSource.Play();
		}
	}
}
