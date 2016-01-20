using UnityEngine;
using UnityEditor;
using System.Collections;
using kode80.GUIWrapper;

namespace kode80.EditorTools
{
	public class RecordVideoWindow : EditorWindow
	{
		private GUIVertical _gui;
		private GUIButton _recordButton;

		[MenuItem( "Window/kode80/Editor Tools/Record Video")]
		public static void Init()
		{
			RecordVideoWindow win = EditorWindow.GetWindow( typeof( RecordVideoWindow)) as RecordVideoWindow;
			win.titleContent = new GUIContent( "Record Video");
			win.Show();
		}

		void OnEnable()
		{
			_gui = new GUIVertical();
			_gui.Add( new GUIButton( new GUIContent( "Pick Output Folder"), PickOutputFolderClicked));
			_gui.Add( new GUISpace());
			_recordButton = _gui.Add( new GUIButton( new GUIContent( "Record"), PickOutputFolderClicked)) as GUIButton;
			_recordButton.isEnabled = false;

			EditorApplication.playmodeStateChanged += PlayModeStateChanged;
		}

		void OnDisable()
		{
			_gui = null;
			_recordButton = null;

			EditorApplication.playmodeStateChanged -= PlayModeStateChanged;
		}

		void OnGUI()
		{
			if( _gui != null)
			{
				_gui.OnGUI();
			}
		}

		#region GUI Actions

		void PickOutputFolderClicked( GUIBase sender)
		{
			GUIButton button = sender as GUIButton;
		}

		void RecordClicked( GUIBase sender)
		{
		}

		#endregion

		void PlayModeStateChanged()
		{
			_recordButton.isEnabled = EditorApplication.isPlaying;
		}
	}
}