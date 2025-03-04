using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace VampireFPS;

public class InputState
{
	public const int MaxInputs = 4;

	public readonly KeyboardState[] CurrentKeyboardStates;

	public readonly GamePadState[] CurrentGamePadStates;

	public readonly KeyboardState[] LastKeyboardStates;

	public readonly GamePadState[] LastGamePadStates;

	public readonly bool[] GamePadWasConnected;

	public TouchCollection TouchState;

	public readonly List<GestureSample> Gestures = new List<GestureSample>();

	public InputState()
	{
		CurrentKeyboardStates = (KeyboardState[])(object)new KeyboardState[4];
		CurrentGamePadStates = (GamePadState[])(object)new GamePadState[4];
		LastKeyboardStates = (KeyboardState[])(object)new KeyboardState[4];
		LastGamePadStates = (GamePadState[])(object)new GamePadState[4];
		GamePadWasConnected = new bool[4];
	}

	public void Update()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 4; i++)
		{
			ref KeyboardState reference = ref LastKeyboardStates[i];
			reference = CurrentKeyboardStates[i];
			ref GamePadState reference2 = ref LastGamePadStates[i];
			reference2 = CurrentGamePadStates[i];
			ref KeyboardState reference3 = ref CurrentKeyboardStates[i];
			reference3 = Keyboard.GetState((PlayerIndex)i);
			ref GamePadState reference4 = ref CurrentGamePadStates[i];
			reference4 = GamePad.GetState((PlayerIndex)i);
			if (((GamePadState)(ref CurrentGamePadStates[i])).IsConnected)
			{
				GamePadWasConnected[i] = true;
			}
		}
		TouchState = TouchPanel.GetState();
		Gestures.Clear();
		while (TouchPanel.IsGestureAvailable)
		{
			Gestures.Add(TouchPanel.ReadGesture());
		}
	}

	public bool IsNewKeyPress(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected I4, but got Unknown
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (controllingPlayer.HasValue)
		{
			playerIndex = (PlayerIndex)(int)controllingPlayer.Value;
			int num = (int)playerIndex;
			if (((KeyboardState)(ref CurrentKeyboardStates[num])).IsKeyDown(key))
			{
				return ((KeyboardState)(ref LastKeyboardStates[num])).IsKeyUp(key);
			}
			return false;
		}
		if (!IsNewKeyPress(key, (PlayerIndex)0, out playerIndex) && !IsNewKeyPress(key, (PlayerIndex)1, out playerIndex) && !IsNewKeyPress(key, (PlayerIndex)2, out playerIndex))
		{
			return IsNewKeyPress(key, (PlayerIndex)3, out playerIndex);
		}
		return true;
	}

	public bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected I4, but got Unknown
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (controllingPlayer.HasValue)
		{
			playerIndex = (PlayerIndex)(int)controllingPlayer.Value;
			int num = (int)playerIndex;
			if (((GamePadState)(ref CurrentGamePadStates[num])).IsButtonDown(button))
			{
				return ((GamePadState)(ref LastGamePadStates[num])).IsButtonUp(button);
			}
			return false;
		}
		if (!IsNewButtonPress(button, (PlayerIndex)0, out playerIndex) && !IsNewButtonPress(button, (PlayerIndex)1, out playerIndex) && !IsNewButtonPress(button, (PlayerIndex)2, out playerIndex))
		{
			return IsNewButtonPress(button, (PlayerIndex)3, out playerIndex);
		}
		return true;
	}

	public bool IsMenuSelect(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
	{
		if (!IsNewKeyPress((Keys)32, controllingPlayer, out playerIndex) && !IsNewKeyPress((Keys)13, controllingPlayer, out playerIndex) && !IsNewButtonPress((Buttons)4096, controllingPlayer, out playerIndex))
		{
			return IsNewButtonPress((Buttons)16, controllingPlayer, out playerIndex);
		}
		return true;
	}

	public bool IsMenuCancel(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
	{
		if (!IsNewKeyPress((Keys)27, controllingPlayer, out playerIndex) && !IsNewButtonPress((Buttons)8192, controllingPlayer, out playerIndex))
		{
			return IsNewButtonPress((Buttons)32, controllingPlayer, out playerIndex);
		}
		return true;
	}

	public bool IsMenuUp(PlayerIndex? controllingPlayer)
	{
		if (!IsNewKeyPress((Keys)38, controllingPlayer, out var playerIndex) && !IsNewButtonPress((Buttons)1, controllingPlayer, out playerIndex))
		{
			return IsNewButtonPress((Buttons)268435456, controllingPlayer, out playerIndex);
		}
		return true;
	}

	public bool IsMenuDown(PlayerIndex? controllingPlayer)
	{
		if (!IsNewKeyPress((Keys)40, controllingPlayer, out var playerIndex) && !IsNewButtonPress((Buttons)2, controllingPlayer, out playerIndex))
		{
			return IsNewButtonPress((Buttons)536870912, controllingPlayer, out playerIndex);
		}
		return true;
	}

	public bool IsPauseGame(PlayerIndex? controllingPlayer)
	{
		if (!IsNewKeyPress((Keys)46, controllingPlayer, out var playerIndex))
		{
			return IsNewButtonPress((Buttons)16, controllingPlayer, out playerIndex);
		}
		return true;
	}
}
