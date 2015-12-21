using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(CircleCollider2D))]
public class GenericFootballer : MonoBehaviour {

	[SerializeField] public CircleCollider2D m_clickCollider;
	[SerializeField] private SpriteRenderer _renderer;
	[SerializeField] private float _waitdelay;
	public void set_wait_delay(float tar) {
		_waitdelay = tar;
		_current_mode = GenericFootballerMode.CatchWait;
	}

	private float _select_reticule_theta;
	[SerializeField] private SpriteRenderer _select_reticule;
	private float _select_reticule_target_alpha = 0.0f;
	public void set_select_reticule_alpha(float tar) {
		_select_reticule_target_alpha = tar;
	}

	public enum GenericFootballerMode {
		Idle,
		CommandMoving,
		CatchWait,
		Stunned,
		PlayerTeamHasBall,
		MoveBackIntoBounds
	}

	[SerializeField] public GenericFootballerMode _current_mode;

	private static int __alloct = 0;
	[SerializeField] private int _id = 0;

	private SpriteAnimator _animator;
	public void force_play_animation(string anim) {
		_animator.play_anim(anim);
	}
	public void force_facing_direction(bool to_right) {
		Vector3 rts = _renderer.transform.localScale;
		if (to_right) {
			rts.x = Mathf.Abs(rts.x);
		} else {
			rts.x = -Mathf.Abs(rts.x);
		}
		_renderer.transform.localScale = rts;
	}
	
	public void sim_initialize(FootballerAnimResource anims) {
		_id = ++__alloct;
		_current_mode = GenericFootballerMode.Idle;
		Color color = _select_reticule.color;
		color.a = 0;
		_select_reticule.color = color;
		this.set_select_reticule_alpha(0);
		_throw_charge_ct = 0;
		this.update_calculated_velocity();

		_animator = this.gameObject.AddComponent<SpriteAnimator>();
		_animator._tar = _renderer;
		anims.animations_add_to_animator(_animator);
		_animator.play_anim(FootballerAnimResource.ANIM_IDLE);

	}

	public void sim_update_image() {
		if (_current_mode == GenericFootballerMode.Stunned) {
			_animator.play_anim(FootballerAnimResource.ANIM_HURT);

		} else if (Main.LevelController.footballer_has_ball(this)) {
			if (_ball_charging) {
				_animator.play_anim(FootballerAnimResource.ANIM_HOLD);
			} else {
				_animator.play_anim(FootballerAnimResource.ANIM_RUN_BALL);
			}
		} else {
			if (this.get_calculated_velocity().magnitude > 0f) {
				_animator.play_anim(FootballerAnimResource.ANIM_RUN);
			} else {
				_animator.play_anim(FootballerAnimResource.ANIM_IDLE);
			}
		}


	}

	void Update() {
		_renderer.sortingOrder = (int)(-transform.position.y * 100);
		_select_reticule.sortingOrder = _renderer.sortingOrder-1;

		float reticule_anim_speed = 0.5f;
		float reticule_scale = 200;
		Color tar = Color.white;
		if (Main.LevelController.footballer_has_ball(this)) {
			reticule_anim_speed = -1.3f;
			reticule_scale = 270;
			tar = new Color(0.25f,0.86f,0.25f);

		} else if (Main.LevelController.m_timeoutSelectedFootballer == this) {
			reticule_anim_speed = 1.5f;
			reticule_scale = 270;

		} else if (_selected_ct > 0) {
			reticule_anim_speed = 1.5f;
			reticule_scale = 230;

		}
		tar.a = _select_reticule.color.a;
		_select_reticule.color = tar;
		_selected_ct -= Util.dt_scale;

	
		_select_reticule_theta += reticule_anim_speed * Util.dt_scale;
		Util.transform_set_euler_world(_select_reticule.transform,new Vector3(0,0,_select_reticule_theta));
		Color color = _select_reticule.color;
		color.a = Util.drpt(color.a,_select_reticule_target_alpha,1/4.0f);
		_select_reticule.color = color;
		
		_select_reticule.transform.localScale = Util.valv(reticule_scale);
		
	}

