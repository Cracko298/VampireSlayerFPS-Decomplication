using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;

namespace VampireFPS;

internal class HunterMenuScreen : MenuScreen
{
	private NetworkSession networkSession;

	private Texture2D gradientTexture;

	public HunterMenuScreen(NetworkSession networkSession)
		: base(Resources.ChooseCharacter)
	{
		this.networkSession = networkSession;
		MenuEntry menuEntry = new MenuEntry(Resources.AutoChoose);
		menuEntry.Selected += OnAutoChoose;
		base.MenuEntries.Add(menuEntry);
		MenuEntry menuEntry2 = new MenuEntry(Resources.FatherD);
		menuEntry2.Selected += OnChooseFatherD;
		base.MenuEntries.Add(menuEntry2);
		MenuEntry menuEntry3 = new MenuEntry(Resources.Molly);
		menuEntry3.Selected += OnChooseMolly;
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
		g.m_PlayerManager.GetLocalPlayer().AutoChooseClass();
		if (g.m_PlayerManager.GetLocalPlayer().GetClass() != 0)
		{
			g.m_PlayerManager.GetLocalPlayer().SpawnLocal();
			ExitScreen();
		}
	}

	private void OnChooseFatherD(object sender, PlayerIndexEventArgs e)
	{
		g.m_PlayerManager.GetLocalPlayer().SetClass(Player.CLASS.FatherD);
		if (g.m_PlayerManager.GetLocalPlayer().GetClass() != 0)
		{
			g.m_PlayerManager.GetLocalPlayer().SpawnLocal();
			ExitScreen();
		}
	}

	private void OnChooseMolly(object sender, PlayerIndexEventArgs e)
	{
		g.m_PlayerManager.GetLocalPlayer().SetClass(Player.CLASS.Molly);
		if (g.m_PlayerManager.GetLocalPlayer().GetClass() != 0)
		{
			g.m_PlayerManager.GetLocalPlayer().SpawnLocal();
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
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		SpriteBatch spriteBatch = base.ScreenManager.SpriteBatch;
		_ = base.ScreenManager.Font;
		base.ScreenManager.FadeBackBufferToBlack(base.TransitionAlpha * 2f / 3f);
		Viewport viewport = ((DrawableGameComponent)base.ScreenManager).GraphicsDevice.Viewport;
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector((float)((Viewport)(ref viewport)).Width, (float)((Viewport)(ref viewport)).Height);
		Vector2 val2 = default(Vector2);
		((Vector2)(ref val2))._002Ector(540f, 400f);
		Vector2 val3 = (val - val2) / 2f;
		val3.Y = 30f;
		Rectangle val4 = default(Rectangle);
		((Rectangle)(ref val4))._002Ector((int)val3.X - 32, (int)val3.Y - 16, (int)val2.X + 64, 650);
		Color val5 = Color.White * base.TransitionAlpha;
		spriteBatch.Begin();
		spriteBatch.Draw(gradientTexture, val4, val5);
		Vector2 val6 = default(Vector2);
		((Vector2)(ref val6))._002Ector(430f, 400f);
		spriteBatch.DrawString(g.m_App.smallfont, "SLAYER CONTROLS:", val6, Color.DarkGoldenrod * base.TransitionAlpha);
		val6.Y += 30f;
		spriteBatch.DrawString(g.m_App.smallfont, "A: JUMP", val6, Color.DarkGoldenrod * base.TransitionAlpha);
		val6.Y += 22f;
		spriteBatch.DrawString(g.m_App.smallfont, "B: CROUCH", val6, Color.DarkGoldenrod * base.TransitionAlpha);
		val6.Y += 22f;
		spriteBatch.DrawString(g.m_App.smallfont, "X: RELOAD", val6, Color.DarkGoldenrod * base.TransitionAlpha);
		val6.Y += 22f;
		spriteBatch.DrawString(g.m_App.smallfont, "Y: CHANGE WEAPON", val6, Color.DarkGoldenrod * base.TransitionAlpha);
		val6.Y += 22f;
		spriteBatch.DrawString(g.m_App.smallfont, "RIGHT TRIGGER: FIRE WEAPON", val6, Color.DarkGoldenrod * base.TransitionAlpha);
		val6.Y += 22f;
		spriteBatch.DrawString(g.m_App.smallfont, "LEFT TRIGGER: ZOOM", val6, Color.DarkGoldenrod * base.TransitionAlpha);
		val6.Y += 22f;
		spriteBatch.DrawString(g.m_App.smallfont, "RIGHT BUMPER: FLASHLIGHT", val6, Color.DarkGoldenrod * base.TransitionAlpha);
		val6.Y += 22f;
		spriteBatch.DrawString(g.m_App.smallfont, "BACK: SHOW SCORES", val6, Color.DarkGoldenrod * base.TransitionAlpha);
		spriteBatch.End();
		base.Draw(gameTime);
	}
}
