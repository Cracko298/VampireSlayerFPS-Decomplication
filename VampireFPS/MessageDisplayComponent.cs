using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VampireFPS;

internal class MessageDisplayComponent : DrawableGameComponent, IMessageDisplay, IDrawable, IUpdateable
{
	private class NotificationMessage
	{
		public string Text;

		public float Position;

		public TimeSpan Age;

		public NotificationMessage(string text, float position)
		{
			Text = text;
			Position = position;
			Age = TimeSpan.Zero;
		}
	}

	private SpriteBatch spriteBatch;

	private SpriteFont font;

	private List<NotificationMessage> messages = new List<NotificationMessage>();

	private object syncObject = new object();

	private static readonly TimeSpan fadeInTime = TimeSpan.FromSeconds(0.25);

	private static readonly TimeSpan showTime = TimeSpan.FromSeconds(5.0);

	private static readonly TimeSpan fadeOutTime = TimeSpan.FromSeconds(0.5);

	public MessageDisplayComponent(Game game)
		: base(game)
	{
		game.Services.AddService(typeof(IMessageDisplay), (object)this);
	}

	protected override void LoadContent()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		spriteBatch = new SpriteBatch(((DrawableGameComponent)this).GraphicsDevice);
		font = ((GameComponent)this).Game.Content.Load<SpriteFont>("fonts\\notmarykate_16_drop2_solid");
	}

	public override void Update(GameTime gameTime)
	{
		lock (syncObject)
		{
			int num = 0;
			float num2 = 0f;
			while (num < messages.Count)
			{
				NotificationMessage notificationMessage = messages[num];
				float num3 = num2 - notificationMessage.Position;
				float val = (float)gameTime.ElapsedGameTime.TotalSeconds * 2f;
				notificationMessage.Position += num3 * Math.Min(val, 1f);
				notificationMessage.Age += gameTime.ElapsedGameTime;
				if (notificationMessage.Age < showTime + fadeOutTime)
				{
					num++;
					if (notificationMessage.Age < showTime)
					{
						num2 += 1f;
					}
				}
				else
				{
					messages.RemoveAt(num);
				}
			}
		}
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		lock (syncObject)
		{
			if (messages.Count == 0)
			{
				return;
			}
			Viewport viewport = ((DrawableGameComponent)this).GraphicsDevice.Viewport;
			Vector2 val = default(Vector2);
			((Vector2)(ref val))._002Ector((float)(((Viewport)(ref viewport)).Width - 100), 0f);
			spriteBatch.Begin();
			foreach (NotificationMessage message in messages)
			{
				float num = 1f;
				if (message.Age < fadeInTime)
				{
					num = (float)(message.Age.TotalSeconds / fadeInTime.TotalSeconds);
				}
				else if (message.Age > showTime)
				{
					num = (float)((showTime + fadeOutTime - message.Age).TotalSeconds / fadeOutTime.TotalSeconds);
				}
				val.Y = 80f + message.Position * (float)font.LineSpacing * 0.75f;
				Vector2 val2 = font.MeasureString(message.Text);
				val2.Y = 0f;
				spriteBatch.DrawString(font, message.Text, val + Vector2.One, Color.Black * num, 0f, val2, 0.75f, (SpriteEffects)0, 0f);
				spriteBatch.DrawString(font, message.Text, val, Color.White * num, 0f, val2, 0.75f, (SpriteEffects)0, 0f);
			}
			spriteBatch.End();
		}
	}

	public void ShowMessage(string message, params object[] parameters)
	{
		string text = string.Format(message, parameters);
		lock (syncObject)
		{
			float position = messages.Count;
			messages.Add(new NotificationMessage(text, position));
		}
	}
}
