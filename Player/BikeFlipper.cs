using UnityEngine;

namespace Player
{
	using Managers;

	/// <summary>
	/// Manages the flipping behavior.
	/// </summary>
	[RequireComponent(typeof(BikeStates))]
	public class BikeFlipper : MonoBehaviour
	{
		[SerializeField] private float flipSpeed = 100f;

		private PlayerRefs _playerRefs;
		private BikeStates _bikeStates;
		private Transform _frameTransform;
		private Transform _ridersTransform;
		private bool _isFlipTransitionNeeded;


		#region /* UNITY LIFECYCLE */

		private void Awake()
		{
			_playerRefs = transform.root.GetComponent<PlayerRefs>();

			_bikeStates = _playerRefs.States;
			_frameTransform = _playerRefs.FrameTransform;
			_ridersTransform = _playerRefs.RidersTransform;
		}

		private void OnEnable() => SubscribeToEvents();

		private void OnDisable() => UnsubscribeFromEvents();
		
		private void Update()
		{
			if (GameManager.State != GameManager.GameState.Playing) 
				return;
			
			ApplyFlipTransition();
		}

		#endregion

		#region /* SUBSCRIPTION HANDLING */

		private void SubscribeToEvents() => _bikeStates.OnFlipStateChange += HandleFlipStateChange;

		private void UnsubscribeFromEvents() => _bikeStates.OnFlipStateChange -= HandleFlipStateChange;

		private void HandleFlipStateChange() => _isFlipTransitionNeeded = true;

		#endregion

		#region /* CUSTOM METHODS */

		/// <summary>
		/// Applies a flip transition to the bike frame and rider by adjusting their local scales on X-axis.
		/// </summary>
		private void ApplyFlipTransition()
		{
			// Check if the transition is needed.
			if (!_isFlipTransitionNeeded) return;

			// Allow other components to read whether this method is in progress or not.
			_bikeStates.IsFlipInTransition = true;
			
			// Get the current and target scales.
			var currentScale = _frameTransform.localScale.x;
			var targetScale = _bikeStates.IsFlipped ? -1 : 1;

			// Check if the transition is close enough to be finished.
			if (Mathf.Abs(currentScale - targetScale) < 0.01f)
			{
				// Finish the scaling process.
				currentScale = targetScale;
				_isFlipTransitionNeeded = false;
				_bikeStates.IsFlipInTransition = false;
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

		#endregion
	}
}