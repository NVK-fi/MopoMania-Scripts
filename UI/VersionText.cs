using UnityEngine;

namespace UI
{
	/// <summary>
	/// Grabs the version from Project Settings and updates the attached TextMesh text. 
	/// </summary>
	[RequireComponent(typeof(TextMesh))]
	public class VersionText : MonoBehaviour
	{
		private void Start()
		{
			var textMesh = GetComponent<TextMesh>();
			
			textMesh.text = Application.version;
		}
	}
}
