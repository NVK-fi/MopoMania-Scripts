namespace Player
{
	using Managers;
	using ScriptableObjects;
	using UnityEngine;

	/// <summary>
	/// Handles rotating the bike based on player's input.
	/// </summary>
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(BikeStates))]
	public class BikeRotate : MonoBehaviour
	{
		private PlayerRefs _playerRefs;
		private BikeData _bikeData;
		private Rigidbody2D _rigidbody;

		private void Awake()
		{
			_playerRefs = transform.root.GetComponent<PlayerRefs>();
			_rigidbody = GetComponent<Rigidbody2D>();
			_bikeData = _playerRefs.BikeData;
		}

		private void FixedUpdate()
		{
			if (GameManager.State != GameManager.GameState.Playing) 
				return;
			
			ApplyPlayerInput();
			ConstrainAngularVelocity();
		}

		/// <summary>
		/// Applies the player's input for rotating the bike.
		/// </summary>
		private void ApplyPlayerInput()
		{
			if (_playerRefs.States.RotatingState != BikeStates.RotatingStates.Neither)
				AddTorque();
		}
		
		/// <summary>
		/// Rotates the bike based on player input and predefined torque.
		/// </summary>
		private void AddTorque()
		{
			_rigidbody.AddTorque(-(int)_playerRefs.States.RotatingState * _bikeData.rotatingTorque * Time.fixedDeltaTime);
		}

		/// <summary>
		/// Constrains the bike's angular velocity to prevent overly fast rotations.
		/// </summary>
		private void ConstrainAngularVelocity()
		{
			_rigidbody.angularVelocity =
				Mathf.Clamp(_rigidbody.angularVelocity, -_bikeData.rotatingMaxAngularVelocity,
					_bikeData.rotatingMaxAngularVelocity);
		}
	}
}