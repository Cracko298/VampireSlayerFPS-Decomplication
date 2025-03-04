using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace VampireFPS;

internal class NetworkBusyScreen : GameScreen
{
	private IAsyncResult asyncResult;

	private Texture2D gradientTexture;

	private Texture2D catTexture;

	public event EventHandler<OperationCompletedEventArgs> OperationCompleted;

	public NetworkBusyScreen(IAsyncResult asyncResult)
	{
		this.asyncResult = asyncResult;
		base.IsPopup = true;
		base.TransitionOnTime = TimeSpan.FromSeconds(0.1);
		base.TransitionOffTime = TimeSpan.FromSeconds(0.2);
	}

	public override void LoadContent()
	{
		ContentManager content = ((GameComponent)base.ScreenManager).Game.Content;
		gradientTexture = content.Load<Texture2D>("sprites\\back");
		catTexture = content.Load<Texture2D>("sprites\\busyicon");
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		if (asyncResult != null && asyncResult.IsCompleted)
		{
			if (this.OperationCompleted != null)
			{
				this.OperationCompleted(this, new OperationCompletedEventArgs(asyncResult));
			}
			ExitScreen();
			asyncResult = null;
		}
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		SpriteBatch spriteBatch = base.ScreenManager.SpriteBatch;
		SpriteFont font = base.ScreenManager.Font;
		string networkBusy = Resources.NetworkBusy;
		Viewport viewport = ((DrawableGameComponent)base.ScreenManager).GraphicsDevice.Viewport;
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector((float)((Viewport)(ref viewport)).Width, (float)((Viewport)(ref viewport)).Height);
		Vector2 val2 = font.MeasureString(networkBusy);
		Vector2 val3 = default(Vector2);
		((Vector2)(ref val3))._002Ector((float)catTexture.Width);
		val2.X = Math.Max(val2.X, val3.X);
		val2.Y += val3.Y + 16f;
		Vector2 val4 = (val - val2) / 2f;
		Rectangle val5 = default(Rectangle);
		((Rectangle)(ref val5))._002Ector((int)val4.X - 32, (int)val4.Y - 16, (int)val2.X + 64, (int)val2.Y + 32);
		Color val6 = Color.White * base.TransitionAlpha;
		spriteBatch.Begin();
		spriteBatch.Draw(gradientTexture, val5, val6);
		spriteBatch.DrawString(font, networkBusy, val4, val6);
		float num = (float)gameTime.TotalGameTime.TotalSeconds * 3f;
		Vector2 val7 = default(Vector2);
		((Vector2)(ref val7))._002Ector(val4.X + val2.X / 2f, val4.Y + val2.Y - val3.Y / 2f);
		spriteBatch.Draw(catTexture, val7, (Rectangle?)null, val6, num, val3 / 2f, 1f, (SpriteEffects)0, 0f);
		spriteBatch.End();
	}
}