	[SerializeField] public float _throw_charge_ct;
	private List<Vector3> __tmp = new List<Vector3>();
	private bool _ball_charging = false;
	public void sim_update() {
		this.sim_update_image();
		if (Main.LevelController.get_footballer_team(this) == Team.PlayerTeam) {
			this.playerteam_sim_update();
		} else {
			this.enemyteam_sim_update();
		}
		this.update_calculated_velocity();
		this.sim_update_bump();
	}

	private void enemyteam_sim_update() {
		if (_current_mode == GenericFootballerMode.Stunned) {
			_stunned_vel.x = Util.drpt(_stunned_vel.x,0,0.01f);
			_stunned_vel.y = Util.drpt(_stunned_vel.y,0,0.01f);
			this.transform.position = Util.vec_add(this.transform.position,Util.vec_scale(_stunned_vel,Util.dt_scale));
			_stunned_mode_ct -= Util.dt_scale;
			if (_stunned_mode_ct <= 0) _current_mode = GenericFootballerMode.Idle;
		}
		
		// Adjust facing direction.
		BotBase bot = this.GetComponent<BotBase>();
		if (bot != null) {
			Vector3 rts = _renderer.transform.localScale;
			if (bot.Steering.CurrentVelocity.x > 0.0f) {
				rts.x = Mathf.Abs(rts.x) * 1;
			} else if (bot.Steering.CurrentVelocity.x < 0.0f) {
				rts.x = Mathf.Abs(rts.x) * -1;
			}
			_renderer.transform.localScale = rts;
		}
	}

