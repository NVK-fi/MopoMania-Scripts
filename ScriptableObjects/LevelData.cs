using UnityEngine;

namespace ScriptableObjects
{
	using System;

	/// <summary>
	/// Contains data for the levels.
	/// </summary>
	[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData")]
	public class LevelData : ScriptableObject
	{
		public Sprite levelIcon;
		public int levelNumber;
		public int totalEggs;
		public float defaultBestTime;
		public SkyData sky;
		public UIData ui;
	}

	/// <summary>
	/// Parameters to determine the background sky.
	/// </summary>
	[Serializable]
	public struct SkyData
	{
		public float sunlightAlpha;
		public float gradientAlpha;
	}

	/// <summary>
	/// In-game UI visibility settings. Adjust these to make the text visible enough.  
	/// </summary>
	[Serializable]
	public struct UIData
	{
		public float iconAlpha;
		public float textAlpha;
	}
}
