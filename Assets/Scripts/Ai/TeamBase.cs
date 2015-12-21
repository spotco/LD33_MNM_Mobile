using UnityEngine;
using System.Collections.Generic;

/**
 * Responsible for controlling members of a team, and dictating overall
 * team strategy (defense, offense, etc).
 */
public class TeamBase : MonoBehaviour {
	private FiniteStateMachine<TeamBase> _FSM;
	
	private List<BotBase> _teamMembers = new List<BotBase>();
	
	public FiniteStateMachine<TeamBase> DBG_FSM {
		get { return _FSM; }
	}
	
	public Team Team {
		get; set;
	}
	public Team OtherTeam {
		get {
			return Team == Team.PlayerTeam ? Team.EnemyTeam : Team.PlayerTeam;
		}
	}
	
	public List<BotBase> TeamMembers {
		get { return _teamMembers; }
	}
	
	public List<BotBase> GetTeamExcept(BotBase exceptBot) {
		List<BotBase> members = new List<BotBase>();
		for (int i = 0; i < _teamMembers.Count; i++) {
			if (exceptBot != _teamMembers[i]) {
				members.Add(_teamMembers[i]);
			}
		}
		return members;
	}
	
	public Team GetBallTeamOwner() {
		BotBase owner = GetBallOwner();
		if (owner != null) {
			return owner.Team;
		}
		return Team.None;
	}
	
	public BotBase GetBallOwner() {
		GenericFootballer footballer = Main.LevelController.nullableCurrentFootballerWithBall();
		if (footballer != null) {
			return footballer.GetComponent<BotBase>();
		}
		return null;
	}
	
	public void SetPlayers(List<BotBase> bots) {
		foreach (BotBase bot in bots) {
			bot.TeamBase = this;
		}
		
		_teamMembers.Clear();
		_teamMembers = bots;
	}
	
	public void StartMatch() {
		ChangeState(TeamState_Defend.Instance);
		
		foreach (BotBase bot in this.TeamMembers) {
			bot.Msg_GoHome();
		}
	}
	
	/*
			Field regions:
			0	3	6 9  12 15
			1 4 7 10 13 16
			2 5 8 11 14 17
	 */
	
	public FieldRegion GetFieldRegion(Vector3 pos) {
		int region = Main.FieldController.GetRegion(pos);
		if (this.Team == Team.EnemyTeam) {
			// TODO: assuming they are on right side
			if (region < 6) {
				return FieldRegion.Forwardfield;
			} else if (region < 12) {
				return FieldRegion.Midfield;
			} else {
				return FieldRegion.Backfield;
			}
		} else {
			Debug.LogWarning("Unhandled.");
			return FieldRegion.None;
		}
	}
	
	public void ChangeState(FSMState<TeamBase> s) {
		_FSM.ChangeState(s);
	}
		
	public void Awake() {
		_FSM = new FiniteStateMachine<TeamBase>();
		_FSM.Configure(this, TeamState_Wait.Instance);
	}
 
	public void Update() {
		if (Main.IsPaused(PauseFlags.TimeOut | PauseFlags.Ai)) {
			return;
		}
		
		_FSM.Update();
	}
}
