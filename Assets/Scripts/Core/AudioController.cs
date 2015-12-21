using UnityEngine;
using System.Collections;

namespace Uzu
{
	/// <summary>
	/// Configuration for an audio controller.
	/// </summary>
	public struct AudioControllerConfig
	{
		/// <summary>
		/// The max # of audio sources that this controller can handle.
		/// </summary>
		public int AudioSourceMaxCount { get; set; }

		/// <summary>
		/// Reference to the loader used to load this controller's audio clips.
		/// </summary>
		public AudioLoader AudioLoader { get; set; }
	}

	/// <summary>
	/// Handles playing of audio.
	/// </summary>
	public class AudioController : MonoBehaviour
	{
		/// <summary>
		/// Options passed when playing a sound.
		/// </summary>
		public struct PlayOptions
		{
			public bool Loop;
			public Vector3 Position;
			public float Volume;
			public float FadeInTime;
		}

		/// <summary>
		/// Initializes the audio controller.
		/// </summary>
		public void Initialize (AudioControllerConfig config)
		{
			// AudioSource allocation.
			{
				int maxCount = Mathf.Max (1, config.AudioSourceMaxCount);
				_availableSources = new FixedList<AudioSource> (maxCount);
				_availableSourceInfoIndices = new FixedList<int> (maxCount);
				_activeSourceInfoIndices = new FixedList<int> (maxCount);
				_sourceInfos = new FixedList<AudioSourceInfo> (maxCount);
				for (int i = 0; i < maxCount; i++) {
					GameObject go = new GameObject ("AudioSource_" + i);
					Transform xform = go.transform;
					xform.parent = this.transform;
					AudioSource audioSource = go.AddComponent<AudioSource> ();
					ReturnSourceToPool (audioSource);

					_availableSourceInfoIndices.Add (i);
					_sourceInfos.Add (new AudioSourceInfo ());
				}
			}

			_audioLoader = config.AudioLoader;

			if (_audioLoader == null) {
				Debug.LogError ("AudioLoader not set!");
			}
		}

		/// <summary>
		/// Gets the # of available audio sources remaining.
		/// </summary>
		public int GetAvailableSourceCount ()
		{
			return _availableSources.Count;
		}

		public bool IsHandleValid (AudioHandle handle)
		{
			return GetSourceInfo (handle) != null;
		}

		/// <summary>
		/// Mute.
		/// </summary>
		public bool IsMuted {
			get { return _isMuted; }
			set { _isMuted = value; }
		}

		public AudioHandle Play (string clipId, PlayOptions options)
		{
			AudioClip clip = GetClip (clipId);
			if (clip != null) {
				AudioHandle handle = GetSource ();
				AudioSourceInfo sourceInfo = GetSourceInfo (handle);
				if (sourceInfo != null) {
					AudioSource source = sourceInfo.Source;
					source.clip = clip;
					source.loop = options.Loop;
					source.transform.localPosition = options.Position;

					sourceInfo.ClipId = clipId;
					sourceInfo.TargetVolume = options.Volume;

					if (options.FadeInTime > 0.0f &&
					    !_isMuted) {
						sourceInfo.VolumeFluctuationSpeed = sourceInfo.TargetVolume / options.FadeInTime;
						source.volume = 0.0f;
					}
					else {
						SetSourceVolume (sourceInfo, sourceInfo.TargetVolume);
					}

					source.Play ();
					
					handle._handle_audio_source = source;
				}
			
				return handle;
			}

			return new AudioHandle ();
		}

		public bool IsPlaying (AudioHandle handle)
		{
			AudioSourceInfo sourceInfo = GetSourceInfo (handle);
			if (sourceInfo != null) {
				return sourceInfo.Source.isPlaying;
			}

			return false;
		}

		public void Stop (AudioHandle handle)
		{
			Stop (handle, 0.0f);
		}

		public void Stop (AudioHandle handle, float fadeOutTime)
		{
			AudioSourceInfo sourceInfo = GetSourceInfo (handle);
			if (sourceInfo != null) {
				if (fadeOutTime > 0.0f &&
				    !_isMuted) {
					sourceInfo.VolumeFluctuationSpeed = -sourceInfo.Source.volume / fadeOutTime;
					sourceInfo.TargetVolume = 0.0f;
				}
				else {
					sourceInfo.Source.Stop ();
				}
			}
		}

		public void StopAll ()
		{
			for (int i = 0; i < _activeSourceInfoIndices.Count; i++) {
				AudioSourceInfo sourceInfo = _sourceInfos [_activeSourceInfoIndices [i]];
				sourceInfo.Source.Stop ();
			}
		}

		public string GetClipId (AudioHandle handle)
		{
			AudioSourceInfo sourceInfo = GetSourceInfo (handle);
			if (sourceInfo != null) {
				return sourceInfo.ClipId;
			}

			return string.Empty;
		}

