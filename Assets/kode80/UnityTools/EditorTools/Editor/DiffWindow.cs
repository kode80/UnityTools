using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace kode80.EditorTools
{
	public class DiffWindow : EditorWindow 
	{
		private GameObject gameObjectA;
		private GameObject gameObjectB;
		private Vector2 scrollPosition;
		private List<DiffRecord> diffs;
		private int selectedIndex = -1;
		private int stopHighlightCount;

		[MenuItem( "Window/kode80/Editor Tools/Diff")]
		public static void Init()
		{
			EditorWindow.GetWindow<DiffWindow>( "Diff").Show();
		}

		void OnEnable()
		{
		}

		void OnDisable()
		{
		}

		void OnGUI()
		{
			if( diffs == null) {
				diffs = new List<DiffRecord>();
			}

			bool refresh = false;
			var newGameObject = EditorGUILayout.ObjectField( gameObjectA, typeof(GameObject), true) as GameObject;
			if( newGameObject != gameObjectA)
			{
				gameObjectA = newGameObject;
				refresh = true;
			}

			newGameObject = EditorGUILayout.ObjectField( gameObjectB, typeof(GameObject), true) as GameObject;
			if( newGameObject != gameObjectB)
			{
				gameObjectB = newGameObject;
				refresh = true;
			}

			if( GUILayout.Button( "Refresh")) {
				refresh = true;
			}

			if( refresh && 
				gameObjectA != null &&
				gameObjectB != null)
			{
				diffs.Clear();
				selectedIndex = -1;

				var diff = new Diff();
				diff.Compare( gameObjectA, gameObjectB, diffs);
			}


			scrollPosition = EditorGUILayout.BeginScrollView( scrollPosition);

			int count = diffs.Count;

			for( int i=0; i<count; i++)
			{
				var record = diffs[ i];

				EditorGUILayout.BeginVertical();

				EditorGUILayout.LabelField( record.gameObjectPath + "." + record.propertyName + " (" + record.propertyType + ") ");

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.BeginVertical();
				if( GUILayout.Button( "Select " + record.gameObjectA.name)) {
					Selection.activeGameObject = record.gameObjectA;
					selectedIndex = i;
				}
				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical();
				if( GUILayout.Button( "Select " + record.gameObjectB.name)) {
					Selection.activeGameObject = record.gameObjectB;
					selectedIndex = i;
				}
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.EndVertical();
				EditorGUILayout.Space();
			}

			EditorGUILayout.EndScrollView();
		}

		void OnSelectionChange()
		{
			if( selectedIndex > -1)
			{
				Highlighter.Highlight( "Inspector", diffs[selectedIndex].propertyName);
				stopHighlightCount = 20;
			}
		}

		void Update()
		{
			if( stopHighlightCount > 0)
			{
				stopHighlightCount--;
				if( stopHighlightCount == 0)
				{
					Highlighter.Stop();
				}
			}
		}
	}
}