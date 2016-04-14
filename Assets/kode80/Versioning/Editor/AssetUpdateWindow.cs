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
		private List<GUILabel> _assetUpdateLabels;
		private List<GUIHorizontal> _assetUpdateButtonContainers;

		[MenuItem( "Window/kode80/Check for Asset Updates")]
		public static void Init()
		{
			AssetUpdateWindow win = EditorWindow.GetWindow( typeof( AssetUpdateWindow)) as AssetUpdateWindow;
			win.titleContent = new GUIContent( "Asset Updater");
			win.Show();
		}

		void OnEnable()
		{
			AssetUpdater.Instance.Refresh();
			CreateGUI();
		}

		void OnDisable()
		{
			_gui = null;
			_assetUpdateLabels = null;
			_assetUpdateButtonContainers = null;
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
			AssetVersion version = AssetUpdater.Instance.GetLocalVersion( sender.tag);
			AssetVersion remoteVersion = AssetUpdater.Instance.GetRemoteVersion( sender.tag);

			if( remoteVersion != null)
			{
				Debug.Log( (version.Version < remoteVersion.Version) ? "Local version out of date" : "Latest version installed");
 				Application.OpenURL( Uri.EscapeUriString( remoteVersion.packageURI.ToString()));
			}
		}

		private void ReleaseNotesButtonPressed( GUIBase sender)
		{
			AssetVersion version = AssetUpdater.Instance.GetLocalVersion( sender.tag);
			AssetVersion remoteVersion = AssetUpdater.Instance.GetRemoteVersion( sender.tag);

			if( remoteVersion != null) 
			{
				Debug.Log( (version.Version < remoteVersion.Version) ? "Local version out of date" : "Latest version installed");

				string title = remoteVersion.Name + " (" + remoteVersion.Version + ") Release Notes";
				EditorUtility.DisplayDialog( title, remoteVersion.Notes, "OK");
			}
		}

		private void CreateGUI()
		{
			AssetUpdater updater = AssetUpdater.Instance;

			_gui = new GUIVertical();
			GUIScrollView scrollView = _gui.Add( new GUIScrollView()) as GUIScrollView;
			scrollView.Add( new GUILabel( new GUIContent( "Installed Assets")));

			GUIStyle style = CreateBackgroundStyle( 55, 70);
			_assetUpdateLabels = new List<GUILabel>();
			_assetUpdateButtonContainers = new List<GUIHorizontal>();

			int count = updater.AssetCount;
			for( int i=0; i<count; i++)
			{
				AssetVersion localVersion = updater.GetLocalVersion( i);
				AssetVersion remoteVersion = updater.GetRemoteVersion( i);

				GUIHorizontal bar = scrollView.Add( new GUIHorizontal( style)) as GUIHorizontal;
				GUIVertical infoContainer = bar.Add( new GUIVertical()) as GUIVertical;
				infoContainer.Add( new GUILabel( new GUIContent( localVersion.Name + " (" + localVersion.Version + ")")));
				infoContainer.Add( new GUILabel( new GUIContent( localVersion.Author)));

				string labelText = UpdateTextForVersion( localVersion, remoteVersion);

				GUIVertical updateContainer = bar.Add( new GUIVertical()) as GUIVertical;
				GUILabel label = updateContainer.Add( new GUILabel( new GUIContent( labelText))) as GUILabel;
				GUIHorizontal buttonsContainer = updateContainer.Add( new GUIHorizontal()) as GUIHorizontal;
				buttonsContainer.Add( new GUIButton( new GUIContent( "Release Notes"), ReleaseNotesButtonPressed));
				buttonsContainer.Add( new GUIButton( new GUIContent( "Download"), DownloadButtonPressed));
				buttonsContainer.isHidden = remoteVersion == null || 
											(localVersion.Version < remoteVersion.Version) == false;

				_assetUpdateLabels.Add( label);
				_assetUpdateButtonContainers.Add( buttonsContainer);
			}
		}

		private string UpdateTextForVersion( AssetVersion local, AssetVersion remote)
		{
			string text = "Checking for Updates...";
			if( remote != null) {
				if( remote.Version > local.Version) {
					text = "Update Available: " + remote.Version;
				}
				else {
					text = "Installed Version is Latest";
				}	
			}

			return text;
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
	}
}