		public void SetVolume (AudioHandle handle, float volume)
		{
			SetVolume (handle, volume, 0.0f);
		}

		public void SetVolume (AudioHandle handle, float volume, float duration)
		{
			AudioSourceInfo sourceInfo = GetSourceInfo (handle);
			if (sourceInfo != null) {
				if (duration > 0.0f &&
				    !_isMuted) {
					float deltaVolume = volume - sourceInfo.Source.volume;
					sourceInfo.VolumeFluctuationSpeed = deltaVolume / duration;
					sourceInfo.TargetVolume = volume;
				}
				else {
					SetSourceVolume (sourceInfo, volume);
				}
			}
		}

		public float GetVolume (AudioHandle handle)
		{
			AudioSourceInfo sourceInfo = GetSourceInfo (handle);
			if (sourceInfo != null) {
				return sourceInfo.Source.volume;
			}

			return 0.0f;
		}

		public void SetPitch (AudioHandle handle, float pitch)
		{
			AudioSourceInfo sourceInfo = GetSourceInfo (handle);
			if (sourceInfo != null) {
				sourceInfo.Source.pitch = pitch;
			}
		}

		public float GetPitch (AudioHandle handle)
		{
			AudioSourceInfo sourceInfo = GetSourceInfo (handle);
			if (sourceInfo != null) {
				return sourceInfo.Source.pitch;
			}

			return 1.0f;
		}

		#region Implementation.
		private AudioLoader _audioLoader;
		private FixedList<AudioSource> _availableSources;
		private FixedList<int> _availableSourceInfoIndices;
		private FixedList<int> _activeSourceInfoIndices;
		private FixedList<AudioSourceInfo> _sourceInfos;
		private bool _isMuted;
		private bool _wasMuted;

		private class AudioSourceInfo
		{
			/// <summary>
			/// Unique identifier used to reverse-lookup this source info
			/// to make sure it is actually valid.
			/// </summary>
			public int HandleId;

			/// <summary>
			/// The clipId used to create this source.
			/// </summary>
			public string ClipId;

			/// <summary>
			/// The AudioSource that this info is referring to.
			/// </summary>
			public AudioSource Source;

			/// <summary>
			/// The maximum volume this sound can reach.
			/// </summary>
			public float TargetVolume;

			/// <summary>
			/// Rate of change of volume during fade effects.
			/// </summary>
			public float VolumeFluctuationSpeed;

			public AudioSourceInfo ()
			{
				Reset ();
			}

			public void Reset ()
			{
				HandleId = AudioHandle.INVALID_ID;
				ClipId = string.Empty;
				Source = null;
				VolumeFluctuationSpeed = 0.0f;
				TargetVolume = 1.0f;
			}
		}

		#region Unique id generation.
		/// <summary>
		/// Handle counter.
		/// Start from 1 since 0 is an invalid id.
		/// </summary>
		private int HANDLE_ID_CNT = 1;

		private int CreateHandleId ()
		{
			// GUID could also be used, but this seems fine for now.
			return HANDLE_ID_CNT++;
		}
		#endregion

		private void SetSourceVolume (AudioSourceInfo sourceInfo, float volume)
		{
			if (_isMuted) {
				sourceInfo.Source.volume = 0.0f;
			}
			else {
				sourceInfo.Source.volume = volume;
			}

			sourceInfo.TargetVolume = volume;
		}

		private AudioClip GetClip (string clipId)
		{
			if (_audioLoader == null) {
				Debug.LogWarning ("AudioLoader not registered.");
				return null;
			}

			AudioClip clip = _audioLoader.GetClip (clipId);
			if (clip == null) {
				Debug.LogWarning ("AudioClip id [" + clipId + "] not found.");
			}

			return clip;
		}

		private AudioHandle GetSource ()
		{
			if (_availableSourceInfoIndices.Count > 0) {
				int index = _availableSourceInfoIndices.Count - 1;

				// Get a source.
				AudioSource source = GetSourceFromPool (index);

				// Get a source info index.
				int infoIndex = _availableSourceInfoIndices [index];
				_availableSourceInfoIndices.RemoveAt (index);
				_activeSourceInfoIndices.Add (infoIndex);

				// Set up the source info.
				AudioSourceInfo sourceInfo = _sourceInfos [infoIndex];
				Uzu.Dbg.Assert (sourceInfo.Source == null);
				sourceInfo.Source = source;
				Uzu.Dbg.Assert (sourceInfo.Source != null);

				int handleId = CreateHandleId ();
				sourceInfo.HandleId = handleId;

				return new AudioHandle (handleId, infoIndex);
			}

			Debug.LogWarning ("No AudioSources available.");
			return new AudioHandle ();
		}

