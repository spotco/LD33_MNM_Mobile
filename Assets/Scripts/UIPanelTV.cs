using UnityEngine;
using System.Collections;

public class UIPanelTV : Uzu.UiPanel {

	[SerializeField] private GameObject _MNM_logo;
	[SerializeField] private GameObject _zoomexit_target;
	[SerializeField] private GameObject _zoomexit_start;
	[SerializeField] private CameraFade _camera_fade;

	[SerializeField] private BobDirection _skele_talk_left,_skele_talk_right;

	[SerializeField] private ChatManager _chats;

	public override void OnInitialize() {
	}

	public override void OnEnter(Uzu.PanelEnterContext context) {
		Main.AudioController.PlayEffect("crowd");
		Main.AudioController.PlayBgm(AudioClipIds.BGM_MENU_LOOP);
		gameObject.SetActive(true);
		Main.GameCamera.gameObject.SetActive(false);
		Main.Instance._tvCamera.gameObject.SetActive(true);
		_MNM_logo.SetActive(true);
		_current_mode = UIPanelTVMode.Idle;

		Main.Instance._tvCamera.transform.position = _zoomexit_start.transform.position;
		_camera_fade.set_alpha(0);
		_camera_fade.set_target_alpha(0);

		_chats.clear_messages();
		if (Main._current_repeat_reason == RepeatReason.None) {
			if (Main._current_level == GameLevel.Level1) {
				_chats.push_message("Welcome back to Monday Night Monsters!",1);
				_chats.push_message("And do we have a SPOOKY good show for you tonight.",2);
				_chats.push_message("The hometown favorites, the Blues, face off...",1);
				_chats.push_message("..against their division rival, the Reds!",1);
				_chats.push_message("I'm so excited, my BONES are SHAKING!",2);
				_chats.push_message("So for all you first time viewers, let's talk controls.",1);
				_chats.push_message("You directly control the blue ball carrier with your mouse.",1);
				_chats.push_message("With the ball, click and hold to throw a pass.",1);
				_chats.push_message("If you miss, try not to get RATTLED!",2);
				_chats.push_message("Hold space to call a timeout.",1);
				_chats.push_message("When in timeout, click and drag a teammate to give them commands.",1);
				_chats.push_message("You're like a modern day Napolean BONE-apart!",2);
				_chats.push_message("Kickoff's just about to get underway.",1);
				_chats.push_message("Can the Blues score in the first quarter?",1);
				_chats.push_message("They've got five minutes on the clock.",1);
				_chats.push_message("Let's see if they got the GUTS to win, cuz' I sure don't!",2);

			} else if (Main._current_level == GameLevel.Level2) {
				_chats.push_message("It's a close fought game, and we're nearing halftime.",1);
				_chats.push_message("And we're all tied up, with three minutes left.",1);
				_chats.push_message("Better hold on, because this game was BONE to be WILD!",2);
				_chats.push_message("Can either team score before the half?",1);
				_chats.push_message("The players are lining back up on the field.",1);
				_chats.push_message("Time for the Blue team to show a little BACK BONE!",2);
				_chats.push_message("Can they step up to the occasion? I'm DYING to find out!",2);

			} else if (Main._current_level == GameLevel.Level3) {
				_chats.push_message("Who would have guessed? It's down to the final two minutes...",1);
				_chats.push_message("And both teams are in a dead heat.",1);
				_chats.push_message("You've gotta be pulling my leg...OFF!",2);
				_chats.push_message("Can the hometown favorites pull through?",1);
				_chats.push_message("If not, I think it's COFFINS for them!",2);
				_chats.push_message("The Blues just need to score one more to claim the lead.",1);
				_chats.push_message("What kind of formation will they go with the game on the line?",1);
				_chats.push_message("Hmm...are they gonna try the SHORT RIB or the FULL RACK?",2);

			} else {
				_chats.push_message("I can't believe it! The Blues pulled through...",1);
				_chats.push_message("...and won in BREATHTAKING fashon!",2);
				_chats.push_message("It didn't take Sherlock Bones to see that one coming!",2);
				_chats.push_message("So that's it for today, tune next week for more Monday Night Monsters!",1);
				_chats.push_message("And coming up next...",1);
				_chats.push_message("The hit TV Emmy award-winning daytime drama, All My Monsters.",1);
				_chats.push_message("Only on the Monster Network, where YOU are the Monster!",2);
				_chats.push_message("Thanks for playing!",1);
			}

		} else {
			if (Main._current_repeat_reason == RepeatReason.ScoredOn) {
				_chats.push_message("What a shock! The Reds broke away and scored!",1);
				_chats.push_message("Could this be the turning point of the game?",1);
				_chats.push_message("The Blue team sure made some BONEHEADED mistakes!",2);
				_chats.push_message("Like all scoring plays, this one's under review.",1);
				_chats.push_message("Let's watch a replay to figure out exactly what just happened.",1);

			} else {
				_chats.push_message("How dissapointing! Neither team was able to score!",1);
				_chats.push_message("Could this be the turning point of the game?",1);
				_chats.push_message("The Blue team sure made some BONEHEADED mistakes!",2);
				_chats.push_message("Let's watch a replay to figure out exactly what just happened.",1);
			}
		}
	}
	
