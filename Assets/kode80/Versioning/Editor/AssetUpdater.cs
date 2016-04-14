using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace kode80.Versioning
{
	public class AssetUpdater
	{
		//public delegate void LocalVersionsChanged( AssetUpdater updater);
		//public LocalVersionsChanged localVersionsChanged;

		private static AssetUpdater _instance;
		public static AssetUpdater Instance {
			get {
				if( _instance == null) {
					_instance = new AssetUpdater();
				}
				return _instance;
			}
		}

		private List<AssetVersion> _localVersions;
		private Dictionary<AssetVersion,AssetVersion> _localToRemoteVersions;

		public int AssetCount { get { return _localVersions.Count; } }

		private AssetUpdater()
		{
			_localVersions = new List<AssetVersion>();
			_localToRemoteVersions = new Dictionary<AssetVersion, AssetVersion>();
		}

		public void Refresh()
		{
			List<AssetVersion> localVersions = FindLocalVersions();
			if( VersionListsAreEqual( localVersions, _localVersions) == false)
			{
				_localVersions = localVersions;
				// ...issue update event
			}
		}

		public AssetVersion GetLocalVersion( int index) {
			return _localVersions[ index];
		}

		public AssetVersion GetRemoteVersion( int index) {
			AssetVersion localVersion = GetLocalVersion( index);

			if( _localToRemoteVersions.ContainsKey( localVersion)) {
				return _localToRemoteVersions[ localVersion];
			}

			return null;
		}

		private List<AssetVersion> FindLocalVersions()
		{
			List<AssetVersion> versions = new List<AssetVersion>();
			string[] paths = Directory.GetFiles( Application.dataPath, "AssetVersion.xml", SearchOption.AllDirectories);

			foreach( string path in paths)
			{
				string localXML = File.ReadAllText( path);
				AssetVersion version = AssetVersion.ParseXML( localXML);

				if( version != null) {
					versions.Add( version);
				}
			}

			return versions;
		}

		private bool VersionListsAreEqual( List<AssetVersion> a, List<AssetVersion> b)
		{
			if( a == b) { return true; }
			if( a.Count != b.Count) { return false; }

			Dictionary<string,bool> hash = new Dictionary<string, bool>();

			foreach( AssetVersion version in a) { 
				hash[ version.ToString()] = true; 
			}

			foreach( AssetVersion version in b) { 
				if( hash.ContainsKey( version.ToString()) == false) { 
					return false; 
				} 
			}

			return true;
		}
	}
}
