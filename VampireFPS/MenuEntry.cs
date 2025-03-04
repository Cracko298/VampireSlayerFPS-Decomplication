using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VampireFPS;

internal class MenuEntry
{
	private string text;

	private float selectionFade;

	private Vector2 position;

	public string Text
	{
		get
		{
			return text;
		}
		set
		{
			text = value;
		}
	}

	public Vector2 Position
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return position;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			position = value;
		}
	}

	public event EventHandler<PlayerIndexEventArgs> Selected;

	protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (this.Selected != null)
		{
			g.m_SoundManager.Play(33);
			this.Selected(this, new PlayerIndexEventArgs(playerIndex));
		}
	}

	public MenuEntry(string text)
	{
		this.text = text;
	}

	public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
	{
		float num = (float)gameTime.ElapsedGameTime.TotalSeconds * 4f;
		if (isSelected)
		{
			selectionFade = Math.Min(selectionFade + num, 1f);
		}
		else
		{
			selectionFade = Math.Max(selectionFade - num, 0f);
		}
	}

	public virtual void Draw(MenuScreen screen, bool isSelected, GameTime gameTime)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		Color val = (Color)(isSelected ? Color.White : new Color(196, 0, 0));
		_ = gameTime.TotalGameTime.TotalSeconds;
		float num = 1f;
		float num2 = 1f + num * 0.05f * selectionFade;
		val *= screen.TransitionAlpha;
		ScreenManager screenManager = screen.ScreenManager;
		SpriteBatch spriteBatch = screenManager.SpriteBatch;
		SpriteFont font = screenManager.Font;
		Vector2 val2 = default(Vector2);
		((Vector2)(ref val2))._002Ector(0f, (float)(font.LineSpacing / 2));
		spriteBatch.DrawString(font, text, position, val, 0f, val2, num2, (SpriteEffects)0, 0f);
	}

	public virtual int GetHeight(MenuScreen screen)
	{
		return screen.ScreenManager.Font.LineSpacing;
	}

	public virtual int GetWidth(MenuScreen screen)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return (int)screen.ScreenManager.Font.MeasureString(Text).X;
	}
}
