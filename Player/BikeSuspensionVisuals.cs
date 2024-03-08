namespace Player
{
	using Managers;
	using UnityEngine;

	/// <summary>
	/// Manages the visuals of the bike suspension based on player input and game state.
	/// </summary>
	public class BikeSuspensionVisuals : MonoBehaviour
	{
		[SerializeField] private Suspension suspensionSide;
		[SerializeField] private float defaultAngle;
		[SerializeField] private float defaultScale;
		[SerializeField] private float spriteLength;

		private enum Suspension {Front, Rear}
		private PlayerRefs _playerRefs;
		private Rigidbody2D _targetTire;
		
		#region /* UNITY LIFECYCLE */
		
		private void Awake()
		{
			_playerRefs = transform.root.GetComponent<PlayerRefs>();
		}

		private void Start()
		{
			UpdateTargetTire();
		}

		private void OnEnable()
		{
			_playerRefs.States.OnFlipTransitionFinish += UpdateTargetTire;
			LevelManager.OnPlayerFail += ApplyValues;
			LevelManager.OnPlayerRespawn += ApplyDefaultValues;
		}

		private void OnDisable()
		{
			_playerRefs.States.OnFlipTransitionFinish -= UpdateTargetTire;
			LevelManager.OnPlayerFail -= ApplyValues;
			LevelManager.OnPlayerRespawn -= ApplyDefaultValues;
		}

		private void FixedUpdate() // These sprites only need an update when the rigid-bodies have been updated.
		{
			ApplyValues();
		}

		#endregion

		#region /* CUSTOM METHODS */

		private void ApplyValues()
		{
			ApplyRotation();
			ApplyScale();
		}

		private void ApplyDefaultValues()
		{
			transform.localRotation = Quaternion.Euler(0f, 0f, defaultAngle);
			transform.localScale = suspensionSide == Suspension.Front 
				? new Vector3(1f, defaultScale, 1f) 
				: new Vector3(defaultScale, 1f, 1f);
		}
		
		/// <summary>
		/// Updates the target tire depending on which suspension is using this script.
		/// </summary>
		private void UpdateTargetTire()
		{
			_targetTire = suspensionSide == Suspension.Front 
				? _playerRefs.FrontTireRigidbody
				: _playerRefs.RearTireRigidbody;
		}
		
		/// <summary>
		/// Applies the rotation to the suspension sprite based on the relative position to the tire.
		/// Uses a default angle when the bike is flipping, otherwise calculates the required angle.
		/// </summary>
		private void ApplyRotation()
		{
			 var rotation = _playerRefs.States.IsFlipInTransition 
					? defaultAngle 
					: CalculateAngle();

			 transform.localRotation = Quaternion.Euler(0f, 0f, rotation);
		}

		/// <summary>
		/// Adjusts the scale of the suspension sprite based on its distance to the tire.
		/// Uses a default scale when the bike is flipping, otherwise calculates the required scale.
		/// </summary>
		private void ApplyScale()
		{
			var scale = _playerRefs.States.IsFlipInTransition
				? defaultScale
				: CalculateSuspensionScale();

			transform.localScale = suspensionSide == Suspension.Front 
				? new Vector3(1f, scale, 1f) 
				: new Vector3(scale, 1f, 1f);
		}
		
		/// <summary>
		/// Calculates the rotation required for the suspension to look towards the tire in Bike's space.
		/// </summary>
		/// <returns>The angle for the sprite.</returns>
		private float CalculateAngle()
		{
			// Get the direction from the suspension origin to the tire in world space.
			var worldDirection = _targetTire.position - (Vector2)transform.position;
    
			// Convert the world direction to Bike's local space.
			var localDirection = transform.root.InverseTransformDirection(worldDirection);

			float angle;
			
			// Calculate the angle taking into account if the bike is flipped. 
			if (suspensionSide == Suspension.Front)
			{
				angle = _playerRefs.States.IsFlipped 
					? Vector3.SignedAngle(Vector3.down, localDirection, Vector3.back)
					: Vector3.SignedAngle(Vector3.down, localDirection, Vector3.forward);
			}
			else
			{
				angle = _playerRefs.States.IsFlipped 
					? Vector3.SignedAngle(Vector3.right, localDirection, Vector3.back)
					: Vector3.SignedAngle(Vector3.left, localDirection, Vector3.forward);
			}

			return angle;
		}

		/// <summary>
		/// Calculates the scale required for the suspension to be the right length.
		/// </summary>
		/// <returns>A scale factor for the axis.</returns>
		private float CalculateSuspensionScale()
		{
			// Get the distance from origin to the tire.
			var distanceToTire = Vector3.Distance(transform.position, _targetTire.position);
			
			// Calculate the scale.
			return distanceToTire / spriteLength;
		}

		#endregion
	}
}
