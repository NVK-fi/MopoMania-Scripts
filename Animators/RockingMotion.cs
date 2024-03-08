using UnityEngine;

namespace Animators
{
	/// <summary>
	/// Rotates the attached transform in rocking motion in local space.
	/// </summary>
	public class RockingMotion : MonoBehaviour
	{
		[SerializeField] private float rotationSpeed = 2f;
		[SerializeField] private float rockingAmount = 30f;
		[SerializeField] private float rockingClamp = 30f;

		void Update()
		{
			var angle = rockingAmount * Mathf.Sin(rotationSpeed * Time.time);
			
			if (Mathf.Abs(angle) > rockingClamp)
				angle = Mathf.Sign(angle) * rockingClamp;
			
			transform.localRotation = Quaternion.Euler(0f, 0f, angle);
		}
	}
}