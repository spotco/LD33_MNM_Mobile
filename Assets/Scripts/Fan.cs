using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Fan : MonoBehaviour {
	[SerializeField]
	private Vector2 _jumpRange;
	[SerializeField]
	private Vector2 _jumpTime;
	
	private float _jumpAmount;
	private float _jumpDuration;
	[SerializeField] private Transform _targetTransform;
	
	private void Awake() {
		
		// _jumpAmount = Random.Range(_jumpRange.x, _jumpRange.y);
		
		DoJumpUp();
	}
	
	private void DoJumpUp() {
		_jumpAmount = Random.Range(_jumpRange.x, _jumpRange.y);
		_jumpDuration = Random.Range(_jumpTime.x, _jumpTime.y);
		
		LeanTween.moveZ(
			_targetTransform.gameObject,
			_targetTransform.position.z + _jumpAmount, _jumpDuration)
			.setOnComplete(() => {
			DoJumpDown();
		}).setEase(LeanTweenType.easeOutQuad);
	}
	
	private void DoJumpDown() {
		LeanTween.moveZ(
			_targetTransform.gameObject,
			_targetTransform.position.z - _jumpAmount, _jumpDuration)
			.setOnComplete(() => {
			DoJumpUp();
		}).setEase(LeanTweenType.easeOutQuad);
	}
	
	void Update () {
		// float jump
		// LeanTween.moveY(
		// 	this.gameObject,
		// 	this.transform.position.y + deltaX, time)
		// 		.setOnComplete(() => {
		// }).setEase(LeanTweenType.easeOutQuad);
	}
}