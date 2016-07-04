using UnityEngine;
using System.Collections;

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

		private void Regenerate()
		{
			rootNode.Collapse();
		}
	}
}