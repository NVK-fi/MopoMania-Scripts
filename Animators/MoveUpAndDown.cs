using UnityEngine;

namespace Animators
{
	/// <summary>
	/// Moves the attached transform up and down in local space.
	/// </summary>
	public class MoveUpAndDown : MonoBehaviour
	{
		[SerializeField] private float frequency = 1f;
		[SerializeField] private float amplitude = 1f;
		[SerializeField] private float startingOffset;
		[SerializeField] private bool useXPositionAsOffset;
		private float _startingHeight;

		private void Awake()
		{
			_startingHeight = transform.localPosition.y;
			if (useXPositionAsOffset)
				startingOffset += transform.position.x;
		}

		private void Update()
		{
			var oldPosition = transform.localPosition;
			var newHeight = _startingHeight + Mathf.Sin((Time.time - startingOffset) * frequency ) * amplitude;
			var newPosition = new Vector3(oldPosition.x, newHeight, oldPosition.z);
			transform.localPosition = newPosition;
		}
	}
}