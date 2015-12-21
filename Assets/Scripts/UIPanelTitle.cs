using UnityEngine;
using System.Collections;

public class UIPanelTitle : Uzu.UiPanel {

	public override void OnInitialize() {}

	[SerializeField] private CameraFade _camera_fade;
	[SerializeField] private SpriteRenderer _spotco_logo;
	[SerializeField] private SpriteRenderer _mnm_logo;
	[SerializeField] private SpriteRenderer _mnm_base;
	[SerializeField] private SpriteRenderer _cover;
	[SerializeField] private SpriteRenderer _click_anywhere_to_start;
	[SerializeField] private SpriteRenderer _credits;

	private Uzu.AudioHandle _intro_bgm_handle;

	enum TitleState {
		SpotcoLogoIn,
		SpotcoLogoOut,
		MNMLogoIn,
		FadeToLoop,
		Loop,
		FadeOutToTV,
		GotoTV
	}

	private TitleState _current_state;
	private float _anim_t;

	public override void OnEnter(Uzu.PanelEnterContext context) {
		Main.GameCamera.gameObject.SetActive(false);
		Main.Instance._tvCamera.gameObject.SetActive(false);
		Main.Instance._titleCamera.transform.parent.gameObject.SetActive(true);
		gameObject.SetActive(true);
		_intro_bgm_handle = Main.AudioController.PlayBgm(AudioClipIds.BGM_MENU_INTRO,false);

		sprite_set_alpha(_spotco_logo,0);
		sprite_set_alpha(_mnm_base,1);
		sprite_set_alpha(_mnm_logo,1);
		sprite_set_y(_mnm_logo,-280);
		sprite_set_alpha(_mnm_base,1);
		sprite_set_alpha(_cover,1);
		sprite_set_alpha(_click_anywhere_to_start,0);
		sprite_set_alpha(_credits,0);
		_camera_fade.set_alpha(1);
		_camera_fade.set_target_alpha(1);

		_current_state = TitleState.SpotcoLogoIn;

		_anim_t = 0;
	}
	
	public override void OnExit(Uzu.PanelExitContext context) {
		Main.Instance._titleCamera.transform.parent.gameObject.SetActive(false);
		gameObject.SetActive(false);
	}

	void Update() {
		if (_intro_bgm_handle._handle_audio_source != null && !_intro_bgm_handle._handle_audio_source.isPlaying) {
			_intro_bgm_handle._handle_audio_source = null;
			Main.AudioController.PlayBgm(AudioClipIds.BGM_MENU_LOOP);
		}
		switch (_current_state) {
		case TitleState.SpotcoLogoIn:{
			_anim_t += 0.015f;
			sprite_set_alpha(_spotco_logo,_anim_t);
			if (_anim_t >= 1.0f) {
				_current_state = TitleState.SpotcoLogoOut;
			}

		} break;
		case TitleState.SpotcoLogoOut:{
			_anim_t -= 0.015f;
			sprite_set_alpha(_spotco_logo,_anim_t);
			if (_anim_t <= 0.0f) {
				_current_state = TitleState.MNMLogoIn;
				_anim_t = 0;
				Main.AudioController.PlayEffect("sfx_pickup");
			}

		} break;
		case TitleState.MNMLogoIn:{
			_anim_t += 0.025f;
			sprite_set_y(_mnm_logo,
				Util.y_for_point_of_2pt_line(
					new Vector2(0,-280),
					new Vector2(1,300),
					Util.bezier_val_for_t(
						new Vector2(0,0),
						new Vector2(0.5f,0),
						new Vector2(0.5f,1),
						new Vector2(1,1),
						_anim_t
					).y));
			if (_anim_t >= 1) {
				_current_state = TitleState.FadeToLoop;
				_anim_t = 1;
				sprite_set_alpha(_cover,0);
			}
		} break;
		case TitleState.FadeToLoop: {
			_anim_t -= 0.05f;
			_camera_fade.set_alpha(_anim_t);
			_camera_fade.set_target_alpha(_anim_t);
			sprite_set_alpha(_credits,1-_anim_t);
			if (_anim_t <= 0) {
				Main.AudioController.PlayEffect("crowd");
				_current_state = TitleState.Loop;
				_anim_t = 0;
			}

		} break;
		case TitleState.Loop: {
			_anim_t += 0.05f;
			sprite_set_alpha(_click_anywhere_to_start,(Mathf.Sin(_anim_t)+1)/2.0f);
			if (Input.GetMouseButtonUp(0)) {
				_current_state = TitleState.FadeOutToTV;
				_anim_t = 0;
				Main.AudioController.PlayEffect("sfx_checkpoint");
			}

		} break;
		case TitleState.FadeOutToTV: {
			_anim_t += 0.015f;
			_camera_fade.set_alpha(_anim_t);
			_camera_fade.set_target_alpha(_anim_t);
			sprite_set_alpha(_mnm_base,1-_anim_t);
			sprite_set_alpha(_mnm_logo,1-_anim_t);
			sprite_set_alpha(_credits,1-_anim_t);
			sprite_set_alpha(_click_anywhere_to_start,0);
			if (_anim_t >= 1) {
				_current_state = TitleState.GotoTV;
			}

		} break;
		case TitleState.GotoTV: {
			Main.PanelManager.ChangeCurrentPanel(PanelIds.Tv);

		} break;
		}
	}

	private static void sprite_set_alpha(SpriteRenderer sprite, float val) {
		Color c = sprite.color;
		c.a = val;
		sprite.color = c;
	}
	private static void sprite_set_y(SpriteRenderer sprite, float y) {
		sprite.transform.localPosition = new Vector3(sprite.transform.localPosition.x,y,sprite.transform.localPosition.z);
	}
}
