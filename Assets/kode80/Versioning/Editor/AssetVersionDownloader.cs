using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace kode80.Versioning
{
	public class AssetVersionDownloader 
	{
		public delegate void RemoteVersionDownloadFinished( AssetVersion local, AssetVersion remote);
		public RemoteVersionDownloadFinished remoteVersionDownloadFinished;

		public delegate void RemoteVersionDownloadFailed( AssetVersion local);
		public RemoteVersionDownloadFailed remoteVersionDownloadFailed;

		private WebClient _webClient;
		private List<AssetVersion> _queue;
		private AssetVersion _currentLocalVersion;

		public AssetVersionDownloader()
		{
			_queue = new List<AssetVersion>();
		}

		public void Add( AssetVersion local)
		{
			_queue.Add( local);
			AttemptNextDownload();
		}

		public void CancelAll()
		{
			if( _webClient != null) {
				_webClient.CancelAsync();
			}

			_queue.Clear();
		}

		private void AttemptNextDownload()
		{
			if( _webClient == null && _queue.Count > 0)
			{
				_currentLocalVersion = _queue[0];
				_queue.RemoveAt( 0);

				using( _webClient = new WebClient())
				{
					_webClient.DownloadStringCompleted += WebClientCompleted;

					try {
						_webClient.DownloadStringAsync( _currentLocalVersion.versionURI);
					}
					catch( Exception e) {
						HandleFailedDownload();
					}
				}
			}
		}

		private void WebClientCompleted(object sender, DownloadStringCompletedEventArgs e)
		{
			if( e.Cancelled || e.Error != null) 
			{
				HandleFailedDownload();
			}
			else 
			{
				AssetVersion remote = AssetVersion.ParseXML( e.Result);
				if( remote == null) 
				{
					HandleFailedDownload();
				}
				else 
				{
					HandleFinishedDownload( remote);
				}
			}
		}

		private void HandleFinishedDownload( AssetVersion remote)
		{
			if( remoteVersionDownloadFinished != null) {
				remoteVersionDownloadFinished( _currentLocalVersion, remote);
			}

			_currentLocalVersion = null;
			_webClient = null;
			AttemptNextDownload();
		}

		private void HandleFailedDownload()
		{
			if( remoteVersionDownloadFailed != null) {
				remoteVersionDownloadFailed( _currentLocalVersion);
			}

			_currentLocalVersion = null;
			_webClient = null;
			AttemptNextDownload();
		}
	}
}
