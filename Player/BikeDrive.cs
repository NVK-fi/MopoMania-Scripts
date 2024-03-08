using ScriptableObjects;
using Tools;
using UnityEngine;

namespace Player
{
	using Managers;

	/// <summary>
	/// Manages the bike's throttling and braking.
	/// </summary>
	[RequireComponent(typeof(BikeStates))]
	public class BikeDrive : MonoBehaviour
	{
		private PlayerRefs _playerRefs;
		private BikeStates _bikeStates;
		private BikeData _bikeData;
		private Rigidbody2D _leftTire;
		private Rigidbody2D _rightTire;
		private Rigidbody2D _rearTire;
		private float _brakeInertiaDefault;

		private void Awake()
		{
			_playerRefs = transform.root.GetComponent<PlayerRefs>();

			_bikeStates = _playerRefs.States;
			_bikeData = _playerRefs.BikeData;
			_leftTire = _playerRefs.LeftTireRigidbody;
			_rightTire = _playerRefs.RightTireRigidbody;
			_rearTire = _playerRefs.RearTireRigidbody;
		}

		private void Start()
		{
			// Get the default inertia value for a tire.
			_brakeInertiaDefault = _leftTire.inertia;
		}

		private void OnEnable() => SubscribeToEvents();

		private void OnDisable() => UnsubscribeFromEvents();

		private void FixedUpdate()
		{
			if (GameManager.State != GameManager.GameState.Playing) 
				return;
			
			ApplyPlayerInput();
			ConstrainTireAngularVelocity();
		}

		private void SubscribeToEvents()
		{
			_bikeStates.OnFlipStateChange += HandleFlipStateChange;
			_bikeStates.OnDrivingStateChange += HandleDrivingStateChange;
		}
		private void UnsubscribeFromEvents()
		{
			_bikeStates.OnFlipStateChange -= HandleFlipStateChange;
			_bikeStates.OnDrivingStateChange -= HandleDrivingStateChange;
		}

		/// <summary>
		/// Handles changes when the bike flips.
		/// </summary>
		private void HandleFlipStateChange()
		{
			// Update the rear tire reference.
			_rearTire = _playerRefs.RearTireRigidbody;
			
			// Swap the tires' angular velocities.
			(_leftTire.angularVelocity, _rightTire.angularVelocity) = (_rightTire.angularVelocity, _leftTire.angularVelocity);
		}

		/// <summary>
		/// Handles changes in the driving state by releasing the brakes if needed.
		/// </summary>
		private void HandleDrivingStateChange()
		{
			if (_bikeStates.DrivingState != BikeStates.DrivingStates.Brake)
				ReleaseBrakes();
		}

		/// <summary>
		/// Applies the throttle, braking, rotating and flipping.
		/// </summary>
		private void ApplyPlayerInput()
		{
			if (_bikeStates.DrivingState == BikeStates.DrivingStates.Throttle) Throttle();
			else if (_bikeStates.DrivingState == BikeStates.DrivingStates.Brake) Brake();
		}

		/// <summary>
		/// Limit the tires from rotating too fast.
		/// </summary>
		private void ConstrainTireAngularVelocity()
		{
			_leftTire.angularVelocity =
				Mathf.Clamp(_leftTire.angularVelocity, -_bikeData.tiresMaxAngularVelocity,
					_bikeData.tiresMaxAngularVelocity);
			_rightTire.angularVelocity =
				Mathf.Clamp(_rightTire.angularVelocity, -_bikeData.tiresMaxAngularVelocity,
					_bikeData.tiresMaxAngularVelocity);
		}

		/// <summary>
		/// Applies throttling torque to the rear tire.
		/// </summary>
		private void Throttle()
		{
			// The direction of rotation will change based on if the bike has been flipped.
			var directionMultiplier = _bikeStates.IsFlipped ? 1 : -1;

			// Adds torque based on signed throttleTorque value multiplied with fixedDeltaTime.
			_rearTire.AddTorque(directionMultiplier * _bikeData.throttleTorque * Time.fixedDeltaTime);
		}

		/// <summary>
		/// Brakes both tires by setting angular velocity to zero and adjusting inertia.
		/// </summary>
		private void Brake()
		{
			_leftTire.angularVelocity *= _bikeData.brakeAngularVelocityMultiplier;
			_leftTire.inertia = _bikeData.brakeInertia;
			_rightTire.angularVelocity *= _bikeData.brakeAngularVelocityMultiplier;
			_rightTire.inertia = _bikeData.brakeInertia;
		}

		/// <summary>
		/// Releases the brakes by resetting both of the tires' inertiae to their default values.
		/// </summary>
		private void ReleaseBrakes()
		{
			_leftTire.inertia = _brakeInertiaDefault;
			_rightTire.inertia = _brakeInertiaDefault;
		}
	}
}