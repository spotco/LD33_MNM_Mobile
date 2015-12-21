using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
public class LooseBall : MonoBehaviour {

	[SerializeField] public Vector2 _vel; 
	[SerializeField] public float _z, _vz;
	[SerializeField] private GameObject _ball;
	[SerializeField] private float _initial_uncatchable_ct;
	[SerializeField] private SpriteRenderer _renderer;
	private SpriteAnimator _animator;

	public void sim_initialize(Vector2 position, Vector2 vel) {
		this.transform.position = position;
		_vel = vel;
		_z = 0;
		_vz = Mathf.Min(vel.magnitude * 0.25f,15);
		_initial_uncatchable_ct = 20;
		_animator = this.gameObject.AddComponent<SpriteAnimator>();
		_animator.add_anim(
			"ball",SpriteResourceDB.get_ball_anim(),5
		);
		_animator._tar = _renderer;
		_animator.play_anim("ball");
	}

	public void Update() {
		_renderer.sortingOrder = (int)(-transform.position.y * 100)+1;
	}

	private void set_ball_z(float z) {
		Vector3 set = new Vector3(0,0,(-z-5)*2);
		_ball.transform.localPosition = set;
	}

	public void sim_update() {
		Vector3 pre_position = transform.position;
		Vector2 fvel = Util.vec_scale(_vel,Util.dt_scale);
		transform.position = Util.vec_add(transform.position,fvel);
		if (!Main.LevelController.m_ballBounds.OverlapPoint(transform.position)) {
			transform.position = pre_position;
			_vel.x *= -0.15f;
			_vel.y *= -0.15f;
		}

		_vz -= 0.05f * Util.dt_scale;
		_z += _vz;
		if (_z <= 0) {
			_z = 0;
			_vz = Mathf.Abs(_vz) * 0.5f;
			_vel.x *= 0.5f;
			_vel.y *= 0.5f;
		}
		this.set_ball_z(_z);

		this._initial_uncatchable_ct -= Util.dt_scale;
		if (this._initial_uncatchable_ct <= 0 && _z < 50) {
			for (int i = 0; i < Main.LevelController.m_playerTeamFootballers.Count; i++) {
				GenericFootballer itr = Main.LevelController.m_playerTeamFootballers[i];
				if (itr.can_pickup_ball() && itr.collider_contains(this.GetComponent<CircleCollider2D>())) {
					Main.LevelController.PickupLooseBall(this,itr);
					Main.AudioController.PlayEffect("sfx_pickup");

					return;
				}
			}
			for (int i = 0; i < Main.LevelController.m_enemyTeamFootballers.Count; i++) {
				GenericFootballer itr = Main.LevelController.m_enemyTeamFootballers[i];
				if (itr.can_pickup_ball() && itr.collider_contains(this.GetComponent<CircleCollider2D>())) {
					Main.LevelController.PickupLooseBall(this,itr);
					Main.AudioController.PlayEffect("sfx_pickup");
					return;
				}
			}
		}
		
		if (_z > 1) {
			_particle_emit_ct -= 1;
			if (_particle_emit_ct <= 0) {
				Main.LevelController.ball_move_particle_at(_ball.transform.position, Util.rad2deg * Mathf.Atan2(_vel.y,_vel.x));
				_particle_emit_ct = Util.y_for_point_of_2pt_line(new Vector2(8,1),new Vector2(2,5),(new Vector3(_vel.x,_vel.y,_vz)).magnitude);
			}
		}
	}
	
	float _particle_emit_ct = 0;
}
