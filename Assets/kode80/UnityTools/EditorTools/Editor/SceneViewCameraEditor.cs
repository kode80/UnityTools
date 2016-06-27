using UnityEngine;
using UnityEditor;
using System.Collections;

namespace kode80.EditorTools
{
	[CustomEditor(typeof(SceneViewCamera))]
	public class SceneViewCameraEditor : Editor 
	{
		private int frameCount;
		private bool isForwardsPressed;
		private bool isBackwardsPressed;
		private bool isLeftPressed;
		private bool isRightPressed;
		private bool isUpPressed;
		private bool isDownPressed;
		private bool isRunPressed;

		private Tool previousTool;
		private bool cameraControlsEnabled;
		private float movementSpeed = 0.005f;

		void OnEnable()
		{
			frameCount = 0;
			EditorApplication.update += EditorUpdate;

			previousTool = Tools.current;
			Tools.current = Tool.None;
		}

		void OnDisable()
		{
			EditorApplication.update -= EditorUpdate;
			Tools.current = previousTool;
		}

		void OnSceneGUI()
		{
			Tools.current = Tool.None;

			Handles.BeginGUI();
			GUILayout.BeginArea( new Rect( 5, 5, 200, 100));
			var rect = EditorGUILayout.BeginVertical();
			GUI.Box( rect, GUIContent.none, EditorStyles.helpBox);

			bool newCameraEnabled = GUILayout.Toggle( cameraControlsEnabled, "Camera Enabled");
			if( newCameraEnabled != cameraControlsEnabled)
			{
				cameraControlsEnabled = newCameraEnabled;
				if( cameraControlsEnabled)
				{
					EditorApplication.ExecuteMenuItem( "GameObject/Align With View");
				}
			}
			movementSpeed = GUILayout.HorizontalSlider( movementSpeed, 0.0f, 0.1f);

			EditorGUILayout.EndVertical();
			GUILayout.EndArea();
			Handles.EndGUI();

			HandleCameraKeyInput();
		}

		void HandleCameraKeyInput()
		{
			var currentEvent = Event.current;

			if( currentEvent != null && cameraControlsEnabled)
			{
				if( currentEvent.type == EventType.KeyDown)
				{
					if( currentEvent.keyCode == KeyCode.W) { isForwardsPressed = true; }
					if( currentEvent.keyCode == KeyCode.S) { isBackwardsPressed = true; }
					if( currentEvent.keyCode == KeyCode.A) { isLeftPressed = true; }
					if( currentEvent.keyCode == KeyCode.D) { isRightPressed = true; }
					if( currentEvent.keyCode == KeyCode.Q) { isDownPressed = true; }
					if( currentEvent.keyCode == KeyCode.E) { isUpPressed = true; }

					if( currentEvent.keyCode == KeyCode.C) { isRunPressed = true; }

					currentEvent.Use();
				}
				else if( currentEvent.type == EventType.KeyUp)
				{
					if( currentEvent.keyCode == KeyCode.W) { isForwardsPressed = false; }
					if( currentEvent.keyCode == KeyCode.S) { isBackwardsPressed = false; }
					if( currentEvent.keyCode == KeyCode.A) { isLeftPressed = false; }
					if( currentEvent.keyCode == KeyCode.D) { isRightPressed = false; }
					if( currentEvent.keyCode == KeyCode.Q) { isDownPressed = false; }
					if( currentEvent.keyCode == KeyCode.E) { isUpPressed = false; }

					if( currentEvent.keyCode == KeyCode.C) { isRunPressed = false; }

					currentEvent.Use();
				}
			}
		}

		void EditorUpdate()
		{
			var cameraTransform = (target as SceneViewCamera).transform;

			if( cameraControlsEnabled)
			{
				if( frameCount % 6 == 0)
				{
					Vector3 movement = Vector3.zero;
					float speed = movementSpeed * (isRunPressed ? 3.0f : 1.0f);

					if( isForwardsPressed) { movement += cameraTransform.forward * speed; }
					else if( isBackwardsPressed) { movement -= cameraTransform.forward * speed; }

					if( isLeftPressed) { movement -= cameraTransform.right * speed; }
					else if( isRightPressed) { movement += cameraTransform.right * speed; }

					if( isUpPressed) { movement += cameraTransform.up * speed; }
					else if( isDownPressed) { movement -= cameraTransform.up * speed; }

					cameraTransform.position += movement;

					EditorApplication.ExecuteMenuItem( "GameObject/Align View to Selected");
				}
				frameCount++;
			}
		}
	}
}