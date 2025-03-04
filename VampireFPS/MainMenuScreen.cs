using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;

namespace VampireFPS;

internal class MainMenuScreen : MenuScreen
{
	private MenuEntry buyMenuEntry;

	public MainMenuScreen()
		: base(Resources.MainMenu)
	{
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		MenuEntry menuEntry = new MenuEntry(Resources.SinglePlayer);
		MenuEntry menuEntry2 = new MenuEntry(Resources.PlayerMatch);
		MenuEntry menuEntry3 = new MenuEntry(Resources.SystemLink);
		MenuEntry menuEntry4 = new MenuEntry(Resources.HelpOptions);
		MenuEntry menuEntry5 = new MenuEntry(Resources.Exit);
		if (g.m_App.IsTrialMode())
		{
			buyMenuEntry = new MenuEntry(Resources.Buy);
			buyMenuEntry.Selected += BuyMenuEntrySelected;
		}
		menuEntry.Selected += SinglePlayerMenuEntrySelected;
		menuEntry2.Selected += LiveMenuEntrySelected;
		menuEntry3.Selected += SystemLinkMenuEntrySelected;
		menuEntry5.Selected += base.OnCancel;
		menuEntry4.Selected += HelpAndOptionsMenuEntrySelected;
		base.MenuEntries.Add(menuEntry2);
		base.MenuEntries.Add(menuEntry);
		base.MenuEntries.Add(menuEntry3);
		base.MenuEntries.Add(menuEntry4);
		if (g.m_App.IsTrialMode())
		{
			base.MenuEntries.Add(buyMenuEntry);
		}
		base.MenuEntries.Add(menuEntry5);
		if (Gamer.SignedInGamers[g.m_App.m_PlayerOnePadId] != null)
		{
			Gamer.SignedInGamers[g.m_App.m_PlayerOnePadId].Presence.PresenceMode = (GamerPresenceMode)46;
		}
		ResetGame();
	}

	private void ResetGame()
	{
		g.m_BotPathManager.DeleteAll();
		g.m_CameraManager.Init();
		g.m_ItemManager.DeleteAll();
		g.m_PlayerManager.DeleteAll();
		g.m_App.Reset();
		GC.Collect();
	}

	private void SinglePlayerMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		ResetGame();
		g.m_PlayerManager.Create(255, bot: false);
		LoadingScreen.Load(base.ScreenManager, true, e.PlayerIndex, new GameplayScreen(null));
	}

	private void LiveMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		ResetGame();
		CreateOrFindSession((NetworkSessionType)2, e.PlayerIndex);
	}

	private void SystemLinkMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		ResetGame();
		CreateOrFindSession((NetworkSessionType)1, e.PlayerIndex);
	}

	private void CreateOrFindSession(NetworkSessionType sessionType, PlayerIndex playerIndex)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		ProfileSignInScreen profileSignInScreen = new ProfileSignInScreen(sessionType);
		profileSignInScreen.ProfileSignedIn += delegate
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			GameScreen screen = new CreateOrFindSessionScreen(sessionType);
			base.ScreenManager.AddScreen(screen, playerIndex);
		};
		base.ScreenManager.AddScreen(profileSignInScreen, playerIndex);
	}

	protected override void OnCancel(PlayerIndex playerIndex)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		MessageBoxScreen messageBoxScreen = new MessageBoxScreen(Resources.ConfirmExitSample);
		messageBoxScreen.Accepted += ConfirmExitMessageBoxAccepted;
		base.ScreenManager.AddScreen(messageBoxScreen, playerIndex);
	}

	private void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
	{
		((GameComponent)base.ScreenManager).Game.Exit();
	}

	private void HelpAndOptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		GameScreen screen = new HelpAndOptionsMenuScreen();
		base.ScreenManager.AddScreen(screen, e.PlayerIndex);
	}

	private void BuyMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (!Guide.IsVisible)
		{
			if (g.m_App.CanPurchaseContent(g.m_App.m_PlayerOnePadId))
			{
				Guide.ShowMarketplace(g.m_App.m_PlayerOnePadId);
			}
			else if (Gamer.SignedInGamers[g.m_App.m_PlayerOnePadId] != null)
			{
				g.m_App.m_ShowPermissionTime = (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds + 3f;
			}
			else
			{
				Guide.ShowSignIn(1, true);
			}
		}
	}

	public override void LoadContent()
	{
		MediaPlayer.Play(g.m_App.m_MenuMusic);
		MediaPlayer.IsRepeating = true;
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		if (!g.m_App.IsTrialMode() && buyMenuEntry != null)
		{
			base.MenuEntries.Remove(buyMenuEntry);
		}
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		SpriteBatch spriteBatch = base.ScreenManager.SpriteBatch;
		SpriteFont font = base.ScreenManager.Font;
		if ((double)g.m_App.m_ShowPermissionTime > gameTime.TotalGameTime.TotalSeconds)
		{
			spriteBatch.Begin();
			spriteBatch.DrawString(font, "No permissions to BUY on this Profile", new Vector2(200f, 600f), Color.Red);
			spriteBatch.End();
		}
		base.Draw(gameTime);
	}
}
