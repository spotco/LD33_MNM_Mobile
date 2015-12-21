/// <summary>
/// Flags used for pausing.
/// </summary>
public static class PauseFlags
{
	public const int Ai = 1 << 0;
	public const int TimeOut = 1 << 5;
	public const int TalkingHeadStop = 1 << 6;
}

public static class PanelIds {
  //public const string Main = "MainPanel";
  public const string Title = "TitlePanel";
  public const string Game = "GamePanel";
  public const string Tv = "TVPanel";
}

public static class AudioClipIds {
	public const string BGM_FANFARE = "bgm_fanfare";
	public const string BGM_MAIN_LOOP = "bgm_main_loop";
	public const string BGM_MENU_INTRO = "bgm_menu_intro";
	public const string BGM_MENU_LOOP = "bgm_menu_loop";
}