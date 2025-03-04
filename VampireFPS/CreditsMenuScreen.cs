using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace VampireFPS;

internal class CreditsMenuScreen : MenuScreen
{
	private Texture2D gradientTexture;

	public CreditsMenuScreen()
		: base(Resources.VampireSlayerFPS)
	{
		MenuEntry menuEntry = new MenuEntry("");
		menuEntry.Selected += OnChooseBack;
		base.MenuEntries.Add(menuEntry);
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
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		SpriteBatch spriteBatch = base.ScreenManager.SpriteBatch;
		_ = base.ScreenManager.Font;
		base.ScreenManager.FadeBackBufferToBlack(base.TransitionAlpha * 2f / 3f);
		spriteBatch.Begin();
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(90f, 160f);
		spriteBatch.DrawString(g.m_App.font, "Design, Programming, Art and Animation", val, new Color(196f, 0f, 0f) * base.TransitionAlpha);
		val.Y += 60f;
		spriteBatch.DrawString(g.m_App.font, "                   by Mark Gornall", val, new Color(196f, 0f, 0f) * base.TransitionAlpha);
		val.Y += 90f;
		spriteBatch.DrawString(g.m_App.font, "Powered by Sunburn (synapsegaming.com)", val, new Color(196f, 0f, 0f) * base.TransitionAlpha);
		val.Y += 60f;
		spriteBatch.DrawString(g.m_App.font, "Physics by BEPU (bepuphysics.com)", val, new Color(196f, 0f, 0f) * base.TransitionAlpha);
		val.Y += 60f;
		spriteBatch.DrawString(g.m_App.font, "Particles by DPSF (xnaparticles.com)", val, new Color(196f, 0f, 0f) * base.TransitionAlpha);
		val.Y += 60f;
		spriteBatch.DrawString(g.m_App.font, "Animation by SgMotion (sgmotion.codeplex.com)", val, new Color(196f, 0f, 0f) * base.TransitionAlpha);
		val.Y += 60f;
		spriteBatch.DrawString(g.m_App.font, "Catallus 101 performed by A.Z. Foreman", val, new Color(196f, 0f, 0f) * base.TransitionAlpha);
		val.Y += 60f;
		spriteBatch.End();
		base.Draw(gameTime);
	}
}
