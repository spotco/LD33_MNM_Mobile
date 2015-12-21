using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Steering : MonoBehaviour {
	[SerializeField]
	private float _maxSpeed = 200.0f;
	[SerializeField]
	private float _maxForce = 100.0f;
	[SerializeField]
	private float _mass = 1.0f;
	
	[System.Flags]
	private enum Mode {
		None = 0x0,
		Seek = 0x1,
		Arrive = 0x2,
		Interpose = 0x4,
		Pursuit = 0x8,
	}
	
	private Mode _currentMode = Mode.None;
	private Vector3 _steeringForce;
	private Vector3 _currentPosition;
	private Vector3 _currentTarget;
	private Vector3 _currentVelocity;
	
	public Vector3 CurrentTarget {
		get { return _currentTarget; }
		set { _currentTarget = value; }
	}
	public Vector3 CurrentVelocity {
		get { return _currentVelocity; }
	}
	public Vector3 CurrentInterposeTarget {
		get; set;
	}
	public float CurrentInterposeDistance {
		get; set;
	}
	public Vector3 CurrentEvaderVelocity {
		get; set;
	}
	
	public void AllOff() {
		_currentMode = Mode.None;
	}
	public void SeekOn() {
		_currentMode |= Mode.Seek;
	}
	public void SeekOff() {
		_currentMode &= ~Mode.Seek;
	}
	public void ArriveOn() {
		_currentMode |= Mode.Arrive;
	}
	public void ArriveOff() {
		_currentMode &= ~Mode.Arrive;
	}
	public void InterposeOn() {
		_currentMode |= Mode.Interpose;
	}
	public void InterposeOff() {
		_currentMode &= ~Mode.Interpose;
	}
	public void PursuitOn() {
		_currentMode |= Mode.Pursuit;
	}
	public void PursuitOff() {
		_currentMode &= ~Mode.Pursuit;
	}
	
	private bool IsOn(Mode mode) {
		return (mode & _currentMode) != 0;
	}
	
	public void DoUpdate() {
		// No steering.
		if (_currentMode == Mode.None) {
			_currentVelocity = Vector3.zero;
			return;
		}
		
		_currentPosition = transform.localPosition;
		
		_steeringForce = Vector3.zero;
		_steeringForce = SumForces();
		_steeringForce = Vector3.ClampMagnitude(_steeringForce, _maxForce);
		
		_currentVelocity += (_steeringForce / _mass) * Time.deltaTime;
		_currentVelocity = Vector3.ClampMagnitude(_currentVelocity, _maxSpeed);
		
		transform.localPosition = _currentPosition + _currentVelocity * Time.deltaTime;
	}
	
	private Vector3 SumForces() {
		Vector3 deltaForce = Vector3.zero;
		
		if (IsOn(Mode.Seek)) {
			deltaForce += Seek(_currentTarget);
			
			if (!AccumulateForce(deltaForce)) {
				return _steeringForce;
			}
		}
		
		if (IsOn(Mode.Arrive)) {
			deltaForce += Arrive(_currentTarget);
			
			if (!AccumulateForce(deltaForce)) {
				return _steeringForce;
			}
		}
		
		if (IsOn(Mode.Interpose)) {
			deltaForce += Interpose(_currentTarget, CurrentInterposeTarget, CurrentInterposeDistance);
			
			if (!AccumulateForce(deltaForce)) {
				return _steeringForce;
			}
		}
		
		if (IsOn(Mode.Pursuit)) {
			deltaForce += Pursuit(_currentTarget);
			
			if (!AccumulateForce(deltaForce)) {
				return _steeringForce;
			}
		}

		return _steeringForce;
	}
	
	private bool AccumulateForce(Vector3 forceToAdd) {
		float magnitudeSoFar = _steeringForce.magnitude;
		float magnitudeRemaining = _maxForce - magnitudeSoFar;

		if (magnitudeRemaining <= 0.0) {
				return false;
		}

		float deltaMagnitude = forceToAdd.magnitude;
		if (deltaMagnitude > magnitudeRemaining) {
				deltaMagnitude = magnitudeRemaining;
		}

		_steeringForce += forceToAdd.normalized * deltaMagnitude;

		return true;
	}
	
	private Vector3 Seek(Vector3 targetPos) {
		return ((targetPos - _currentPosition).normalized * _maxSpeed) - _currentVelocity;
	}
	
	private Vector3 Arrive(Vector3 targetPos) {
		Vector3 toTarget = targetPos - _currentPosition;
		
	  float dist = toTarget.magnitude;
	  if (dist > 0) {
	    const float DECELERATION_TWEAK = 0.6f;
	
	    float speed =  dist / DECELERATION_TWEAK;
	    speed = Mathf.Min(speed, _maxSpeed);
			
	    Vector3 desiredVelocity = toTarget * speed / dist;
	    return desiredVelocity - _currentVelocity;
	  }
	
	  return Vector3.zero;
	}
	
	private Vector3 Interpose(Vector3 targetPos, Vector3 anchorPos, float distFromTarget) {
		Vector3 target = anchorPos + (targetPos - anchorPos).normalized * distFromTarget;
		return Arrive(target);
	}
	
	private Vector3 Pursuit(Vector3 targetPos) {
		Vector3 toEvader = targetPos - _currentPosition;
		float relativeHeading = Vector3.Dot(CurrentEvaderVelocity.normalized, _currentVelocity.normalized);
		
		if (Vector3.Dot(toEvader, _currentVelocity.normalized) > 0.0f &&
				relativeHeading < -0.95f /* ~18 degrees */) {
			return Seek(targetPos);
		}
		
		float lookAheadTime = toEvader.magnitude / (_maxSpeed + CurrentEvaderVelocity.magnitude);
		return Seek(targetPos + CurrentEvaderVelocity * lookAheadTime);
	}
}
