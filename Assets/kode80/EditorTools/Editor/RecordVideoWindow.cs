using UnityEngine;
using UnityEditor;
using System.Collections;

namespace kode80.EditorTools
{
	public class RecordVideoWindow : EditorWindow
	{
		[MenuItem( "Window/kode80/Editor Tools/Record Video")]
		public static void Init()
		{
			RecordVideoWindow win = EditorWindow.GetWindow( typeof( RecordVideoWindow)) as RecordVideoWindow;
			win.titleContent = new GUIContent( "Record Video");
			win.Show();
		}
	}
}