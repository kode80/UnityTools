using UnityEngine;
using System.Collections;

namespace kode80.Common
{
	public class MeshOctree : Octree<int>
	{
		private Mesh mesh;

		public MeshOctree( Mesh mesh, int maxDepth = 1) : base( mesh.bounds, null, maxDepth)
		{
			this.mesh = mesh;
			rootNode.itemOverlapsBounds = HandleItemOverlapsBounds;

			int count = mesh.triangles.Length;
			for( int i=0; i<count; i+=3) {
				rootNode.Add( mesh.triangles[i]);
			}
			Regenerate();
		}

		private bool HandleItemOverlapsBounds( int item, Bounds bounds)
		{
			Vector3 vertex = mesh.vertices[ item];
			return bounds.Contains( vertex);
		}
	}
}