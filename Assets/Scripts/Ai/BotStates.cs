using UnityEngine;

public static class BotConstants {
	public const float ArriveDistance = 10.0f;
	public const float KeeperFetchDistance = 400.0f;
	public const float DefenderChaseDistance = 500.0f;
	public const float AttackerShootDistance = 400.0f;
}

/**
 * 
 */
public class BotState_TendGoal : FSMState<BotBase> {
	
	static readonly BotState_TendGoal instance = new BotState_TendGoal();
	public static BotState_TendGoal Instance {
		get {
			return instance;
		}
	}
	static BotState_TendGoal() { }
	
	private BotState_TendGoal() { }
	
	public override void Enter (BotBase bot) {
		bot.Steering.ArriveOn();
	}
	
	public override void Execute (BotBase bot) {
		{
			Vector3 targetPos;
			Vector3 topPost, bottomPost;
			bot.GetGoalpostPositions(bot.Team, out topPost, out bottomPost);
			Uzu.Math.ClosestPtPointSegment(
				topPost, bottomPost,
				bot.GetBallPosition(),
				out targetPos
			);
			bot.Steering.CurrentTarget = targetPos;
		}
		
		BotBase ballOwner = bot.GetBallOwner();
		
		// We have the ball - throw it back into play.
		if (ballOwner == bot) {
			bot.ChangeState(BotState_PutBallBackInPlay.Instance);
			return;
		}
		
		// Ball is loose and close to us - go get it.
		float ballDistance = Vector3.Distance(bot.GetBallPosition(), bot.transform.position);
		if (ballOwner == null && ballDistance <= BotConstants.KeeperFetchDistance) {
			bot.ChangeState(BotState_Intercept.Instance);
			return;
		}
	}
	
	public override void Exit(BotBase bot) {
		bot.Steering.ArriveOff();
	}
}

/**
 * 
 */
public class BotState_Intercept : FSMState<BotBase> {
	
	static readonly BotState_Intercept instance = new BotState_Intercept();
	public static BotState_Intercept Instance {
		get {
			return instance;
		}
	}
	static BotState_Intercept() { }
	
	private BotState_Intercept() { }
	
	public override void Enter (BotBase bot) {
		bot.Steering.PursuitOn();
	}
	
	public override void Execute (BotBase bot) {
		if (!bot.IsBallLoose()) {
			bot.ChangeState(BotState_GoHome.Instance);
			return;
		}
		
		if (bot.GetDistanceFromGoal(bot.Team) >= BotConstants.KeeperFetchDistance) {
			Debug.Log("Too far from goal - go home.");
			bot.ChangeState(BotState_GoHome.Instance);
			return;
		}
		
		bot.Steering.CurrentTarget = bot.GetBallPosition();
		bot.Steering.CurrentEvaderVelocity = bot.GetBallVelocity();
	}
	
	public override void Exit(BotBase bot) {
		bot.Steering.PursuitOff();
	}
}

public class BotState_PutBallBackInPlay : FSMState<BotBase> {
	static readonly BotState_PutBallBackInPlay instance = new BotState_PutBallBackInPlay();
	public static BotState_PutBallBackInPlay Instance {
		get {
			return instance;
		}
	}
	static BotState_PutBallBackInPlay() { }
	
	private BotState_PutBallBackInPlay() { }
	
	public override void Enter (BotBase bot) {
		
	}
	
	public override void Execute (BotBase bot) {
		// Throw the ball.
		if (bot.GetBallOwner() == bot) {
			BotBase throwTarget = bot.GetClosestTeammate();
			bot.ThrowBall(throwTarget.transform.position);
		}
		
		bot.ChangeState(BotState_GoHome.Instance);
	}
	
	public override void Exit(BotBase bot) {
		
	}
}

/**
 * 
 */
public class BotState_GoHome : FSMState<BotBase> {
	
	static readonly BotState_GoHome instance = new BotState_GoHome();
	public static BotState_GoHome Instance {
		get {
			return instance;
		}
	}
	static BotState_GoHome() { }
	
	private BotState_GoHome() { }
	
	public override void Enter (BotBase bot) {
		bot.Steering.ArriveOn();
		bot.Steering.CurrentTarget = bot.HomePosition;
	}
	
