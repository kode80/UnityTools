using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

namespace kode80.Colliders
{
	[CustomEditor( typeof(ChainCollider))]
	public class ChainColliderEditor : Editor
	{
		private int selectedPointIndex;

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			if( GUILayout.Button( "Bake Colliders"))
			{
				var chain = target as ChainCollider;
				chain.CreateColliders( chain.colliderParent == null ? chain.transform : chain.colliderParent);
				chain.enabled = false;
			}
		}

		void OnSceneGUI()
		{
			var chain = target as ChainCollider;
			if( chain.enabled == false) { return; }

			int count = chain.points.Length;

			for( int i=0; i<count; i++)
			{
				var point = chain.transform.localToWorldMatrix.MultiplyPoint( chain.points[i]);

				if( selectedPointIndex == i)
				{
					var newPoint = Handles.PositionHandle( point, Quaternion.identity);
					if( newPoint != point)
					{
						Undo.RecordObject( chain, "Moved Chain Collider Point");
						point = newPoint;
					}
				}
				else
				{
					var size = HandleUtility.GetHandleSize(point);
					if( Handles.Button(point, Quaternion.identity, size * 0.1f, size * 0.1f, Handles.SphereCap)) {
						selectedPointIndex = i;
					}
				}


				point = chain.transform.worldToLocalMatrix.MultiplyPoint( point);
				chain.points[i] = point;
			}

			Handles.BeginGUI();
			GUILayout.BeginArea( new Rect( 5, 5, 140, 100));
			var rect = EditorGUILayout.BeginVertical();
			GUI.Box( rect, GUIContent.none, EditorStyles.helpBox);

			if( GUILayout.Button( "Insert Point Before")) { 
				Undo.RecordObject( chain, "Inserted Chain Collider Point");
				chain.InsertPoint( selectedPointIndex); 
			}
			if( GUILayout.Button( "Insert Point After")) { 
				Undo.RecordObject( chain, "Inserted Chain Collider Point");
				chain.InsertPoint( selectedPointIndex + 1); 
				selectedPointIndex++;
			}
			if( GUILayout.Button( "Delete Point")) { 
				Undo.RecordObject( chain, "Deleted Chain Collider Point");
				chain.DeletePoint( selectedPointIndex); 
				selectedPointIndex = Math.Min( selectedPointIndex, chain.points.Length - 1);
			}

			GUILayout.Space( 10);

			if( GUILayout.Button( "Apply Parent Rotation")) {
				Undo.RecordObject( chain, "Apply Chain Collider Parent Rotation");
				chain.ApplyParentRotation(); 
			}

			EditorGUILayout.EndVertical();
			GUILayout.EndArea();
			Handles.EndGUI();
		}
	}
}