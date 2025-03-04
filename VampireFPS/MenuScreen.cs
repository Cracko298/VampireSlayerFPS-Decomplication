using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VampireFPS;

internal abstract class MenuScreen : GameScreen
{
	private List<MenuEntry> menuEntries = new List<MenuEntry>();

	public int selectedEntry;

	private string menuTitle;

	protected IList<MenuEntry> MenuEntries => menuEntries;

	public MenuScreen(string menuTitle)
	{
		this.menuTitle = menuTitle;
		base.TransitionOnTime = TimeSpan.FromSeconds(0.5);
		base.TransitionOffTime = TimeSpan.FromSeconds(0.5);
	}

	public override void HandleInput(InputState input)
	{
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		if (input.IsMenuUp(base.ControllingPlayer))
		{
			selectedEntry--;
			if (selectedEntry < 0)
			{
				selectedEntry = menuEntries.Count - 1;
			}
			if (menuEntries.Count > 1)
			{
				g.m_SoundManager.Play(36);
			}
		}
		if (input.IsMenuDown(base.ControllingPlayer))
		{
			selectedEntry++;
			if (selectedEntry >= menuEntries.Count)
			{
				selectedEntry = 0;
			}
			if (menuEntries.Count > 1)
			{
				g.m_SoundManager.Play(35);
			}
		}
		if (input.IsMenuSelect(base.ControllingPlayer, out var playerIndex))
		{
			OnSelectEntry(selectedEntry, playerIndex);
		}
		else if (input.IsMenuCancel(base.ControllingPlayer, out playerIndex))
		{
			OnCancel(playerIndex);
			g.m_SoundManager.Play(34);
		}
	}

	protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		menuEntries[entryIndex].OnSelectEntry(playerIndex);
	}

	protected virtual void OnCancel(PlayerIndex playerIndex)
	{
		ExitScreen();
	}

	protected void OnCancel(object sender, PlayerIndexEventArgs e)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		OnCancel(e.PlayerIndex);
	}

	protected virtual void UpdateMenuEntryLocations()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		Vector2 position = default(Vector2);
		((Vector2)(ref position))._002Ector(0f, 175f);
		for (int i = 0; i < menuEntries.Count; i++)
		{
			MenuEntry menuEntry = menuEntries[i];
			Viewport viewport = ((DrawableGameComponent)base.ScreenManager).GraphicsDevice.Viewport;
			position.X = ((Viewport)(ref viewport)).Width / 2 - menuEntry.GetWidth(this) / 2;
			menuEntry.Position = position;
			position.Y += (float)(menuEntry.GetHeight(this) + 10);
		}
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		for (int i = 0; i < menuEntries.Count; i++)
		{
			bool isSelected = base.IsActive && i == selectedEntry;
			menuEntries[i].Update(this, isSelected, gameTime);
		}
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		UpdateMenuEntryLocations();
		GraphicsDevice graphicsDevice = ((DrawableGameComponent)base.ScreenManager).GraphicsDevice;
		SpriteBatch spriteBatch = base.ScreenManager.SpriteBatch;
		SpriteFont font = base.ScreenManager.Font;
		spriteBatch.Begin();
		for (int i = 0; i < menuEntries.Count; i++)
		{
			MenuEntry menuEntry = menuEntries[i];
			bool isSelected = base.IsActive && i == selectedEntry;
			menuEntry.Draw(this, isSelected, gameTime);
		}
		Viewport viewport = graphicsDevice.Viewport;
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector((float)(((Viewport)(ref viewport)).Width / 2), 80f);
		Vector2 val2 = font.MeasureString(menuTitle) / 2f;
		Color val3 = new Color(255, 255, 0) * base.TransitionAlpha;
		float num = 1.25f;
		spriteBatch.DrawString(font, menuTitle, val, val3, 0f, val2, num, (SpriteEffects)0, 0f);
		spriteBatch.End();
	}
}
