using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class SpriteAnimator : MonoBehaviour {

	private class SpriteAnimator_Animation {
		public List<Sprite> _frames;
		public float _speed;
	}

	[SerializeField] public SpriteRenderer _tar;
	private Dictionary<string,SpriteAnimator_Animation> _anim_name_to_anim = new Dictionary<string,SpriteAnimator_Animation>();
	private float _ct;
	private int _i;
	private bool _repeating = true;
	[SerializeField] private string _current_anim_name = "";

	public void add_anim(string name, List<Sprite> frames, float speed) {
		_anim_name_to_anim[name] = new SpriteAnimator_Animation() {
			_frames = frames,
			_speed = speed
		};
	}

	public void play_anim(string name) {
		if (_current_anim_name != name) {
			_current_anim_name = name;
			_ct = 0;
			_i = 0;
		}
	}
	
	public void set_repeating(bool val) {
		_repeating = val;
	}

	void Update () {
		if (Main.IsPaused(PauseFlags.TimeOut)) return;
		if (!this._anim_name_to_anim.ContainsKey(this._current_anim_name)) return;
		SpriteAnimator_Animation animation = this._anim_name_to_anim[this._current_anim_name];
		if (animation._frames.Count == 0) return;
		if (animation._speed <= 0) return;

		if (_i >= animation._frames.Count) _i = 0;
		_tar.sprite = animation._frames[_i];
		_ct -= Util.dt_scale;
		while (_ct <= 0) {
			_ct += animation._speed;
			if (_i+1 >= animation._frames.Count) {
				if (_repeating) {
					_i = 0;
				} else {
					animation._speed = 0;
				}	
			} else {
				_i++;
			}
		}
	}
}