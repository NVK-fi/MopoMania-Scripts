using UnityEngine;

namespace Player
{
	using Managers;
	using Tools;

	/// <summary>
	/// Manages the sounds of the bike engine based on player input and game state.
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	public class BikeEngineSounds : MonoBehaviour
	{
		[SerializeField] private float idlePitch = 1f;
		[SerializeField] private float minThrottlePitch = 1.3f;
		[SerializeField] private float maxThrottlePitch = 1.5f;
		[SerializeField] private float angularVelocityAtMaxPitch = 3000f;
		[SerializeField] private float decelerationTime = .5f;
		
		private PlayerRefs _playerRefs;
		private BikeStates _bikeStates;
		private AudioSource _audioSource;
		private Coroutine _audioFadeCoroutine;
		private bool _isThrottling;
		private bool _isIdling = true;
		private float _lerpTimer;
		private float _lastThrottlePitch;

		private void Awake()
		{
			_playerRefs = GetComponentInParent<PlayerRefs>();
			this.IsReferenceNull(_playerRefs);

			_bikeStates = _playerRefs.States;
			_audioSource = GetComponent<AudioSource>();
		}

		private void Update()
		{
			if (_isIdling && !_isThrottling) return;
			
			if (_isThrottling)
			{
				var angularVelocity = Mathf.Abs(_playerRefs.RearTireRigidbody.angularVelocity);
				var normalizedAngularVelocity = Mathf.Clamp01(angularVelocity / angularVelocityAtMaxPitch);
				
				var pitch = Mathf.Lerp(idlePitch, maxThrottlePitch, normalizedAngularVelocity);
				pitch = Mathf.Max(pitch, minThrottlePitch);
				
				_audioSource.pitch = pitch;

				_lerpTimer = decelerationTime - decelerationTime * normalizedAngularVelocity;
			}
			else
			{
				// Check if there's a need to lerp the pitch.
				if (_audioSource.pitch - idlePitch > 0.01f)
				{
					_lerpTimer += Time.deltaTime;
					_audioSource.pitch = Mathf.Lerp(_lastThrottlePitch, idlePitch, _lerpTimer / decelerationTime);	
				}
				else
				{
					_audioSource.pitch = idlePitch;

					_lerpTimer = 0f;
					_isIdling = true;
				}
			}
		}
		
		private void OnEnable()
		{
			GameManager.Instance.OnGameStateChange += HandleGameStateChange;
			_bikeStates.OnDrivingStateChange += HandleDrivingStateChange;
		}

		private void OnDisable()
		{
			_bikeStates.OnDrivingStateChange -= HandleDrivingStateChange;
			GameManager.Instance.OnGameStateChange -= HandleGameStateChange;
		}

		/// <summary>
		/// Handles changes in game state by adjusting the engine sound volume.
		/// </summary>
		private void HandleGameStateChange()
		{
			if (_audioFadeCoroutine != null)
			{
				StopCoroutine(_audioFadeCoroutine);
				_audioFadeCoroutine = null;
			}
			
			var state = GameManager.State;
			_audioFadeCoroutine = state switch
			{
				GameManager.GameState.Finished => StartCoroutine(_audioSource.ChangeVolumeOverTime(0, 1.2f)),
				GameManager.GameState.Failed => StartCoroutine(_audioSource.ChangeVolumeOverTime(0, .2f)),
				GameManager.GameState.Playing => StartCoroutine(_audioSource.ChangeVolumeOverTime(1, .1f)),
				_ => _audioFadeCoroutine
			};
		}

		/// <summary>
		/// Handles changes in driving state.
		/// </summary>
		private void HandleDrivingStateChange()
		{
			_isThrottling = _bikeStates.DrivingState == BikeStates.DrivingStates.Throttle;

			if (_isThrottling)
				_isIdling = false;
			else
				_lastThrottlePitch = _audioSource.pitch;
		}
	}
}
