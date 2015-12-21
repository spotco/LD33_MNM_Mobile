using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

/*
AI stuff:
1. goalies should be able to block when ball carrier is in corner directly above/below goal
2. defenders should chase whenever the ball carrier is anywhere on the right side of the field
3. attackers should chase the ball carrier at all times
4. fix bug where an AI may stop responding if it gets bumped too many times
*/

public class LevelController : MonoBehaviour, TouchEventDelegate {

	[SerializeField] private GameObject proto_team;
	[SerializeField] private GameObject proto_genericFootballer;
	[SerializeField] private GameObject proto_looseBall;
	[SerializeField] private GameObject proto_mouseTarget;
	
	[SerializeField] private GameObject proto_bloodParticle;
	[SerializeField] private GameObject proto_ballTrailParticle;
	[SerializeField] private GameObject proto_refNoticeParticle;
	[SerializeField] private GameObject proto_catchParticle;
	[SerializeField] private GameObject proto_collisionParticle;
	[SerializeField] private GameObject proto_confettiParticle;
	
	[SerializeField] private GameObject proto_referee;

	public enum LevelControllerMode {
		Opening,
		GamePlay,
		Timeout,
		GoalZoomOut
	}
	
	public enum StartMode {
		Sequence,
		Immediate,
	}

	[SerializeField] public BoxCollider2D m_gameBounds;
	[SerializeField] public BoxCollider2D m_ballBounds;
	[SerializeField] public BoxCollider2D m_minGameBounds;
	[SerializeField] public AnimatedGoalPost m_playerGoal;
	[SerializeField] public AnimatedGoalPost m_enemyGoal;
	[SerializeField] public Transform _left_goal_line;
	[SerializeField] public Transform _right_goal_line;

	[System.NonSerialized] public PathRenderer m_pathRenderer;
	public List<GenericFootballer> m_playerTeamFootballers = new List<GenericFootballer>();
	public List<GenericFootballer> m_enemyTeamFootballers = new List<GenericFootballer>();


	public List<GenericFootballer> m_playerTeamFootballersWithBall = new List<GenericFootballer>();
	public List<GenericFootballer> m_enemyTeamFootballersWithBall = new List<GenericFootballer>();

	//Nullable
	public GenericFootballer m_timeoutSelectedFootballer;
	public List<LooseBall> m_looseBalls = new List<LooseBall>();
	
	public List<int> m_matchOpeningAnimIds = new List<int>();

	public SPParticleSystem m_particles;

	public LevelControllerMode m_currentMode;
	
	private TeamBase m_playerTeam;
	private TeamBase m_enemyTeam;

	private Referee m_topReferee, m_bottomReferee;

	public InGameCommentaryManager m_commentaryManager;
	
	public MNMTouchControlManager _control_manager = new MNMTouchControlManager();
	
	private GameObject m_mouseTargetIcon;
	private float m_mouseTargetIconTheta;
	private void mouse_target_icon_set_alpha(float val) {
		Color c = m_mouseTargetIcon.GetComponent<SpriteRenderer>().color;
		c.a = val;
		m_mouseTargetIcon.GetComponent<SpriteRenderer>().color = c;
	}
	
	private Difficulty _currentDifficulty;
	public Difficulty CurrentDifficulty {
		get { return _currentDifficulty; }
		set {
			_currentDifficulty = value;
		}
	}

	private Team __commentary_last_team_to_own_ball = Team.None;
		
