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
		private List<GUIButton> _downloadButtons;

		[MenuItem( "Window/kode80/Check for Asset Updates")]
		public static void Init()
		{
			AssetUpdateWindow win = EditorWindow.GetWindow( typeof( AssetUpdateWindow)) as AssetUpdateWindow;
			win.titleContent = new GUIContent( "Asset Updater");
			win.Show();
		}

		void OnEnable()
		{
			AssetUpdater.Instance.remoteVersionDownloadFinished += RemoteVersionDownloadFinished;
			AssetUpdater.Instance.remoteVersionDownloadFailed += RemoteVersionDownloadFailed;

			AssetUpdater.Instance.Refresh();
			CreateGUI();
		}

		void OnDisable()
		{
			AssetUpdater.Instance.remoteVersionDownloadFinished -= RemoteVersionDownloadFinished;
			AssetUpdater.Instance.remoteVersionDownloadFailed -= RemoteVersionDownloadFailed;

			_gui = null;
			_assetUpdateLabels = null;
			_downloadButtons = null;
		}

		void OnGUI()
		{
			if( _gui != null)
			{
				_gui.OnGUI();
			}
		}

		#region AssetUpdater delegate

		private void RemoteVersionDownloadFinished( AssetUpdater updater, int assetIndex)
		{
			AssetVersion local = AssetUpdater.Instance.GetLocalVersion( assetIndex);
			AssetVersion remote = AssetUpdater.Instance.GetRemoteVersion( assetIndex);

			_assetUpdateLabels[ assetIndex].content.text = UpdateTextForVersion( local, remote);
			_downloadButtons[ assetIndex].isHidden = (local.Version < remote.Version) == false;
			Repaint();
		}

		private void RemoteVersionDownloadFailed( AssetUpdater updater, int assetIndex)
		{
			_assetUpdateLabels[ assetIndex].content.text = "Error: couldn't download update info";
			Repaint();
		}

		#endregion

		#region GUI delegates

		private void RefreshButtonPressed( GUIBase sender)
		{
			AssetUpdater.Instance.Refresh( true);
			CreateGUI();
		}

		private void DownloadButtonPressed( GUIBase sender)
		{
			AssetVersion remoteVersion = AssetUpdater.Instance.GetRemoteVersion( sender.tag);

			if( remoteVersion != null)
			{
 				Application.OpenURL( Uri.EscapeUriString( remoteVersion.packageURI.ToString()));
			}
		}

		private void ReleaseNotesButtonPressed( GUIBase sender)
		{
			AssetVersion localVersion = AssetUpdater.Instance.GetLocalVersion( sender.tag);
			AssetVersion remoteVersion = AssetUpdater.Instance.GetRemoteVersion( sender.tag);
			AssetVersion version = remoteVersion != null && localVersion.Version < remoteVersion.Version ?
								   remoteVersion : localVersion;

			string title = version.Name + " (" + version.Version + ") Release Notes";
			EditorUtility.DisplayDialog( title, version.Notes, "OK");
		}

		#endregion

		private void CreateGUI()
		{
			AssetUpdater updater = AssetUpdater.Instance;

			_gui = new GUIVertical();
			GUIScrollView scrollView = _gui.Add( new GUIScrollView()) as GUIScrollView;

			scrollView.Add( new GUILabel( new GUIContent( "Installed Assets")));


			GUIStyle style = EditorGUIUtility.isProSkin ? CreateBackgroundStyle( 55, 70) : 
				CreateBackgroundStyle( 170, 235);
			_assetUpdateLabels = new List<GUILabel>();
			_downloadButtons = new List<GUIButton>();

			GUIStyle statusStyle = new GUIStyle();
			statusStyle.margin = new RectOffset( 2, 4, 2, 2);
			statusStyle.normal.textColor = EditorGUIUtility.isProSkin ? new Color( 0.6f, 0.6f, 0.6f) :
				new Color( 0.4f, 0.4f, 0.4f);
			statusStyle.alignment = TextAnchor.MiddleRight;

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
				label.style = statusStyle;

				GUIHorizontal buttonsContainer = updateContainer.Add( new GUIHorizontal()) as GUIHorizontal;
				GUIButton button = buttonsContainer.Add( new GUIButton( new GUIContent( "Release Notes"), 
																		ReleaseNotesButtonPressed)) as GUIButton;
				button.tag = i;

				button = buttonsContainer.Add( new GUIButton( new GUIContent( "Download"), 
															  DownloadButtonPressed)) as GUIButton;
				button.tag = i;
				button.isHidden = remoteVersion == null || 
								  (localVersion.Version < remoteVersion.Version) == false;

				_assetUpdateLabels.Add( label);
				_downloadButtons.Add( button);
			}

			GUIHorizontal refreshContainer = scrollView.Add( new GUIHorizontal()) as GUIHorizontal;
			refreshContainer.Add( new GUISpace( true));
			refreshContainer.Add( new GUIButton( new GUIContent( "Refresh"), RefreshButtonPressed));
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
			Texture2D texture = new Texture2D( 1, height, TextureFormat.RGB24, false, true);
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