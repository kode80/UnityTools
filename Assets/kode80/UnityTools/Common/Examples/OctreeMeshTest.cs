using UnityEngine;
using System.Collections;
using kode80.Common;

public class OctreeMeshTest : MonoBehaviour 
{
	public Mesh targetMesh;
	[Range( 1, 8)]
	public int maxDepth = 1;
	private MeshOctree octree;

	void OnValidate()
	{
		if( targetMesh != null)
		{
			octree = new MeshOctree( targetMesh, maxDepth);
		}
	}

	void OnDrawGizmos()
	{
		if( octree != null) {
			octree.DrawGizmos( Color.red);
		}
	}
}
