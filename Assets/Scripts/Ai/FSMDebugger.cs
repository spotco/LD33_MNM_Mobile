using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FSMDebugger : MonoBehaviour {
	[SerializeField]
	private TeamBase _team;
	
	[SerializeField]
	private Text _teamText;
	[SerializeField]
	private Text[] _botTexts;
	
	public TeamBase Team {
		get { return _team; }
		set { _team = value; }
	}
	
	private void Update() {
		if (_team == null) {
			_teamText.text = "-";
			for (int i = 0; i < _botTexts.Length; i++) {
				_botTexts[i].text = "-";
			}
			return;
		}
		
		var fsm = _team.DBG_FSM;
		_teamText.text = fsm.CurrentState.Name;
		var members = _team.TeamMembers;
		for (int i = 0; i < members.Count; i++) {
			 _botTexts[i].text = members[i].DBG_FSM.CurrentState.Name;
		}
	}
}
