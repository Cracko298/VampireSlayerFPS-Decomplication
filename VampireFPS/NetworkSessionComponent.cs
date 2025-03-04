using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;

namespace VampireFPS;

internal class NetworkSessionComponent : GameComponent
{
	public enum PacketTypes
	{
		None,
		PlayerData,
		ShipData,
		WorldSetup,
		WorldData,
		ShipInput,
		PowerUpSpawn,
		ShipDeath,
		ShipSpawn,
		GameWon,
		FireWeapon,
		TorchChanged,
		DoDamage,
		WeaponChanged,
		Spawned,
		Crouch,
		Score,
		WeaponType,
		Team,
		Class,
		Anim,
		Staked,
		Resurrect,
		Intermission,
		Level,
		Feed,
		DeleteBot,
		RankUp,
		CleanItems,
		CreateBot,
		BotData,
		BotFireWeapon,
		BotTorchChanged,
		BotDoDamage,
		BotDeath,
		BotWeaponChanged,
		BotSpawned,
		BotScore,
		BotWeaponType,
		BotTeam,
		BotClass,
		BotAnim,
		BotStaked,
		BotResurrect
	}

	public const int MaxLocalGamers = 1;

	private ScreenManager screenManager;

	private NetworkSession networkSession;

	private IMessageDisplay messageDisplay;

	private bool notifyWhenPlayersJoinOrLeave;

	private string sessionEndMessage;

	private PacketWriter packetWriter = new PacketWriter();

