using UnityEngine;

namespace Player
{
	using Tools;

	/// <summary>
	/// Controls the bike's exhaust smoke based on its motion and throttle input.
	/// </summary>
	[RequireComponent(typeof(ParticleSystem))]
	[RequireComponent(typeof(AudioSource))]
	public class BikeExhaustParticlesAndSound : MonoBehaviour
	{
		[SerializeField] private float tireAngularVelocityMaximum = 10f;
		[SerializeField, Range(0.01f, 1f)] private float chanceToEmitParticles = 1f;
		[SerializeField] private float soundRandomPitchMinimum = 0.5f;
		[SerializeField] private float soundRandomPitchMaximum = 1.0f;
		
		private PlayerRefs _playerRefs;
		private ParticleSystem _particleSystem;
		private AudioSource _audioSource;
		private BikeStates.DrivingStates _previousDrivingState = BikeStates.DrivingStates.Brake;

		private void Awake()
		{
			_playerRefs = GetComponentInParent<PlayerRefs>();

			_particleSystem = GetComponent<ParticleSystem>();
			_audioSource = GetComponent<AudioSource>();
		}
		
		private void OnEnable() => _playerRefs.States.OnDrivingStateChange += HandleDrivingStateChange;
		private void OnDisable() => _playerRefs.States.OnDrivingStateChange -= HandleDrivingStateChange;
		
		/// <summary>
		/// Handles changes in the bike's driving state to control exhaust particle and sound emissions.
		/// </summary>
		private void HandleDrivingStateChange()
		{
			if (_playerRefs.States.DrivingState == BikeStates.DrivingStates.Throttle
			    && _previousDrivingState == BikeStates.DrivingStates.Brake
			    && !_particleSystem.isPlaying
			    && !_audioSource.isPlaying
			    && Mathf.Abs(_playerRefs.RearTireRigidbody.angularVelocity) < tireAngularVelocityMaximum
			    && Random.Range(0f, 1f) < chanceToEmitParticles)
			{
				PlayEffects();
			}
			
			_previousDrivingState = _playerRefs.States.DrivingState;
		}

		/// <summary>
		/// Plays the particle system and sound effect.
		/// </summary>
		public void PlayEffects()
		{
			_particleSystem.Play();
			_audioSource.PlayWithRandomPitch(soundRandomPitchMinimum, soundRandomPitchMaximum);
		}
	}
}
