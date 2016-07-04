using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using kode80.Common;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class OctreeTransformTest : MonoBehaviour 
{
	public bool recreateOctree;
	private Octree<Transform> octree;
	private List<OctreeNode<Transform>> foundNodes;

	// Use this for initialization
	void Start () {
	
	}

	void Update()
	{
		RecreateOctree();

		#if UNITY_EDITOR
		if( Selection.activeGameObject != null) {
			foundNodes = octree.GetNodesContainingItem( Selection.activeGameObject.transform);
		}
		else {
			foundNodes = null;
		}
		#endif
	}

	void OnDrawGizmos()
	{
		if( octree == null) { return; }

		octree.DrawGizmos( Color.red);

		if( foundNodes != null)
		{
			Gizmos.color = Color.green;
			foreach( var node in foundNodes) {
				node.DrawGizmo();
			}
		}
	}

	private void RecreateOctree()
	{
		Transform[] transforms = GetComponentsInChildren<Transform>( true);
		Bounds bounds = new Bounds();
		List<Transform> contents = new List<Transform>();

		foreach( Transform t in transforms) 
		{
			if( t != transform)
			{
				bounds.Encapsulate(t.position);
				contents.Add( t);
			}
		}

		octree = new Octree<Transform>( bounds, HandleItemOverlapsBounds, 8);
		octree.Add( contents);
	}

	private bool HandleItemOverlapsBounds(Transform item, Bounds bounds)
	{
		return bounds.Contains( item.position);
	}
}
