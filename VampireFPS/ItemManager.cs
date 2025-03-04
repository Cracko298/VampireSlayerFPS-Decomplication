using DPSF;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Collision;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Lights;
using SynapseGaming.LightingSystem.Rendering;

namespace VampireFPS;

public class ItemManager
{
	public const int MAX_ITEMS = 64;

	public Item[] m_Item;

	public Model[] m_Model;

	private int m_NextId;

	public Texture2D m_ShotgunCrosshair;

	public Texture2D m_Mac10Crosshair;

	public Texture2D m_CrossbowCrosshair;

	public Texture2D m_StakeCrosshair;

	public ItemManager()
	{
		m_Item = new Item[64];
		for (int i = 0; i < 64; i++)
		{
			m_Item[i] = new Item();
		}
		m_Model = (Model[])(object)new Model[7];
	}

	public int Create(int type, int triggerId, Vector3 pos, float rot, Player player)
	{
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Expected O, but got Unknown
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		int num = -1;
		if (m_NextId >= 64)
		{
			m_NextId = 0;
		}
		for (int i = m_NextId; i < 64; i++)
		{
			if (m_Item[i].m_Id == -1)
			{
				num = i;
				flag = true;
				m_NextId = i + 1;
				break;
			}
		}
		if (!flag)
		{
			for (int j = 0; j < 64; j++)
			{
				if (m_Item[j].m_Id == -1)
				{
					num = j;
					flag = true;
					m_NextId = j + 1;
					break;
				}
			}
		}
		if (num == -1)
		{
			return -1;
		}
		m_Item[num].m_Type = type;
		m_Item[num].m_Id = num;
		m_Item[num].m_Player = player;
		m_Item[num].m_VfxSystemIdx = 0;
		if (m_Item[num].m_Type != 6)
		{
			if (m_Model[type] == null)
			{
				Delete(num);
				return -1;
			}
			m_Item[num].m_Model = m_Model[type];
			Item obj = m_Item[num];
			SceneObject val = new SceneObject(m_Item[num].m_Model);
			((SceneEntity)val).UpdateType = (UpdateType)1;
			val.Visibility = (ObjectVisibility)0;
			val.StaticLightingType = (StaticLightingType)1;
			val.CollisionType = (CollisionType)0;
			val.AffectedByGravity = false;
			((SceneEntity)val).Name = $"Item{num}";
			((SceneEntity)val).World = Matrix.CreateTranslation(pos);
			obj.m_SceneObject = val;
			((ISubmit<SceneEntity>)(object)g.m_App.sceneInterface.ObjectManager).Submit((SceneEntity)(object)m_Item[num].m_SceneObject);
		}
		switch (m_Item[num].m_Type)
		{
		case 0:
			m_Item[num].InitShotgun();
			break;
		case 1:
			m_Item[num].InitMac10();
			break;
		case 2:
			m_Item[num].InitCrossbow();
			break;
		case 4:
			m_Item[num].InitCrucifix();
			break;
		case 5:
			m_Item[num].InitStake();
			break;
		case 6:
			m_Item[num].InitClaws();
			break;
		}
		if (flag)
		{
			return num;
		}
		return -1;
	}

	public void DeleteAll()
	{
		m_NextId = 0;
		for (int i = 0; i < 64; i++)
		{
			if (m_Item[i].m_Id != -1)
			{
				m_Item[i].Delete();
			}
		}
	}

	public void Delete(int id)
	{
		m_Item[id].Delete();
	}

	public void Update()
	{
		for (int i = 0; i < 64; i++)
		{
			if (m_Item[i].m_Id != -1)
			{
				m_Item[i].Update();
			}
		}
	}

	public Item FindObjectByType(int type)
	{
		for (int i = 0; i < 64; i++)
		{
			if (m_Item[i].m_Id != -1 && m_Item[i].m_Type == type)
			{
				return m_Item[i];
			}
		}
		return null;
	}

	public void Copy(Item[] src, Item[] dest)
	{
		for (int i = 0; i < 64; i++)
		{
			dest[i].m_Id = src[i].m_Id;
			dest[i].m_Type = src[i].m_Type;
		}
	}

	public void LoadContent(ContentManager Content)
	{
		m_Model[0] = Content.Load<Model>("Models\\shotgun");
		m_Model[1] = Content.Load<Model>("Models\\mac10");
		m_Model[2] = Content.Load<Model>("Models\\crossbow");
		m_Model[3] = Content.Load<Model>("Models\\crossbow_empty");
		m_Model[4] = Content.Load<Model>("Models\\crucifix");
		m_Model[5] = Content.Load<Model>("Models\\stake");
		m_ShotgunCrosshair = Content.Load<Texture2D>("Sprites\\shotguncrosshair");
		m_Mac10Crosshair = Content.Load<Texture2D>("Sprites\\mac10crosshair");
		m_CrossbowCrosshair = Content.Load<Texture2D>("Sprites\\crossbowcrosshair");
		m_StakeCrosshair = Content.Load<Texture2D>("Sprites\\stakecrosshair");
	}

