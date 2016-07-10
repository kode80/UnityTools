using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace kode80.EditorTools
{
	public struct DiffRecord
	{
		public GameObject gameObjectA;
		public GameObject gameObjectB;
		public string propertyName;
		public string gameObjectAPath;
		public string gameObjectBPath;
		public SerializedPropertyType propertyType;
	}

	public class Diff
	{
		private Vector2 scrollPosition;

		public List<DiffRecord> Compare( GameObject gameObjectA, GameObject gameObjectB)
		{
			var diffs = new List<DiffRecord>();
			Compare( gameObjectA, gameObjectB, "", "", diffs);
			return diffs;
		}

		private void Compare( GameObject gameObjectA, GameObject gameObjectB, 
							  string gameObjectAPath, string gameObjectBPath, 
							  List<DiffRecord> diffs)
		{
			if( gameObjectAPath == "") { gameObjectAPath = gameObjectA.name; }
			else { gameObjectAPath += "." + gameObjectA.name; }

			if( gameObjectBPath == "") { gameObjectBPath = gameObjectB.name; }
			else { gameObjectBPath += "." + gameObjectB.name; }

			var componentsA = gameObjectA.GetComponents<Component>();
			var componentsB = gameObjectB.GetComponents<Component>();

			if( componentsA.Length != componentsB.Length)
			{
				return;
			}

			int count = componentsA.Length;

			for( int i=0; i<count; i++)
			{
				var serializedA = new SerializedObject( componentsA[i]);
				var serializedB = new SerializedObject( componentsB[i]);
				var iterA = serializedA.GetIterator();
				var iterB = serializedB.GetIterator();
				SerializedProperty p;
				while( iterA.NextVisible( true) && iterB.NextVisible( true))
				{
					if( iterA.name == iterB.name &&
						iterA.propertyType == iterB.propertyType)
					{
						bool isDiff = CompareValues( iterA, iterB) == false;

						if( isDiff && IsPropertyParentType( iterA) == false)
						{
							var record = new DiffRecord();
							record.gameObjectA = gameObjectA;
							record.gameObjectB = gameObjectB;
							record.propertyName = iterA.propertyPath;
							record.propertyType = iterA.propertyType;
							record.gameObjectAPath = gameObjectAPath;
							record.gameObjectBPath = gameObjectBPath;
							diffs.Add( record);
						}
					}
					else
					{
						break;
					}
				}
			}

			var childCountA = gameObjectA.transform.childCount;
			var childCountB = gameObjectA.transform.childCount;

			if( childCountA != childCountB) {
				return;
			}

			for( int j=0; j<childCountA; j++) 
			{
				Compare( gameObjectA.transform.GetChild( j).gameObject,
						 gameObjectB.transform.GetChild( j).gameObject,
						 gameObjectAPath,
						 gameObjectBPath,
						 diffs);
			}
		}

		private bool CompareValues( SerializedProperty propertyA, SerializedProperty propertyB)
		{
			if( propertyA.propertyType != propertyB.propertyType) {
				return false;
			}

			switch( propertyA.propertyType)
			{
			case SerializedPropertyType.Integer:
				return propertyA.intValue == propertyB.intValue;

			case SerializedPropertyType.Boolean:
				return propertyA.boolValue == propertyB.boolValue;

			case SerializedPropertyType.Float:
				return propertyA.floatValue == propertyB.floatValue;
				
			case SerializedPropertyType.String:
				return propertyA.stringValue == propertyB.stringValue;
				
			case SerializedPropertyType.Color:
				return propertyA.colorValue == propertyB.colorValue;
				
			case SerializedPropertyType.ObjectReference:
				return propertyA.objectReferenceValue == propertyB.objectReferenceValue;
				
			case SerializedPropertyType.LayerMask:
				return propertyA.intValue == propertyB.intValue;
				
			case SerializedPropertyType.Enum:
				return propertyA.enumValueIndex == propertyB.enumValueIndex;
				
			case SerializedPropertyType.Vector2:
				return propertyA.vector2Value == propertyB.vector2Value;
				
			case SerializedPropertyType.Vector3:
				return propertyA.vector3Value == propertyB.vector3Value;
				
			case SerializedPropertyType.Vector4:
				return propertyA.vector4Value == propertyB.vector4Value;
				
			case SerializedPropertyType.Rect:
				return propertyA.rectValue == propertyB.rectValue;
				
			case SerializedPropertyType.ArraySize:
				return propertyA.arraySize == propertyB.arraySize;
				
			case SerializedPropertyType.Character:
				return propertyA.intValue == propertyB.intValue;
				
			case SerializedPropertyType.AnimationCurve:
				return propertyA.animationCurveValue == propertyB.animationCurveValue;
				
			case SerializedPropertyType.Bounds:
				return propertyA.boundsValue == propertyB.boundsValue;
				
			case SerializedPropertyType.Gradient:
				return propertyA.objectReferenceValue == propertyB.objectReferenceValue;
				
			case SerializedPropertyType.Quaternion:
				return propertyA.quaternionValue == propertyB.quaternionValue;
			}

			return false;
		}

		private bool IsPropertyParentType( SerializedProperty property)
		{
			switch( property.propertyType)
			{
			case SerializedPropertyType.Vector2:
			case SerializedPropertyType.Vector3:
			case SerializedPropertyType.Vector4:
			case SerializedPropertyType.Rect:
			case SerializedPropertyType.Bounds:
			case SerializedPropertyType.Quaternion:
				return true;
			}

			return false;
		}
	}
}
