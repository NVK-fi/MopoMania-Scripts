using UnityEngine;

namespace Player
{
	using Tools;

	/// <summary>
	/// Manages the visual representation of a bike, setting the appropriate sprites based on BikeData at Start().
	/// </summary>
	[RequireComponent(typeof(PlayerRefs))]
	public class BikeSpritesSetup : MonoBehaviour
	{
		[SerializeField] private SpriteRenderer frameSpriteRenderer;
		[SerializeField] private SpriteRenderer leftTireSpriteRenderer;
		[SerializeField] private SpriteRenderer rightTireSpriteRenderer;
		
		private PlayerRefs _playerRefs;

		private void Awake()
		{
			_playerRefs = GetComponent<PlayerRefs>();

			this.IsReferenceNull(frameSpriteRenderer);
			this.IsReferenceNull(leftTireSpriteRenderer);
			this.IsReferenceNull(rightTireSpriteRenderer);
		}

		private void Start()
		{
			frameSpriteRenderer.sprite = _playerRefs.BikeData.frameSprite;
			leftTireSpriteRenderer.sprite = _playerRefs.BikeData.tireSprite;
			rightTireSpriteRenderer.sprite = _playerRefs.BikeData.tireSprite;
		}
	}
}