		private AudioSourceInfo GetSourceInfo (AudioHandle handle)
		{
			if (handle.Id != AudioHandle.INVALID_ID) {
				AudioSourceInfo sourceInfo = _sourceInfos [handle.Index];

				// Verify handle integrity.
				if (sourceInfo.HandleId == handle.Id) {
					return sourceInfo;
				}
			}

			return null;
		}

		private AudioSource GetSourceFromPool (int index)
		{
			AudioSource source = _availableSources [index];
			_availableSources.RemoveAt (index);
			source.gameObject.SetActive (true);
			return source;
		}

		private void ReturnSourceToPool (AudioSource source)
		{
			source.gameObject.SetActive (false);
			_availableSources.Add (source);
		}

		private bool _isPaused = false;

		private void OnApplicationPause(bool isPaused)
		{
			// Shyam Memo (2014.09.18):
			// Fix for issue introduced in Unity 4.5.4 where one extra frame
			// is executed after applicationWillResignActive event and
			// before applicationDidEnterBackground event. Since one extra
			// frame executes, the Update method of the AudioController is cleaning
			// up all the sounds.
			//
			// To fix this, we pause the AudioController in OnApplicationPause (which
			// is called immediately after applicationWillResignActive, but
			// before the next frame is executed), and skip updating of the
			// AudioController for this extra frame. When the app resumes,
			// _isPaused becomes false, and the controller continues its updates.
			_isPaused = isPaused;
		}

		private void Update ()
		{
			// Don't update if system is paused.
			if (_isPaused) {
				return;
			}

			// Fade in / out processing.
			{
				for (int i = 0; i < _activeSourceInfoIndices.Count; i++) {
					AudioSourceInfo sourceInfo = _sourceInfos [_activeSourceInfoIndices [i]];

					// No change in volume - skip.
					if (Mathf.Approximately (sourceInfo.VolumeFluctuationSpeed, 0.0f)) {
						continue;
					}

					AudioSource source = sourceInfo.Source;

					float deltaVolume = sourceInfo.VolumeFluctuationSpeed * Time.deltaTime;
					float newVolume = source.volume + deltaVolume;

					// Increasing volume (fade in).
					if (sourceInfo.VolumeFluctuationSpeed > 0.0f) {
						if (newVolume >= sourceInfo.TargetVolume) {
							source.volume = sourceInfo.TargetVolume;
							sourceInfo.VolumeFluctuationSpeed = 0.0f;
						}
						else {
							source.volume = newVolume;
						}
					}
					// Decreasing volume (fade out).
					else {
						if (newVolume <= sourceInfo.TargetVolume) {
							source.volume = sourceInfo.TargetVolume;
							sourceInfo.VolumeFluctuationSpeed = 0.0f;

							if (Mathf.Approximately(source.volume, 0.0f)) {
								source.Stop ();
							}
						}
						else {
							source.volume = newVolume;
						}
					}
				}
			}

			// Clean up finished sounds.
			{
				for (int i = 0; i < _activeSourceInfoIndices.Count; /*i++*/) {
					int sourceInfoIndex = _activeSourceInfoIndices [i];
					AudioSourceInfo sourceInfo = _sourceInfos [sourceInfoIndex];
					AudioSource source = sourceInfo.Source;
					if (!source.isPlaying) {
						// Reset state.
						sourceInfo.Reset ();

						// Remove from active pool.
						{
							// Move last element to avoid array copy.
							int lastIndex = _activeSourceInfoIndices.Count - 1;
							_activeSourceInfoIndices [i] = _activeSourceInfoIndices [lastIndex];
							_activeSourceInfoIndices.RemoveAt (lastIndex);
						}

						// Add back to available pools.
						_availableSourceInfoIndices.Add (sourceInfoIndex);
						ReturnSourceToPool (source);

						Uzu.Dbg.Assert (_availableSourceInfoIndices.Count == _availableSources.Count);

						continue;
					}

					i++;
				}
			}

			// Mute processing.
			{
				// Was there a change?
				if (_isMuted != _wasMuted) {
					if (_isMuted) {
						// Mute all active sources.
						for (int i = 0; i < _activeSourceInfoIndices.Count; i++) {
							AudioSourceInfo sourceInfo = _sourceInfos [_activeSourceInfoIndices [i]];
							sourceInfo.Source.volume = 0.0f;
						}
					}
					else {
						// Unmute all active sources.
						for (int i = 0; i < _activeSourceInfoIndices.Count; i++) {
							AudioSourceInfo sourceInfo = _sourceInfos [_activeSourceInfoIndices [i]];
							sourceInfo.Source.volume = sourceInfo.TargetVolume;
						}
					}
				}

				_wasMuted = _isMuted;
			}
		}
		#endregion
	}
}