	public void InitParticleSystems()
	{
		for (int i = 0; i < 64; i++)
		{
			if (m_Item[i].m_WeaponSmoke == null)
			{
				m_Item[i].m_WeaponSmoke = new ExplosionFireSmokeParticleSystem((Game)(object)g.m_App);
				((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)(object)m_Item[i].m_WeaponSmoke).Enabled = false;
				((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)(object)m_Item[i].m_WeaponSmoke).AutoInitialize(((Game)g.m_App).GraphicsDevice, ((Game)g.m_App).Content, g.m_App.screenManager.SpriteBatch);
				g.m_App.m_ParticleSystemManager.AddParticleSystem((IDPSFParticleSystem)(object)m_Item[i].m_WeaponSmoke);
			}
			for (int j = 0; j < 3; j++)
			{
				if (m_Item[i].m_WeaponSparks[j] == null)
				{
					m_Item[i].m_WeaponSparks[j] = new ExplosionFlyingSparksParticleSystem((Game)(object)g.m_App);
					((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)m_Item[i].m_WeaponSparks[j]).Enabled = false;
					((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)m_Item[i].m_WeaponSparks[j]).AutoInitialize(((Game)g.m_App).GraphicsDevice, ((Game)g.m_App).Content, g.m_App.screenManager.SpriteBatch);
					g.m_App.m_ParticleSystemManager.AddParticleSystem((IDPSFParticleSystem)(object)m_Item[i].m_WeaponSparks[j]);
				}
				if (m_Item[i].m_WeaponBuckshot[j] == null)
				{
					m_Item[i].m_WeaponBuckshot[j] = new BuckshotQuadSprayParticleSystem((Game)(object)g.m_App);
					((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)m_Item[i].m_WeaponBuckshot[j]).Enabled = false;
					((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)m_Item[i].m_WeaponBuckshot[j]).AutoInitialize(((Game)g.m_App).GraphicsDevice, ((Game)g.m_App).Content, g.m_App.screenManager.SpriteBatch);
					g.m_App.m_ParticleSystemManager.AddParticleSystem((IDPSFParticleSystem)(object)m_Item[i].m_WeaponBuckshot[j]);
				}
			}
		}
	}

	public float GetWeaponRecoil(int id)
	{
		return m_Item[id].m_Type switch
		{
			0 => 0.06f, 
			1 => 0.02f, 
			2 => 0.04f, 
			4 => 0.04f, 
			5 => 0f, 
			6 => 0f, 
			_ => 0f, 
		};
	}

	public int GetWeaponFireAnim(int id)
	{
		return m_Item[id].m_Type switch
		{
			0 => 1, 
			1 => 2, 
			2 => 2, 
			4 => 9, 
			5 => 11, 
			6 => 1, 
			_ => 2, 
		};
	}

	public bool GetWeaponAnimShouldLoop(int id)
	{
		int type = m_Item[id].m_Type;
		if (type == 4)
		{
			return true;
		}
		return false;
	}

	public bool GetWeaponShouldShowAmmo(int id)
	{
		switch (m_Item[id].m_Type)
		{
		case 4:
		case 5:
		case 6:
			return false;
		default:
			return true;
		}
	}

	public void ResetAmmo(int id)
	{
		switch (m_Item[id].m_Type)
		{
		case 0:
			m_Item[id].m_WeaponAmmo = 75;
			m_Item[id].m_WeaponAmmoInClip = 8;
			break;
		case 1:
			m_Item[id].m_WeaponAmmo = 200;
			m_Item[id].m_WeaponAmmoInClip = 25;
			break;
		case 2:
			m_Item[id].m_WeaponAmmo = 15;
			m_Item[id].m_WeaponAmmoInClip = 1;
			break;
		case 4:
			m_Item[id].m_WeaponAmmo = 0;
			m_Item[id].m_WeaponAmmoInClip = 0;
			break;
		case 5:
			m_Item[id].m_WeaponAmmo = 0;
			m_Item[id].m_WeaponAmmoInClip = 0;
			break;
		case 6:
			m_Item[id].m_WeaponAmmo = 0;
			m_Item[id].m_WeaponAmmoInClip = 0;
			break;
		case 3:
			break;
		}
	}
}
