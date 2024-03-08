using UnityEngine;

namespace Player
{
	/// <summary>
	/// Holds physics calculations and properties.
	/// Make sure it is executed before the default time in Script Execution Order list. 
	/// </summary>
	[RequireComponent(typeof(PlayerRefs))]
	public class PlayerPhysics : MonoBehaviour
	{
		/// <summary>
		/// Exceeding this will clamp the velocity property for the frame.
		/// Used to mitigate potential problems occurring from teleportation.
		/// </summary>
		[SerializeField] private float maxVelocityMagnitude = 50f;
		
		/// <summary>
		/// The clamped acceleration vector of the bike in world space.
		/// </summary>
		public Vector2 BikeAcceleration { get; private set; }
		
		/// <summary>
		/// The clamped velocity vector of the bike in world space.
		/// </summary>
		public Vector2 BikeVelocity { get; private set; }
		
		/// <summary>
		/// The clamped acceleration vector of the fowl in world space.
		/// </summary>
		public Vector2 FowlAcceleration { get; private set; }
		
		/// <summary>
		/// The clamped velocity vector of the fowl in world space.
		/// </summary>
		public Vector2 FowlVelocity { get; private set; }

		private PlayerRefs _playerRefs;
		private Rigidbody2D _bikeRigidbody;
		private Rigidbody2D _fowlRigidbody;
		private Vector2 _bikePreviousVelocity;
		private Vector2 _fowlPreviousVelocity;
		private Vector2 _fowlPreviousPosition;
		private Vector3 _leftSuspensionDefaultPosition;
		private Vector3 _rightSuspensionDefaultPosition;
		
		#region /* UNITY LIFECYCLE */

		private void Awake()
		{
			_playerRefs = GetComponentInParent<PlayerRefs>();
			_bikeRigidbody = _playerRefs.BikeRigidbody;
			_fowlPreviousPosition = _playerRefs.FowlTransform.position;

			_leftSuspensionDefaultPosition = _playerRefs.LeftTireRigidbody.transform.parent.localPosition;
			_rightSuspensionDefaultPosition = _playerRefs.RightTireRigidbody.transform.parent.localPosition;
		}

		private void FixedUpdate()
		{
			UpdateVelocities();
			UpdateAccelerations();
		}

		#endregion

		#region /* CUSTOM METHODS */

		/// <summary>
		/// Resets the physics properties to their initial states.
		/// </summary>
		public void ResetPhysics()
		{
			_bikePreviousVelocity = Vector2.zero;
			_fowlPreviousVelocity = Vector2.zero;
			_fowlPreviousPosition = Vector2.zero;
			
			var rigidbodies = transform.root.GetComponentsInChildren<Rigidbody2D>();
			foreach (var rb in rigidbodies) 
				rb.Sleep();

			var leftTire = _playerRefs.LeftTireRigidbody.transform;
			leftTire.localRotation = Quaternion.identity;
			leftTire.localPosition = Vector3.zero;
			leftTire.parent.localPosition = _leftSuspensionDefaultPosition;

			var rightTire = _playerRefs.RightTireRigidbody.transform;
			rightTire.localRotation = Quaternion.identity;
			rightTire.localPosition = Vector3.zero;
			rightTire.parent.localPosition = _rightSuspensionDefaultPosition;
		}
		
		/// <summary>
		/// Updates the velocities of the bike and fowl based on their current positions.
		/// </summary>
		private void UpdateVelocities()
		{
			BikeVelocity = _bikeRigidbody.velocity;
			
			var fowlCurrentPosition = (Vector2)_playerRefs.FowlTransform.position;
			FowlVelocity = RateOfChange(fowlCurrentPosition, ref _fowlPreviousPosition, Time.fixedDeltaTime);
			
			// Clamp the velocities to mitigate potential problems occurring from teleportation.
			BikeVelocity = Vector2.ClampMagnitude(BikeVelocity, maxVelocityMagnitude);
			FowlVelocity = Vector2.ClampMagnitude(FowlVelocity, maxVelocityMagnitude);
		}

		/// <summary>
		/// Updates the accelerations of the bike and fowl based on their previous velocities.
		/// </summary>
		private void UpdateAccelerations()
		{
			// Calculate the accelerations based on previous velocities.
			BikeAcceleration = RateOfChange(BikeVelocity, ref _bikePreviousVelocity, Time.fixedDeltaTime);
			FowlAcceleration = RateOfChange(FowlVelocity, ref _fowlPreviousVelocity, Time.fixedDeltaTime);
		}
		
		private static Vector2 RateOfChange(Vector2 currentValue, ref Vector2 previousValue, float time)
		{
			var delta = (currentValue - previousValue) / time;
			previousValue = currentValue;
			return delta;
		}
		
		#endregion
	}
}
