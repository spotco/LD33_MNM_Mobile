using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour {
	
	private Vector3 _currentTargetPos;
	private Vector3 _cachedPosition;
	private Vector3 _offset_dir;
	private float _zoom;
	private float _target_zoom;
	private Camera _cachedCamera;
	private Vector3 _manualOffset;
	private Vector3 _tarManualOffset;
	
	public void SetTarget(Transform target) {
		_currentTargetPos = target.localPosition;
	}
	
	public void SetTargetPos(Vector3 targetPos) {
		_currentTargetPos = targetPos;
	}

	public void SetTargetPositionToCurrent() {
		_currentTargetPos = _cachedPosition;
	}

	public void SetTargetZoom(float val) {
		_target_zoom = val;
	}

	public void SetManualOffset(Vector3 tar) {
		_tarManualOffset = tar;
	}

	public Vector3 GetCurrentPosition() {
		return _cachedPosition + _manualOffset;
	}

	Vector3 _start_pos;
	void Awake() {
		_offset_dir = transform.localPosition.normalized;
		_zoom = _target_zoom = transform.localPosition.magnitude;
		_cachedPosition = Vector3.zero;
		_cachedCamera = this.GetComponent<Camera>();
		_currentTargetPos = this.transform.localPosition;
		_manualOffset = Vector3.zero;
		_start_pos = this.transform.position;
	}

	public void reset() {
		this.transform.position = _start_pos;

		_offset_dir = transform.localPosition.normalized;
		_zoom = _target_zoom = transform.localPosition.magnitude;
		_cachedPosition = Vector3.zero;
		_cachedCamera = this.GetComponent<Camera>();
		_currentTargetPos = this.transform.localPosition;
		_manualOffset = Vector3.zero;
		_start_pos = this.transform.position;
	}
	
	void LateUpdate () {
		// Position interp.
		{
			Vector3 target_move_to_position = new Vector3(
				Util.drpt(_cachedPosition.x,_currentTargetPos.x,1/10.0f),
				Util.drpt(_cachedPosition.y,_currentTargetPos.y,1/10.0f),
				Util.drpt(_cachedPosition.z,_currentTargetPos.z,1/10.0f)
			);
			Vector3 target_move_delta = Util.vec_sub(target_move_to_position,_cachedPosition);
			Vector3 max_move_delta = Util.vec_scale(Util.vec_sub(_currentTargetPos,_cachedPosition).normalized,5.0f);

			Vector3 neuCachedPosition = Util.vec_add(_cachedPosition, new Vector3(
				Mathf.Sign(target_move_delta.x) * Mathf.Min(Mathf.Abs(target_move_delta.x),Mathf.Abs(max_move_delta.x)),
				Mathf.Sign(target_move_delta.y) * Mathf.Min(Mathf.Abs(target_move_delta.y),Mathf.Abs(max_move_delta.y)),
				Mathf.Sign(target_move_delta.z) * Mathf.Min(Mathf.Abs(target_move_delta.z),Mathf.Abs(max_move_delta.z))
			));
			if (Main.LevelController.m_gameBounds.OverlapPoint(new Vector3(neuCachedPosition.x,neuCachedPosition.y))) {
				_cachedPosition = neuCachedPosition;
			}

			_zoom = Util.drpt(_zoom,_target_zoom,1/20.0f);

			_manualOffset = new Vector3(
				Util.drpt(_manualOffset.x,_tarManualOffset.x,1/15.0f),
				Util.drpt(_manualOffset.y,_tarManualOffset.y,1/15.0f),
				Util.drpt(_manualOffset.z,_tarManualOffset.z,1/15.0f)
			);

			transform.localPosition = _cachedPosition + Util.vec_scale(_offset_dir,_zoom) + _manualOffset;
		}
	}
}
