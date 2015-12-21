using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;

public class UiPanelGame : Uzu.UiPanel {

	public static UiPanelGame inst;

	[SerializeField] private Sprite _popup_sprite_gameon, _popup_sprite_goal, _popup_sprite_timeup;
	[SerializeField] private Image _popup_message;

	[SerializeField] private MultiText _home_score;
	[SerializeField] private MultiText _away_score;
	[SerializeField] private MultiText _time_text;
	[SerializeField] private MultiText _quarter_text;
	[SerializeField] private Image _pause_icon;
	[SerializeField] public CameraFade _fadein;
	
	[SerializeField] public GameObject _chat_head_1, _chat_head_2;

	[SerializeField] public ChatManager _chats;

	public override void OnInitialize() {
		inst = this;
	}

	private float _tar_pause_icon_alpha = 0;
	private void set_pause_icon_alpha(float val) {
		Color c = _pause_icon.color;
		c.a = val;
		_pause_icon.color = c;
	}

	public Uzu.AudioHandle _bgm_handle;
	[SerializeField] AudioMixerGroup _bgm_mixer_group;

	private AudioMixerSnapshot _audio_normal_snapshot, _audio_paused_snapshot;
	public void bgm_audio_set_paused_mode(bool val) {
		if (val) {
			_bgm_mixer_group.audioMixer.TransitionToSnapshots(new AudioMixerSnapshot[]{ _audio_paused_snapshot },new float[] { 1.0f},0.25f);
		} else {
			_bgm_mixer_group.audioMixer.TransitionToSnapshots(new AudioMixerSnapshot[]{ _audio_normal_snapshot },new float[] { 1.0f},0.25f);
		}

	}

	public override void OnEnter(Uzu.PanelEnterContext context) {
		_bgm_handle = Main.AudioController.PlayBgm(AudioClipIds.BGM_MAIN_LOOP);

		_bgm_handle._handle_audio_source.outputAudioMixerGroup = _bgm_mixer_group;
		
		_audio_normal_snapshot = _bgm_mixer_group.audioMixer.FindSnapshot("Normal");
		_audio_paused_snapshot = _bgm_mixer_group.audioMixer.FindSnapshot("Paused");

		this.bgm_audio_set_paused_mode(false);

		

		gameObject.SetActive(true);
		_home_score.set_string("0");
		_home_score.set_string("0");
		_time_text.set_string("0:00:00");
		set_pause_icon_alpha(0);
		_tar_pause_icon_alpha = 0;
		_quarter_text.set_string(Main.LevelController._quarter_display);
		//Main.LevelController.StartLevel();
		_fadein.set_alpha(1.0f);
		_fadein.set_target_alpha(0.0f);
	}
	
	public override void OnExit(Uzu.PanelExitContext context) {
		_bgm_handle._handle_audio_source.outputAudioMixerGroup = null;
		gameObject.SetActive(false);
	}
	
	private void Update() {
		if (Main.LevelController.m_currentMode == LevelController.LevelControllerMode.Timeout) {
			_tar_pause_icon_alpha = 0.75f;
		} else {
			_tar_pause_icon_alpha = 0.0f;
		}
		set_pause_icon_alpha(Util.drpt(_pause_icon.color.a,_tar_pause_icon_alpha,1/5.0f));

		_home_score.set_string(Main.LevelController._player_team_score+"");
		_away_score.set_string(Main.LevelController._enemy_team_score+"");
		_time_text.set_string(Main.LevelController.get_time_remaining_formatted());
		_quarter_text.set_string(Main.LevelController._quarter_display);
		
		#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.R)) {
			Main.LevelController.StartLevel(LevelController.StartMode.Sequence);
		}
		if (Input.GetKeyDown(KeyCode.T)) {
			Main.LevelController.StartLevel(LevelController.StartMode.Immediate);
		}
		#endif

		if (_popup_t > 0) {
			_popup_t-=0.015f*Util.dt_scale;
			float t = 1-_popup_t;
			popup_set_alpha(Util.bezier_val_for_t(new Vector2(0,0),new Vector2(0,2), new Vector2(0.5f,1), new Vector2(1,0),t).y);
			_popup_message.transform.localScale = Util.valv(Util.bezier_val_for_t(new Vector2(0,2),new Vector2(0.25f,0.25f), new Vector2(1,1), new Vector2(1,1.25f),t).y);
		} else {
			popup_set_alpha(0);
		}
		
		if (!_chats._text_scroll.finished()) {
			if (_chats._current_id == 1) {
				_chat_head_1.SetActive(true);
				_chat_head_2.SetActive(false);
			} else {
				_chat_head_1.SetActive(false);
				_chat_head_2.SetActive(true);
			}
			if (_time_until_next_talk_sound <= 0) {
				_time_until_next_talk_sound = 0.25f;
				if (_chats._current_id == 1) {
					Main.AudioController.PlayEffect("speak_0_"+((int)Util.rand_range(0,6)));
				} else {
					Main.AudioController.PlayEffect("speak_1_"+((int)Util.rand_range(0,6)));
				}
			} else {
				_time_until_next_talk_sound -= Time.deltaTime;
			}
			Main.Unpause(PauseFlags.TalkingHeadStop);
		} else {
			_time_until_next_talk_sound = 0;
			Main.Pause(PauseFlags.TalkingHeadStop);
		}
	}
	private float _time_until_next_talk_sound;

	public bool can_take_message() { return _chats._messages.Count == 0 && _chats._ct <= 0; }

	private void popup_set_alpha(float val) {
		Color c = _popup_message.color;
		c.a = val;
		_popup_message.color = c;
	}

	private float _popup_t;
	public void show_popup_message(int i) {
		_popup_t = 1;
		if (i == 0) {
			_popup_message.sprite = _popup_sprite_gameon;
		} else if (i == 1) {
			_popup_message.sprite = _popup_sprite_goal;
		} else {
			_popup_message.sprite = _popup_sprite_timeup;
		}
	}
}
