using UnityEngine;

namespace UI
{
	using System.Collections;
	using Tools;
	
	/// <summary>
	/// Represents a button for input, audio and window settings.
	/// </summary>
	public class ButtonForSettings : ButtonBase
	{
		// TODO: This, among other button types, has a bad structure and might contain smelly code.
		
		[field: SerializeField] public GameSettings.Setting Setting { get; private set; }
		[field: SerializeField] public Sprite[] Sprites { get; private set; }

		private ButtonHighlight _buttonHighlight;
		private SpriteRenderer _spriteRenderer;
		
		public void Awake()
		{
			_buttonHighlight = GetComponentInParent<MenuInput>().Highlight;
			this.IsReferenceNull(_buttonHighlight);
			
			_spriteRenderer = GetComponent<SpriteRenderer>();
		}
		
		private void Start()
		{
			UpdateVisuals();
		}

		public override void ExecuteAction()
		{
			StartCoroutine(ExecuteActionCoroutine());
		}

		private IEnumerator ExecuteActionCoroutine()
		{
			yield return _buttonHighlight.SetHighlightAlpha(0.05f);

			Setting.SetPlayerPrefsValue(Setting.GetPlayerPrefsValue() + 1);
			UpdateVisuals();

			switch (Setting)
			{
				case GameSettings.Setting.Volume:
					AudioListener.volume = GameSettings.GetVolumeFromPlayerPrefs();
					break;

				case GameSettings.Setting.Screen:
					GameSettings.ToggleFullscreen(GameSettings.Setting.Screen.GetPlayerPrefsValue() == 0);
					break;
			}
			
			StartCoroutine(_buttonHighlight.SetHighlightAlpha(1f));
		}
		

		/// <summary>
		/// Updates the visual representation of the setting button based on the current value in PlayerPrefs.
		/// </summary>
		private void UpdateVisuals()
		{
			if (Sprites.Length >= Setting.GetOptionCount())
				_spriteRenderer.sprite = Sprites[Setting.GetPlayerPrefsValue()];
		}
	}
}