	private void playerteam_sim_update() {
		if (Main.LevelController.footballer_has_ball(this)) {
			this.set_select_reticule_alpha(0.5f);
		} else {
			this.set_select_reticule_alpha(0.0f);
		}

		Vector3 last_pos = transform.position;

		if (!Main.LevelController.m_gameBounds.OverlapPoint(transform.position) && _stunned_mode_ct <= 0) {
			_current_mode = GenericFootballerMode.MoveBackIntoBounds;
		} else if (Main.LevelController.footballer_has_ball(this)) {
			_current_mode = GenericFootballerMode.PlayerTeamHasBall;
		} else if (_current_mode == GenericFootballerMode.PlayerTeamHasBall && !Main.LevelController.footballer_has_ball(this)) {
			_current_mode = GenericFootballerMode.Idle;
		}

		if (_current_mode == GenericFootballerMode.MoveBackIntoBounds) {
			Vector3 tar_pos = Main.LevelController.m_minGameBounds.bounds.ClosestPoint(transform.position);
			tar_pos.z = 0;
			float speed = this.get_move_speed();
			Vector3 delta =  Util.vec_sub(tar_pos,transform.position);
			if (delta.magnitude < speed) {
				transform.position = tar_pos;
			} else {
				Vector3 dir = delta.normalized;
				float mag = delta.magnitude;
				dir.Scale(Util.valv(speed));
				transform.position = Util.vec_add(transform.position,dir);
			}

			if (Main.LevelController.m_gameBounds.OverlapPoint(transform.position)) {
				if (_has_command_move_to_point) {
					_current_mode = GenericFootballerMode.CommandMoving;
				} else {
					_current_mode = GenericFootballerMode.Idle;
				}
			}

		} else if (_current_mode == GenericFootballerMode.Stunned) {
			Main.LevelController.m_pathRenderer.clear_path(_id);
			_stunned_vel.x = Util.drpt(_stunned_vel.x,0,0.01f);
			_stunned_vel.y = Util.drpt(_stunned_vel.y,0,0.01f);
			this.transform.position = Util.vec_add(this.transform.position,Util.vec_scale(_stunned_vel,Util.dt_scale));
			_stunned_mode_ct -= Util.dt_scale;
			if (_stunned_mode_ct <= 0) {
				if (_has_command_move_to_point) {
					_current_mode = GenericFootballerMode.CommandMoving;
				} else {
					_current_mode = GenericFootballerMode.Idle;
				}
			}

		} else if (_current_mode == GenericFootballerMode.CatchWait) {
			_waitdelay -= Util.dt_scale;
			if (_waitdelay < 0) _current_mode = GenericFootballerMode.Idle;
			
		} else if (_current_mode == GenericFootballerMode.PlayerTeamHasBall) {
			_has_command_move_to_point = false;
			Vector3 delta =  Util.vec_sub(Main.LevelController.GetLastMousePointInBallBounds(),transform.position);
			Vector3 dir = delta.normalized;
			float mag = delta.magnitude;
			Main.LevelController.m_pathRenderer.clear_path(_id);
			
			if (Input.GetMouseButton(0)) {
				_ball_charging = true;
				_throw_charge_ct = Mathf.Clamp(Mathf.Min(_throw_charge_ct+Util.dt_scale*10.0f,mag),0,700);
				
				Vector3 p0 = this.transform.position;
				Vector3 p3 = Util.vec_add(p0,Util.vec_scale(dir,_throw_charge_ct));
				Vector3 p1 = new Vector3(
					Util.lerp(p0.x,p3.x,0.25f),
					Util.lerp(p0.y,p3.y,0.25f),
					-150
				);
				Vector3 p2 = new Vector3(
					Util.lerp(p0.x,p3.x,0.75f),
					Util.lerp(p0.y,p3.y,0.75f),
					-150
				);
				__tmp.Clear();
				for (float i = 0; i < 1.0f; i += 0.125f) {
					__tmp.Add(Util.bezier_val_for_t(p0,p1,p2,p3,i));
				}
				__tmp.Add(p3);
				Main.LevelController.m_pathRenderer.id_draw_path(_id,this.transform.position,__tmp.ToArray());
				
			} else if (!Input.GetMouseButton(0) && _ball_charging) {
				if (_throw_charge_ct > 100) {
					throw_ball(dir, _throw_charge_ct);
				}
				_ball_charging = false;
				_throw_charge_ct = 0;
				
			} else {
				_throw_charge_ct = 0;
				float scf = Mathf.Clamp(mag,0,200)/200.0f;
				float speed = this.get_move_speed_active() * Util.dt_scale;
				dir.Scale(Util.valv(scf*speed));
				if (transform.position.x + dir.x > Main.LevelController._left_goal_line.transform.position.x && transform.position.x + dir.x < Main.LevelController._right_goal_line.transform.position.x){
					transform.position = Util.vec_add(transform.position,dir);
				}
			}
			
		} else if (_current_mode == GenericFootballerMode.Idle) {
			
		} else if (_current_mode == GenericFootballerMode.CommandMoving) {
			float speed = this.get_move_speed() * Util.dt_scale;
			Vector3 delta = Util.vec_sub(new Vector3(_command_move_to_point.x,_command_move_to_point.y),transform.position);
			if (delta.magnitude <= speed) {
				transform.position = new Vector3(_command_move_to_point.x,_command_move_to_point.y);
				_current_mode = GenericFootballerMode.Idle;
				_has_command_move_to_point = false;
				
			} else {
				Vector3 dir = delta.normalized;
				dir.Scale(Util.valv(speed));
				transform.position = Util.vec_add(transform.position,dir);
			}
			
		}
		
		Vector3 rts = _renderer.transform.localScale;
		if (Main.LevelController.footballer_has_ball(this) && _ball_charging) {
			if (Main.LevelController.GetLastMousePointInBallBounds().x > transform.position.x) {
				rts.x = Mathf.Abs(rts.x) * 1;
			} else {
				rts.x = Mathf.Abs(rts.x) * -1;
			}

		} else if (transform.position.x > last_pos.x) {
			rts.x = Mathf.Abs(rts.x) * 1;
		} else if (transform.position.x < last_pos.x) {
			rts.x = Mathf.Abs(rts.x) * -1;
		}
		_renderer.transform.localScale = rts;
	}
	
