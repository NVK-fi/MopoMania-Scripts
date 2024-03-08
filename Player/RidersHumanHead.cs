using UnityEngine;

namespace Player
{
	using Tools;
	using Unity.Mathematics;

	/// <summary>
	/// Rotates and moves the head slightly based on the rotation and velocity of the bike.
	/// </summary>
	public class RidersHumanHead : MonoBehaviour
	{
		[Header("Rotation")]
		[Range(0f, 30f)][SerializeField] private float rotationLimit = 15f;
		[Range(10f, 100f)][SerializeField] private float upsideDownSectorSize = 60f;
		[SerializeField] private float rotationSlerpSpeed = 10f;
		[Header("Movement")]
		[SerializeField] private float movementDistance = .05f;
		[SerializeField] private float movementLerpSpeed = 8f;
		[SerializeField] private float accelerationThreshold = 6f;
		[SerializeField] private float maxAcceleration = 10f;

		private PlayerRefs _playerRefs;
		private float _defaultPosition;
		private float _defaultHeadAngle;
		private float _defaultBodyAngle;
		private Transform _headTransform;
		private Transform _bodyTransform;
		private float4 _bounds;

		private void Awake()
		{
			_headTransform = transform;
			_bodyTransform = _headTransform.parent;
			_playerRefs = _headTransform.root.GetComponent<PlayerRefs>();
		}

		private void Start()
		{
			_defaultPosition = _headTransform.localPosition.x;
			_defaultHeadAngle = _headTransform.localRotation.eulerAngles.z;
			_defaultBodyAngle = _bodyTransform.localRotation.eulerAngles.z;

			CalculateSectorBounds();
		}

		private void FixedUpdate()
		{
			ApplyHeadRotation();
			ApplyHeadMovement();
		}

		/// <summary>
		/// Computes and applies the appropriate rotation to the head based on the bike's direction.
		/// </summary>
		private void ApplyHeadRotation()
		{
			var targetAngle = _playerRefs.States.IsFlipInTransition ? _defaultHeadAngle : CalculateHeadAngle();
			
			var targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
			
			_headTransform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, rotationSlerpSpeed * Time.fixedDeltaTime);
		}

		/// <summary>
		/// Sets the sector bounds for head rotation.
		/// These sectors determine how the head should rotate relative to the bike's body.
		/// </summary>
		private void CalculateSectorBounds()
		{
			_bounds[0] = rotationLimit;
			_bounds[1] = 180f - upsideDownSectorSize * .5f;
			_bounds[2] = 180f + upsideDownSectorSize * .5f;
			_bounds[3] = 360f - rotationLimit;
		}

		/// <summary>
		/// Calculates the desired angle for the head based on the current body rotation.
		/// The rotation is divided into sectors to determine the head's orientation calculation.
		/// </summary>
		/// <returns>The desired angle for the head in degrees.</returns>
		private float CalculateHeadAngle()
		{
			var bodyAngle = _bodyTransform.eulerAngles.z % 360;

			// Calculate the angle based on the current sector the bike is in.
			var headAngle = 
				bodyAngle.IsBetween(_bounds[0], _bounds[1]) ? _bounds[3]
				: bodyAngle.IsBetween(_bounds[1], _bounds[2]) ? InterpolateUpsideDownSector()
				: bodyAngle.IsBetween(_bounds[2], _bounds[3]) ? _bounds[0]
				: -bodyAngle;

			// Calculate the default angle offset and invert it if the bike is flipped.
			var defaultAngle = _defaultBodyAngle + _defaultHeadAngle;
			defaultAngle *= _playerRefs.States.IsFlipped ? -1 : 1;
			
			// Add the offset angle.
			headAngle += defaultAngle;
			
			// Invert the target angle if the bike is flipped.
			headAngle *= _playerRefs.States.IsFlipped ? -1 : 1;
			
			return headAngle;
			
			float InterpolateUpsideDownSector()
			{
				// Calculates the transition value between 0 and 1 for the head's angle.
				var inverseLerp = Mathf.InverseLerp(_bounds[1], _bounds[2], bodyAngle);
				
				// Returns the interpolated angle within the correct range.
				return Mathf.LerpAngle(_bounds[3], _bounds[0], inverseLerp);
			}
		}

		/// <summary>
		/// Calculates and applies the head's movement.
		/// When accelerating forwards, the head is pushed back on its X-axis and vice versa.
		/// </summary>
		private void ApplyHeadMovement()
		{
			// Get the current local position and acceleration.
			var localPosition = transform.localPosition;
			var acceleration = transform.InverseTransformDirection(_playerRefs.Physics.BikeAcceleration).x;
			var offset = 0f;

			// Calculate the offset if the acceleration exceeds the threshold.
			if (Mathf.Abs(acceleration) > accelerationThreshold)
			{
				var normalizedAcceleration = Mathf.Clamp(acceleration / maxAcceleration, -1f, 1f);
				offset = normalizedAcceleration * movementDistance;
			}

			// Lerp the position based on the offset and apply the movement. 
			localPosition.x = Mathf.Lerp(localPosition.x, _defaultPosition - offset, movementLerpSpeed * Time.fixedDeltaTime);
			transform.localPosition = localPosition;
		}

	}
}
