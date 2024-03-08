using UnityEngine;

namespace UI
{
	using System;
	using System.Collections;
	using Tools;

	/// <summary>
	/// Handles selection highlighting effect for menu buttons.
	/// </summary>
	[RequireComponent(typeof(SpriteRenderer))]
	public class ButtonHighlight : MonoBehaviour
	{
		[SerializeField] private float movementFadeOutDuration = .03f;
		[SerializeField] private float movementFadeInDuration = .15f;
		[SerializeField] private float manualFadingDuration = .1f;
		[SerializeField] private Sprite highlight32;
		[SerializeField] private Sprite highlight64;
		[SerializeField] private Sprite highlight128;
		
		private Coroutine _highlightCoroutine;
		private SpriteRenderer _spriteRenderer;
		public static event Action<ButtonBase> OnHighlightStart;
		
		private void Awake()
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
		}

		public void Highlight(ButtonBase button)
		{
			if (_highlightCoroutine != null)
				StopCoroutine(_highlightCoroutine);
			_highlightCoroutine = StartCoroutine(MoveAndFadeHighlight(button));
		}

		public IEnumerator SetHighlightAlpha(float targetAlpha)
		{
			if (_highlightCoroutine != null)
				StopCoroutine(_highlightCoroutine);

			yield return _highlightCoroutine = StartCoroutine(FadeHighlight(targetAlpha, manualFadingDuration));
		}
		
		private IEnumerator MoveAndFadeHighlight(ButtonBase button)
		{
			yield return FadeHighlight(0f, movementFadeOutDuration);

			transform.position = button.transform.position;
			
			_spriteRenderer.sprite = button switch
			{
				ButtonForLevelSelection => highlight128,
				ButtonForSettings { Setting: GameSettings.Setting.Input } => highlight64,
				_ => highlight32
			};
			
			OnHighlightStart?.Invoke(button);

			yield return FadeHighlight(1f, movementFadeInDuration);
		}

		private IEnumerator FadeHighlight(float targetAlpha, float duration)
		{
			var startColor = _spriteRenderer.color;
			var targetColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

			var elapsedTime = (1 - Mathf.Abs(targetAlpha - startColor.a)) * duration;

			while (elapsedTime < duration)
			{
				_spriteRenderer.color = Color.Lerp(startColor, targetColor, elapsedTime / duration);
				elapsedTime += Time.unscaledDeltaTime;
				yield return null;
			}

			_spriteRenderer.color = targetColor;
		}
	}
}
