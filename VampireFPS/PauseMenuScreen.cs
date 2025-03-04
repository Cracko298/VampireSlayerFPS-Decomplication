using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;

namespace VampireFPS;

internal class PauseMenuScreen : MenuScreen
{
	private NetworkSession networkSession;

	public PauseMenuScreen(NetworkSession networkSession)
		: base(Resources.Paused)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Invalid comparison between Unknown and I4
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		this.networkSession = networkSession;
		g.m_LoadSaveManager.SaveGame();
		if (Gamer.SignedInGamers[g.m_App.m_PlayerOnePadId] != null && (int)g.m_App.m_PlayerOnePadId != -1)
		{
			GamePad.SetVibration(g.m_App.m_PlayerOnePadId, 0f, 0f);
		}
		MenuEntry menuEntry = new MenuEntry(Resources.ResumeGame);
		menuEntry.Selected += base.OnCancel;
		base.MenuEntries.Add(menuEntry);
		if (g.m_PlayerManager.GetLocalPlayer().m_Health > 0 && !g.m_PlayerManager.GetLocalPlayer().m_bRagdoll && g.m_PlayerManager.GetLocalPlayer().m_ChangeTeamTime < (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds)
		{
			MenuEntry menuEntry2 = new MenuEntry("CHANGE TEAM");
			menuEntry2.Selected += ChangeTeamSessionMenuEntrySelected;
			base.MenuEntries.Add(menuEntry2);
		}
		if (networkSession == null)
		{
			MenuEntry menuEntry3 = new MenuEntry(Resources.QuitGame);
			menuEntry3.Selected += QuitGameMenuEntrySelected;
			base.MenuEntries.Add(menuEntry3);
		}
		else
		{
			MenuEntry menuEntry4 = new MenuEntry(networkSession.IsHost ? Resources.EndSession : Resources.LeaveSession);
			menuEntry4.Selected += LeaveSessionMenuEntrySelected;
			base.MenuEntries.Add(menuEntry4);
		}
	}

	private void ChangeTeamSessionMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		if (g.m_PlayerManager.GetLocalPlayer().m_Health > 0 && !g.m_PlayerManager.GetLocalPlayer().m_bRagdoll)
		{
			g.m_PlayerManager.GetLocalPlayer().CleanItems();
			if (networkSession != null)
			{
				g.m_PlayerManager.GetLocalPlayer().m_RequestCleanItems = true;
			}
			g.m_PlayerManager.GetLocalPlayer().m_State = Player.STATE.JoinTeam;
			g.m_PlayerManager.GetLocalPlayer().m_ChangeTeamTime = (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds + 60f;
		}
		ExitScreen();
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
}
