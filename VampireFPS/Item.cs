using System;
using System.Collections.Generic;
using BEPUphysics;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using DPSF;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Collision;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Lights;
using SynapseGaming.LightingSystem.Rendering;
using SynapseGaming.LightingSystem.Shadows;

namespace VampireFPS;

public class Item
{
	public enum OBJ
	{
		SHOTGUN,
		MAC10,
		CROSSBOW,
		CROSSBOW_EMPTY,
		CRUCIFIX,
		STAKE,
		CLAWS,
		END
	}

	public enum RELOAD
	{
		None,
		Start,
		ShellLoad,
		Shell,
		Ending,
		End,
		Empty
	}

	private const float CRUCIFIX_DAMAGE_RANGE_SQ = 25f;

	private const int CRUCIFX_BURN_DAMAGE = 25;

	private const float CRUCIFIX_DAMAGE_RATE = 1f;

	private const float ZOOM_CROSSBOW = 20f;

	private const float ZOOM_MAC10 = 30f;

	private const float CLAWS_FIRE_RATE = 750f;

	private const float CLAWS_SECOND_FIRE_TIME = 200f;

	private const int CLAWS_DAMAGE = 100;

	private const float CLAWS_RANGE = 6.35f;

	public const int MAX_ITEMS_VFX = 3;

	public const int MAC10_MAX_AMMO = 200;

	public const int MAC10_CLIP_SIZE = 25;

	public const int SHOTGUN_MAX_AMMO = 75;

	public const int SHOTGUN_CLIP_SIZE = 8;

	public const int CROSSBOW_MAX_AMMO = 15;

	public const int CROSSBOW_CLIP_SIZE = 1;

	public const int CRUCIFIX_MAX_AMMO = 0;

	public const int CRUCIFIX_CLIP_SIZE = 0;

	public const int STAKE_MAX_AMMO = 0;

	public const int STAKE_CLIP_SIZE = 0;

	public const int CLAWS_MAX_AMMO = 0;

	public const int CLAWS_CLIP_SIZE = 0;

	private static float STAKE_FIRE_RATE = 533f;

	private static float SHOTGUN_FIRE_RATE = 1000f;

	private float m_CrucifixUseTime;

	private float m_CrucifixDamageTime;

	private static float CRUCIFIX_FIRE_RATE = 30000f;

	private static float CRUCIFIX_ACTIVE_TIME = 5f;

	public SceneObject m_CrossbowEmpty;

	private static float CROSSBOW_FIRE_RATE = 3000f;

	private static float MAC10_FIRE_RATE = 150f;

	private bool m_bDoSecondClaw;

	private float m_SecondClawTimer;

	public int m_Id;

	public Model m_Model;

	public SceneObject m_SceneObject;

	public int m_Type;

	public Player m_Player;

	public int m_VfxSystemIdx;

	public ExplosionFireSmokeParticleSystem m_WeaponSmoke;

	public ExplosionFlyingSparksParticleSystem[] m_WeaponSparks;

	public BuckshotQuadSprayParticleSystem[] m_WeaponBuckshot;

	public float m_WeaponTimer;

	public PointLight m_WeaponFlash;

	public int m_WeaponFlashCount;

	public int m_WeaponAmmo;

	public int m_WeaponAmmoInClip;

	private RELOAD m_ReloadState;

	private bool m_bShouldRicochetSFX = true;

	private bool m_bShouldRicochetVFX = true;

	private bool m_bZoomed;

	private Vector2 centre = new Vector2(512f, 288f);

	private Vector2 spriteHalfSize = new Vector2(32f, 32f);

	public void InitStake()
	{
		m_WeaponTimer = 0f;
		m_WeaponAmmo = 0;
		m_WeaponAmmoInClip = 0;
		m_bShouldRicochetVFX = false;
		m_bShouldRicochetSFX = false;
	}

	public void UpdateStake()
	{
	}

