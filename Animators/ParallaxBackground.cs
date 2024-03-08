using UnityEngine;

namespace Animators
{
	/// <summary>
	/// Attach to the background vistas to have a parallax effect.
	/// </summary>
	public class ParallaxBackground : MonoBehaviour
	{
		[SerializeField] private Transform targetToFollow;
		[SerializeField] private float parallaxMultiplier = 0.5f;
		
		private bool _canUpdate = true;
		private Vector3 _startPosition;

		private void Start()
		{
			_startPosition = transform.localPosition;
			if (targetToFollow != null) return;
			
			Debug.LogError("Target transform not assigned in " + name + "!");
			_canUpdate = false;
		}

		private void LateUpdate()
		{
			if (!_canUpdate) return;

			var followPosition = targetToFollow.position;
			
			transform.localPosition = new Vector3(
				_startPosition.x + followPosition.x * parallaxMultiplier, 
				_startPosition.y + followPosition.y * parallaxMultiplier,
				transform.localPosition.z);
		}
	}
}
