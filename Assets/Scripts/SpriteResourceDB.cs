using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public enum FootballerResourceKey {
	Enemy2,
	Enemy3,
	Player1,
	EnemyGoalie
}

public class SpriteResourceDB {

	private static Dictionary<FootballerResourceKey,FootballerAnimResource> _footballer_key_to_anim_resc;
	private static List<Sprite> _ball_anim;
	public static List<Sprite> _collision_anim;
	public static List<Sprite> _catch_anim;

	private static void Initialize() {
		_footballer_key_to_anim_resc = new Dictionary<FootballerResourceKey, FootballerAnimResource>();
		_footballer_key_to_anim_resc[FootballerResourceKey.Player1] = new FootballerAnimResource(){
			_hold_resc_keys = new List<string>() {
				"Player1/Hold/P1_Hold_0",
				"Player1/Hold/P1_Hold_2",
				"Player1/Hold/P1_Hold_4",
				"Player1/Hold/P1_Hold_6"
			},
			_hurt_resc_keys = new List<string>() {
				"Player1/Hurt/P1_Hurt_0",
				"Player1/Hurt/P1_Hurt_1",
				"Player1/Hurt/P1_Hurt_2",
				"Player1/Hurt/P1_Hurt_3"
			},
			_idle_resc_keys = new List<string>() {
				"Player1/Idle/P1 Idle_0",
				"Player1/Idle/P1 Idle_2",
				"Player1/Idle/P1 Idle_3",
				"Player1/Idle/P1 Idle_4",
				"Player1/Idle/P1 Idle_5",
				"Player1/Idle/P1 Idle_6",
				"Player1/Idle/P1 Idle_7",
				"Player1/Idle/P1 Idle_8"
			},
			_run_resc_keys = new List<string>() {
				"Player1/Run/P1_Run_0",
				"Player1/Run/P1_Run_1",
				"Player1/Run/P1_Run_2",
				"Player1/Run/P1_Run_3",
				"Player1/Run/P1_Run_4",
				"Player1/Run/P1_Run_5",
				"Player1/Run/P1_Run_6",
				"Player1/Run/P1_Run_7"
			},
			_run_ball_resc_keys = new List<string>() {
				"Player1/Run Ball/P1_Runball_0",
				"Player1/Run Ball/P1_Runball_1",
				"Player1/Run Ball/P1_Runball_2",
				"Player1/Run Ball/P1_Runball_3",
				"Player1/Run Ball/P1_Runball_4",
				"Player1/Run Ball/P1_Runball_5",
				"Player1/Run Ball/P1_Runball_6",
				"Player1/Run Ball/P1_Runball_7"
			},
			_windup_resc_keys = new List<string>() {
				"Player1/Windup/P1_Windup__000",
				"Player1/Windup/P1_Windup__001",
				"Player1/Windup/P1_Windup__002",
				"Player1/Windup/P1_Windup__003",
				"Player1/Windup/P1_Windup__004",
				"Player1/Windup/P1_Windup__005",
				"Player1/Windup/P1_Windup__006",
				"Player1/Windup/P1_Windup__007"
			}
		};
		_footballer_key_to_anim_resc[FootballerResourceKey.Player1].load_keys_to_frames();

		_footballer_key_to_anim_resc[FootballerResourceKey.Enemy2] = new FootballerAnimResource(){
			_hold_resc_keys = new List<string>() {
				"Enemy2/Hold/E2_Hold__000",
				"Enemy2/Hold/E2_Hold__001",
				"Enemy2/Hold/E2_Hold__002",
				"Enemy2/Hold/E2_Hold__003"
			},
			_hurt_resc_keys = new List<string>() {
				"Enemy2/Hurt/E2_Hurt__000",
				"Enemy2/Hurt/E2_Hurt__001",
				"Enemy2/Hurt/E2_Hurt__002",
				"Enemy2/Hurt/E2_Hurt__003"
			},
			_idle_resc_keys = new List<string>() {
				"Enemy2/Idle/E2_Idle__000",
				"Enemy2/Idle/E2_Idle__001",
				"Enemy2/Idle/E2_Idle__002",
				"Enemy2/Idle/E2_Idle__003",
				"Enemy2/Idle/E2_Idle__004",
				"Enemy2/Idle/E2_Idle__005",
				"Enemy2/Idle/E2_Idle__006",
				"Enemy2/Idle/E2_Idle__007"
			},
			_run_resc_keys = new List<string>() {
				"Enemy2/Run/E2_Run__000",
				"Enemy2/Run/E2_Run__001",
				"Enemy2/Run/E2_Run__002",
				"Enemy2/Run/E2_Run__003",
				"Enemy2/Run/E2_Run__004",
				"Enemy2/Run/E2_Run__005",
				"Enemy2/Run/E2_Run__006",
				"Enemy2/Run/E2_Run__007"
			},
			_run_ball_resc_keys = new List<string>() {
				"Enemy2/Run Ball/E2_Run Ball__000",
				"Enemy2/Run Ball/E2_Run Ball__001",
				"Enemy2/Run Ball/E2_Run Ball__002",
				"Enemy2/Run Ball/E2_Run Ball__003",
				"Enemy2/Run Ball/E2_Run Ball__004",
				"Enemy2/Run Ball/E2_Run Ball__005",
				"Enemy2/Run Ball/E2_Run Ball__006",
				"Enemy2/Run Ball/E2_Run Ball__007"
			},
			_windup_resc_keys = new List<string>() {
				"Enemy2/Windup/E2_Windup__000",
				"Enemy2/Windup/E2_Windup__001",
				"Enemy2/Windup/E2_Windup__002",
				"Enemy2/Windup/E2_Windup__003",
				"Enemy2/Windup/E2_Windup__004",
				"Enemy2/Windup/E2_Windup__005",
				"Enemy2/Windup/E2_Windup__006",
				"Enemy2/Windup/E2_Windup__007"
			}
		};
		_footballer_key_to_anim_resc[FootballerResourceKey.Enemy2].load_keys_to_frames();

		_footballer_key_to_anim_resc[FootballerResourceKey.Enemy3] = new FootballerAnimResource(){
			_hold_resc_keys = new List<string>() {
				"Enemy3/Hold/E3_Hold__000",
				"Enemy3/Hold/E3_Hold__001",
				"Enemy3/Hold/E3_Hold__002",
				"Enemy3/Hold/E3_Hold__003"
			},
			_hurt_resc_keys = new List<string>() {
				"Enemy3/Hurt/E3_Hurt__000",
				"Enemy3/Hurt/E3_Hurt__001",
				"Enemy3/Hurt/E3_Hurt__002",
				"Enemy3/Hurt/E3_Hurt__003"
			},
			_idle_resc_keys = new List<string>() {
				"Enemy3/Idle/E3_Idle__000",
				"Enemy3/Idle/E3_Idle__001",
				"Enemy3/Idle/E3_Idle__002",
				"Enemy3/Idle/E3_Idle__003",
				"Enemy3/Idle/E3_Idle__004",
				"Enemy3/Idle/E3_Idle__005",
				"Enemy3/Idle/E3_Idle__006",
				"Enemy3/Idle/E3_Idle__007"
			},
			_run_resc_keys = new List<string>() {
				"Enemy3/Run/E3_Run__000",
				"Enemy3/Run/E3_Run__001",
				"Enemy3/Run/E3_Run__002",
				"Enemy3/Run/E3_Run__003",
				"Enemy3/Run/E3_Run__004",
				"Enemy3/Run/E3_Run__005",
				"Enemy3/Run/E3_Run__006",
				"Enemy3/Run/E3_Run__007"
			},
			_run_ball_resc_keys = new List<string>() {
				"Enemy3/Run Ball/E3_Run Ball__000",
				"Enemy3/Run Ball/E3_Run Ball__001",
				"Enemy3/Run Ball/E3_Run Ball__002",
				"Enemy3/Run Ball/E3_Run Ball__003",
				"Enemy3/Run Ball/E3_Run Ball__004",
				"Enemy3/Run Ball/E3_Run Ball__005",
				"Enemy3/Run Ball/E3_Run Ball__006",
				"Enemy3/Run Ball/E3_Run Ball__007"
			},
			_windup_resc_keys = new List<string>() {
				"Enemy3/Windup/E3_Windup__000",
				"Enemy3/Windup/E3_Windup__001",
				"Enemy3/Windup/E3_Windup__002",
				"Enemy3/Windup/E3_Windup__003",
				"Enemy3/Windup/E3_Windup__004",
				"Enemy3/Windup/E3_Windup__005",
				"Enemy3/Windup/E3_Windup__006",
				"Enemy3/Windup/E3_Windup__007"
			}
		};
		_footballer_key_to_anim_resc[FootballerResourceKey.Enemy3].load_keys_to_frames();

		_footballer_key_to_anim_resc[FootballerResourceKey.EnemyGoalie] = new FootballerAnimResource(){
			_hold_resc_keys = new List<string>() {
				"Enemy Goalie/Hold/EG_Hold__000",
				"Enemy Goalie/Hold/EG_Hold__001",
				"Enemy Goalie/Hold/EG_Hold__002",
				"Enemy Goalie/Hold/EG_Hold__003"
			},
			_hurt_resc_keys = new List<string>() {
				"Enemy Goalie/Hurt/EG_Hurt__000",
				"Enemy Goalie/Hurt/EG_Hurt__001",
				"Enemy Goalie/Hurt/EG_Hurt__002",
				"Enemy Goalie/Hurt/EG_Hurt__003"
			},
			_idle_resc_keys = new List<string>() {
				"Enemy Goalie/Idle/EG_Idle__000",
				"Enemy Goalie/Idle/EG_Idle__001",
				"Enemy Goalie/Idle/EG_Idle__002",
				"Enemy Goalie/Idle/EG_Idle__003",
				"Enemy Goalie/Idle/EG_Idle__004",
				"Enemy Goalie/Idle/EG_Idle__005",
				"Enemy Goalie/Idle/EG_Idle__006",
				"Enemy Goalie/Idle/EG_Idle__007"
			},
			_run_resc_keys = new List<string>() {
				"Enemy Goalie/Run/EG_Run__000",
				"Enemy Goalie/Run/EG_Run__001",
				"Enemy Goalie/Run/EG_Run__002",
				"Enemy Goalie/Run/EG_Run__003",
				"Enemy Goalie/Run/EG_Run__004",
				"Enemy Goalie/Run/EG_Run__005",
				"Enemy Goalie/Run/EG_Run__006",
				"Enemy Goalie/Run/EG_Run__007"
			},
			_run_ball_resc_keys = new List<string>() {
				"Enemy Goalie/Run Ball/EG_Run Ball__000",
				"Enemy Goalie/Run Ball/EG_Run Ball__001",
				"Enemy Goalie/Run Ball/EG_Run Ball__002",
				"Enemy Goalie/Run Ball/EG_Run Ball__003",
				"Enemy Goalie/Run Ball/EG_Run Ball__004",
				"Enemy Goalie/Run Ball/EG_Run Ball__005",
				"Enemy Goalie/Run Ball/EG_Run Ball__006",
				"Enemy Goalie/Run Ball/EG_Run Ball__007"
			},
			_windup_resc_keys = new List<string>() {
				"Enemy Goalie/Windup/EG_Windup__000",
				"Enemy Goalie/Windup/EG_Windup__001",
				"Enemy Goalie/Windup/EG_Windup__002",
				"Enemy Goalie/Windup/EG_Windup__003",
				"Enemy Goalie/Windup/EG_Windup__004",
				"Enemy Goalie/Windup/EG_Windup__005",
				"Enemy Goalie/Windup/EG_Windup__006",
				"Enemy Goalie/Windup/EG_Windup__007"
			}
		};
		_footballer_key_to_anim_resc[FootballerResourceKey.EnemyGoalie].load_keys_to_frames();

		_ball_anim = new List<Sprite>();
		FootballerAnimResource.keys_to_frames(new List<string>(){
			"Ball/Ball__000",
			"Ball/Ball__001",
			"Ball/Ball__002",
			"Ball/Ball__003"
		},_ball_anim);
		
		_collision_anim = new List<Sprite>();
		FootballerAnimResource.keys_to_frames(new List<string>(){
			"Collision/Collide1",
			"Collision/Collide2",
			"Collision/Collide3",
			"Collision/Collide4",
		},_collision_anim);
		
		_catch_anim = new List<Sprite>();
		FootballerAnimResource.keys_to_frames(new List<string>(){
			"Catch/Catch1",
			"Catch/Catch2",
			"Catch/Catch3",
			"Catch/Catch4",
			"Catch/Catch5"
		},_catch_anim);
	}

