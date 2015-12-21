using UnityEngine;

public class TeamState_Attack : FSMState<TeamBase> {
	
	static readonly TeamState_Attack instance = new TeamState_Attack();
	public static TeamState_Attack Instance {
		get {
			return instance;
		}
	}
	static TeamState_Attack() { }
	
	private TeamState_Attack() { }
	
	public override void Enter (TeamBase team) {
		
	}
	
	public override void Execute (TeamBase team) {
		if (team.GetBallTeamOwner() == team.OtherTeam) {
			team.ChangeState(TeamState_Defend.Instance);
		}
	}
	
	public override void Exit(TeamBase team) {
	}
}

/**
 * 
 */
public class TeamState_Defend : FSMState<TeamBase> {
	
	static readonly TeamState_Defend instance = new TeamState_Defend();
	public static TeamState_Defend Instance {
		get {
			return instance;
		}
	}
	static TeamState_Defend() { }
	
	private TeamState_Defend() { }
	
	public override void Enter (TeamBase team) {
		
	}
	
	public override void Execute (TeamBase team) {
		if (team.GetBallTeamOwner() == team.Team) {
			team.ChangeState(TeamState_Attack.Instance);
		}
	}
	
	public override void Exit(TeamBase team) {
	}
}

/**
 * 
 */
public class TeamState_Wait : FSMState<TeamBase> {
	
	static readonly TeamState_Wait instance = new TeamState_Wait();
	public static TeamState_Wait Instance {
		get {
			return instance;
		}
	}
	static TeamState_Wait() { }
	
	private TeamState_Wait() { }
	
	public override void Enter (TeamBase team) {
		Main.Pause(PauseFlags.Ai);
	}
	
	public override void Execute (TeamBase team) {
	}
	
	public override void Exit(TeamBase team) {
		Main.Unpause(PauseFlags.Ai);
	}
}