	public void StartLevel(StartMode startMode = StartMode.Sequence) {
		Main.AudioController.PlayEffect("crowd");
		ResetLevel();
		_control_manager.i_initialize();
		//Debug.Log("Start level: " + CurrentDifficulty);

		m_commentaryManager = new InGameCommentaryManager();
		
		m_pathRenderer = this.GetComponent<PathRenderer>();
		
		m_playerTeam = this.CreateTeam(Team.PlayerTeam);
		m_enemyTeam = this.CreateTeam(Team.EnemyTeam);

		this.set_time_remaining_seconds(300);
		
		if (CurrentDifficulty == Difficulty.Easy) {
			this.set_time_remaining_seconds(300);
			_player_team_score = 0;
			_enemy_team_score = 0;
			_quarter_display = "1ST";
			{
				int[] regions = { 7, 6, 8 };
				FootballerResourceKey[] keys = { FootballerResourceKey.Player1, FootballerResourceKey.Player1, FootballerResourceKey.Player1 };
				FieldPosition[] fps = { FieldPosition.Keeper, FieldPosition.Defender, FieldPosition.Defender };
				SpawnTeam(7, m_playerTeam, regions, keys, fps);
			}
			{
				int[] regions = { 16, 12, 14 };
				FootballerResourceKey[] keys = { FootballerResourceKey.EnemyGoalie, FootballerResourceKey.Enemy3, FootballerResourceKey.Enemy3 };
				FieldPosition[] fps = { FieldPosition.Keeper, FieldPosition.Defender, FieldPosition.Defender };
				SpawnTeam(10, m_enemyTeam, regions, keys, fps);
			}
			m_commentaryManager.notify_do_tutorial();

		} else if (CurrentDifficulty == Difficulty.Normal) {
			this.set_time_remaining_seconds(200);
			_player_team_score = 2;
			_enemy_team_score = 2;
			_quarter_display = "2ND";
			{
				int[] regions = { 7, 3, 4, 5 };
				FootballerResourceKey[] keys = { FootballerResourceKey.Player1, FootballerResourceKey.Player1, FootballerResourceKey.Player1, FootballerResourceKey.Player1 };
				FieldPosition[] fps = { FieldPosition.Keeper, FieldPosition.Defender, FieldPosition.Defender, FieldPosition.Attacker };
				SpawnTeam(7, m_playerTeam, regions, keys, fps);
			}
			{
				int[] regions = { 16, 12, 14, 13 };
				FootballerResourceKey[] keys = { FootballerResourceKey.EnemyGoalie, FootballerResourceKey.Enemy3, FootballerResourceKey.Enemy3, FootballerResourceKey.Enemy2 };
				FieldPosition[] fps = { FieldPosition.Keeper, FieldPosition.Defender, FieldPosition.Defender, FieldPosition.Attacker };
				SpawnTeam(10, m_enemyTeam, regions, keys, fps);
			}
		} else {
			this.set_time_remaining_seconds(120);
			_player_team_score = 4;
			_enemy_team_score = 4;
			_quarter_display = "4TH";
			{
				int[] regions = { 7, 3, 5, 6, 8 };
				FootballerResourceKey[] keys = { FootballerResourceKey.Player1, FootballerResourceKey.Player1, FootballerResourceKey.Player1, FootballerResourceKey.Player1, FootballerResourceKey.Player1 };
				FieldPosition[] fps = { FieldPosition.Keeper, FieldPosition.Defender, FieldPosition.Defender, FieldPosition.Attacker, FieldPosition.Attacker };
				SpawnTeam(7, m_playerTeam, regions, keys, fps);
			}
			{
				int[] regions = { 16, 12, 14, 9, 11 };
				FootballerResourceKey[] keys = { FootballerResourceKey.EnemyGoalie, FootballerResourceKey.Enemy3, FootballerResourceKey.Enemy3, FootballerResourceKey.Enemy2, FootballerResourceKey.Enemy2 };
				FieldPosition[] fps = { FieldPosition.Keeper, FieldPosition.Defender, FieldPosition.Defender, FieldPosition.Attacker, FieldPosition.Attacker };
				SpawnTeam(10, m_enemyTeam, regions, keys, fps);
			}
		}
		
		// Debugging.
		if (Main.FSMDebugger != null) {
			Main.FSMDebugger.Team = m_enemyTeam;
		}
		
		if (m_mouseTargetIcon == null) {
			m_mouseTargetIcon = Util.proto_clone(proto_mouseTarget);
		}
		if (m_particles == null) {
			m_particles = SPParticleSystem.cons_anchor(Main.Instance._particleRoot.transform);
		}
		
		if (m_topReferee == null) {
			m_topReferee = Util.proto_clone(proto_referee).GetComponent<Referee>();
			m_bottomReferee = Util.proto_clone(proto_referee).GetComponent<Referee>();
		}
		m_topReferee.sim_initialize(Referee.RefereeMode.Top);
		m_bottomReferee.sim_initialize(Referee.RefereeMode.Bottom);
		
		_camera_focus_position = new Vector3(0,-300,0);
		Main.GameCamera.SetTargetPos(_camera_focus_position);
		Main.GameCamera.SetTargetZoom(800);
		switch (startMode) {
			case StartMode.Sequence:
				DoMatchOpeningSequence();
				break;
			case StartMode.Immediate:
				DoMatchOpeningImmediate();
				break;
		}
	}

	private float _countdown_ct;
	private float _last_countdown_ct;
	private void DoMatchOpeningSequence() {
		m_currentMode = LevelControllerMode.Opening;
		_countdown_ct = 0;
		
		List<BotBase> allBots = new List<BotBase>(
			m_playerTeam.TeamMembers.Count + m_enemyTeam.TeamMembers.Count);
		allBots.AddRange(m_playerTeam.TeamMembers);
		allBots.AddRange(m_enemyTeam.TeamMembers);
		
		for (int i = 0; i < allBots.Count; i++) {
			BotBase bot = allBots[i];
			GenericFootballer footballer = bot.GetComponent<GenericFootballer>();
			footballer.force_play_animation(FootballerAnimResource.ANIM_RUN);
			footballer.force_facing_direction(bot.HomePosition.x >= bot.transform.position.x ? true : false);
			
			float d = Vector3.Distance(bot.transform.position, bot.HomePosition);
			float r = Util.rand_range(200.0f, 220.0f);
			float t = d / r;
			_countdown_ct = Math.Max(_countdown_ct,t);
			_last_countdown_ct = _countdown_ct;
			
			LTDescr animDesc = LeanTween.move(
				bot.gameObject,
				bot.HomePosition,
				t)
				.setEase(LeanTweenType.linear);
			int animId = animDesc.id;
			animDesc.setOnComplete(() => {
				footballer.force_play_animation(FootballerAnimResource.ANIM_IDLE);
				footballer.force_facing_direction(bot.Team == Team.PlayerTeam ? true : false);
				
				m_matchOpeningAnimIds.Remove(animId);
			});
			
			m_matchOpeningAnimIds.Add(animId);
		}
		
		{
			m_topReferee.transform.position = Main.FieldController.GetFieldCenter();
			CreateLooseBall(m_topReferee.transform.position, Vector3.zero);
		}
	}
	
