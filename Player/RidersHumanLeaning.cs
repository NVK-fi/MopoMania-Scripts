using UnityEngine;

namespace Player
{
	using Tools;

	/// <summary>
	/// Controls the leaning animation of the rider based on the player's rotation input and the direction of the bike.
	/// </summary>
	public class RidersHumanLeaning : MonoBehaviour
	{
		private Animator _animator;
		private PlayerRefs _playerRefs;
		[SerializeField] private string parameterName = "Leaning";
		
		private void Awake()
		{
			_playerRefs = transform.root.GetComponent<PlayerRefs>();
			
			_animator = GetComponent<Animator>();
			this.IsReferenceNull(_animator);
		}

		private void OnEnable()
		{
			_playerRefs.States.OnRotatingStateChange += UpdateLeaning;
			_playerRefs.States.OnFlipStateChange += UpdateLeaning;
		}

		private void OnDisable()
		{
			_playerRefs.States.OnRotatingStateChange -= UpdateLeaning;
			_playerRefs.States.OnFlipStateChange -= UpdateLeaning;
		}

		/// <summary>
		/// Updates the animator's leaning integer based on the player's rotation input and direction of the bike.  
		/// </summary>
		private void UpdateLeaning()
		{
			// Get the leaning integer (1, 0 or -1) from an enum.
			var leaningInteger = (int)_playerRefs.States.RotatingState;

			// Reverse the leaning if the bike is flipped.
			leaningInteger *= (_playerRefs.States.IsFlipped ? -1 : 1);
			
			// Set the parameter.
			_animator.SetInteger(parameterName, leaningInteger);
		}
	}
}
