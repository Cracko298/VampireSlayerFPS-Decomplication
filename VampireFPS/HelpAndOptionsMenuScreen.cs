using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace VampireFPS;

internal class HelpAndOptionsMenuScreen : MenuScreen
{
	private Texture2D gradientTexture;

	public HelpAndOptionsMenuScreen()
		: base(Resources.HelpOptions)
	{
		MenuEntry menuEntry = new MenuEntry(Resources.Help);
		menuEntry.Selected += OnChooseHelp;
		base.MenuEntries.Add(menuEntry);
		MenuEntry menuEntry2 = new MenuEntry(Resources.Options);
		menuEntry2.Selected += OnChooseOptions;
		base.MenuEntries.Add(menuEntry2);
		MenuEntry menuEntry3 = new MenuEntry(Resources.Credits);
		menuEntry3.Selected += CreditsMenuEntrySelected;
		base.MenuEntries.Add(menuEntry3);
		MenuEntry menuEntry4 = new MenuEntry(Resources.Back);
		menuEntry4.Selected += OnChooseBack;
		base.MenuEntries.Add(menuEntry4);
	}

	private void OnChooseHelp(object sender, PlayerIndexEventArgs e)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		GameScreen screen = new HelpMenuScreen();
		base.ScreenManager.AddScreen(screen, e.PlayerIndex);
	}

	private void OnChooseOptions(object sender, PlayerIndexEventArgs e)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		GameScreen screen = new OptionsMenuScreen();
		base.ScreenManager.AddScreen(screen, e.PlayerIndex);
	}

	private void CreditsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		GameScreen screen = new CreditsMenuScreen();
		base.ScreenManager.AddScreen(screen, e.PlayerIndex);
	}

	private void OnChooseBack(object sender, PlayerIndexEventArgs e)
	{
		ExitScreen();
	}

	public override void LoadContent()
	{
		ContentManager content = ((GameComponent)base.ScreenManager).Game.Content;
		gradientTexture = content.Load<Texture2D>("sprites\\back");
	}

	public override void Draw(GameTime gameTime)
	{
		_ = base.ScreenManager.SpriteBatch;
		_ = base.ScreenManager.Font;
		base.ScreenManager.FadeBackBufferToBlack(base.TransitionAlpha * 2f / 3f);
		base.Draw(gameTime);
	}
}