	private void DoMatchOpeningImmediate() {
		m_currentMode = LevelControllerMode.GamePlay;
		
		List<BotBase> allBots = new List<BotBase>(
			m_playerTeam.TeamMembers.Count + m_enemyTeam.TeamMembers.Count);
		allBots.AddRange(m_playerTeam.TeamMembers);
		allBots.AddRange(m_enemyTeam.TeamMembers);
		
		for (int i = 0; i < allBots.Count; i++) {
			allBots[i].transform.position = allBots[i].HomePosition;
		}
		
		{
			Vector3 pos = Main.FieldController.GetFieldCenter();
			CreateLooseBall(pos, Vector3.zero);
		}
		
		m_enemyTeam.StartMatch();
	}
	
	private void ResetLevel() {
		if (m_pathRenderer != null) {
			m_pathRenderer.clear_paths();
		}
		
		m_matchOpeningAnimIds.Clear();
		
		if (m_playerTeam != null) {
			TeamBase team = m_playerTeam.GetComponent<TeamBase>();
			foreach (BotBase member in team.TeamMembers) {
				if (member != null) {
					GameObject.Destroy(member.gameObject);
				}
			}
			GameObject.Destroy(m_playerTeam.gameObject);
		}
		
		if (m_enemyTeam != null) {
			TeamBase team = m_enemyTeam.GetComponent<TeamBase>();
			foreach (BotBase member in team.TeamMembers) {
				if (member != null) {
					GameObject.Destroy(member.gameObject);
				}
			}
			GameObject.Destroy(m_enemyTeam.gameObject);
		}
		
		if (m_looseBalls != null) {
			for (int i = 0; i < m_looseBalls.Count; i++) {
				GameObject.Destroy(m_looseBalls[i].gameObject);
			}
			m_looseBalls.Clear();
		}
		
		m_playerTeamFootballers.Clear();
		m_enemyTeamFootballers.Clear();
		
		m_playerTeamFootballersWithBall.Clear();
		m_enemyTeamFootballersWithBall.Clear();
		if (m_particles != null) m_particles.clear();
		// Memory cleanup.
		{
			System.GC.Collect();
			Resources.UnloadUnusedAssets();
		}
		Main.GameCamera.reset();
	}
	
	private void SpawnTeam(int centerRegion, TeamBase team,
		int[] regions, FootballerResourceKey[] resources, FieldPosition[] fieldPositions) {
		const float startOffset = 100.0f;
		Vector3 centerPos = Main.FieldController.GetRegionPosition(centerRegion);
		float deltaAngle = Mathf.Deg2Rad * (360.0f / regions.Length);
		List<BotBase> bots = new List<BotBase>(regions.Length);
		for (int i = 0; i < regions.Length; i++) {
			BotBase bot = this.CreateFootballer(team,
				centerPos + Uzu.Math.RadiansToDirectionVector(deltaAngle * i) * startOffset,
				SpriteResourceDB.get_footballer_anim_resource(resources[i]));
			bot.HomePosition = Main.FieldController.GetRegionPosition(regions[i]);
			bot.FieldPosition = fieldPositions[i];
			bots.Add(bot);
		}
		team.SetPlayers(bots);
	}
	
	public void TouchBeginWithScreenPosition(Vector2 spos) { _control_manager.TouchBeginWithScreenPosition(spos); }
	public void TouchHoldWithScreenPosition(Vector2 spos) { _control_manager.TouchHoldWithScreenPosition(spos); }
	public void TouchEnd() { _control_manager.TouchEnd(); }
	public int GetID() { return this.GetInstanceID(); }
	public void notify_pause_button_toggled() {
		_control_manager.notify_pause_button_toggled();
	}

	public Vector3 _camera_focus_position;
	
	private GenericFootballer _try_gameplay_select_footballer;
	private System.DateTime _try_gameplay_select_footballer_time;

