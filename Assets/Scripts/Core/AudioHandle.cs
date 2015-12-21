using UnityEngine;
using System.Collections;

namespace Uzu
{
	public struct AudioHandle
	{
		#region Implementation.
		private int _id;
		private int _index;

		public const int INVALID_ID = 0;

		public AudioHandle (int id, int index)
		{
			_id = id;
			_index = index;
			_handle_audio_source = null;
		}

		/// <summary>
		/// Unique identifier for this handle.
		/// </summary>
		public int Id {
			get { return _id; }
		}

		/// <summary>
		/// Index into the resource list used by this handle.
		/// </summary>
		public int Index {
			get { return _index; }
		}
		#endregion
		
		public AudioSource _handle_audio_source;
		
	}
}