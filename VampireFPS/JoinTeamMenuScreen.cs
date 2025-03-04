using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;

namespace VampireFPS;

internal class JoinTeamMenuScreen : MenuScreen
{
	private NetworkSession networkSession;

	private Texture2D gradientTexture;

	public JoinTeamMenuScreen(NetworkSession networkSession)
		: base(Resources.JoinTeam)
	{
		this.networkSession = networkSession;
		MenuEntry menuEntry = new MenuEntry(Resources.AutoChoose);
		menuEntry.Selected += OnAutoChoose;
		base.MenuEntries.Add(menuEntry);
		MenuEntry menuEntry2 = new MenuEntry(Resources.HunterTeam);
		menuEntry2.Selected += OnHunterTeam;
		base.MenuEntries.Add(menuEntry2);
		MenuEntry menuEntry3 = new MenuEntry(Resources.VampireTeam);
		menuEntry3.Selected += OnVampireTeam;
		base.MenuEntries.Add(menuEntry3);
		if (networkSession == null)
		{
			MenuEntry menuEntry4 = new MenuEntry(Resources.QuitGame);
			menuEntry4.Selected += QuitGameMenuEntrySelected;
			base.MenuEntries.Add(menuEntry4);
		}
		else
		{
			MenuEntry menuEntry5 = new MenuEntry(networkSession.IsHost ? Resources.EndSession : Resources.LeaveSession);
			menuEntry5.Selected += LeaveSessionMenuEntrySelected;
			base.MenuEntries.Add(menuEntry5);
		}
	}

	protected override void OnCancel(PlayerIndex playerIndex)
	{
	}

	private void OnAutoChoose(object sender, PlayerIndexEventArgs e)
	{
		g.m_PlayerManager.GetLocalPlayer().AutoChooseTeam();
		if (g.m_PlayerManager.GetLocalPlayer().m_Team != 0)
		{
			g.m_PlayerManager.GetLocalPlayer().SetState(Player.STATE.ChooseCharacter);
			GameScreen[] screens = g.m_App.screenManager.GetScreens();
			if (g.m_PlayerManager.GetLocalPlayer().GetTeam() == Player.TEAM.Hunter)
			{
				g.m_App.screenManager.AddScreen(new HunterMenuScreen(g.m_App.m_NetworkSession), screens[0].ControllingPlayer);
			}
			else
			{
				g.m_App.screenManager.AddScreen(new VampireMenuScreen(g.m_App.m_NetworkSession), screens[0].ControllingPlayer);
			}
			ExitScreen();
		}
	}

	private void OnVampireTeam(object sender, PlayerIndexEventArgs e)
	{
		g.m_PlayerManager.GetLocalPlayer().SetTeam(Player.TEAM.Vampire);
		if (g.m_PlayerManager.GetLocalPlayer().m_Team != 0)
		{
			g.m_PlayerManager.GetLocalPlayer().SetState(Player.STATE.ChooseCharacter);
			GameScreen[] screens = g.m_App.screenManager.GetScreens();
			g.m_App.screenManager.AddScreen(new VampireMenuScreen(g.m_App.m_NetworkSession), screens[0].ControllingPlayer);
			ExitScreen();
		}
	}

	private void OnHunterTeam(object sender, PlayerIndexEventArgs e)
	{
		g.m_PlayerManager.GetLocalPlayer().SetTeam(Player.TEAM.Hunter);
		if (g.m_PlayerManager.GetLocalPlayer().m_Team != 0)
		{
			g.m_PlayerManager.GetLocalPlayer().SetState(Player.STATE.ChooseCharacter);
			GameScreen[] screens = g.m_App.screenManager.GetScreens();
			g.m_App.screenManager.AddScreen(new HunterMenuScreen(g.m_App.m_NetworkSession), screens[0].ControllingPlayer);
			ExitScreen();
		}
	}

	private void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		MessageBoxScreen messageBoxScreen = new MessageBoxScreen(Resources.ConfirmQuitGame);
		messageBoxScreen.Accepted += ConfirmQuitMessageBoxAccepted;
		base.ScreenManager.AddScreen(messageBoxScreen, base.ControllingPlayer);
	}

	private void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
	{
		LoadingScreen.Load(base.ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen());
	}

	private void LeaveSessionMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		NetworkSessionComponent.LeaveSession(base.ScreenManager, e.PlayerIndex);
	}

	public override void LoadContent()
	{
		ContentManager content = ((GameComponent)base.ScreenManager).Game.Content;
		gradientTexture = content.Load<Texture2D>("sprites\\back");
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		SpriteBatch spriteBatch = base.ScreenManager.SpriteBatch;
		_ = base.ScreenManager.Font;
		base.ScreenManager.FadeBackBufferToBlack(base.TransitionAlpha * 2f / 3f);
		Viewport viewport = ((DrawableGameComponent)base.ScreenManager).GraphicsDevice.Viewport;
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector((float)((Viewport)(ref viewport)).Width, (float)((Viewport)(ref viewport)).Height);
		Vector2 val2 = default(Vector2);
		((Vector2)(ref val2))._002Ector(400f, 400f);
		Vector2 val3 = (val - val2) / 2f;
		val3.Y = 30f;
		Rectangle val4 = default(Rectangle);
		((Rectangle)(ref val4))._002Ector((int)val3.X - 32, (int)val3.Y - 16, (int)val2.X + 64, (int)val2.Y + 32);
		Color val5 = Color.White * base.TransitionAlpha;
		spriteBatch.Begin();
		spriteBatch.Draw(gradientTexture, val4, val5);
		spriteBatch.End();
		base.Draw(gameTime);
	}
}