	public void FixedUpdate() {
		if (Main.PanelManager.CurrentPanelId != PanelIds.Game) return;
		
		UiPanelGame.inst._touch_dispatcher.PDispatchTouchWithDelegate(UiPanelGame.inst,UiPanelGame.inst._touch_bounds);
		
		m_commentaryManager.i_update();

		float mouse_target_anim_speed = 0.3f;
		if (m_currentMode == LevelControllerMode.GoalZoomOut) {
			UiPanelGame.inst._fadein.set_target_alpha(1);
			_camera_focus_position.x = CurveAnimUtil.ApplyFrictionTick(_camera_focus_position.x,_goalzoomoutfocuspos.x,1/10.0f);
			_camera_focus_position.y = CurveAnimUtil.ApplyFrictionTick(_camera_focus_position.y,_goalzoomoutfocuspos.y,1/10.0f);
			_camera_focus_position.z = CurveAnimUtil.ApplyFrictionTick(_camera_focus_position.z,_goalzoomoutfocuspos.z,1/10.0f);
			Main.GameCamera.SetTargetPos(_camera_focus_position);
			Main.GameCamera.SetTargetZoom(300);
			m_enemyGoal.spawn_confetti();
			m_particles.i_update(this);
			_control_manager.get_and_clear_pause_button_pressed();
			if (UiPanelGame.inst._fadein.is_transition_finished()) {
				this.ResetLevel();
				Main.PanelManager.ChangeCurrentPanel(PanelIds.Tv);
			}

		} else if (m_currentMode == LevelControllerMode.GamePlay) {

			Team ball_owner_team = this.get_footballer_team(this.nullableCurrentFootballerWithBall());
			if (ball_owner_team != Team.None) {
				__commentary_last_team_to_own_ball = ball_owner_team;
			}

			_time_remaining = Math.Max(0,_time_remaining-TimeSpan.FromSeconds(Time.deltaTime).Ticks);
			if (_time_remaining <= 0) {
				m_currentMode = LevelControllerMode.GoalZoomOut;
				
				Main._current_repeat_reason = RepeatReason.Timeout;
				UiPanelGame.inst.show_popup_message(2);
				_goalzoomoutfocuspos = Main.GameCamera.GetCurrentPosition();
				return;
			}
			m_particles.i_update(this);
			if (m_playerTeamFootballersWithBall.Count > 0) {
				Vector3 tar_pos = m_playerTeamFootballersWithBall[0].transform.position;
				_camera_focus_position.x = CurveAnimUtil.ApplyFrictionTick(_camera_focus_position.x,tar_pos.x,1/10.0f);
				_camera_focus_position.y = CurveAnimUtil.ApplyFrictionTick(_camera_focus_position.y,tar_pos.y,1/10.0f);
				_camera_focus_position.z = CurveAnimUtil.ApplyFrictionTick(_camera_focus_position.z,tar_pos.z,1/10.0f);
				Main.GameCamera.SetTargetPos(_camera_focus_position);
				Main.GameCamera.SetTargetZoom(600);
				Main.GameCamera.SetManualOffset(new Vector3(0,0,0));

			} else {
				Main.GameCamera.SetTargetPos(_camera_focus_position);
				Main.GameCamera.SetTargetZoom(600);
				Main.GameCamera.SetManualOffset(new Vector3(0,0,0));
				this.camera_control_pan();
			}

			mouse_target_anim_speed = 2.0f;

			bool skip_updates = false;
			if (_control_manager._this_frame_touch_ended && !_control_manager._has_touch_activated_drag) {
				GenericFootballer clicked_footballer = IsPointTouchFootballer(_control_manager._last_world_touch_point,m_playerTeamFootballers);
				if (clicked_footballer != null) {
					if (m_playerTeamFootballersWithBall.Count > 0) {
						_try_gameplay_select_footballer = clicked_footballer;
						_try_gameplay_select_footballer_time = System.DateTime.Now;
					} else {
						skip_updates = true;
					}
				}
			}
			
			if (_try_gameplay_select_footballer != null) {
				if (System.DateTime.Now.Subtract(_try_gameplay_select_footballer_time).TotalSeconds > 0.11f) {
					_try_gameplay_select_footballer = null;
					if (!_control_manager._this_touch_is_double_tap) {
						skip_updates = true;
					}
				}
			}
			
			if (!skip_updates) {
				for (int i = m_looseBalls.Count-1; i >= 0; i--) {
					LooseBall itr = this.m_looseBalls[i];	
					itr.sim_update();
				}
				for (int i = this.m_playerTeamFootballers.Count-1; i >= 0; i--) {
					GenericFootballer itr = this.m_playerTeamFootballers[i];	
					itr.sim_update();
				}
				
				for (int i = 0; i < this.m_enemyTeamFootballers.Count; i++) {
					GenericFootballer itr = this.m_enemyTeamFootballers[i];	
					itr.sim_update();
				}
			}

			if (_control_manager.get_and_clear_pause_button_pressed() || skip_updates) {
				for (int i = 0; i < this.m_playerTeamFootballers.Count; i++) {
					GenericFootballer itr = this.m_playerTeamFootballers[i];
					itr.timeout_start();
				}
				m_currentMode = LevelControllerMode.Timeout;
				m_timeoutSelectedFootballer = null;
				Main.Pause(PauseFlags.TimeOut);
				m_commentaryManager.notify_tutorial_just_pressed_space();
				Main.AudioController.PlayEffect("sfx_pause");
				UiPanelGame.inst.bgm_audio_set_paused_mode(true);
			}

			for (int i = m_looseBalls.Count-1; i >= 0; i--) {
				LooseBall itr = this.m_looseBalls[i];
				if (m_enemyGoal.box_collider().OverlapPoint(itr.transform.position)) {
					this.blood_anim_at(itr.transform.position);
					m_looseBalls.Remove(itr);
					this.enemy_goal_score(itr.transform.position);
					Destroy(itr.gameObject);
					m_enemyGoal.play_eat_anim(40);
					Main.AudioController.PlayEffect("sfx_checkpoint");

				}
				if (m_playerGoal.box_collider().OverlapPoint(itr.transform.position)) {
					this.blood_anim_at(itr.transform.position);
					m_looseBalls.Remove(itr);
					this.player_goal_score(itr.transform.position);
					Destroy(itr.gameObject);
					m_playerGoal.play_eat_anim(40);
					UiPanelGame.inst._chats.clear_messages();
					Main.AudioController.PlayEffect("sfx_checkpoint");


				}
			}
			m_bottomReferee.sim_update();
			m_topReferee.sim_update();


		} else if (m_currentMode == LevelControllerMode.Timeout) {
			Main.GameCamera.SetTargetPos(_camera_focus_position);
			Main.GameCamera.SetManualOffset(new Vector3(0,0,0));
			Main.GameCamera.SetTargetZoom(800);
			

			for (int i = 0; i < this.m_playerTeamFootballers.Count; i++) {
				GenericFootballer itr = this.m_playerTeamFootballers[i];
				itr.timeout_update();
			}

			keyboard_switch_timeout_selected_footballer();
			
			bool force_untimeout = false;
			if (_control_manager._this_frame_touch_begin) {
				GenericFootballer clicked_footballer = IsPointTouchFootballer(_control_manager._last_world_touch_point,m_playerTeamFootballers);
				m_timeoutSelectedFootballer = clicked_footballer;
				
			} else {
				if (m_timeoutSelectedFootballer != null) {
					m_commentaryManager.notify_tutorial_command_issued();
					Vector2 click_pt = this.point_to_within_goallines_point(m_timeoutSelectedFootballer.transform.position,_control_manager._last_world_touch_point);
					m_timeoutSelectedFootballer.CommandMoveTo(click_pt);
					_control_manager._scroll_avg_vel = new Vector2();
					if (!_control_manager._is_touch_down) {
						m_timeoutSelectedFootballer = null;
					}
				
				} else {
					this.camera_control_pan();
					if (_control_manager._this_frame_touch_ended && !_control_manager._has_touch_activated_drag) {
						force_untimeout = true;
					}
				}
			}

			if (_control_manager.get_and_clear_pause_button_pressed() || force_untimeout) {
				m_currentMode = LevelControllerMode.GamePlay;
				for (int i = 0; i < this.m_playerTeamFootballers.Count; i++) {
					GenericFootballer itr = this.m_playerTeamFootballers[i];
					itr.timeout_end();
				}
				Main.Unpause(PauseFlags.TimeOut);
				Main.AudioController.PlayEffect("sfx_unpause");
				UiPanelGame.inst.bgm_audio_set_paused_mode(false);
			}
			
		} else if (m_currentMode == LevelControllerMode.Opening) {
			mouse_target_anim_speed = 2.0f;
			_control_manager.get_and_clear_pause_button_pressed();
			//m_mouseTargetIcon.SetActive(true);
			mouse_target_icon_set_alpha(1.0f);
			m_particles.i_update(this);
			_countdown_ct -= Time.deltaTime;
			if (_countdown_ct < 4f && _last_countdown_ct > 4f) {
				UiPanelGame.inst._chats.push_message("3...",2);
				Main.AudioController.PlayEffect("sfx_ready");
			} else if (_countdown_ct < 3f && _last_countdown_ct > 3f) {
				UiPanelGame.inst._chats.push_message("2...",1);
				Main.AudioController.PlayEffect("sfx_ready");
			} else if (_countdown_ct < 2f && _last_countdown_ct > 2f) {
				UiPanelGame.inst._chats.push_message("1...",2);
				Main.AudioController.PlayEffect("sfx_ready");
			}
			_last_countdown_ct = _countdown_ct;

			if (m_matchOpeningAnimIds.Count == 0) {
				m_currentMode = LevelControllerMode.GamePlay;
				//m_mouseTargetIcon.SetActive(true);
				
				// throw it in
				if (m_looseBalls.Count > 0) {
					LooseBall lb = m_looseBalls[0];
					Vector3 throwDir = m_playerTeamFootballers[0].transform.position - lb.transform.position;
					throwDir.Normalize();
					
					lb.sim_initialize(lb.transform.position, throwDir * 4.0f);
					UiPanelGame.inst.show_popup_message(0);
					Main.AudioController.PlayEffect("sfx_go");
				}
				UiPanelGame.inst._chats.push_message("And the match is underway!",1);
				m_enemyTeam.StartMatch();
			}
		}
		
		if (_control_manager._this_frame_touch_begin || _control_manager._this_frame_touch_ended) {
			_mouse_target_ct = MOUSE_TARGET_CT_MAX;
		}
		float mouse_target_anim_t = (MOUSE_TARGET_CT_MAX-_mouse_target_ct)/MOUSE_TARGET_CT_MAX;
		this.mouse_target_icon_set_alpha(Util.bezier_val_for_t(new Vector2(0,1),new Vector2(0,1),new Vector2(1,1),new Vector2(1,0),mouse_target_anim_t).y);
		m_mouseTargetIcon.transform.localScale = Util.valv(
			Util.y_for_point_of_2pt_line(new Vector2(0,50),new Vector2(1,75),
			Util.bezier_val_for_t(new Vector2(0,1.5f),new Vector2(0,0.5f),new Vector2(1,1),new Vector2(1,0),mouse_target_anim_t).y));
		
		_mouse_target_ct = Mathf.Clamp(_mouse_target_ct-CurveAnimUtil.GetDeltaTimeScale(),0,MOUSE_TARGET_CT_MAX);
		
		m_mouseTargetIcon.transform.position = _control_manager._last_world_touch_point;
		m_mouseTargetIconTheta += mouse_target_anim_speed * Util.dt_scale;
		Util.transform_set_euler_world(m_mouseTargetIcon.transform,new Vector3(0,0,m_mouseTargetIconTheta));
		
		_control_manager.i_update();
	}
	
