using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace VampireFPS;

internal class TitleScreen : GameScreen
{
	private ContentManager content;

	private Texture2D backgroundTexture;

	public GamePadState m_TitleGamepadState;

	public KeyboardState m_TitleKeyboardState;

	private bool done;

	public TitleScreen()
	{
		base.TransitionOnTime = TimeSpan.FromSeconds(0.5);
		base.TransitionOffTime = TimeSpan.FromSeconds(0.5);
	}

	public override void LoadContent()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		if (content == null)
		{
			content = new ContentManager((IServiceProvider)((GameComponent)base.ScreenManager).Game.Services, "Content");
		}
		backgroundTexture = content.Load<Texture2D>("backgrounds\\background");
		g.m_App.PreCacheSunburn();
	}

	public override void UnloadContent()
	{
		content.Unload();
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Invalid comparison between Unknown and I4
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Invalid comparison between Unknown and I4
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		if (((KeyboardState)(ref m_TitleKeyboardState)).IsKeyDown((Keys)32) && !done)
		{
			if ((int)g.m_App.m_PlayerOnePadId == -1)
			{
				g.m_App.m_PlayerOnePadId = (PlayerIndex)0;
			}
			base.ScreenManager.AddScreen(new BackgroundScreen(), null);
			base.ScreenManager.AddScreen(new MainMenuScreen(), null);
			g.m_LoadSaveManager.LoadOptions();
			g.m_SoundManager.Play(33);
			done = true;
			ExitScreen();
		}
		for (int i = 0; i < 4; i++)
		{
			m_TitleGamepadState = GamePad.GetState((PlayerIndex)i);
			m_TitleKeyboardState = Keyboard.GetState((PlayerIndex)i);
			if ((((GamePadState)(ref m_TitleGamepadState)).IsButtonDown((Buttons)4096) || ((GamePadState)(ref m_TitleGamepadState)).IsButtonDown((Buttons)16)) && !done)
			{
				GamePadCapabilities capabilities = GamePad.GetCapabilities((PlayerIndex)i);
				if ((int)((GamePadCapabilities)(ref capabilities)).GamePadType == 1)
				{
					g.m_App.m_PlayerOnePadId = (PlayerIndex)i;
					base.ScreenManager.AddScreen(new BackgroundScreen(), null);
					base.ScreenManager.AddScreen(new MainMenuScreen(), null);
					g.m_SoundManager.Play(33);
					ExitScreen();
					g.m_LoadSaveManager.LoadOptions();
					done = true;
				}
			}
		}
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen: false);
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		SpriteBatch spriteBatch = base.ScreenManager.SpriteBatch;
		Viewport viewport = ((DrawableGameComponent)base.ScreenManager).GraphicsDevice.Viewport;
		Rectangle val = default(Rectangle);
		((Rectangle)(ref val))._002Ector(0, 0, ((Viewport)(ref viewport)).Width, ((Viewport)(ref viewport)).Height);
		spriteBatch.Begin();
		spriteBatch.Draw(backgroundTexture, val, new Color(base.TransitionAlpha, base.TransitionAlpha, base.TransitionAlpha));
		SpriteFont font = base.ScreenManager.Font;
		Vector2 val2 = default(Vector2);
		((Vector2)(ref val2))._002Ector((float)((Viewport)(ref viewport)).Width, (float)((Viewport)(ref viewport)).Height);
		Vector2 val3 = font.MeasureString("PRESS START");
		Vector2 val4 = (val2 - val3) / 2f;
		spriteBatch.DrawString(font, "PRESS START", val4 + new Vector2(0f, 250f), new Color(base.TransitionAlpha, base.TransitionAlpha, base.TransitionAlpha));
		spriteBatch.End();
	}
}
