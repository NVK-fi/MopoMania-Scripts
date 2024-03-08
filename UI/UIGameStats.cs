using System;
using Managers;
using ScriptableObjects;
using UnityEngine;

namespace UI
{
	using System.Collections;
	using Tools;

	/// <summary>
	/// Manages and updates the UI elements displaying game statistics such as collected eggs, time elapsed, and the best time to beat.
	/// Also handles the slight visual feedback when the game finishes.
	/// </summary>
	public class UIGameStats : MonoBehaviour
	{
		[SerializeField] private TextMesh eggsText;
		[SerializeField] private SpriteRenderer eggsIcon;
		[SerializeField] private SpriteRenderer levelIcon;
		[SerializeField] private TextMesh timeText;
		[SerializeField] private SpriteRenderer timeIcon;
		[SerializeField] private TextMesh bestTimeText;

		private LevelData _data;
		
		private Color _iconColor;
		private Color _textColor;
		
		private void Awake()
		{
			this.IsReferenceNull(eggsText);
			this.IsReferenceNull(eggsIcon);
			this.IsReferenceNull(levelIcon);
			this.IsReferenceNull(timeText);
			this.IsReferenceNull(timeIcon);
			this.IsReferenceNull(bestTimeText);
		}
		
		private void Start()
		{
			if (LevelManager.Instance != null)
				_data = LevelManager.Instance.Data;

			_iconColor = Color.white.ChangeAlpha(_data.ui.iconAlpha);
			_textColor = Color.white.ChangeAlpha(_data.ui.textAlpha);
			
			eggsIcon.color = _iconColor;
			timeIcon.color = _iconColor;
			levelIcon.color = _iconColor;
			eggsText.color = _textColor;
			timeText.color = _textColor;
			bestTimeText.color = _iconColor;
			
			InitializeLevelInfo();
			UpdateTimer();
			UpdateEggs();
		}

		private void LateUpdate()
		{
			if (GameManager.State != GameManager.GameState.Playing) return;
			
			UpdateTimer();
		}

		private void OnEnable()
		{
			EggManager.OnEggCollected += UpdateEggs;
			GameManager.Instance.OnGameStateChange += GameStateChanged;
		}

		private void OnDisable()
		{
			EggManager.OnEggCollected -= UpdateEggs;
			GameManager.Instance.OnGameStateChange -= GameStateChanged;
		}

		private void GameStateChanged()
		{
			if (GameManager.State == GameManager.GameState.Finished)
				StartCoroutine(FinishCoroutine());
			else
				UpdateEggs();
			
			UpdateTimer();
		}

		private void InitializeLevelInfo()
		{
			// Set the level icon.
			levelIcon.sprite = _data.levelIcon;
			
			// Set the time to beat.
			var bestTime = PlayerPrefs.GetFloat(LevelManager.Instance.Data.name + "_BestTime", float.MaxValue);
			bestTime = Mathf.Min(LevelManager.Instance.Data.defaultBestTime, bestTime);
			var span = TimeSpan.FromSeconds(bestTime);
			bestTimeText.text = new string($"{span.Minutes:D1}:{span.Seconds:D2}.{span.Milliseconds:D3}");
		}

		private void UpdateTimer()
		{
			if (!timeText || !LevelManager.Instance) return;
			
			var span = TimeSpan.FromSeconds(LevelManager.Instance.LevelTimer.PlayTime);
			timeText.text = new string($"{span.Minutes:D1}:{span.Seconds:D2}.{span.Milliseconds:D3}");
		}

		private void UpdateEggs()
		{
			if (!eggsText) return;
			
			var savedEggs = EggManager.EggsCollected.Count + EggManager.EggsPreserved.Count;
			savedEggs = Mathf.Clamp(savedEggs, 0, LevelManager.Instance.Data.totalEggs);
				
			eggsText.text = new string($"{savedEggs}/{_data.totalEggs}");
		}

		private IEnumerator FinishCoroutine()
		{
			var color = Color.white.ChangeAlpha(0f);
			
			yield return StartCoroutine(FlashColorCoroutine(color, color, .5f));

			gameObject.SetActive(false);
		}

		private IEnumerator FlashColorCoroutine(Color iconColor, Color textColor, float duration)
		{
			var time = 0f;

			while (time < duration)
			{
				time += Time.unscaledDeltaTime;

				var newIconColor = Color.Lerp(Color.white, iconColor, time / duration);
				var newTextColor = Color.Lerp(Color.white, textColor, time / duration);

				eggsIcon.color = newIconColor;
				timeIcon.color = newIconColor;
				levelIcon.color = newIconColor;
				eggsText.color = newTextColor;
				timeText.color = newTextColor;
				bestTimeText.color = newIconColor;
				yield return null;
			}

			eggsIcon.color = iconColor;
			timeIcon.color = iconColor;
			levelIcon.color = iconColor;
			eggsText.color = textColor;
			timeText.color = textColor;
			bestTimeText.color = iconColor;
		}
	}
}
