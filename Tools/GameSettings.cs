namespace Tools
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// A static class for managing game settings stored in PlayerPrefs.
	/// </summary>
	public static class GameSettings
	{
		public enum Setting { Input, Volume, Screen }

		private static readonly Dictionary<Setting, int> OptionCounts = new()
		{
			{ Setting.Input, 4 }, 
			{ Setting.Volume, 3 }, 
			{ Setting.Screen, 2 }
		};

		public static void Initialize()
		{
			CorrectFullscreenValue();
			AudioListener.volume = GetVolumeFromPlayerPrefs();
		}

		/// <summary>
		/// Gets the setting's value from PlayerPrefs and returns a validated version of it.
		/// </summary>
		/// <returns>The validated setting value.</returns>
		public static int GetPlayerPrefsValue(this Setting setting)
		{
			var value = PlayerPrefs.GetInt(setting.PlayerPrefsName(), 0);
			return value < 0 || value >= OptionCounts[setting] 
				? 0 
				: value;
		}

		/// <summary>
		/// Rotates the setting value in PlayerPrefs and saves the change.
		/// </summary>
		public static void SetPlayerPrefsValue(this Setting setting, int newValue)
		{
			newValue %= OptionCounts[setting];
			PlayerPrefs.SetInt(setting.PlayerPrefsName(), newValue);
			PlayerPrefs.Save();
		}

		/// <summary>
		/// Gets the amount of options a setting has.
		/// </summary>
		public static int GetOptionCount(this Setting setting) => OptionCounts[setting];

		private static string PlayerPrefsName(this Setting setting) 
			=> new($"Settings_{Enum.GetName(typeof(Setting), setting)}");

		/// <summary>
		/// Corrects the fullscreen setting's value if it is wrong.
		/// </summary>
		public static void CorrectFullscreenValue()
		{
			if (Setting.Screen.GetPlayerPrefsValue() == 0 && !Screen.fullScreen)
				Setting.Screen.SetPlayerPrefsValue(1);
			else if (Setting.Screen.GetPlayerPrefsValue() != 0 && Screen.fullScreen)
				Setting.Screen.SetPlayerPrefsValue(0);
		}

		/// <summary>
		/// Gets the global audio volume based on PlayerPrefs. 
		/// </summary>
		public static float GetVolumeFromPlayerPrefs()
		{
			return Setting.Volume.GetPlayerPrefsValue() switch
			{
				1 => 0f,
				2 => 0.5f,
				_ => 1f
			};
		}

		/// <summary>
		/// Toggles fullscreen mode and updates PlayerPrefs accordingly.
		/// </summary>
		public static void ToggleFullscreen(bool fullscreen)
		{
			if (fullscreen)
			{
				PlayerPrefs.SetInt("Settings_WindowedWidth", Screen.width);
				PlayerPrefs.SetInt("Settings_WindowedHeight", Screen.height);
				PlayerPrefs.Save();
						
				Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
			}
			else
			{
				var width = PlayerPrefs.GetInt("Settings_WindowedWidth", 800);
				var height = PlayerPrefs.GetInt("Settings_WindowedHeight", 600);
						
				Screen.SetResolution(width, height, false);
			}
		}
	}
}