	public override void Execute (BotBase bot) {
		if (Vector3.Distance(bot.HomePosition, bot.transform.localPosition) <= BotConstants.ArriveDistance) {
			if (bot.FieldPosition == FieldPosition.Keeper) {
				bot.ChangeState(BotState_TendGoal.Instance);
			} else {
				bot.ChangeState(BotState_Idle.Instance);
			}
		}
	}
	
	public override void Exit(BotBase bot) {
		bot.Steering.ArriveOff();
	}
}

/**
 * 
 */
public class BotState_Idle : FSMState<BotBase> {
	static readonly BotState_Idle instance = new BotState_Idle();
	public static BotState_Idle Instance {
		get {
			return instance;
		}
	}
	static BotState_Idle() { }
	
	private BotState_Idle() { }
	
	public override void Enter (BotBase bot) {
		
	}
	
	public override void Execute (BotBase bot) {
		if (bot.FieldPosition == FieldPosition.Defender) {
			if (bot.GetBallTeamOwner() == bot.Team) {
				// If a teammate already has it, do nothing.
			} else {
				if (Vector3.Distance(bot.HomePosition, bot.GetBallPosition()) <= BotConstants.DefenderChaseDistance) {
					bot.ChangeState(BotState_ChaseBall.Instance);
					return;
				}
			}
		} else if (bot.FieldPosition == FieldPosition.Attacker) {
			// Teammate forward has the ball, let's help him.
			BotBase owner = bot.GetBallOwner();
			if (owner != null &&
				  owner.Team == bot.Team &&
					owner != bot &&
					owner.FieldPosition == FieldPosition.Attacker) {
				bot.ChangeState(BotState_Pressure.Instance);
				return;
			}
			
			Vector3 ballPosition = bot.GetBallPosition();
			FieldRegion fieldRegion = bot.TeamBase.GetFieldRegion(ballPosition);
			
			// Get ball.
			if (fieldRegion == FieldRegion.Midfield) {
				// Attempt to stay in bot's vertical half of the field (top/bottom).
				float verticalOffset = Mathf.Abs(bot.HomePosition.y - bot.GetBallPosition().y);
				float checkHeight = Main.FieldController.GetFieldSize().y * 0.333333f;
				if (verticalOffset < checkHeight) {
					bot.ChangeState(BotState_ChaseBall.Instance);
					return;
				}
			}
			// Ball is loose in forward region - get it.
			if (fieldRegion == FieldRegion.Forwardfield) {
				if (bot.GetBallOwner() == null) {
					bot.ChangeState(BotState_ChaseBall.Instance);
					return;
				}
			}
		}
		
		if (bot.FieldPosition != FieldPosition.Keeper) {
			bot.ChangeState(BotState_Roam.Instance);
		}
	}
	
	public override void Exit(BotBase bot) {
		
	}
}

/**
 * 
 */
public class BotState_Roam : FSMState<BotBase> {
	static readonly BotState_Roam instance = new BotState_Roam();
	public static BotState_Roam Instance {
		get {
			return instance;
		}
	}
	static BotState_Roam() { }
	
	private BotState_Roam() { }
	
	private Vector3 GetRoamPosition(BotBase bot) {
		Vector3 dir = Uzu.Math.RandomOnUnitCircle();
		float offset = Random.Range(40.0f, 60.0f);
		return bot.HomePosition + dir * offset;
	}
	
	public override void Enter (BotBase bot) {
		bot.Steering.ArriveOn();
		bot.Steering.CurrentTarget = GetRoamPosition(bot);
		bot.RoamTime = Random.Range(0.5f, 1.0f);
	}
	
	public override void Execute (BotBase bot) {
		if (Vector3.Distance(bot.Steering.CurrentTarget, bot.transform.position) <= BotConstants.ArriveDistance) {
			bot.Steering.ArriveOff();
			
			if (bot.RoamTime <= 0.0f) {
				bot.ChangeState(BotState_Idle.Instance);
			} else {
				bot.RoamTime -= Time.deltaTime;
			}
		}
	}
	
	public override void Exit(BotBase bot) {
		bot.Steering.ArriveOff();
	}
}

/**
 * 
 */
