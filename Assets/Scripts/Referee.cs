using UnityEngine;
using System.Collections.Generic;

public class Referee : MonoBehaviour {

	[SerializeField] public GameObject m_center;
	[SerializeField] public GameObject m_RefereeTopBase;
	[SerializeField] public GameObject m_leftGoalLine;
	[SerializeField] public GameObject m_rightGoalLine;
	[SerializeField] public GameObject m_RefereeBottomBase;
	[SerializeField] public BoxCollider2D m_RefereeGameBounds;

	[SerializeField] private List<SpriteRenderer> _renderer;

	public enum RefereeMode {
		Top, 
		Bottom
	}
	public RefereeMode _self_mode;
	private Vector3 _center,_basepos,_left_goal_line,_right_goal_line;

	public void Update() {
		for (int i = 0; i < _renderer.Count; i++) {
			_renderer[i].sortingOrder = (int)(-transform.position.y * 100)+i;	
		}
	}

	public void sim_initialize(RefereeMode mode) {
		_self_mode = mode;
		if (_self_mode == RefereeMode.Bottom) {
			_center = m_center.transform.position;
			_left_goal_line = m_leftGoalLine.transform.position;
			_right_goal_line = m_rightGoalLine.transform.position;
			_basepos = m_RefereeBottomBase.transform.position;
		} else {
			_center = m_center.transform.position;
			_left_goal_line = m_leftGoalLine.transform.position;
			_right_goal_line = m_rightGoalLine.transform.position;
			_basepos = m_RefereeTopBase.transform.position;
		}
		this.transform.position = _basepos;
		_movespeed = 8.0f;
	}

	[SerializeField] public float _movespeed ;

	private float _notice_particle_delay_ct;

	public void sim_update() {
		bool do_possibly_spawn_notice_particle = false;

		Vector3 current_ball_pos = Main.LevelController.currentBallPosition();
		Vector3 target_pos = transform.position;
		if (current_ball_pos.magnitude != 0) {
			if (!m_RefereeGameBounds.OverlapPoint(current_ball_pos)) {
				if (_self_mode == RefereeMode.Bottom && current_ball_pos.y < _center.y) {
					target_pos = current_ball_pos;
					do_possibly_spawn_notice_particle = true;
				} else if (_self_mode == RefereeMode.Top && current_ball_pos.y > _center.y) {
					target_pos = current_ball_pos;
					do_possibly_spawn_notice_particle = true;
				} else {
					target_pos = new Vector3(current_ball_pos.x,_basepos.y,_basepos.z);
					_notice_particle_delay_ct = 0;
				}
			} else {
				target_pos = new Vector3(current_ball_pos.x,_basepos.y + (_self_mode == RefereeMode.Top?40:-40),_basepos.z);
				_notice_particle_delay_ct = 0;
			}
		} else {
			_notice_particle_delay_ct = 0;
		}

		if (do_possibly_spawn_notice_particle) {
			_notice_particle_delay_ct -= 1;
			if (_notice_particle_delay_ct <= 0) {
				Main.LevelController.m_commentaryManager.notify_event(CommentaryEvent.OutOfBounds);
				Main.LevelController.ref_notice_particle_at(transform.position + new Vector3(0,120,0));
				_notice_particle_delay_ct = 150;
			}
		}

		float speed = _movespeed * Util.dt_scale;
		Vector3 delta =  Util.vec_sub(target_pos,transform.position);
		if (delta.magnitude < speed) {
			transform.position = target_pos;
		} else {
			Vector3 dir = delta.normalized;
			float mag = delta.magnitude;
			dir.Scale(Util.valv(speed));
			transform.position = Util.vec_add(transform.position,dir);
		}

		for (int i = 0; i < Main.LevelController.m_looseBalls.Count; i++ ) {
			LooseBall itr = Main.LevelController.m_looseBalls[i];
			if (Vector3.Distance(itr.transform.position,this.transform.position) < 40 && itr._vel.magnitude < 0.1f) {
				itr._vz = 3;
				itr._z = 0;
				Main.AudioController.PlayEffect("sfx_throw");
				if (_self_mode == RefereeMode.Top) {
					if (itr.transform.position.x < _left_goal_line.x) {
						itr._vel = Util.vec_scale((new Vector2(1,Util.rand_range(-0.15f,0))).normalized,4);

					} else if (itr.transform.position.x > _right_goal_line.x) {
						itr._vel = Util.vec_scale((new Vector2(-1,Util.rand_range(-0.15f,0))).normalized,4);

					} else {
						itr._vel = Util.vec_scale((new Vector2(Util.rand_range(-1,1),-1)).normalized,4);
					}


				} else {
					if (itr.transform.position.x < _left_goal_line.x) {
						itr._vel = Util.vec_scale((new Vector2(1,Util.rand_range(0,0.15f))).normalized,4);

					} else if (itr.transform.position.x > _right_goal_line.x) {
						itr._vel = Util.vec_scale((new Vector2(-1,Util.rand_range(0,0.15f))).normalized,4);

					} else {
						itr._vel = Util.vec_scale((new Vector2(Util.rand_range(-1,1),1)).normalized,4);
					}

				}

			}
		}

	}
}
