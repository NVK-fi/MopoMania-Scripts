using UnityEngine;

namespace UI
{
	using System;
	using Managers;
	using ScriptableObjects;

	/// <summary>
	/// Updates the visual elements of the finish stats UI based on the player's performance in the level.
	/// </summary>
	[RequireComponent(typeof(SpriteRenderer))]
	public class UIFinishStats : MonoBehaviour
	{
		[SerializeField] private SpriteRenderer levelIcon;
		[SerializeField] private TextMesh eggsText;
		[SerializeField] private SpriteRenderer eggsIcon;
		[SerializeField] private TextMesh timeText;
		[SerializeField] private SpriteRenderer timeIcon;
		
		private static readonly Color LockedIconColor = new(.25f, .25f, .25f);
		private static readonly Color LockedTextColor = new(.05f, .05f, .05f);
		private LevelData _data;
		
		private void OnEnable()
		{
			_data = LevelManager.Instance.Data;
			UpdateVisuals();
		}

		private void UpdateVisuals()
		{
			levelIcon.sprite = _data.levelIcon;
		
			if (eggsText)
			{
				var savedEggs = EggManager.EggsPreserved.Count;
				savedEggs = Mathf.Clamp(savedEggs, 0, _data.totalEggs);

				if (savedEggs < _data.totalEggs)
				{
					eggsText.color = LockedTextColor;
					eggsIcon.color = LockedIconColor;
				}
				
				eggsText.text = new string($"{savedEggs}/{_data.totalEggs}");
			}

			if (timeText)
			{
				var time = LevelManager.Instance.LevelTimer.PlayTime;

				// Target time was NOT met, grey the numbers.
				if (time > _data.defaultBestTime)
				{
					timeText.color = LockedTextColor;
					timeIcon.color = LockedIconColor;
				}

				var span = TimeSpan.FromSeconds(time);
				timeText.text = new string($"{span.Minutes:D1}:{span.Seconds:D2}.{span.Milliseconds:D3}");
			}
		}
	}
}
