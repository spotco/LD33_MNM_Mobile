using UnityEngine;
using System.Collections;

namespace Uzu
{
	/// <summary>
	/// Audio loader.
	/// Holds all the audio data to be used by the system.
	/// Multiple loaders can be used in a single scene, and switched in and out as needed.
	/// </summary>
	public class AudioLoader : MonoBehaviour
	{
		public AudioClip[] AudioClips = new AudioClip[0];
		public string[] AudioClipIds = new string[0];
		
		public AudioClip GetClip (string id)
		{
			// TODO: construct LUT in awake and use for optimization
			for (int i = 0; i < AudioClipIds.Length; i++) {
				if (AudioClipIds [i] == id) {
					return AudioClips [i];
				}
			}
			
			return null;
		}
		
		protected void Awake ()
		{
			// No need to update.
			this.enabled = false;
		}
	}
}