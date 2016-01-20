using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;
using System.Collections;
using kode80.GUIWrapper;

namespace kode80.EditorTools
{
	public class RecordVideoWindow : EditorWindow
	{
		const string HasInitializedPrefsKey = "kode80.EditorTools.RecordVideoWindow.HasInitialized";
		const string FFmpegPathPrefsKey = "kode80.EditorTools.RecordVideoWindow.FFmpegPath";
		const string OutputFolderPrefsKey = "kode80.EditorTools.RecordVideoWindow.OutputFolder";
		const string SuperSizePrefsKey = "kode80.EditorTools.RecordVideoWindow.SuperSize";
		const string FrameratePrefsKey = "kode80.EditorTools.RecordVideoWindow.Framerate";
		const string OutputFormatPrefsKey = "kode80.EditorTools.RecordVideoWindow.OutputFormat";

		enum OutputFormat
		{
			Frames = 0,
			FramesAndMP4 = 1,
			FramesAndGIF = 2
		}

		private GUIVertical _gui;
		private GUIButton _recordButton;
		private RecordVideo _recordVideo;
		private int _sceneCount = 0;

		[MenuItem( "Window/kode80/Editor Tools/Record Video")]
		public static void Init()
		{
			RecordVideoWindow win = EditorWindow.GetWindow( typeof( RecordVideoWindow)) as RecordVideoWindow;
			win.titleContent = new GUIContent( "Record Video");
			win.Show();
		}

		void OnEnable()
		{
			InitializedPrefsIfNeeded();

			_recordVideo = FindOrCreateRecordVideo();

			_gui = new GUIVertical();
			_gui.Add( new GUIButton( new GUIContent( FFmpegPathButtonText(), FFmpegPathButtonText()), PickFFmpegPathClicked));
			_gui.Add( new GUIButton( new GUIContent( OutputFolderButtonText(), OutputFolderButtonText()), PickOutputFolderClicked));
			_gui.Add( new GUIIntSlider( new GUIContent( "Super Size", 
														"Frames will be rendered at this multiple of the current resolution"), 
										_recordVideo.superSize, 1, 4, SuperSizeChanged));
			_gui.Add( new GUIIntSlider( new GUIContent( "Framerate", 
														"The fixed framerate of recorded video"), 
										_recordVideo.captureFramerate, 10, 60, FramerateChanged));
			_gui.Add( new GUIEnumPopup( new GUIContent( "Output Format"), 
										(OutputFormat) EditorPrefs.GetInt( OutputFormatPrefsKey), 
										OutputFormatChanged));
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
				button.content.tooltip = OutputFolderButtonText();
			}
		}

		void PickFFmpegPathClicked( GUIBase sender)
		{
			GUIButton button = sender as GUIButton;
			string oldPath = EditorPrefs.GetString( FFmpegPathPrefsKey);
			string path = EditorUtility.OpenFilePanel( "Pick FFmpeg Path", oldPath, "");
			if( path.Length > 0)
			{
				EditorPrefs.SetString( FFmpegPathPrefsKey, path);
				button.content.text = FFmpegPathButtonText();
				button.content.tooltip = FFmpegPathButtonText();
			}
		}

		void SuperSizeChanged( GUIBase sender)
		{
			GUIIntSlider slider = sender as GUIIntSlider;
			EditorPrefs.SetInt( SuperSizePrefsKey, slider.value);
			_recordVideo.superSize = slider.value;
		}

		void FramerateChanged( GUIBase sender)
		{
			GUIIntSlider slider = sender as GUIIntSlider;
			EditorPrefs.SetInt( FrameratePrefsKey, slider.value);
			_recordVideo.captureFramerate = slider.value;
		}

		void OutputFormatChanged( GUIBase sender)
		{
			GUIEnumPopup popup = sender as GUIEnumPopup;
			EditorPrefs.SetInt( OutputFormatPrefsKey, (int)(OutputFormat)popup.value);
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

			if( EditorApplication.isPlayingOrWillChangePlaymode == false && EditorApplication.isPlaying == false)
			{
				CompileVideos();
			}
		}

		void InitializedPrefsIfNeeded()
		{
			bool hasInitialized = EditorPrefs.GetBool( HasInitializedPrefsKey);
			if( hasInitialized == false)
			{
				EditorPrefs.DeleteKey( FFmpegPathPrefsKey);
				EditorPrefs.DeleteKey( OutputFolderPrefsKey);
				EditorPrefs.SetInt( SuperSizePrefsKey, 1);
				EditorPrefs.SetInt( FrameratePrefsKey, 60);
				EditorPrefs.SetInt( OutputFormatPrefsKey, (int)OutputFormat.FramesAndMP4);
				EditorPrefs.SetBool( HasInitializedPrefsKey, true);
			}
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
			recordVideo.superSize = EditorPrefs.GetInt( SuperSizePrefsKey);
			recordVideo.captureFramerate = EditorPrefs.GetInt( FrameratePrefsKey);

			return recordVideo;
		}

		void UpdateRecordButtonText()
		{
			_recordButton.content.text = _recordVideo.isRecording ? "Stop" : "Record";
		}

		string FFmpegPathButtonText()
		{
			string path = EditorPrefs.GetString( FFmpegPathPrefsKey);
			return path.Length == 0 ? "Pick FFmpeg Path" :
									  "FFmpeg Path: " + path;
		}

		string OutputFolderButtonText()
		{
			bool pathNotSet = _recordVideo.folderPath == null || _recordVideo.folderPath.Length == 0;
			return pathNotSet ? "Pick Output Folder" :
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
				_sceneCount = _recordVideo.sceneNumber + 1;
			}
			UpdateRecordButtonText();
			Repaint();
		}

		void CompileVideos()
		{
			OutputFormat format = (OutputFormat) EditorPrefs.GetInt( OutputFormatPrefsKey);
			if( format != OutputFormat.Frames)
			{
				for( int i=0; i<_sceneCount; i++)
				{
					CompileVideo( i, format == OutputFormat.FramesAndGIF);
				}
				_sceneCount = 0;
			}
		}

		void CompileVideo( int sceneNumber, bool createGif=false)
		{
			string path = EditorPrefs.GetString( FFmpegPathPrefsKey);

			if( path.Length > 0 && File.Exists( path))
			{
				int framerate = EditorPrefs.GetInt( FrameratePrefsKey);
				string outputPath = EditorPrefs.GetString( OutputFolderPrefsKey);
				string args = createGif ?
							  string.Format( "-i \"Scene{0:D03}Frame%08d.png\" -y \"Scene{1:D03}.gif\"", 
											 sceneNumber, sceneNumber) :
							  string.Format( "-i \"Scene{0:D03}Frame%08d.png\" -y -c:v libx264 -pix_fmt yuv420p -r {1} -preset ultrafast \"Scene{2:D03}.mp4\"", 
						 					 sceneNumber, framerate, sceneNumber);
				
				Process process = new Process();
				process.StartInfo.FileName = path;
				process.StartInfo.Arguments = args;
				process.StartInfo.WorkingDirectory = outputPath;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardError = true;

				process.Start();
				UnityEngine.Debug.Log( "FFmpeg Output: " + process.StandardError.ReadToEnd());
				process.WaitForExit();
			}
		}
	}
}