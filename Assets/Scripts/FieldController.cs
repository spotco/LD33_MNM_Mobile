using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldController : MonoBehaviour {
	[SerializeField]
	private Transform _leftGoalLineTop;
	[SerializeField]
	private Transform _leftGoalLineBottom;
	[SerializeField]
	private Transform _rightGoalLineTop;
	[SerializeField]
	private Transform _rightGoalLineBottom;
	
	List<Vector3> _regionPositions = new List<Vector3>();
	
	public void GetLeftGoalLinePositions(out Vector3 top, out Vector3 bottom) {
		top = _leftGoalLineTop.position;
		bottom = _leftGoalLineBottom.position;
	}
	
	public void GetRightGoalLinePositions(out Vector3 top, out Vector3 bottom) {
		top = _rightGoalLineTop.position;
		bottom = _rightGoalLineBottom.position;
	}
	
	public Vector3 GetFieldSize() {
		float height = Vector3.Distance(_regionPositions[0], _regionPositions[1]) * 3;
		float width = Vector3.Distance(_regionPositions[0], _regionPositions[3]) * 6;
		return new Vector3(width, height, 0.0f);
	}
	
	public Vector3 GetFieldCenter() {
		return (_regionPositions[7] + _regionPositions[10]) * 0.5f;
	}
	
	public int GetRegion(Vector3 pos) {
		float minDist = float.MaxValue;
		int minIdx = -1;
		for (int i = 0; i < _regionPositions.Count; i++) {
			float dist = Vector3.Distance(_regionPositions[i], pos);
			if (dist < minDist) {
				minDist = dist;
				minIdx = i;
			}
		}
		return minIdx;
	}
	
	public Vector3 GetRegionPosition(int region) {
		if (region >= _regionPositions.Count) {
			Debug.Log("Invalid region: " + region);
			return Vector3.zero;
		}
		
		return _regionPositions[region];
	}
	
	private void Awake() {
		// Grid.
		{
			BoxCollider2D[] regions = this.transform.Find("Grid").GetComponentsInChildren<BoxCollider2D>();
			
			for (int i = 0; i < regions.Length; i++) {
				Vector3 cCenter = regions[i].offset;
				Vector3 worldPos = regions[i].transform.TransformPoint(cCenter);
				_regionPositions.Add(worldPos);
			}
		}
	}
}

public enum FieldRegion {
	Backfield,
	Midfield,
	Forwardfield,
	None,
}