using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteAnimator))]
public class AnimatedGoalPost : MonoBehaviour {

	[SerializeField] private Sprite _sprite_mouth_open;
	[SerializeField] private Sprite _sprite_mouth_closed;

	[SerializeField] private GameObject _tongue;
	[SerializeField] private SpriteRenderer _top_renderer;

	private Vector3 _tongue_original_position;
	private float _tongue_theta, _tongue_theta_2;

	[System.NonSerialized] public SpriteAnimator _animator;

	private Vector3 _self_original_position;

	[SerializeField] private List<GameObject> _confetti_spawn_point;

	void Start () {
		_self_original_position = this.transform.position;
		_tongue_original_position = _tongue.transform.localPosition;
		_animator = this.GetComponent<SpriteAnimator>();

		_animator.add_anim(
			"idle",
			new List<Sprite> { _sprite_mouth_open },
			1
		);
		_animator.add_anim(
			"eat",
			new List<Sprite> { _sprite_mouth_open, _sprite_mouth_closed },
			20
		);
		_animator.play_anim("idle");

	}
	void Update () {
		if (Main.IsPaused(PauseFlags.TimeOut)) return;
		_tongue_theta += Util.dt_scale * 0.05f;
		_tongue_theta_2 += Util.dt_scale * 0.02f;
		_tongue.transform.localPosition = _tongue_original_position + new Vector3(0.25f * Mathf.Sin(_tongue_theta),0.1f * Mathf.Sin(_tongue_theta_2),0);

		_eat_anim_ct -= Util.dt_scale;
		if (_eat_anim_ct > 0) {
			_animator.play_anim("eat");
		} else {
			_animator.play_anim("idle");
		}
	}
	public BoxCollider2D box_collider() { return this.GetComponent<BoxCollider2D>(); }

	private float _eat_anim_ct;
	public void play_eat_anim(float ct) {
		_eat_anim_ct = ct;
	}

	public void spawn_confetti() {
		if (_confetti_spawn_point == null) return;
		for (int i = 0; i < _confetti_spawn_point.Count; i++) {
			Main.LevelController.confetti_particle_at(_confetti_spawn_point[i].transform.position);
		}
	}
}
