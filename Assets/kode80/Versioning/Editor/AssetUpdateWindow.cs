using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Net;
using kode80.GUIWrapper;

namespace kode80.Versioning
{
	public class AssetUpdateWindow : EditorWindow 
	{
		private GUIVertical _gui;
		private List<AssetVersion> _localVersions;

		[MenuItem( "Window/kode80/Check for Asset Updates")]
		public static void Init()
		{
			AssetUpdateWindow win = EditorWindow.GetWindow( typeof( AssetUpdateWindow)) as AssetUpdateWindow;
			win.titleContent = new GUIContent( "Asset Updater");
			win.Show();
		}

		void OnEnable()
		{
			FindLocalVersions();
			CreateGUI();
		}

		void OnDisable()
		{
			_gui = null;
		}

		void OnGUI()
		{
			if( _gui != null)
			{
				_gui.OnGUI();
			}
		}

		private void DownloadButtonPressed( GUIBase sender)
		{
			AssetVersion version = _localVersions[ sender.tag];
			AssetVersion remoteVersion = DownloadRemoteVersionInfo( version);
			Debug.Log( (version.Version < remoteVersion.Version) ? "Local version out of date" : "Latest version installed");

			Application.OpenURL( Uri.EscapeUriString( remoteVersion.packageURI.ToString()));
		}

		private void ReleaseNotesButtonPressed( GUIBase sender)
		{
			AssetVersion version = _localVersions[ sender.tag];
			AssetVersion remoteVersion = DownloadRemoteVersionInfo( version);
			Debug.Log( (version.Version < remoteVersion.Version) ? "Local version out of date" : "Latest version installed");

			string title = remoteVersion.Name + " (" + remoteVersion.Version + ") Release Notes";
			EditorUtility.DisplayDialog( title, remoteVersion.Notes, "OK");
		}

		private AssetVersion DownloadRemoteVersionInfo( AssetVersion localVersion)
		{
			WebClient client = new WebClient();
			Debug.Log( "Downloading: " + localVersion.versionURI);
			string xmlString = client.DownloadString( localVersion.versionURI);

			return AssetVersion.ParseXML( xmlString);
		}

		private void CreateGUI()
		{
			_gui = new GUIVertical();
			GUIScrollView scrollView = _gui.Add( new GUIScrollView()) as GUIScrollView;
			scrollView.Add( new GUILabel( new GUIContent( "Installed Assets")));

			GUIStyle style = CreateBackgroundStyle( 55, 70);

			for( int i=0; i<_localVersions.Count; i++)
			{
				AssetVersion version = _localVersions[i];
				GUIHorizontal bar = scrollView.Add( new GUIHorizontal( style)) as GUIHorizontal;
				GUIVertical infoContainer = bar.Add( new GUIVertical()) as GUIVertical;
				infoContainer.Add( new GUILabel( new GUIContent( version.Name + " (" + version.Version + ")")));
				infoContainer.Add( new GUILabel( new GUIContent( version.Author)));

				GUIButton button = bar.Add( new GUIButton( new GUIContent( "Release Notes"), ReleaseNotesButtonPressed)) as GUIButton;
				button.tag = i;

				button = bar.Add( new GUIButton( new GUIContent( "Download"), DownloadButtonPressed)) as GUIButton;
				button.tag = i;
			}
		}

		private GUIStyle CreateBackgroundStyle( byte gray0, byte gray1)
		{
			const int height = 64;
			float gray = gray0;
			float step = ((float)gray1 - (float)gray0) / (float)height;

			GUIStyle style = new GUIStyle();
			Texture2D texture = new Texture2D( 1, height);
			for( int i=0; i<height; i++) {
				byte g = (byte)gray;
				texture.SetPixel( 0, i, new Color32( g, g, g, 255));
				gray += step;
			}
			texture.Apply();
			style.normal.background = texture;

			return style;
		}

		private void FindLocalVersions()
		{
			_localVersions = new List<AssetVersion>();
			string[] paths = Directory.GetFiles( Application.dataPath, "AssetVersion.xml", SearchOption.AllDirectories);

			foreach( string path in paths)
			{
				string localXML = File.ReadAllText( path);
				AssetVersion localVersion = AssetVersion.ParseXML( localXML);
				if( localVersion != null) {
					_localVersions.Add( localVersion);
				}
			}
		}
	}
}