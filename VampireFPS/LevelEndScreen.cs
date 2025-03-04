using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VampireFPS;

internal class LevelEndScreen : MenuScreen
{
	private Texture2D gradientTexture;

	private Texture2D helpTexture;

	public LevelEndScreen()
		: base("")
	{
	}

	public override void LoadContent()
	{
		_ = ((GameComponent)base.ScreenManager).Game.Content;
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		SpriteBatch spriteBatch = base.ScreenManager.SpriteBatch;
		SpriteFont font = base.ScreenManager.Font;
		base.ScreenManager.FadeBackBufferToBlack(base.TransitionAlpha * 2f / 3f);
		spriteBatch.Begin();
		new Vector2(90f, 220f);
		spriteBatch.DrawString(font, "SCOREBOARD ETC", new Vector2(640f, 360f), Color.White);
		spriteBatch.End();
		base.Draw(gameTime);
	}
}
