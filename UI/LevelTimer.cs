using UnityEngine;

namespace UI
{
	using Managers;

	/// <summary>
	/// Tracks the elapsed time during gameplay.
	/// </summary>
	public class LevelTimer : MonoBehaviour
	{
		[field:SerializeField] public float PlayTime { get; private set; } = 0f;

		private const float MaxTime = 599.999f;
		private GameManager _gameManager;
		private bool _isPlaying;

		private void Awake()
		{
			_gameManager = GameManager.Instance;
		}

		private void OnEnable() => _gameManager.OnGameStateChange += OnStateChange;

		private void OnDisable() => _gameManager.OnGameStateChange -= OnStateChange;

		/// <summary>
		/// Handles the change in game state.
		/// </summary>
		private void OnStateChange() => _isPlaying = GameManager.State == GameManager.GameState.Playing;

		private void Update()
		{
			if (!_isPlaying) return;
			
			PlayTime += Time.deltaTime;

			if (PlayTime < MaxTime) return;
			PlayTime = MaxTime;
			enabled = false;
		}
	}
}