	public override void OnExit(Uzu.PanelExitContext context) {
		gameObject.SetActive(false);
		Main.GameCamera.gameObject.SetActive(true);
		Main.Instance._tvCamera.gameObject.SetActive(false);
		_MNM_logo.SetActive(false);
	}

	enum UIPanelTVMode {
		Idle,
		ZoomExit
	}
	private UIPanelTVMode _current_mode;
	private float _anim_t;
	
	private float _time_until_next_talk_sound = 0;
	private void Update() {
		if (_current_mode == UIPanelTVMode.Idle) {
			if (Input.GetKey(KeyCode.Space) || (_chats._messages.Count == 0 && _chats._text_scroll.finished() && _chats._ct <= 0)) {
				_current_mode = UIPanelTVMode.ZoomExit;
				_anim_t = 0;
				_camera_fade.set_target_alpha(1);
			}
			_skele_talk_left.set_enabled(false);
			_skele_talk_right.set_enabled(false);
			if (!_chats._text_scroll.finished()) {
				if (_chats._current_id == 1) {
					_skele_talk_left.set_enabled(true);
				} else if (_chats._current_id == 2) {
					_skele_talk_right.set_enabled(true);
				}
				if (_time_until_next_talk_sound <= 0) {
					_time_until_next_talk_sound = 0.25f;
					if (_chats._current_id == 1) {
						Main.AudioController.PlayEffect("speak_0_"+((int)Util.rand_range(0,6)));
					} else {
						Main.AudioController.PlayEffect("speak_1_"+((int)Util.rand_range(0,6)));
					}
				} else {
					_time_until_next_talk_sound -= Time.deltaTime;
				}
				
			}

		} else if (_current_mode == UIPanelTVMode.ZoomExit) {
			_anim_t += Util.dt_scale * 0.05f;
			Main.Instance._tvCamera.transform.position = Vector3.Lerp(_zoomexit_start.transform.position,_zoomexit_target.transform.position,_anim_t);
			if (_anim_t >= 1) {
				if (Main._current_level == GameLevel.Level1) {
					Main.PanelManager.ChangeCurrentPanel(PanelIds.Game);
					Main.LevelController.CurrentDifficulty = Difficulty.Easy;
					Main.LevelController.StartLevel(LevelController.StartMode.Sequence);

				} else if (Main._current_level == GameLevel.Level2) {
					Main.PanelManager.ChangeCurrentPanel(PanelIds.Game);
					Main.LevelController.CurrentDifficulty = Difficulty.Normal;
					Main.LevelController.StartLevel(LevelController.StartMode.Sequence);

				} else if (Main._current_level == GameLevel.Level3) {
					Main.PanelManager.ChangeCurrentPanel(PanelIds.Game);
					Main.LevelController.CurrentDifficulty = Difficulty.Hard;
					Main.LevelController.StartLevel(LevelController.StartMode.Sequence);

				} else {
					Main._current_level = GameLevel.Level1;
					Main.PanelManager.ChangeCurrentPanel(PanelIds.Title);
				}

			}
		}
	}
}