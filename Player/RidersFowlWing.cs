using UnityEngine;

namespace Player
{
	/// <summary>
	/// Handles the visual representation and rotation of the fowl's wing.
	/// </summary>
	public class RidersFowlWing : MonoBehaviour
	{
		[SerializeField] private float upperRotation = 30f;
		[SerializeField] private float lowerRotation = 15f;
		[SerializeField] private float velocityThreshold = 2f;
		[SerializeField] private float slerpSpeed = 5f;

		private PlayerRefs _playerRefs;
		
		private void Awake()
		{
			_playerRefs = transform.root.GetComponent<PlayerRefs>();
		}

		private void Update() => RotateWing();
		
		/// <summary>
		/// Adjusts the wing's rotation based on the fowl's vertical velocity. 
		/// Uses different rotation extents for upward and downward movements.
		/// </summary>
		private void RotateWing()
		{
			var velocityNormalized = _playerRefs.Physics.FowlVelocity.normalized;
			var yVelocity = transform.parent.InverseTransformDirection(velocityNormalized).y;

			var targetAngle = 0f;

			// Set a new targetAngle if the velocity's magnitude is over the threshold.
			if (_playerRefs.Physics.FowlVelocity.sqrMagnitude > velocityThreshold)
			{
				targetAngle = yVelocity < 0 ? upperRotation : lowerRotation;
				targetAngle *= yVelocity;
			}

			var targetRotation = Quaternion.Euler(0f, 0f, targetAngle);

			transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, slerpSpeed * Time.deltaTime);
		}
	}
}
