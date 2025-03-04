using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace VampireFPS;

internal class MessageBoxScreen : GameScreen
{
	private string message;

	private Texture2D gradientTexture;

	public event EventHandler<PlayerIndexEventArgs> Accepted;

	public event EventHandler<PlayerIndexEventArgs> Cancelled;

	public MessageBoxScreen(string message)
		: this(message, includeUsageText: true)
	{
	}

	public MessageBoxScreen(string message, bool includeUsageText)
	{
		if (includeUsageText)
		{
			this.message = message + Resources.MessageBoxUsage;
		}
		else
		{
			this.message = message;
		}
		base.IsPopup = true;
		base.TransitionOnTime = TimeSpan.FromSeconds(0.2);
		base.TransitionOffTime = TimeSpan.FromSeconds(0.2);
	}

	public override void LoadContent()
	{
		ContentManager content = ((GameComponent)base.ScreenManager).Game.Content;
		gradientTexture = content.Load<Texture2D>("sprites\\back");
	}

	public override void HandleInput(InputState input)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (input.IsMenuSelect(base.ControllingPlayer, out var playerIndex))
		{
			if (this.Accepted != null)
			{
				this.Accepted(this, new PlayerIndexEventArgs(playerIndex));
			}
			g.m_SoundManager.Play(33);
			ExitScreen();
		}
		else if (input.IsMenuCancel(base.ControllingPlayer, out playerIndex))
		{
			if (this.Cancelled != null)
			{
				this.Cancelled(this, new PlayerIndexEventArgs(playerIndex));
			}
			g.m_SoundManager.Play(34);
			ExitScreen();
		}
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		SpriteBatch spriteBatch = base.ScreenManager.SpriteBatch;
		SpriteFont font = base.ScreenManager.Font;
		base.ScreenManager.FadeBackBufferToBlack(base.TransitionAlpha * 2f / 3f);
		Viewport viewport = ((DrawableGameComponent)base.ScreenManager).GraphicsDevice.Viewport;
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector((float)((Viewport)(ref viewport)).Width, (float)((Viewport)(ref viewport)).Height);
		Vector2 val2 = font.MeasureString(message);
		Vector2 val3 = (val - val2) / 2f;
		Rectangle val4 = default(Rectangle);
		((Rectangle)(ref val4))._002Ector((int)val3.X - 32, (int)val3.Y - 16, (int)val2.X + 64, (int)val2.Y + 32);
		Color val5 = Color.White * base.TransitionAlpha;
		spriteBatch.Begin();
		spriteBatch.Draw(gradientTexture, val4, val5);
		spriteBatch.DrawString(font, message, val3, val5);
		spriteBatch.End();
	}
}
