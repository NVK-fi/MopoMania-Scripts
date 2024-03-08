using UnityEngine;
using Tools;

namespace ScriptableObjects
{
	/// <summary>
	/// A ScriptableObject that contains various properties and configurations for a bike. 
	/// This includes visual representations like sprites, as well as physics-based properties such as throttle torque and brake inertia.
	/// </summary>
	[CreateAssetMenu(fileName = "FowlData", menuName = "ScriptableObjects/FowlData", order = 2)]
	public class FowlData : ScriptableObject
	{
		/// <summary>
		/// The sprites used to visually represent the fowl's body.
		/// The order of these is important: Higher index sprites will get used as the fowl goes faster.
		/// Minimum requirement is to have just one sprite. 
		/// </summary>
		[Header("Sprites")] public Sprite[] fowlSprites;

		/// <summary>
		/// The sprite used to visually represent the fowl's wing.
		/// </summary>
		public Sprite wingSprite;

		private void OnEnable()
		{
			this.IsReferenceNull(fowlSprites);
			this.IsReferenceNull(wingSprite);
		}
	}
}