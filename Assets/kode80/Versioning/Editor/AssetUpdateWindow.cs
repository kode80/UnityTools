using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Xml;
using System.Net;
using kode80.GUIWrapper;

namespace kode80.Versioning
{
	public class AssetUpdateWindow : EditorWindow 
	{
		private GUIVertical _gui;

		[MenuItem( "Window/kode80/Update")]
		public static void Init()
		{
			AssetUpdateWindow win = EditorWindow.GetWindow( typeof( AssetUpdateWindow)) as AssetUpdateWindow;
			win.titleContent = new GUIContent( "Asset Update");
			win.Show();
		}

		void OnEnable()
		{
			Uri versionURI = new Uri( "http://kode80.com/assets/version/UnityTools.xml");
			Debug.Log( versionURI);

			WebClient client = new WebClient();
			string xmlString = client.DownloadString( versionURI);

			AssetVersion assetVersion = AssetVersion.ParseXML( xmlString);
			if( assetVersion != null)
			{
				Debug.Log( assetVersion);
			}
		}
	}
}