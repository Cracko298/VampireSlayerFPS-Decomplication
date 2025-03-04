using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace VampireFPS;

internal class BackgroundScreen : GameScreen
{
	private ContentManager content;

	private Texture2D backgroundTexture;

	public BackgroundScreen()
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
	}

	public override void UnloadContent()
	{
		content.Unload();
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen: false);
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		SpriteBatch spriteBatch = base.ScreenManager.SpriteBatch;
		Viewport viewport = ((DrawableGameComponent)base.ScreenManager).GraphicsDevice.Viewport;
		Rectangle val = default(Rectangle);
		((Rectangle)(ref val))._002Ector(0, 0, ((Viewport)(ref viewport)).Width, ((Viewport)(ref viewport)).Height);
		spriteBatch.Begin();
		spriteBatch.Draw(backgroundTexture, val, new Color(base.TransitionAlpha, base.TransitionAlpha, base.TransitionAlpha));
		spriteBatch.End();
	}
}
