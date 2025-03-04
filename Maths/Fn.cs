using System;
using Microsoft.Xna.Framework;

namespace Maths;

public class Fn
{
	public static Vector2 vUp = new Vector2(0f, 1f);

	public static float DegToRad(float deg)
	{
		return deg * ((float)Math.PI / 180f);
	}

	public static void Rotate(ref Vector2 vec, float ang)
	{
		float x = vec.X;
		float num = (float)Math.Cos(0f - ang);
		float num2 = (float)Math.Sin(0f - ang);
		vec.X = vec.X * num - vec.Y * num2;
		vec.Y = x * num2 + vec.Y * num;
	}

	public static float ClerpT(float from, float to, float step, float tol)
	{
		float num = NormRot(to - from) * step;
		if (Math.Abs(num) > tol)
		{
			num = ((!(num < 0f)) ? tol : (0f - tol));
		}
		return from + num;
	}

	public static float Clerp(float from, float to, float step)
	{
		float num = MathHelper.WrapAngle(to - from) * step;
		return from + num;
	}

	public static float DotProduct(Vector2 a, Vector2 b)
	{
		return a.X * b.X + a.Y * b.Y;
	}

	public static Vector2 ClosestPointOnLine(Vector2 l1, Vector2 l2, Vector2 pt)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = l2 - l1;
		Vector2 val2 = pt - l1;
		if (Vector2.Dot(val2, val) < 0f)
		{
			return l1;
		}
		Vector2 val3 = default(Vector2);
		((Vector2)(ref val3))._002Ector(val.X, val.Y);
		((Vector2)(ref val3)).Normalize();
		float num = Vector2.Dot(val2, val3);
		if (num * num > Vector2.Dot(val, val))
		{
			return l2;
		}
		return l1 + val3 * num;
	}

	public static float NormRot(float x)
	{
		if (Math.Abs(x) < (float)Math.PI)
		{
			return x;
		}
		float num = x % ((float)Math.PI * 2f);
		if (Math.Abs(num) < (float)Math.PI)
		{
			return num;
		}
		if (x >= 0f)
		{
			return num - (float)Math.PI * 2f;
		}
		return num + (float)Math.PI * 2f;
	}

	public static float LerpCosT(float from, float to, float step, float tol)
	{
		float num = (to - from) * step;
		if (Math.Abs(num) > tol)
		{
			num = ((!(num < 0f)) ? tol : (0f - tol));
		}
		float num2 = (1f - (float)Math.Cos(num * (float)Math.PI)) * 0.5f;
		return from * (1f - num2) + to * num2;
	}

	public float Signed2DTriArea(Vector2 a, Vector2 b, Vector2 c)
	{
		return (a.X - c.X) * (b.Y - c.Y) - (a.Y - c.Y) * (b.X - c.X);
	}

	public int Test2DSegmentSegment(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t, Vector2 p)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		float num = Signed2DTriArea(a, b, d);
		float num2 = Signed2DTriArea(a, b, c);
		if (num * num2 < 0f)
		{
			float num3 = Signed2DTriArea(c, d, a);
			float num4 = num3 + num2 - num;
			if (num3 * num4 < 0f)
			{
				t = num3 / (num3 - num4);
				p = a + t * (b - a);
				return 1;
			}
		}
		return 0;
	}
}
