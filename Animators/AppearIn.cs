using UnityEngine;

namespace Animators
{
	using System.Collections;

	/// <summary>
	/// Causes the attached sprite renderer to fade in over a specified duration after a delay.
	/// </summary>
	[RequireComponent(typeof(SpriteRenderer))]
	public class AppearIn : MonoBehaviour
	{
		[SerializeField] private float timeBeforeFadeIn;
		[SerializeField] private float fadeInTime;

		private SpriteRenderer _spriteRenderer;

		private void Start()
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
			
			StartCoroutine(AppearInCoroutine());
		}

		private IEnumerator AppearInCoroutine()
		{
			yield return new WaitForSeconds(timeBeforeFadeIn);
			
			var elapsedTime = 0f;
			
			var color = _spriteRenderer.color;
			color = new Color(color.r, color.g, color.b, 0f);
			_spriteRenderer.color = color;

			while (elapsedTime < fadeInTime)
			{
				elapsedTime += Time.deltaTime;
				
				var alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInTime);
				color = new Color(color.r, color.g, color.b, alpha);
				_spriteRenderer.color = color;
				
				yield return null;
			}
		}
	}
}
