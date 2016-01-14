//  Copyright (c) 2016, Ben Hopkins (kode80)
//  All rights reserved.
//  
//  Redistribution and use in source and binary forms, with or without modification, 
//  are permitted provided that the following conditions are met:
//  
//  1. Redistributions of source code must retain the above copyright notice, 
//     this list of conditions and the following disclaimer.
//  
//  2. Redistributions in binary form must reproduce the above copyright notice, 
//     this list of conditions and the following disclaimer in the documentation 
//     and/or other materials provided with the distribution.
//  
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY 
//  EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
//  MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL 
//  THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
//  SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT 
//  OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
//  HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, 
//  EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using kode80.GUIWrapper;

namespace kode80.EditorTools
{
	public class ComponentList : EditorWindow 
	{
		private GUIVertical _gui;

		[MenuItem( "Window/kode80/Editor Tools/Component List")]
		public static void Init()
		{
			ComponentList win = EditorWindow.GetWindow( typeof( ComponentList)) as ComponentList;
			win.titleContent = new GUIContent( "Component List");
			win.Show();
		}

		public void RefreshList( GameObject gameObject)
		{
			_gui = new GUIVertical();

			if( gameObject == null)
			{
			}
			else
			{
				Component[] components = gameObject.GetComponents<Component>();
				int index = 0;
				foreach( Component component in components)
				{
					GUIDelayedIntField field = new GUIDelayedIntField( new GUIContent( component.GetType().Name), 
																	   index++, 
																	   ComponentIndexChanged);
					_gui.Add( field);
				}
				Repaint();
			}
		}

		void ComponentIndexChanged( GUIBase sender)
		{
			GUIDelayedIntField field = sender as GUIDelayedIntField;
		}

		void OnEnable()
		{
			GameObject gameObject = Selection.activeTransform ? Selection.activeTransform.gameObject : null;
			RefreshList( gameObject);
		}

		void OnDisable()
		{
		}

		void OnGUI()
		{
			if( _gui != null)
			{
				_gui.OnGUI();
			}
		}
	}
}
