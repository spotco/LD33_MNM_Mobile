using UnityEngine;
using System.Collections;

/**
 * Attach to a game object for simple keyboard controlled movement.
 */
public class DebugController : MonoBehaviour {
	[SerializeField]
	private float _speed = 100.0f;
	
	private void Update() {
		Vector3 moveVec = Vector3.zero;
		if(Input.GetKey(KeyCode.UpArrow)) {
			moveVec += Vector3.up;
		}
		if(Input.GetKey(KeyCode.DownArrow)) {
			moveVec -= Vector3.up;
		}
		if(Input.GetKey(KeyCode.RightArrow)) {
			moveVec += Vector3.right;
		}
		if(Input.GetKey(KeyCode.LeftArrow)) {
			moveVec -= Vector3.right;
		}
		
		if (!Mathf.Approximately(moveVec.magnitude, 0.0f)) {
			Vector3 newPos = transform.localPosition + (moveVec * Time.deltaTime * _speed);
			transform.localPosition = newPos;
		}
	}
}
