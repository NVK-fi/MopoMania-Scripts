namespace UI
{
	using UnityEngine;

	/// <summary>
	/// Makes a UI element follow the position of a target camera.
	/// </summary>
	[DefaultExecutionOrder(100)]
	public class UIFollow : MonoBehaviour
	{
		[field: SerializeField] public Camera Target { get; set; }
		
		private void LateUpdate()
		{
			var position = Target.transform.position;
			transform.position = new Vector3(position.x, position.y, transform.position.z);
		}
	}
}
