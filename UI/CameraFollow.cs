namespace UI
{
	using Managers;
	using Player;
	using UnityEngine;

	/// <summary>
	/// Makes the camera follow the player.
	/// </summary>
	public class CameraFollow : MonoBehaviour
	{
		[field: SerializeField] public PlayerRefs Target { get; set; }
		[field: SerializeField] public float MinimumHeight { get; private set; }
		[field: SerializeField] public float OffsetAboveMinimumHeight { get; private set; }
		private Transform _targetTransform;
		private bool _shouldFollowPlayer;
		
		private void Start()
		{
			Target ??= FindObjectOfType<PlayerRefs>();
			_targetTransform = Target.transform;
			CheckIfCameraShouldFollowPlayer();
		}
		private void LateUpdate()
		{
			if (!_shouldFollowPlayer) return;
			
			var bikePosition = _targetTransform.position;
			
			var y = bikePosition.y < MinimumHeight + OffsetAboveMinimumHeight
				? MinimumHeight
				: bikePosition.y - OffsetAboveMinimumHeight;
			
			transform.position = new Vector3(bikePosition.x, y, -10f);
		}

		private void OnEnable() => GameManager.Instance.OnGameStateChange += CheckIfCameraShouldFollowPlayer;
		private void OnDisable() => GameManager.Instance.OnGameStateChange -= CheckIfCameraShouldFollowPlayer;

		private void CheckIfCameraShouldFollowPlayer()
		{
			_shouldFollowPlayer = GameManager.State == GameManager.GameState.Playing
				|| GameManager.State == GameManager.GameState.PreStart;
		}
	}
}