public class BotState_Wait : FSMState<BotBase> {
	static readonly BotState_Wait instance = new BotState_Wait();
	public static BotState_Wait Instance {
		get {
			return instance;
		}
	}
	static BotState_Wait() { }
	
	private BotState_Wait() { }
	
	public override void Enter (BotBase bot) {
		bot.Steering.AllOff();
	}
	
	public override void Execute (BotBase bot) {
		if (bot.IsStunned()) {
			// Waiting..
			return;
		}
		
		// Unstunned.
		bot.RevertState();
	}
	
	public override void Exit(BotBase bot) {
	}
}

/**
 * 
 */
public class BotState_Pressure : FSMState<BotBase> {
	static readonly BotState_Pressure instance = new BotState_Pressure();
	public static BotState_Pressure Instance {
		get {
			return instance;
		}
	}
	static BotState_Pressure() { }
	
	private BotState_Pressure() { }
	
	public override void Enter (BotBase bot) {
		// Somewhere in offensive territory...
		Vector3 pressurePos = (Main.FieldController.GetRegionPosition(0) +
			Main.FieldController.GetRegionPosition(8)) * 0.5f;
		bot.Steering.ArriveOn();
		bot.Steering.CurrentTarget = pressurePos;
	}
	
	public override void Execute (BotBase bot) {
		if (bot.FieldPosition != FieldPosition.Attacker) {
			Debug.LogWarning("Non attacker shouldn't be here...");
			return;
		}
		
		// Other team has the ball.
		if (bot.GetBallTeamOwner() == bot.OtherTeam) {
			bot.ChangeState(BotState_ChaseBall.Instance);
			return;
		}
		
		// Arrived.
		if (Vector3.Distance(bot.Steering.CurrentTarget, bot.transform.position) <= BotConstants.ArriveDistance) {
			bot.ChangeState(BotState_Idle.Instance);
			return;
		}
		
		// If we overshoot ball position, stop.
		if (bot.transform.position.x < bot.GetBallPosition().x) {
			bot.ChangeState(BotState_Idle.Instance);
			return;
		}
	}
	
	public override void Exit(BotBase bot) {
		bot.Steering.ArriveOff();
	}
}

/**
 * 
 */
public class BotState_ChaseBall : FSMState<BotBase> {
	static readonly BotState_ChaseBall instance = new BotState_ChaseBall();
	public static BotState_ChaseBall Instance {
		get {
			return instance;
		}
	}
	static BotState_ChaseBall() { }
	
	private BotState_ChaseBall() { }
	
	public override void Enter (BotBase bot) {
		bot.Steering.PursuitOn();
	}
	
	public override void Execute (BotBase bot) {
		if (bot.FieldPosition == FieldPosition.Defender) {
			// Too far - go home.
			if (Vector3.Distance(bot.HomePosition, bot.GetBallPosition()) > BotConstants.DefenderChaseDistance) {
				bot.ChangeState(BotState_GoHome.Instance);
				return;
			}
			
			// We got the ball!
			if (bot.GetBallOwner() == bot) {
				bot.ChangeState(BotState_Dribble.Instance);
				return;
			}
			
			// Someone else from our team got it - go home.
			if (bot.GetBallTeamOwner() == bot.Team) {
				bot.ChangeState(BotState_GoHome.Instance);
				return;
			}
		} else if (bot.FieldPosition == FieldPosition.Attacker) {
			// We got the ball!
			BotBase ballOwner = bot.GetBallOwner();
			if (ballOwner == bot) {
				bot.ChangeState(BotState_Dribble.Instance);
				return;
			}
			
			Vector3 ballPosition = bot.GetBallPosition();
			FieldRegion fieldRegion = bot.TeamBase.GetFieldRegion(ballPosition);
			// Ignore - defender will get it.
			if (fieldRegion == FieldRegion.Backfield) {
				bot.ChangeState(BotState_Idle.Instance);
				return;
			}
			
			// Ball in defender region and other team has control.
			if (fieldRegion == FieldRegion.Forwardfield) {
				if (ballOwner != null && ballOwner.Team != bot.Team) {
					bot.ChangeState(BotState_GoHome.Instance);
					return;
				}
			}
			
			// Someone else from our team has the ball.
			if (bot.GetBallTeamOwner() == bot.Team) {
				bot.ChangeState(BotState_Pressure.Instance);
				return;
			}
		}
				
		bot.Steering.CurrentTarget = bot.GetBallPosition();
		bot.Steering.CurrentEvaderVelocity = bot.GetBallVelocity();
	}
	
