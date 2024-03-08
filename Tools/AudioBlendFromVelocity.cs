using UnityEngine;

namespace Tools
{
	using System.Collections;
	using Managers;
	using Player;

	/// <summary>
	/// Controls the blending of audio sources based on the player's velocity.
	/// </summary>
	public class AudioBlendFromVelocity : MonoBehaviour
	{
		[field: SerializeField] public bool ShouldBlendAudio { get; set; } = true;
		[SerializeField] private AudioSource audioSourceAtLowVelocity;
		[SerializeField] private AudioSource audioSourceAtHighVelocity;
		[SerializeField] private AnimationCurve lowVelocityVolumeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
		[SerializeField] private AnimationCurve highVelocityVolumeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
		[SerializeField] private float minimumVelocity = 50f;
		[SerializeField] private float maximumVelocity = 300f;
		[SerializeField, Range(0.01f, 1f)] private float updateTime = .1f;
		[SerializeField, Range(1, 100)] private int velocityGranularity = 10;
		
		private PlayerPhysics _playerPhysics;
		private Coroutine _blendCoroutine;
		private int _previousVelocityIndex;
		
		private void Awake()
		{
			this.IsReferenceNull(audioSourceAtLowVelocity);
			this.IsReferenceNull(audioSourceAtHighVelocity);

			_playerPhysics = LevelManager.Instance.Player.Physics;
		}

		private void OnEnable()
		{
			if (_blendCoroutine != null) StopCoroutine(_blendCoroutine);
			_blendCoroutine = StartCoroutine(AudioBlendCoroutine());
		}

		private void OnDisable()
		{
			if (_blendCoroutine != null) StopCoroutine(_blendCoroutine);
			_blendCoroutine = null;
		}

		/// <summary>
		/// Runs a Coroutine to blend the audio sources based on the player's velocity.
		/// Stops and returns the audio to their default volumes when ShouldBlendAudio is set to false.
		/// </summary>
		private IEnumerator AudioBlendCoroutine()
		{
			while (ShouldBlendAudio)
			{
				yield return new WaitForSecondsRealtime(updateTime);
				BlendAudio();
			}
			
			// Set volumes back to default values.
			audioSourceAtLowVelocity.volume = 1f;
			audioSourceAtHighVelocity.volume = 0f;
		}

		/// <summary>
		/// Adjusts the volume of audio sources based on the player's current velocity.
		/// The velocity value is normalized and converted to a granular index.
		/// This index is then used to evaluate the volumes from the animation curves and set the volume on the audio sources.
		/// </summary>
		private void BlendAudio()
		{
			var velocitySqrMagnitude = GameManager.State is GameManager.GameState.Playing
			? _playerPhysics.BikeVelocity.sqrMagnitude
			: 0f;
			
			var normalizedVelocity = Mathf.InverseLerp(minimumVelocity, maximumVelocity, velocitySqrMagnitude);
			
			// Create the velocityIndex to check if the velocity has changed enough for the update to be necessary.
			var velocityIndex = Mathf.FloorToInt(normalizedVelocity * velocityGranularity);
			if (velocityIndex == _previousVelocityIndex) return;
			
			// Gradually change the velocityIndex towards the wanted index, making the changes happen less suddenly.
			velocityIndex = _previousVelocityIndex + (velocityIndex > _previousVelocityIndex ? 1 : -1);
			_previousVelocityIndex = velocityIndex;

			// Normalize the velocityIndex to use it with the volume curves.
			var granularVelocity = velocityIndex / (float)velocityGranularity;

			audioSourceAtLowVelocity.volume = lowVelocityVolumeCurve.Evaluate(granularVelocity);
			audioSourceAtHighVelocity.volume = highVelocityVolumeCurve.Evaluate(granularVelocity);
		}
	}
}
