using UnityEngine;
using System.Collections.Generic;

public struct LoadedCommentary {
	public string _text;
	public int _announcer;
	public LoadedCommentary(string text, int announcer) {
		_text = text;
		_announcer = announcer;
	}
}
public enum CommentaryEvent {
	Idle,
	PassComplete,
	Fumbled,
	Interception,
	Block,
	ShotOnGoal,
	OutOfBounds
}
public class InGameCommentaryManager {

	private static List<LoadedCommentary> _idle_commentaries = new List<LoadedCommentary>() {
		new LoadedCommentary("They need a good play here.",1),
		new LoadedCommentary("If they don't put up points I don't see how they can win.",2),
		new LoadedCommentary("Are they going to run or pass?",1),
		new LoadedCommentary("They'll score if they can get it in the goal.",2),
		new LoadedCommentary("Some yards is better than none yards.",2),
		new LoadedCommentary("Real doozy of a game here...",2),
		new LoadedCommentary("This game's making me BONE TIRED...",2),
		new LoadedCommentary("When are they gonna make their move?",1)
	};

	private static List<LoadedCommentary> _pass_complete_commentaries = new List<LoadedCommentary>() {
		new LoadedCommentary("Nice pass completion by the Blues!",1),
		new LoadedCommentary("The Blues are moving the BONE forward.",2),
		new LoadedCommentary("He SPINE-D that one from across the field!",2),
		new LoadedCommentary("What a perfect spiral, right down the field.",1),
		new LoadedCommentary("They're really racking up the yards on this drive.",1),
		new LoadedCommentary("I think they call that play Spider 2 Y Bananna.",2),
		new LoadedCommentary("The forward's making a go at it!",1),
		new LoadedCommentary("That's some good play calling there.",1),
		new LoadedCommentary("Quick, short passes down the field. I like it!",1),
		new LoadedCommentary("He's 10 of 12 on the day.",1),
		new LoadedCommentary("They're taking the long bomb!",2)
	};

	private static List<LoadedCommentary> _fumbled_commentaries = new List<LoadedCommentary>() {
		new LoadedCommentary("And they jarred the ball loose!",1),
		new LoadedCommentary("BONE breaking hit right there!",2),
		new LoadedCommentary("And he can't hold on to the ball!",1),
		new LoadedCommentary("Fumble!",1),
		new LoadedCommentary("That guy's got a real BONE to pick!",2),
		new LoadedCommentary("Loose ball!",1)
	};

	private static List<LoadedCommentary> _interception_commentaries = new List<LoadedCommentary>() {
		new LoadedCommentary("That pick really hit my FUNNYBONE here!",2),
		new LoadedCommentary("Intercepted! What a terrible mistake!",1),
		new LoadedCommentary("This team just got picked CLEAN!",2),
		new LoadedCommentary("They're lacking a little coordination here...",1),
		new LoadedCommentary("What a BONEHEADED turnover!",1),
		new LoadedCommentary("Oh no! Who was he even aiming to?",1),
		new LoadedCommentary("I'm glad I've got no nose, cuz that shot STANK!",2),
		new LoadedCommentary("At least they GRAVE it their all!",2)
	};

	private static List<LoadedCommentary> _block_commentaries = new List<LoadedCommentary>() {
		new LoadedCommentary("Solid blocking there!",1),
		new LoadedCommentary("Ouch! Hard block in the back.",1),
		new LoadedCommentary("Lucky they didn't call holding on that one.",1),
		new LoadedCommentary("Nice block! I could feel that one in my bones.",2),
		new LoadedCommentary("He got a little RATTLED up there.",2),
		new LoadedCommentary("They don't call this a contact sport for no reason!",1),
		new LoadedCommentary("It's monsters blocking monsters out there!",1),
		new LoadedCommentary("What a MONSTER block they just made out there!",2),
		new LoadedCommentary("That hit went a little low.",1),
		new LoadedCommentary("Should have called a penalty on that one.",1),
		new LoadedCommentary("Now that's some good tackling form.",1),
		new LoadedCommentary("No wonder they call them the Legion of Boom!",2)
	};

	private static List<LoadedCommentary> _out_of_bounds_commentaries = new List<LoadedCommentary>() {
		new LoadedCommentary("And the ball gets knocked out of bounds!",1),
		new LoadedCommentary("And that one's gonna be taken out!",1),
		new LoadedCommentary("Out of bounds! Time for the onside kick?",1),
		new LoadedCommentary("Now this is what I call refball!",2),
		new LoadedCommentary("Anyone remember deflategate?",2),
		new LoadedCommentary("And he gives this one to the refs!",1)
	};

