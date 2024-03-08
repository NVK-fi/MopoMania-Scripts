using UnityEngine;

namespace UI
{
	using Managers;
	using ScriptableObjects;
	using Tools;

	/// <summary>
	/// Controls the visual representation of the background sky.
	/// </summary>
	public class SkySetter : MonoBehaviour
	{
		[SerializeField] private SpriteRenderer topGradient;
		[SerializeField] private SpriteRenderer bottomGradient;
		[SerializeField] private SpriteRenderer sunlight;
		[SerializeField] private float lerpSpeed = 1f;

		private bool _isUpdateNeeded;
		private SkyData _sky;

		private void OnEnable() => ButtonHighlight.OnHighlightStart += OnButtonHighlight;
		private void OnDisable() => ButtonHighlight.OnHighlightStart -= OnButtonHighlight;

		private void OnButtonHighlight(ButtonBase button)
		{
			_sky = button.Data.sky;
			_isUpdateNeeded = true;
		}

		private void Start()
		{
			if (LevelManager.Instance != null)
				ApplySky(LevelManager.Instance.Data.sky);
		}

		private void Update()
		{
			if (_isUpdateNeeded) LerpSky();
		}

		/// <summary>
		/// Lerps the sky visuals towards the target sky data.
		/// </summary>
		private void LerpSky()
		{
			sunlight.color = Color.Lerp(
				sunlight.color, 
				sunlight.color.ChangeAlpha(_sky.sunlightAlpha),
				lerpSpeed * Time.unscaledDeltaTime);
			
			topGradient.color = Color.Lerp(
				topGradient.color, 
				topGradient.color.ChangeAlpha(_sky.gradientAlpha),
				lerpSpeed * Time.unscaledDeltaTime);
			
			bottomGradient = topGradient;

			var sunlightDifference = _sky.sunlightAlpha - sunlight.color.a;
			var gradientDifference = _sky.gradientAlpha - topGradient.color.a;
			if (!(sunlightDifference.Abs() < 0.01f) || !(gradientDifference.Abs() < 0.01f)) return;
			
			ApplySky(_sky);
			_isUpdateNeeded = false;
		}

		/// <summary>
		/// Applies the target sky data.
		/// </summary>
		private void ApplySky(SkyData sky)
		{
			sunlight.color = sunlight.color.ChangeAlpha(sky.sunlightAlpha);
			topGradient.color = topGradient.color.ChangeAlpha(sky.gradientAlpha);
			bottomGradient.color = bottomGradient.color.ChangeAlpha(sky.gradientAlpha);
		}
	}
}
