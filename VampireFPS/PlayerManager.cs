using System;
using System.Collections.ObjectModel;
using BEPUphysics;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.Entities;
using BEPUphysicsDemos.AlternateMovement.Character;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using SgMotion;
using SynapseGaming.LightingSystem.Rendering;

namespace VampireFPS;

public class PlayerManager
{
	public const int MAX_PLAYERS = 10;

	public const float CHAR_HEIGHT = 4.318f;

	public const float CHAR_RADIUS = 1.016f;

	public const float CHAR_MASS = 10f;

	public const float CHAR_CROUCH_HEIGHT = 1.778f;

	public const float CHAR_JUMP_SPEED = 9f;

	public const float VAMPIRE_JUMP_SPEED = 20f;

	private const int NUM_TEAM_SPAWN_POINTS = 10;

	private const float SPAWN_RADIUS_SQ = 6.4515996f;

	private const float SHOW_NAME_DIST = 25.4f;

	private const float SHOW_NAME_MIN_X = 50f;

	private const float SHOW_NAME_MAX_X = 974f;

	private const float SHOW_NAME_MIN_Y = 50f;

	private const float SHOW_NAME_MAX_Y = 526f;

	public Player[] m_Player;

	public Player[] m_SortedPlayer;

	public Vector3 m_SpawnPos;

	public byte m_NextBotId;

	public CollisionGroup m_CylinderGroup;

	public SkinnedModel m_FullModel_FatherD;

	public SkinnedModel m_FullModel_SlayerF;

	public SkinnedModel m_FullModel_VampM;

	public SkinnedModel m_FullModel_VampF;

	public SkinnedModel m_ViewModel;

	public SkinnedModel m_ViewModel_Claws;

	public bool RAGDOLLS_COLLIDE_WITH_PLAYERS;

	public Vector3 CHAR_STARTPOS = new Vector3(0f, 4.5f, 0f);

	public int m_Frame;

