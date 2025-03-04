using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using BEPUphysics;
using BEPUphysics.CollisionTests.CollisionAlgorithms;
using BEPUphysics.Constraints;
using DPSF;
using EasyStorage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Rendering;
using SynapseGaming.LightingSystem.Rendering.Deferred;

namespace VampireFPS;

public class App : Game
{
	private static class Program
	{
		[STAThread]
		private static void Main(string[] args)
		{
			g.m_PlayerManager = new PlayerManager();
			g.m_CameraManager = new CameraManager();
			g.m_ItemManager = new ItemManager();
			g.m_BotPathManager = new BotPathManager();
			g.m_SoundManager = new SoundManager();
			g.m_LoadSaveManager = new LoadSaveManager();
			if (Debugger.IsAttached)
			{
				App app = new App(args);
				try
				{
					g.m_App = app;
					((Game)app).Run();
					return;
				}
				finally
				{
					((IDisposable)app)?.Dispose();
				}
			}
			try
			{
				App app2 = new App(args);
				try
				{
					g.m_App = app2;
					((Game)app2).Run();
				}
				finally
				{
					((IDisposable)app2)?.Dispose();
				}
			}
			catch (Exception exception)
			{
				CrashDebugGame crashDebugGame = new CrashDebugGame(exception);
				try
				{
					((Game)crashDebugGame).Run();
				}
				finally
				{
					((IDisposable)crashDebugGame)?.Dispose();
				}
			}
		}
	}

	public class CrashDebugGame : Game
	{
		private SpriteBatch spriteBatch;

		private SpriteFont font_debug;

		private readonly Exception exception;

		public CrashDebugGame(Exception exception)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			this.exception = exception;
			new GraphicsDeviceManager((Game)(object)this);
			((Game)this).Content.RootDirectory = "Content";
		}

