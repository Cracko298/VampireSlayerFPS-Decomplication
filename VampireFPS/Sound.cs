using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace VampireFPS;

public struct Sound
{
	public const int MAX_INSTANCES_PER_SOUND = 4;

	private const float SOUND_RANGE_SQ = 16129f;

	private const float PITCH_AMNT = 0.2f;

	public SoundEffectInstance[] m_Instance;

	public int m_Index;

	public void Add(SoundEffect s)
	{
		for (int i = 0; i < 4; i++)
		{
			m_Instance[i] = s.CreateInstance();
		}
	}

	public SoundEffectInstance Play()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		m_Index++;
		if (m_Index >= 4)
		{
			m_Index = 0;
		}
		if ((int)m_Instance[m_Index].State == 0)
		{
			return null;
		}
		m_Instance[m_Index].Volume = 1f;
		m_Instance[m_Index].Play();
		return m_Instance[m_Index];
	}

	public SoundEffectInstance Play(float vol)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		m_Index++;
		if (m_Index >= 4)
		{
			m_Index = 0;
		}
		if ((int)m_Instance[m_Index].State == 0)
		{
			return null;
		}
		m_Instance[m_Index].Volume = vol;
		m_Instance[m_Index].Play();
		return m_Instance[m_Index];
	}

	public SoundEffectInstance Play3D(Vector3 pos)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		m_Index++;
		if (m_Index >= 4)
		{
			m_Index = 0;
		}
		if ((int)m_Instance[m_Index].State == 0)
		{
			return null;
		}
		Vector3 val = pos - g.m_CameraManager.m_Position;
		float num = ((Vector3)(ref val)).LengthSquared();
		float num2 = 1f - num / 16129f;
		num2 = MathHelper.Clamp(num2, 0f, 1f);
		num2 = num2 * num2 * num2;
		if (num2 < 0.01f)
		{
			return null;
		}
		float num3 = (float)Math.Atan2(g.m_CameraManager.m_Position.Z - pos.Z, g.m_CameraManager.m_Position.X - pos.X) + g.m_PlayerManager.GetLocalPlayer().m_Rotation.Y + (float)Math.PI / 2f;
		num3 = MathHelper.WrapAngle(num3);
		num3 = 0f - (float)Math.Sin(num3);
		num3 = MathHelper.Clamp(num3, -1f, 1f);
		m_Instance[m_Index].Play();
		m_Instance[m_Index].Pan = num3;
		m_Instance[m_Index].Volume = num2;
		return m_Instance[m_Index];
	}

	public SoundEffectInstance Play3D(Vector3 pos, float vol)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		m_Index++;
		if (m_Index >= 4)
		{
			m_Index = 0;
		}
		if ((int)m_Instance[m_Index].State == 0)
		{
			return null;
		}
		Vector3 val = pos - g.m_CameraManager.m_Position;
		float num = ((Vector3)(ref val)).LengthSquared();
		float num2 = 1f - num / 16129f;
		num2 = MathHelper.Clamp(num2, 0f, 1f);
		num2 = num2 * num2 * num2;
		if (num2 < 0.01f)
		{
			return null;
		}
		float num3 = (float)Math.Atan2(g.m_CameraManager.m_Position.Z - pos.Z, g.m_CameraManager.m_Position.X - pos.X) + g.m_PlayerManager.GetLocalPlayer().m_Rotation.Y + (float)Math.PI / 2f;
		num3 = MathHelper.WrapAngle(num3);
		num3 = 0f - (float)Math.Sin(num3);
		num3 = MathHelper.Clamp(num3, -1f, 1f);
		m_Instance[m_Index].Play();
		m_Instance[m_Index].Pan = num3;
		m_Instance[m_Index].Volume = num2 * vol;
		return m_Instance[m_Index];
	}

	private float RandPitch()
	{
		return (float)g.m_App.m_Rand.NextDouble() * 0.2f - 0.1f;
	}

	public SoundEffectInstance PlayLooped()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		m_Index++;
		if (m_Index >= 4)
		{
			m_Index = 0;
		}
		if ((int)m_Instance[m_Index].State == 0)
		{
			return null;
		}
		if (!m_Instance[m_Index].IsLooped)
		{
			m_Instance[m_Index].IsLooped = true;
		}
		m_Instance[m_Index].Resume();
		return m_Instance[m_Index];
	}

	public void StopLooped(SoundEffectInstance s)
	{
		for (int i = 0; i < 4; i++)
		{
			if (m_Instance[i] == s)
			{
				m_Instance[i].Stop();
			}
		}
	}

	public void Stop()
	{
		for (int i = 0; i < 4; i++)
		{
			m_Instance[i].Stop();
		}
	}
}