	public PlayerManager()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		m_Player = new Player[10];
		m_SortedPlayer = new Player[10];
		for (int i = 0; i < 10; i++)
		{
			m_Player[i] = new Player();
		}
		for (int j = 0; j < 10; j++)
		{
			m_Player[j].m_Id = -1;
		}
		m_SpawnPos = Vector3.Zero;
		m_CylinderGroup = new CollisionGroup();
		m_NextBotId = 0;
	}

	public void LoadModels()
	{
		m_FullModel_FatherD = ((Game)g.m_App).Content.Load<SkinnedModel>("Models/player");
		m_FullModel_SlayerF = ((Game)g.m_App).Content.Load<SkinnedModel>("Models/hunterF");
		m_FullModel_VampM = ((Game)g.m_App).Content.Load<SkinnedModel>("Models/vampm");
		m_FullModel_VampF = ((Game)g.m_App).Content.Load<SkinnedModel>("Models/vampf");
		m_ViewModel = ((Game)g.m_App).Content.Load<SkinnedModel>("Models/view");
		m_ViewModel_Claws = ((Game)g.m_App).Content.Load<SkinnedModel>("Models/view_claws");
	}

	public Player Create(short netId, bool bot)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 10; i++)
		{
			if (m_Player[i].m_Id != -1)
			{
				continue;
			}
			m_Player[i].m_Id = i;
			m_Player[i].m_NetId = netId;
			m_Player[i].m_Position = new Vector3(0f, 1000f, 0f);
			m_Player[i].m_Rotation.Y = 0f;
			m_Player[i].m_LAX = 0f;
			m_Player[i].m_LAY = 0f;
			m_Player[i].m_Health = 100;
			m_Player[i].m_bRequestDied = false;
			m_Player[i].m_bFired = false;
			m_Player[i].m_Bot = bot;
			m_Player[i].m_bTorchChanged = false;
			m_Player[i].m_bWeaponChanged = false;
			m_Player[i].m_Anim = -1;
			m_Player[i].m_ViewAnim = -1;
			m_Player[i].m_bRequestSendDamage = false;
			m_Player[i].m_RequestedDamageAmount = 0;
			m_Player[i].m_RequestedPlayerToDamageNetID = 255;
			m_Player[i].m_RequestedHitPos = Vector3.Zero;
			m_Player[i].m_RequestedHitZone = byte.MaxValue;
			m_Player[i].m_RequestedAttacker = 255;
			m_Player[i].m_LastAttackerNetId = 255;
			m_Player[i].m_State = Player.STATE.JoinTeam;
			m_Player[i].m_Team = Player.TEAM.None;
			m_Player[i].m_Class = Player.CLASS.None;
			m_Player[i].m_NetworkPosition = m_Player[i].m_Position;
			m_Player[i].m_NetworkRotation = m_Player[i].m_Rotation.Y;
			m_Player[i].m_RespawnTimer = 0f;
			m_Player[i].m_Crouch = false;
			m_Player[i].m_RequestSendCrouch = false;
			m_Player[i].m_SpinePitch = 0f;
			m_Player[i].m_Kills = 0;
			m_Player[i].m_Deaths = 0;
			m_Player[i].m_XP = 0;
			m_Player[i].m_Rank = 1;
			m_Player[i].m_XPForNextRank = 0;
			m_Player[i].m_RequestSendTeam = false;
			m_Player[i].m_RequestSendClass = false;
			m_Player[i].m_WeaponItemIndex = -1;
			m_Player[i].m_AnimChanged = false;
			m_Player[i].m_bStaked = false;
			m_Player[i].m_RequestFeed = false;
			m_Player[i].m_RequestFeedPosition = Vector3.Zero;
			m_Player[i].m_FeedTimeOut = 0f;
			m_Player[i].m_HasAmmoToGive = false;
			m_Player[i].m_RequestDelete = false;
			m_Player[i].m_RequestRankUp = false;
			bool flag = false;
			if (m_Player[i].IsLocalPlayer())
			{
				flag = true;
			}
			if (flag)
			{
				m_Player[i].SetXPForNextLevel();
				m_Player[i].m_RequestSendScore = true;
			}
			m_Player[i].InitRagdollCollisionGroup();
			if (!flag || bot || m_Player[i].UPDATEFULLMODEL_DEBUG)
			{
				m_Player[i].SetState(Player.STATE.InGame);
			}
			if (flag || (bot && GetLocalPlayer().IsHost()))
			{
				m_Player[i].m_CharacterController = new CharacterController(CHAR_STARTPOS, 4.318f, 1.778f, 1.016f, 10f);
				((Entity)m_Player[i].m_CharacterController.Body).Tag = m_Player[i];
				((Entity)m_Player[i].m_CharacterController.Body).Position = m_Player[i].m_Position;
				m_Player[i].m_CharacterController.JumpSpeed = 9f;
				g.m_App.m_Space.Add((ISpaceObject)(object)m_Player[i].m_CharacterController);
				if (!RAGDOLLS_COLLIDE_WITH_PLAYERS)
				{
					((BroadPhaseEntry)((Entity<ConvexCollidable<CylinderShape>>)(object)m_Player[i].m_CharacterController.Body).CollisionInformation).CollisionRules.Group = m_CylinderGroup;
				}
			}
			if (flag && !bot)
			{
				m_Player[i].DisableCollisionAndGravity();
			}
			m_Player[i].InitTorchLight();
			m_Player[i].InitParticleSystems();
			if (bot && GetLocalPlayer().IsHost())
			{
				m_Player[i].BotSetClassAndTeam();
			}
			if (bot)
			{
				m_Player[i].InitBot();
			}
			return m_Player[i];
		}
		return null;
	}

	public void Delete(int id)
	{
		m_Player[id].Reset();
		m_Player[id].Delete();
	}

	public void DeleteAll()
	{
		for (int i = 0; i < 10; i++)
		{
			m_Player[i].Reset();
			m_Player[i].Delete();
		}
	}

	public void Render()
	{
		for (int i = 0; i < 10; i++)
		{
			if (m_Player[i].m_Id != -1)
			{
				m_Player[i].Render();
			}
		}
	}

	public void Update()
	{
		m_Frame++;
		for (int i = 0; i < 10; i++)
		{
			if (m_Player[i].m_Id != -1)
			{
				m_Player[i].Update();
			}
		}
	}

	public void UpdatePrevousPositions()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 10; i++)
		{
			if (m_Player[i].m_Id != -1)
			{
				m_Player[i].m_PrevPosition = m_Player[i].m_Position;
			}
		}
	}

	public void ClearDamageRequests()
	{
		for (int i = 0; i < 10; i++)
		{
			if (m_Player[i].m_Id != -1)
			{
				m_Player[i].m_RequestedDamageAmount = 0;
			}
		}
	}

	public int GetPlayerExistsWithNetId(short netId)
	{
		for (int i = 0; i < 10; i++)
		{
			if (m_Player[i].m_Id != -1 && m_Player[i].m_NetId == netId)
			{
				return m_Player[i].m_Id;
			}
		}
		return -1;
	}

	public int NumPlayers()
	{
		int num = 0;
		for (int i = 0; i < 10; i++)
		{
			if (m_Player[i].m_Id != -1)
			{
				num++;
			}
		}
		return num;
	}

	public Player GetLocalPlayer()
	{
		if (g.m_App.m_NetworkSession != null)
		{
			if (((ReadOnlyCollection<LocalNetworkGamer>)(object)g.m_App.m_NetworkSession.LocalGamers).Count > 0)
			{
				for (int i = 0; i < 10; i++)
				{
					if (m_Player[i].m_Id != -1 && m_Player[i].m_NetId == ((NetworkGamer)((ReadOnlyCollection<LocalNetworkGamer>)(object)g.m_App.m_NetworkSession.LocalGamers)[0]).Id)
					{
						return m_Player[i];
					}
				}
			}
			return null;
		}
		if (m_Player[0].m_Id != -1)
		{
			return m_Player[0];
		}
		return null;
	}

	public short GetNextBotId()
	{
		m_NextBotId++;
		if (m_NextBotId > 250)
		{
			m_NextBotId = 0;
		}
		return (short)(m_NextBotId | 0x100);
	}

	public Player GetBot(short botId)
	{
		for (int i = 0; i < 10; i++)
		{
			if (m_Player[i].m_Bot && m_Player[i].m_NetId == botId && m_Player[i].m_Id != -1)
			{
				return m_Player[i];
			}
		}
		return null;
	}

	public bool BotExists(short botId)
	{
		for (int i = 0; i < 10; i++)
		{
			if (m_Player[i].m_Bot && m_Player[i].m_NetId == botId && m_Player[i].m_Id != -1)
			{
				return true;
			}
		}
		return false;
	}

	public void FindSpawnPos(int team, out Vector3 pos, out float rotY)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		TriggerEntity triggerEntity = null;
		int num = g.m_App.m_Rand.Next(10);
		for (int i = num; i < 10; i++)
		{
			string text = $"Team{team + 1}_Spawn{i + 1}";
			if (g.m_App.sceneInterface.ObjectManager.Find<TriggerEntity>(text, false, ref triggerEntity))
			{
				Matrix world = ((SceneEntity)triggerEntity).World;
				if (!PlayerNearPos(((Matrix)(ref world)).Translation, 6.4515996f))
				{
					Matrix world2 = ((SceneEntity)triggerEntity).World;
					pos = ((Matrix)(ref world2)).Translation;
					pos.Y += 0.85f;
					Matrix world3 = ((SceneEntity)triggerEntity).World;
					Vector3 forward = ((Matrix)(ref world3)).Forward;
					rotY = (float)Math.Atan2(forward.X, forward.Z);
					return;
				}
			}
		}
		for (int j = 0; j < num; j++)
		{
			string text = $"Team{team + 1}_Spawn{j + 1}";
			if (g.m_App.sceneInterface.ObjectManager.Find<TriggerEntity>(text, false, ref triggerEntity))
			{
				Matrix world4 = ((SceneEntity)triggerEntity).World;
				if (!PlayerNearPos(((Matrix)(ref world4)).Translation, 6.4515996f))
				{
					Matrix world5 = ((SceneEntity)triggerEntity).World;
					pos = ((Matrix)(ref world5)).Translation;
					pos.Y += 0.85f;
					Matrix world6 = ((SceneEntity)triggerEntity).World;
					Vector3 forward2 = ((Matrix)(ref world6)).Forward;
					rotY = (float)Math.Atan2(forward2.X, forward2.Z);
					return;
				}
			}
		}
		pos = new Vector3(0f, 2f, 0f);
		rotY = 0f;
	}

	public bool PlayerNearPos(Vector3 pos, float radiusSq)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 10; i++)
		{
			if (m_Player[i].m_Id != -1)
			{
				Vector3 val = pos - m_Player[i].m_Position;
				float num = ((Vector3)(ref val)).LengthSquared();
				if (num < radiusSq)
				{
					return true;
				}
			}
		}
		return false;
	}

	public Player GetPlayerNearMe(int id, float radiusSq)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = g.m_PlayerManager.m_Player[id].m_Position;
		for (int i = 0; i < 10; i++)
		{
			if (m_Player[i].m_Id != -1 && m_Player[i].m_Id != id)
			{
				Vector3 val = position - m_Player[i].m_Position;
				float num = ((Vector3)(ref val)).LengthSquared();
				if (num < radiusSq)
				{
					return m_Player[i];
				}
			}
		}
		return null;
	}

	public void InitParticleSystems()
	{
		for (int i = 0; i < 10; i++)
		{
			m_Player[i].InitParticleSystems();
		}
	}

	public void DrawTeamMatesNames(SpriteBatch spriteBatch)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.Zero;
		float num = 0f;
		Vector3 zero = Vector3.Zero;
		if (g.m_PlayerManager.GetLocalPlayer() == null || g.m_PlayerManager.GetLocalPlayer().m_ViewSceneObject == null)
		{
			return;
		}
		for (int i = 0; i < 10; i++)
		{
			if (g.m_PlayerManager.m_Player[i].m_Id == -1 || g.m_PlayerManager.m_Player[i].m_Id == g.m_PlayerManager.GetLocalPlayer().m_Id || g.m_PlayerManager.m_Player[i].m_Team != g.m_PlayerManager.GetLocalPlayer().m_Team)
			{
				continue;
			}
			val = g.m_PlayerManager.m_Player[i].m_Position - g.m_PlayerManager.GetLocalPlayer().m_Position;
			Matrix world = ((SceneEntity)g.m_PlayerManager.GetLocalPlayer().m_ViewSceneObject).World;
			float num2 = Vector3.Dot(((Matrix)(ref world)).Forward, val);
			if (num2 < 0f)
			{
				continue;
			}
			num = ((Vector3)(ref val)).LengthSquared();
			if (num < 645.16f)
			{
				zero = g.m_CameraManager.WorldToScreen(g.m_PlayerManager.m_Player[i].m_Position + new Vector3(0f, 1.15f, 0f));
				if (zero.X > 50f && zero.X < 974f && zero.Y > 50f && zero.Y < 526f)
				{
					spriteBatch.DrawString(g.m_App.smallfont, g.m_PlayerManager.m_Player[i].GetName(), new Vector2(zero.X, zero.Y), Color.White);
				}
			}
		}
	}

	public void SortPlayers()
	{
		for (int i = 0; i < 10; i++)
		{
			m_SortedPlayer[i] = m_Player[i];
		}
		bool flag = false;
		do
		{
			flag = false;
			for (int j = 1; j < 10; j++)
			{
				if (m_SortedPlayer[j - 1].m_Kills < m_SortedPlayer[j].m_Kills)
				{
					Player player = m_SortedPlayer[j - 1];
					m_SortedPlayer[j - 1] = m_SortedPlayer[j];
					m_SortedPlayer[j] = player;
					flag = true;
				}
				if (m_SortedPlayer[j - 1].m_Kills == m_SortedPlayer[j].m_Kills && m_SortedPlayer[j - 1].m_Deaths > m_SortedPlayer[j].m_Deaths)
				{
					Player player2 = m_SortedPlayer[j - 1];
					m_SortedPlayer[j - 1] = m_SortedPlayer[j];
					m_SortedPlayer[j] = player2;
					flag = true;
				}
			}
		}
		while (flag);
	}

	public void ReInitTorches()
	{
		for (int i = 0; i < 10; i++)
		{
			if (m_Player[i].m_Id != -1)
			{
				m_Player[i].InitTorchLight();
			}
		}
	}

	public void ReInitBots()
	{
		for (int i = 0; i < 10; i++)
		{
			if (m_Player[i].m_Id != -1 && m_Player[i].m_Bot)
			{
				m_Player[i].BotSetClassAndTeam();
				m_Player[i].m_RequestSendTeam = true;
				m_Player[i].m_RequestSendClass = true;
				m_Player[i].InitBot();
			}
		}
	}

	public void ReInitPlayers()
	{
		for (int i = 0; i < 10; i++)
		{
			if (m_Player[i].m_Id != -1)
			{
				m_Player[i].CleanItems();
			}
		}
	}

	public int NumBots()
	{
		int num = 0;
		for (int i = 0; i < 10; i++)
		{
			if (m_Player[i].m_Id != -1 && m_Player[i].m_Bot)
			{
				num++;
			}
		}
		return num;
	}

	public int NumPlayersOnTeams()
	{
		int num = 0;
		for (int i = 0; i < 10; i++)
		{
			if (m_Player[i].m_Id != -1 && !m_Player[i].m_Bot && m_Player[i].m_Team != 0 && m_Player[i].m_Class != 0)
			{
				num++;
			}
		}
		return num;
	}

	public int NumSlayers()
	{
		int num = 0;
		for (int i = 0; i < 10; i++)
		{
			if (m_Player[i].m_Id != -1 && m_Player[i].m_Team == Player.TEAM.Hunter)
			{
				num++;
			}
		}
		return num;
	}

	public int NumVampires()
	{
		int num = 0;
		for (int i = 0; i < 10; i++)
		{
			if (m_Player[i].m_Id != -1 && m_Player[i].m_Team == Player.TEAM.Vampire)
			{
				num++;
			}
		}
		return num;
	}

	public void RemoveBot()
	{
		int num = g.m_PlayerManager.NumSlayers();
		int num2 = g.m_PlayerManager.NumVampires();
		Player.TEAM tEAM = Player.TEAM.None;
		if (num > num2)
		{
			tEAM = Player.TEAM.Hunter;
		}
		else if (num < num2)
		{
			tEAM = Player.TEAM.Vampire;
		}
		for (int i = 0; i < 10; i++)
		{
			if (m_Player[i].m_Id != -1 && m_Player[i].m_Bot && m_Player[i].m_Team == tEAM)
			{
				m_Player[i].DeleteBot();
				return;
			}
		}
		for (int j = 0; j < 10; j++)
		{
			if (m_Player[j].m_Id != -1 && m_Player[j].m_Bot)
			{
				m_Player[j].DeleteBot();
				break;
			}
		}
	}
}
