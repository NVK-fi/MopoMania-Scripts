using UnityEngine;

namespace Items
{
	using System;
	using System.Collections;
	using Managers;
	using Tools;

	/// <summary>
	/// Represents a checkpoint in the game that can be activated by the player.
	/// </summary>
	public class Checkpoint : MonoBehaviour
	{
		[field: SerializeField] public Transform RespawnPoint { get; private set; }
		[SerializeField] private Sprite lampSpriteOff;
		[SerializeField] private Sprite lampSpriteOn;
		[SerializeField] private SpriteRenderer lampSpriteRenderer;
		[SerializeField] private SpriteRenderer lightSpriteRenderer;
		[SerializeField] private AudioSource lightAudioSource;
		[SerializeField] private BlinkEffect blinkEffect; 
		
		private void Awake()
		{
			this.IsReferenceNull(RespawnPoint);
			this.IsReferenceNull(lampSpriteOff);
			this.IsReferenceNull(lampSpriteOn);
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			// Activate the checkpoint when player enters the trigger zone.
			if (other.CompareTag("Bike") || other.CompareTag("Human") || other.CompareTag("Fowl"))
				LevelManager.Instance.ActivateCheckpoint(this);
		}

		/// <summary>
		/// Toggles the checkpoint's visuals and effects.
		/// </summary>
		/// <param name="turnOn">Whether to turn on the checkpoint.</param>
		/// <param name="playEffects">Whether to play activation effects.</param>
		public void Toggle(bool turnOn = true, bool playEffects = true)
		{
			lampSpriteRenderer.sprite = turnOn 
				? lampSpriteOn 
				: lampSpriteOff;
			
			lightSpriteRenderer.enabled = turnOn;

			if (turnOn && playEffects)
			{
				lightAudioSource.PlayWithRandomPitch(0.95f, 1.1f);
				StartCoroutine(blinkEffect.BlinkCoroutine(lightSpriteRenderer));
			};
		}

		/// <summary>
		/// Represents the blinking effect of the checkpoint's light.
		/// </summary>
		[Serializable]
		private class BlinkEffect
		{
			[SerializeField] private float duration = .3f;
			[SerializeField] private float alphaMultiplier = .5f;
			[SerializeField] private float sizeMultiplier = 2f;
			
			public IEnumerator BlinkCoroutine(SpriteRenderer lightSpriteRenderer)
			{
				var initialScale = lightSpriteRenderer.transform.localScale;
				var blinkScale = initialScale * sizeMultiplier;

				var initialColor = lightSpriteRenderer.color;
				var blinkColor = new Color(initialColor.r, initialColor.g, initialColor.b,  initialColor.a * alphaMultiplier);
				
				var elapsedTime = 0f;
				while (elapsedTime < duration)
				{
					lightSpriteRenderer.transform.localScale =
						Vector3.Lerp(blinkScale, initialScale, elapsedTime / duration);

					lightSpriteRenderer.color =
						Color.Lerp(blinkColor, initialColor, elapsedTime / duration);
				
					elapsedTime += Time.unscaledDeltaTime;
					yield return null;
				}

				lightSpriteRenderer.transform.localScale = initialScale;
				lightSpriteRenderer.color = initialColor;
			}
		}
	}
}