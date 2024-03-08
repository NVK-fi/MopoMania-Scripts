using UnityEngine;

namespace UI
{
	using System.Collections;
	using Tools;

	/// <summary>
	/// Represents a button for Exit and Website actions.
	/// </summary>
	public class ButtonForActions : ButtonBase
	{
		// TODO: This, among other button types, has a bad structure and might contain smelly code.
		
		public enum ActionType { Exit, Website }
		[field: SerializeField] public ActionType Type { get; private set; }
		[SerializeField] private Sprite defaultSprite;
		[SerializeField] private Sprite highlightSprite;
		[SerializeField] private SpriteRenderer spriteRenderer;
		
		private ButtonHighlight _buttonHighlight;
		
		private void Awake()
		{
			_buttonHighlight = GetComponentInParent<MenuInput>().Highlight;
			this.IsReferenceNull(_buttonHighlight);
			this.IsReferenceNull(defaultSprite);
			this.IsReferenceNull(highlightSprite);
			this.IsReferenceNull(spriteRenderer);
		}

		private void OnEnable() => ButtonHighlight.OnHighlightStart += UpdateSprite;

		private void OnDisable() => ButtonHighlight.OnHighlightStart -= UpdateSprite;

		private void UpdateSprite(ButtonBase highlightedButton)
		{
			spriteRenderer.sprite = highlightedButton == this
				? highlightSprite
				: defaultSprite;
		}

		public override void ExecuteAction()
		{
			StartCoroutine(ExecuteActionCoroutine());
		}

		private IEnumerator ExecuteActionCoroutine()
		{
			yield return _buttonHighlight.SetHighlightAlpha(0.05f);
			
			switch (Type)
			{
				case ActionType.Exit:
					Exit();
					break;
				case ActionType.Website:
					Application.OpenURL("https://NVK.fi");
					break;
			}
			
			StartCoroutine(_buttonHighlight.SetHighlightAlpha(1f));
		}

		private void Exit()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
					Application.Quit();
#endif
		}
	}
}
