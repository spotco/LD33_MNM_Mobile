using UnityEngine;
using System.Collections;

public class BobDirection : MonoBehaviour {

	[SerializeField] public float _vt_min, _vt_max;
	[SerializeField] public bool _has_gen_x;
	[SerializeField] public float _gen_x_min_min, _gen_x_min_max, _gen_x_max_min, _gen_x_max_max;

	[SerializeField] private float _x_min, _x_max;
	[SerializeField] public bool _has_start_t;
	[SerializeField] public float _start_t;
	[SerializeField] public Transform _target;

	private float _vt;
	private float _x;
	private float _t;
	private Vector3 _start_target_pos;
	
	[SerializeField] private bool _local;

	void Start () {
		if (_has_start_t) {
			_t = _start_t;
		} else {
			_t = Util.rand_range(-3.14f,3.14f);
		}
		if (_has_gen_x) {
			_x_min = Util.rand_range(_gen_x_min_min,_gen_x_min_max);
			_x_max = Util.rand_range(_gen_x_max_min,_gen_x_max_max);
		}

		_vt = Util.rand_range(_vt_min,_vt_max);
		if (_target == null) {
			Debug.Log(this.gameObject.name);
			return;
		}
		if (!_local) {
			_start_target_pos = _target.position;
		} else {
			_start_target_pos = _target.localPosition;
		}
		
		_enabled = true;
	}
	

	void Update () {
		if (Main.IsPaused(PauseFlags.TimeOut) && !_is_talking_head) return;
		if (Main.IsPaused(PauseFlags.TalkingHeadStop) && _local && _is_talking_head) return;
		_t += _vt * Util.dt_scale;
		if (_enabled) {
			if (!_local) {
				_target.transform.position = _start_target_pos + new Vector3(0,(Mathf.Sin(_t)+1)*0.5f*(_x_max-_x_min) + _x_min,0);
			} else {
				_target.transform.localPosition = _start_target_pos + new Vector3(0,(Mathf.Sin(_t)+1)*0.5f*(_x_max-_x_min) + _x_min,0);
			}
		} else {
			if (!_local) {
				if (_target == null) {
					Debug.Log(this.gameObject.name);
					return;
				}
				_target.transform.position = _start_target_pos;
			} else {
				_target.transform.localPosition = _start_target_pos;
			}
		}
	}

	[SerializeField] private bool _is_talking_head;
	[SerializeField] private bool _enabled;
	public void set_enabled(bool tar) {
		_enabled = tar;
	}
}
