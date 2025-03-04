using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace VampireFPS;

internal class OptionsMenuScreen : MenuScreen
{
	private const int NUM_MENU_ITEMS = 14;

	private int VOL_IDX = 8;

	private Texture2D gradientTexture;

	private Desc[] m_Desc;

	private float m_DelayTime;

	private void SetDescValues()
	{
		m_Desc[0].m_Text = Resources.Horizontal;
		m_Desc[0].m_UseYesNo = false;
		m_Desc[0].m_Min = 1;
		m_Desc[0].m_Max = 99;
		m_Desc[0].m_Value = 50;
		m_Desc[1].m_Text = Resources.Vertical;
		m_Desc[1].m_UseYesNo = false;
		m_Desc[1].m_Min = 1;
		m_Desc[1].m_Max = 99;
		m_Desc[1].m_Value = 50;
		m_Desc[2].m_Text = Resources.InvertY;
		m_Desc[2].m_UseYesNo = true;
		m_Desc[2].m_Value = 0;
		m_Desc[3].m_Text = Resources.Vibration;
		m_Desc[3].m_UseYesNo = true;
		m_Desc[3].m_Value = 1;
		m_Desc[4].m_Text = Resources.BotsSP;
		m_Desc[4].m_UseYesNo = false;
		m_Desc[4].m_Min = 0;
		m_Desc[4].m_Max = 5;
		m_Desc[4].m_Value = 5;
		m_Desc[5].m_Text = Resources.BotsMP;
		m_Desc[5].m_UseYesNo = false;
		m_Desc[5].m_Min = 0;
		m_Desc[5].m_Max = 5;
		m_Desc[5].m_Value = 1;
		m_Desc[6].m_Text = Resources.MapTime;
		m_Desc[6].m_UseYesNo = false;
		m_Desc[6].m_Min = 5;
		m_Desc[6].m_Max = 90;
		m_Desc[6].m_Value = 10;
		m_Desc[7].m_Text = Resources.Blood;
		m_Desc[7].m_UseYesNo = true;
		m_Desc[7].m_Value = 1;
		m_Desc[VOL_IDX].m_Text = Resources.Vol;
		m_Desc[VOL_IDX].m_UseYesNo = false;
		m_Desc[VOL_IDX].m_Min = 0;
		m_Desc[VOL_IDX].m_Max = 100;
		m_Desc[VOL_IDX].m_Value = 100;
		m_Desc[9].m_Text = "START MAP";
		m_Desc[9].m_UseYesNo = false;
		m_Desc[9].m_Min = 1;
		m_Desc[9].m_Max = 2;
		m_Desc[9].m_Value = 1;
		m_Desc[10].m_Text = "MAX ONLINE PLAYERS";
		m_Desc[10].m_UseYesNo = false;
		m_Desc[10].m_Min = 2;
		m_Desc[10].m_Max = 6;
		m_Desc[10].m_Value = 6;
		m_Desc[11].m_Text = Resources.Reset;
		m_Desc[11].m_HasValue = false;
		m_Desc[12].m_Text = Resources.Cancel;
		m_Desc[12].m_HasValue = false;
		m_Desc[13].m_Text = Resources.Accept;
		m_Desc[13].m_HasValue = false;
	}

