namespace Player
{
	using ScriptableObjects;
	using Tools;
	using UnityEngine;

	/// <summary>
	/// This class manages the display of different fowl sprites based on the velocity of the bike.
	/// </summary>
	[RequireComponent(typeof(SpriteRenderer))]
	public class RidersFowlSprites : MonoBehaviour
	{
		[SerializeField] private FowlData fowlData;
		[SerializeField] private float topVelocity = 300f;
		[SerializeField] private SpriteRenderer wingSpriteRenderer;

		private PlayerRefs _playerRefs;
		private SpriteRenderer _spriteRenderer;
		private int _spriteCount;
		private int _currentSpriteIndex;
		private Vector2 _previousVelocity;

		private void Awake()
		{
			_playerRefs = transform.root.GetComponent<PlayerRefs>();
			_spriteRenderer = GetComponent<SpriteRenderer>();

			this.IsReferenceNull(fowlData);
			this.IsReferenceNull(wingSpriteRenderer);
		}
		
		private void OnEnable()
		{
			_spriteCount = fowlData.fowlSprites.Length;
			_spriteRenderer.sprite = fowlData.fowlSprites[0];
			
			// Apply the correct wing sprite from FowlData.
			wingSpriteRenderer.sprite = _playerRefs.FowlData.wingSprite;

			// Disable this component if no updates are needed.
			if (_spriteCount <= 1) enabled = false;
		}
		
		private void Update() => ApplyFowlSprite();

		/// <summary>
		/// Determines the fowl sprite based on the bike's velocity and updates the sprite if needed.
		/// </summary>
		private void ApplyFowlSprite()
		{
			var velocityVector = _playerRefs.Physics.FowlVelocity;
		
			// Return early if the velocity has not changed.
			if (_previousVelocity == velocityVector) return;
			_previousVelocity = velocityVector;
			
			var velocity = velocityVector.sqrMagnitude;
			var intervalSize = topVelocity / _spriteCount;
			var spriteIndex = Mathf.Clamp((int)(velocity / intervalSize), 0, _spriteCount - 1);

			// Return early if the sprite does not change.
			if (spriteIndex == _currentSpriteIndex) return;
			_currentSpriteIndex = spriteIndex;

			_spriteRenderer.sprite = fowlData.fowlSprites[spriteIndex];
		}
	}
}