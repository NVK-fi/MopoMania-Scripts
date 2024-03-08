using UnityEngine;

namespace Player
{
	using Tools;

	/// <summary>
	/// Plays an animation when throttling and cross-fades back to idle.
	/// </summary>
	public class BikeThrottleAnimator : MonoBehaviour
	{
		[SerializeField, Range(0f, 1f)] private float idleCrossFadeTime = 0.5f;
		[SerializeField] private AnimationClip idleAnimationClip;
		[SerializeField] private AnimationClip throttleAnimationClip;
		
		private PlayerRefs _playerRefs;
		private Animation _animation;
		
		private void Awake()
		{
			_playerRefs = transform.root.GetComponent<PlayerRefs>();
			
			_animation = GetComponent<Animation>();
			this.IsReferenceNull(_animation);
		}

		private void OnEnable() => _playerRefs.States.OnDrivingStateChange += HandleDrivingStateChange;

		private void OnDisable() => _playerRefs.States.OnDrivingStateChange -= HandleDrivingStateChange;

		private void HandleDrivingStateChange()
		{
			if (_playerRefs.States.DrivingState == BikeStates.DrivingStates.Throttle)
			{
				_animation.Stop();
				_animation.clip = throttleAnimationClip;
				_animation.Play();
			}
			else
			{
				_animation.CrossFade(idleAnimationClip.name, idleCrossFadeTime);	
			}
		}
	}
}
