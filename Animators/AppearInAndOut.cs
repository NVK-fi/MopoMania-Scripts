using UnityEngine;

namespace Animators
{
	/// <summary>
	/// Causes the attached sprite renderers to fade in and out over specified durations.
	/// </summary>
	public class AppearInAndOut : MonoBehaviour
	{
			[SerializeField] private float fadingTime = 2f;
			[SerializeField] private float visibilityTime = 2f;
			[SerializeField] private float hiddenTime = 2f;
			[SerializeField] private Phase phase = Phase.Hidden;
			[SerializeField, Range(0f, 1f)] private float phaseStartingPoint;
			[SerializeField] private SpriteRenderer[] spriteRenderers;
			private float _timer;

			private enum Phase
			{
				Hidden = 0,
				FadeIn = 1,
				Visible = 2,
				FadeOut = 3
			}
			
			private void Awake()
			{
				if (spriteRenderers.Length == 0)
				{
					Debug.LogError("No SpriteRenderers attached!");
					enabled = false;
				}

				_timer = phase switch
				{
					Phase.Hidden => hiddenTime,
					Phase.FadeIn => fadingTime,
					Phase.Visible => visibilityTime,
					Phase.FadeOut => fadingTime,
					_ => _timer
				};
				_timer *= phaseStartingPoint;
			}

			private void Update()
			{
				_timer += Time.deltaTime;

				switch (phase)
				{
					case Phase.Hidden:
						CheckTimer(hiddenTime);
						break;
					case Phase.FadeIn:
						ApplyAlphaToSpriteRenderers(Mathf.Lerp(0f, 1f, _timer / fadingTime));
						CheckTimer(fadingTime);
						break;
					case Phase.Visible:
						CheckTimer(visibilityTime);
						break;
					case Phase.FadeOut:
						ApplyAlphaToSpriteRenderers(Mathf.Lerp(1f, 0f, _timer / fadingTime));
						CheckTimer(fadingTime);
						break;
					default:
						_timer = 0;
						phase = 0;
						break;
				}
			}

			private void CheckTimer(float targetTime)
			{
				if (_timer < targetTime) return;
				
				phase++;
				_timer = 0f;
			}
			
			private void ApplyAlphaToSpriteRenderers(float alpha)
			{
				if (spriteRenderers == null) return;
				
				foreach (var spriteRenderer in spriteRenderers)
				{
					var color = spriteRenderer.color;
					color.a = alpha;
					spriteRenderer.color = color;
				}
			}
	}
}
