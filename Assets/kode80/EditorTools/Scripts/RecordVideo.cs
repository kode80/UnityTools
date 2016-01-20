using UnityEngine;
using System.Collections;

namespace kode80.EditorTools
{
	// The RecordVideo component is used by RecordVideoWindow
	// to record frames during play mode and should't be added
	// manually by the user, so hide it from the component menu.
	[AddComponentMenu("")]
	public class RecordVideo : MonoBehaviour 
	{

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}
}
