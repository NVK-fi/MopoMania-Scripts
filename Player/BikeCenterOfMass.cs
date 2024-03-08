namespace Player
{
	using UnityEngine;

	/// <summary>
	/// Adjusts the center of mass of the bike based on its leaning.
	/// </summary>
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(BikeStates))]
	public class BikeCenterOfMass : MonoBehaviour
	{
		[SerializeField] private float localY;
		[SerializeField, Range(0f,1f)] private float leaningAmountOnX;
		[SerializeField] private bool drawGizmo;

		private Vector2 _centerOfMass;
		private Rigidbody2D _rigidbody2D;
		private BikeStates _bikeStates;

		private void Awake()
		{
			_rigidbody2D = GetComponent<Rigidbody2D>();
			_bikeStates = GetComponent<BikeStates>();

			_centerOfMass.y = localY;
		}

		private void OnEnable() => _bikeStates.OnRotatingStateChange += RotatingStateChanged;
		private void OnDisable() => _bikeStates.OnRotatingStateChange -= RotatingStateChanged;

		/// <summary>
		/// Adjusts the center of mass based on the bike's rotating state.
		/// </summary>
		private void RotatingStateChanged()
		{
			_centerOfMass.x = _bikeStates.RotatingState switch
			{
				BikeStates.RotatingStates.AntiClockwise => -leaningAmountOnX,
				BikeStates.RotatingStates.Neither => 0,
				BikeStates.RotatingStates.Clockwise => leaningAmountOnX,
				_ => _centerOfMass.x
			};
			
			_rigidbody2D.centerOfMass = _centerOfMass;
		}
		
		private void OnDrawGizmos()
		{
			if (!drawGizmo) return;
			
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.TransformPoint(_centerOfMass), 0.05f);
		}
	}
}
