using System;
using Microsoft.Xna.Framework;

namespace VampireFPS;

internal class PlayerIndexEventArgs : EventArgs
{
	private PlayerIndex playerIndex;

	public PlayerIndex PlayerIndex => playerIndex;

	public PlayerIndexEventArgs(PlayerIndex playerIndex)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		this.playerIndex = playerIndex;
	}
}
