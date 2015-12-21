using UnityEngine;
using System.Collections;
using System;

public class MNMTouchControlManager : TouchEventDelegate {

	private bool _pause_button_pressed;
	private Plane _ground_plane;
	private Camera _game_camera;
	
	public Vector3 _last_world_touch_point;
	
	public void i_initialize() {
		_pause_button_pressed = false;
		_ground_plane = new Plane(new Vector3(0,0,-1), new Vector3(0,0,0));
		_game_camera = Main.GameCamera.GetComponent<Camera>();
		
		_last_world_touch_point = new Vector3();
	}
	
	private Vector3 spos_to_world_pos(Vector2 spos) {
		float enter;
		Ray ray = _game_camera.ScreenPointToRay(spos);
		_ground_plane.Raycast(ray, out enter);
		return ray.GetPoint(enter);
	}
	
	public Vector2 _touch_start;
	public Vector2 _scroll_avg_vel;
	public Vector2 _scroll_frame_vel;
	public Vector2 _last_frame_touch_spos;
	public bool _is_touch_down;
	public bool _has_touch_activated_drag;
	public bool _has_touch_activated_swipe;
	public bool _has_activated_tap;
	public float _touch_hold_ct;
	public bool _this_touch_is_double_tap;
	public DateTime _last_touch_end_time = new DateTime(0);
	
	public bool _this_frame_touch_begin;
	public bool _this_frame_touch_ended;
	
	public void TouchBeginWithScreenPosition(Vector2 spos) {
		_last_world_touch_point = this.spos_to_world_pos(spos);
		
		_touch_start = spos;
		_last_frame_touch_spos = spos;
		_scroll_avg_vel = new Vector2();
		_scroll_frame_vel = new Vector2();
		_is_touch_down = true;
		_has_touch_activated_drag = false;
		_has_touch_activated_swipe = false;
		_touch_hold_ct = 0;
		_has_activated_tap = false;
		_this_frame_touch_begin = true;
		
		_this_touch_is_double_tap = DateTime.Now.Subtract(_last_touch_end_time).TotalSeconds < 0.1f;
		
	}
	public void TouchHoldWithScreenPosition(Vector2 spos) {
		_last_world_touch_point = this.spos_to_world_pos(spos);
		_touch_hold_ct += CurveAnimUtil.GetDeltaTimeScale();
		
		_scroll_frame_vel = spos - _last_frame_touch_spos;
		_scroll_avg_vel.x = SPUtil.RunningAverage(_scroll_avg_vel.x,_scroll_frame_vel.x,5.0f);
		_scroll_avg_vel.y = SPUtil.RunningAverage(_scroll_avg_vel.y,_scroll_frame_vel.y,5.0f);
		
		if (!_has_touch_activated_drag && Vector2.Distance(_touch_start,spos) > 30) {
			_has_touch_activated_drag = true;
		}
		if (!_has_touch_activated_swipe && Vector2.Distance(_touch_start,spos) > 50 && _scroll_avg_vel.magnitude > 10 && _touch_hold_ct > 4.0f) {
			_has_touch_activated_swipe = true;
		}
		
		_last_frame_touch_spos = spos;
	}
	public void TouchEnd() {
		_is_touch_down = false;
		if (!_has_touch_activated_drag) {
			_has_activated_tap = true;
		}
		_this_frame_touch_ended = true;
		_last_touch_end_time = DateTime.Now;
	}
	public void notify_pause_button_toggled() {
		_pause_button_pressed = true;
	}
	public bool get_and_clear_pause_button_pressed() {
		bool rtv = _pause_button_pressed;
		_pause_button_pressed = false;
		return rtv;
	}
	
	public Vector3 get_last_touch_world_position() {
		return _last_world_touch_point;
	}
	
	public void i_update() {
		if (!_is_touch_down) {
			float mag = _scroll_avg_vel.magnitude;
			_scroll_avg_vel = _scroll_avg_vel.normalized;
			_scroll_avg_vel.Scale(SPUtil.VectorOfValue(CurveAnimUtil.ApplyFrictionTick(mag,0,1/10.0f)));
		}
		_has_activated_tap = false;
		_this_frame_touch_begin = false;
		_this_frame_touch_ended = false;
	}
	
	public int GetID() { return 0; }
	
	
}
