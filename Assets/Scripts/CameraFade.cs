using UnityEngine;
using System.Collections;

public class CameraFade : MonoBehaviour {

	private float _target_alpha = 0;

	public void set_alpha(float val) {
		Color c = this.GetComponent<SpriteRenderer>().color;
		c.a = val;
		this.GetComponent<SpriteRenderer>().color = c;
	}

	public void set_target_alpha(float tar) {
		_target_alpha = tar;
	}

	public bool is_transition_finished() {
		return Mathf.Abs(this.GetComponent<SpriteRenderer>().color.a - _target_alpha) < 0.05f;
	}

	void Update () {
		set_alpha(Util.drpt(this.GetComponent<SpriteRenderer>().color.a,_target_alpha,1/30.0f));
	}

	public float get_alpha() {
		return this.GetComponent<SpriteRenderer>().color.a;
	}
}
