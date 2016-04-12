using UnityEngine;
using UnityEditor;
using System;
using System.IO;
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
			string[] paths = Directory.GetFiles( Application.dataPath, "AssetVersion.xml", SearchOption.AllDirectories);
			foreach( string path in paths)
			{
				string localXML = File.ReadAllText( path);
				AssetVersion localVersion = AssetVersion.ParseXML( localXML);
				AssetVersion remoteVersion = DownloadRemoteVersionInfo( localVersion);

				string status = remoteVersion.Version > localVersion.Version ? "out of date" : "up to date";
				Debug.Log( localVersion.Name + " by " + localVersion.Author + " is " + status);
			}


		}

		private AssetVersion DownloadRemoteVersionInfo( AssetVersion localVersion)
		{
			WebClient client = new WebClient();
			string xmlString = client.DownloadString( localVersion.versionURI);

			return AssetVersion.ParseXML( xmlString);
		}
	}
}