using System.Collections.Generic;

namespace Tools
{
    /// <summary>
    /// A collection of utility methods for working with edges.
    /// </summary>
	public static class EdgeHelpers
	{
        /// <summary>
        /// Represents an edge with its vertices and triangle index.
        /// </summary>
		public struct Edge
		{
			public readonly int Vertex1;
			public readonly int Vertex2;
			public int TriangleIndex;

            /// <summary>
            /// Creates a new Edge.
            /// </summary>
            /// <param name="vertex1">The first vertex of the edge.</param>
            /// <param name="vertex2">The second vertex of the edge.</param>
            /// <param name="triangleIndex">The index of the triangle that the edge belongs to.</param>
			public Edge(int vertex1, int vertex2, int triangleIndex)
			{
				Vertex1 = vertex1;
				Vertex2 = vertex2;
				TriangleIndex = triangleIndex;
			}
		}
        
        /// <summary>
        /// Filters a list of edges to include only those edges that are on the boundary of a mesh.
        /// </summary>
        /// <param name="edges">The list of edges to filter.</param>
        /// <returns>List of boundary edges.</returns>
		public static List<Edge> FindBoundary(this List<Edge> edges)
		{
			var result = new List<Edge>(edges);
			
			// Loop through the edges from last to first, to prevent OBOE.
			for (var i = result.Count - 1; i > 0; i--)
			{
				for (var n = i - 1; n >= 0; n--)
				{
					
					// If duplicate edges are found, aka a shared edge, remove them.
					if (result[i].Vertex1 == result[n].Vertex2 && result[i].Vertex2 == result[n].Vertex1)
					{
						result.RemoveAt(i);
						result.RemoveAt(n);
						i--;
						break;
					}
				}
			}

			// We are left with a list of non-shared edges.
			return result;
		}

        /// <summary>
        /// Creates a list of edges from an array of vertices arranged as triangles.
        /// </summary>
        /// <param name="indices">Array of indices representing vertices of triangles.</param>
        /// <returns>A list of edges.</returns>
		public static List<Edge> GetEdges(int[] indices)
		{
			var result = new List<Edge>();
			
			for (var i = 0; i < indices.Length; i += 3)
			{
				var vertex1 = indices[i];
				var vertex2 = indices[i + 1];
				var vertex3 = indices[i + 2];
				result.Add(new Edge(vertex1, vertex2, i));
				result.Add(new Edge(vertex2, vertex3, i));
				result.Add(new Edge(vertex3, vertex1, i));
			}

			return result;
		}

        /// <summary>
        /// Sorts the given list of edges in order of their connection.
        /// </summary>
        /// <param name="edges">The list of edges to sort.</param>
        /// <returns>A sorted list of edges.</returns>
		public static List<Edge> SortEdges(this List<Edge> edges)
		{
			var sortedEdges = new List<Edge>(edges);
			
			// Go through the edges in order.
			for (var i = 0; i < sortedEdges.Count - 2; i++)
			{
				var currentEdge = sortedEdges[i];
				
				// Go through the remaining edges in the list.
				for (var j = i + 1; j < sortedEdges.Count; j++)
				{
					var nextEdge = sortedEdges[j];
					
					// Test if the next edge is found.
					if (currentEdge.Vertex2 != nextEdge.Vertex1) continue;
					
					// Test if the edges are already in order.
					if (j == i + 1)
						break;
					
					// If an unsorted match is found, swap the "currentEdge + 1" with the match.
					sortedEdges[j] = sortedEdges[i + 1];
					sortedEdges[i + 1] = nextEdge;
					break;
				}
			}

			return sortedEdges;
		}
	}
}