	public OptionsMenuScreen()
		: base(Resources.Options)
	{
		m_Desc = new Desc[14];
		for (int i = 0; i < 14; i++)
		{
			m_Desc[i] = new Desc();
		}
		SetDescValues();
		m_Desc[0].m_Value = (int)(g.m_App.m_OptionsHoriz * 10f);
		m_Desc[1].m_Value = (int)(g.m_App.m_OptionsVert * 10f);
		m_Desc[2].m_Value = (g.m_App.m_OptionsInvertY ? 1 : 0);
		m_Desc[3].m_Value = (g.m_App.m_OptionsVibration ? 1 : 0);
		m_Desc[4].m_Value = g.m_App.m_OptionsBotsSP;
		m_Desc[5].m_Value = g.m_App.m_OptionsBotsMP;
		m_Desc[6].m_Value = g.m_App.m_OptionsMapTime;
		m_Desc[7].m_Value = (g.m_App.m_OptionsBlood ? 1 : 0);
		m_Desc[8].m_Value = (int)(g.m_App.m_OptionsVol * 100f);
		m_Desc[9].m_Value = 1;
		m_Desc[10].m_Value = 6;
		MenuEntry item = new MenuEntry(Resources.Horizontal);
		base.MenuEntries.Add(item);
		MenuEntry item2 = new MenuEntry(Resources.Vertical);
		base.MenuEntries.Add(item2);
		MenuEntry item3 = new MenuEntry(Resources.InvertY);
		base.MenuEntries.Add(item3);
		MenuEntry item4 = new MenuEntry(Resources.Vibration);
		base.MenuEntries.Add(item4);
		MenuEntry item5 = new MenuEntry(Resources.BotsSP);
		base.MenuEntries.Add(item5);
		MenuEntry item6 = new MenuEntry(Resources.BotsMP);
		base.MenuEntries.Add(item6);
		MenuEntry item7 = new MenuEntry(Resources.MapTime);
		base.MenuEntries.Add(item7);
		MenuEntry item8 = new MenuEntry(Resources.Blood);
		base.MenuEntries.Add(item8);
		MenuEntry item9 = new MenuEntry(Resources.Vol);
		base.MenuEntries.Add(item9);
		MenuEntry item10 = new MenuEntry("Start Map");
		base.MenuEntries.Add(item10);
		MenuEntry menuEntry = new MenuEntry(Resources.Reset);
		menuEntry.Selected += OnChooseReset;
		base.MenuEntries.Add(menuEntry);
		MenuEntry menuEntry2 = new MenuEntry(Resources.Cancel);
		menuEntry2.Selected += OnChooseCancel;
		base.MenuEntries.Add(menuEntry2);
		MenuEntry menuEntry3 = new MenuEntry(Resources.Accept);
		menuEntry3.Selected += OnChooseAccept;
		base.MenuEntries.Add(menuEntry3);
	}

	private void OnChooseReset(object sender, PlayerIndexEventArgs e)
	{
		SetDescValues();
	}

	private void OnChooseAccept(object sender, PlayerIndexEventArgs e)
	{
		g.m_App.m_OptionsHoriz = (float)m_Desc[0].m_Value / 10f;
		g.m_App.m_OptionsVert = (float)m_Desc[1].m_Value / 10f;
		g.m_App.m_OptionsInvertY = m_Desc[2].m_Value != 0;
		g.m_App.m_OptionsVibration = m_Desc[3].m_Value != 0;
		g.m_App.m_OptionsBotsSP = m_Desc[4].m_Value;
		g.m_App.m_OptionsBotsMP = m_Desc[5].m_Value;
		g.m_App.m_OptionsMapTime = m_Desc[6].m_Value;
		g.m_App.m_OptionsBlood = m_Desc[7].m_Value != 0;
		g.m_App.m_OptionsVol = (float)m_Desc[8].m_Value / 100f;
		g.m_App.m_Level = m_Desc[9].m_Value;
		g.m_App.m_OptionsMaxPlayers = m_Desc[10].m_Value;
		g.m_LoadSaveManager.SaveOptions();
		SoundEffect.MasterVolume = g.m_App.m_OptionsVol;
		ExitScreen();
	}

	private void OnChooseCancel(object sender, PlayerIndexEventArgs e)
	{
		SoundEffect.MasterVolume = g.m_App.m_OptionsVol;
		ExitScreen();
	}

	public override void LoadContent()
	{
		ContentManager content = ((GameComponent)base.ScreenManager).Game.Content;
		gradientTexture = content.Load<Texture2D>("sprites\\back");
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		for (int i = 0; i < base.MenuEntries.Count; i++)
		{
			if (!m_Desc[i].m_HasValue)
			{
				continue;
			}
			if (m_Desc[i].m_UseYesNo)
			{
				if (m_Desc[i].m_Value == 0)
				{
					base.MenuEntries[i].Text = $"{m_Desc[i].m_Text} : NO";
				}
				else
				{
					base.MenuEntries[i].Text = $"{m_Desc[i].m_Text} : YES";
				}
			}
			else
			{
				base.MenuEntries[i].Text = $"{m_Desc[i].m_Text} : {m_Desc[i].m_Value}";
			}
		}
		base.Update(gameTime, otherScreenHasFocus: false, coveredByOtherScreen: false);
	}

