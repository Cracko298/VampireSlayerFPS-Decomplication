using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace VampireFPS;

internal class TrialModeCounter : GameComponent
{
	private const int TimeOut = 8;

	private bool trialMode = true;

	private bool trialModeTimeout;

	private bool showGuide;

	private Stopwatch stopWatch;

	private Game game;

	public bool IsTrialMode
	{
		get
		{
			if (!trialMode)
			{
				return false;
			}
			return Guide.IsTrialMode;
		}
	}

	public TrialModeCounter(Game game)
		: base(game)
	{
		this.game = game;
	}

	public override void Initialize()
	{
		stopWatch = new Stopwatch();
		stopWatch.Start();
	}

	public override void Update(GameTime gameTime)
	{
		if (!Guide.IsTrialMode)
		{
			((GameComponent)this).Enabled = false;
			return;
		}
		if (stopWatch.Elapsed.Minutes >= 8 && !trialModeTimeout)
		{
			trialModeTimeout = true;
			showGuide = true;
		}
		if (!showGuide || Guide.IsVisible)
		{
			return;
		}
		try
		{
			Guide.BeginShowMessageBox("Time Expired", "The Trial for this community game has\r\nended. You can restart the demo to play\r\nagain, or unlock the game below.\r\n\r\nWould you like to unlock the full game?", (IEnumerable<string>)new string[2] { "Exit Game", "Unlock Game" }, 0, (MessageBoxIcon)3, (AsyncCallback)delegate(IAsyncResult result)
			{
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				int? num = Guide.EndShowMessageBox(result);
				if (num.HasValue && num.Value == 1)
				{
					trialMode = false;
					((GameComponent)this).Enabled = false;
					Guide.ShowMarketplace(g.m_App.m_PlayerOnePadId);
				}
				else
				{
					game.Exit();
				}
			}, (object)null);
			showGuide = false;
		}
		catch
		{
		}
	}
}