	public static FootballerAnimResource get_footballer_anim_resource(FootballerResourceKey key) {
		if (_footballer_key_to_anim_resc == null) SpriteResourceDB.Initialize();
		return _footballer_key_to_anim_resc[key];
	}

	public static List<Sprite> get_ball_anim() { return _ball_anim; }

}

public class FootballerAnimResource {
	public List<string> _hold_resc_keys;
	public List<string> _hurt_resc_keys;
	public List<string> _idle_resc_keys;
	public List<string> _run_resc_keys;
	public List<string> _run_ball_resc_keys;
	public List<string> _windup_resc_keys;

	public List<Sprite> _hold_frames = new List<Sprite>();
	public List<Sprite> _hurt_frames = new List<Sprite>();
	public List<Sprite> _idle_frames = new List<Sprite>();
	public List<Sprite> _run_frames = new List<Sprite>();
	public List<Sprite> _run_ball_frames = new List<Sprite>();
	public List<Sprite> _windup_frames = new List<Sprite>();

	public void load_keys_to_frames() {
		this.keys_to_frames(_hold_resc_keys,_hold_frames);
		this.keys_to_frames(_hurt_resc_keys,_hurt_frames);
		this.keys_to_frames(_idle_resc_keys,_idle_frames);
		this.keys_to_frames(_run_resc_keys,_run_frames);
		this.keys_to_frames(_run_ball_resc_keys,_run_ball_frames);
		this.keys_to_frames(_windup_resc_keys,_windup_frames);
	}

	public static void keys_to_frames(List<string> keys, List<Sprite> frames) {
		for (int i = 0; i < keys.Count; i++) {
			string itr_key = keys[i];
			Sprite val = Resources.Load<Sprite>(itr_key);
			if (val == null) {
				throw new UnityException("key is null:"+itr_key);
			}
			frames.Add(val);
		}
	}

	public void animations_add_to_animator(SpriteAnimator anim) {
		anim.add_anim(ANIM_HOLD,_hold_frames,5);
		anim.add_anim(ANIM_HURT,_hurt_frames,5);
		anim.add_anim(ANIM_IDLE,_idle_frames,5);
		anim.add_anim(ANIM_RUN,_run_frames,5);
		anim.add_anim(ANIM_RUN_BALL,_run_ball_frames,5);
		anim.add_anim(ANIM_WINDUP,_windup_frames,5);
	}

	public const string ANIM_HOLD = "hold";
	public const string ANIM_HURT = "hurt";
	public const string ANIM_IDLE = "idle";
	public const string ANIM_RUN = "run";
	public const string ANIM_RUN_BALL = "run_ball";
	public const string ANIM_WINDUP = "windup";


}
