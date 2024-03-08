using UnityEngine;

namespace Tools
{
	/// <summary>
	/// Limits the framerate so we wouldn't hear any coil whine from the GPU.
	/// </summary>
	public class FPSLimiter : MonoBehaviour
	{
		[SerializeField] private int maxFPS = 200;

		private void Awake()
		{
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = maxFPS;
		}
	}
}
