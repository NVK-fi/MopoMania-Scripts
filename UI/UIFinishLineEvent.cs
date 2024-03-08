using UnityEngine;

namespace UI
{
    using System.Collections;
    using Managers;
    using Tools;
    using UnityEngine.Rendering.PostProcessing;

    /// <summary>
    /// Controls the camera blur and pause-background alpha based on the game state.
    /// </summary>
    public class UIFinishLineEvent : MonoBehaviour
    {
        [SerializeField] private PostProcessVolume blurPostProcessVolume;
        [SerializeField] private SpriteRenderer background;
        [SerializeField] private GameObject results;

        private void Awake()
        {
            this.IsReferenceNull(blurPostProcessVolume);
            this.IsReferenceNull(background);
        }

        private void OnEnable() => GameManager.Instance.OnGameStateChange += CheckGameState;
        private void OnDisable() => GameManager.Instance.OnGameStateChange -= CheckGameState;

        /// <summary>
        /// Checks the current game state and adjusts camera blur and SpriteRenderers' alphas accordingly.
        /// </summary>
        private void CheckGameState()
        {
	        if (GameManager.State != GameManager.GameState.Finished) return;
	        
	        StartCoroutine(FinishLineCoroutine());
        }

        private IEnumerator FinishLineCoroutine()
        {
	        yield return new WaitForSecondsRealtime(0.5f);

	        results.SetActive(true);
    
	        const float duration = 0.5f;
	        var elapsedTime = 0f;
	        var color = background.color;

	        while (elapsedTime < duration / 2f)
	        {
		        elapsedTime += Time.unscaledDeltaTime;

		        var ratio = elapsedTime / duration;

		        background.color = new Color(color.r, color.g, color.b, ratio);
		        blurPostProcessVolume.weight = ratio;

		        yield return null;
	        }

	        while (elapsedTime < duration)
	        {
		        elapsedTime += Time.unscaledDeltaTime;

		        var ratio = elapsedTime / duration;

		        background.color = new Color(color.r, color.g, color.b, ratio);
		        blurPostProcessVolume.weight = ratio;

		        yield return null;
	        }
	        
	        background.color = new Color(color.r, color.g, color.b, 1f);
	        blurPostProcessVolume.weight = 1f;
        }
    }
}