	private NetworkSessionComponent(ScreenManager screenManager, NetworkSession networkSession)
		: base(((GameComponent)screenManager).Game)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		this.screenManager = screenManager;
		this.networkSession = networkSession;
		g.m_App.m_NetworkSession = networkSession;
		networkSession.GamerJoined += GamerJoined;
		networkSession.GamerLeft += GamerLeft;
		networkSession.SessionEnded += NetworkSessionEnded;
		networkSession.AllowJoinInProgress = true;
	}

	public static void Create(ScreenManager screenManager, NetworkSession networkSession)
	{
		Game game = ((GameComponent)screenManager).Game;
		game.Services.AddService(typeof(NetworkSession), (object)networkSession);
		((Collection<IGameComponent>)(object)game.Components).Add((IGameComponent)(object)new NetworkSessionComponent(screenManager, networkSession));
	}

	public override void Initialize()
	{
		((GameComponent)this).Initialize();
		messageDisplay = (IMessageDisplay)((GameComponent)this).Game.Services.GetService(typeof(IMessageDisplay));
		if (messageDisplay != null)
		{
			notifyWhenPlayersJoinOrLeave = true;
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			((Collection<IGameComponent>)(object)((GameComponent)this).Game.Components).Remove((IGameComponent)(object)this);
			((GameComponent)this).Game.Services.RemoveService(typeof(NetworkSession));
			if (networkSession != null)
			{
				networkSession.Dispose();
				networkSession = null;
				g.m_App.m_NetworkSession = null;
			}
		}
		((GameComponent)this).Dispose(disposing);
	}

	public override void Update(GameTime gameTime)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Invalid comparison between Unknown and I4
		if (networkSession == null)
		{
			return;
		}
		try
		{
			networkSession.Update();
			if ((int)networkSession.SessionState == 2)
			{
				LeaveSession();
			}
		}
		catch (Exception)
		{
			sessionEndMessage = Resources.ErrorNetwork;
			LeaveSession();
		}
	}

	private void GamerJoined(object sender, GamerJoinedEventArgs e)
	{
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < ((ReadOnlyCollection<NetworkGamer>)(object)networkSession.AllGamers).Count; i++)
		{
			if (((ReadOnlyCollection<NetworkGamer>)(object)networkSession.AllGamers)[i] == e.Gamer)
			{
				Player tag = g.m_PlayerManager.Create(e.Gamer.Id, bot: false);
				((Gamer)e.Gamer).Tag = tag;
			}
		}
		if (((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count > 0 && !e.Gamer.IsLocal)
		{
			if (((Gamer)((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0]).Tag is Player player)
			{
				((BinaryWriter)(object)packetWriter).Write((byte)1);
				packetWriter.Write(player.m_Position);
				((BinaryWriter)(object)packetWriter).Write(player.m_Rotation.Y);
				((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)3, e.Gamer);
				((BinaryWriter)(object)packetWriter).Write((byte)16);
				((BinaryWriter)(object)packetWriter).Write(player.m_Kills);
				((BinaryWriter)(object)packetWriter).Write(player.m_Deaths);
				((BinaryWriter)(object)packetWriter).Write(player.m_Rank);
				((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)3, e.Gamer);
				((BinaryWriter)(object)packetWriter).Write((byte)18);
				((BinaryWriter)(object)packetWriter).Write((byte)player.m_Team);
				((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)3, e.Gamer);
				((BinaryWriter)(object)packetWriter).Write((byte)19);
				((BinaryWriter)(object)packetWriter).Write((byte)player.m_Class);
				((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)3, e.Gamer);
				if (player.m_WeaponItemIndex != -1)
				{
					((BinaryWriter)(object)packetWriter).Write((byte)17);
					((BinaryWriter)(object)packetWriter).Write(g.m_ItemManager.m_Item[player.m_WeaponItemIndex].m_Type);
					((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)3, e.Gamer);
				}
			}
			if (networkSession.IsHost)
			{
				((BinaryWriter)(object)packetWriter).Write((byte)24);
				((BinaryWriter)(object)packetWriter).Write((byte)g.m_App.m_Level);
				((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)3, e.Gamer);
				if (g.m_App.m_Intermission)
				{
					((BinaryWriter)(object)packetWriter).Write((byte)23);
					((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)3, e.Gamer);
				}
				for (int j = 0; j < 10; j++)
				{
					if (g.m_PlayerManager.m_Player[j].m_Id != -1 && g.m_PlayerManager.m_Player[j].m_Bot)
					{
						((BinaryWriter)(object)packetWriter).Write((byte)29);
						packetWriter.Write(g.m_PlayerManager.m_Player[j].m_Position);
						((BinaryWriter)(object)packetWriter).Write(g.m_PlayerManager.m_Player[j].m_Rotation.Y);
						((BinaryWriter)(object)packetWriter).Write(g.m_PlayerManager.m_Player[j].m_NetId);
						((BinaryWriter)(object)packetWriter).Write((byte)g.m_PlayerManager.m_Player[j].m_Team);
						((BinaryWriter)(object)packetWriter).Write((byte)g.m_PlayerManager.m_Player[j].m_Class);
						((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)3, e.Gamer);
						((BinaryWriter)(object)packetWriter).Write((byte)37);
						((BinaryWriter)(object)packetWriter).Write(g.m_PlayerManager.m_Player[j].m_NetId);
						((BinaryWriter)(object)packetWriter).Write(g.m_PlayerManager.m_Player[j].m_Kills);
						((BinaryWriter)(object)packetWriter).Write(g.m_PlayerManager.m_Player[j].m_Deaths);
						((BinaryWriter)(object)packetWriter).Write(g.m_PlayerManager.m_Player[j].m_Rank);
						((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)3, e.Gamer);
						((BinaryWriter)(object)packetWriter).Write((byte)38);
						((BinaryWriter)(object)packetWriter).Write(g.m_PlayerManager.m_Player[j].m_NetId);
						((BinaryWriter)(object)packetWriter).Write(g.m_ItemManager.m_Item[g.m_PlayerManager.m_Player[j].m_WeaponItemIndex].m_Type);
						((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)3, e.Gamer);
					}
				}
			}
		}
		if (notifyWhenPlayersJoinOrLeave)
		{
			messageDisplay.ShowMessage(Resources.MessageGamerJoined, ((Gamer)e.Gamer).Gamertag);
		}
		GC.Collect();
	}

	private void GamerLeft(object sender, GamerLeftEventArgs e)
	{
		Player player = ((Gamer)e.Gamer).Tag as Player;
		player.Delete();
		if (notifyWhenPlayersJoinOrLeave)
		{
			messageDisplay.ShowMessage(Resources.MessageGamerLeft, ((Gamer)e.Gamer).Gamertag);
		}
		GC.Collect();
	}

	private void NetworkSessionEnded(object sender, NetworkSessionEndedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected I4, but got Unknown
		NetworkSessionEndReason endReason = e.EndReason;
		switch ((int)endReason)
		{
		case 0:
			sessionEndMessage = null;
			break;
		case 1:
			sessionEndMessage = Resources.ErrorHostEndedSession;
			break;
		case 2:
			sessionEndMessage = Resources.ErrorRemovedByHost;
			break;
		default:
			sessionEndMessage = Resources.ErrorDisconnected;
			break;
		}
		notifyWhenPlayersJoinOrLeave = false;
	}

	public static void InviteAccepted(ScreenManager screenManager, InviteAcceptedEventArgs e)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		NetworkSessionComponent networkSessionComponent = FindSessionComponent(((GameComponent)screenManager).Game);
		if (g.m_PlayerManager.GetLocalPlayer() != null)
		{
			return;
		}
		if (networkSessionComponent != null)
		{
			((GameComponent)networkSessionComponent).Dispose();
		}
		try
		{
			IEnumerable<SignedInGamer> enumerable = ChooseGamers((NetworkSessionType)2, e.Gamer.PlayerIndex);
			IAsyncResult asyncResult = NetworkSession.BeginJoinInvited(enumerable, (AsyncCallback)null, (object)null);
			NetworkBusyScreen networkBusyScreen = new NetworkBusyScreen(asyncResult);
			networkBusyScreen.OperationCompleted += JoinInvitedOperationCompleted;
			LoadingScreen.Load(screenManager, false, null, new BackgroundScreen(), networkBusyScreen);
		}
		catch (Exception exception)
		{
			NetworkErrorScreen networkErrorScreen = new NetworkErrorScreen(exception);
			LoadingScreen.Load(screenManager, false, null, new BackgroundScreen(), new MainMenuScreen(), networkErrorScreen);
		}
	}

	private static void JoinInvitedOperationCompleted(object sender, OperationCompletedEventArgs e)
	{
		ScreenManager screenManager = ((GameScreen)sender).ScreenManager;
		try
		{
			NetworkSession val = NetworkSession.EndJoinInvited(e.AsyncResult);
			Create(screenManager, val);
			screenManager.AddScreen(new LobbyScreen(val), null);
		}
		catch (Exception exception)
		{
			screenManager.AddScreen(new MainMenuScreen(), null);
			screenManager.AddScreen(new NetworkErrorScreen(exception), null);
		}
	}

	public static bool IsOnlineSessionType(NetworkSessionType sessionType)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected I4, but got Unknown
		switch ((int)sessionType)
		{
		case 0:
		case 1:
			return false;
		case 2:
		case 3:
			return true;
		default:
			throw new NotSupportedException();
		}
	}

	public static IEnumerable<SignedInGamer> ChooseGamers(NetworkSessionType sessionType, PlayerIndex playerIndex)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		List<SignedInGamer> list = new List<SignedInGamer>();
		SignedInGamer val = Gamer.SignedInGamers[playerIndex];
		if (val == null)
		{
			throw new GamerPrivilegeException();
		}
		list.Add(val);
		GamerCollectionEnumerator<SignedInGamer> enumerator = ((GamerCollection<SignedInGamer>)(object)Gamer.SignedInGamers).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				SignedInGamer current = enumerator.Current;
				if (list.Count >= 1)
				{
					break;
				}
				if (current != val && (!IsOnlineSessionType(sessionType) || (current.IsSignedInToLive && current.Privileges.AllowOnlineSessions)))
				{
					if (val.IsGuest && !current.IsGuest && list[0] == val)
					{
						list.Insert(0, current);
					}
					else
					{
						list.Add(current);
					}
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		return list;
	}

	public static void LeaveSession(ScreenManager screenManager, PlayerIndex playerIndex)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		NetworkSessionComponent self = FindSessionComponent(((GameComponent)screenManager).Game);
		if (self != null)
		{
			string message = ((!self.networkSession.IsHost) ? Resources.ConfirmLeaveSession : Resources.ConfirmEndSession);
			MessageBoxScreen messageBoxScreen = new MessageBoxScreen(message);
			messageBoxScreen.Accepted += delegate
			{
				self.LeaveSession();
			};
			screenManager.AddScreen(messageBoxScreen, playerIndex);
		}
	}

	private void LeaveSession()
	{
		g.m_PlayerManager.DeleteAll();
		((GameComponent)this).Dispose();
		MessageBoxScreen messageBoxScreen = (string.IsNullOrEmpty(sessionEndMessage) ? null : new MessageBoxScreen(sessionEndMessage, includeUsageText: false));
		GameScreen[] screens = screenManager.GetScreens();
		for (int i = 0; i < screens.Length; i++)
		{
			if (screens[i] is MainMenuScreen)
			{
				for (int j = i + 1; j < screens.Length; j++)
				{
					screens[j].ExitScreen();
				}
				if (messageBoxScreen != null)
				{
					screenManager.AddScreen(messageBoxScreen, null);
				}
				return;
			}
		}
		LoadingScreen.Load(screenManager, false, null, new BackgroundScreen(), new MainMenuScreen(), messageBoxScreen);
	}

	private static NetworkSessionComponent FindSessionComponent(Game game)
	{
		return ((IEnumerable)game.Components).OfType<NetworkSessionComponent>().FirstOrDefault();
	}
}
