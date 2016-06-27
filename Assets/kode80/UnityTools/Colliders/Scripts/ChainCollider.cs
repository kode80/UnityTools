using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace kode80.Colliders
{
	public class ChainCollider : MonoBehaviour 
	{
		public Vector2 squareSize = Vector3.one;
		[Range( -1.0f, 1.0f)]
		public float overshoot = 0.0f;
		public bool isClosed;
		public Vector3[] points = new Vector3[] { -Vector3.one, Vector3.one };

		[Space]
		public bool isTrigger;
		public PhysicMaterial material;
		public Transform colliderParent;

		void Awake () 
		{
			CreateColliders( colliderParent == null ? transform : colliderParent);
			enabled = false;
		}

		void Update () 
		{
		}

		void OnDrawGizmos()
		{
			Gizmos.color = Color.grey;
			OnDrawGizmosSelected();
		}

		void OnDrawGizmosSelected()
		{
			if( Application.isPlaying || enabled == false) { return; }


			int count = points.Length;
			if( isClosed == false) { count--; }

			for( int i=0; i<count; i++)
			{
				var p0 = points[i];
				var p1 = points[(i+1)%points.Length];

				var delta = p1 - p0;
				var direction = delta.normalized;
				var size = new Vector3( squareSize.x, squareSize.y, Vector3.Distance( p0, p1) + overshoot);
				var rotation = p0 == p1 ? Quaternion.identity : Quaternion.LookRotation( direction);

				var oldMatrix = Gizmos.matrix;
				var newMatrix = Matrix4x4.TRS( p0 + delta * 0.5f, rotation, Vector3.one);
				newMatrix = transform.localToWorldMatrix * newMatrix;
				Gizmos.matrix = newMatrix;
				Gizmos.DrawWireCube( Vector3.zero, size);
				Gizmos.matrix = oldMatrix;
			}
		}

		public void CreateColliders( Transform parent)
		{
			int count = points.Length;
			if( isClosed == false) { count--; }

			for( int i=0; i<count; i++)
			{
				var colliderTransform = CreateBoxCollider( i).transform;
				colliderTransform.SetParent( transform, false);
				colliderTransform.SetParent( parent, true);
			}
		}

		private BoxCollider CreateBoxCollider( int index)
		{
			var p0 = points[index];
			var p1 = points[(index + 1)%points.Length];

			var delta = p1 - p0;
			var direction = delta.normalized;
			var size = new Vector3( squareSize.x, squareSize.y, Vector3.Distance( p0, p1) + overshoot);
			var rotation = p0 == p1 ? Quaternion.identity : Quaternion.LookRotation( direction);

			var collider = new GameObject( "Chain_Box_" + index).AddComponent<BoxCollider>();
			collider.size = size;
			collider.transform.localPosition = p0 + delta * 0.5f;
			collider.transform.localRotation = rotation;
			collider.isTrigger = isTrigger;
			collider.material = material;

			return collider;
		}

		public void InsertPoint( int index)
		{
			index = Math.Min( Math.Max( index, 0), points.Length);

			Vector3 before = points[Math.Min( Math.Max( index-1, 0), points.Length-1)];
			Vector3 after = points[Math.Min( Math.Max( index, 0), points.Length-1)];
			Vector3 newPoint = before + ((after - before) * 0.5f);

			var list = points.ToList();
			list.Insert( index, newPoint);
			points = list.ToArray();
		}

		public void DeletePoint( int index)
		{
			index = Math.Min( Math.Max( index, 0), points.Length - 1);

			var list = points.ToList();
			list.RemoveAt( index);
			points = list.ToArray();
		}

		public void ApplyParentRotation()
		{
			int count = points.Length;
			for( int i=0; i<count; i++)
			{
				points[i] = transform.localRotation * points[i];
			}
			transform.localRotation = Quaternion.identity;
		}
	}
}