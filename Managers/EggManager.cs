namespace Managers
{
	using System;
	using System.Collections.Generic;
	using Items;
	using UnityEngine;
	using UnityEngine.SceneManagement;

	public static class EggManager
	{
		/// <summary>
		/// An event raised when an Egg is collected by the player.
		/// </summary>
		public static Action OnEggCollected;
		
		/// <summary>
		/// A list of collected Eggs to be saved into the EggsPreserved list.
		/// </summary>
		public static List<Egg> EggsCollected { get; } = new();
		
		/// <summary>
		/// A list of saved Eggs. The Eggs are saved at Checkpoints.
		/// </summary>
		public static List<Egg> EggsPreserved { get; } = new();

		/// <summary>
		/// Collects an egg to EggsCollected list.
		/// </summary>
		/// <param name="egg">The egg to be collected.</param>
		public static void Collect(this Egg egg)
		{
			if (EggsCollected.Contains(egg) || EggsPreserved.Contains(egg))
				return;
			
			EggsCollected.Add(egg);
			OnEggCollected?.Invoke();
		}

		public static void AddCollectedEggsToPreserve()
		{
			if (EggsCollected.Count <= 0) return;
			
			EggsPreserved.AddRange(EggsCollected);
			EggsCollected.Clear();
			
			EggCollectingAudio.Instance.PlaySavingEffects();
		}

		/// <summary>
		/// Saves the individual preserved Eggs to PlayerPrefs and updates the count.
		/// </summary>
		public static void SavePreservedEggsToPlayerPrefs()
		{
			var sceneName = SceneManager.GetActiveScene().name;
			
			// Set each preserved Egg to 1 in PlayerPrefs. 
			foreach (var egg in EggsPreserved) 
				PlayerPrefs.SetInt(sceneName + "_Egg_" + egg.Identifier, 1);
			PlayerPrefs.Save();
		}

		/// <summary>
		/// Loads the individual Egg identifiers from PlayerPrefs.
		/// Disables them in the level if they are set as already collected.  
		/// </summary>
		/// <param name="eggsInScene">The list of all Eggs in the scene.</param>
		public static void SetupEggsFromPlayerPrefs(List<Egg> eggsInScene)
		{
			// Clear the collected and preserved eggs lists.
			EggsCollected.Clear();
			EggsPreserved.Clear();
			
			var sceneName = SceneManager.GetActiveScene().name;
			
			foreach (var egg in eggsInScene)
			{
				// Check if an Egg with corresponding identifier is found in the PlayerPrefs and set as 1.
				if (PlayerPrefs.GetInt(sceneName + "_Egg_" + egg.Identifier, 0) == 1)
				{
					// Add the found Egg to the list of already preserved Eggs, and disable the GameObject.
					EggsPreserved.Add(egg);
					egg.gameObject.SetActive(false);
				}
			}
		}

		/// <summary>
		/// Clears collected eggs and optionally preserved eggs.
		/// </summary>
		/// <param name="clearPreserved">Whether to clear preserved eggs.</param>
		public static void ClearEggs(bool clearPreserved = false)
		{
			foreach (var egg in EggsCollected)
			{
				egg.gameObject.SetActive(true);
			}
			EggsCollected.Clear();

			if (!clearPreserved) return;
			
			foreach (var egg in EggsPreserved)
			{
				egg.gameObject.SetActive(true);
			}
			EggsPreserved.Clear();
		}
	}
}