	public void throw_ball(Vector3 dir, float charge_ct) {
		float vel = Mathf.Clamp(charge_ct/2000.0f * 10 + 6,6,18);
		Main.LevelController.m_commentaryManager.notify_tutorial_pass_thrown();
		Main.AudioController.PlayEffect("sfx_throw");
		Main.LevelController.CreateLooseBall(
			this.transform.position,
			Util.vec_scale(dir,vel)
			);
		Main.LevelController.m_playerTeamFootballersWithBall.Remove(this);
		Main.LevelController.m_enemyTeamFootballersWithBall.Remove(this);

		if (Main.LevelController.get_footballer_team(this) == Team.PlayerTeam && dir.x > 0.7f && this.transform.position.x > 950) {
			Main.LevelController.m_commentaryManager.notify_event(CommentaryEvent.ShotOnGoal,true);
		}
	}

	public float get_move_speed() { return 3.5f; }
	public float get_move_speed_active() { return 2.5f; }

	public void sim_update_bump() {
		_cannot_stun_ct -= Util.dt_scale;
		for (int i = 0; i < Main.LevelController.m_playerTeamFootballers.Count; i++) {
			GenericFootballer itr = Main.LevelController.m_playerTeamFootballers[i];
			if (itr != this) this.check_bump_with_target(itr);
		}
		for (int i = 0; i < Main.LevelController.m_enemyTeamFootballers.Count; i++) {
			GenericFootballer itr = Main.LevelController.m_enemyTeamFootballers[i];
			if (itr != this) this.check_bump_with_target(itr);
		}

		Vector3 imgpos = _renderer.transform.localPosition;
		_stunned_upwards_vel -= 0.085f * Util.dt_scale;
		imgpos.y += _stunned_upwards_vel;
		if (imgpos.y <= 0) {
			imgpos.y = 0;
			_stunned_upwards_vel = Mathf.Abs(_stunned_upwards_vel) * 0.5f;
		}
		_renderer.transform.localPosition = imgpos;
	}

	private void check_bump_with_target(GenericFootballer tar) {
		if (this.collider_contains(tar.GetComponent<CircleCollider2D>()) && this.get_calculated_velocity().magnitude > 0) {
			Vector2 calc_vel_dir = this.get_calculated_velocity().normalized;
			float mag = Mathf.Max(this.get_calculated_velocity().magnitude,tar.get_calculated_velocity().magnitude);
			Vector2 bump_vel = calc_vel_dir * mag;

			Vector3 hit_spot = new Vector3(
				(transform.position.x + tar.transform.position.x)/2,
				(transform.position.y + tar.transform.position.y)/2,
				(transform.position.z + tar.transform.position.z)/2
			);
			if (Main.LevelController.get_footballer_team(this) == Team.PlayerTeam) {
				Main.LevelController.m_commentaryManager.notify_event(CommentaryEvent.Block);
			}
			this.apply_bump(Util.vec_scale(bump_vel,-1),hit_spot,true);
			tar.apply_bump(bump_vel,hit_spot);
		}
	}

	[SerializeField] private Vector3 _stunned_vel = Vector3.zero;
	[SerializeField] private float _stunned_mode_ct = 0;
	[SerializeField] private float _stunned_upwards_vel = 0;
	private float _cannot_stun_ct = 0;
	private void apply_bump(Vector3 vel, Vector3 hit_spot, bool do_sfx = false) {
		if (_current_mode != GenericFootballerMode.Stunned && _cannot_stun_ct <= 0) {
			if (do_sfx) {
				Main.AudioController.PlayEffect("sfx_hit");
			}
			if (vel.magnitude == 0) vel = new Vector3(Util.rand_range(-1,1),Util.rand_range(-1,1));
			int testct = 0;
			while (vel.magnitude < 2.0f && testct < 20) {
				vel = Util.vec_scale(vel,1.1f);
				testct++;
			}
			vel = Util.vec_rotate_rad(vel,Util.rand_range(-1.0f,1.0f));

			_cannot_stun_ct = Util.rand_range(30,100);
			Main.LevelController.blood_anim_at(hit_spot,4);
			Main.LevelController.collision_particle_at(this.transform.position);
			_stunned_vel = vel;
			_stunned_mode_ct = 55;
			_current_mode = GenericFootballerMode.Stunned;
			_stunned_upwards_vel = 1.5f;
			if (Main.LevelController.footballer_has_ball(this)) {
				Main.LevelController.m_commentaryManager.notify_event(CommentaryEvent.Fumbled,true);

				Main.LevelController.CreateLooseBall(
					this.transform.position,
					Util.vec_rotate_rad(Util.vec_scale(vel.normalized,3.5f),Util.rand_range(-1.4f,1.4f))
				);
				Main.LevelController.m_playerTeamFootballersWithBall.Remove(this);
				Main.LevelController.m_enemyTeamFootballersWithBall.Remove(this);
				_ball_charging = false;
				_throw_charge_ct = 0;
			}
			
			// reset previous command
			//_has_command_move_to_point = false;
			
			// ai event
			GetComponent<BotBase>().Msg_Stunned();
		}
	}

