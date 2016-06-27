using UnityEngine;
using System.Collections;

namespace kode80.Colliders
{
	public class CylinderCollider : MonoBehaviour 
	{
		public float radius = 0.5f;
		public float height = 1.0f;
		[Range( 4, 64)]
		public int boxCount = 5;
		[Range( 0.1f, 3.0f)]
		public float widthScale = 1.0f;
		public bool capTop;
		public bool capBottom;

		[Space]
		public bool alwaysDrawGizmo;

		[Space]
		public bool isTrigger;
		public PhysicMaterial material;
		public Transform colliderParent;

		void Awake () 
		{
			CreateColliders( colliderParent == null ? transform : colliderParent);
			enabled = false;
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		void OnDrawGizmos()
		{
			Gizmos.color = Color.grey;
			OnDrawGizmosSelected();
		}

		void OnDrawGizmosSelected()
		{
			if( Application.isPlaying || enabled == false) { return; }


			float radiusStep = CalculateRotationStep();
			float rotationY = 0.0f;
			Vector3 boxSize =  CalculateBoxSize();

			for( int i=0; i<boxCount; i++)
			{
				var oldMatrix = Gizmos.matrix;
				var newMatrix = Matrix4x4.TRS( Vector3.zero, Quaternion.Euler( 0.0f, rotationY, 0.0f), Vector3.one);
				Gizmos.matrix = transform.localToWorldMatrix * newMatrix;
				Gizmos.DrawWireCube( Vector3.zero, boxSize);
				Gizmos.matrix = oldMatrix;

				rotationY += radiusStep;
			}

			Vector3 capOffset = new Vector3( 0.0f, height * 0.5f, 0.0f);
			if( capBottom) 
			{ 
				var oldMatrix = Gizmos.matrix;
				var newMatrix = Matrix4x4.TRS( capOffset * -1.0f, Quaternion.identity, Vector3.one);
				Gizmos.matrix = transform.localToWorldMatrix * newMatrix;
				Gizmos.DrawWireSphere( Vector3.zero, radius); 
				Gizmos.matrix = oldMatrix;
			}

			if( capTop) 
			{ 
				var oldMatrix = Gizmos.matrix;
				var newMatrix = Matrix4x4.TRS( capOffset, Quaternion.identity, Vector3.one);
				Gizmos.matrix = transform.localToWorldMatrix * newMatrix;
				Gizmos.DrawWireSphere( Vector3.zero, radius); 
				Gizmos.matrix = oldMatrix;
			}
		}

		public void CreateColliders( Transform parent)
		{
			Vector3 boxSize =  CalculateBoxSize();
			float radiusStep = CalculateRotationStep();
			Transform colliderTransform;

			for( int i=0; i<boxCount; i++) {
				colliderTransform = CreateBoxCollider( i, boxSize, radiusStep * i).transform;
				colliderTransform.SetParent( transform, false);
				colliderTransform.SetParent( parent, true);
			}

			if( capTop) {
				colliderTransform = CreateCapCollider( true).transform;
				colliderTransform.SetParent( transform, false);
				colliderTransform.SetParent( parent, true);
			}

			if( capBottom) {
				colliderTransform = CreateCapCollider( false).transform;
				colliderTransform.SetParent( transform, false);
				colliderTransform.SetParent( parent, true);
			}
		}

		private BoxCollider CreateBoxCollider( int index, Vector3 size, float rotationY)
		{
			var collider = new GameObject( "Cylinder_Box_" + index).AddComponent<BoxCollider>();
			collider.size = size;
			collider.transform.localRotation = Quaternion.Euler( 0.0f, rotationY, 0.0f);
			collider.isTrigger = isTrigger;
			collider.material = material;
			return collider;
		}

		private SphereCollider CreateCapCollider( bool isTop)
		{
			var collider = new GameObject( "Cylinder_Cap_" + (isTop ? "Top" : "Bottom")).AddComponent<SphereCollider>();
			collider.radius = radius;
			collider.center = new Vector3( 0.0f, height * (isTop ? 0.5f : -0.5f), 0.0f);
			collider.isTrigger = isTrigger;
			collider.material = material;
			return collider;
		}

		private Vector3 CalculateBoxSize()
		{
			float circumference = radius * 2.0f * Mathf.PI;

			float width = circumference / boxCount;
			width = radius / boxCount * 2.0f * widthScale;

			return new Vector3( width, height, radius * 2.0f);
		}

		private float CalculateRotationStep()
		{
			return 360.0f / boxCount;
		}
	}
}
