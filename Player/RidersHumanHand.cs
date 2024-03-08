using UnityEngine;

namespace Player
{
	
	/// <summary>
	/// Rotates the rider's hand based on throttle.
	/// </summary>
	public class RidersHumanHand : MonoBehaviour
	{
		[SerializeField] private float throttlingRotation = 30f;
		[SerializeField] private float throttlingSlerpSpeed = 10f;
		private PlayerRefs _playerRefs;
		private float _targetAngle;
		private float _defaultAngle;
		
		private void Awake()
		{
			_playerRefs = transform.root.GetComponent<PlayerRefs>();
		}

		private void Start()
		{
			_defaultAngle = transform.localEulerAngles.z;
			_targetAngle = _defaultAngle;
		}

		private void Update()
		{
			RotateHand();
		}

		private void OnEnable() => _playerRefs.States.OnDrivingStateChange += HandleDrivingStateChange;

		private void OnDisable() => _playerRefs.States.OnDrivingStateChange -= HandleDrivingStateChange;

		/// <summary>
		/// Handles the change in the driving state of the bike and adjusts the target angle accordingly.
		/// </summary>
		private void HandleDrivingStateChange()
		{
			_targetAngle = _playerRefs.States.DrivingState == BikeStates.DrivingStates.Throttle 
				? _defaultAngle + throttlingRotation 
				: _defaultAngle;
		}

		/// <summary>
		/// Rotates the rider's hand towards the target angle using spherical interpolation.
		/// </summary>
		private void RotateHand()
		{
			var startRotation = transform.localRotation;
			var targetRotation = Quaternion.Euler(0f, 0f, _targetAngle);

			transform.localRotation =
				Quaternion.Slerp(startRotation, targetRotation, Time.deltaTime * throttlingSlerpSpeed);
		}
	}
}
