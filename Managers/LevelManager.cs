using UnityEngine;

namespace Managers
{
	using System;
	using System.Collections;
	using System.Linq;
	using Items;
	using Player;
	using ScriptableObjects;
	using Tools;
	using UI;

	public class LevelManager : MonoBehaviour
	{
		/// <summary>
		/// The instance of LevelManager singleton.
		/// </summary>
		public static LevelManager Instance;

		/// <summary>
		/// The LevelData scriptable object for the level.
		/// </summary>
		[field: SerializeField] public LevelData Data { get; private set; }
		
		/// <summary>
		/// The Player GameObject.
		/// </summary>
		[field: SerializeField] public PlayerRefs Player { get; private set; }
		
		/// <summary>
		/// The Timer component.
		/// </summary>
		[field: SerializeField] public LevelTimer LevelTimer { get; private set; }
		
		/// <summary>
		/// The currently activated Checkpoint. Can be null.
		/// </summary>
		[field: SerializeField] public Checkpoint CurrentCheckpoint { get; private set; }

		public static event Action OnPlayerFail;
		public static event Action OnPlayerRespawn;

		#region /* UNITY LIFECYCLE */
		private void Awake()
		{
			// Singleton
			if (Instance != null) Destroy(gameObject);
			else Instance = this;

			GameManager.Instance.SetState(GameManager.GameState.PreStart);
			Time.timeScale = 1;
			
			this.IsReferenceNull(Player);
			this.IsReferenceNull(CurrentCheckpoint);
			
			var eggsInScene = FindObjectsOfType<Egg>().ToList();
			EggManager.SetupEggsFromPlayerPrefs(eggsInScene);
		}

		#endregion

		#region /* CUSTOM METHODS */

		/// <summary>
		/// Attempts to start the level if it's in the pre-start state.
		/// </summary>
		/// <returns>True if the level is started successfully, false otherwise.</returns>
		public bool TryStartLevel()
		{
			if (GameManager.State != GameManager.GameState.PreStart) 
				return false;

			Player.BikeRigidbody.constraints = RigidbodyConstraints2D.None;
			
			GameManager.Instance.SetState(GameManager.GameState.Playing);
			
			CurrentCheckpoint.Toggle();
			return true;
		}

		/// <summary>
		/// Attempts to respawn the player if the game state is failed.
		/// </summary>
		/// <returns>True if the player is respawned successfully, false otherwise.</returns>

		public bool TryRespawn()
		{
			if (GameManager.State != GameManager.GameState.Failed) 
				return false;
			
			Player.Physics.ResetPhysics();
			Player.transform.rotation = CurrentCheckpoint.RespawnPoint.rotation;
			Player.transform.position = CurrentCheckpoint.RespawnPoint.position;
			
			EggManager.ClearEggs();

			OnPlayerRespawn?.Invoke();
			
			Time.timeScale = 1;
			GameManager.Instance.SetState(GameManager.GameState.Playing);
			
			return true;
		}

		/// <summary>
		/// Marks the level as failed, stopping the game and triggering appropriate events.
		/// </summary>
		public void Fail()
		{
			GameManager.Instance.SetState(GameManager.GameState.Failed);
			Time.timeScale = 0;
			OnPlayerFail?.Invoke();
		}
		
		/// <summary>
		/// Marks the level as finished, saving progress and triggering appropriate events.
		/// </summary>
		public void Finish()
		{
			if (GameManager.State == GameManager.GameState.Finished) return;

			EggManager.AddCollectedEggsToPreserve();
			EggManager.SavePreservedEggsToPlayerPrefs();
			
			GameManager.Instance.SetState(GameManager.GameState.Finished);
			UpdateProgressToPlayerPrefs();
			StartCoroutine(FinishSlowMotionCoroutine());
		}

		/// <summary>
		/// Updates the player progress in PlayerPrefs based on level completion and best time.
		/// </summary>
		private void UpdateProgressToPlayerPrefs()
		{
			// The player beat the latest level.
			if (Data.levelNumber >= PlayerPrefs.GetInt("Levels_Progress", 1))
			{
				var progress = Data.levelNumber + 1;
				PlayerPrefs.SetInt("Levels_Progress", progress);
				PlayerPrefs.SetInt("Levels_LastPlayed", progress);
				PlayerPrefs.Save();
			}
			
			// The player beat their personal record.
			var time = LevelTimer.PlayTime;
			if (time < PlayerPrefs.GetFloat(Data.name + "_BestTime", float.MaxValue))
			{
				PlayerPrefs.SetFloat(Data.name + "_BestTime", time);
				PlayerPrefs.Save();
			}
		}

		/// <summary>
		/// A coroutine for slow-motion effect when finishing a level.
		/// </summary>
		private static IEnumerator FinishSlowMotionCoroutine()
		{
			Time.timeScale = 0.75f;
			yield return new WaitForSecondsRealtime(0.5f);
			Time.timeScale = 0;
		}

		/// <summary>
		/// Changes the current Checkpoint and triggers appropriate events.
		/// </summary>
		/// <param name="checkpoint">The Checkpoint to be activated.</param>
		public void ActivateCheckpoint(Checkpoint checkpoint)
		{
			EggManager.AddCollectedEggsToPreserve();
			
			// Ignore the rest, if trying to assign the same Checkpoint twice.
			if (CurrentCheckpoint == checkpoint) return;
			
			// Deactivate the old visuals.
			if (CurrentCheckpoint != null) 
				CurrentCheckpoint.Toggle(false, false);
			
			// Activate the new one.
			CurrentCheckpoint = checkpoint;
			CurrentCheckpoint!.Toggle();
		}
		
		#endregion
	}
}
