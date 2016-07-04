using UnityEngine;
using System.Collections;

namespace kode80.Common
{
	public class TransformOctree : Octree<Transform>
	{
		public TransformOctree( Bounds bounds, int maxDepth=1) : base( bounds, HandleItemOverlapsBounds, maxDepth)
		{
		}

		private static bool HandleItemOverlapsBounds (Transform item, Bounds bounds)
		{
			return bounds.Contains( item.position);
		}
	}
}