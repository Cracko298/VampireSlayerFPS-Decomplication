using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace VampireFPS;

public class ScreenManager : DrawableGameComponent
{
	private List<GameScreen> screens = new List<GameScreen>();

	private List<GameScreen> screensToUpdate = new List<GameScreen>();

	private InputState input = new InputState();

	private SpriteBatch spriteBatch;

	private SpriteFont font;

	private Texture2D blankTexture;

	private bool isInitialized;

	private bool traceEnabled;

	public SpriteBatch SpriteBatch => spriteBatch;

	public SpriteFont Font
	{
		get
		{
			return font;
		}
		set
		{
			font = value;
		}
	}

	public bool TraceEnabled
	{
		get
		{
			return traceEnabled;
		}
		set
		{
			traceEnabled = value;
		}
	}

	public ScreenManager(Game game)
		: base(game)
	{
		TouchPanel.EnabledGestures = (GestureType)0;
	}

	public override void Initialize()
	{
		((DrawableGameComponent)this).Initialize();
		isInitialized = true;
	}

	protected override void LoadContent()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		ContentManager content = ((GameComponent)this).Game.Content;
		spriteBatch = new SpriteBatch(((DrawableGameComponent)this).GraphicsDevice);
		font = ((GameComponent)this).Game.Content.Load<SpriteFont>("fonts\\notmarykate_26_drop2_solid");
		blankTexture = content.Load<Texture2D>("sprites\\blank");
		foreach (GameScreen screen in screens)
		{
			screen.LoadContent();
		}
	}

	protected override void UnloadContent()
	{
		foreach (GameScreen screen in screens)
		{
			screen.UnloadContent();
		}
	}

	public override void Update(GameTime gameTime)
	{
		input.Update();
		screensToUpdate.Clear();
		foreach (GameScreen screen in screens)
		{
			screensToUpdate.Add(screen);
		}
		bool flag = !((GameComponent)this).Game.IsActive;
		bool coveredByOtherScreen = false;
		while (screensToUpdate.Count > 0)
		{
			GameScreen gameScreen = screensToUpdate[screensToUpdate.Count - 1];
			screensToUpdate.RemoveAt(screensToUpdate.Count - 1);
			gameScreen.Update(gameTime, flag, coveredByOtherScreen);
			if (gameScreen.ScreenState == ScreenState.TransitionOn || gameScreen.ScreenState == ScreenState.Active)
			{
				if (!flag)
				{
					gameScreen.HandleInput(input);
					flag = true;
				}
				if (!gameScreen.IsPopup)
				{
					coveredByOtherScreen = true;
				}
			}
		}
		if (traceEnabled)
		{
			TraceScreens();
		}
	}

	private void TraceScreens()
	{
		List<string> list = new List<string>();
		foreach (GameScreen screen in screens)
		{
			list.Add(screen.GetType().Name);
		}
	}

	public override void Draw(GameTime gameTime)
	{
		foreach (GameScreen screen in screens)
		{
			if (screen.ScreenState != ScreenState.Hidden)
			{
				screen.Draw(gameTime);
			}
		}
	}

	public void AddScreen(GameScreen screen, PlayerIndex? controllingPlayer)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		screen.ControllingPlayer = controllingPlayer;
		screen.ScreenManager = this;
		screen.IsExiting = false;
		if (isInitialized)
		{
			screen.LoadContent();
		}
		screens.Add(screen);
		TouchPanel.EnabledGestures = screen.EnabledGestures;
	}

	public void RemoveScreen(GameScreen screen)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (isInitialized)
		{
			screen.UnloadContent();
		}
		screens.Remove(screen);
		screensToUpdate.Remove(screen);
		if (screens.Count > 0)
		{
			TouchPanel.EnabledGestures = screens[screens.Count - 1].EnabledGestures;
		}
	}

	public GameScreen[] GetScreens()
	{
		return screens.ToArray();
	}

	public void FadeBackBufferToBlack(float alpha)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		Viewport viewport = ((DrawableGameComponent)this).GraphicsDevice.Viewport;
		spriteBatch.Begin();
		spriteBatch.Draw(blankTexture, new Rectangle(0, 0, ((Viewport)(ref viewport)).Width, ((Viewport)(ref viewport)).Height), Color.Black * alpha);
		spriteBatch.End();
	}
}
