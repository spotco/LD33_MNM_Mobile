using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
	public void Shake(float intensity, float decay)
	{
		// Only store the original rotation if we are not currently shaking.
		if (_shakeIntensity <= 0.0f) {
			_originalRotation = transform.localRotation;
		}

		_shakeIntensity = intensity;
		_shakeDecay = decay;
		_sleep = false;
	}

	private float _shakeDecay;
	private float _shakeIntensity;

	private Vector3 _shakeVector;
	private Quaternion _originalRotation;
	private bool _sleep = true;

	private void Update ()
	{
		if (_sleep) {
			return;
		}

		Vector3 originalPosition = transform.localPosition - _shakeVector;

		if (_shakeIntensity > 0.0f) {
			_shakeVector = Random.insideUnitSphere * _shakeIntensity;

			transform.localPosition = originalPosition + _shakeVector;
			transform.localRotation = new Quaternion(
				_originalRotation.x + Random.Range (-_shakeIntensity, _shakeIntensity) * 0.2f,
				_originalRotation.y + Random.Range (-_shakeIntensity, _shakeIntensity) * 0.2f,
				_originalRotation.z + Random.Range (-_shakeIntensity, _shakeIntensity) * 0.2f,
				_originalRotation.w + Random.Range (-_shakeIntensity, _shakeIntensity) * 0.2f);

			_shakeIntensity -= _shakeDecay * Time.deltaTime;
		}
		else {
			transform.localPosition = originalPosition;
			transform.localRotation = _originalRotation;
			_shakeVector = Vector3.zero;
			_sleep = true;
		}
	}
}