	private void camera_control_pan() {
		if (_control_manager._is_touch_down && _control_manager._has_touch_activated_drag) {
			Vector2 tmp_point = _camera_focus_position + Util.vec_scale(_control_manager._scroll_frame_vel,-1);
			if (m_gameBounds.OverlapPoint(tmp_point)) {
				_camera_focus_position = tmp_point;
			}
			
		} else {
			Vector2 tmp_point = _camera_focus_position + Util.vec_scale(_control_manager._scroll_avg_vel,-1);
			if (m_gameBounds.OverlapPoint(tmp_point)) {
				_camera_focus_position = tmp_point;
			}
		}
	}
	
	private const float MOUSE_TARGET_CT_MAX = 20;
	private float _mouse_target_ct = 0;

	public Vector2 point_to_within_goallines_point(Vector2 start, Vector2 click_pt) {
		if (click_pt.x > _right_goal_line.position.x) {
			Vector2 intersection_pt = Util.line_seg_intersection_pts(
				start,
				click_pt,
				new Vector2(_right_goal_line.position.x,-9999),
				new Vector2(_right_goal_line.position.x,9999)
			);
			if (!float.IsNaN(intersection_pt.x)) {
				click_pt = intersection_pt;
			}

		} else if (click_pt.x < _left_goal_line.position.x) {
			Vector2 intersection_pt = Util.line_seg_intersection_pts(
				start,
				click_pt,
				new Vector2(_left_goal_line.position.x,-9999),
				new Vector2(_left_goal_line.position.x,9999)
			);
			if (!float.IsNaN(intersection_pt.x)) {
				click_pt = intersection_pt;
			}
		}
		if (click_pt.x > _right_goal_line.position.x) {
			click_pt.x = _right_goal_line.position.x;
		} else if (click_pt.x < _left_goal_line.position.x) {
			click_pt.x = _left_goal_line.position.x;
		}
		return click_pt;
	}

