using System;
using Microsoft.Xna.Framework;

namespace AppNamespace;

public class Util
{
	public static Vector3 SmoothRamp(Vector3 From, Vector3 To, float Time, Vector3 Vel)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		CalcAccelReq(From, To, Time, Vel);
		Vector3 val = From - To;
		float num = ((Vector3)(ref val)).Length() / Time;
		if (((Vector3)(ref Vel)).LengthSquared() > num * num)
		{
			((Vector3)(ref Vel)).Normalize();
			Vel *= num;
		}
		return From + Vel;
	}

	public static Vector3 CalcAccelReq(Vector3 start, Vector3 end, float t, Vector3 Vel)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		return (end - start - Vel * t) / (0.5f * t * t);
	}

	public static Vector3 CalcVelocityReq(Vector3 start, Vector3 end, float t, float g)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(0f, 0f - g, 0f);
		return (end - start - 0.5f * val * (t * t)) / t;
	}

	public static Vector2 CalcVelocityReq(Vector2 start, Vector2 end, float t, float g)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(0f, 0f - g);
		return (end - start - 0.5f * val * (t * t)) / t;
	}

	public static float CalcLaunchAngle(float V, float X, float Y, float G, bool bHigh)
	{
		float num = V * V;
		float num2 = num * num - G * (G * X * X + 2f * Y * num);
		if (num2 < 0f)
		{
			return (float)Math.PI / 4f;
		}
		float num3 = (float)Math.Sqrt(num2);
		float num4 = G * X;
		if (bHigh)
		{
			return (float)Math.Atan((num + num3) / num4);
		}
		return (float)Math.Atan((num - num3) / num4);
	}
}