	private static List<LoadedCommentary> _shot_on_goal_commentaries = new List<LoadedCommentary>() {
		new LoadedCommentary("And he takes a shot on goal!",1),
		new LoadedCommentary("He cuts inside, and shoots!",1),
		new LoadedCommentary("Good team play, and they take the shot!",1),
		new LoadedCommentary("Another solid shot on goal!",1)
	};

	private static LoadedCommentary random_commentary_for_event_type(CommentaryEvent type) {
		List<LoadedCommentary> use;
		if (type == CommentaryEvent.Idle) {
			use = _idle_commentaries;
		} else if (type == CommentaryEvent.Fumbled) {
			use = _fumbled_commentaries;
		} else if (type == CommentaryEvent.Interception) {
			use = _interception_commentaries;
		} else if (type == CommentaryEvent.PassComplete) {
			use = _pass_complete_commentaries;
		} else if (type == CommentaryEvent.Block) {
			use = _block_commentaries;
		} else if (type == CommentaryEvent.OutOfBounds) {
			use = _out_of_bounds_commentaries;
		} else {
			use = _shot_on_goal_commentaries;
		}
		return use[(int)Util.rand_range(0,use.Count)];
	}

	private bool _do_tutorial = false;
	private bool _tutorial_has_issued_command = false;
	private bool _tutorial_has_passed_ball = false;
	public void i_initialize() {
		_time_since_last_important = 100;
	}

	public void i_update() {
		if (_do_tutorial) {
			if (UiPanelGame.inst.can_take_message() && Main.LevelController.m_currentMode != LevelController.LevelControllerMode.Opening) {
				if (!_tutorial_has_issued_command) {
					if (Main.LevelController.m_currentMode != LevelController.LevelControllerMode.Timeout) {
						UiPanelGame.inst._chats.push_message("Hold space to call timeout!",1);

					} else {
						UiPanelGame.inst._chats.push_message("Click and drag teammates in timeout to give commands!",1);
					}

				} else if (!_tutorial_has_passed_ball) {
					if (Main.LevelController.get_footballer_team(Main.LevelController.nullableCurrentFootballerWithBall()) == Team.PlayerTeam) {
						UiPanelGame.inst._chats.push_message("Click and hold to throw a pass.",1);
					}
					
				} else {
					if (Main.LevelController.m_currentMode == LevelController.LevelControllerMode.GamePlay) {
						_do_tutorial = false;
					}
				}
			}
		} else {
			this.i_update_commentary();
		}
	}

	private CommentaryEvent _last_event;
	private float _time_since_last_commentary = 0;
	private float _time_since_last_important = 0;
	private float _time_since_last_cheer = 0;
	private void i_update_commentary() {
		_time_since_last_commentary += 1;
		_time_since_last_important += 1;
		_time_since_last_cheer += 1;
		if (UiPanelGame.inst.can_take_message()) {
			if (_last_event == CommentaryEvent.Idle) {
				if (_time_since_last_commentary > 850) {
					LoadedCommentary tar = random_commentary_for_event_type(_last_event);
					UiPanelGame.inst._chats.push_message(tar._text,tar._announcer);
					_time_since_last_commentary = 0;
					_last_event = CommentaryEvent.Idle;
				}
			} else if (_time_since_last_commentary > 200) {
				LoadedCommentary tar = random_commentary_for_event_type(_last_event);
				UiPanelGame.inst._chats.push_message(tar._text,tar._announcer);
				_time_since_last_commentary = 0;	
				_last_event = CommentaryEvent.Idle;
			}
		}
	}

	public void notify_event(CommentaryEvent type, bool important = false) {
		if (_last_event == CommentaryEvent.ShotOnGoal) return;
		_last_event = type;
		if (important && _time_since_last_important > 100) {
			UiPanelGame.inst._chats.clear_messages();
			if (_time_since_last_cheer > 500) {
				Main.AudioController.PlayEffect("crowd");
				_time_since_last_cheer = 0;
			}
			_time_since_last_important = 0;
			_time_since_last_commentary = 0;
			LoadedCommentary tar = random_commentary_for_event_type(_last_event);
			UiPanelGame.inst._chats.push_message(tar._text,tar._announcer);
		}
	}

	public void notify_do_tutorial() {
		_do_tutorial = true;
	}
	public void notify_tutorial_command_issued() {
		_tutorial_has_issued_command = true;
	}
	public void notify_tutorial_pass_thrown() {
		if (_tutorial_has_issued_command) {
			_tutorial_has_passed_ball = true;
		}
	}
	public void notify_tutorial_just_pressed_space() {
		if (!_tutorial_has_issued_command) {
			UiPanelGame.inst._chats.clear_messages();
		}
	}
}