	private BotBase _prev_ball_owner;

	public void CreateLooseBall(Vector2 start, Vector2 vel) {
		// ai msg
		{
			if (_prev_ball_owner != null) {
				_prev_ball_owner.Msg_LostBall();
				_prev_ball_owner = null;
			}
		}
		
		GameObject neu_obj = Util.proto_clone(proto_looseBall);
		LooseBall rtv = neu_obj.GetComponent<LooseBall>();
		rtv.sim_initialize(start,vel);
		m_looseBalls.Add(rtv);
	}
	
	public void PickupLooseBall(LooseBall looseball, GenericFootballer tar) {
		m_looseBalls.Remove(looseball);
		this.catch_particle_at(looseball.transform.position);
		if (this.get_footballer_team(tar) == Team.PlayerTeam) {
			m_playerTeamFootballersWithBall.Add(tar);
			tar._current_mode = GenericFootballer.GenericFootballerMode.Idle;
			tar.set_wait_delay(15);
		} else {
			this.m_enemyTeamFootballersWithBall.Add(tar);
			tar._current_mode = GenericFootballer.GenericFootballerMode.Idle;
			tar.set_wait_delay(15);
		}
		
		// ai msg
		{
			BotBase new_ball_owner = tar.GetComponent<BotBase>();
			if (_prev_ball_owner != null) {
				_prev_ball_owner.Msg_LostBall();
			}
			new_ball_owner.Msg_GotBall();
			_prev_ball_owner = new_ball_owner;
		}

		if (__commentary_last_team_to_own_ball == Team.PlayerTeam) {
			if (__commentary_last_team_to_own_ball == this.get_footballer_team(tar)) {
				m_commentaryManager.notify_event(CommentaryEvent.PassComplete);
			} else if (__commentary_last_team_to_own_ball != this.get_footballer_team(tar) && new Vector3(looseball._vel.x,looseball._vel.y,looseball._vz).magnitude > 1) {
				m_commentaryManager.notify_event(CommentaryEvent.Interception,true);
			}
		}

		Destroy(looseball.gameObject);
	}
	
