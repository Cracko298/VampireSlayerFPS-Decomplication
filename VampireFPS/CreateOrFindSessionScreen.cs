using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;

namespace VampireFPS;

internal class CreateOrFindSessionScreen : MenuScreen
{
	private NetworkSessionType sessionType;

	public CreateOrFindSessionScreen(NetworkSessionType sessionType)
		: base(GetMenuTitle(sessionType))
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		this.sessionType = sessionType;
		MenuEntry menuEntry = new MenuEntry(Resources.CreateSession);
		MenuEntry menuEntry2 = new MenuEntry("CREATE PRIVATE SESSION");
		MenuEntry menuEntry3 = new MenuEntry(Resources.FindSessions);
		MenuEntry menuEntry4 = new MenuEntry(Resources.Back);
		menuEntry.Selected += CreateSessionMenuEntrySelected;
		menuEntry2.Selected += CreateFriendsOnlySessionMenuEntrySelected;
		menuEntry3.Selected += FindSessionsMenuEntrySelected;
		menuEntry4.Selected += base.OnCancel;
		base.MenuEntries.Add(menuEntry3);
		base.MenuEntries.Add(menuEntry);
		base.MenuEntries.Add(menuEntry2);
		base.MenuEntries.Add(menuEntry4);
	}

	private static string GetMenuTitle(NetworkSessionType sessionType)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected I4, but got Unknown
		return (sessionType - 1) switch
		{
			1 => Resources.PlayerMatch, 
			0 => Resources.SystemLink, 
			_ => throw new NotSupportedException(), 
		};
	}

	private void CreateSessionMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			IEnumerable<SignedInGamer> enumerable = NetworkSessionComponent.ChooseGamers(sessionType, base.ControllingPlayer.Value);
			IAsyncResult asyncResult = NetworkSession.BeginCreate(sessionType, enumerable, g.m_App.m_OptionsMaxPlayers, 0, (NetworkSessionProperties)null, (AsyncCallback)null, (object)null);
			NetworkBusyScreen networkBusyScreen = new NetworkBusyScreen(asyncResult);
			networkBusyScreen.OperationCompleted += CreateSessionOperationCompleted;
			base.ScreenManager.AddScreen(networkBusyScreen, base.ControllingPlayer);
		}
		catch (Exception exception)
		{
			NetworkErrorScreen screen = new NetworkErrorScreen(exception);
			base.ScreenManager.AddScreen(screen, base.ControllingPlayer);
		}
	}

	private void CreateFriendsOnlySessionMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			IEnumerable<SignedInGamer> enumerable = NetworkSessionComponent.ChooseGamers(sessionType, base.ControllingPlayer.Value);
			IAsyncResult asyncResult = NetworkSession.BeginCreate(sessionType, enumerable, g.m_App.m_OptionsMaxPlayers, g.m_App.m_OptionsMaxPlayers - 1, (NetworkSessionProperties)null, (AsyncCallback)null, (object)null);
			NetworkBusyScreen networkBusyScreen = new NetworkBusyScreen(asyncResult);
			networkBusyScreen.OperationCompleted += CreateSessionOperationCompleted;
			base.ScreenManager.AddScreen(networkBusyScreen, base.ControllingPlayer);
		}
		catch (Exception exception)
		{
			NetworkErrorScreen screen = new NetworkErrorScreen(exception);
			base.ScreenManager.AddScreen(screen, base.ControllingPlayer);
		}
	}

	private void CreateSessionOperationCompleted(object sender, OperationCompletedEventArgs e)
	{
		try
		{
			NetworkSession networkSession = NetworkSession.EndCreate(e.AsyncResult);
			NetworkSessionComponent.Create(base.ScreenManager, networkSession);
			base.ScreenManager.AddScreen(new LobbyScreen(networkSession), null);
		}
		catch (Exception exception)
		{
			NetworkErrorScreen screen = new NetworkErrorScreen(exception);
			base.ScreenManager.AddScreen(screen, base.ControllingPlayer);
		}
	}

	private void FindSessionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			IEnumerable<SignedInGamer> enumerable = NetworkSessionComponent.ChooseGamers(sessionType, base.ControllingPlayer.Value);
			IAsyncResult asyncResult = NetworkSession.BeginFind(sessionType, enumerable, (NetworkSessionProperties)null, (AsyncCallback)null, (object)null);
			NetworkBusyScreen networkBusyScreen = new NetworkBusyScreen(asyncResult);
			networkBusyScreen.OperationCompleted += FindSessionsOperationCompleted;
			base.ScreenManager.AddScreen(networkBusyScreen, base.ControllingPlayer);
		}
		catch (Exception exception)
		{
			NetworkErrorScreen screen = new NetworkErrorScreen(exception);
			base.ScreenManager.AddScreen(screen, base.ControllingPlayer);
		}
	}

	private void FindSessionsOperationCompleted(object sender, OperationCompletedEventArgs e)
	{
		GameScreen screen;
		try
		{
			AvailableNetworkSessionCollection val = NetworkSession.EndFind(e.AsyncResult);
			if (((ReadOnlyCollection<AvailableNetworkSession>)(object)val).Count == 0)
			{
				val.Dispose();
				screen = new MessageBoxScreen(Resources.NoSessionsFound, includeUsageText: false);
			}
			else
			{
				screen = new JoinSessionScreen(val);
			}
		}
		catch (Exception exception)
		{
			screen = new NetworkErrorScreen(exception);
		}
		base.ScreenManager.AddScreen(screen, base.ControllingPlayer);
	}
}
