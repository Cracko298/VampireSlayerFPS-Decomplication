using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using SynapseGaming.LightingSystem.Rendering;

namespace VampireFPS;

internal class LoadingScreen : GameScreen
{
	private bool loadingIsSlow;

	private bool otherScreensAreGone;

	private GameScreen[] screensToLoad;

	private Thread backgroundThread;

	private EventWaitHandle backgroundThreadExit;

	private GraphicsDevice graphicsDevice;

	private NetworkSession networkSession;

	private IMessageDisplay messageDisplay;

	private GameTime loadStartTime;

	private TimeSpan loadAnimationTimer;

	private int NUM_TIPS = 15;

	public static string[] LoadingTips = new string[30]
	{
		"Tap left trigger to leap", "", "Tap the right bumper to", "feed off dead bodies", "Using the crucifix makes", "you briefly invulnerable", "A Crossbow shot to the head", "will knock down a vampire", "Vampires are temporarily", "invulnerable if they resurrect",
		"Vampires resurrect with", "reduced health", "Run over slayer bodies", "to collect ammo", "Vampires walk silently", "", "Toggle crouch with B button", "", "Adjust the number of bots", "in the options menu",
		"Adjust the map cycle time", "in the options menu", "Use your stake to kill a", "vampire before they get up", "Bots will leave when real", "players join", "The crucifix will damage", "nearby vampires", "Tap the right bumper to", "toggle your flashlight"
	};

	private int m_CurrentTip = -1;

	private float m_ChangeTipTime;

	private float TIP_TIME = 6f;

	private LoadingScreen(ScreenManager screenManager, bool loadingIsSlow, GameScreen[] screensToLoad)
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		this.loadingIsSlow = loadingIsSlow;
		this.screensToLoad = screensToLoad;
		base.TransitionOnTime = TimeSpan.FromSeconds(0.5);
		if (loadingIsSlow)
		{
			backgroundThread = new Thread(BackgroundWorkerThread);
			backgroundThreadExit = new ManualResetEvent(initialState: false);
			graphicsDevice = ((DrawableGameComponent)screenManager).GraphicsDevice;
			IServiceProvider services = (IServiceProvider)((GameComponent)screenManager).Game.Services;
			networkSession = (NetworkSession)services.GetService(typeof(NetworkSession));
			messageDisplay = (IMessageDisplay)services.GetService(typeof(IMessageDisplay));
			ChooseTip();
		}
	}

	private void ChooseTip()
	{
		int num = 0;
		bool flag = false;
		while (num < 100 && !flag)
		{
			num++;
			int num2 = g.m_App.m_Rand.Next(0, NUM_TIPS) * 2;
			if (num2 != m_CurrentTip)
			{
				flag = true;
				m_CurrentTip = num2;
			}
		}
	}

	public static void Load(ScreenManager screenManager, bool loadingIsSlow, PlayerIndex? controllingPlayer, params GameScreen[] screensToLoad)
	{
		GameScreen[] screens = screenManager.GetScreens();
		foreach (GameScreen gameScreen in screens)
		{
			gameScreen.ExitScreen();
		}
		LoadingScreen screen = new LoadingScreen(screenManager, loadingIsSlow, screensToLoad);
		screenManager.AddScreen(screen, controllingPlayer);
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		if (!otherScreensAreGone)
		{
			return;
		}
		if (backgroundThread != null)
		{
			loadStartTime = gameTime;
			backgroundThread.Start();
		}
		base.ScreenManager.RemoveScreen(this);
		GameScreen[] array = screensToLoad;
		foreach (GameScreen gameScreen in array)
		{
			if (gameScreen != null)
			{
				base.ScreenManager.AddScreen(gameScreen, base.ControllingPlayer);
			}
		}
		if (backgroundThread != null)
		{
			backgroundThreadExit.Set();
			backgroundThread.Join();
		}
		((GameComponent)base.ScreenManager).Game.ResetElapsedTime();
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		if (!SplashScreenGameComponent.DisplayComplete)
		{
			base.Draw(gameTime);
			return;
		}
		if (base.ScreenState == ScreenState.Active && base.ScreenManager.GetScreens().Length == 1)
		{
			otherScreensAreGone = true;
		}
		if (loadingIsSlow)
		{
			((DrawableGameComponent)base.ScreenManager).GraphicsDevice.Clear(Color.Black);
			SpriteBatch spriteBatch = base.ScreenManager.SpriteBatch;
			SpriteFont font = base.ScreenManager.Font;
			string loading = Resources.Loading;
			Viewport viewport = ((DrawableGameComponent)base.ScreenManager).GraphicsDevice.Viewport;
			Vector2 val = default(Vector2);
			((Vector2)(ref val))._002Ector((float)((Viewport)(ref viewport)).Width, (float)((Viewport)(ref viewport)).Height);
			Vector2 val2 = font.MeasureString(loading);
			Vector2 val3 = (val - val2) / 2f + new Vector2(0f, 100f);
			Color val4 = Color.Red * base.TransitionAlpha;
			loadAnimationTimer += gameTime.ElapsedGameTime;
			int count = (int)(loadAnimationTimer.TotalSeconds * 1.0) % 20;
			loading += new string('.', count);
			m_ChangeTipTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (m_ChangeTipTime > TIP_TIME)
			{
				ChooseTip();
				m_ChangeTipTime = 0f;
			}
			Vector2 val5 = font.MeasureString(LoadingTips[m_CurrentTip]);
			Vector2 val6 = (val - val5) / 2f + new Vector2(0f, -100f);
			Color val7 = Color.LightGray * base.TransitionAlpha;
			spriteBatch.Begin();
			spriteBatch.DrawString(font, LoadingTips[m_CurrentTip], val6, val7);
			val5 = font.MeasureString(LoadingTips[m_CurrentTip + 1]);
			val6 = (val - val5) / 2f + new Vector2(0f, -60f);
			spriteBatch.DrawString(font, LoadingTips[m_CurrentTip + 1], val6, val7);
			spriteBatch.DrawString(font, loading, val3, val4);
			spriteBatch.End();
		}
	}

	private void BackgroundWorkerThread()
	{
		long lastTime = Stopwatch.GetTimestamp();
		while (!backgroundThreadExit.WaitOne(33))
		{
			GameTime gameTime = GetGameTime(ref lastTime);
			DrawLoadAnimation(gameTime);
			UpdateNetworkSession();
		}
	}

	private GameTime GetGameTime(ref long lastTime)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		long timestamp = Stopwatch.GetTimestamp();
		long num = timestamp - lastTime;
		lastTime = timestamp;
		TimeSpan timeSpan = TimeSpan.FromTicks(num * 10000000 / Stopwatch.Frequency);
		return new GameTime(loadStartTime.TotalGameTime + timeSpan, timeSpan);
	}

	private void DrawLoadAnimation(GameTime gameTime)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (graphicsDevice == null || graphicsDevice.IsDisposed)
		{
			return;
		}
		try
		{
			graphicsDevice.Clear(Color.Black);
			Draw(gameTime);
			if (messageDisplay != null)
			{
				((IUpdateable)messageDisplay).Update(gameTime);
				((IDrawable)messageDisplay).Draw(gameTime);
			}
			graphicsDevice.Present();
		}
		catch
		{
			graphicsDevice = null;
		}
	}

	private void UpdateNetworkSession()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		if (networkSession == null || (int)networkSession.SessionState == 2)
		{
			return;
		}
		try
		{
			networkSession.Update();
		}
		catch
		{
			networkSession = null;
			g.m_App.m_NetworkSession = null;
		}
	}
}