	private TeamBase CreateTeam(Team team) {
		GameObject neu_obj = Util.proto_clone(proto_team);
		TeamBase rtv = neu_obj.GetComponent<TeamBase>();
		rtv.Team = team;
		return rtv;
	}
	
	private BotBase CreateFootballer(TeamBase team, Vector3 pos, FootballerAnimResource anims) {
		GameObject neu_obj = Util.proto_clone(proto_genericFootballer);
		GenericFootballer rtv = neu_obj.GetComponent<GenericFootballer>();
		rtv.transform.position = pos;
		rtv.sim_initialize(anims);
		if (team.Team == Team.PlayerTeam) {
			m_playerTeamFootballers.Add(rtv);
		} else {
			m_enemyTeamFootballers.Add(rtv);
		}
		return neu_obj.GetComponent<BotBase>();
	}

	public void blood_anim_at(Vector3 pos, int reps = 32) {
		for (int i = 0; i < reps; i++ ) {
			RotateFadeOutSPParticle tmp = RotateFadeOutSPParticle.cons(proto_bloodParticle);
			tmp.transform.position = pos;
			tmp.set_ctmax(35);
			float scale = Util.rand_range(25,100);
			tmp._scmax = scale;
			tmp._scmin = scale;
			tmp._alpha.x = 0.6f;
			tmp._alpha.y = 0.0f;
			tmp._vr = Util.rand_range(-30,30);
			tmp._velocity.x = Util.rand_range(-3,3);
			tmp._velocity.y = Util.rand_range(0,7);
			tmp._gravity = 0.2f;
			m_particles.add_particle(tmp);
		}
	}
	
	public void ball_move_particle_at(Vector3 pos, float rotation) {
		RotateFadeOutSPParticle tmp = RotateFadeOutSPParticle.cons(proto_ballTrailParticle);
		tmp.transform.position = pos + new Vector3(Util.rand_range(-10,10),Util.rand_range(-10,10),0);
		tmp.set_ctmax(35);
		float scale = Util.rand_range(25,100);
		tmp._scmax = scale;
		tmp._scmin = scale;
		tmp._alpha.x = 0.6f;
		tmp._alpha.y = 0.0f;
		tmp._vr = 0;
		tmp.set_self_rotation(rotation);
		tmp._velocity.x = Util.rand_range(-2,2);
		tmp._velocity.y = Util.rand_range(-2,2);
		m_particles.add_particle(tmp);
	}
	
	public void catch_particle_at(Vector3 pos) {
		RotateFadeOutSPParticle tmp = RotateFadeOutSPParticle.cons(proto_catchParticle);
		tmp.transform.position = pos;
		tmp._scmin = 25;
		tmp._scmax = 35;
		tmp.set_sprite_animation(SpriteResourceDB._catch_anim,4,false);
		tmp.set_ctmax(50);
		tmp._alpha.x = 0.8f;
		tmp._alpha.y = 0.0f;
		m_particles.add_particle(tmp);
	}
	public void collision_particle_at(Vector3 pos) {
		RotateFadeOutSPParticle tmp = RotateFadeOutSPParticle.cons(proto_collisionParticle);
		tmp.transform.position = pos;
		tmp._scmin = 25;
		tmp._scmax = 35;
		tmp.set_sprite_animation(SpriteResourceDB._collision_anim,4,false);
		tmp.set_ctmax(50);
		tmp._alpha.x = 0.8f;
		tmp._alpha.y = 0.0f;
		m_particles.add_particle(tmp);
	}
	public void confetti_particle_at(Vector3 pos) {
		RotateFadeOutSPParticle tmp = RotateFadeOutSPParticle.cons(proto_confettiParticle);
		tmp.transform.position = pos;
		tmp._scmin = 65;
		tmp._scmax = 65;
		tmp.set_self_rotation(Util.rand_range(0,360));
		tmp.set_ctmax(100);
		tmp._alpha.x = 0.8f;
		tmp._alpha.y = 0.0f;
		tmp._velocity.x = Util.rand_range(-2,2);
		tmp._velocity.y = Util.rand_range(5,10);
		tmp.set_vrx(Util.rand_range(-30,30));
		tmp._vr = Util.rand_range(-30,30);
		float rnd = Util.rand_range(0,3);
		if (rnd < 1) {
			tmp.set_color(new Vector3(255.0f/255.0f,40.0f/255.0f,131.0f/255.0f));
		} else if (rnd < 2) {
			tmp.set_color(new Vector3(255.0f/255.0f,192.0f/255.0f,40.0f/255.0f));
		} else {
			tmp.set_color(new Vector3(202.0f/255.0f,40.0f/255.0f,255.0f/255.0f));
		}
		m_particles.add_particle(tmp);
	}
	public void ref_notice_particle_at(Vector3 pos) {
		RotateFadeOutSPParticle tmp = RotateFadeOutSPParticle.cons(proto_refNoticeParticle);
		tmp.transform.position = pos;
		tmp._scmin = 80;
		tmp._scmax = 120;
		tmp.set_scale(tmp._scmin);
		tmp.set_ctmax(60);
		tmp._alpha.x = 0.8f;
		tmp._alpha.y = 0.0f;
		m_particles.add_particle(tmp);
	}
	
