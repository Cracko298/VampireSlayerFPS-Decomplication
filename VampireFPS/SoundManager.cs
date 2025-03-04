using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace VampireFPS;

public class SoundManager
{
	public enum SFX
	{
		ShotgunFire1,
		ShotgunFire2,
		ShotgunFire3,
		ShotgunReload,
		Ricochet1,
		Ricochet2,
		Ricochet3,
		Ricochet4,
		Ricochet5,
		Ricochet6,
		Ricochet7,
		Ricochet8,
		Ricochet9,
		Hit1,
		Hit2,
		step_right1,
		step_right2,
		step_right3,
		step_left1,
		step_left2,
		step_left3,
		Mac10Fire1,
		Mac10Fire2,
		Mac10Fire3,
		Mac10Reload,
		CrossbowFire1,
		CrossbowFire2,
		CrossbowFire3,
		CrossbowReload,
		Stake1,
		Stake2,
		Stake3,
		DryFire,
		Select,
		Back,
		Up,
		Down,
		Catallus1,
		Catallus2,
		Catallus3,
		Catallus4,
		Catallus5,
		Catallus6,
		Staked,
		VampAttack1,
		VampAttack2,
		VampFDie,
		VampMDie,
		VampJump,
		Leap1,
		Leap2,
		Leap3,
		ResF,
		ResM,
		Feed,
		VampSwipe,
		Body1,
		Body2,
		Body3,
		Body4,
		Body5,
		Pickup,
		END
	}

	private float m_FootStepTime;

	public Sound[] m_Sound;

	public SoundManager()
	{
		m_Sound = new Sound[62];
		for (int i = 0; i < 62; i++)
		{
			m_Sound[i].m_Instance = (SoundEffectInstance[])(object)new SoundEffectInstance[4];
		}
	}

	public SoundEffectInstance Play(int id)
	{
		return m_Sound[id].Play();
	}

	public SoundEffectInstance Play(int id, float vol)
	{
		return m_Sound[id].Play(vol);
	}

	public SoundEffectInstance Play3D(int id, Vector3 pos)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return m_Sound[id].Play3D(pos);
	}

	public SoundEffectInstance Play3D(int id, Vector3 pos, float vol)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return m_Sound[id].Play3D(pos, vol);
	}

	public SoundEffectInstance PlayLooped(int id)
	{
		return m_Sound[id].PlayLooped();
	}

	public void Add(int id, SoundEffect s)
	{
		m_Sound[id].Add(s);
	}

	public bool IsPlaying(int id)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 4; i++)
		{
			if ((int)m_Sound[id].m_Instance[i].State == 0)
			{
				return true;
			}
		}
		return false;
	}

	public void Stop(int id)
	{
		m_Sound[id].Stop();
	}

	public void PlayLocalFootsteps(float bobY)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = g.m_PlayerManager.GetLocalPlayer().m_Position;
		float moveVol = g.m_PlayerManager.GetLocalPlayer().GetMoveVol();
		float num = (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds;
		if (bobY > 0.95f && m_FootStepTime < num)
		{
			m_FootStepTime = num + 0.4f;
			int num2 = g.m_App.m_Rand.Next(6);
			Play3D(15 + num2, position, moveVol);
		}
	}
}
