using System;
using System.Collections.ObjectModel;
using System.IO;
using BEPUphysics;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.DataStructures;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Rendering;

namespace VampireFPS;

internal class GameplayScreen : GameScreen
{
	private const float LEVEL_TIME = 1000000f;

	private const int updatesBetweenWorldDataSend = 30;

	private const int updatesBetweenStatusPackets = 4;

	private const int updatesBetweenBotPackets = 4;

	private NetworkSession networkSession;

	private PacketWriter packetWriter = new PacketWriter();

	private PacketReader packetReader = new PacketReader();

	private int updatesSinceStatusPacket;

	private int updatesSinceBotPacket = 2;

	private ContentManager content;

	private Vector2 playerPosition = new Vector2(100f, 100f);

	private Vector2 enemyPosition = new Vector2(100f, 100f);

	private Random random = new Random();

	private bool jump;

	private bool leap;

	private Vector2 movement;

	private float turn;

	private float pauseAlpha;

	private GamePadState m_GamepadState;

	private GamePadState m_OldGamepadState;

	private KeyboardState m_KeyboardState;

	private KeyboardState m_OldKeyboardState;

	private Model skybox;

	private Matrix skyboxWorld;

	private float m_LevelTime;

	public StaticMesh m_CollisionMesh;

	private new bool IsActive
	{
		get
		{
			if (networkSession == null)
			{
				return base.IsActive;
			}
			return !base.IsExiting;
		}
	}

