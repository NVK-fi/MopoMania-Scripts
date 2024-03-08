using UnityEngine;

namespace Player
{
	using UnityEngine.InputSystem;

	/// <summary>
	/// Manages the flipping behaviour in the main menu.
	/// Replaces BikeFlipper.
	/// </summary>
	[RequireComponent(typeof(PlayerRefs))]
	public class BikeFlipperMainMenu : MonoBehaviour
	{
		[SerializeField] private float flipSpeed = 12f;

		private bool _isFlipped;
		private bool _isFlipTransitionNeeded;
		
		private Transform _frameTransform;
		private Transform _ridersTransform;
		private PlayerInput _playerInput;
		private PlayerRefs _playerRefs;
		
		private void Awake()
		{
			_playerInput = new PlayerInput();
			_playerRefs = GetComponent<PlayerRefs>();
			_frameTransform = _playerRefs.FrameTransform;
			_ridersTransform = _playerRefs.RidersTransform;
		}

		private void OnEnable()
		{
			_playerInput.Enable();
			_playerInput.Main.Flip.started += OnFlip;
		}

		private void OnDisable()
		{
			_playerInput.Disable();
			_playerInput.Main.Flip.started -= OnFlip;
		}

		private void OnFlip(InputAction.CallbackContext _)
		{
			_isFlipTransitionNeeded = true;
			_isFlipped = !_isFlipped;
		}

		private void Update()
		{
			// Check if the transition is needed.
			if (!_isFlipTransitionNeeded) return;
			
			ApplyFlipTransition();
		}

		/// <summary>
		/// Applies a flip transition to the bike frame and rider by adjusting their local scales on X-axis.
		/// </summary>
		private void ApplyFlipTransition()
		{
			// Get the current and target scales.
			var currentScale = _frameTransform.localScale.x;
			var targetScale = _isFlipped ? -1 : 1;

			// Check if the transition is close enough to be finished.
			if (Mathf.Abs(currentScale - targetScale) < 0.01f)
			{
				// Finish the scaling process.
				currentScale = targetScale;
				_isFlipTransitionNeeded = false;
			}
			else
			{
				// Transition towards the target scale in small (flipSpeed) increments.
				currentScale = Mathf.MoveTowards(currentScale, targetScale,
					flipSpeed * Time.deltaTime);
			}

			// Apply the scaling.
			_frameTransform.localScale = new Vector3(currentScale, 1, 1);
			_ridersTransform.localScale = new Vector3(currentScale, 1, 1);
		}
	}
}