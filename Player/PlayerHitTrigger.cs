using Managers;
using UnityEngine;

namespace Player
{
	using Tools;

	/// <summary>
	/// Triggers a fail state when the player hits specific colliders in the game.
	/// </summary>
	[RequireComponent(typeof(Collider2D))]
	public class PlayerHitTrigger : MonoBehaviour
	{
		[SerializeField] private AudioSource hitAudioSource;
		[SerializeField] private float minPitch = .9f;
		[SerializeField] private float maxPitch = 1.2f;

		private void Start()
		{
			this.IsReferenceNull(hitAudioSource);

			var attachedCollider = GetComponent<Collider2D>();
			
			if (!attachedCollider.isTrigger) 
				Debug.LogWarning("The " + attachedCollider.GetType().Name + " at " + name + " should be a trigger for " + GetType().Name + " script to work.");
		}

		private void OnEnable() => GameManager.Instance.OnGameStateChange += GameStateChanged;
		private void OnDisable() => GameManager.Instance.OnGameStateChange -= GameStateChanged;

		private void GameStateChanged() 
			=> GetComponent<Collider2D>().isTrigger = GameManager.State != GameManager.GameState.Finished;

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("LevelCollider"))
			{
				if (GameManager.State == GameManager.GameState.Playing)
					LevelManager.Instance.Fail();
				
				hitAudioSource.PlayWithRandomPitch(minPitch, maxPitch);
			}
			else if (other.CompareTag("LevelBounds") && GameManager.State == GameManager.GameState.Playing)
			{
				LevelManager.Instance.Fail();
			}
		}
	}
}