	public GameplayScreen(NetworkSession networkSession)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		this.networkSession = networkSession;
		base.TransitionOnTime = TimeSpan.FromSeconds(1.5);
		base.TransitionOffTime = TimeSpan.FromSeconds(0.5);
		if (Gamer.SignedInGamers[g.m_App.m_PlayerOnePadId] != null)
		{
			if (networkSession != null)
			{
				Gamer.SignedInGamers[g.m_App.m_PlayerOnePadId].Presence.PresenceMode = (GamerPresenceMode)2;
			}
			else
			{
				Gamer.SignedInGamers[g.m_App.m_PlayerOnePadId].Presence.PresenceMode = (GamerPresenceMode)1;
			}
		}
	}

	public override void LoadContent()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		if (content == null)
		{
			content = new ContentManager((IServiceProvider)((GameComponent)base.ScreenManager).Game.Services, "Content");
		}
		if (g.m_PlayerManager.GetLocalPlayer().IsHost())
		{
			LoadSunburnScene();
		}
		((GameComponent)base.ScreenManager).Game.ResetElapsedTime();
		g.m_CameraManager.Init();
		g.m_LoadSaveManager.LoadGame();
		m_LevelTime = (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds + (float)g.m_App.m_OptionsMapTime * 60f;
	}

	public void LoadSunburnScene()
	{
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		if (g.m_App.m_Level == 1)
		{
			g.m_App.scene = g.m_App.m_Scene1;
		}
		else
		{
			g.m_App.scene = g.m_App.m_Scene2;
		}
		g.m_App.sceneInterface.Submit((IScene)(object)g.m_App.scene);
		if (g.m_App.m_Level == 1)
		{
			g.m_App.environment = g.m_App.m_Environment1;
		}
		else
		{
			g.m_App.environment = g.m_App.m_Environment2;
		}
		Vector3[] array = default(Vector3[]);
		int[] array2 = default(int[]);
		if (g.m_App.m_Level == 1)
		{
			TriangleMesh.GetVerticesAndIndicesFromModel(g.m_App.m_CollisionModel1, ref array, ref array2);
		}
		else
		{
			TriangleMesh.GetVerticesAndIndicesFromModel(g.m_App.m_CollisionModel2, ref array, ref array2);
		}
		m_CollisionMesh = new StaticMesh(array, array2, new AffineTransform(new Vector3(0f, 0f, 0f)));
		g.m_App.m_Space.Add((ISpaceObject)(object)m_CollisionMesh);
		if (g.m_App.m_Level > 1)
		{
			skybox = g.m_App.m_Skybox;
			skyboxWorld = Matrix.CreateTranslation(new Vector3(0f, 0f, -20f)) * Matrix.CreateRotationX(-(float)Math.PI / 2f) * Matrix.CreateRotationY((float)Math.PI * -17f / 20f);
		}
		if (g.m_App.m_Level == 1)
		{
			MediaPlayer.Play(g.m_App.m_Level1Music);
			MediaPlayer.IsRepeating = true;
		}
		else
		{
			MediaPlayer.Play(g.m_App.m_Level2Music);
			MediaPlayer.IsRepeating = true;
		}
		g.m_CameraManager.Init();
		g.m_CameraManager.Update();
		g.m_App.m_RumbleFrames = 0;
	}

	public override void UnloadContent()
	{
		if (m_CollisionMesh != null)
		{
			g.m_App.m_Space.Remove((ISpaceObject)(object)m_CollisionMesh);
			m_CollisionMesh = null;
		}
		content.Unload();
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen: false);
		if (coveredByOtherScreen)
		{
			pauseAlpha = Math.Min(pauseAlpha + 1f / 32f, 1f);
		}
		else
		{
			pauseAlpha = Math.Max(pauseAlpha - 1f / 32f, 0f);
		}
		if (!g.m_App.m_Paused || g.m_App.m_SingleStep)
		{
			g.m_App.m_SingleStep = false;
			if (g.m_App.m_Intermission)
			{
				UpdateNetworking((float)gameTime.ElapsedGameTime.TotalSeconds);
			}
			if (IsActive && !g.m_App.m_Intermission)
			{
				g.m_App.sceneInterface.Update(gameTime);
				g.m_App.m_ParticleSystemManager.UpdateAllParticleSystems((float)gameTime.ElapsedGameTime.TotalSeconds);
				UpdateNetworking((float)gameTime.ElapsedGameTime.TotalSeconds);
				g.m_PlayerManager.ClearDamageRequests();
				g.m_ItemManager.Update();
				g.m_PlayerManager.Update();
				g.m_CameraManager.Update();
				g.m_BotPathManager.Update();
				g.m_PlayerManager.UpdatePrevousPositions();
				g.m_App.m_Space.Update();
			}
			if (networkSession != null && !base.IsExiting && (int)networkSession.SessionState == 0)
			{
				LoadingScreen.Load(base.ScreenManager, true, null, new BackgroundScreen(), new LobbyScreen(networkSession));
			}
			if (networkSession == null || networkSession.IsHost)
			{
				UpdateGameRules();
			}
			if (g.m_App.m_Intermission && g.m_App.m_IntermissionTime < (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds)
			{
				UpdateIntermission();
			}
			if (networkSession == null || networkSession.IsHost)
			{
				UpdateBotSpawner();
			}
		}
	}

	private void UpdateGameRules()
	{
		if (g.m_App.scene != null)
		{
			float num = (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds;
			if (m_LevelTime < num && !g.m_App.m_Intermission)
			{
				g.m_App.m_RequestIntermission = true;
				g.m_App.m_Intermission = true;
				g.m_App.m_IntermissionTime = num + 5f;
			}
		}
	}

	public void UpdateIntermission()
	{
		g.m_App.ClearSystemMessages();
		g.m_BotPathManager.DeleteAll();
		g.m_App.m_Space.Remove((ISpaceObject)(object)m_CollisionMesh);
		m_CollisionMesh = null;
		g.m_App.sceneInterface.Clear();
		if (g.m_App.m_Level == 1)
		{
			g.m_App.m_Level = 2;
		}
		else
		{
			g.m_App.m_Level = 1;
		}
		LoadSunburnScene();
		g.m_BotPathManager.LoadBotPath();
		((GameComponent)base.ScreenManager).Game.ResetElapsedTime();
		g.m_CameraManager.Init();
		g.m_LoadSaveManager.SaveGame();
		m_LevelTime = (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds + (float)g.m_App.m_OptionsMapTime * 60f;
		g.m_PlayerManager.ReInitPlayers();
		g.m_PlayerManager.ReInitTorches();
		if (g.m_PlayerManager.GetLocalPlayer().IsHost())
		{
			g.m_PlayerManager.ReInitBots();
		}
		GameScreen[] screens = g.m_App.screenManager.GetScreens();
		for (int i = 0; i < screens.Length; i++)
		{
			if (screens[i] is JoinTeamMenuScreen)
			{
				screens[i].ScreenManager.RemoveScreen(screens[i]);
			}
			if (screens[i] is HunterMenuScreen)
			{
				screens[i].ScreenManager.RemoveScreen(screens[i]);
			}
			if (screens[i] is VampireMenuScreen)
			{
				screens[i].ScreenManager.RemoveScreen(screens[i]);
			}
		}
		g.m_PlayerManager.GetLocalPlayer().m_State = Player.STATE.JoinTeam;
		g.m_App.m_Intermission = false;
		GC.Collect();
	}

	private void UpdateBotSpawner()
	{
		if (g.m_PlayerManager.GetLocalPlayer() == null || g.m_PlayerManager.GetLocalPlayer().m_State != Player.STATE.InGame || g.m_PlayerManager.GetLocalPlayer().m_Team == Player.TEAM.None)
		{
			return;
		}
		if (g.m_App.m_NetworkSession != null)
		{
			if (g.m_App.m_OptionsBotsMP > 0)
			{
				int num = g.m_App.m_OptionsBotsMP - (g.m_PlayerManager.NumPlayersOnTeams() - 1);
				if (g.m_PlayerManager.NumBots() < num)
				{
					g.m_BotPathManager.LoadBotPath();
					SendCreateBot();
				}
				else if (g.m_PlayerManager.NumBots() > num)
				{
					g.m_PlayerManager.RemoveBot();
				}
			}
		}
		else if (g.m_PlayerManager.NumBots() < g.m_App.m_OptionsBotsSP)
		{
			g.m_BotPathManager.LoadBotPath();
			g.m_PlayerManager.Create(g.m_PlayerManager.GetNextBotId(), bot: true);
		}
	}

	public override void HandleInput(InputState input)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (input == null)
		{
			throw new ArgumentNullException("input");
		}
		if (base.ControllingPlayer.HasValue)
		{
			HandlePlayerInput(input, base.ControllingPlayer.Value);
		}
		else
		{
			if (networkSession == null)
			{
				return;
			}
			GamerCollectionEnumerator<LocalNetworkGamer> enumerator = networkSession.LocalGamers.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					LocalNetworkGamer current = enumerator.Current;
					if (!HandlePlayerInput(input, current.SignedInGamer.PlayerIndex))
					{
						break;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
		}
	}

	private bool HandlePlayerInput(InputState input, PlayerIndex playerIndex)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		m_KeyboardState = input.CurrentKeyboardStates[playerIndex];
		m_GamepadState = input.CurrentGamePadStates[playerIndex];
		bool flag = !((GamePadState)(ref m_GamepadState)).IsConnected && input.GamePadWasConnected[playerIndex];
		if (input.IsPauseGame(playerIndex) || flag)
		{
			base.ScreenManager.AddScreen(new PauseMenuScreen(networkSession), playerIndex);
			return false;
		}
		movement = Vector2.Zero;
		jump = false;
		leap = false;
		turn = 0f;
		UpdateGamepadControl();
		if (((Vector2)(ref movement)).Length() > 1f)
		{
			((Vector2)(ref movement)).Normalize();
		}
		g.m_PlayerManager.GetLocalPlayer().m_Movement = movement;
		g.m_PlayerManager.GetLocalPlayer().m_Turn = turn;
		g.m_PlayerManager.GetLocalPlayer().m_Jump = jump;
		g.m_PlayerManager.GetLocalPlayer().m_Leap = leap;
		m_OldGamepadState = m_GamepadState;
		m_OldKeyboardState = m_KeyboardState;
		g.m_PlayerManager.GetLocalPlayer().UpdateRumble(playerIndex);
		return true;
	}

	private void UpdateGamepadControl()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Invalid comparison between Unknown and I4
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		if (g.m_PlayerManager.GetLocalPlayer().m_State == Player.STATE.InGame || g.m_PlayerManager.GetLocalPlayer().m_State == Player.STATE.LocalDeath)
		{
			GamePadButtons buttons = ((GamePadState)(ref m_GamepadState)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).Back == 1)
			{
				g.m_App.m_ShowScoreboard = true;
			}
			else
			{
				g.m_App.m_ShowScoreboard = false;
			}
		}
		if (g.m_PlayerManager.GetLocalPlayer().m_State != Player.STATE.InGame)
		{
			return;
		}
		float num = 1f;
		if (g.m_PlayerManager.GetLocalPlayer().m_Team == Player.TEAM.Hunter)
		{
			GamePadTriggers triggers = ((GamePadState)(ref m_GamepadState)).Triggers;
			if (((GamePadTriggers)(ref triggers)).Left > 0.1f)
			{
				g.m_PlayerManager.GetLocalPlayer().ZoomWeapon(bZoom: true);
				num = 0.25f;
			}
			else
			{
				g.m_PlayerManager.GetLocalPlayer().ZoomWeapon(bZoom: false);
			}
		}
		GamePadThumbSticks thumbSticks = ((GamePadState)(ref m_GamepadState)).ThumbSticks;
		if (Math.Abs(((GamePadThumbSticks)(ref thumbSticks)).Left.Y) > 0.1f)
		{
			ref Vector2 reference = ref movement;
			float y = reference.Y;
			GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref m_GamepadState)).ThumbSticks;
			reference.Y = y - ((GamePadThumbSticks)(ref thumbSticks2)).Left.Y * num;
		}
		GamePadThumbSticks thumbSticks3 = ((GamePadState)(ref m_GamepadState)).ThumbSticks;
		if (Math.Abs(((GamePadThumbSticks)(ref thumbSticks3)).Left.X) > 0.1f)
		{
			ref Vector2 reference2 = ref movement;
			float x = reference2.X;
			GamePadThumbSticks thumbSticks4 = ((GamePadState)(ref m_GamepadState)).ThumbSticks;
			reference2.X = x + ((GamePadThumbSticks)(ref thumbSticks4)).Left.X * num;
		}
		GamePadThumbSticks thumbSticks5 = ((GamePadState)(ref m_GamepadState)).ThumbSticks;
		if (Math.Abs(((GamePadThumbSticks)(ref thumbSticks5)).Right.X) > 0.1f)
		{
			GamePadThumbSticks thumbSticks6 = ((GamePadState)(ref m_GamepadState)).ThumbSticks;
			float x2 = ((GamePadThumbSticks)(ref thumbSticks6)).Right.X;
			GamePadThumbSticks thumbSticks7 = ((GamePadState)(ref m_GamepadState)).ThumbSticks;
			float num2 = x2 * ((GamePadThumbSticks)(ref thumbSticks7)).Right.X;
			GamePadThumbSticks thumbSticks8 = ((GamePadState)(ref m_GamepadState)).ThumbSticks;
			turn = num2 * ((GamePadThumbSticks)(ref thumbSticks8)).Right.X * num;
		}
		float num3 = (g.m_App.m_OptionsInvertY ? (-1f) : 1f);
		GamePadThumbSticks thumbSticks9 = ((GamePadState)(ref m_GamepadState)).ThumbSticks;
		if (Math.Abs(((GamePadThumbSticks)(ref thumbSticks9)).Right.Y) > 0.1f)
		{
			CameraManager cameraManager = g.m_CameraManager;
			float pitch = cameraManager.m_Pitch;
			GamePadThumbSticks thumbSticks10 = ((GamePadState)(ref m_GamepadState)).ThumbSticks;
			float y2 = ((GamePadThumbSticks)(ref thumbSticks10)).Right.Y;
			GamePadThumbSticks thumbSticks11 = ((GamePadState)(ref m_GamepadState)).ThumbSticks;
			float num4 = y2 * ((GamePadThumbSticks)(ref thumbSticks11)).Right.Y;
			GamePadThumbSticks thumbSticks12 = ((GamePadState)(ref m_GamepadState)).ThumbSticks;
			cameraManager.m_Pitch = pitch + num4 * ((GamePadThumbSticks)(ref thumbSticks12)).Right.Y * 0.015f * num * (60f * (float)g.m_App.m_GameTime.ElapsedGameTime.TotalSeconds) * (g.m_App.m_OptionsVert * 0.4f) * num3;
		}
		if (g.m_CameraManager.m_Pitch < -1.1f)
		{
			g.m_CameraManager.m_Pitch = -1.1f;
		}
		if (g.m_CameraManager.m_Pitch > 1.2f)
		{
			g.m_CameraManager.m_Pitch = 1.2f;
		}
		GamePadTriggers triggers2 = ((GamePadState)(ref m_GamepadState)).Triggers;
		if (((GamePadTriggers)(ref triggers2)).Right > 0.1f)
		{
			g.m_PlayerManager.GetLocalPlayer().m_bFired = true;
			g.m_PlayerManager.GetLocalPlayer().FireWeapon();
		}
		if (Debounce((Buttons)512))
		{
			if (g.m_PlayerManager.GetLocalPlayer().m_Team == Player.TEAM.Hunter)
			{
				g.m_PlayerManager.GetLocalPlayer().ToggleTorchLight();
				g.m_PlayerManager.GetLocalPlayer().m_bTorchChanged = true;
			}
			else
			{
				g.m_PlayerManager.GetLocalPlayer().TryFeed();
			}
		}
		if (Debounce((Buttons)4096))
		{
			jump = true;
		}
		if (g.m_PlayerManager.GetLocalPlayer().m_Team == Player.TEAM.Vampire)
		{
			GamePadTriggers triggers3 = ((GamePadState)(ref m_GamepadState)).Triggers;
			if (((GamePadTriggers)(ref triggers3)).Left > 0.1f)
			{
				GamePadTriggers triggers4 = ((GamePadState)(ref m_OldGamepadState)).Triggers;
				if (((GamePadTriggers)(ref triggers4)).Left < 0.1f)
				{
					leap = true;
				}
			}
		}
		if (Debounce((Buttons)8192))
		{
			g.m_PlayerManager.GetLocalPlayer().Crouch();
		}
		if (Debounce((Buttons)16384))
		{
			int weaponItemIndex = g.m_PlayerManager.GetLocalPlayer().m_WeaponItemIndex;
			if (weaponItemIndex != -1)
			{
				g.m_ItemManager.m_Item[weaponItemIndex].Reload();
			}
		}
		if (Debounce((Buttons)32768))
		{
			g.m_PlayerManager.GetLocalPlayer().HolsterWeapon();
		}
	}

	private void UpdateKeyboardControl()
	{
	}

	public bool Debounce(Buttons b)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Invalid comparison between Unknown and I4
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Invalid comparison between Unknown and I4
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Invalid comparison between Unknown and I4
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Invalid comparison between Unknown and I4
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Invalid comparison between Unknown and I4
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Invalid comparison between Unknown and I4
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Invalid comparison between Unknown and I4
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Invalid comparison between Unknown and I4
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Invalid comparison between Unknown and I4
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Invalid comparison between Unknown and I4
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Invalid comparison between Unknown and I4
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Invalid comparison between Unknown and I4
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Invalid comparison between Unknown and I4
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Invalid comparison between Unknown and I4
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Invalid comparison between Unknown and I4
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Invalid comparison between Unknown and I4
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		if ((int)b <= 512)
		{
			if ((int)b <= 32)
			{
				if ((int)b != 16)
				{
					if ((int)b == 32)
					{
						GamePadButtons buttons = ((GamePadState)(ref m_GamepadState)).Buttons;
						if ((int)((GamePadButtons)(ref buttons)).Back == 1)
						{
							GamePadButtons buttons2 = ((GamePadState)(ref m_OldGamepadState)).Buttons;
							if ((int)((GamePadButtons)(ref buttons2)).Back == 0)
							{
								return true;
							}
						}
					}
				}
				else
				{
					GamePadButtons buttons3 = ((GamePadState)(ref m_GamepadState)).Buttons;
					if ((int)((GamePadButtons)(ref buttons3)).Start == 1)
					{
						GamePadButtons buttons4 = ((GamePadState)(ref m_OldGamepadState)).Buttons;
						if ((int)((GamePadButtons)(ref buttons4)).Start == 0)
						{
							return true;
						}
					}
				}
			}
			else if ((int)b != 256)
			{
				if ((int)b == 512)
				{
					GamePadButtons buttons5 = ((GamePadState)(ref m_GamepadState)).Buttons;
					if ((int)((GamePadButtons)(ref buttons5)).RightShoulder == 1)
					{
						GamePadButtons buttons6 = ((GamePadState)(ref m_OldGamepadState)).Buttons;
						if ((int)((GamePadButtons)(ref buttons6)).RightShoulder == 0)
						{
							return true;
						}
					}
				}
			}
			else
			{
				GamePadButtons buttons7 = ((GamePadState)(ref m_GamepadState)).Buttons;
				if ((int)((GamePadButtons)(ref buttons7)).LeftShoulder == 1)
				{
					GamePadButtons buttons8 = ((GamePadState)(ref m_OldGamepadState)).Buttons;
					if ((int)((GamePadButtons)(ref buttons8)).LeftShoulder == 0)
					{
						return true;
					}
				}
			}
		}
		else if ((int)b <= 8192)
		{
			if ((int)b != 4096)
			{
				if ((int)b == 8192)
				{
					GamePadButtons buttons9 = ((GamePadState)(ref m_GamepadState)).Buttons;
					if ((int)((GamePadButtons)(ref buttons9)).B == 1)
					{
						GamePadButtons buttons10 = ((GamePadState)(ref m_OldGamepadState)).Buttons;
						if ((int)((GamePadButtons)(ref buttons10)).B == 0)
						{
							return true;
						}
					}
				}
			}
			else
			{
				GamePadButtons buttons11 = ((GamePadState)(ref m_GamepadState)).Buttons;
				if ((int)((GamePadButtons)(ref buttons11)).A == 1)
				{
					GamePadButtons buttons12 = ((GamePadState)(ref m_OldGamepadState)).Buttons;
					if ((int)((GamePadButtons)(ref buttons12)).A == 0)
					{
						return true;
					}
				}
			}
		}
		else if ((int)b != 16384)
		{
			if ((int)b == 32768)
			{
				GamePadButtons buttons13 = ((GamePadState)(ref m_GamepadState)).Buttons;
				if ((int)((GamePadButtons)(ref buttons13)).Y == 1)
				{
					GamePadButtons buttons14 = ((GamePadState)(ref m_OldGamepadState)).Buttons;
					if ((int)((GamePadButtons)(ref buttons14)).Y == 0)
					{
						return true;
					}
				}
			}
		}
		else
		{
			GamePadButtons buttons15 = ((GamePadState)(ref m_GamepadState)).Buttons;
			if ((int)((GamePadButtons)(ref buttons15)).X == 1)
			{
				GamePadButtons buttons16 = ((GamePadState)(ref m_OldGamepadState)).Buttons;
				if ((int)((GamePadButtons)(ref buttons16)).X == 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		g.m_App.sceneState.BeginFrameRendering(g.m_CameraManager.m_ViewMatrix, g.m_CameraManager.m_ProjectionMatrix, gameTime, (ISceneEnvironment)(object)g.m_App.environment, g.m_App.frameBuffers, true);
		g.m_App.sceneInterface.BeginFrameRendering((ISceneState)(object)g.m_App.sceneState);
		RenderSky((ISceneState)(object)g.m_App.sceneState);
		g.m_App.sceneInterface.RenderManager.Render();
		g.m_App.graphics.GraphicsDevice.DepthStencilState = DepthStencilState.None;
		g.m_App.m_ParticleSystemManager.SetCameraPositionForAllParticleSystems(g.m_CameraManager.m_Position);
		g.m_App.m_ParticleSystemManager.SetWorldViewProjectionMatricesForAllParticleSystems(Matrix.Identity, g.m_CameraManager.m_ViewMatrix, g.m_CameraManager.m_ProjectionMatrix);
		g.m_App.m_ParticleSystemManager.DrawAllParticleSystems();
		g.m_App.screenManager.SpriteBatch.Begin();
		g.m_App.DrawHud();
		g.m_App.DebugDraw();
		g.m_App.screenManager.SpriteBatch.End();
		g.m_App.sceneInterface.EndFrameRendering();
		g.m_App.sceneState.EndFrameRendering();
	}

	protected void RenderSky(ISceneState scenestate)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		if (g.m_App.m_Level == 1)
		{
			return;
		}
		if (skybox == null)
		{
			((Game)g.m_App).GraphicsDevice.Clear((ClearOptions)7, Color.Black, 1f, 0);
			return;
		}
		((Game)g.m_App).GraphicsDevice.Clear((ClearOptions)6, Color.Black, 1f, 0);
		((Game)g.m_App).GraphicsDevice.BlendState = BlendState.Opaque;
		((Game)g.m_App).GraphicsDevice.DepthStencilState = DepthStencilState.None;
		((Game)g.m_App).GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
		((Game)g.m_App).GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicClamp;
		for (int i = 1; i < 8; i++)
		{
			((Game)g.m_App).GraphicsDevice.SamplerStates[i] = SamplerState.PointClamp;
		}
		Matrix view = scenestate.View;
		((Matrix)(ref view)).Translation = Vector3.Zero;
		Enumerator enumerator = skybox.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				Enumerator enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						Effect current2 = ((Enumerator)(ref enumerator2)).Current;
						if (current2 is BasicEffect)
						{
							BasicEffect val = (BasicEffect)(object)((current2 is BasicEffect) ? current2 : null);
							val.LightingEnabled = false;
							val.DiffuseColor = new Vector3(1f, 1f, 1f);
							val.View = view;
							val.World = skyboxWorld;
							val.Projection = scenestate.Projection;
						}
					}
				}
				finally
				{
					((IDisposable)(Enumerator)(ref enumerator2)/*cast due to .constrained prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(Enumerator)(ref enumerator)/*cast due to .constrained prefix*/).Dispose();
		}
		((Game)g.m_App).GraphicsDevice.DepthStencilState = DepthStencilState.Default;
	}

	private void UpdateNetworking(float elapsedTime)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Invalid comparison between Unknown and I4
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		ProcessPackets();
		if (networkSession == null || (int)networkSession.SessionState != 1)
		{
			return;
		}
		for (int i = 0; i < ((ReadOnlyCollection<NetworkGamer>)(object)networkSession.AllGamers).Count; i++)
		{
			NetworkGamer val = ((ReadOnlyCollection<NetworkGamer>)(object)networkSession.AllGamers)[i];
			Player player = ((Gamer)val).Tag as Player;
			if (player != null)
			{
			}
		}
		if (networkSession.IsHost)
		{
			_ = networkSession.Host;
			bool flag = false;
			for (int j = 0; j < 10; j++)
			{
				if (g.m_PlayerManager.m_Player[j].m_Id != -1 && g.m_PlayerManager.m_Player[j].m_Bot)
				{
					if (g.m_PlayerManager.m_Player[j].m_RequestSendTeam)
					{
						SendBotTeamChangedMessage(g.m_PlayerManager.m_Player[j].m_NetId);
					}
					if (g.m_PlayerManager.m_Player[j].m_RequestSendClass)
					{
						SendBotClassChangedMessage(g.m_PlayerManager.m_Player[j].m_NetId);
					}
					if (g.m_PlayerManager.m_Player[j].m_bRequestDied)
					{
						SendBotDeath(g.m_PlayerManager.m_Player[j].m_NetId);
					}
					if (g.m_PlayerManager.m_Player[j].m_bFired)
					{
						SendBotFireMessage(g.m_PlayerManager.m_Player[j].m_NetId);
					}
					if (g.m_PlayerManager.m_Player[j].m_bTorchChanged)
					{
						SendBotTorchChangedMessage(g.m_PlayerManager.m_Player[j].m_NetId);
					}
					if (g.m_PlayerManager.m_Player[j].m_bRequestSendDamage)
					{
						SendBotDamageMessage(g.m_PlayerManager.m_Player[j].m_NetId);
					}
					if (g.m_PlayerManager.m_Player[j].m_bRequestStaked)
					{
						SendBotStaked(g.m_PlayerManager.m_Player[j].m_NetId);
					}
					if (g.m_PlayerManager.m_Player[j].m_bWeaponChanged)
					{
						SendBotWeaponChangedMessage(g.m_PlayerManager.m_Player[j].m_NetId);
					}
					if (g.m_PlayerManager.m_Player[j].m_bRequestSendSpawn)
					{
						SendBotSpawnMessage(g.m_PlayerManager.m_Player[j].m_NetId);
					}
					if (g.m_PlayerManager.m_Player[j].m_RequestResurrect)
					{
						SendBotResurrectMessage(g.m_PlayerManager.m_Player[j].m_NetId);
					}
					if (g.m_PlayerManager.m_Player[j].m_AnimChanged)
					{
						SendBotAnimChangedMessage(g.m_PlayerManager.m_Player[j].m_NetId);
					}
					if (updatesSinceBotPacket >= 4)
					{
						SendBotData(g.m_PlayerManager.m_Player[j].m_NetId);
					}
					flag = true;
				}
			}
			if (flag)
			{
				if (updatesSinceBotPacket >= 4)
				{
					updatesSinceBotPacket = 0;
				}
				else
				{
					updatesSinceBotPacket++;
				}
			}
			if (g.m_App.m_RequestIntermission)
			{
				((BinaryWriter)(object)packetWriter).Write((byte)23);
				((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)3);
				g.m_App.m_RequestIntermission = false;
			}
			if (g.m_App.m_RequestDeleteBotId != 255)
			{
				((BinaryWriter)(object)packetWriter).Write((byte)26);
				((BinaryWriter)(object)packetWriter).Write(g.m_App.m_RequestDeleteBotId);
				((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)3);
				g.m_App.m_RequestDeleteBotId = 255;
			}
		}
		ProcessLocalPlayerInput();
		GamerCollectionEnumerator<NetworkGamer> enumerator = networkSession.AllGamers.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				NetworkGamer current = enumerator.Current;
				if (((Gamer)current).Tag is Player { m_Id: not -1 } player2 && current.IsLocal)
				{
					if (player2.m_RequestCleanItems)
					{
						SendCleanItemsMessage();
					}
					if (player2.m_RequestSendTeam)
					{
						SendTeamChangedMessage();
					}
					if (player2.m_RequestSendClass)
					{
						SendClassChangedMessage();
					}
					if (player2.m_bRequestDied)
					{
						SendLocalShipDeath();
					}
					if (player2.m_bFired)
					{
						SendFireMessage();
					}
					if (player2.m_bTorchChanged)
					{
						SendTorchChangedMessage();
					}
					if (player2.m_bRequestSendDamage)
					{
						SendDamageMessage();
					}
					if (player2.m_bRequestStaked)
					{
						SendLocalShipStaked();
					}
					if (player2.m_bWeaponChanged)
					{
						SendWeaponChangedMessage();
					}
					if (player2.m_bRequestSendSpawn)
					{
						SendSpawnMessage();
					}
					if (player2.m_RequestResurrect)
					{
						SendResurrectMessage();
					}
					if (player2.m_RequestSendCrouch)
					{
						SendCrouchChangedMessage();
					}
					if (player2.m_RequestSendScore)
					{
						SendScoreChangedMessage();
					}
					if (player2.m_AnimChanged)
					{
						SendAnimChangedMessage();
					}
					if (player2.m_RequestFeed)
					{
						SendFeedMessage();
					}
					if (player2.m_RequestRankUp)
					{
						SendRankUpMessage();
					}
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		if (updatesSinceStatusPacket >= 4)
		{
			updatesSinceStatusPacket = 0;
			SendLocalShipData();
		}
		else
		{
			updatesSinceStatusPacket++;
		}
	}

	private void ProcessLocalPlayerInput()
	{
		if (networkSession != null)
		{
			_ = ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count;
			_ = 0;
		}
	}

	private void SendCreateBot()
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		if (networkSession != null && ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count > 0 && networkSession.IsHost)
		{
			if (((Gamer)((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0]).Tag is Player { m_Id: not -1 })
			{
				Player player2 = g.m_PlayerManager.Create(g.m_PlayerManager.GetNextBotId(), bot: true);
				((BinaryWriter)(object)packetWriter).Write((byte)29);
				packetWriter.Write(Vector3.Zero);
				((BinaryWriter)(object)packetWriter).Write(0f);
				((BinaryWriter)(object)packetWriter).Write(player2.m_NetId);
				((BinaryWriter)(object)packetWriter).Write((byte)player2.GetTeam());
				((BinaryWriter)(object)packetWriter).Write((byte)player2.GetClass());
				((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)3);
			}
		}
		else
		{
			g.m_PlayerManager.Create(g.m_PlayerManager.GetNextBotId(), bot: true);
		}
	}

	private void SendLocalShipData()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (networkSession != null && ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count > 0 && ((Gamer)((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0]).Tag is Player { m_Id: not -1 } player)
		{
			((BinaryWriter)(object)packetWriter).Write((byte)2);
			packetWriter.Write(player.m_Position);
			((BinaryWriter)(object)packetWriter).Write(player.m_Rotation.Y);
			((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)0);
		}
	}

	private void SendBotData(short i)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (networkSession != null && ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count > 0)
		{
			((BinaryWriter)(object)packetWriter).Write((byte)30);
			((BinaryWriter)(object)packetWriter).Write(i);
			packetWriter.Write(g.m_PlayerManager.GetBot(i).m_Position);
			((BinaryWriter)(object)packetWriter).Write(g.m_PlayerManager.GetBot(i).m_Rotation.Y);
			((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)0);
		}
	}

	private void SendLocalShipDeath()
	{
		if (networkSession != null && ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count > 0)
		{
			LocalNetworkGamer val = ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0];
			if (((Gamer)val).Tag is Player { m_Id: not -1 } player)
			{
				((BinaryWriter)(object)packetWriter).Write((byte)7);
				short lastAttackerNetId = player.m_LastAttackerNetId;
				((BinaryWriter)(object)packetWriter).Write(lastAttackerNetId);
				val.SendData(packetWriter, (SendDataOptions)3);
				player.m_bRequestDied = false;
			}
		}
	}

	private void SendBotDeath(short botId)
	{
		if (networkSession != null && ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count > 0)
		{
			((BinaryWriter)(object)packetWriter).Write((byte)34);
			((BinaryWriter)(object)packetWriter).Write(botId);
			((BinaryWriter)(object)packetWriter).Write(g.m_PlayerManager.GetBot(botId).m_LastAttackerNetId);
			((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)3);
			g.m_PlayerManager.GetBot(botId).m_bRequestDied = false;
		}
	}

	private void SendLocalShipStaked()
	{
		if (networkSession != null && ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count > 0)
		{
			LocalNetworkGamer val = ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0];
			if (((Gamer)val).Tag is Player { m_Id: not -1 } player)
			{
				((BinaryWriter)(object)packetWriter).Write((byte)21);
				((BinaryWriter)(object)packetWriter).Write(player.m_LastAttackerNetId);
				val.SendData(packetWriter, (SendDataOptions)3);
				player.m_bRequestStaked = false;
			}
		}
	}

	private void SendBotStaked(short botId)
	{
		if (networkSession != null && ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count > 0)
		{
			((BinaryWriter)(object)packetWriter).Write((byte)42);
			((BinaryWriter)(object)packetWriter).Write(botId);
			((BinaryWriter)(object)packetWriter).Write(g.m_PlayerManager.GetBot(botId).m_LastAttackerNetId);
			((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)3);
			g.m_PlayerManager.GetBot(botId).m_bRequestStaked = false;
		}
	}

	private void SendFireMessage()
	{
		if (networkSession != null && ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count > 0)
		{
			LocalNetworkGamer val = ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0];
			if (((Gamer)val).Tag is Player player)
			{
				((BinaryWriter)(object)packetWriter).Write((byte)10);
				((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)1);
				player.m_bFired = false;
			}
		}
	}

	private void SendBotFireMessage(short botId)
	{
		if (networkSession != null && ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count > 0)
		{
			((BinaryWriter)(object)packetWriter).Write((byte)31);
			((BinaryWriter)(object)packetWriter).Write(botId);
			((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)1);
			g.m_PlayerManager.GetBot(botId).m_bFired = false;
		}
	}

	private void SendTorchChangedMessage()
	{
		if (networkSession != null && ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count > 0)
		{
			LocalNetworkGamer val = ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0];
			if (((Gamer)val).Tag is Player player)
			{
				((BinaryWriter)(object)packetWriter).Write((byte)11);
				((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)1);
				player.m_bTorchChanged = false;
			}
		}
	}

	private void SendBotTorchChangedMessage(short botId)
	{
		if (networkSession != null && ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count > 0)
		{
			((BinaryWriter)(object)packetWriter).Write((byte)32);
			((BinaryWriter)(object)packetWriter).Write(botId);
			((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)1);
			g.m_PlayerManager.GetBot(botId).m_bTorchChanged = false;
		}
	}

	private void SendDamageMessage()
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if (networkSession != null && ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count > 0)
		{
			LocalNetworkGamer val = ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0];
			if (((Gamer)val).Tag is Player player)
			{
				((BinaryWriter)(object)packetWriter).Write((byte)12);
				((BinaryWriter)(object)packetWriter).Write(player.m_RequestedDamageAmount);
				((BinaryWriter)(object)packetWriter).Write(player.m_RequestedPlayerToDamageNetID);
				((BinaryWriter)(object)packetWriter).Write(player.m_RequestedHitZone);
				packetWriter.Write(player.m_RequestedHitPos);
				((BinaryWriter)(object)packetWriter).Write(player.m_RequestedAttacker);
				((BinaryWriter)(object)packetWriter).Write(player.m_RequestedStaked);
				((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)3);
				player.m_bRequestSendDamage = false;
			}
		}
	}

	private void SendBotDamageMessage(short botId)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		((BinaryWriter)(object)packetWriter).Write((byte)33);
		((BinaryWriter)(object)packetWriter).Write(botId);
		((BinaryWriter)(object)packetWriter).Write(g.m_PlayerManager.GetBot(botId).m_RequestedDamageAmount);
		((BinaryWriter)(object)packetWriter).Write(g.m_PlayerManager.GetBot(botId).m_RequestedPlayerToDamageNetID);
		((BinaryWriter)(object)packetWriter).Write(g.m_PlayerManager.GetBot(botId).m_RequestedHitZone);
		packetWriter.Write(g.m_PlayerManager.GetBot(botId).m_RequestedHitPos);
		((BinaryWriter)(object)packetWriter).Write(g.m_PlayerManager.GetBot(botId).m_RequestedAttacker);
		((BinaryWriter)(object)packetWriter).Write(g.m_PlayerManager.GetBot(botId).m_RequestedStaked);
		((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)3);
		g.m_PlayerManager.GetBot(botId).m_bRequestSendDamage = false;
	}

	private void SendWeaponChangedMessage()
	{
		if (networkSession != null && ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count > 0)
		{
			LocalNetworkGamer val = ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0];
			if (((Gamer)val).Tag is Player player)
			{
				((BinaryWriter)(object)packetWriter).Write((byte)13);
				((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)3);
				player.m_bWeaponChanged = false;
			}
		}
	}

	private void SendBotWeaponChangedMessage(short botId)
	{
		if (networkSession != null && ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count > 0)
		{
			((BinaryWriter)(object)packetWriter).Write((byte)35);
			((BinaryWriter)(object)packetWriter).Write(botId);
			((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)3);
			g.m_PlayerManager.GetBot(botId).m_bWeaponChanged = false;
		}
	}

	private void SendSpawnMessage()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (networkSession != null && ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count > 0)
		{
			LocalNetworkGamer val = ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0];
			if (((Gamer)val).Tag is Player player)
			{
				((BinaryWriter)(object)packetWriter).Write((byte)14);
				packetWriter.Write(player.m_Position);
				((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)3);
				player.m_bRequestSendSpawn = false;
			}
		}
	}

	private void SendBotSpawnMessage(short botId)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (networkSession != null && ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count > 0)
		{
			((BinaryWriter)(object)packetWriter).Write((byte)36);
			((BinaryWriter)(object)packetWriter).Write(botId);
			packetWriter.Write(g.m_PlayerManager.GetBot(botId).m_Position);
			((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)3);
			g.m_PlayerManager.GetBot(botId).m_bRequestSendSpawn = false;
		}
	}

	private void SendResurrectMessage()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (networkSession != null && ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count > 0)
		{
			LocalNetworkGamer val = ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0];
			if (((Gamer)val).Tag is Player player)
			{
				((BinaryWriter)(object)packetWriter).Write((byte)22);
				packetWriter.Write(player.m_Position);
				((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)1);
				player.m_RequestResurrect = false;
			}
		}
	}

	private void SendBotResurrectMessage(short botId)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (networkSession != null && ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count > 0)
		{
			((BinaryWriter)(object)packetWriter).Write((byte)43);
			((BinaryWriter)(object)packetWriter).Write(botId);
			packetWriter.Write(g.m_PlayerManager.GetBot(botId).m_Position);
			((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)1);
			g.m_PlayerManager.GetBot(botId).m_RequestResurrect = false;
		}
	}

	private void SendCrouchChangedMessage()
	{
		if (networkSession != null && ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count > 0)
		{
			LocalNetworkGamer val = ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0];
			if (((Gamer)val).Tag is Player player)
			{
				((BinaryWriter)(object)packetWriter).Write((byte)15);
				((BinaryWriter)(object)packetWriter).Write(player.m_Crouch);
				((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)1);
				player.m_RequestSendCrouch = false;
			}
		}
	}

	private void SendTeamChangedMessage()
	{
		if (networkSession != null && ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count > 0)
		{
			LocalNetworkGamer val = ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0];
			if (((Gamer)val).Tag is Player player)
			{
				((BinaryWriter)(object)packetWriter).Write((byte)18);
				((BinaryWriter)(object)packetWriter).Write((byte)player.m_Team);
				((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)3);
				player.m_RequestSendTeam = false;
			}
		}
	}

	private void SendBotTeamChangedMessage(short botId)
	{
		if (networkSession != null && ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count > 0)
		{
			((BinaryWriter)(object)packetWriter).Write((byte)39);
			((BinaryWriter)(object)packetWriter).Write(botId);
			((BinaryWriter)(object)packetWriter).Write((byte)g.m_PlayerManager.GetBot(botId).m_Team);
			((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)3);
			g.m_PlayerManager.GetBot(botId).m_RequestSendTeam = false;
		}
	}

	private void SendClassChangedMessage()
	{
		if (networkSession != null && ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count > 0)
		{
			LocalNetworkGamer val = ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0];
			if (((Gamer)val).Tag is Player player)
			{
				((BinaryWriter)(object)packetWriter).Write((byte)19);
				((BinaryWriter)(object)packetWriter).Write((byte)player.m_Class);
				((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)3);
				player.m_RequestSendClass = false;
			}
		}
	}

	private void SendBotClassChangedMessage(short botId)
	{
		if (networkSession != null && ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count > 0)
		{
			((BinaryWriter)(object)packetWriter).Write((byte)40);
			((BinaryWriter)(object)packetWriter).Write(botId);
			((BinaryWriter)(object)packetWriter).Write((byte)g.m_PlayerManager.GetBot(botId).m_Class);
			((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)3);
			g.m_PlayerManager.GetBot(botId).m_RequestSendClass = false;
		}
	}

	private void SendScoreChangedMessage()
	{
		if (networkSession != null && ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count > 0)
		{
			LocalNetworkGamer val = ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0];
			if (((Gamer)val).Tag is Player player)
			{
				((BinaryWriter)(object)packetWriter).Write((byte)16);
				((BinaryWriter)(object)packetWriter).Write(player.m_Kills);
				((BinaryWriter)(object)packetWriter).Write(player.m_Deaths);
				((BinaryWriter)(object)packetWriter).Write(player.m_Rank);
				((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)1);
				player.m_RequestSendScore = false;
			}
		}
	}

	private void SendAnimChangedMessage()
	{
		if (networkSession != null && ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count > 0)
		{
			LocalNetworkGamer val = ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0];
			if (((Gamer)val).Tag is Player player)
			{
				((BinaryWriter)(object)packetWriter).Write((byte)20);
				((BinaryWriter)(object)packetWriter).Write(player.m_Anim);
				((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)1);
				player.m_AnimChanged = false;
			}
		}
	}

	private void SendFeedMessage()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (networkSession != null && ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count > 0)
		{
			LocalNetworkGamer val = ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0];
			if (((Gamer)val).Tag is Player player)
			{
				((BinaryWriter)(object)packetWriter).Write((byte)25);
				packetWriter.Write(player.m_RequestFeedPosition);
				((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)1);
				player.m_RequestFeed = false;
			}
		}
	}

	private void SendRankUpMessage()
	{
		if (networkSession != null && ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count > 0)
		{
			LocalNetworkGamer val = ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0];
			if (((Gamer)val).Tag is Player player)
			{
				((BinaryWriter)(object)packetWriter).Write((byte)27);
				((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)0);
				player.m_RequestRankUp = false;
			}
		}
	}

	private void SendCleanItemsMessage()
	{
		if (networkSession != null && ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count > 0)
		{
			LocalNetworkGamer val = ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0];
			if (((Gamer)val).Tag is Player player)
			{
				((BinaryWriter)(object)packetWriter).Write((byte)28);
				((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)3);
				player.m_RequestCleanItems = false;
			}
		}
	}

	private void SendBotAnimChangedMessage(short botId)
	{
		if (networkSession != null && ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count > 0)
		{
			((BinaryWriter)(object)packetWriter).Write((byte)41);
			((BinaryWriter)(object)packetWriter).Write(botId);
			((BinaryWriter)(object)packetWriter).Write(g.m_PlayerManager.GetBot(botId).m_Anim);
			((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)1);
			g.m_PlayerManager.GetBot(botId).m_AnimChanged = false;
		}
	}

	private void ProcessPackets()
	{
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_08dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0930: Unknown result type (might be due to invalid IL or missing references)
		//IL_0935: Unknown result type (might be due to invalid IL or missing references)
		//IL_0900: Unknown result type (might be due to invalid IL or missing references)
		//IL_0954: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0665: Unknown result type (might be due to invalid IL or missing references)
		//IL_066a: Unknown result type (might be due to invalid IL or missing references)
		//IL_066e: Unknown result type (might be due to invalid IL or missing references)
		//IL_07eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_084b: Unknown result type (might be due to invalid IL or missing references)
		if (networkSession == null || ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count <= 0)
		{
			return;
		}
		NetworkGamer val = default(NetworkGamer);
		while (((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].IsDataAvailable)
		{
			((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].ReceiveData(packetReader, ref val);
			switch ((NetworkSessionComponent.PacketTypes)((BinaryReader)(object)packetReader).ReadByte())
			{
			case NetworkSessionComponent.PacketTypes.PlayerData:
				UpdatePlayerData(val);
				break;
			case NetworkSessionComponent.PacketTypes.Level:
				if (val != null && !val.IsLocal)
				{
					g.m_App.m_Level = ((BinaryReader)(object)packetReader).ReadByte();
					LoadSunburnScene();
				}
				break;
			case NetworkSessionComponent.PacketTypes.CreateBot:
				if (val != null && !val.IsLocal)
				{
					g.m_BotPathManager.LoadBotPath();
					Vector3 position = packetReader.ReadVector3();
					float y = ((BinaryReader)(object)packetReader).ReadSingle();
					short netId2 = ((BinaryReader)(object)packetReader).ReadInt16();
					byte t2 = ((BinaryReader)(object)packetReader).ReadByte();
					byte c2 = ((BinaryReader)(object)packetReader).ReadByte();
					Player player6 = g.m_PlayerManager.Create(netId2, bot: true);
					player6.PeerSetTeam((Player.TEAM)t2);
					player6.PeerSetClass((Player.CLASS)c2);
					player6.m_Position = position;
					player6.m_Rotation.Y = y;
				}
				break;
			case NetworkSessionComponent.PacketTypes.ShipData:
				if (val != null && !val.IsLocal)
				{
					UpdateShipData(val);
				}
				break;
			case NetworkSessionComponent.PacketTypes.BotData:
				if (val != null && !val.IsLocal)
				{
					UpdateBotData(val);
				}
				break;
			case NetworkSessionComponent.PacketTypes.FireWeapon:
				if (val != null && !val.IsLocal && ((Gamer)val).Tag is Player { m_Id: not -1 } player16)
				{
					player16.SimulateFireWeapon();
				}
				break;
			case NetworkSessionComponent.PacketTypes.DoDamage:
				if (val != null && !val.IsLocal && ((Gamer)val).Tag is Player { m_Id: not -1 })
				{
					sbyte damage2 = ((BinaryReader)(object)packetReader).ReadSByte();
					short netId3 = ((BinaryReader)(object)packetReader).ReadInt16();
					byte hitZone2 = ((BinaryReader)(object)packetReader).ReadByte();
					Vector3 vctHitPos2 = packetReader.ReadVector3();
					short attackerNetId2 = ((BinaryReader)(object)packetReader).ReadInt16();
					bool bStaked2 = ((BinaryReader)(object)packetReader).ReadBoolean();
					int playerExistsWithNetId2 = g.m_PlayerManager.GetPlayerExistsWithNetId(netId3);
					if (playerExistsWithNetId2 != -1 && g.m_PlayerManager.m_Player[playerExistsWithNetId2].m_Id != -1)
					{
						g.m_PlayerManager.m_Player[playerExistsWithNetId2].DoDamage(damage2, hitZone2, vctHitPos2, attackerNetId2, bStaked2);
					}
				}
				break;
			case NetworkSessionComponent.PacketTypes.TorchChanged:
				if (val != null && !val.IsLocal && ((Gamer)val).Tag is Player { m_Id: not -1 } player2)
				{
					player2.ToggleTorchLight();
				}
				break;
			case NetworkSessionComponent.PacketTypes.WeaponChanged:
				if (val != null && !val.IsLocal && ((Gamer)val).Tag is Player { m_Id: not -1 } player9)
				{
					player9.PeerChangeWeapon();
				}
				break;
			case NetworkSessionComponent.PacketTypes.Spawned:
				if (val != null && !val.IsLocal && ((Gamer)val).Tag is Player { m_Id: not -1 } player17)
				{
					Vector3 pos5 = packetReader.ReadVector3();
					player17.PeerSpawned(pos5);
				}
				break;
			case NetworkSessionComponent.PacketTypes.Resurrect:
				if (val != null && !val.IsLocal && ((Gamer)val).Tag is Player { m_Id: not -1 } player11)
				{
					Vector3 pos2 = packetReader.ReadVector3();
					player11.PeerResurrect(pos2);
				}
				break;
			case NetworkSessionComponent.PacketTypes.Crouch:
				if (val != null && !val.IsLocal && ((Gamer)val).Tag is Player { m_Id: not -1 } player5)
				{
					bool crouch = ((BinaryReader)(object)packetReader).ReadBoolean();
					player5.PeerCrouch(crouch);
				}
				break;
			case NetworkSessionComponent.PacketTypes.Score:
				if (val != null && !val.IsLocal && ((Gamer)val).Tag is Player { m_Id: not -1 } player13)
				{
					player13.m_Kills = ((BinaryReader)(object)packetReader).ReadInt32();
					player13.m_Deaths = ((BinaryReader)(object)packetReader).ReadInt32();
					player13.m_Rank = ((BinaryReader)(object)packetReader).ReadInt32();
				}
				break;
			case NetworkSessionComponent.PacketTypes.WeaponType:
				if (val != null && !val.IsLocal && ((Gamer)val).Tag is Player { m_Id: not -1 } player8)
				{
					int weaponByType = ((BinaryReader)(object)packetReader).ReadInt32();
					player8.SetWeaponByType(weaponByType);
				}
				break;
			case NetworkSessionComponent.PacketTypes.Team:
				if (val != null && !val.IsLocal && ((Gamer)val).Tag is Player { m_Id: not -1 } player3)
				{
					byte t = ((BinaryReader)(object)packetReader).ReadByte();
					player3.PeerSetTeam((Player.TEAM)t);
				}
				break;
			case NetworkSessionComponent.PacketTypes.Class:
				if (val != null && !val.IsLocal && ((Gamer)val).Tag is Player { m_Id: not -1 } player15)
				{
					byte c3 = ((BinaryReader)(object)packetReader).ReadByte();
					player15.PeerSetClass((Player.CLASS)c3);
				}
				break;
			case NetworkSessionComponent.PacketTypes.Anim:
				if (val != null && !val.IsLocal && ((Gamer)val).Tag is Player { m_Id: not -1 } player10)
				{
					byte anim2 = ((BinaryReader)(object)packetReader).ReadByte();
					player10.PeerSetAnim(anim2);
				}
				break;
			case NetworkSessionComponent.PacketTypes.Feed:
				if (val != null && !val.IsLocal && ((Gamer)val).Tag is Player { m_Id: not -1 } player7)
				{
					Vector3 pos = packetReader.ReadVector3();
					player7.PeerFeed(pos);
				}
				break;
			case NetworkSessionComponent.PacketTypes.RankUp:
				if (val != null && !val.IsLocal && ((Gamer)val).Tag is Player { m_Id: not -1 } player)
				{
					player.PeerRankUp();
				}
				break;
			case NetworkSessionComponent.PacketTypes.CleanItems:
				if (val != null && !val.IsLocal && ((Gamer)val).Tag is Player { m_Id: not -1 } player14)
				{
					player14.CleanItems();
				}
				break;
			case NetworkSessionComponent.PacketTypes.BotFireWeapon:
				if (val != null && !val.IsLocal)
				{
					short botId9 = ((BinaryReader)(object)packetReader).ReadInt16();
					if (g.m_PlayerManager.BotExists(botId9))
					{
						g.m_PlayerManager.GetBot(botId9).SimulateFireWeapon();
					}
				}
				break;
			case NetworkSessionComponent.PacketTypes.BotTorchChanged:
				if (val != null && !val.IsLocal)
				{
					short botId6 = ((BinaryReader)(object)packetReader).ReadInt16();
					if (g.m_PlayerManager.BotExists(botId6))
					{
						g.m_PlayerManager.GetBot(botId6).ToggleTorchLight();
					}
				}
				break;
			case NetworkSessionComponent.PacketTypes.BotDoDamage:
				if (val != null && !val.IsLocal && ((Gamer)val).Tag is Player { m_Id: not -1 })
				{
					((BinaryReader)(object)packetReader).ReadInt16();
					sbyte damage = ((BinaryReader)(object)packetReader).ReadSByte();
					short netId = ((BinaryReader)(object)packetReader).ReadInt16();
					byte hitZone = ((BinaryReader)(object)packetReader).ReadByte();
					Vector3 vctHitPos = packetReader.ReadVector3();
					short attackerNetId = ((BinaryReader)(object)packetReader).ReadInt16();
					bool bStaked = ((BinaryReader)(object)packetReader).ReadBoolean();
					int playerExistsWithNetId = g.m_PlayerManager.GetPlayerExistsWithNetId(netId);
					if (playerExistsWithNetId != -1 && g.m_PlayerManager.m_Player[playerExistsWithNetId].m_Id != -1)
					{
						g.m_PlayerManager.m_Player[playerExistsWithNetId].DoDamage(damage, hitZone, vctHitPos, attackerNetId, bStaked);
					}
				}
				break;
			case NetworkSessionComponent.PacketTypes.BotDeath:
				KillBot(val);
				break;
			case NetworkSessionComponent.PacketTypes.BotStaked:
				BotStaked(val);
				break;
			case NetworkSessionComponent.PacketTypes.BotWeaponChanged:
				if (val != null && !val.IsLocal)
				{
					short botId2 = ((BinaryReader)(object)packetReader).ReadInt16();
					if (g.m_PlayerManager.BotExists(botId2))
					{
						g.m_PlayerManager.GetBot(botId2).PeerChangeWeapon();
					}
				}
				break;
			case NetworkSessionComponent.PacketTypes.BotSpawned:
				if (val != null && !val.IsLocal)
				{
					short botId11 = ((BinaryReader)(object)packetReader).ReadInt16();
					Vector3 pos4 = packetReader.ReadVector3();
					if (g.m_PlayerManager.BotExists(botId11))
					{
						g.m_PlayerManager.GetBot(botId11).PeerSpawned(pos4);
					}
				}
				break;
			case NetworkSessionComponent.PacketTypes.BotResurrect:
				if (val != null && !val.IsLocal)
				{
					short botId10 = ((BinaryReader)(object)packetReader).ReadInt16();
					Vector3 pos3 = packetReader.ReadVector3();
					if (g.m_PlayerManager.BotExists(botId10))
					{
						g.m_PlayerManager.GetBot(botId10).PeerResurrect(pos3);
					}
				}
				break;
			case NetworkSessionComponent.PacketTypes.BotScore:
				if (val != null && !val.IsLocal)
				{
					short botId8 = ((BinaryReader)(object)packetReader).ReadInt16();
					if (g.m_PlayerManager.BotExists(botId8))
					{
						g.m_PlayerManager.GetBot(botId8).m_Kills = ((BinaryReader)(object)packetReader).ReadInt32();
						g.m_PlayerManager.GetBot(botId8).m_Deaths = ((BinaryReader)(object)packetReader).ReadInt32();
						g.m_PlayerManager.GetBot(botId8).m_Rank = ((BinaryReader)(object)packetReader).ReadInt32();
					}
				}
				break;
			case NetworkSessionComponent.PacketTypes.BotWeaponType:
				if (val != null && !val.IsLocal)
				{
					short botId7 = ((BinaryReader)(object)packetReader).ReadInt16();
					int weaponByType2 = ((BinaryReader)(object)packetReader).ReadInt32();
					if (g.m_PlayerManager.BotExists(botId7))
					{
						g.m_PlayerManager.GetBot(botId7).SetWeaponByType(weaponByType2);
					}
				}
				break;
			case NetworkSessionComponent.PacketTypes.BotTeam:
				if (val != null && !val.IsLocal)
				{
					short botId5 = ((BinaryReader)(object)packetReader).ReadInt16();
					byte t3 = ((BinaryReader)(object)packetReader).ReadByte();
					if (g.m_PlayerManager.BotExists(botId5))
					{
						g.m_PlayerManager.GetBot(botId5).PeerSetTeam((Player.TEAM)t3);
					}
				}
				break;
			case NetworkSessionComponent.PacketTypes.BotClass:
				if (val != null && !val.IsLocal)
				{
					short botId4 = ((BinaryReader)(object)packetReader).ReadInt16();
					byte c = ((BinaryReader)(object)packetReader).ReadByte();
					if (g.m_PlayerManager.BotExists(botId4))
					{
						g.m_PlayerManager.GetBot(botId4).PeerSetClass((Player.CLASS)c);
					}
				}
				break;
			case NetworkSessionComponent.PacketTypes.BotAnim:
				if (val != null && !val.IsLocal)
				{
					short botId3 = ((BinaryReader)(object)packetReader).ReadInt16();
					byte anim = ((BinaryReader)(object)packetReader).ReadByte();
					if (g.m_PlayerManager.BotExists(botId3))
					{
						g.m_PlayerManager.GetBot(botId3).PeerSetAnim(anim);
					}
				}
				break;
			case NetworkSessionComponent.PacketTypes.ShipDeath:
				KillShip(val);
				break;
			case NetworkSessionComponent.PacketTypes.Staked:
				Staked(val);
				break;
			case NetworkSessionComponent.PacketTypes.Intermission:
				if (val != null && !val.IsLocal)
				{
					g.m_App.m_Intermission = true;
					g.m_App.m_IntermissionTime = (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds + 5f;
				}
				break;
			case NetworkSessionComponent.PacketTypes.DeleteBot:
				if (val != null && !val.IsLocal)
				{
					short botId = ((BinaryReader)(object)packetReader).ReadInt16();
					if (g.m_PlayerManager.BotExists(botId))
					{
						g.m_PlayerManager.GetBot(botId).RequestDelete();
					}
				}
				break;
			}
		}
	}

	private void KillShip(NetworkGamer sender)
	{
		if (sender != null && !sender.IsLocal && ((Gamer)sender).Tag is Player { m_Id: not -1 } player)
		{
			short lastAttackerNetId = ((BinaryReader)(object)packetReader).ReadInt16();
			player.Kill(lastAttackerNetId);
		}
	}

	private void KillBot(NetworkGamer sender)
	{
		if (sender != null && !sender.IsLocal && ((Gamer)sender).Tag is Player { m_Id: not -1 })
		{
			short botId = ((BinaryReader)(object)packetReader).ReadInt16();
			short lastAttackerNetId = ((BinaryReader)(object)packetReader).ReadInt16();
			if (g.m_PlayerManager.BotExists(botId))
			{
				g.m_PlayerManager.GetBot(botId).Kill(lastAttackerNetId);
			}
		}
	}

	private void Staked(NetworkGamer sender)
	{
		if (sender != null && !sender.IsLocal && ((Gamer)sender).Tag is Player { m_Id: not -1 } player)
		{
			short lastAttackerNetId = ((BinaryReader)(object)packetReader).ReadInt16();
			player.Staked(lastAttackerNetId);
		}
	}

	private void BotStaked(NetworkGamer sender)
	{
		if (sender != null && !sender.IsLocal && ((Gamer)sender).Tag is Player { m_Id: not -1 })
		{
			short botId = ((BinaryReader)(object)packetReader).ReadInt16();
			short lastAttackerNetId = ((BinaryReader)(object)packetReader).ReadInt16();
			if (g.m_PlayerManager.BotExists(botId))
			{
				g.m_PlayerManager.GetBot(botId).Staked(lastAttackerNetId);
			}
		}
	}

	private void UpdatePlayerData(NetworkGamer sender)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (networkSession != null && ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count > 0 && sender != null && ((Gamer)sender).Tag is Player { m_Id: not -1 } player)
		{
			player.m_Position = packetReader.ReadVector3();
			player.m_Rotation.Y = ((BinaryReader)(object)packetReader).ReadSingle();
			player.m_NetworkPosition = player.m_Position;
			player.m_NetworkRotation = player.m_Rotation.Y;
		}
	}

	private void UpdateShipData(NetworkGamer sender)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (sender != null && ((Gamer)sender).Tag is Player { m_Id: not -1 } player)
		{
			player.m_NetworkPosition = packetReader.ReadVector3();
			player.m_NetworkRotation = ((BinaryReader)(object)packetReader).ReadSingle();
		}
	}

	private void UpdateBotData(NetworkGamer sender)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (sender != null)
		{
			short botId = ((BinaryReader)(object)packetReader).ReadInt16();
			if (g.m_PlayerManager.BotExists(botId))
			{
				g.m_PlayerManager.GetBot(botId).m_NetworkPosition = packetReader.ReadVector3();
				g.m_PlayerManager.GetBot(botId).m_NetworkRotation = ((BinaryReader)(object)packetReader).ReadSingle();
			}
		}
	}
}
