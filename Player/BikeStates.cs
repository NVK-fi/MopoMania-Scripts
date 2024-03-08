namespace Player
{
	using System;
	using Managers;
	using UnityEngine;
	using UnityEngine.InputSystem;

	/// <summary>
	/// Manages and exposes various states of the bike such as throttling, braking, and flipping.
	/// It is executed after PlayerRefs and PlayerPhysics but before any other bike-related script.
	/// </summary>
	public class BikeStates : MonoBehaviour
	{
		/// <summary>
		/// Enumerates the different driving states the bike can be in.
		/// </summary>
		public enum DrivingStates
		{
			Brake = -1,
			Idle = 0,
			Throttle = 1
		}

		/// <summary>
		/// Enumerates the different rotation states the bike can be in.
		/// </summary>
		public enum RotatingStates
		{
			AntiClockwise = -1,
			Neither = 0,
			Clockwise = 1
		}

		/// <summary>
		/// Gets the driving state of the bike. When changed, it triggers the DrivingStateChanged event.
		/// </summary>
		public DrivingStates DrivingState
		{
			get => _drivingState;
			private set
			{
				if (_drivingState == value) return;

				_drivingState = value;
				
				OnDrivingStateChange?.Invoke();

				if (_drivingState == DrivingStates.Throttle) 
					OnThrottleKeyDown?.Invoke();
			}
		}

		/// <summary>
		/// Gets the rotation state of the bike. It triggers the RotatingStateChanged event. 
		/// </summary>
		public RotatingStates RotatingState
		{
			get => _rotatingState;
			private set
			{
				if (_rotatingState == value) return;

				_rotatingState = value;
				OnRotatingStateChange?.Invoke();
			}
		}

		/// <summary>
		/// Gets the binary flipped state of the bike. When changed, it triggers the FlipStateChanged event.
		/// </summary>
		public bool IsFlipped
		{
			get => _isFlipped;
			private set
			{
				if (_isFlipped == value) return;

				_isFlipped = value;
				OnFlipStateChange?.Invoke();
			}
		}

		/// <summary>
		/// True if the bike is currently transitioning from one direction to another visually.
		/// When set to false, meaning the transition is finished, it invokes an event.
		/// </summary>
		public bool IsFlipInTransition
		{
			get => _isFlipInTransition;
			set
			{
				if (_isFlipInTransition == value) return;
				
				_isFlipInTransition = value;
				
				// Invoke an event when the transition is set as finished.
				if (_isFlipInTransition == false) OnFlipTransitionFinish?.Invoke();
			}
		}

		/// <summary>
		/// The event triggered each time the throttle key is pressed down.
		/// </summary>
		public event Action OnThrottleKeyDown;
		
		/// <summary>
		/// The event triggered when the bike changes its state of throttling or braking.
		/// </summary>
		public event Action OnDrivingStateChange;

		/// <summary>
		/// The event triggered when the bike's rotating state changes.
		/// </summary>
		public event Action OnRotatingStateChange;

		/// <summary>
		/// Event triggered when the bike's flip state changes.
		/// </summary>
		public event Action OnFlipStateChange;
		
		/// <summary>
		/// Event triggered when the bike's flip transition is finished.
		/// </summary>
		public event Action OnFlipTransitionFinish;

		private int _controlScheme;
		private PlayerInput _playerInput;
		private DrivingStates _drivingState;
		private RotatingStates _rotatingState;
		private bool _isTryingToThrottle;
		private bool _isTryingToBrake;
		private bool _isTryingToRotateLeft;
		private bool _isTryingToRotateRight;
		private bool _isFlipped;
		private bool _isFlipInTransition;

		#region /* UNITY LIFECYCLE */

		private void Awake()
		{
			_playerInput = new PlayerInput();
		}
		
		private void OnEnable()
		{
			_playerInput.Enable();
			SubscribeToEvents();
			UpdateInputSetting();
		}

		private void OnDisable()
		{
			_playerInput.Disable();
			UnsubscribeFromEvents();

			OnDrivingStateChange = null;
			OnRotatingStateChange = null;
			OnFlipStateChange = null;
			OnFlipTransitionFinish = null;
		}

		#endregion

		#region /* SUBSCRIPTION HANDLING */

		private void SubscribeToEvents()
		{
			_playerInput.Main.Up.started += OnUpStart;
			_playerInput.Main.Up.canceled += OnUpCancel;
			_playerInput.Main.Down.started += OnDownStart;
			_playerInput.Main.Down.canceled += OnDownCancel;
			_playerInput.Main.Right.started += OnRightStart;
			_playerInput.Main.Right.canceled += OnRightCancel;
			_playerInput.Main.Left.started += OnLeftStart;
			_playerInput.Main.Left.canceled += OnLeftCancel;
			_playerInput.Main.Flip.started += OnFlip;
			_playerInput.Main.Use.started += OnFlip;
		}

		private void UnsubscribeFromEvents()
		{
			_playerInput.Main.Up.started -= OnUpStart;
			_playerInput.Main.Up.canceled -= OnUpCancel;
			_playerInput.Main.Down.started -= OnDownStart;
			_playerInput.Main.Down.canceled -= OnDownCancel;
			_playerInput.Main.Right.started -= OnRightStart;
			_playerInput.Main.Right.canceled -= OnRightCancel;
			_playerInput.Main.Left.started -= OnLeftStart;
			_playerInput.Main.Left.canceled -= OnLeftCancel;
			_playerInput.Main.Flip.started -= OnFlip;
			_playerInput.Main.Use.started -= OnFlip;
		}

		private void OnUpStart(InputAction.CallbackContext context)
		{
			LevelManager.Instance.TryStartLevel();
			
			switch (_controlScheme)
			{
				case 0:
				case 1:
					ComputeDrivingInput(DrivingStates.Throttle, true);
					break;
				case 2:
					ComputeRotatingInput(!IsFlipped ? RotatingStates.AntiClockwise : RotatingStates.Clockwise, true);
					break;
				case 3:
					ComputeRotatingInput(!IsFlipped ? RotatingStates.Clockwise : RotatingStates.AntiClockwise, true);
					break;
			}
		}

		private void OnUpCancel(InputAction.CallbackContext context)
		{
			switch (_controlScheme)
			{
				case 0:
				case 1:
					ComputeDrivingInput(DrivingStates.Throttle, false);
					break;
				case 2:
					ComputeRotatingInput(!IsFlipped ? RotatingStates.AntiClockwise : RotatingStates.Clockwise, false);
					break;
				case 3:
					ComputeRotatingInput(!IsFlipped ? RotatingStates.Clockwise : RotatingStates.AntiClockwise, false);
					break;
			}
		}

		private void OnDownStart(InputAction.CallbackContext context)
		{
			LevelManager.Instance.TryStartLevel();
			
			switch (_controlScheme)
			{
				case 0:
				case 1:
					ComputeDrivingInput(DrivingStates.Brake, true);
					break;
				case 2:
					ComputeRotatingInput(!IsFlipped ? RotatingStates.Clockwise : RotatingStates.AntiClockwise, true);
					break;
				case 3:
					ComputeRotatingInput(!IsFlipped ? RotatingStates.AntiClockwise : RotatingStates.Clockwise, true);
					break;
			}
		}

		private void OnDownCancel(InputAction.CallbackContext context)
		{
			switch (_controlScheme)
			{
				case 0:
				case 1:
					ComputeDrivingInput(DrivingStates.Brake, false);
					break;
				case 2:
					ComputeRotatingInput(!IsFlipped ? RotatingStates.Clockwise : RotatingStates.AntiClockwise, false);
					break;
				case 3:
					ComputeRotatingInput(!IsFlipped ? RotatingStates.AntiClockwise : RotatingStates.Clockwise, false);
					break;
			}
		}

		private void OnLeftStart(InputAction.CallbackContext context)
		{
			LevelManager.Instance.TryStartLevel();
			
			switch (_controlScheme)
			{
				case 0:
					ComputeRotatingInput(RotatingStates.AntiClockwise, true);
					break;
				case 1:
					ComputeRotatingInput(RotatingStates.Clockwise, true);
					break;
				case 2:
				case 3:
					ComputeDrivingInput(!IsFlipped ? DrivingStates.Brake : DrivingStates.Throttle, true);
					break;
			}
		}

		private void OnLeftCancel(InputAction.CallbackContext context)
		{
			switch (_controlScheme)
			{
				case 0:
					ComputeRotatingInput(RotatingStates.AntiClockwise, false);
					break;
				case 1:
					ComputeRotatingInput(RotatingStates.Clockwise, false);
					break;
				case 2:
				case 3:
					ComputeDrivingInput(!IsFlipped ? DrivingStates.Brake : DrivingStates.Throttle, false);
					break;
			}
		}

		private void OnRightStart(InputAction.CallbackContext context)
		{
			LevelManager.Instance.TryStartLevel();

			switch (_controlScheme)
			{
				case 0:
					ComputeRotatingInput(RotatingStates.Clockwise, true);
					break;
				case 1:
					ComputeRotatingInput(RotatingStates.AntiClockwise, true);
					break;
				case 2:
				case 3:
					ComputeDrivingInput(!IsFlipped ? DrivingStates.Throttle : DrivingStates.Brake, true);
					break;
			}
		}

		private void OnRightCancel(InputAction.CallbackContext context)
		{
			switch (_controlScheme)
			{
				case 0:
					ComputeRotatingInput(RotatingStates.Clockwise, false);
					break;
				case 1:
					ComputeRotatingInput(RotatingStates.AntiClockwise, false);
					break;
				case 2:
				case 3:
					ComputeDrivingInput(!IsFlipped ? DrivingStates.Throttle : DrivingStates.Brake, false);
					break;
			}
		}

		private void OnFlip(InputAction.CallbackContext context)
		{
			if (LevelManager.Instance.TryRespawn())
				return;
			
			LevelManager.Instance.TryStartLevel();
			
			IsFlipped = !IsFlipped;
			
			// Flip the current inputs if the bike controls flip.
			if (_controlScheme is 2 or 3) 
				FlipDrivingAndRotatingStates();
		}

		#endregion

		#region /* CUSTOM METHODS */

		/// <summary>
		/// Flip the driving and rotating inputs and states.
		/// For example, braking becomes throttling.
		/// </summary>
		private void FlipDrivingAndRotatingStates()
		{
			(_isTryingToThrottle, _isTryingToBrake) = (_isTryingToBrake, _isTryingToThrottle);
			
			DrivingState = DrivingState switch
			{
				DrivingStates.Brake => DrivingStates.Throttle,
				DrivingStates.Throttle => DrivingStates.Brake,
				_ => DrivingStates.Idle
			};

			(_isTryingToRotateLeft, _isTryingToRotateRight) = (_isTryingToRotateRight, _isTryingToRotateLeft);
			
			RotatingState = RotatingState switch
			{
				RotatingStates.Clockwise => RotatingStates.AntiClockwise,
				RotatingStates.AntiClockwise => RotatingStates.Clockwise,
				_ => RotatingStates.Neither
			};
		}

		/// <summary>
		/// Computes and sets the driving state based on user input.
		/// </summary>
		/// <param name="input">The driving input (throttle/brake).</param>
		/// <param name="isPressed">Indicates if the input is started or canceled.</param>
		private void ComputeDrivingInput(DrivingStates input, bool isPressed)
		{
			switch (input)
			{
				case DrivingStates.Brake:
					_isTryingToBrake = isPressed;
					break;
				case DrivingStates.Throttle:
					_isTryingToThrottle = isPressed;
					break;
			}

			if (_isTryingToBrake) DrivingState = DrivingStates.Brake;
			else if (_isTryingToThrottle) DrivingState = DrivingStates.Throttle;
			else DrivingState = DrivingStates.Idle;
		}

		/// <summary>
		/// Computes and sets the rotating state based on user input.
		/// </summary>
		/// <param name="input">The rotating input (left/right).</param>
		/// <param name="isPressed">Indicates if the input is started or canceled.</param>
		private void ComputeRotatingInput(RotatingStates input, bool isPressed)
		{
			switch (input)
			{
				case RotatingStates.AntiClockwise:
					_isTryingToRotateLeft = isPressed;
					break;
				case RotatingStates.Clockwise:
					_isTryingToRotateRight = isPressed;
					break;
			}

			if (_isTryingToRotateLeft && !_isTryingToRotateRight) RotatingState = RotatingStates.AntiClockwise;
			else if (!_isTryingToRotateLeft && _isTryingToRotateRight) RotatingState = RotatingStates.Clockwise;
			else RotatingState = RotatingStates.Neither;
		}

		/// <summary>
		/// Gets the setting's value from PlayerPrefs and returns a validated version of it.
		/// </summary>
		/// <returns>The validated setting value.</returns>
		private void UpdateInputSetting()
		{
			if (!PlayerPrefs.HasKey("Settings_Input"))
			{
				_controlScheme = 0;
				return;
			}
			
			var value = PlayerPrefs.GetInt("Settings_Input");
			_controlScheme = value is < 0 or >= 4 ? 0 : value;
		}

		#endregion
	}
}