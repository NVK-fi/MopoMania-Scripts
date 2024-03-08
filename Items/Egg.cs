using Managers;
using UnityEngine;

namespace Items
{
	using System.Collections;
	using System.Linq;


	/// <summary>
	/// Represents an egg object in the game.
	/// </summary>
	[RequireComponent(typeof(SpriteRenderer))]
	[RequireComponent(typeof(Collider2D))]
	public class Egg : MonoBehaviour
	{
		private const int MaxEggsInScene = 9;
		
		[field: SerializeField]
		public int Identifier { get; set; }

		[SerializeField] private float collectingEffectDuration = .25f;
		[SerializeField] private float collectingEffectSize = 2.5f;
		[SerializeField] private SpriteRenderer auraSpriteRenderer;

		private SpriteRenderer _eggSpriteRenderer;
		private Collider2D _eggCollider2D;
		private Color _initialAuraColor;
		private Vector3 _initialAuraScale;

		private void OnValidate()
		{
			// List and sort all eggs found in the scene.
			var eggsInScene = FindObjectsOfType<Egg>().ToList();
			eggsInScene.Sort((egg1, egg2) => egg1.Identifier.CompareTo(egg2.Identifier));

			if (eggsInScene.Count > MaxEggsInScene)
			{
				Debug.LogError("The maximum of Eggs in scene is reached! Please remove some. ( " + eggsInScene.Count + " / " + Egg.MaxEggsInScene + " Eggs )");
				return;
			}
			
			// Set a unique identifier for each egg.
			if (Identifier is < 0 or >= MaxEggsInScene) Identifier = 0;
			foreach (var comparedEgg in eggsInScene)
			{
				if (comparedEgg != this && comparedEgg.Identifier == Identifier)
					Identifier++;
			}
		}

		private void Awake()
		{
			_eggSpriteRenderer = GetComponent<SpriteRenderer>();
			_eggCollider2D = GetComponent<Collider2D>();
			
			_initialAuraScale = auraSpriteRenderer.transform.localScale;
			_initialAuraColor = auraSpriteRenderer.color;
		}

		private void OnEnable()
		{
			_eggCollider2D.enabled = true;
			_eggSpriteRenderer.enabled = true;
			auraSpriteRenderer.transform.localScale = _initialAuraScale;
			auraSpriteRenderer.color = _initialAuraColor;
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			// Check for a collision with the player and collect the egg.
			if (other.CompareTag("Bike") || other.CompareTag("Human") || other.CompareTag("Fowl"))
				CollectEgg();
		}

		private void CollectEgg()
		{
			// Mark the Egg as collected.
			this.Collect();

			_eggCollider2D.enabled = false;
			_eggSpriteRenderer.enabled = false;
			
			EggCollectingAudio.Instance.PlayPickupEffects();
			StartCoroutine(EggCollectingEffectCoroutine());
		}

		/// <summary>
		/// Applies a collecting effect to the Egg's aura sprite renderer,
		/// making it expand and fade out over a given duration.
		/// </summary>
		private IEnumerator EggCollectingEffectCoroutine()
		{
			var targetAuraScale = _initialAuraScale * collectingEffectSize;
			var targetAuraColor = new Color(_initialAuraColor.r, _initialAuraColor.g, _initialAuraColor.b, 0f);

			var elapsedTime = 0f;
			while (elapsedTime < collectingEffectDuration)
			{
				auraSpriteRenderer.transform.localScale =
					Vector3.Lerp(_initialAuraScale, targetAuraScale, elapsedTime / collectingEffectDuration);

				auraSpriteRenderer.color =
					Color.Lerp(_initialAuraColor, targetAuraColor, elapsedTime / collectingEffectDuration);
				
				elapsedTime += Time.unscaledDeltaTime;
				yield return null;
			}

			auraSpriteRenderer.transform.localScale = _initialAuraScale;
			gameObject.SetActive(false);
		}
	}
}
