using UnityEngine;

/**
 * Responsible for controlling the AI of a single bot.
 */
[RequireComponent (typeof (Steering))]
public class BotBase : MonoBehaviour {
	private FiniteStateMachine<BotBase> _FSM;
	private Steering _steering;
	
	public FiniteStateMachine<BotBase> DBG_FSM {
		get { return _FSM; }
	}
	
	public TeamBase TeamBase {
		get; set;
	}
	public Team Team {
		get {
			return TeamBase.Team;
		}
	}
	public Team OtherTeam {
		get {
			return Team == Team.PlayerTeam ? Team.EnemyTeam : Team.PlayerTeam;
		}
	}
	
	public FieldPosition FieldPosition {
		get; set;
	}
	
	public Steering Steering {
		get { return _steering; }
	}
	
	public float DribbleTime { get; set; }
	public float RoamTime { get; set; }
	
	public void ChangeState(FSMState<BotBase> e) {
		_FSM.ChangeState(e);		
	}
	public void RevertState() {
		_FSM.ChangeState(_FSM.PreviousState);
	}
	
	public Vector3 HomePosition {
		get; set;
	}
	
	public void Msg_GoHome() {
		ChangeState(BotState_GoHome.Instance);
	}
	
	public bool IsAtHomePosition() {
		return Vector3.Distance(HomePosition, this.transform.localPosition) <= BotConstants.ArriveDistance;
	}
	
	public float GetDistanceFromGoal(Team team) {
		return Vector3.Distance(GetGoalpostPosition(team), this.transform.position);
	}
	
	public Vector3 GetGoalpostPosition(Team team) {
		Vector3 top, bottom;
		GetGoalpostPositions(team, out top, out bottom);
		return (top + bottom) * 0.5f;
	}
	
	public void GetGoalpostPositions(Team team, out Vector3 top, out Vector3 bottom) {
		if (team == Team.PlayerTeam) {
			Main.FieldController.GetLeftGoalLinePositions(out top, out bottom);
		} else {
			Main.FieldController.GetRightGoalLinePositions(out top, out bottom);
		}
	}
	
	public float GetBallDistance() {
		return Vector3.Distance(GetBallPosition(), this.transform.position);
	}
	
	public Vector3 GetBallPosition() {
		return Main.LevelController.currentBallPosition();
	}
	
	public Vector3 GetBallVelocity() {
		return Main.LevelController.currentLooseBallVelocity();
	}
	
	public void ThrowBall(Vector3 targetPos) {
		Vector3 throwDir = targetPos - this.transform.position;
		float throwDist = throwDir.magnitude;
		throwDir /= throwDist;
		this.GetComponent<GenericFootballer>().throw_ball(throwDir, throwDist);
	}
	
	public bool IsBallLoose() {
		return GetBallOwner() == null;
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
	
	public BotBase GetClosestTeammate() {
		var members = TeamBase.GetTeamExcept(this);
		float minDist = float.MaxValue;
		int minIdx = -1;
		for (int i = 0; i < members.Count; i++) {
			float dist = Vector3.Distance(this.transform.position, members[i].transform.position);
			if (dist < minDist) {
				minDist = dist;
				minIdx = i;
			}
		}
		if (minIdx == -1) {
			Debug.Log("No one to pass to!");
			return null;
		}
		return members[minIdx];
	}
	
	public void Msg_ReceivePass() {
		// Stay in position.
		if (FieldPosition == FieldPosition.Keeper) {
			return;
		}
		
		ChangeState(BotState_ReceivePass.Instance);
	}
	
	public void Msg_Stunned() {
		ChangeState(BotState_Wait.Instance);
	}
	
	public void Msg_GotBall() {
		if (FieldPosition == FieldPosition.Defender ||
				FieldPosition == FieldPosition.Attacker) {
			ChangeState(BotState_Dribble.Instance);
		}
	}
	
	public void Msg_LostBall() {
		
	}
	
	public bool IsStunned() {
		return GetComponent<GenericFootballer>()._current_mode == GenericFootballer.GenericFootballerMode.Stunned;
	}
	
	public void Awake() {
		_FSM = new FiniteStateMachine<BotBase>();
		_steering = GetComponent<Steering>();
		_FSM.Configure(this, BotState_Idle.Instance);
	}
 
	public void Update() {
		if (Main.IsPaused(PauseFlags.TimeOut | PauseFlags.Ai)) {
			return;
		}
		// HACK: don't update for player
		if (this.Team == Team.PlayerTeam) {
			return;
		}
		
		_FSM.Update();
		Steering.DoUpdate();
	}
}

public enum FieldPosition {
	Keeper,
	Defender,
	Attacker,
}