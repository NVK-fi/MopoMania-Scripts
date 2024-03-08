using UnityEngine;

namespace UI
{
	using System;
	using System.Collections;
	using Managers;
	using Tools;

	/// <summary>
	/// Represents a button for levels.
	/// </summary>
	[RequireComponent(typeof(SpriteRenderer))]
	public class ButtonForLevelSelection : ButtonBase
	{
		// TODO: This, among other button types, has a bad structure and might contain smelly code.
		
		[SerializeField] private SpriteRenderer levelIcon;
		[SerializeField] private TextMesh eggsText;
		[SerializeField] private SpriteRenderer eggsIcon;
		[SerializeField] private TextMesh timeText;
		[SerializeField] private SpriteRenderer timeIcon;
		
		private MenuInput _menuInput;
		private ButtonHighlight _buttonHighlight;
		private int _lastPlayedLevel;
		private static readonly Color LockedIconColor = new(.35f, .35f, .35f, .75f);
		private static readonly Color LockedTextColor = new(.09f, .09f, .09f, .75f);
		
		private void Awake()
		{
			_menuInput = GetComponentInParent<MenuInput>();
			this.IsReferenceNull(_menuInput);
			
			_buttonHighlight = _menuInput.Highlight;
			
			UpdateVisuals();
		}

		private void Start()
		{
			_lastPlayedLevel = PlayerPrefs.GetInt("Levels_LastPlayed");
			if (_lastPlayedLevel == Data.levelNumber)
			{
				_menuInput.NavigateToButton(this);
				_menuInput.InitialButton = this;
			}
		}

		private void UpdateVisuals()
		{
			levelIcon.sprite = Data.levelIcon;
			
			if (!IsLevelUnlocked())
			{
				levelIcon.color = LockedIconColor;
				eggsText.color = Color.clear;
				eggsIcon.color = Color.clear;
				timeText.color = Color.clear;
				timeIcon.color = Color.clear;
				return;
			}
			
			if (eggsText)
			{
				var savedEggs = 0;
				for (var i = 0; i < Data.totalEggs; i++)
					if (PlayerPrefs.GetInt(Data.name + "_Egg_" + i, 0) == 1) 
						savedEggs++;
				
				if (savedEggs < Data.totalEggs)
				{
					eggsText.color = LockedTextColor;
					eggsIcon.color = LockedIconColor;
				}
				
				eggsText.text = new string($"{savedEggs}/{Data.totalEggs}");
			}

			if (timeText)
			{
				var bestTime = PlayerPrefs.GetFloat(Data.name + "_BestTime", float.MaxValue);

				if (bestTime > Data.defaultBestTime)
				{
					bestTime = Data.defaultBestTime;
					
					timeText.color = LockedTextColor;
					timeIcon.color = LockedIconColor;
				}

				var span = TimeSpan.FromSeconds(bestTime);
				timeText.text = new string($"{span.Minutes:D1}:{span.Seconds:D2}.{span.Milliseconds:D3}");
			}
		}

		public override void ExecuteAction()
		{
			StartCoroutine(ExecuteActionCoroutine());

			if (!IsLevelUnlocked()) return;
			
			FadeOutAudio();
		}

		private IEnumerator ExecuteActionCoroutine()
		{
		
			yield return _buttonHighlight.SetHighlightAlpha(0.05f);

			if (IsLevelUnlocked())
			{
				PlayerPrefs.SetInt("Levels_LastPlayed", Data.levelNumber);
				PlayerPrefs.Save();
			
				GameManager.LoadScene(Data.name);
			}
			
			StartCoroutine(_buttonHighlight.SetHighlightAlpha(1f));
		}

		private void FadeOutAudio()
		{
			if (AudioFader.GlobalVolumeFaderCoroutine != null)
			{
				StopCoroutine(AudioFader.GlobalVolumeFaderCoroutine);
				AudioFader.GlobalVolumeFaderCoroutine = null;
			}

			AudioFader.GlobalVolumeFaderCoroutine =
				StartCoroutine(AudioFader.UpdateGlobalVolumeOverTime(AudioListener.volume, 0,.1f));
		}

		private bool IsLevelUnlocked() 
			=> PlayerPrefs.GetInt("Levels_Progress", 1) >= Data.levelNumber;

	}
}