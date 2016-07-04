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
	private OctreeNode<Transform> rootNode;
	private List<OctreeNode<Transform>> foundNodes;

	// Use this for initialization
	void Start () {
	
	}

	void Update()
	{
		RecreateOctree();

		#if UNITY_EDITOR
		if( Selection.activeGameObject != null) {
			foundNodes = rootNode.GetNodesContainingItem( Selection.activeGameObject.transform);
		}
		else {
			foundNodes = null;
		}
		#endif
	}

	void OnDrawGizmos()
	{
		if( rootNode == null) { return; }

		Gizmos.color = Color.red;
		DrawOctree( rootNode);

		if( foundNodes != null)
		{
			Gizmos.color = Color.green;
			foreach( var node in foundNodes) {
				Gizmos.DrawWireCube( node.Bounds.center, node.Bounds.size);
			}
		}
	}

	private void RecreateOctree()
	{
		Transform[] transforms = GetComponentsInChildren<Transform>( true);
		Bounds rootBounds = new Bounds();
		List<Transform> contents = new List<Transform>();

		foreach( Transform t in transforms) 
		{
			if( t != transform)
			{
				rootBounds.Encapsulate(t.position);
				contents.Add( t);
			}
		}

		rootNode = new OctreeNode<Transform>( rootBounds, HandleItemOverlapsBounds);
		rootNode.AddItem( contents);
		SubdivideOctree( rootNode, 0, 8);
	}

	private bool HandleItemOverlapsBounds(Transform item, Bounds bounds)
	{
		return bounds.Contains( item.position);
	}

	private void DrawOctree( OctreeNode<Transform> node)
	{
		if( node.SubNodes == null && node.Contents.Count == 0) { return; }
			
		Gizmos.DrawWireCube( node.Bounds.center, node.Bounds.size);
		if( node.SubNodes != null)
		{
			foreach( OctreeNode<Transform> subNode in node.SubNodes) {
				DrawOctree( subNode);
			}
		}
	}

	private void SubdivideOctree( OctreeNode<Transform> node, int currentDepth, int maxDepth)
	{
		if( node.Contents.Count > 1 && currentDepth < maxDepth) {
			node.Subdivide();
			foreach( OctreeNode<Transform> subNode in node.SubNodes) {
				SubdivideOctree( subNode, currentDepth+1, maxDepth);
			}
		}
	}
}
