using UnityEngine;
using Tools;

namespace ScriptableObjects
{
	/// <summary>
	/// A ScriptableObject that contains various properties and configurations for a bike. 
	/// This includes visual representations like sprites, as well as physics-based properties such as throttle torque and brake inertia.
	/// </summary>
	[CreateAssetMenu(fileName = "BikeData", menuName = "ScriptableObjects/BikeData", order = 1)]
	public class BikeData : ScriptableObject
	{
		/// <summary>
		/// The sprite used to visually represent the bike's frame.
		/// </summary>
		[Header("Sprites")] public Sprite frameSprite;

		/// <summary>
		/// The sprite used to visually represent the bike's tires.
		/// </summary>
		public Sprite tireSprite;

		/// <summary>
		/// The amount of torque applied to the bike when the throttle is activated. 
		/// Represents the force that drives the bike forward or backward.
		/// </summary>
		[Header("Throttle")] public float throttleTorque = 800;

		/// <summary>
		/// The maximum angular velocity for the bike's tires. 
		/// This acts as a speed limiter, ensuring the tires don't rotate too quickly.
		/// </summary>
		[Header("Tires")] public float tiresMaxAngularVelocity = 2500f;

		/// <summary>
		/// The inertia applied to the bike's tires when the brakes are activated. 
		/// This determines how quickly the bike comes to a stop when braking.
		/// </summary>
		[Header("Brakes")] public float brakeInertia = 0.6f;

		/// <summary>
		/// The multiplier for the tires' angular velocities when braking.
		/// This determines how quickly the bike comes to a stop when braking, zero being immediate.
		/// </summary>
		[Range(0f, 1f)] public float brakeAngularVelocityMultiplier = 0.5f; 

		/// <summary>
		/// The torque applied to the bike's main RigidBody2D when rotating.
		/// Determines how fast the bike starts rotating.
		/// </summary>
		[Header("Rotating")] public float rotatingTorque = 2800f;

		/// <summary>
		/// The maximum angular velocity for the bike's frame.
		/// This limits the maximum speed of doing flips with the bike.
		/// </summary>		
		public float rotatingMaxAngularVelocity = 200f;
		
		private void OnEnable()
		{
			this.IsReferenceNull(frameSprite);
			this.IsReferenceNull(tireSprite);
		}
	}
}