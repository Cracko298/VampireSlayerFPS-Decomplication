using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;

namespace VampireFPS;

internal class ProfileSignInScreen : GameScreen
{
	private NetworkSessionType sessionType;

	private bool haveShownGuide;

	private bool haveShownMarketplace;

	public event EventHandler<EventArgs> ProfileSignedIn;

	public ProfileSignInScreen(NetworkSessionType sessionType)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		this.sessionType = sessionType;
		base.IsPopup = true;
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		if (ValidProfileSignedIn())
		{
			if (this.ProfileSignedIn != null)
			{
				this.ProfileSignedIn(this, EventArgs.Empty);
			}
			ExitScreen();
		}
		else
		{
			if (!base.IsActive || Guide.IsVisible)
			{
				return;
			}
			if (g.m_App.IsTrialMode() && NetworkSessionComponent.IsOnlineSessionType(sessionType) && Gamer.SignedInGamers[base.ControllingPlayer.Value] != null && !haveShownMarketplace)
			{
				if (!g.m_App.CanPurchaseContent(base.ControllingPlayer.Value))
				{
					g.m_App.m_ShowPermissionTime = (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds + 5f;
					ExitScreen();
				}
				else
				{
					ShowMarketplace();
					haveShownMarketplace = true;
				}
			}
			else if (!haveShownGuide && !haveShownMarketplace)
			{
				Guide.ShowSignIn(1, NetworkSessionComponent.IsOnlineSessionType(sessionType));
				haveShownGuide = true;
			}
			else
			{
				ExitScreen();
			}
		}
	}

	private bool ValidProfileSignedIn()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		SignedInGamer val = Gamer.SignedInGamers[base.ControllingPlayer.Value];
		if (val == null)
		{
			return false;
		}
		if (NetworkSessionComponent.IsOnlineSessionType(sessionType))
		{
			if (!val.IsSignedInToLive)
			{
				return false;
			}
			if (!val.Privileges.AllowOnlineSessions)
			{
				return false;
			}
		}
		return true;
	}

	private void ShowMarketplace()
	{
		MessageBoxScreen messageBoxScreen = new MessageBoxScreen(Resources.ConfirmMarketplace);
		messageBoxScreen.Accepted += delegate
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			Guide.ShowMarketplace(base.ControllingPlayer.Value);
		};
		base.ScreenManager.AddScreen(messageBoxScreen, base.ControllingPlayer);
	}
}
