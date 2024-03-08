using UnityEngine;

namespace UI
{
	using ScriptableObjects;

	/// <summary>
	/// The base class for menu buttons.
	/// </summary>
	[RequireComponent(typeof(SpriteRenderer))]
	public abstract class ButtonBase : MonoBehaviour
	{
		[field: SerializeField] public ButtonBase ButtonAbove { get; set; }
		[field: SerializeField] public ButtonBase ButtonBelow { get; set; }
		[field: SerializeField] public ButtonBase ButtonOnLeft { get; set; }
		[field: SerializeField] public ButtonBase ButtonOnRight { get; set; }
		
		[field: SerializeField] public LevelData Data { get; set; }
		public abstract void ExecuteAction();
	}
}