	private bool FireStake()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (m_WeaponTimer > (float)g.m_App.m_GameTime.TotalGameTime.TotalMilliseconds)
		{
			return false;
		}
		Vector3 aimPosition = m_Player.GetAimPosition();
		Vector3 aimVector = m_Player.GetAimVector();
		DoWeaponRayCast(aimPosition, aimVector, 3.81f, 20);
		PlayStakeFireSFX();
		m_WeaponTimer = (float)g.m_App.m_GameTime.TotalGameTime.TotalMilliseconds + STAKE_FIRE_RATE;
		return true;
	}

	private void PlayStakeFireSFX()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = m_Player.m_Position;
		Matrix val = Matrix.CreateRotationY(m_Player.m_Rotation.Y);
		Vector3 pos = position + ((Matrix)(ref val)).Forward;
		int num = g.m_App.m_Rand.Next(3);
		g.m_SoundManager.Play3D(29 + num, pos);
	}

	private bool SimulateFireStake()
	{
		PlayStakeFireSFX();
		return true;
	}

	private void HideStake()
	{
	}

	public void InitShotgun()
	{
		m_WeaponTimer = 0f;
		InitWeaponFlash();
		m_WeaponAmmo = 75;
		m_WeaponAmmoInClip = 8;
		if (m_WeaponSmoke == null)
		{
			m_WeaponSmoke = new ExplosionFireSmokeParticleSystem((Game)(object)g.m_App);
			((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)(object)m_WeaponSmoke).Enabled = false;
			((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)(object)m_WeaponSmoke).AutoInitialize(((Game)g.m_App).GraphicsDevice, ((Game)g.m_App).Content, g.m_App.screenManager.SpriteBatch);
			g.m_App.m_ParticleSystemManager.AddParticleSystem((IDPSFParticleSystem)(object)m_WeaponSmoke);
		}
		for (int i = 0; i < 3; i++)
		{
			if (m_WeaponSparks[i] == null)
			{
				m_WeaponSparks[i] = new ExplosionFlyingSparksParticleSystem((Game)(object)g.m_App);
				((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)m_WeaponSparks[i]).Enabled = false;
				((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)m_WeaponSparks[i]).AutoInitialize(((Game)g.m_App).GraphicsDevice, ((Game)g.m_App).Content, g.m_App.screenManager.SpriteBatch);
				g.m_App.m_ParticleSystemManager.AddParticleSystem((IDPSFParticleSystem)(object)m_WeaponSparks[i]);
			}
			if (m_WeaponBuckshot[i] == null)
			{
				m_WeaponBuckshot[i] = new BuckshotQuadSprayParticleSystem((Game)(object)g.m_App);
				((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)m_WeaponBuckshot[i]).Enabled = false;
				((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)m_WeaponBuckshot[i]).AutoInitialize(((Game)g.m_App).GraphicsDevice, ((Game)g.m_App).Content, g.m_App.screenManager.SpriteBatch);
				g.m_App.m_ParticleSystemManager.AddParticleSystem((IDPSFParticleSystem)(object)m_WeaponBuckshot[i]);
			}
		}
		DisableWeaponVFX();
		m_bShouldRicochetVFX = true;
		m_bShouldRicochetSFX = true;
	}

	public void UpdateShotgun()
	{
		m_Player.m_RequestedDamageAmount = 0;
		UpdateWeaponFlash();
	}

	private bool FireShotgun()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (m_WeaponTimer > (float)g.m_App.m_GameTime.TotalGameTime.TotalMilliseconds)
		{
			return false;
		}
		if (m_WeaponAmmoInClip == 0)
		{
			g.m_SoundManager.Play3D(32, m_Player.m_Position);
			m_WeaponTimer = (float)g.m_App.m_GameTime.TotalGameTime.TotalMilliseconds + SHOTGUN_FIRE_RATE;
			return false;
		}
		Vector3 aimPosition = m_Player.GetAimPosition();
		Vector3 aimVector = m_Player.GetAimVector();
		StartSmokePuffAndFlash(aimVector);
		Vector3 val = DPSFHelper.RandomNormalizedVector();
		val = Vector3.Transform(aimVector, Quaternion.CreateFromAxisAngle(val, (float)((g.m_App.m_Rand.NextDouble() - 0.5) * (double)MathHelper.ToRadians(10f))));
		DoWeaponRayCast(aimPosition, val, 76.2f, 30);
		val = DPSFHelper.RandomNormalizedVector();
		val = Vector3.Transform(aimVector, Quaternion.CreateFromAxisAngle(val, (float)((g.m_App.m_Rand.NextDouble() - 0.5) * (double)MathHelper.ToRadians(10f))));
		DoWeaponRayCast(aimPosition, val, 76.2f, 30);
		val = DPSFHelper.RandomNormalizedVector();
		val = Vector3.Transform(aimVector, Quaternion.CreateFromAxisAngle(val, (float)((g.m_App.m_Rand.NextDouble() - 0.5) * (double)MathHelper.ToRadians(10f))));
		DoWeaponRayCast(aimPosition, val, 76.2f, 30);
		PlayShotgunFireSFX();
		m_WeaponAmmoInClip--;
		m_WeaponTimer = (float)g.m_App.m_GameTime.TotalGameTime.TotalMilliseconds + SHOTGUN_FIRE_RATE;
		m_ReloadState = RELOAD.None;
		if (!m_Player.m_Bot)
		{
			g.m_App.m_RumbleFrames = 4;
		}
		return true;
	}

	private void PlayShotgunFireSFX()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = m_Player.m_Position;
		Matrix val = Matrix.CreateRotationY(m_Player.m_Rotation.Y);
		Vector3 pos = position + ((Matrix)(ref val)).Forward;
		int id = g.m_App.m_Rand.Next(3);
		g.m_SoundManager.Play3D(id, pos);
	}

	private bool SimulateFireShotgun()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 aimVector = m_Player.GetAimVector();
		StartSmokePuffAndFlash(aimVector);
		PlayShotgunFireSFX();
		return true;
	}

	private void HideShotgun()
	{
	}

	public void InitCrucifix()
	{
		m_WeaponTimer = 0f;
		m_CrucifixUseTime = 0f;
		m_WeaponAmmo = 0;
		m_WeaponAmmoInClip = 0;
		m_bShouldRicochetVFX = false;
		m_bShouldRicochetSFX = false;
	}

	public void UpdateCrucifix()
	{
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds;
		if (m_Player.m_CurrentViewAnim == 9 && m_CrucifixUseTime < num)
		{
			m_Player.PlayViewAnim(8, loop: true, 0f);
		}
		if (!(m_CrucifixUseTime > num) || !(m_CrucifixDamageTime < num))
		{
			return;
		}
		m_CrucifixDamageTime = num + 1f;
		float num2 = 0f;
		for (int i = 0; i < 10; i++)
		{
			if (g.m_PlayerManager.m_Player[i].m_Id != -1 && (float)g.m_PlayerManager.m_Player[i].m_Health > 0f && m_Player.m_Id != i)
			{
				Vector3 val = m_Player.m_Position - g.m_PlayerManager.m_Player[i].m_Position;
				num2 = ((Vector3)(ref val)).LengthSquared();
				if (num2 < 25f)
				{
					m_Player.RequestDamageOther(1, 25, g.m_PlayerManager.m_Player[i], g.m_PlayerManager.m_Player[i].m_Position + new Vector3(0f, 1.5f, 0f));
				}
			}
		}
	}

	private bool FireCrucifix()
	{
		if (m_WeaponTimer > (float)g.m_App.m_GameTime.TotalGameTime.TotalMilliseconds)
		{
			return false;
		}
		PlayCrucifixFireSFX();
		m_WeaponTimer = (float)g.m_App.m_GameTime.TotalGameTime.TotalMilliseconds + CRUCIFIX_FIRE_RATE;
		m_CrucifixUseTime = (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds + CRUCIFIX_ACTIVE_TIME;
		m_Player.m_InvincibilityTime = (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds + CRUCIFIX_ACTIVE_TIME;
		return true;
	}

	private void PlayCrucifixFireSFX()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = m_Player.m_Position;
		Matrix val = Matrix.CreateRotationY(m_Player.m_Rotation.Y);
		Vector3 pos = position + ((Matrix)(ref val)).Forward;
		int num = g.m_App.m_Rand.Next(6);
		g.m_SoundManager.Play3D(37 + num, pos);
	}

	private bool SimulateFireCrucifix()
	{
		PlayCrucifixFireSFX();
		return true;
	}

	private void HideCrucifix()
	{
		m_CrucifixUseTime = 0f;
		m_Player.m_InvincibilityTime = 0f;
	}

	public void InitCrossbow()
	{
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Expected O, but got Unknown
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		m_WeaponTimer = 0f;
		m_WeaponAmmo = 15;
		m_WeaponAmmoInClip = 1;
		if (m_WeaponSmoke == null)
		{
			m_WeaponSmoke = new ExplosionFireSmokeParticleSystem((Game)(object)g.m_App);
			((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)(object)m_WeaponSmoke).Enabled = false;
			((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)(object)m_WeaponSmoke).AutoInitialize(((Game)g.m_App).GraphicsDevice, ((Game)g.m_App).Content, g.m_App.screenManager.SpriteBatch);
			g.m_App.m_ParticleSystemManager.AddParticleSystem((IDPSFParticleSystem)(object)m_WeaponSmoke);
		}
		for (int i = 0; i < 3; i++)
		{
			if (m_WeaponSparks[i] == null)
			{
				m_WeaponSparks[i] = new ExplosionFlyingSparksParticleSystem((Game)(object)g.m_App);
				((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)m_WeaponSparks[i]).Enabled = false;
				((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)m_WeaponSparks[i]).AutoInitialize(((Game)g.m_App).GraphicsDevice, ((Game)g.m_App).Content, g.m_App.screenManager.SpriteBatch);
				g.m_App.m_ParticleSystemManager.AddParticleSystem((IDPSFParticleSystem)(object)m_WeaponSparks[i]);
			}
			if (m_WeaponBuckshot[i] == null)
			{
				m_WeaponBuckshot[i] = new BuckshotQuadSprayParticleSystem((Game)(object)g.m_App);
				((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)m_WeaponBuckshot[i]).Enabled = false;
				((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)m_WeaponBuckshot[i]).AutoInitialize(((Game)g.m_App).GraphicsDevice, ((Game)g.m_App).Content, g.m_App.screenManager.SpriteBatch);
				g.m_App.m_ParticleSystemManager.AddParticleSystem((IDPSFParticleSystem)(object)m_WeaponBuckshot[i]);
			}
		}
		DisableWeaponVFX();
		m_bShouldRicochetVFX = true;
		m_bShouldRicochetSFX = false;
		SceneObject val = new SceneObject(g.m_ItemManager.m_Model[3]);
		((SceneEntity)val).UpdateType = (UpdateType)1;
		val.Visibility = (ObjectVisibility)0;
		val.StaticLightingType = (StaticLightingType)1;
		val.CollisionType = (CollisionType)0;
		val.AffectedByGravity = false;
		((SceneEntity)val).Name = $"CrossbowEmpty";
		((SceneEntity)val).World = Matrix.CreateTranslation(Vector3.Zero);
		m_CrossbowEmpty = val;
		((ISubmit<SceneEntity>)(object)g.m_App.sceneInterface.ObjectManager).Submit((SceneEntity)(object)m_CrossbowEmpty);
	}

	public void UpdateCrossbow()
	{
		if (m_ReloadState == RELOAD.Empty || m_WeaponAmmoInClip == 0)
		{
			m_CrossbowEmpty.Visibility = (ObjectVisibility)1;
			m_SceneObject.Visibility = (ObjectVisibility)0;
		}
		else
		{
			m_CrossbowEmpty.Visibility = (ObjectVisibility)0;
			m_SceneObject.Visibility = (ObjectVisibility)1;
		}
	}

	private bool FireCrossbow()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (m_WeaponTimer > (float)g.m_App.m_GameTime.TotalGameTime.TotalMilliseconds)
		{
			return false;
		}
		if (m_WeaponAmmoInClip == 0)
		{
			g.m_SoundManager.Play3D(32, m_Player.m_Position);
			m_WeaponTimer = (float)g.m_App.m_GameTime.TotalGameTime.TotalMilliseconds + CROSSBOW_FIRE_RATE;
			return false;
		}
		Vector3 aimPosition = m_Player.GetAimPosition();
		Vector3 aimVector = m_Player.GetAimVector();
		Vector3 val = DPSFHelper.RandomNormalizedVector();
		val = Vector3.Transform(aimVector, Quaternion.CreateFromAxisAngle(val, (float)((g.m_App.m_Rand.NextDouble() - 0.5) * (double)MathHelper.ToRadians(0.1f))));
		DoWeaponRayCast(aimPosition, val, 254f, 30);
		PlayCrossbowFireSFX();
		m_WeaponAmmoInClip--;
		m_WeaponTimer = (float)g.m_App.m_GameTime.TotalGameTime.TotalMilliseconds + CROSSBOW_FIRE_RATE;
		if (!m_Player.m_Bot)
		{
			g.m_App.m_RumbleFrames = 4;
		}
		return true;
	}

	private void PlayCrossbowFireSFX()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = m_Player.m_Position;
		Matrix val = Matrix.CreateRotationY(m_Player.m_Rotation.Y);
		Vector3 pos = position + ((Matrix)(ref val)).Forward;
		int num = g.m_App.m_Rand.Next(3);
		g.m_SoundManager.Play3D(25 + num, pos);
	}

	private bool SimulateFireCrossbow()
	{
		PlayCrossbowFireSFX();
		return true;
	}

	private void HideCrossbow()
	{
		if (m_CrossbowEmpty != null)
		{
			m_CrossbowEmpty.Visibility = (ObjectVisibility)0;
		}
		g.m_CameraManager.SetDefaultFov();
	}

	private void ZoomCrossbow(bool bZoom)
	{
		if (bZoom)
		{
			g.m_CameraManager.SetTargetFov(20f);
		}
		else
		{
			g.m_CameraManager.SetDefaultFov();
		}
	}

	public void InitMac10()
	{
		m_WeaponTimer = 0f;
		InitWeaponFlash();
		m_WeaponAmmo = 200;
		m_WeaponAmmoInClip = 25;
		if (m_WeaponSmoke == null)
		{
			m_WeaponSmoke = new ExplosionFireSmokeParticleSystem((Game)(object)g.m_App);
			((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)(object)m_WeaponSmoke).Enabled = false;
			((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)(object)m_WeaponSmoke).AutoInitialize(((Game)g.m_App).GraphicsDevice, ((Game)g.m_App).Content, g.m_App.screenManager.SpriteBatch);
			g.m_App.m_ParticleSystemManager.AddParticleSystem((IDPSFParticleSystem)(object)m_WeaponSmoke);
		}
		for (int i = 0; i < 3; i++)
		{
			if (m_WeaponSparks[i] == null)
			{
				m_WeaponSparks[i] = new ExplosionFlyingSparksParticleSystem((Game)(object)g.m_App);
				((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)m_WeaponSparks[i]).Enabled = false;
				((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)m_WeaponSparks[i]).AutoInitialize(((Game)g.m_App).GraphicsDevice, ((Game)g.m_App).Content, g.m_App.screenManager.SpriteBatch);
				g.m_App.m_ParticleSystemManager.AddParticleSystem((IDPSFParticleSystem)(object)m_WeaponSparks[i]);
			}
			if (m_WeaponBuckshot[i] == null)
			{
				m_WeaponBuckshot[i] = new BuckshotQuadSprayParticleSystem((Game)(object)g.m_App);
				((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)m_WeaponBuckshot[i]).Enabled = false;
				((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)m_WeaponBuckshot[i]).AutoInitialize(((Game)g.m_App).GraphicsDevice, ((Game)g.m_App).Content, g.m_App.screenManager.SpriteBatch);
				g.m_App.m_ParticleSystemManager.AddParticleSystem((IDPSFParticleSystem)(object)m_WeaponBuckshot[i]);
			}
		}
		DisableWeaponVFX();
		m_bShouldRicochetVFX = true;
		m_bShouldRicochetSFX = true;
	}

	public void UpdateMac10()
	{
		UpdateWeaponFlash();
	}

	private bool FireMac10()
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		if (m_WeaponTimer > (float)g.m_App.m_GameTime.TotalGameTime.TotalMilliseconds)
		{
			return false;
		}
		if (m_WeaponAmmoInClip == 0)
		{
			g.m_SoundManager.Play3D(32, m_Player.m_Position);
			m_WeaponTimer = (float)g.m_App.m_GameTime.TotalGameTime.TotalMilliseconds + MAC10_FIRE_RATE;
			return false;
		}
		Vector3 aimPosition = m_Player.GetAimPosition();
		Vector3 aimVector = m_Player.GetAimVector();
		StartSmokePuffAndFlash(aimVector);
		Vector3 val = DPSFHelper.RandomNormalizedVector();
		float num = 6f;
		if (m_bZoomed)
		{
			num = 4f;
		}
		val = Vector3.Transform(aimVector, Quaternion.CreateFromAxisAngle(val, (float)((g.m_App.m_Rand.NextDouble() - 0.5) * (double)MathHelper.ToRadians(num))));
		DoWeaponRayCast(aimPosition, val, 101.6f, 15);
		PlayMac10FireSFX();
		m_WeaponAmmoInClip--;
		m_WeaponTimer = (float)g.m_App.m_GameTime.TotalGameTime.TotalMilliseconds + MAC10_FIRE_RATE;
		if (!m_Player.m_Bot)
		{
			g.m_App.m_RumbleFrames = 3;
		}
		return true;
	}

	private void PlayMac10FireSFX()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = m_Player.m_Position;
		Matrix val = Matrix.CreateRotationY(m_Player.m_Rotation.Y);
		Vector3 pos = position + ((Matrix)(ref val)).Forward;
		int num = g.m_App.m_Rand.Next(3);
		g.m_SoundManager.Play3D(21 + num, pos);
	}

	private bool SimulateFireMac10()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 aimVector = m_Player.GetAimVector();
		StartSmokePuffAndFlash(aimVector);
		PlayMac10FireSFX();
		return true;
	}

	private void HideMac10()
	{
	}

	private void ZoomMac10(bool bZoom)
	{
		if (bZoom)
		{
			g.m_CameraManager.SetTargetFov(30f);
		}
		else
		{
			g.m_CameraManager.SetDefaultFov();
		}
	}

	public void InitClaws()
	{
		m_WeaponTimer = 0f;
		m_WeaponAmmo = 0;
		m_WeaponAmmoInClip = 0;
		m_bShouldRicochetVFX = false;
		m_bShouldRicochetSFX = false;
	}

	public void UpdateClaws()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (m_bDoSecondClaw && m_SecondClawTimer < (float)g.m_App.m_GameTime.TotalGameTime.TotalMilliseconds)
		{
			Vector3 aimPosition = m_Player.GetAimPosition();
			Vector3 aimVector = m_Player.GetAimVector();
			DoWeaponRayCast(aimPosition, aimVector, 6.35f, 100);
			m_bDoSecondClaw = false;
		}
	}

	private bool FireClaws()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (m_WeaponTimer > (float)g.m_App.m_GameTime.TotalGameTime.TotalMilliseconds)
		{
			return false;
		}
		Vector3 aimPosition = m_Player.GetAimPosition();
		Vector3 aimVector = m_Player.GetAimVector();
		DoWeaponRayCast(aimPosition, aimVector, 6.35f, 100);
		PlayClawsFireSFX();
		m_WeaponTimer = (float)g.m_App.m_GameTime.TotalGameTime.TotalMilliseconds + 750f;
		m_bDoSecondClaw = true;
		m_SecondClawTimer = (float)g.m_App.m_GameTime.TotalGameTime.TotalMilliseconds + 200f;
		return true;
	}

	private void PlayClawsFireSFX()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = m_Player.m_Position;
		Matrix val = Matrix.CreateRotationY(m_Player.m_Rotation.Y);
		Vector3 pos = position + ((Matrix)(ref val)).Forward;
		g.m_SoundManager.Play3D(55, pos);
	}

	private bool SimulateFireClaws()
	{
		PlayClawsFireSFX();
		return true;
	}

	private void HideClaws()
	{
	}

	public Item()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		m_Type = -1;
		m_Id = -1;
		m_Model = null;
		m_SceneObject = null;
		m_Player = null;
		m_VfxSystemIdx = 0;
		m_WeaponBuckshot = new BuckshotQuadSprayParticleSystem[3];
		m_WeaponSparks = new ExplosionFlyingSparksParticleSystem[3];
		m_WeaponSmoke = null;
		for (int i = 0; i < 3; i++)
		{
			m_WeaponBuckshot[i] = null;
			m_WeaponSparks[i] = null;
		}
	}

	public void Update()
	{
		if (m_ReloadState != 0)
		{
			UpdateReload();
		}
		switch (m_Type)
		{
		case 0:
			if (ActiveWeapon())
			{
				UpdateShotgun();
			}
			break;
		case 1:
			if (ActiveWeapon())
			{
				UpdateMac10();
			}
			break;
		case 2:
			if (ActiveWeapon())
			{
				UpdateCrossbow();
			}
			break;
		case 4:
			if (ActiveWeapon())
			{
				UpdateCrucifix();
			}
			break;
		case 5:
			if (ActiveWeapon())
			{
				UpdateStake();
			}
			break;
		case 6:
			if (ActiveWeapon())
			{
				UpdateClaws();
			}
			break;
		case 3:
			break;
		}
	}

	private bool ActiveWeapon()
	{
		if (m_Player == null)
		{
			return false;
		}
		if (m_Player.m_Health <= 0)
		{
			return false;
		}
		if (m_Player.m_WeaponItemIndex == m_Id)
		{
			return true;
		}
		return false;
	}

	private void UpdateReload()
	{
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		switch (m_ReloadState)
		{
		case RELOAD.Start:
			if (m_Player.m_ViewAnimationController.HasFinished)
			{
				m_ReloadState = RELOAD.ShellLoad;
			}
			break;
		case RELOAD.ShellLoad:
		{
			if (Math.Min(8 - m_WeaponAmmoInClip, m_WeaponAmmo) == 0)
			{
				m_ReloadState = RELOAD.Ending;
				break;
			}
			int num2 = 1;
			m_ReloadState = RELOAD.Shell;
			m_WeaponAmmo -= num2;
			m_WeaponAmmoInClip += num2;
			g.m_SoundManager.Play3D(3, m_Player.m_Position);
			m_Player.PlayViewAnim(5, loop: false, 0f);
			break;
		}
		case RELOAD.Shell:
			if (m_Player.m_ViewAnimationController.HasFinished)
			{
				m_ReloadState = RELOAD.ShellLoad;
			}
			break;
		case RELOAD.Ending:
			m_Player.PlayViewAnim(6, loop: false, 0f);
			m_ReloadState = RELOAD.End;
			break;
		case RELOAD.End:
			if (m_Player.m_ViewAnimationController.HasFinished)
			{
				m_Player.PlayViewAnim(0, loop: true, 0f);
				m_ReloadState = RELOAD.None;
			}
			break;
		case RELOAD.Empty:
		{
			float num = m_WeaponTimer - (float)g.m_App.m_GameTime.TotalGameTime.TotalMilliseconds;
			if (num < 500f)
			{
				m_ReloadState = RELOAD.None;
			}
			break;
		}
		case RELOAD.None:
			break;
		}
	}

	public void Delete()
	{
		m_Id = -1;
		if (m_SceneObject != null)
		{
			((ISubmit<SceneEntity>)(object)g.m_App.sceneInterface.ObjectManager).Remove((SceneEntity)(object)m_SceneObject);
			m_SceneObject = null;
		}
		if (m_CrossbowEmpty != null)
		{
			((ISubmit<SceneEntity>)(object)g.m_App.sceneInterface.ObjectManager).Remove((SceneEntity)(object)m_CrossbowEmpty);
			m_CrossbowEmpty = null;
		}
		if (m_WeaponFlash != null)
		{
			((ISubmit<BaseLight>)(object)g.m_App.sceneInterface.LightManager).Remove((BaseLight)(object)m_WeaponFlash);
			m_WeaponFlash = null;
			m_WeaponFlashCount = 0;
		}
	}

	public bool Fire()
	{
		if (m_Player.m_CurrentViewAnim == 12)
		{
			return false;
		}
		bool result = false;
		switch (m_Type)
		{
		case 0:
			result = FireShotgun();
			break;
		case 1:
			result = FireMac10();
			break;
		case 2:
			result = FireCrossbow();
			break;
		case 4:
			result = FireCrucifix();
			break;
		case 5:
			result = FireStake();
			break;
		case 6:
			result = FireClaws();
			break;
		}
		return result;
	}

	public bool SimulateFire()
	{
		bool result = false;
		switch (m_Type)
		{
		case 0:
			result = SimulateFireShotgun();
			break;
		case 1:
			result = SimulateFireMac10();
			break;
		case 2:
			result = SimulateFireCrossbow();
			break;
		case 4:
			result = SimulateFireCrucifix();
			break;
		case 5:
			result = SimulateFireStake();
			break;
		case 6:
			result = SimulateFireClaws();
			break;
		}
		return result;
	}

	public void DoWeaponRayCast(Vector3 position, Vector3 direction, float range, int damage)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		List<RayCastResult> list = new List<RayCastResult>();
		Ray val = default(Ray);
		((Ray)(ref val))._002Ector(position, direction);
		if (!g.m_App.m_Space.RayCast(val, range, (IList<RayCastResult>)list))
		{
			return;
		}
		list.Sort();
		Player player = null;
		int num = 255;
		Vector3 vctHitPos = Vector3.Zero;
		for (int i = 0; i < list.Count; i++)
		{
			BroadPhaseEntry hitObject = list[i].HitObject;
			EntityCollidable val2 = (EntityCollidable)(object)((hitObject is EntityCollidable) ? hitObject : null);
			if (val2 == null)
			{
				Vector3 location = list[i].HitData.Location;
				if (m_bShouldRicochetVFX)
				{
					((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)m_WeaponSparks[m_VfxSystemIdx]).Enabled = true;
					((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)m_WeaponSparks[m_VfxSystemIdx]).Visible = true;
					((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)m_WeaponSparks[m_VfxSystemIdx]).Emitter.BurstParticles = 40;
					((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)m_WeaponSparks[m_VfxSystemIdx]).Emitter.PositionData.Position = location;
					((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)m_WeaponSparks[m_VfxSystemIdx]).LerpEmittersPositionAndOrientationOnNextUpdate = false;
					m_WeaponSparks[m_VfxSystemIdx].Normal = list[i].HitData.Normal;
					((Vector3)(ref m_WeaponSparks[m_VfxSystemIdx].Normal)).Normalize();
					((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)m_WeaponBuckshot[m_VfxSystemIdx]).Enabled = true;
					((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)m_WeaponBuckshot[m_VfxSystemIdx]).Visible = true;
					((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)m_WeaponBuckshot[m_VfxSystemIdx]).Emitter.BurstParticles = 40;
					((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)m_WeaponBuckshot[m_VfxSystemIdx]).Emitter.PositionData.Position = location;
					((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)m_WeaponBuckshot[m_VfxSystemIdx]).LerpEmittersPositionAndOrientationOnNextUpdate = false;
					m_WeaponBuckshot[m_VfxSystemIdx].Normal = m_WeaponSparks[m_VfxSystemIdx].Normal;
				}
				if (m_bShouldRicochetSFX)
				{
					int num2 = g.m_App.m_Rand.Next(9);
					g.m_SoundManager.Play3D(4 + num2, location);
				}
				m_VfxSystemIdx++;
				if (m_VfxSystemIdx >= 3)
				{
					m_VfxSystemIdx = 0;
				}
				break;
			}
			if (val2.Entity.Tag is HitTag hitTag)
			{
				if (g.m_PlayerManager.m_Player[hitTag.m_PlayerId].m_Id != -1 && hitTag.m_PlayerId != m_Player.m_Id)
				{
					player = g.m_PlayerManager.m_Player[hitTag.m_PlayerId];
				}
				if (player != null)
				{
					if (hitTag.m_HitZone == 1)
					{
						num = 1;
						vctHitPos = list[i].HitData.Location;
					}
					else if (hitTag.m_HitZone == 2)
					{
						num = 2;
						vctHitPos = list[i].HitData.Location;
					}
					else if (hitTag.m_HitZone == 3)
					{
						num = 3;
						vctHitPos = list[i].HitData.Location;
					}
				}
			}
			if (player != null && num != 255)
			{
				m_Player.RequestDamageOther(num, damage, player, vctHitPos);
				break;
			}
		}
	}

	public void DrawCrosshair(SpriteBatch spriteBatch)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		switch (m_Type)
		{
		case 0:
			spriteBatch.Draw(g.m_ItemManager.m_ShotgunCrosshair, centre - spriteHalfSize, Color.White);
			break;
		case 1:
			spriteBatch.Draw(g.m_ItemManager.m_Mac10Crosshair, centre - spriteHalfSize, Color.White);
			break;
		case 2:
			spriteBatch.Draw(g.m_ItemManager.m_CrossbowCrosshair, centre - spriteHalfSize, Color.White);
			break;
		case 5:
		case 6:
			spriteBatch.Draw(g.m_ItemManager.m_StakeCrosshair, centre - spriteHalfSize, Color.White);
			break;
		case 3:
		case 4:
			break;
		}
	}

	public void StartSmokePuffAndFlash(Vector3 direction)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		PointLight weaponFlash = m_WeaponFlash;
		Matrix world = ((SceneEntity)m_SceneObject).World;
		weaponFlash.Position = ((Matrix)(ref world)).Translation + direction;
		((BaseLight)m_WeaponFlash).Enabled = true;
		m_WeaponFlashCount = 2;
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)(object)m_WeaponSmoke).Enabled = true;
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)(object)m_WeaponSmoke).Visible = true;
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)(object)m_WeaponSmoke).Emitter.BurstTime = 0.2f;
		Position3D positionData = ((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)(object)m_WeaponSmoke).Emitter.PositionData;
		Matrix world2 = ((SceneEntity)m_SceneObject).World;
		positionData.Position = ((Matrix)(ref world2)).Translation + direction * 1.7f;
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)(object)m_WeaponSmoke).Emitter.PositionData.Velocity = direction * 0.1f;
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)(object)m_WeaponSmoke).LerpEmittersPositionAndOrientationOnNextUpdate = false;
	}

	public void Zoom(bool bZoom)
	{
		if (!m_Player.m_Bot && m_Player == g.m_PlayerManager.GetLocalPlayer())
		{
			m_bZoomed = bZoom;
			switch (m_Type)
			{
			case 1:
				ZoomMac10(bZoom);
				break;
			case 2:
				ZoomCrossbow(bZoom);
				break;
			case 0:
			case 3:
			case 4:
			case 5:
			case 6:
				break;
			}
		}
	}

	public void InitWeaponFlash()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		m_WeaponFlash = new PointLight();
		((BaseLight)m_WeaponFlash).LightingType = (LightingType)1;
		((BaseLight)m_WeaponFlash).DiffuseColor = new Vector3(10f, 10f, 8f);
		((BaseLight)m_WeaponFlash).Intensity = 0.1f;
		m_WeaponFlash.Radius = 30f;
		((BaseLight)m_WeaponFlash).FalloffStrength = 1f;
		((BaseLight)m_WeaponFlash).Enabled = false;
		m_WeaponFlash.ShadowType = (ShadowType)0;
		((ISubmit<BaseLight>)(object)g.m_App.sceneInterface.LightManager).Submit((BaseLight)(object)m_WeaponFlash);
		m_WeaponFlashCount = 0;
	}

	public void UpdateWeaponFlash()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseLight)m_WeaponFlash).Enabled)
		{
			if (m_WeaponFlashCount > 0)
			{
				m_WeaponFlashCount--;
				PointLight weaponFlash = m_WeaponFlash;
				Matrix world = ((SceneEntity)m_SceneObject).World;
				Vector3 translation = ((Matrix)(ref world)).Translation;
				Matrix world2 = ((SceneEntity)m_SceneObject).World;
				weaponFlash.Position = translation + ((Matrix)(ref world2)).Forward * 0.5f;
			}
			else
			{
				((BaseLight)m_WeaponFlash).Enabled = false;
			}
		}
	}

	public void Reload()
	{
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		int animId = 0;
		float num2 = 1000f;
		int id = 24;
		switch (m_Type)
		{
		case 0:
			if (8 == m_WeaponAmmoInClip)
			{
				return;
			}
			if (!m_Player.m_Bot)
			{
				m_ReloadState = RELOAD.Start;
				m_Player.PlayViewAnim(4, loop: false, 0f);
				return;
			}
			num = 8;
			animId = 3;
			m_ReloadState = RELOAD.None;
			num2 = 3000f;
			break;
		case 1:
			num = 25;
			animId = 3;
			num2 = 1350f;
			id = 24;
			m_ReloadState = RELOAD.None;
			break;
		case 2:
			num = 1;
			animId = 7;
			num2 = 1650f;
			id = 28;
			m_ReloadState = RELOAD.Empty;
			break;
		}
		int num3 = Math.Min(num - m_WeaponAmmoInClip, m_WeaponAmmo);
		if (num3 != 0)
		{
			m_WeaponAmmo -= num3;
			m_WeaponAmmoInClip += num3;
			m_Player.PlayViewAnim(animId, loop: false, 0f);
			g.m_SoundManager.Play3D(id, m_Player.m_Position);
			m_WeaponTimer = (float)g.m_App.m_GameTime.TotalGameTime.TotalMilliseconds + num2;
		}
	}

	public void Show()
	{
		if (m_SceneObject != null)
		{
			m_SceneObject.Visibility = (ObjectVisibility)1;
		}
		m_WeaponTimer = (float)g.m_App.m_GameTime.TotalGameTime.TotalMilliseconds + 300f;
	}

	public void Hide()
	{
		if (m_SceneObject != null)
		{
			m_SceneObject.Visibility = (ObjectVisibility)0;
		}
		Zoom(bZoom: false);
		m_ReloadState = RELOAD.None;
		if (m_WeaponFlash != null)
		{
			((BaseLight)m_WeaponFlash).Enabled = false;
			m_WeaponFlashCount = 0;
		}
		DisableWeaponVFX();
		switch (m_Type)
		{
		case 0:
			HideShotgun();
			break;
		case 1:
			HideMac10();
			break;
		case 2:
			HideCrossbow();
			break;
		case 4:
			HideCrucifix();
			break;
		case 5:
			HideStake();
			break;
		case 6:
			HideClaws();
			break;
		case 3:
			break;
		}
	}

	private void EnableWeaponVFX()
	{
		if (m_WeaponSmoke != null)
		{
			((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)(object)m_WeaponSmoke).Enabled = true;
		}
		for (int i = 0; i < 3; i++)
		{
			if (m_WeaponBuckshot[i] != null)
			{
				((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)m_WeaponBuckshot[i]).Enabled = true;
			}
			if (m_WeaponSparks[i] != null)
			{
				((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)m_WeaponSparks[i]).Enabled = true;
			}
		}
	}

	private void DisableWeaponVFX()
	{
		if (m_WeaponSmoke != null)
		{
			((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)(object)m_WeaponSmoke).Enabled = false;
			((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)(object)m_WeaponSmoke).Visible = false;
		}
		for (int i = 0; i < 3; i++)
		{
			if (m_WeaponBuckshot[i] != null)
			{
				((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)m_WeaponBuckshot[i]).Enabled = false;
				((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)m_WeaponBuckshot[i]).Visible = false;
			}
			if (m_WeaponSparks[i] != null)
			{
				((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)m_WeaponSparks[i]).Enabled = false;
				((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)m_WeaponSparks[i]).Visible = false;
			}
		}
	}
}
