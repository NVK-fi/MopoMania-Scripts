using UnityEngine;

namespace Player
{
	using ScriptableObjects;
	using Tools;

	/// <summary>
	/// Holds references to various parts of the bike.
	/// Make sure it is executed before the default time in Script Execution Order list. 
	/// </summary>
	[DefaultExecutionOrder(-7)]
	[RequireComponent(typeof(BikeStates))]
	[RequireComponent(typeof(PlayerPhysics))]
	[RequireComponent(typeof(Rigidbody2D))]
	public class PlayerRefs : MonoBehaviour
	{
		[field: Header("ScriptableObjects")] 
		[field: SerializeField] public BikeData BikeData { get; private set; }
		[field: SerializeField] public FowlData FowlData { get; private set; }
		
		[field: Header("Rigidbodies")] 
		[field: SerializeField] public Rigidbody2D BikeRigidbody { get; private set; }
		[field: SerializeField] public Rigidbody2D LeftTireRigidbody { get; private set; }
		[field: SerializeField] public Rigidbody2D RightTireRigidbody { get; private set; }
		public Rigidbody2D FrontTireRigidbody => States.IsFlipped ? LeftTireRigidbody : RightTireRigidbody;
		public Rigidbody2D RearTireRigidbody => States.IsFlipped ? RightTireRigidbody : LeftTireRigidbody;
		
		[field: Header("Transforms")] 
		[field: SerializeField] public Transform FrameTransform { get; private set; }
		[field: SerializeField] public Transform RidersTransform { get; private set; }
		[field: SerializeField] public Transform FowlTransform { get; private set; }
		
		public BikeStates States { get; private set; }
		public PlayerPhysics Physics { get; private set; }

		private void Awake()
		{
			this.IsReferenceNull(BikeData);
			this.IsReferenceNull(FowlData);
			
			this.IsReferenceNull(BikeRigidbody);
			this.IsReferenceNull(LeftTireRigidbody);
			this.IsReferenceNull(RightTireRigidbody);

			this.IsReferenceNull(FrameTransform);
			this.IsReferenceNull(RidersTransform);
			this.IsReferenceNull(FowlTransform);
			
			States = GetComponent<BikeStates>();
			this.IsReferenceNull(States);

			Physics = GetComponent<PlayerPhysics>();
			this.IsReferenceNull(Physics);
		}
	}
}
