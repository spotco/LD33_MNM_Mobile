using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Uzu
{
	/// <summary>
	/// Custom editor for the loading of audio data.
	/// </summary>
	[CustomEditor(typeof(AudioLoader))]
	public class AudioLoaderEditor : Editor
	{
		public override void OnInspectorGUI ()
		{
			GUI.SetNextControlName ("DragDropBox");
			EditorGUILayout.HelpBox ("Drag AudioClips here.", MessageType.None);
			
			AudioLoader loader = (AudioLoader)this.target;
			
			// TODO: sort alphabetically by clip id
			{
				for (int i = 0; i < loader.AudioClips.Length; i++) {
					GUILayout.BeginHorizontal ();
					
					loader.AudioClipIds [i] = EditorGUILayout.TextField (string.Empty, loader.AudioClipIds [i], GUILayout.Width (100));
					loader.AudioClips [i] = (AudioClip)EditorGUILayout.ObjectField (loader.AudioClips [i], typeof(AudioClip), true);
					
					if (GUILayout.Button ("X", EditorStyles.miniButton, GUILayout.Width (18))) {
						ArrayUtility.RemoveAt<string> (ref loader.AudioClipIds, i);
						ArrayUtility.RemoveAt<AudioClip> (ref loader.AudioClips, i);
					}
					
					GUILayout.EndHorizontal ();
				}
			}
			
			if (Event.current.type == EventType.DragPerform || Event.current.type == EventType.DragUpdated) {
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				
				if (Event.current.type == EventType.DragPerform) {
					foreach (Object obj in DragAndDrop.objectReferences) {
						if (obj is AudioClip) {
							ArrayUtility.Add<string> (ref loader.AudioClipIds, "<id>");
							ArrayUtility.Add<AudioClip> (ref loader.AudioClips, (AudioClip)obj);
						}
					}
				}
			}
		}
	}
}