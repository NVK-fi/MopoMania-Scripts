using UnityEngine;

namespace UI
{
	using Managers;
	using UnityEngine.InputSystem;
	using UnityEngine.SceneManagement;
	using PlayerInput = Player.PlayerInput;

	/// <summary>
	/// Listens to player input and changes or restarts the scene if necessary.
	/// </summary>
	public class EscapeInput : MonoBehaviour
	{
		private PlayerInput _playerInput;

		private void Awake()
		{
			_playerInput = new PlayerInput();
		}

		private void OnEnable()
		{
			_playerInput.Enable();
			_playerInput.Main.Use.started += OnUse;
			_playerInput.Main.Flip.started += OnUse;
			_playerInput.Main.Escape.started += OnEscape;
			_playerInput.Main.Restart.started += OnRestart;
		}

		private void OnDisable()
		{
			_playerInput.Disable();
			_playerInput.Main.Use.started -= OnUse;
			_playerInput.Main.Flip.started -= OnUse;
			_playerInput.Main.Escape.started -= OnEscape;
			_playerInput.Main.Restart.started -= OnRestart;
		}

		private void OnUse(InputAction.CallbackContext _)
		{
			if (GameManager.State == GameManager.GameState.Finished)
				GameManager.LoadScene("MainMenu");
		}

		private void OnEscape(InputAction.CallbackContext _)
		{
			GameManager.LoadScene("MainMenu");
		}

		private void OnRestart(InputAction.CallbackContext _)
		{
			GameManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}
}