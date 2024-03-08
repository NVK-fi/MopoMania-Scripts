using UnityEngine;

namespace Player
{
	/// <summary>
	/// Updates the angular drag of the rigidbody attached to the tire based on if the tire is spinning freely.
	/// </summary>
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(CircleCollider2D))]
	public class BikeTireDrag : MonoBehaviour
	{
		[field: SerializeField] private float touchingAngularDrag = 0.05f;
		[field: SerializeField] private float freeAngularDrag = 0.5f;
		private Rigidbody2D _rigidbody;
		
		private void Awake()
		{
			_rigidbody = GetComponent<Rigidbody2D>();
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			// Check if the collider started touching something.
			if (collision.contacts.Length <= 0) return;
			
			// Update the Rigidbody2D's angular drag.
			_rigidbody.angularDrag = touchingAngularDrag;
		}
		
		private void OnCollisionExit2D(Collision2D collision)
		{
			// Check if the collider stopped touching something.
			if (collision.contacts.Length > 0) return;
			
			// Update the Rigidbody2D's angular drag.
			_rigidbody.angularDrag = freeAngularDrag;
		}
	}
}
