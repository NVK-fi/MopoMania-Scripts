using UnityEngine;

namespace UI
{
	using System.Collections;
	using Tools;
	using UnityEngine.Rendering.PostProcessing;
	using UnityEngine.Serialization;

	/// <summary>
	/// Controls the fading in of the camera.
	/// </summary>
	public class CameraFadeIn : MonoBehaviour
	{
		[SerializeField] private float duration = 1f;
		[FormerlySerializedAs("backgroundSprite")] [SerializeField] private SpriteRenderer overlaySprite;
		[SerializeField] private PostProcessVolume blurPostProcessVolume;
		
		private void Awake()
		{
			this.IsReferenceNull(overlaySprite);
			this.IsReferenceNull(blurPostProcessVolume);
		}

		private void Start()
		{
			StartCoroutine(FadeBlur());

			var targetVolume = GameSettings.GetVolumeFromPlayerPrefs();

			if (AudioFader.GlobalVolumeFaderCoroutine != null)
			{
				StopCoroutine(AudioFader.GlobalVolumeFaderCoroutine);
				AudioFader.GlobalVolumeFaderCoroutine = null;
			}
			AudioFader.GlobalVolumeFaderCoroutine = 
				StartCoroutine(AudioFader.UpdateGlobalVolumeOverTime(0f, targetVolume, duration * 0.5f));
		}

		private IEnumerator FadeBlur()
		{
			var elapsedTime = 0f;
			
			var color = overlaySprite.color;
			var alphaTarget = overlaySprite.color.a;
			var weightTarget = blurPostProcessVolume.weight;

			while (elapsedTime < duration)
			{
				elapsedTime += Time.unscaledDeltaTime;

				var ratio = elapsedTime / duration;

				var alpha = Mathf.Lerp(1f, alphaTarget, ratio);
				var weight = Mathf.Lerp(1f, weightTarget, ratio);
				
				overlaySprite.color = new Color(color.r, color.g, color.b, alpha);
				blurPostProcessVolume.weight = weight;

				yield return null;
			}
			
			overlaySprite.color =  new Color(color.r, color.g, color.b, alphaTarget);
			blurPostProcessVolume.weight = weightTarget;
		}
	}
}
