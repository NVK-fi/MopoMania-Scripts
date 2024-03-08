using UnityEngine;

namespace Items
{
	using Managers;
	using Tools;

	/// <summary>
	/// Represents a finish line in the game.
	/// Triggers victory conditions and effects when the player crosses it.
	/// </summary>
	public class FinishLine : MonoBehaviour
	{
		[SerializeField] private AudioSource flagAudioSource;
		[SerializeField] private AudioSource confettiAudioSource;
		[SerializeField] private ParticleSystem confettiParticleSystem;
		private bool _isPlayingEffects;

		private void Awake()
		{
			this.IsReferenceNull(flagAudioSource);
			this.IsReferenceNull(confettiAudioSource);
			this.IsReferenceNull(confettiParticleSystem);
		}

		private void OnEnable() => GameManager.Instance.OnGameStateChange += OnGameStateChange;
		private void OnDisable() => GameManager.Instance.OnGameStateChange -= OnGameStateChange;

		private void OnGameStateChange()
		{
			// Adjust flag audio volume based on game state.
			flagAudioSource.volume = GameManager.State is GameManager.GameState.Playing or GameManager.GameState.PreStart
				? 1f 
				: 0f;
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			// Check if the triggering object is the player.
			if (!other.CompareTag("Bike") && !other.CompareTag("Human") && !other.CompareTag("Fowl")) return;
			
			LevelManager.Instance.Finish();

			// Play victory effects if not already playing.
			if (_isPlayingEffects) return;
			_isPlayingEffects = true;
			confettiParticleSystem.Play();
			confettiAudioSource.Play();
		}
	}
}
