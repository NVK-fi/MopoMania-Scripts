using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace UI
{
	using System.Collections.Generic;
	using System.Linq;
	using Tools;
	using PlayerInput = Player.PlayerInput;

	/// <summary>
	/// Represents a menu input component.
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	public class MenuInput : MonoBehaviour
	{
		[field: SerializeField] public ButtonHighlight Highlight { get; private set; }
		[field: SerializeField] public ButtonBase InitialButton { get; set; }
		[field: SerializeField] public ButtonBase EscapeButton { get; private set; }
		[SerializeField] private float initialBrowsingDelay = 0.5f;
		[SerializeField] private float browsingDelay = 0.2f;
		[SerializeField] private AudioClip[] movementAudioClips;
		[SerializeField] private AudioClip[] selectionAudioClips;

		private ButtonBase _currentButton;
		private PlayerInput _playerInput;
		private Coroutine _keyHoldingCoroutine;
		private AudioSource _audioSource;

		private enum Direction
		{
			None,
			Up,
			Down,
			Left,
			Right
		}

		private Dictionary<Direction, int> _navigationKeysHeld = new()
		{
			{ Direction.Up, 0 },
			{ Direction.Down, 0 },
			{ Direction.Left, 0 },
			{ Direction.Right, 0 }
		};

		private void Awake()
		{
			_playerInput = new PlayerInput();
			
			this.IsReferenceNull(InitialButton);
			_currentButton = InitialButton;
			
			this.IsReferenceNull(EscapeButton);

			_audioSource = GetComponent<AudioSource>();
			
			Time.timeScale = 1;
		}

		private void OnEnable()
		{
			_playerInput.Enable();
			SubscribeToInputs();
		}

		private void OnDisable()
		{
			_playerInput.Disable();
			UnsubscribeFromInputs();
		}

		private void SubscribeToInputs()
		{
			_playerInput.Main.Up.started += OnUpStarted;
			_playerInput.Main.Up.canceled += OnUpCanceled;
			_playerInput.Main.Down.started += OnDownStarted;
			_playerInput.Main.Down.canceled += OnDownCanceled;
			_playerInput.Main.Left.started += OnLeftStarted;
			_playerInput.Main.Left.canceled += OnLeftCanceled;
			_playerInput.Main.Right.started += OnRightStarted;
			_playerInput.Main.Right.canceled += OnRightCanceled;
			_playerInput.Main.Use.started += OnUse;
			_playerInput.Main.Escape.started += OnEscape;
			_playerInput.Main.Restart.started += OnEscape;
		}

		private void UnsubscribeFromInputs()
		{
			_playerInput.Main.Up.started -= OnUpStarted;
			_playerInput.Main.Up.canceled -= OnUpCanceled;
			_playerInput.Main.Down.started -= OnDownStarted;
			_playerInput.Main.Down.canceled -= OnDownCanceled;
			_playerInput.Main.Left.started -= OnLeftStarted;
			_playerInput.Main.Left.canceled -= OnLeftCanceled;
			_playerInput.Main.Right.started -= OnRightStarted;
			_playerInput.Main.Right.canceled -= OnRightCanceled;
			_playerInput.Main.Use.started -= OnUse;
			_playerInput.Main.Escape.started -= OnEscape;
			_playerInput.Main.Restart.started -= OnEscape;
		}

		private void OnUpStarted(InputAction.CallbackContext _) => PressKey(Direction.Up);
		private void OnUpCanceled(InputAction.CallbackContext _) => ReleaseKey(Direction.Up);
		private void OnDownStarted(InputAction.CallbackContext _) => PressKey(Direction.Down);
		private void OnDownCanceled(InputAction.CallbackContext _) => ReleaseKey(Direction.Down);
		private void OnLeftStarted(InputAction.CallbackContext _) => PressKey(Direction.Left);
		private void OnLeftCanceled(InputAction.CallbackContext _) => ReleaseKey(Direction.Left);
		private void OnRightStarted(InputAction.CallbackContext _) => PressKey(Direction.Right);
		private void OnRightCanceled(InputAction.CallbackContext _) => ReleaseKey(Direction.Right);

		private void OnEscape(InputAction.CallbackContext _) => NavigateToButton(_currentButton == EscapeButton 
			? InitialButton 
			: EscapeButton);

		private void OnUse(InputAction.CallbackContext _)
		{
			_currentButton.ExecuteAction();

			if (_currentButton is not ButtonForSettings) return;
			_audioSource.SelectRandomClip(selectionAudioClips, true);
			_audioSource.Play();
		}

		/// <summary>
		/// Initiates keypress tracking and navigation for the specified direction.
		/// </summary>
		/// <param name="direction">The direction for which the key was pressed.</param>
		private void PressKey(Direction direction)
		{
			_navigationKeysHeld[direction]++;
			NavigateWithDirection(direction);

			if (_keyHoldingCoroutine != null) StopCoroutine(_keyHoldingCoroutine);
			_keyHoldingCoroutine = StartCoroutine(KeyHoldingCoroutine());
		}

		/// <summary>
		/// Stops the keypress for the specified direction.
		/// Other keys can still be held while the current key releases.
		/// </summary>
		/// <param name="direction">The direction for which the key was released.</param>
		private void ReleaseKey(Direction direction)
		{
			_navigationKeysHeld[direction]--;

			if (_navigationKeysHeld[direction] < 0) _navigationKeysHeld[direction] = 0;

			if (_keyHoldingCoroutine == null || _navigationKeysHeld.Any(pair => pair.Value > 0)) return;

			// Stop the coroutine if no more keys are held.
			StopCoroutine(_keyHoldingCoroutine);
			_keyHoldingCoroutine = null;
		}

		/// <summary>
		/// The coroutine that handles key-holding and navigation after an initial delay.
		/// </summary>
		/// <returns>IEnumerator for coroutine execution.</returns>
		private IEnumerator KeyHoldingCoroutine()
		{
			yield return new WaitForSeconds(initialBrowsingDelay);

			var continueLooping = true;
			while (continueLooping)
			{
				var sumX = -_navigationKeysHeld[Direction.Left] + _navigationKeysHeld[Direction.Right];
				var sumY = -_navigationKeysHeld[Direction.Down] + _navigationKeysHeld[Direction.Up];

				// Stop the loop if there's nowhere to navigate to.
				continueLooping = 
					NavigateWithDirection(GetDirection(sumX, 0)) 
					|| NavigateWithDirection(GetDirection(0, sumY));

				yield return new WaitForSeconds(browsingDelay);
			}
		}

		/// <summary>
		/// Navigates to the set button directly.
		/// </summary>
		public void NavigateToButton(ButtonBase button)
		{
			Highlight.Highlight(button);
			_currentButton = button;

			_audioSource.SelectRandomClip(movementAudioClips, true);
			_audioSource.Play();
		}
		
		/// <summary>
		/// Navigates to the next button in the specified direction if possible and updates the backtrack reference.
		/// </summary>
		/// <param name="direction">The direction in which to navigate (Up, Down, Left, Right).</param>
		private bool NavigateWithDirection(Direction direction)
		{
			var nextButton = GetButtonFromDirection(direction);
			if (!nextButton) return false;
			
			UpdateBacktrackReference(nextButton, direction);
			Highlight.Highlight(nextButton);
			_currentButton = nextButton;
			
			_audioSource.SelectRandomClip(movementAudioClips, true);
			_audioSource.Play();
			
			return true;
		}

		/// <summary>
		/// Determines the navigation direction based on the sum of horizontal and vertical keypresses.
		/// Vertical navigation is always prioritized.
		/// </summary>
		/// <param name="sumX">The sum of horizontal keypresses (-1 for left, 0 for none, 1 for right).</param>
		/// <param name="sumY">The sum of vertical keypresses (-1 for down, 0 for none, 1 for up).</param>
		/// <returns>The calculated navigation direction.</returns>
		private static Direction GetDirection(int sumX, int sumY)
		{
			if (sumY > 0)
				return Direction.Up;
			if (sumY < 0)
				return Direction.Down;
			if (sumX < 0)
				return Direction.Left;
			if (sumX > 0)
				return Direction.Right;
			return Direction.None;
		}

		/// <summary>
		/// Gets the next button to navigate to based on the current direction.
		/// </summary>
		/// <param name="direction">The direction in which navigation is happening.</param>
		/// <returns>The next button to navigate to.</returns>
		private ButtonBase GetButtonFromDirection(Direction direction)
		{
			return direction switch
			{
				Direction.Up => _currentButton.ButtonAbove,
				Direction.Down => _currentButton.ButtonBelow,
				Direction.Left => _currentButton.ButtonOnLeft,
				Direction.Right => _currentButton.ButtonOnRight,
				_ => null
			};
		}

		/// <summary>
		/// Updates the reference for backtracking on the next button's side based on the current direction.
		/// </summary>
		/// <param name="nextButton">The next button to navigate to.</param>
		/// <param name="direction">The direction in which navigation is happening.</param>
		private void UpdateBacktrackReference(ButtonBase nextButton, Direction direction)
		{
			switch (direction)
			{
				case Direction.Up:
					nextButton.ButtonBelow = _currentButton;
					break;
				case Direction.Down:
					nextButton.ButtonAbove = _currentButton;
					break;
				case Direction.Left:
					nextButton.ButtonOnRight = _currentButton;
					break;
				case Direction.Right:
					nextButton.ButtonOnLeft = _currentButton;
					break;
			}
		}
	}
}