	public void timeout_start() {
		Main.LevelController.m_pathRenderer.clear_path(_id);
		if (_current_mode == GenericFootballerMode.CommandMoving || (_current_mode == GenericFootballerMode.Stunned && _has_command_move_to_point)) {
			Vector3 tar_pos = new Vector3(_command_move_to_point.x,_command_move_to_point.y);                         
			Main.LevelController.m_pathRenderer.id_draw_path(_id,this.transform.position,new Vector3[] { tar_pos });
		}
	}

	public void timeout_end() {
		Main.LevelController.m_pathRenderer.clear_path(_id);
	}

	public void timeout_update() {
		if (Main.LevelController.footballer_has_ball(this)) {
			this.set_select_reticule_alpha(0.5f);
		} else if (!this.can_take_commands()) {
			this.set_select_reticule_alpha(0.0f);
		} else if (Main.LevelController.m_timeoutSelectedFootballer == this) {
			this.set_select_reticule_alpha(0.75f);
		} else {
			this.set_select_reticule_alpha(0.25f);
		}

	}

	[SerializeField] private Vector2 _command_move_to_point;
	[SerializeField] private bool _has_command_move_to_point;
	public void CommandMoveTo(Vector2 pos) {
		if (!this.can_take_commands()) return;
		_command_move_to_point = pos;
		
		if (_current_mode != GenericFootballerMode.Stunned) {
			_current_mode = GenericFootballerMode.CommandMoving;
		}
		_has_command_move_to_point = true;

		Main.LevelController.m_pathRenderer.clear_path(_id);
		Vector3 tar_pos = new Vector3(_command_move_to_point.x,_command_move_to_point.y);                         
		Main.LevelController.m_pathRenderer.id_draw_path(_id,this.transform.position,new Vector3[] { tar_pos });
	}

	public bool ContainsPoint(Vector3 pt) {
		CircleCollider2D col = this.GetComponent<CircleCollider2D>();
		return Vector3.Distance(pt,this.transform.position) < col.radius;
	}

	public bool ContainsPointClick(Vector3 pt) {
		return Vector3.Distance(pt,this.transform.position) < 65;
	}

	[SerializeField] private float _selected_ct;
	public void SetSelectedForAFrame() {
		_selected_ct = 4;
	}

	public bool collider_contains(CircleCollider2D col) {
		float rads = col.radius + this.GetComponent<CircleCollider2D>().radius;
		return Vector3.Distance(col.transform.position,this.transform.position) < rads;
	}

	public bool can_pickup_ball() { return this._current_mode != GenericFootballerMode.Stunned && this._current_mode != GenericFootballerMode.MoveBackIntoBounds; }
	public bool can_take_commands() { 
		return 
				this._current_mode != GenericFootballerMode.MoveBackIntoBounds && 
				this._current_mode != GenericFootballerMode.PlayerTeamHasBall; 
	}

	private Vector2 _last_pos;
	private Vector2 _calc_vel;
	public Vector2 get_calculated_velocity() {
		return _calc_vel;
	}
	private void update_calculated_velocity() {
		Vector2 neu_pos = new Vector2(transform.position.x,transform.position.y);
		_calc_vel = (neu_pos - _last_pos);
		_last_pos = neu_pos;
	}

}