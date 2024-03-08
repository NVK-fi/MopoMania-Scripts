namespace Tools
{
	using UnityEngine;
	using UnityEngine.SceneManagement;
	
	/// <summary>
	/// Automatically snaps the points of a Polygon Collider 2D to the grid and creates a mesh from it.
	/// This component is intended for use in the Unity Editor to create levels with.
	/// </summary>
	[RequireComponent(typeof(PolygonCollider2D))]
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(MeshFilter))]
	public class PolygonCollider2DMesher : MonoBehaviour
	{
#if UNITY_EDITOR

		[field: SerializeField] private bool showDebug;
		
		/// <summary>
		/// Snaps the points of Polygon Collider 2D to the grid.
		/// </summary>
		public void Snap()
		{
			var polygonCollider2D = GetComponent<PolygonCollider2D>();
			
			for (var pathIndex = 0; pathIndex < polygonCollider2D.pathCount; pathIndex++)
			{
				var path = polygonCollider2D.GetPath(pathIndex);

				var roundedPointsCounter = 0;
				for (long pointIndex = 0; pointIndex < path.Length; pointIndex++)
				{
					var currentPoint = path[pointIndex];
					
					var roundedPoint = currentPoint;
					roundedPoint.x = Mathf.Round(currentPoint.x * 4f) * .25f;
					roundedPoint.y = Mathf.Round(currentPoint.y * 4f) * .25f;
					
					if (currentPoint == roundedPoint) continue;

					path[pointIndex] = roundedPoint;
					roundedPointsCounter++;
				}
				
				polygonCollider2D.points = path;
				polygonCollider2D.SetPath(pathIndex, path);

				if (showDebug)
					print(roundedPointsCounter + " points rounded.");
			}
		}
		
		/// <summary>
		/// Creates a mesh from the polygon collider, then saves and applies it.
		/// </summary>
		public void UpdateMesh()
		{
			var polygonCollider2D = GetComponent<PolygonCollider2D>();
			var meshFilter = GetComponent<MeshFilter>();
			
			// Create the mesh.
			var mesh = polygonCollider2D.CreateMesh(false, false);
			mesh.hideFlags = HideFlags.None;
			
			// Save the mesh.
			var meshName = SceneManager.GetActiveScene().name + "_Mesh_" + polygonCollider2D.GetHashCode();
			var meshPath = "Assets/Meshes/" + meshName + ".asset";
			UnityEditor.AssetDatabase.CreateAsset(mesh, meshPath);
			UnityEditor.AssetDatabase.SaveAssets();
			UnityEditor.AssetDatabase.Refresh();
			
			// Apply the mesh to the mesh filter.
			meshFilter.mesh = mesh;

			if (showDebug)
				Debug.Log("Mesh saved at: " + meshPath);
		}
		
#endif
		
	}
}
