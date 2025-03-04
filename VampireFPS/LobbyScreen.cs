using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;

namespace VampireFPS;

internal class LobbyScreen : GameScreen
{
	private NetworkSession networkSession;

	private Texture2D isReadyTexture;

	private Texture2D hasVoiceTexture;

	private Texture2D isTalkingTexture;

	private Texture2D voiceMutedTexture;

	public LobbyScreen(NetworkSession networkSession)
	{
		this.networkSession = networkSession;
		base.TransitionOnTime = TimeSpan.FromSeconds(0.5);
		base.TransitionOffTime = TimeSpan.FromSeconds(0.5);
	}

	public override void LoadContent()
	{
		ContentManager content = ((GameComponent)base.ScreenManager).Game.Content;
		isReadyTexture = content.Load<Texture2D>("sprites\\chat_ready");
		hasVoiceTexture = content.Load<Texture2D>("sprites\\chat_able");
		isTalkingTexture = content.Load<Texture2D>("sprites\\chat_talking");
		voiceMutedTexture = content.Load<Texture2D>("sprites\\chat_mute");
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Invalid comparison between Unknown and I4
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		if (!base.IsExiting)
		{
			if ((int)networkSession.SessionState == 1)
			{
				LoadingScreen.Load(base.ScreenManager, true, null, new GameplayScreen(networkSession));
			}
			if (networkSession.IsHost && (int)networkSession.SessionState == 0)
			{
				networkSession.StartGame();
			}
		}
	}

	public override void HandleInput(InputState input)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		GamerCollectionEnumerator<LocalNetworkGamer> enumerator = networkSession.LocalGamers.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				LocalNetworkGamer current = enumerator.Current;
				PlayerIndex playerIndex = current.SignedInGamer.PlayerIndex;
				if (input.IsMenuSelect(playerIndex, out var playerIndex2))
				{
					HandleMenuSelect(current);
				}
				else if (input.IsMenuCancel(playerIndex, out playerIndex2))
				{
					HandleMenuCancel(current);
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
	}

	private void HandleMenuSelect(LocalNetworkGamer gamer)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (!((NetworkGamer)gamer).IsReady)
		{
			((NetworkGamer)gamer).IsReady = true;
		}
		else if (((NetworkGamer)gamer).IsHost)
		{
			MessageBoxScreen messageBoxScreen = new MessageBoxScreen(Resources.ConfirmForceStartGame);
			messageBoxScreen.Accepted += ConfirmStartGameMessageBoxAccepted;
			base.ScreenManager.AddScreen(messageBoxScreen, gamer.SignedInGamer.PlayerIndex);
		}
	}

	private void ConfirmStartGameMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if ((int)networkSession.SessionState == 0)
		{
			networkSession.StartGame();
		}
	}

	private void HandleMenuCancel(LocalNetworkGamer gamer)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (((NetworkGamer)gamer).IsReady)
		{
			((NetworkGamer)gamer).IsReady = false;
			return;
		}
		PlayerIndex playerIndex = gamer.SignedInGamer.PlayerIndex;
		NetworkSessionComponent.LeaveSession(base.ScreenManager, playerIndex);
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		SpriteBatch spriteBatch = base.ScreenManager.SpriteBatch;
		SpriteFont font = base.ScreenManager.Font;
		Vector2 position = default(Vector2);
		((Vector2)(ref position))._002Ector(100f, 150f);
		float num = (float)Math.Pow(base.TransitionPosition, 2.0);
		if (base.ScreenState == ScreenState.TransitionOn)
		{
			position.X -= num * 256f;
		}
		else
		{
			position.X += num * 512f;
		}
		spriteBatch.Begin();
		int num2 = 0;
		GamerCollectionEnumerator<NetworkGamer> enumerator = networkSession.AllGamers.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				NetworkGamer current = enumerator.Current;
				DrawGamer(current, position);
				if (++num2 == 8)
				{
					position.X += 433f;
					position.Y = 150f;
				}
				else
				{
					position.Y += (float)base.ScreenManager.Font.LineSpacing;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		string lobby = Resources.Lobby;
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(533f, 80f);
		Vector2 val2 = font.MeasureString(lobby) / 2f;
		Color val3 = new Color(192, 192, 192) * base.TransitionAlpha;
		float num3 = 1.25f;
		val.Y -= num * 100f;
		spriteBatch.DrawString(font, lobby, val, val3, 0f, val2, num3, (SpriteEffects)0, 0f);
		spriteBatch.End();
	}

	private void DrawGamer(NetworkGamer gamer, Vector2 position)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		SpriteBatch spriteBatch = base.ScreenManager.SpriteBatch;
		SpriteFont font = base.ScreenManager.Font;
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(34f, 0f);
		Vector2 val2 = default(Vector2);
		((Vector2)(ref val2))._002Ector(0f, 12f);
		Vector2 val3 = position + val2;
		if (gamer.IsReady)
		{
			spriteBatch.Draw(isReadyTexture, val3, Color.Lime * base.TransitionAlpha);
		}
		val3 += val;
		if (gamer.IsMutedByLocalUser)
		{
			spriteBatch.Draw(voiceMutedTexture, val3, Color.Red * base.TransitionAlpha);
		}
		else if (gamer.IsTalking)
		{
			spriteBatch.Draw(isTalkingTexture, val3, Color.Yellow * base.TransitionAlpha);
		}
		else if (gamer.HasVoice)
		{
			spriteBatch.Draw(hasVoiceTexture, val3, Color.White * base.TransitionAlpha);
		}
		string text = ((Gamer)gamer).Gamertag;
		if (gamer.IsHost)
		{
			text += Resources.HostSuffix;
		}
		Color val4 = (gamer.IsLocal ? Color.Yellow : Color.White);
		spriteBatch.DrawString(font, text, position + val * 2f, val4 * base.TransitionAlpha);
	}
}
