using UnityEngine;

namespace Player
{
	/// <summary>
	/// This class manages the rotation of the fowl based on acceleration.
	/// It visualizes the impact of acceleration on the fowl's posture.
	/// </summary>
	public class RidersFowlTilt : MonoBehaviour
	{
		[SerializeField] private float maxAngle = 30f;
		[SerializeField] private float topAcceleration = 50f;
		[SerializeField] private float slerpSpeed = 10f;

		private PlayerRefs _playerRefs;
		private Quaternion _defaultRotation;
		
		private void Awake()
		{
			_playerRefs = transform.root.GetComponent<PlayerRefs>();
		}

		private void FixedUpdate() => ApplyFowlRotation();

		/// <summary>
		/// Adjusts the fowl's tilt based on its acceleration along the local X-axis. 
		/// The rotation angle is clamped between -maxAngle and maxAngle based on the topAcceleration value.
		/// </summary>
		private void ApplyFowlRotation()
		{
			// Get the acceleration of the bike in the local X-axis.
			var acceleration = transform.InverseTransformDirection(_playerRefs.Physics.FowlAcceleration).x;

			// Normalize and clamp the acceleration between -1 and 1.
			var normalizedAngle = Mathf.Clamp(acceleration / topAcceleration, -1f, 1f);
			
			// Get the desired angle.
			var targetAngle = normalizedAngle * maxAngle;
			
			// Create and apply the target rotation.
			var targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
			
			transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, 
				slerpSpeed * Time.fixedDeltaTime);
		}
	}
}