	public override void Exit(BotBase bot) {
		bot.Steering.PursuitOff();
	}
}

/**
 * 
 */
public class BotState_Dribble : FSMState<BotBase> {
	static readonly BotState_Dribble instance = new BotState_Dribble();
	public static BotState_Dribble Instance {
		get {
			return instance;
		}
	}
	static BotState_Dribble() { }
	
	private BotState_Dribble() { }
	
	public override void Enter (BotBase bot) {
		if (bot.FieldPosition == FieldPosition.Defender) {
			bot.DribbleTime = Random.Range(0.5f, 1.0f);
		} else if (bot.FieldPosition == FieldPosition.Attacker) {
			bot.DribbleTime = Random.Range(4.0f, 6.0f);
		}
		
		bot.Steering.SeekOn();
		bot.Steering.CurrentTarget = bot.GetGoalpostPosition(bot.OtherTeam);
	}
	
	public override void Execute (BotBase bot) {
		if (bot.FieldPosition == FieldPosition.Defender) {
			// Lost the ball.
			if (bot.GetBallOwner() != bot) {
				bot.ChangeState(BotState_ChaseBall.Instance);
				return;
			}
			
			bot.DribbleTime -= Time.deltaTime;
			if (bot.DribbleTime <= 0.0f) {
				BotBase throwTarget = bot.GetClosestTeammate();
				bot.ThrowBall(throwTarget.transform.position);
				bot.ChangeState(BotState_GoHome.Instance);
				
				throwTarget.Msg_ReceivePass();
			}
		} else if (bot.FieldPosition == FieldPosition.Attacker) {
			// Lost the ball.
			if (bot.GetBallOwner() != bot) {
				bot.ChangeState(BotState_ChaseBall.Instance);
				return;
			}
			
			{
				float distToGoal = Vector3.Distance(bot.transform.position, bot.GetGoalpostPosition(bot.OtherTeam));
				bot.DribbleTime -= Time.deltaTime;
				if (distToGoal < BotConstants.AttackerShootDistance ||
					  bot.DribbleTime <= 0.0f) {
					// TODO: if lane to goal is open, attempt a shot... else try to pass to other forward if open.. else pass back to defender
					
					bot.ThrowBall(bot.GetGoalpostPosition(bot.OtherTeam));
					bot.ChangeState(BotState_Pressure.Instance);
					return;
				}
			}
		}
	}
	
	public override void Exit(BotBase bot) {
		bot.Steering.SeekOff();
	}
}

/**
 * 
 */
public class BotState_ReceivePass : FSMState<BotBase> {
	static readonly BotState_ReceivePass instance = new BotState_ReceivePass();
	public static BotState_ReceivePass Instance {
		get {
			return instance;
		}
	}
	static BotState_ReceivePass() { }
	
	private BotState_ReceivePass() { }
	
	public override void Enter (BotBase bot) {
		bot.Steering.PursuitOn();
	}

	public override void Execute (BotBase bot) {
		if (bot.FieldPosition == FieldPosition.Defender) {
			if (bot.GetBallOwner() == bot) {
				bot.ChangeState(BotState_Dribble.Instance);
				return;
			}
			
			if (!bot.IsBallLoose()) {
				bot.ChangeState(BotState_GoHome.Instance);
				return;
			}
		} else if (bot.FieldPosition == FieldPosition.Attacker) {
			if (bot.GetBallOwner() == bot) {
				bot.ChangeState(BotState_Dribble.Instance);
				return;
			}
			
			if (!bot.IsBallLoose()) {
				bot.ChangeState(BotState_ChaseBall.Instance);
				return;
			}
		}
			
		bot.Steering.CurrentTarget = bot.GetBallPosition();
		bot.Steering.CurrentEvaderVelocity = bot.GetBallVelocity();
	}
	
	public override void Exit(BotBase bot) {
		bot.Steering.PursuitOff();
	}
}