using UnityEngine;
using UnityEditor;
using System.Collections;
using kode80.GUIWrapper;

namespace kode80.EditorTools
{
	public class RecordVideoWindow : EditorWindow
	{
		const string OutputFolderPrefsKey = "kode80.EditorTools.RecordVideoWindow.OutputFolder";

		private GUIVertical _gui;
		private GUIButton _recordButton;
		private RecordVideo _recordVideo;

		[MenuItem( "Window/kode80/Editor Tools/Record Video")]
		public static void Init()
		{
			RecordVideoWindow win = EditorWindow.GetWindow( typeof( RecordVideoWindow)) as RecordVideoWindow;
			win.titleContent = new GUIContent( "Record Video");
			win.Show();
		}

		void OnEnable()
		{
			_recordVideo = FindOrCreateRecordVideo();

			_gui = new GUIVertical();
			_gui.Add( new GUIButton( new GUIContent( OutputFolderButtonText()), PickOutputFolderClicked));
			_gui.Add( new GUISpace());
			_recordButton = _gui.Add( new GUIButton( new GUIContent( "Record"), RecordClicked)) as GUIButton;
			_recordButton.isEnabled = EditorApplication.isPlaying;

			EditorApplication.playmodeStateChanged += PlayModeStateChanged;
		}

		void OnDisable()
		{
			_gui = null;
			_recordButton = null;
			_recordVideo = null;

			EditorApplication.playmodeStateChanged -= PlayModeStateChanged;
		}

		void Update()
		{
			if( EditorApplication.isPlaying && Input.GetKeyDown( KeyCode.R))
			{
				ToggleRecording();
			}
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
			string path = EditorUtility.OpenFolderPanel( "Pick Output Folder", _recordVideo.folderPath, "");
			if( path.Length > 0)
			{
				EditorPrefs.SetString( OutputFolderPrefsKey, path);
				_recordVideo.folderPath = path;
				button.content.text = OutputFolderButtonText();
			}
		}

		void RecordClicked( GUIBase sender)
		{
			ToggleRecording();
		}

		#endregion

		void PlayModeStateChanged()
		{
			_recordVideo = FindOrCreateRecordVideo();
			_recordVideo.StopRecording();
			_recordButton.isEnabled = EditorApplication.isPlaying;
			UpdateRecordButtonText();
		}

		RecordVideo FindOrCreateRecordVideo()
		{
			RecordVideo recordVideo = FindObjectOfType<RecordVideo>();
			if( recordVideo == null)
			{
				GameObject gameObject = new GameObject( "RecordVideo");
				gameObject.hideFlags = HideFlags.HideAndDontSave;
				recordVideo = gameObject.AddComponent<RecordVideo>();
			}

			recordVideo.folderPath = EditorPrefs.GetString( OutputFolderPrefsKey);

			return recordVideo;
		}

		void UpdateRecordButtonText()
		{
			_recordButton.content.text = _recordVideo.isRecording ? "Stop" : "Record";
		}

		string OutputFolderButtonText()
		{
			return _recordVideo.folderPath == null ? "Pick Output Folder" :
													 "Output Folder: " + _recordVideo.folderPath;
		}

		void ToggleRecording()
		{
			if( _recordVideo.isRecording)
			{
				_recordVideo.StopRecording();
			}
			else
			{
				_recordVideo.StartRecording();
			}
			UpdateRecordButtonText();
			Repaint();
		}
	}
}