using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace kode80.Common
{
	public class Octree<T>
	{
		private OctreeNode<T> rootNode;

		private int maxDepth;
		public int MaxDepth {
			get { return maxDepth; }
			set {
				if( maxDepth != value)
				{
					maxDepth = value;
					Regenerate();
				}
			}
		}

		public Bounds Bounds { get { return rootNode.Bounds; } }

		public Octree( Bounds bounds, OctreeNode<T>.ItemOverlapsBounds itemOverlapsBounds, int maxDepth = 1)
		{
			rootNode = new OctreeNode<T>( bounds, itemOverlapsBounds);
			MaxDepth = maxDepth;
		}

		public void Add( T item)
		{
			rootNode.Add( item);
			Regenerate();
		}

		public void Add( IList<T> items)
		{
			rootNode.Add( items);
			Regenerate();
		}

		public List<OctreeNode<T>> GetNodesContainingItem( T item)
		{
			return rootNode.GetNodesContainingItem( item);
		}

		public void DrawGizmos( Color color)
		{
			Color previousColor = Gizmos.color;
			Gizmos.color = color;
			DrawGizmos( rootNode);
			Gizmos.color = previousColor;
		}

		private void DrawGizmos( OctreeNode<T> node)
		{
			if( node.SubNodes == null) {
				Gizmos.DrawWireCube( node.Bounds.center, node.Bounds.size);
			}
			else {
				for( int i=0; i<8; i++) {
					node.SubNodes[i].DrawGizmo();
					DrawGizmos( node.SubNodes[i]);
				}
			}
		}

		private void Regenerate()
		{
			rootNode.Collapse();
			SubdivideOctree( rootNode, 0, maxDepth);
		}

		private void SubdivideOctree( OctreeNode<T> node, int currentDepth, int maxDepth)
		{
			if( node.Contents.Count > 1 && currentDepth < maxDepth) 
			{
				node.Subdivide();
				foreach( OctreeNode<T> subNode in node.SubNodes) {
					SubdivideOctree( subNode, currentDepth+1, maxDepth);
				}
			}
		}
	}
}