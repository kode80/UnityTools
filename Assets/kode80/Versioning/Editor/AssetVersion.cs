using UnityEngine;
using System;
using System.Collections;
using System.Xml;

namespace kode80.Versioning
{
	public class AssetVersion
	{
		public string Name { get; private set; }
		public string Author { get; private set; }
		public SemanticVersion Version { get; private set; }
		public string Notes { get; private set; }
		public Uri Uri { get; private set; }

		public static AssetVersion ParseXML( string xmlString)
		{
			XmlDocument xml = new XmlDocument();

			try { xml.LoadXml( xmlString); }
			catch( XmlException e) { return null; }

			XmlNode name = xml.SelectSingleNode( "asset/name");
			XmlNode author = xml.SelectSingleNode( "asset/author");
			XmlNode version = xml.SelectSingleNode( "asset/version");
			XmlNode notes = xml.SelectSingleNode( "asset/notes");
			XmlNode uri = xml.SelectSingleNode( "asset/uri");

			if( name == null || author == null || version == null || notes == null || uri == null) {
				Debug.Log( "Error parsing Asset Version XML");
				return null;
			}

			SemanticVersion semanticVersion = SemanticVersion.Parse( version.InnerText);
			if( semanticVersion == null) {
				Debug.Log( "Error parsing Semantic Version");
				return null;
			}

			AssetVersion assetVersion = new AssetVersion();
			assetVersion.Name = name.InnerText;
			assetVersion.Author = author.InnerText;
			assetVersion.Version = semanticVersion;
			assetVersion.Notes = notes.InnerText;
			assetVersion.Uri = new Uri( uri.InnerText);

			return assetVersion;
		}

		public override string ToString()
		{
			return "Name: " + Name + "\n" +
				"Author: " + Author + "\n" +
				"Version: " + Version + "\n" +
				"Notes: " + Notes + "\n" +
				"URI: " + Uri;
		}
	}
}