	public override void HandleInput(InputState input)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Invalid comparison between Unknown and I4
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Invalid comparison between Unknown and I4
		base.HandleInput(input);
		KeyboardState val = input.CurrentKeyboardStates[base.ControllingPlayer.Value];
		GamePadState val2 = input.CurrentGamePadStates[base.ControllingPlayer.Value];
		float num = (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds;
		if (num < m_DelayTime)
		{
			return;
		}
		for (int i = 0; i < base.MenuEntries.Count; i++)
		{
			bool flag = base.IsActive && i == selectedEntry;
			if (i == VOL_IDX && flag)
			{
				SoundEffect.MasterVolume = (float)m_Desc[i].m_Value / 100f;
			}
			if (!flag)
			{
				continue;
			}
			if (((KeyboardState)(ref val)).IsKeyDown((Keys)37) && m_Desc[i].m_Value > m_Desc[i].m_Min)
			{
				m_Desc[i].m_Value--;
				m_DelayTime = num + 0.1f;
				g.m_SoundManager.Play(36);
			}
			if (((KeyboardState)(ref val)).IsKeyDown((Keys)39) && m_Desc[i].m_Value < m_Desc[i].m_Max)
			{
				m_Desc[i].m_Value++;
				m_DelayTime = num + 0.1f;
				g.m_SoundManager.Play(35);
			}
			GamePadThumbSticks thumbSticks = ((GamePadState)(ref val2)).ThumbSticks;
			if (!(((GamePadThumbSticks)(ref thumbSticks)).Left.X < -0.5f))
			{
				GamePadDPad dPad = ((GamePadState)(ref val2)).DPad;
				if ((int)((GamePadDPad)(ref dPad)).Left != 1)
				{
					goto IL_01db;
				}
			}
			if (m_Desc[i].m_Value > m_Desc[i].m_Min)
			{
				m_Desc[i].m_Value--;
				m_DelayTime = num + 0.1f;
				g.m_SoundManager.Play(36);
			}
			goto IL_01db;
			IL_01db:
			GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref val2)).ThumbSticks;
			if (!(((GamePadThumbSticks)(ref thumbSticks2)).Left.X > 0.5f))
			{
				GamePadDPad dPad2 = ((GamePadState)(ref val2)).DPad;
				if ((int)((GamePadDPad)(ref dPad2)).Right != 1)
				{
					continue;
				}
			}
			if (m_Desc[i].m_Value < m_Desc[i].m_Max)
			{
				m_Desc[i].m_Value++;
				m_DelayTime = num + 0.1f;
				g.m_SoundManager.Play(35);
			}
		}
	}

	protected override void UpdateMenuEntryLocations()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		Vector2 position = default(Vector2);
		((Vector2)(ref position))._002Ector(0f, 125f);
		for (int i = 0; i < base.MenuEntries.Count; i++)
		{
			MenuEntry menuEntry = base.MenuEntries[i];
			Viewport viewport = ((DrawableGameComponent)base.ScreenManager).GraphicsDevice.Viewport;
			position.X = ((Viewport)(ref viewport)).Width / 2 - menuEntry.GetWidth(this) / 2;
			menuEntry.Position = position;
			position.Y += (float)(menuEntry.GetHeight(this) + 10);
		}
	}

	public override void Draw(GameTime gameTime)
	{
		base.ScreenManager.FadeBackBufferToBlack(base.TransitionAlpha * 2f / 3f);
		base.ScreenManager.Font = g.m_App.smallfont;
		base.Draw(gameTime);
		base.ScreenManager.Font = g.m_App.font;
	}
}
