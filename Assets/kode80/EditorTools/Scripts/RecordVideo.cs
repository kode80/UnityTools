using UnityEngine;
using System.IO;
using System.Collections;

namespace kode80.EditorTools
{
	// The RecordVideo component is used by RecordVideoWindow
	// to record frames during play mode and should't be added
	// manually by the user, so hide it from the component menu.
	[AddComponentMenu("")]
	public class RecordVideo : MonoBehaviour 
	{
		public int captureFramerate = 60;
		public int superSize = 1;
		public string folderPath;

		private bool _isRecording = false;
		public bool isRecording { get { return _isRecording; } }
		private int _sceneNumber = 0;
		private int _frameNumber = 0;

		void Start () 
		{
			Time.captureFramerate = captureFramerate;
		}

		void Update () 
		{
			if( _isRecording)
			{
				string path = string.Format("{0}/Scene{1:D03}Frame{2:D08}.png", folderPath, _sceneNumber, _frameNumber );
				Application.CaptureScreenshot( path, superSize);
				_frameNumber++;
			}
		}

		public bool StartRecording()
		{
			if( _isRecording == false && Directory.Exists( folderPath))
			{
				_isRecording = true;
			}

			return _isRecording;
		}

		public void StopRecording()
		{
			if( _isRecording == true)
			{
				_isRecording = false;
				_sceneNumber++;
				_frameNumber = 0;
			}
		}
	}
}
