using UnityEngine;

namespace Managers
{
	using System;
	using Tools;
	using UnityEngine.SceneManagement;

	public class GameManager : MonoBehaviour
	{
		/// <summary>
		/// Singleton instance of the GameManager class.
		/// </summary>
		public static GameManager Instance;

		/// <summary>
		/// Enumeration representing different game states.
		/// </summary>
		public enum GameState { PreStart, Playing, Failed, Finished }
		
		/// <summary>
		/// Represents the current game state.
		/// </summary>
		public static GameState State { get; private set; }
		
		/// <summary>
		/// An event triggered when the game state changes.
		/// </summary>
		public event Action OnGameStateChange;
	
		private void Awake()
		{
			// Singleton
			if (Instance != null) Destroy(gameObject);
			else
			{
				Instance = this;
				DontDestroyOnLoad(Instance);
				
				GC.Collect();
			}

			GameSettings.Initialize();
		}

		/// <summary>
		/// Sets the current game state and triggers the OnGameStateChange event.
		/// </summary>
		/// <param name="state">The new game state to set.</param>
		public void SetState(GameState state)
		{
			if (state == State) return;
			
			State = state;
			OnGameStateChange?.Invoke();
		}

		/// <summary>
		/// Loads a scene by name.
		/// </summary>
		/// <param name="sceneName">Name of the scene to load.</param>
		public static void LoadScene(string sceneName)
		{
			SceneManager.LoadScene(sceneName);
		}
	}
}