		protected override void LoadContent()
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Expected O, but got Unknown
			font_debug = ((Game)this).Content.Load<SpriteFont>("Fonts/SpriteFont1");
			spriteBatch = new SpriteBatch(((Game)this).GraphicsDevice);
		}

		protected override void Update(GameTime gameTime)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Invalid comparison between Unknown and I4
			GamePadState state = GamePad.GetState((PlayerIndex)0);
			GamePadButtons buttons = ((GamePadState)(ref state)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).Back == 1)
			{
				((Game)this).Exit();
			}
			((Game)this).Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			((Game)this).GraphicsDevice.Clear(Color.Black);
			spriteBatch.Begin();
			spriteBatch.DrawString(font_debug, "**** CRASH LOG ****", new Vector2(40f, 50f), Color.White);
			spriteBatch.DrawString(font_debug, "Press Back to Exit", new Vector2(40f, 70f), Color.White);
			spriteBatch.DrawString(font_debug, $"Exception: {exception.Message}", new Vector2(40f, 90f), Color.White);
			spriteBatch.DrawString(font_debug, $"Stack Trace:\n{exception.StackTrace}", new Vector2(40f, 110f), Color.White);
			spriteBatch.End();
			((Game)this).Draw(gameTime);
		}
	}

	public const int WIDTH = 1024;

	public const int HEIGHT = 576;

	private const float moveScale = 100f;

	public SunBurnCoreSystem sunBurnCoreSystem;

	public FrameBuffers frameBuffers;

	public SplashScreenGameComponent splashScreenGameComponent;

	public SceneState sceneState;

	public SceneInterface sceneInterface;

	public ContentRepository contentRepository;

	public SceneEnvironment environment;

	public Scene scene;

	public GraphicsDeviceManager graphics;

	public ScreenManager screenManager;

	private static readonly string[] preloadAssets = new string[6] { "sprites\\back", "sprites\\busyicon", "sprites\\chat_ready", "sprites\\chat_able", "sprites\\chat_talking", "sprites\\chat_mute" };

	public Space m_Space;

	public GameTime m_GameTime;

	public SpriteFont font;

	public SpriteFont smallfont;

	public NetworkSession m_NetworkSession;

	public Random m_Rand;

	public ParticleSystemManager m_ParticleSystemManager;

	public BEPUDebugDrawer m_BEPUDebugDrawer;

	public bool m_Paused;

	public bool m_SingleStep;

	public bool DEBUG_DRAW_CYLINDER;

	public bool DEBUG_DRAW_HITZONES;

	public bool DEBUG_DRAW_RAGDOLL;

	public bool DEBUG_DRAW_VIEW_MODEL_BB;

	public bool DEBUG_DRAW_FULL_MODEL_BB;

	public bool DEBUG_DRAW_COLLISION;

	public IAsyncSaveDevice saveDevice;

	public bool m_bSaveExists;

	public int m_Level = 1;

	private Texture2D gradientTexture;

	private string[] m_SysMessage = new string[5];

	private float[] m_SysMessageTime = new float[5];

	private Color[] m_SysMessageColour = (Color[])(object)new Color[5];

	public PlayerIndex m_PlayerOnePadId;

	private int m_DoneSunburnStartupRender = 10;

	public Song m_Level1Music;

	public Song m_Level2Music;

	public Song m_MenuMusic;

	public bool m_ShowScoreboard;

	public float m_OptionsHoriz = 5f;

	public float m_OptionsVert = 5f;

	public bool m_OptionsInvertY;

	public bool m_OptionsVibration = true;

	public int m_OptionsBotsSP = 5;

	public int m_OptionsBotsMP = 1;

	public int m_OptionsMapTime = 15;

	public bool m_OptionsBlood = true;

	public float m_OptionsVol = 1f;

	public int m_OptionsMaxPlayers = 6;

	public bool m_RequestIntermission;

	public bool m_Intermission;

	public float m_IntermissionTime;

	public Scene m_Scene1;

	public Scene m_Scene2;

	public SceneEnvironment m_Environment1;

	public SceneEnvironment m_Environment2;

	public Model m_CollisionModel1;

	public Model m_CollisionModel2;

	public Model m_Skybox;

	public short m_RequestDeleteBotId = 255;

	public int m_RumbleFrames;

	public float m_ShowPermissionTime;

	public float m_ShowNotSignedInToLiveTime;

	public float m_ShowNoOnlineSessionPrivilidgeTime;

	private float TEXT_IN = 90f;

	private float STRING_WIDTH = 100f;

	private Vector3 viewPosition = new Vector3(86.5f, 11.2f, 57f);

	private Vector3 viewRotation = new Vector3(-2.2f, 0.16f, 0f);

	private Matrix view = Matrix.Identity;

	private Matrix projection = Matrix.Identity;

	public App(string[] args)
	{
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Expected O, but got Unknown
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Expected O, but got Unknown
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Expected O, but got Unknown
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Expected O, but got Unknown
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Expected O, but got Unknown
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Expected O, but got Unknown
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Expected O, but got Unknown
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Expected O, but got Unknown
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Expected O, but got Unknown
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		InitBEPU();
		graphics = new GraphicsDeviceManager((Game)(object)this);
		((Game)this).Content.RootDirectory = "Content";
		graphics.PreferredDepthStencilFormat = (DepthFormat)3;
		graphics.PreferredBackBufferWidth = 1280;
		graphics.PreferredBackBufferHeight = 720;
		graphics.PreferMultiSampling = true;
		splashScreenGameComponent = new SplashScreenGameComponent((Game)(object)this);
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)(object)splashScreenGameComponent);
		sunBurnCoreSystem = new SunBurnCoreSystem((IServiceProvider)((Game)this).Services, ((Game)this).Content);
		sceneState = new SceneState();
		sceneInterface = new SceneInterface();
		sceneInterface.CreateDefaultManagers((RenderingSystemType)1, false);
		frameBuffers = new FrameBuffers(1024, 576, (DetailPreference)1, (DetailPreference)1);
		sceneInterface.ResourceManager.AssignOwnership((IUnloadable)(object)frameBuffers);
		sceneInterface.PostProcessManager.AddPostProcessor((IPostProcessor)new HighDynamicRangePostProcessor());
		IRenderManager renderManager = sceneInterface.RenderManager;
		((DeferredRenderManager)((renderManager is DeferredRenderManager) ? renderManager : null)).DepthFillOptimizationEnabled = true;
		IRenderManager renderManager2 = sceneInterface.RenderManager;
		((DeferredRenderManager)((renderManager2 is DeferredRenderManager) ? renderManager2 : null)).OcclusionQueryEnabled = false;
		screenManager = new ScreenManager((Game)(object)this);
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)(object)screenManager);
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)(object)new MessageDisplayComponent((Game)(object)this));
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)new GamerServicesComponent((Game)(object)this));
		EventHandler<InviteAcceptedEventArgs> eventHandler = delegate(object sender, InviteAcceptedEventArgs e)
		{
			NetworkSessionComponent.InviteAccepted(screenManager, e);
		};
		NetworkSession.InviteAccepted += eventHandler;
		m_Rand = new Random();
		m_ParticleSystemManager = new ParticleSystemManager();
		m_ParticleSystemManager.SetPerformanceProfilingIsEnabledForAllParticleSystems(false);
		m_ParticleSystemManager.SetUpdatesPerSecondForAllParticleSystems(40);
		DPSFDefaultSettings.UpdatesPerSecond = 40;
		DPSFDefaultSettings.PerformanceProfilingIsEnabled = false;
		DPSFDefaultSettings.AutoMemoryManagementSettings.MemoryManagementMode = (AutoMemoryManagerModes)0;
		m_Paused = false;
		m_SingleStep = false;
		ClearSystemMessages();
		m_ShowScoreboard = false;
	}

	public bool IsTrialMode()
	{
		return Guide.IsTrialMode;
	}

	public bool CanPurchaseContent(PlayerIndex player)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (Gamer.SignedInGamers[player] != null)
		{
			SignedInGamer val = Gamer.SignedInGamers[player];
			if (val != null)
			{
				return val.Privileges.AllowPurchaseContent;
			}
			return false;
		}
		return false;
	}

	private void InitBEPU()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		m_Space = new Space();
		SolverSettings.DefaultMinimumIterations = 0;
		m_Space.Solver.IterationLimit = 4;
		GeneralConvexPairTester.UseSimplexCaching = true;
		SolverSettings.DefaultMinimumImpulse = 0.01f;
		m_Space.ThreadManager.AddThread((Action<object>)delegate
		{
			Thread.CurrentThread.SetProcessorAffinity(new int[1] { 3 });
		}, (object)null);
		m_Space.ThreadManager.AddThread((Action<object>)delegate
		{
			Thread.CurrentThread.SetProcessorAffinity(new int[1] { 4 });
		}, (object)null);
		m_Space.ThreadManager.AddThread((Action<object>)delegate
		{
			Thread.CurrentThread.SetProcessorAffinity(new int[1] { 5 });
		}, (object)null);
		m_Space.ForceUpdater.Gravity = new Vector3(0f, -29.43f, 0f);
	}

	protected override void Initialize()
	{
		InitEasyStorage();
		((Game)this).Initialize();
	}

	public void PreCacheSunburn()
	{
		g.m_PlayerManager.LoadModels();
		LoadMusic();
		LoadSFX();
		LoadMisc();
		contentRepository = ((Game)g.m_App).Content.Load<ContentRepository>("Content");
		m_Scene1 = ((Game)g.m_App).Content.Load<Scene>("Scenes/Scene");
		m_Scene2 = ((Game)g.m_App).Content.Load<Scene>("Scenes/Scene2");
		m_Environment1 = ((Game)g.m_App).Content.Load<SceneEnvironment>("Environment/Environment");
		m_Environment2 = ((Game)g.m_App).Content.Load<SceneEnvironment>("Environment/Environment2");
		m_CollisionModel1 = ((Game)g.m_App).Content.Load<Model>("Models\\map3col");
		m_CollisionModel2 = ((Game)g.m_App).Content.Load<Model>("Models\\map4col");
		m_Skybox = ((Game)g.m_App).Content.Load<Model>("Models/nightsky");
	}

	protected override void LoadContent()
	{
		LoadingScreen.Load(screenManager, true, null, new BackgroundScreen(), new TitleScreen());
		m_BEPUDebugDrawer = new BEPUDebugDrawer(((Game)this).GraphicsDevice);
	}

	public void LoadMisc()
	{
		gradientTexture = ((Game)this).Content.Load<Texture2D>("sprites\\back");
		string[] array = preloadAssets;
		foreach (string text in array)
		{
			((Game)this).Content.Load<object>(text);
		}
		font = screenManager.Font;
		smallfont = ((Game)this).Content.Load<SpriteFont>("fonts\\notmarykate_16_drop2_solid");
		g.m_ItemManager.LoadContent(((Game)this).Content);
	}

	public void LoadMusic()
	{
		m_Level1Music = ((Game)this).Content.Load<Song>("Music/HAUNTED_HOUSE_AN_PS034301");
		m_Level2Music = ((Game)this).Content.Load<Song>("Music/Cemetary_Graveyard_Sparse");
		m_MenuMusic = ((Game)this).Content.Load<Song>("Music/UnusExDiscipulisMeis");
	}

	public void LoadSFX()
	{
		g.m_SoundManager.Add(0, ((Game)this).Content.Load<SoundEffect>("Sounds/shotgun1_pump"));
		g.m_SoundManager.Add(1, ((Game)this).Content.Load<SoundEffect>("Sounds/shotgun2_pump"));
		g.m_SoundManager.Add(2, ((Game)this).Content.Load<SoundEffect>("Sounds/shotgun3_pump"));
		g.m_SoundManager.Add(3, ((Game)this).Content.Load<SoundEffect>("Sounds/Shotgun_Reload"));
		g.m_SoundManager.Add(4, ((Game)this).Content.Load<SoundEffect>("Sounds/ricochet1"));
		g.m_SoundManager.Add(5, ((Game)this).Content.Load<SoundEffect>("Sounds/ricochet2"));
		g.m_SoundManager.Add(6, ((Game)this).Content.Load<SoundEffect>("Sounds/ricochet3"));
		g.m_SoundManager.Add(7, ((Game)this).Content.Load<SoundEffect>("Sounds/ricochet4"));
		g.m_SoundManager.Add(8, ((Game)this).Content.Load<SoundEffect>("Sounds/ricochet5"));
		g.m_SoundManager.Add(9, ((Game)this).Content.Load<SoundEffect>("Sounds/ricochet6"));
		g.m_SoundManager.Add(10, ((Game)this).Content.Load<SoundEffect>("Sounds/ricochet7"));
		g.m_SoundManager.Add(11, ((Game)this).Content.Load<SoundEffect>("Sounds/ricochet8"));
		g.m_SoundManager.Add(12, ((Game)this).Content.Load<SoundEffect>("Sounds/ricochet9"));
		g.m_SoundManager.Add(13, ((Game)this).Content.Load<SoundEffect>("Sounds/hit1"));
		g.m_SoundManager.Add(14, ((Game)this).Content.Load<SoundEffect>("Sounds/hit2"));
		g.m_SoundManager.Add(15, ((Game)this).Content.Load<SoundEffect>("Sounds/footsteps_right1"));
		g.m_SoundManager.Add(16, ((Game)this).Content.Load<SoundEffect>("Sounds/footsteps_right2"));
		g.m_SoundManager.Add(17, ((Game)this).Content.Load<SoundEffect>("Sounds/footsteps_right3"));
		g.m_SoundManager.Add(18, ((Game)this).Content.Load<SoundEffect>("Sounds/footsteps_left1"));
		g.m_SoundManager.Add(19, ((Game)this).Content.Load<SoundEffect>("Sounds/footsteps_left2"));
		g.m_SoundManager.Add(20, ((Game)this).Content.Load<SoundEffect>("Sounds/footsteps_left3"));
		g.m_SoundManager.Add(21, ((Game)this).Content.Load<SoundEffect>("Sounds/Mac10_1"));
		g.m_SoundManager.Add(22, ((Game)this).Content.Load<SoundEffect>("Sounds/Mac10_2"));
		g.m_SoundManager.Add(23, ((Game)this).Content.Load<SoundEffect>("Sounds/Mac10_3"));
		g.m_SoundManager.Add(24, ((Game)this).Content.Load<SoundEffect>("Sounds/Mac10_Reload"));
		g.m_SoundManager.Add(32, ((Game)this).Content.Load<SoundEffect>("Sounds/dryfire"));
		g.m_SoundManager.Add(25, ((Game)this).Content.Load<SoundEffect>("Sounds/crossbow_fire1"));
		g.m_SoundManager.Add(26, ((Game)this).Content.Load<SoundEffect>("Sounds/crossbow_fire2"));
		g.m_SoundManager.Add(27, ((Game)this).Content.Load<SoundEffect>("Sounds/crossbow_fire3"));
		g.m_SoundManager.Add(28, ((Game)this).Content.Load<SoundEffect>("Sounds/crossbow_reload"));
		g.m_SoundManager.Add(29, ((Game)this).Content.Load<SoundEffect>("Sounds/stake1"));
		g.m_SoundManager.Add(30, ((Game)this).Content.Load<SoundEffect>("Sounds/stake2"));
		g.m_SoundManager.Add(31, ((Game)this).Content.Load<SoundEffect>("Sounds/stake3"));
		g.m_SoundManager.Add(33, ((Game)this).Content.Load<SoundEffect>("Sounds/select"));
		g.m_SoundManager.Add(34, ((Game)this).Content.Load<SoundEffect>("Sounds/back"));
		g.m_SoundManager.Add(35, ((Game)this).Content.Load<SoundEffect>("Sounds/switch5up"));
		g.m_SoundManager.Add(36, ((Game)this).Content.Load<SoundEffect>("Sounds/switch5down"));
		g.m_SoundManager.Add(37, ((Game)this).Content.Load<SoundEffect>("Sounds/catallus1"));
		g.m_SoundManager.Add(38, ((Game)this).Content.Load<SoundEffect>("Sounds/catallus2"));
		g.m_SoundManager.Add(39, ((Game)this).Content.Load<SoundEffect>("Sounds/catallus3"));
		g.m_SoundManager.Add(40, ((Game)this).Content.Load<SoundEffect>("Sounds/catallus4"));
		g.m_SoundManager.Add(41, ((Game)this).Content.Load<SoundEffect>("Sounds/catallus5"));
		g.m_SoundManager.Add(42, ((Game)this).Content.Load<SoundEffect>("Sounds/catallus6"));
		g.m_SoundManager.Add(43, ((Game)this).Content.Load<SoundEffect>("Sounds/Staked"));
		g.m_SoundManager.Add(44, ((Game)this).Content.Load<SoundEffect>("Sounds/VampAttack1"));
		g.m_SoundManager.Add(45, ((Game)this).Content.Load<SoundEffect>("Sounds/VampAttack2"));
		g.m_SoundManager.Add(46, ((Game)this).Content.Load<SoundEffect>("Sounds/VampFDie"));
		g.m_SoundManager.Add(47, ((Game)this).Content.Load<SoundEffect>("Sounds/VampMDie"));
		g.m_SoundManager.Add(48, ((Game)this).Content.Load<SoundEffect>("Sounds/VampJump"));
		g.m_SoundManager.Add(49, ((Game)this).Content.Load<SoundEffect>("Sounds/Leap1"));
		g.m_SoundManager.Add(50, ((Game)this).Content.Load<SoundEffect>("Sounds/Leap2"));
		g.m_SoundManager.Add(51, ((Game)this).Content.Load<SoundEffect>("Sounds/Leap3"));
		g.m_SoundManager.Add(52, ((Game)this).Content.Load<SoundEffect>("Sounds/ResF"));
		g.m_SoundManager.Add(53, ((Game)this).Content.Load<SoundEffect>("Sounds/ResM"));
		g.m_SoundManager.Add(54, ((Game)this).Content.Load<SoundEffect>("Sounds/Feed"));
		g.m_SoundManager.Add(55, ((Game)this).Content.Load<SoundEffect>("Sounds/VampSwipe"));
		g.m_SoundManager.Add(56, ((Game)this).Content.Load<SoundEffect>("Sounds/Body1"));
		g.m_SoundManager.Add(57, ((Game)this).Content.Load<SoundEffect>("Sounds/Body2"));
		g.m_SoundManager.Add(58, ((Game)this).Content.Load<SoundEffect>("Sounds/Body3"));
		g.m_SoundManager.Add(59, ((Game)this).Content.Load<SoundEffect>("Sounds/Body4"));
		g.m_SoundManager.Add(60, ((Game)this).Content.Load<SoundEffect>("Sounds/Body5"));
		g.m_SoundManager.Add(61, ((Game)this).Content.Load<SoundEffect>("Sounds/Pickup"));
	}

	protected override void UnloadContent()
	{
		sceneInterface.Unload();
		sunBurnCoreSystem.Unload();
		environment = null;
	}

	public void InitEasyStorage()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		SharedSaveDevice val = new SharedSaveDevice();
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)(object)val);
		saveDevice = (IAsyncSaveDevice)(object)val;
		((SaveDevice)val).DeviceSelectorCanceled += delegate(object s, SaveDeviceEventArgs e)
		{
			e.Response = (SaveDeviceEventResponse)2;
		};
		((SaveDevice)val).DeviceDisconnected += delegate(object s, SaveDeviceEventArgs e)
		{
			e.Response = (SaveDeviceEventResponse)2;
		};
		((SaveDevice)val).PromptForDevice();
		saveDevice.SaveCompleted += new SaveCompletedEventHandler(saveDevice_SaveCompleted);
	}

	private void saveDevice_SaveCompleted(object sender, FileActionCompletedEventArgs args)
	{
	}

	public void Reset()
	{
		sceneInterface.Clear();
		sceneInterface.Remove((IScene)(object)g.m_App.scene);
		scene = null;
		ClearSystemMessages();
	}

	protected override void Update(GameTime gameTime)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		m_GameTime = gameTime;
		if (!sceneInterface.Editor.EditorAttached)
		{
			KeyboardState state = Keyboard.GetState();
			if (((KeyboardState)(ref state)).IsKeyDown((Keys)27))
			{
				((Game)this).Exit();
			}
		}
		((Game)this).Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (!SplashScreenGameComponent.DisplayComplete)
		{
			((Game)this).Draw(gameTime);
			return;
		}
		m_GameTime = gameTime;
		if (m_DoneSunburnStartupRender > 0)
		{
			m_DoneSunburnStartupRender--;
			sceneState.BeginFrameRendering(g.m_CameraManager.m_ViewMatrix, g.m_CameraManager.m_ProjectionMatrix, gameTime, (ISceneEnvironment)(object)environment, frameBuffers, true);
			sceneInterface.BeginFrameRendering((ISceneState)(object)sceneState);
			sceneInterface.RenderManager.Render();
			sceneInterface.EndFrameRendering();
			sceneState.EndFrameRendering();
		}
		((Game)this).Draw(gameTime);
	}

	public void DrawHud()
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		Player localPlayer = g.m_PlayerManager.GetLocalPlayer();
		if (localPlayer == null)
		{
			return;
		}
		switch (localPlayer.m_State)
		{
		case Player.STATE.InGame:
			screenManager.SpriteBatch.DrawString(font, $"{localPlayer.m_Health}", new Vector2(0f + TEXT_IN, 576f - TEXT_IN), Color.Red);
			if (localPlayer.m_WeaponItemIndex != -1 && g.m_ItemManager.m_Item[localPlayer.m_WeaponItemIndex].m_Id != -1)
			{
				g.m_ItemManager.m_Item[localPlayer.m_WeaponItemIndex].DrawCrosshair(screenManager.SpriteBatch);
				if (g.m_ItemManager.GetWeaponShouldShowAmmo(localPlayer.m_WeaponItemIndex))
				{
					screenManager.SpriteBatch.DrawString(font, $"{g.m_ItemManager.m_Item[localPlayer.m_WeaponItemIndex].m_WeaponAmmoInClip}|{g.m_ItemManager.m_Item[localPlayer.m_WeaponItemIndex].m_WeaponAmmo}", new Vector2(1024f - TEXT_IN - STRING_WIDTH, 576f - TEXT_IN), Color.Red);
				}
			}
			g.m_PlayerManager.DrawTeamMatesNames(screenManager.SpriteBatch);
			break;
		case Player.STATE.LocalDeath:
		{
			float num = localPlayer.m_RespawnTimer - (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds;
			if (localPlayer.m_bStaked || localPlayer.m_Team == Player.TEAM.Hunter)
			{
				screenManager.SpriteBatch.DrawString(g.m_App.smallfont, $"Respawn in {num:0.0} seconds", new Vector2(0f + TEXT_IN, 576f - TEXT_IN), Color.Red);
			}
			break;
		}
		}
		DrawSystemMessages(screenManager.SpriteBatch);
		if (m_ShowScoreboard || m_Intermission)
		{
			DrawScoreboard(screenManager.SpriteBatch);
		}
	}

	public void DrawSystemMessages(SpriteBatch spriteBatch)
	{
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		Color val = default(Color);
		for (int i = 0; i < 5; i++)
		{
			int num = ((Color)(ref m_SysMessageColour[i])).R - 40 * i;
			int num2 = ((Color)(ref m_SysMessageColour[i])).G - 40 * i;
			int num3 = ((Color)(ref m_SysMessageColour[i])).B - 40 * i;
			int num4 = ((Color)(ref m_SysMessageColour[i])).A - 40 * i;
			if (num < 0)
			{
				num = 0;
			}
			if (num2 < 0)
			{
				num2 = 0;
			}
			if (num3 < 0)
			{
				num3 = 0;
			}
			if (num4 < 0)
			{
				num4 = 0;
			}
			((Color)(ref val))._002Ector(num, num2, num3, num4);
			spriteBatch.DrawString(smallfont, m_SysMessage[i], new Vector2(50f, 150f + -30f * (float)i), val);
			if (m_SysMessageTime[i] < (float)m_GameTime.TotalGameTime.TotalSeconds)
			{
				m_SysMessage[i] = "";
			}
		}
	}

	public void DrawScoreboard(SpriteBatch spriteBatch)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		g.m_PlayerManager.SortPlayers();
		Rectangle val = default(Rectangle);
		((Rectangle)(ref val))._002Ector(80, 80, 864, 416);
		spriteBatch.Draw(gradientTexture, val, new Color(255, 255, 255, 224));
		spriteBatch.DrawString(smallfont, "SLAYERS", new Vector2(120f, 110f), Color.Goldenrod);
		spriteBatch.DrawString(smallfont, "KILLS", new Vector2(400f, 110f), Color.Goldenrod);
		spriteBatch.DrawString(smallfont, "DEATHS", new Vector2(510f, 110f), Color.Goldenrod);
		spriteBatch.DrawString(smallfont, "ALL TIME RANK", new Vector2(640f, 110f), Color.Goldenrod);
		int num = 90;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < 10; i++)
		{
			if (g.m_PlayerManager.m_SortedPlayer[i].m_Id != -1 && g.m_PlayerManager.m_SortedPlayer[i].m_Team == Player.TEAM.Hunter)
			{
				spriteBatch.DrawString(smallfont, $"{g.m_PlayerManager.m_SortedPlayer[i].GetName()}", new Vector2(120f, 140f + 30f * (float)num2), Color.White);
				spriteBatch.DrawString(smallfont, $"{g.m_PlayerManager.m_SortedPlayer[i].m_Kills}", new Vector2(410f, 140f + 30f * (float)num2), Color.White);
				spriteBatch.DrawString(smallfont, $"{g.m_PlayerManager.m_SortedPlayer[i].m_Deaths}", new Vector2(520f, 140f + 30f * (float)num2), Color.White);
				spriteBatch.DrawString(smallfont, $"{g.m_PlayerManager.m_SortedPlayer[i].GetNameForRank()}({g.m_PlayerManager.m_SortedPlayer[i].m_Rank})", new Vector2(640f, 140f + 30f * (float)num2), Color.White);
				num2++;
				num += 30;
				num3 += g.m_PlayerManager.m_SortedPlayer[i].m_Kills;
			}
		}
		spriteBatch.DrawString(smallfont, $"{num3}", new Vector2(280f, 110f), Color.White);
		spriteBatch.DrawString(smallfont, "VAMPIRES", new Vector2(120f, (float)(num + 80 + 30)), Color.Goldenrod);
		spriteBatch.DrawString(smallfont, "KILLS", new Vector2(400f, (float)(num + 80 + 30)), Color.Goldenrod);
		spriteBatch.DrawString(smallfont, "DEATHS", new Vector2(510f, (float)(num + 80 + 30)), Color.Goldenrod);
		spriteBatch.DrawString(smallfont, "ALL TIME RANK", new Vector2(640f, (float)(num + 80 + 30)), Color.Goldenrod);
		num2 = 0;
		int num4 = 0;
		for (int j = 0; j < 10; j++)
		{
			if (g.m_PlayerManager.m_SortedPlayer[j].m_Id != -1 && g.m_PlayerManager.m_SortedPlayer[j].m_Team == Player.TEAM.Vampire)
			{
				spriteBatch.DrawString(smallfont, $"{g.m_PlayerManager.m_SortedPlayer[j].GetName()}", new Vector2(120f, (float)(num + 80 + 60) + 30f * (float)num2), Color.White);
				spriteBatch.DrawString(smallfont, $"{g.m_PlayerManager.m_SortedPlayer[j].m_Kills}", new Vector2(410f, (float)(num + 80 + 60) + 30f * (float)num2), Color.White);
				spriteBatch.DrawString(smallfont, $"{g.m_PlayerManager.m_SortedPlayer[j].m_Deaths}", new Vector2(520f, (float)(num + 80 + 60) + 30f * (float)num2), Color.White);
				spriteBatch.DrawString(smallfont, $"{g.m_PlayerManager.m_SortedPlayer[j].GetNameForRank()}({g.m_PlayerManager.m_SortedPlayer[j].m_Rank})", new Vector2(640f, (float)(num + 80 + 60) + 30f * (float)num2), Color.White);
				num2++;
				num4 += g.m_PlayerManager.m_SortedPlayer[j].m_Kills;
			}
		}
		spriteBatch.DrawString(smallfont, $"{num4}", new Vector2(280f, (float)(num + 80 + 30)), Color.White);
	}

	public void AddSystemMessage(string newMsg, Color c)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		for (int num = 4; num > 0; num--)
		{
			m_SysMessage[num] = m_SysMessage[num - 1];
			m_SysMessageTime[num] = m_SysMessageTime[num - 1];
			ref Color reference = ref m_SysMessageColour[num];
			reference = m_SysMessageColour[num - 1];
		}
		m_SysMessage[0] = newMsg;
		m_SysMessageTime[0] = (float)m_GameTime.TotalGameTime.TotalSeconds + 20f;
		m_SysMessageColour[0] = c;
	}

	public void ClearSystemMessages()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 5; i++)
		{
			m_SysMessage[i] = "";
			m_SysMessageTime[i] = 0f;
			ref Color reference = ref m_SysMessageColour[i];
			reference = Color.White;
		}
	}

	public void DebugDraw()
	{
	}

	private Matrix GetViewMatrix()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		Matrix val = Matrix.CreateFromYawPitchRoll(viewRotation.X, viewRotation.Y, viewRotation.Z);
		Vector3 val2 = viewPosition + Vector3.Transform(Vector3.Backward, val);
		return Matrix.CreateLookAt(viewPosition, val2, Vector3.Up);
	}
}