	public GenericFootballer IsPointTouchFootballer(Vector3 pt, List<GenericFootballer> list) {
		for (int i = 0; i < list.Count; i++) {
			if (list[i].ContainsPointClick(pt)) return list[i];
		}
		return null;
	}

	private void keyboard_switch_timeout_selected_footballer() {
		int tar = -1;
		if (Input.GetKey(KeyCode.Alpha1)) tar = 0;
		if (Input.GetKey(KeyCode.Alpha2)) tar = 1;
		if (Input.GetKey(KeyCode.Alpha3)) tar = 2;
		if (Input.GetKey(KeyCode.Alpha4)) tar = 3;
		if (Input.GetKey(KeyCode.Alpha5)) tar = 4;
		if (tar != -1 && tar < m_playerTeamFootballers.Count) {
			m_timeoutSelectedFootballer = m_playerTeamFootballers[tar];
		}
	}

	public Team get_footballer_team(GenericFootballer tar) {
		if (tar == null) return Team.None;
		if (m_playerTeamFootballers.Contains(tar)) return Team.PlayerTeam;
		if (m_enemyTeamFootballers.Contains(tar)) return Team.EnemyTeam;
		return Team.None;
	}

	public bool footballer_has_ball(GenericFootballer tar) {
		if (get_footballer_team(tar) == Team.PlayerTeam) {
			return m_playerTeamFootballersWithBall.Contains(tar);
		} else {
			return m_enemyTeamFootballersWithBall.Contains(tar);
		}
	}

	public GenericFootballer nullableCurrentFootballerWithBall() {
		if (m_playerTeamFootballersWithBall.Count > 0) return m_playerTeamFootballersWithBall[0];
		if (m_enemyTeamFootballersWithBall.Count > 0) return m_enemyTeamFootballersWithBall[0];
		return null;
	}

	public Vector3 currentBallPosition() {
		GenericFootballer ball_holder = nullableCurrentFootballerWithBall();
		if (ball_holder != null) {
			return ball_holder.transform.position;
		} else {
			if (m_looseBalls.Count > 0) return m_looseBalls[0].transform.position;
			return Vector3.zero;
		}
	}
	
	public Vector3 currentLooseBallVelocity() {
		if (m_looseBalls.Count > 0) {
			return m_looseBalls[0]._vel;
		}
		return Vector3.zero;
	}

	public void set_time_remaining_seconds(int seconds) {
		TimeSpan ticks = new TimeSpan(0,0,0,seconds);
		_time_remaining = ticks.Ticks;
	}

	public string get_time_remaining_formatted() {
		TimeSpan ts = TimeSpan.FromTicks(_time_remaining);
		return  string.Format(@"{0:0}:{1:00}:{2:000}",ts.Minutes,ts.Seconds,ts.Milliseconds);
	}

	public int _player_team_score = 0;
	public int _enemy_team_score = 0;
	public long _time_remaining = 0;
	public string _quarter_display = "1ST";

	private Vector3 _goalzoomoutfocuspos;
	private void enemy_goal_score(Vector3 tar) {
		if (Main._current_level == GameLevel.Level1) {
			Main._current_level = GameLevel.Level2;
		} else if (Main._current_level == GameLevel.Level2) {
			Main._current_level = GameLevel.Level3;
		} else {
			Main._current_level = GameLevel.End;
		}
		_player_team_score++;
		
		m_currentMode = LevelControllerMode.GoalZoomOut;
		Main._current_repeat_reason = RepeatReason.None;
		UiPanelGame.inst.show_popup_message(1);
		_goalzoomoutfocuspos = tar;
	}
	
	private void player_goal_score(Vector3 tar) {
		_enemy_team_score++;
		m_currentMode = LevelControllerMode.GoalZoomOut;
		Main._current_repeat_reason = RepeatReason.ScoredOn;
		UiPanelGame.inst.show_popup_message(1);
		_goalzoomoutfocuspos = tar;
	}

}

public enum Team {
	PlayerTeam,
	EnemyTeam,
	None
}

public enum Difficulty {
	Easy,
	Normal,
	Hard
}
