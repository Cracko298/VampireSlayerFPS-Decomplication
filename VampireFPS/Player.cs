using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using BEPUphysics;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionTests;
using BEPUphysics.Constraints.TwoEntity.Joints;
using BEPUphysics.Constraints.TwoEntity.Motors;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.MathExtensions;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using BEPUphysicsDemos.AlternateMovement.Character;
using DPSF;
using Maths;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using SgMotion;
using SgMotion.Controllers;
using SynapseGaming.LightingSystem.Collision;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Lights;
using SynapseGaming.LightingSystem.Rendering;
using SynapseGaming.LightingSystem.Shadows;

namespace VampireFPS;

public class Player : Actor
{
	public enum STATE
	{
		JoinTeam,
		ChooseCharacter,
		InGame,
		LocalDeath,
		Intermission
	}

	public enum TEAM
	{
		None,
		Vampire,
		Hunter
	}

	public enum CLASS
	{
		None,
		FatherD,
		Molly,
		Edgar,
		Nina
	}

	public enum BOTACTION
	{
		Idle,
		Searching,
		Attacking,
		VampireAttacking,
		Dying,
		Dead,
		Staking
	}

	public const int ANIM_NONE = -1;

	public const int ANIM_RUN = 0;

	public const int ANIM_IDLE = 1;

	public const int ANIM_IDLE_CROUCH = 2;

	public const int ANIM_MOVE_CROUCH = 3;

	public const int ANIM_LEAP = 4;

	public const int ANIM_JUMP = 5;

	public const int VIEW_ANIM_NONE = -1;

	public const int VIEW_ANIM_IDLE = 0;

	public const int VIEW_ANIM_FIRE = 1;

	public const int VIEW_ANIM_FIRE_SHORT = 2;

	public const int VIEW_ANIM_RELOAD_SHORT = 3;

	public const int VIEW_ANIM_RELOAD_START = 4;

	public const int VIEW_ANIM_RELOAD_SHELL = 5;

	public const int VIEW_ANIM_RELOAD_END = 6;

	public const int VIEW_ANIM_RELOAD_CROSSBOW = 7;

	public const int VIEW_ANIM_IDLE_CRUCIFIX = 8;

	public const int VIEW_ANIM_USE_CRUCIFIX = 9;

	public const int VIEW_ANIM_IDLE_STAKE = 10;

	public const int VIEW_ANIM_USE_STAKE = 11;

	public const int VIEW_ANIM_HOLSTER = 12;

	public const int MAX_WEAPONS = 6;

	private const float SPAWN_INVINVIBILITY_TIME = 3f;

	private const float RESURRECT_INVINVIBILITY_TIME = 3f;

	public const int R_PROP = 1;

	public const int R_HAND = 24;

	public const int R_UPPERARM = 27;

	public const int R_PROP_VIEW = 34;

	public const int R_HAND_VIEW = 23;

	private const float LEAP_THESHOLD_SQ = 225f;

	private const float DEFAULT_BLEND_TIME = 0.2f;

	private const float RESPAWN_AFTER_STAKED_TIME = 10f;

	private const float RESPAWN_HUNTER_TIME = 10f;

	private const float RESURRECT_VAMPIRE_TIME = 5f;

	public const int HITZONE_NONE = 255;

	public const int HITZONE_TORSO = 1;

	public const int HITZONE_HEAD = 2;

	public const int HITZONE_LOWERBODY = 3;

	private const float BOT_ATTACK_RANGE = 127f;

	private const float BOT_BEHIND_ATTACK_RANGE = 20.32f;

	private const float BOT_STAKE_RANGE = 127f;

	private const int NUM_BOT_NAMES = 7;

	public float m_LAX;

	public float m_LAY;

	public float thismove;

	public float thisrotate;

	public CharacterController m_CharacterController;

	public AnimationController m_AnimationController;

	public AnimationController m_ViewAnimationController;

	public int m_Anim = -1;

	public int m_ViewAnim = -1;

	public short m_NetId;

	public sbyte m_Health = 100;

	public bool m_bStaked;

	public bool m_bRequestDied;

	public bool m_bRequestStaked;

	public Vector2 m_Movement;

	public float m_Turn;

	public bool m_Jump;

	public bool m_Leap;

	public int m_WeaponItemIndex;

	public int m_StartWeaponItemIndex;

	public int[] m_Weapons = new int[6];

	public SpotLight m_TorchLight;

	public SkinnedModel m_ViewModel;

	public SceneObject m_ViewSceneObject;

	public Vector3 m_FrameMove;

	public float m_PunchAngle;

	public bool UPDATEFULLMODEL_DEBUG;

	public bool m_bFired;

	public bool m_bTorchChanged;

	public bool m_bWeaponChanged;

	public bool m_Bot;

	public bool m_bRagdoll;

	public BloodQuadSprayParticleSystem m_BloodSpray;

	public ExplosionDebrisParticleSystem m_Gibs;

	public bool m_bRequestSendDamage;

	public sbyte m_RequestedDamageAmount;

	public short m_RequestedPlayerToDamageNetID = 255;

	public Vector3 m_RequestedHitPos = Vector3.Zero;

	public byte m_RequestedHitZone = byte.MaxValue;

	public short m_RequestedAttacker = 255;

	public bool m_RequestedStaked;

	public short m_LastAttackerNetId = 255;

	public bool m_DEBUG_Invincible;

	public float m_InvincibilityTime;

	public STATE m_State;

	public TEAM m_Team;

	public CLASS m_Class;

	public float m_RespawnTimer;

	public float m_FootstepTime;

	public int m_CurrentViewAnim = -1;

	public bool m_bRequestSendSpawn;

	public bool m_RequestResurrect;

	public bool m_Crouch;

	public bool m_RequestSendCrouch;

	public float m_SpinePitch;

	public int m_Kills;

	public int m_Deaths;

	public int m_XP;

	public int m_Rank;

	public int m_XPForNextRank;

	public bool m_RequestSendTeam;

	public bool m_RequestSendClass;

	public bool m_RequestSendScore;

	public bool m_AnimChanged;

	public bool m_RequestFeed;

	public Vector3 m_RequestFeedPosition;

	public float m_FeedTimeOut;

	public bool m_HasAmmoToGive;

	public bool m_RequestDelete;

	public bool m_RequestRankUp;

	public bool m_RequestCleanItems;

	public float m_ChangeTeamTime;

	public Entity m_HitZone_Head;

	public Entity m_HitZone_Torso;

	public Entity m_HitZone_LowerBody;

	public Entity m_Torso;

	public Entity m_UpperLeftArm;

	public Entity m_UpperRightArm;

	public Entity m_LowerLeftArm;

	public Entity m_LowerRightArm;

	public Entity m_UpperLeftLeg;

	public Entity m_UpperRightLeg;

	public Entity m_LowerLeftLeg;

	public Entity m_LowerRightLeg;

	public AngularMotor m_AngularMotorUpperLeftArmToTorso;

	public AngularMotor m_AngularMotorLowerLeftArmToUpperLeftArm;

	public AngularMotor m_AngularMotorUpperRightArmToTorso;

	public AngularMotor m_AngularMotorLowerRightArmToUpperRightArm;

	public AngularMotor m_AngularMotorUpperLeftLegToTorso;

	public AngularMotor m_AngularMotorLowerLeftLegToUpperLeftLeg;

	public AngularMotor m_AngularMotorUpperRightLegToTorso;

	public AngularMotor m_AngularMotorLowerRightLegToUpperRightLeg;

	public BallSocketJoint m_BallSocketJointUpperLeftArmToTorso;

	public BallSocketJoint m_BallSocketJointLowerLeftArmToUpperLeftArm;

	public BallSocketJoint m_BallSocketJointUpperRightArmToTorso;

	public BallSocketJoint m_BallSocketJointLowerRightArmToUpperRightArm;

	public BallSocketJoint m_BallSocketJointUpperLeftLegToTorso;

	public BallSocketJoint m_BallSocketJointLowerLeftLegToUpperLeftLeg;

	public BallSocketJoint m_BallSocketJointUpperRightLegToTorso;

	public BallSocketJoint m_BallSocketJointLowerRightLegToUpperRightLeg;

	private CollisionGroup m_RagdollGroup;

	private Vector3 m_RagdollCreatePosition;

	private float m_BodyThumpTime;

	private BOTACTION m_PrevBotAction;

	private BOTACTION m_BotAction;

	private int m_TargetNode;

	private int m_LastSafeNode;

	private bool m_TargetDirectionForward;

	private Vector3 m_BotVecTarget;

	private Vector3 m_BotVecMove;

	private float m_BotTargetRotY;

	private float m_BotSpeed;

	private float m_NextActionTime;

	private float m_AttackTimeout;

	private int m_EnemyId;

	private bool m_BotAllowFire;

	private bool m_BotAllowMove;

	private byte m_BotNameIdx;

	private float m_LookForEnemyTime;

	private float m_BotLeapTime;

	private float m_BeserkTime;

	private Vector3 m_BotAimVector;

	public static string[] BotMaleVampireNames = new string[7] { "Vlad", "Herrick", "Osvaldo", "Dracula", "Max", "Dirk", "Daemon" };

	public static string[] BotFemaleVampireNames = new string[7] { "Eloise", "Marie", "Akasha", "Drusilla", "Kiana", "Imani", "Judith" };

	public static string[] BotMaleSlayerNames = new string[7] { "Alucard", "Van Helsing", "Abraham", "Blade", "Seth", "Drake", "Peter" };

	public static string[] BotFemaleSlayerNames = new string[7] { "Buffy", "Faith", "Helen", "Eliza", "Cindy", "Karina", "Annette" };

	public Player()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		m_LAX = 0f;
		m_LAY = 0f;
		m_Id = -1;
		m_NetId = 255;
		m_Health = 100;
		m_bRequestDied = false;
		m_bRequestStaked = false;
		m_WeaponItemIndex = -1;
		m_ViewSceneObject = null;
		m_FrameMove = Vector3.Zero;
		m_PunchAngle = 0f;
		m_bFired = false;
		m_Bot = false;
		m_bTorchChanged = false;
		m_bWeaponChanged = false;
		m_bRequestSendDamage = false;
		m_RequestedDamageAmount = 0;
		m_RequestedPlayerToDamageNetID = 255;
		m_RequestedHitPos = Vector3.Zero;
		m_RequestedHitZone = byte.MaxValue;
		m_RequestedAttacker = 255;
		m_RequestedStaked = false;
		m_LastAttackerNetId = 255;
		m_RespawnTimer = 0f;
		m_Crouch = false;
		m_RequestSendCrouch = false;
		m_SpinePitch = 0f;
		m_Kills = 0;
		m_Deaths = 0;
		m_XP = 0;
		m_Rank = 1;
		m_XPForNextRank = 0;
		m_RequestSendScore = false;
		m_AnimChanged = false;
		m_RequestFeed = false;
		m_RequestFeedPosition = Vector3.Zero;
		m_FeedTimeOut = 0f;
		m_HasAmmoToGive = false;
		m_RequestRankUp = false;
		m_RequestCleanItems = false;
		m_ChangeTeamTime = 0f;
	}

	public void SetViewModel()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		switch (m_Class)
		{
		case CLASS.FatherD:
		case CLASS.Molly:
			m_ViewModel = g.m_PlayerManager.m_ViewModel;
			break;
		case CLASS.Edgar:
		case CLASS.Nina:
			m_ViewModel = g.m_PlayerManager.m_ViewModel_Claws;
			break;
		}
		m_ViewAnimationController = new AnimationController(m_ViewModel.SkeletonBones);
		PlayViewAnim(0, loop: true, 0f);
		SceneObject val = new SceneObject(m_ViewModel.Model);
		((SceneEntity)val).UpdateType = (UpdateType)1;
		val.Visibility = (ObjectVisibility)1;
		val.StaticLightingType = (StaticLightingType)1;
		val.CollisionType = (CollisionType)0;
		val.AffectedByGravity = false;
		((SceneEntity)val).Name = $"Player{m_Id}";
		((SceneEntity)val).World = Matrix.Identity;
		m_ViewSceneObject = val;
		((ISubmit<SceneEntity>)(object)g.m_App.sceneInterface.ObjectManager).Submit((SceneEntity)(object)m_ViewSceneObject);
		m_ViewAnimationController.TranslationInterpolation = (InterpolationMode)0;
		m_ViewAnimationController.OrientationInterpolation = (InterpolationMode)0;
		m_ViewAnimationController.ScaleInterpolation = (InterpolationMode)0;
		foreach (RenderableMesh item in (ReadOnlyCollection<RenderableMesh>)(object)m_ViewSceneObject.RenderableMeshes)
		{
			BoundingBox meshBoundingBox = item.MeshBoundingBox;
			item.MeshBoundingBox = new BoundingBox(new Vector3(meshBoundingBox.Min.Z, meshBoundingBox.Min.Y + 2.159f - 1f, meshBoundingBox.Min.X), new Vector3(meshBoundingBox.Max.Z, meshBoundingBox.Max.Y + 2.159f + 1f, meshBoundingBox.Max.X));
		}
		((SceneEntity)m_ViewSceneObject).CalculateBounds();
		SetWeapon(m_StartWeaponItemIndex);
	}

	public void SpawnLocal()
	{
		SetViewModel();
		Spawn();
	}

	public void Spawn()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		SetState(STATE.InGame);
		Vector3 pos = Vector3.Zero;
		float rotY = 0f;
		int num = -1;
		if (m_Team == TEAM.Hunter)
		{
			num = 0;
		}
		else if (m_Team == TEAM.Vampire)
		{
			num = 1;
		}
		if (num != -1)
		{
			g.m_PlayerManager.FindSpawnPos(num, out pos, out rotY);
			if (m_bRagdoll)
			{
				DisableRagdoll();
			}
			m_Position = pos;
			m_NetworkPosition = pos;
			m_Rotation.Y = rotY;
			m_NetworkRotation = rotY;
			((Entity)m_CharacterController.Body).Position = m_Position;
		}
		EnableCollisionAndGravity();
		m_InvincibilityTime = (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds + 3f;
		ResetAllAmmo();
		SetWeapon(m_StartWeaponItemIndex);
		m_bRequestSendSpawn = true;
		if (m_Crouch)
		{
			Crouch();
		}
		((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)(object)m_BloodSpray).Enabled = false;
		((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)(object)m_BloodSpray).Visible = false;
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)(object)m_Gibs).Enabled = false;
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)(object)m_Gibs).Visible = false;
		if (m_Team == TEAM.Vampire)
		{
			m_CharacterController.JumpSpeed = 20f;
			m_CharacterController.SlidingJumpSpeed = 20f;
		}
		else
		{
			m_CharacterController.JumpSpeed = 9f;
			m_CharacterController.SlidingJumpSpeed = 9f;
		}
		m_bStaked = false;
		m_TargetNode = -1;
	}

	public void PeerSpawned(Vector3 pos)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (m_bRagdoll)
		{
			DisableRagdoll();
		}
		m_Position = pos;
		m_NetworkPosition = pos;
		if (m_SceneObject != null)
		{
			m_SceneObject.Visibility = (ObjectVisibility)1;
		}
		SetWeapon(m_Weapons[0]);
		m_bStaked = false;
	}

	public void PeerResurrect(Vector3 pos)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (m_bRagdoll)
		{
			DisableRagdoll();
		}
		m_Position = pos;
		m_NetworkPosition = pos;
		g.m_App.AddSystemMessage($"{GetName()} RESURRECTED", Color.White);
		if (m_Class == CLASS.Edgar)
		{
			g.m_SoundManager.Play3D(53, m_Position);
		}
		else
		{
			g.m_SoundManager.Play3D(52, m_Position);
		}
	}

	public bool IsLocalPlayer()
	{
		if (g.m_App.m_NetworkSession == null)
		{
			return true;
		}
		if (((ReadOnlyCollection<LocalNetworkGamer>)(object)g.m_App.m_NetworkSession.LocalGamers).Count > 0 && (m_NetId & 0xFF) == ((NetworkGamer)((ReadOnlyCollection<LocalNetworkGamer>)(object)g.m_App.m_NetworkSession.LocalGamers)[0]).Id)
		{
			return true;
		}
		return false;
	}

	public void Delete()
	{
		m_Id = -1;
		m_NetId = -1;
		m_Bot = false;
		m_RequestDelete = false;
		if (m_bRagdoll)
		{
			DisableRagdoll();
		}
		for (int i = 0; i < 6; i++)
		{
			if (m_Weapons[i] != -1)
			{
				g.m_ItemManager.Delete(m_Weapons[i]);
				m_Weapons[i] = -1;
			}
		}
		m_WeaponItemIndex = -1;
		if (m_CharacterController != null)
		{
			g.m_App.m_Space.Remove((ISpaceObject)(object)m_CharacterController.Body);
			m_CharacterController = null;
		}
		if (m_ViewSceneObject != null)
		{
			((ISubmit<SceneEntity>)(object)g.m_App.sceneInterface.ObjectManager).Remove((SceneEntity)(object)m_ViewSceneObject);
			m_ViewSceneObject = null;
		}
		if (m_SceneObject != null)
		{
			((ISubmit<SceneEntity>)(object)g.m_App.sceneInterface.ObjectManager).Remove((SceneEntity)(object)m_SceneObject);
			m_SceneObject = null;
		}
		if (m_TorchLight != null)
		{
			((ISubmit<BaseLight>)(object)g.m_App.sceneInterface.LightManager).Remove((BaseLight)(object)m_TorchLight);
			m_TorchLight = null;
		}
		DestroyHitZones();
	}

	public void RequestDelete()
	{
		m_RequestDelete = true;
	}

	public void CleanItems()
	{
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		if (m_bRagdoll)
		{
			DisableRagdoll();
		}
		for (int i = 0; i < 6; i++)
		{
			if (m_Weapons[i] != -1)
			{
				g.m_ItemManager.Delete(m_Weapons[i]);
				m_Weapons[i] = -1;
			}
		}
		m_WeaponItemIndex = -1;
		if (m_ViewSceneObject != null)
		{
			((ISubmit<SceneEntity>)(object)g.m_App.sceneInterface.ObjectManager).Remove((SceneEntity)(object)m_ViewSceneObject);
			m_ViewSceneObject = null;
		}
		if (m_SceneObject != null)
		{
			((ISubmit<SceneEntity>)(object)g.m_App.sceneInterface.ObjectManager).Remove((SceneEntity)(object)m_SceneObject);
			m_SceneObject = null;
		}
		DestroyHitZones();
		m_Leap = false;
		m_Jump = false;
		if (m_Crouch)
		{
			Crouch();
		}
		m_FrameMove = Vector3.Zero;
		m_PunchAngle = 0f;
		m_bFired = false;
		m_bTorchChanged = false;
		m_bWeaponChanged = false;
		m_bRequestDied = false;
		m_bRequestStaked = false;
		m_bRequestSendDamage = false;
		m_RequestedDamageAmount = 0;
		m_RequestedPlayerToDamageNetID = 255;
		m_RequestedHitPos = Vector3.Zero;
		m_RequestedHitZone = byte.MaxValue;
		m_RequestedAttacker = 255;
		m_RequestedStaked = false;
		m_LastAttackerNetId = 255;
		m_RespawnTimer = 0f;
		m_RequestSendCrouch = false;
		m_SpinePitch = 0f;
		m_Kills = 0;
		m_Deaths = 0;
		m_RequestSendScore = false;
		m_AnimChanged = false;
		m_Anim = -1;
		m_State = STATE.Intermission;
		m_Team = TEAM.None;
		m_Class = CLASS.None;
		m_bRequestSendSpawn = false;
		m_RequestRankUp = false;
	}

	public void Render()
	{
	}

	public void Update()
	{
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		switch (m_State)
		{
		case STATE.Intermission:
			return;
		case STATE.JoinTeam:
		{
			bool flag = false;
			GameScreen[] screens = g.m_App.screenManager.GetScreens();
			for (int i = 0; i < screens.Length; i++)
			{
				if (screens[i] is JoinTeamMenuScreen)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				g.m_App.screenManager.AddScreen(new JoinTeamMenuScreen(g.m_App.m_NetworkSession), screens[0].ControllingPlayer);
			}
			break;
		}
		case STATE.InGame:
			UpdateInGame();
			UpdateHostBot();
			UpdatePeer();
			UpdateWeaponToBone();
			UpdateTorchLight();
			break;
		case STATE.LocalDeath:
			UpdateLocalDeath();
			UpdateHostBot();
			break;
		}
		m_Rotation.Y = MathHelper.WrapAngle(m_Rotation.Y);
		m_FrameMove = m_Position - m_PrevPosition;
		if (m_Position.Y < -100f && m_CharacterController != null)
		{
			Spawn();
		}
		if (m_RequestDelete)
		{
			g.m_App.AddSystemMessage($"{GetName()} (BOT {m_NetId} LEFT)", Color.White);
			Delete();
		}
	}

	public void UpdateInGame()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		if (IsLocalPlayer() && !m_Bot)
		{
			m_DEBUG_Invincible = false;
			float num = 60f * (float)g.m_App.m_GameTime.ElapsedGameTime.TotalSeconds;
			m_Position.X = ((Entity)m_CharacterController.Body).Position.X;
			m_Position.Z = ((Entity)m_CharacterController.Body).Position.Z;
			m_Position.Y = MathHelper.Lerp(m_Position.Y, ((Entity)m_CharacterController.Body).Position.Y, 0.3f);
			ref Vector3 rotation = ref m_Rotation;
			rotation.Y += (0f - m_Turn) * 0.04f * num * (g.m_App.m_OptionsHoriz * 0.4f);
			Matrix val = Matrix.CreateRotationY(m_Rotation.Y);
			_ = ((Matrix)(ref val)).Forward;
			Vector3 val2 = default(Vector3);
			((Vector3)(ref val2))._002Ector(m_Movement.X, 0f, m_Movement.Y);
			Vector3 val3 = Vector3.Transform(val2 * 0.05f, val);
			val3 *= num;
			Matrix val4 = Matrix.CreateTranslation(m_Position + val3);
			Matrix val5 = val * val4;
			m_Position = ((Matrix)(ref val5)).Translation;
			if (m_Crouch)
			{
				m_CharacterController.HorizontalMotionConstraint.CrouchingSpeed = 8f * ((Vector2)(ref m_Movement)).Length() * num;
			}
			else
			{
				m_CharacterController.HorizontalMotionConstraint.Speed = 12f * ((Vector2)(ref m_Movement)).Length() * num;
			}
			m_CharacterController.HorizontalMotionConstraint.MovementDirection = new Vector2(val3.X, val3.Z);
			((Entity)m_CharacterController.Body).OrientationMatrix = Matrix3X3.CreateFromMatrix(val);
			if (m_Jump)
			{
				m_CharacterController.Jump();
			}
			if (m_Leap && m_Team == TEAM.Vampire)
			{
				m_CharacterController.Leap();
			}
			((Matrix)(ref val4)).Translation = ((Matrix)(ref val4)).Translation + new Vector3(0f, -2.159f, 0f);
			((SceneEntity)m_ViewSceneObject).World = val * val4;
			UpdateViewAnimation();
			Matrix val6 = Matrix.CreateRotationY(3.14f) * Matrix.CreateRotationZ(1.57f) * Matrix.CreateRotationX(g.m_CameraManager.m_Pitch + MathHelper.ToRadians(-14f));
			m_ViewAnimationController.SetBoneController("Bip01_Neck", ref val6);
			Matrix identity = Matrix.Identity;
			m_ViewAnimationController.Update(g.m_App.m_GameTime.ElapsedGameTime, identity);
			m_ViewSceneObject.SkinBones = m_ViewAnimationController.SkinnedBoneTransforms;
			m_PunchAngle = MathHelper.Lerp(m_PunchAngle, 0f, 0.2f);
			if (IsHost())
			{
				UpdateHitZones();
			}
			UpdateLocalAnimForPeers(m_Position - m_PrevPosition);
			TryPickupAmmo();
			CheckPeerCollision();
		}
	}

	public void PlayLeapSFX()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		int num = g.m_App.m_Rand.Next(0, 3);
		g.m_SoundManager.Play3D(49 + num, m_Position);
	}

	public void PlayJumpSFX()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (m_Team == TEAM.Vampire)
		{
			g.m_SoundManager.Play3D(48, m_Position);
		}
	}

	private void UpdateHostBot()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		if (m_Bot && IsHost())
		{
			flag = true;
		}
		if (!flag)
		{
			return;
		}
		if (m_bStaked)
		{
			m_CharacterController.HorizontalMotionConstraint.Speed = 0f;
			m_CharacterController.HorizontalMotionConstraint.CrouchingSpeed = 0f;
			return;
		}
		m_Position = ((Entity)m_CharacterController.Body).Position;
		UpdateBot();
		m_CharacterController.HorizontalMotionConstraint.Speed = m_BotSpeed;
		m_CharacterController.HorizontalMotionConstraint.MovementDirection = new Vector2(m_BotVecMove.X, m_BotVecMove.Z);
		m_Position += m_BotVecMove;
		m_Rotation.Y = Fn.Clerp(m_Rotation.Y, m_BotTargetRotY, 0.2f);
		m_Rotation.Y = MathHelper.WrapAngle(m_Rotation.Y);
		Matrix val = Matrix.CreateTranslation(m_Position);
		((Matrix)(ref val)).Translation = ((Matrix)(ref val)).Translation + new Vector3(0f, -2.159f, 0f);
		Matrix val2 = Matrix.CreateRotationY(m_Rotation.Y);
		((SceneEntity)m_SceneObject).World = val2 * val;
		((Entity)m_CharacterController.Body).OrientationMatrix = Matrix3X3.CreateFromMatrix(val2);
		if ((g.m_PlayerManager.m_Frame & 1) == (m_Id & 1) && (int)m_SceneObject.Visibility != 0)
		{
			m_AnimationController.Speed = 2f;
			m_AnimationController.Update(g.m_App.m_GameTime.ElapsedGameTime, Matrix.Identity);
			m_SceneObject.SkinBones = m_AnimationController.SkinnedBoneTransforms;
		}
		if (m_Leap)
		{
			m_CharacterController.Leap();
		}
		UpdateBotAnimation(m_Position - m_PrevPosition);
		UpdateHitZones();
	}

	private void UpdatePeer()
	{
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		if (m_Bot && IsHost())
		{
			flag = true;
		}
		if (!IsLocalPlayer() && !flag && m_SceneObject != null)
		{
			if (m_bRagdoll)
			{
				UpdateRagdoll();
			}
			m_Position.X = MathHelper.Lerp(m_Position.X, m_NetworkPosition.X, 0.25f);
			m_Position.Y = MathHelper.Lerp(m_Position.Y, m_NetworkPosition.Y, 0.25f);
			m_Position.Z = MathHelper.Lerp(m_Position.Z, m_NetworkPosition.Z, 0.25f);
			m_Rotation.Y = Fn.Clerp(m_Rotation.Y, m_NetworkRotation, 0.3f);
			m_Rotation.Y = MathHelper.WrapAngle(m_Rotation.Y);
			if (m_Crouch)
			{
				ref Vector3 position = ref m_Position;
				position.Y += 0.08f;
			}
			Matrix val = Matrix.CreateTranslation(m_Position);
			((Matrix)(ref val)).Translation = ((Matrix)(ref val)).Translation + new Vector3(0f, -2.159f, 0f);
			Matrix val2 = Matrix.CreateRotationY(m_Rotation.Y);
			((SceneEntity)m_SceneObject).World = val2 * val;
			if ((g.m_PlayerManager.m_Frame & 1) == (m_Id & 1) && (int)m_SceneObject.Visibility != 0)
			{
				m_AnimationController.Speed = 2f;
				m_AnimationController.Update(g.m_App.m_GameTime.ElapsedGameTime, Matrix.Identity);
				m_SceneObject.SkinBones = m_AnimationController.SkinnedBoneTransforms;
			}
			UpdateHitZones();
			UpdateSpinePitch();
			UpdatePeerFootsteps();
		}
	}

	private void UpdateSpinePitch()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (m_Team == TEAM.Vampire)
		{
			return;
		}
		if (!m_bRagdoll && !m_Crouch)
		{
			m_SpinePitch = MathHelper.Lerp(m_SpinePitch, 0f, 0.2f);
			if (m_SpinePitch > 1E-05f)
			{
				Matrix val = Matrix.CreateRotationY(3.14f) * Matrix.CreateRotationZ(1.57f) * Matrix.CreateRotationX(m_SpinePitch);
				m_AnimationController.SetBoneController("Bip01_Spine1", ref val);
			}
			else
			{
				m_AnimationController.ClearBoneController("Bip01_Spine1");
			}
		}
		else
		{
			m_AnimationController.ClearBoneController("Bip01_Spine1");
		}
	}

	public void FireWeapon()
	{
		if (m_WeaponItemIndex != -1)
		{
			if (!g.m_ItemManager.m_Item[m_WeaponItemIndex].Fire())
			{
				m_bFired = false;
			}
			else if (this == g.m_PlayerManager.GetLocalPlayer())
			{
				int weaponFireAnim = g.m_ItemManager.GetWeaponFireAnim(m_WeaponItemIndex);
				bool weaponAnimShouldLoop = g.m_ItemManager.GetWeaponAnimShouldLoop(m_WeaponItemIndex);
				PlayViewAnim(weaponFireAnim, weaponAnimShouldLoop, 0f);
				m_PunchAngle += g.m_ItemManager.GetWeaponRecoil(m_WeaponItemIndex);
			}
		}
	}

	public void SimulateFireWeapon()
	{
		if (m_WeaponItemIndex != -1)
		{
			g.m_ItemManager.m_Item[m_WeaponItemIndex].SimulateFire();
			m_SpinePitch = MathHelper.ToRadians(10f);
		}
	}

	public void UpdateWeaponToBone()
	{
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		if (m_WeaponItemIndex == -1 || g.m_ItemManager.m_Item[m_WeaponItemIndex].m_SceneObject == null)
		{
			return;
		}
		if (this == g.m_PlayerManager.GetLocalPlayer() && !m_Bot && !UPDATEFULLMODEL_DEBUG && m_ViewAnimationController != null)
		{
			Matrix boneAbsoluteTransform = m_ViewAnimationController.GetBoneAbsoluteTransform("Bone01");
			((SceneEntity)g.m_ItemManager.m_Item[m_WeaponItemIndex].m_SceneObject).World = boneAbsoluteTransform * ((SceneEntity)m_ViewSceneObject).World;
			if (g.m_ItemManager.m_Item[m_WeaponItemIndex].m_CrossbowEmpty != null)
			{
				((SceneEntity)g.m_ItemManager.m_Item[m_WeaponItemIndex].m_CrossbowEmpty).World = boneAbsoluteTransform * ((SceneEntity)m_ViewSceneObject).World;
			}
		}
		else if (m_AnimationController != null)
		{
			Matrix boneAbsoluteTransform2 = m_AnimationController.GetBoneAbsoluteTransform("Bone01");
			((SceneEntity)g.m_ItemManager.m_Item[m_WeaponItemIndex].m_SceneObject).World = boneAbsoluteTransform2 * ((SceneEntity)m_SceneObject).World;
		}
	}

	public void ZoomWeapon(bool bZoom)
	{
		if (m_WeaponItemIndex != -1)
		{
			g.m_ItemManager.m_Item[m_WeaponItemIndex].Zoom(bZoom);
		}
	}

	public void InitTorchLight()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		m_TorchLight = new SpotLight();
		((BaseLight)m_TorchLight).LightingType = (LightingType)1;
		((BaseLight)m_TorchLight).DiffuseColor = new Vector3(10f, 10f, 9f);
		((BaseLight)m_TorchLight).Intensity = 0.3f;
		((PointLight)m_TorchLight).Radius = 40f;
		((BaseLight)m_TorchLight).FalloffStrength = 0.1f;
		((BaseLight)m_TorchLight).Enabled = false;
		((PointLight)m_TorchLight).ShadowType = (ShadowType)0;
		((ISubmit<BaseLight>)(object)g.m_App.sceneInterface.LightManager).Submit((BaseLight)(object)m_TorchLight);
	}

	public void UpdateTorchLight()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseLight)m_TorchLight).Enabled && m_WeaponItemIndex != -1 && g.m_ItemManager.m_Item[m_WeaponItemIndex].m_SceneObject != null)
		{
			Matrix world = ((SceneEntity)g.m_ItemManager.m_Item[m_WeaponItemIndex].m_SceneObject).World;
			((PointLight)m_TorchLight).Position = ((Matrix)(ref world)).Translation + ((Matrix)(ref world)).Forward * 0.1f + new Vector3(0f, 0.5f, 0f);
			m_TorchLight.Direction = ((Matrix)(ref world)).Forward;
		}
	}

	public void ToggleTorchLight()
	{
		if (m_Team == TEAM.Hunter && m_WeaponItemIndex != -1 && g.m_ItemManager.m_Item[m_WeaponItemIndex].m_Id != -1 && (g.m_ItemManager.m_Item[m_WeaponItemIndex].m_Type == 2 || g.m_ItemManager.m_Item[m_WeaponItemIndex].m_Type == 0 || g.m_ItemManager.m_Item[m_WeaponItemIndex].m_Type == 1))
		{
			((BaseLight)m_TorchLight).Enabled = !((BaseLight)m_TorchLight).Enabled;
		}
	}

	public void UpdateState()
	{
	}

	public void Reset()
	{
		m_Position.X = 0f;
		m_Position.Y = 0f;
	}

	public void UpdateLocalAnimForPeers(Vector3 move)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (((Vector3)(ref move)).LengthSquared() > 1E-06f)
		{
			Vector3 linearVelocity = ((Entity)m_CharacterController.Body).LinearVelocity;
			linearVelocity.Y = 0f;
			float num = ((Vector3)(ref linearVelocity)).LengthSquared();
			if (m_Anim != 4 && m_Team == TEAM.Vampire && !m_CharacterController.SupportFinder.HasTraction && num > 225f)
			{
				m_Anim = 4;
				m_AnimChanged = true;
			}
			else if (m_Anim != 5 && m_Anim != 4 && !m_CharacterController.SupportFinder.HasTraction)
			{
				m_Anim = 5;
				m_AnimChanged = true;
			}
			else if (m_Anim != 0 && m_CharacterController.SupportFinder.HasTraction)
			{
				m_Anim = 0;
				m_AnimChanged = true;
			}
		}
		else if (m_Anim != 1)
		{
			m_Anim = 1;
			m_AnimChanged = true;
		}
	}

	public void UpdateBotAnimation(Vector3 move)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		if (m_bRagdoll)
		{
			return;
		}
		if (((Vector3)(ref move)).LengthSquared() > 1E-06f)
		{
			Vector3 linearVelocity = ((Entity)m_CharacterController.Body).LinearVelocity;
			linearVelocity.Y = 0f;
			float num = ((Vector3)(ref linearVelocity)).LengthSquared();
			if (m_Anim != 4 && m_Team == TEAM.Vampire && !m_CharacterController.SupportFinder.HasTraction && num > 225f)
			{
				m_AnimationController.CrossFade(((ReadOnlyDictionary<string, AnimationClip>)(object)m_Model.AnimationClips).Values[4], TimeSpan.FromSeconds(0.20000000298023224));
				m_Anim = 4;
				m_AnimChanged = true;
				int num2 = g.m_App.m_Rand.Next(0, 3);
				g.m_SoundManager.Play3D(49 + num2, m_Position);
			}
			else if (m_Anim != 5 && m_Anim != 4 && !m_CharacterController.SupportFinder.HasTraction)
			{
				m_AnimationController.CrossFade(((ReadOnlyDictionary<string, AnimationClip>)(object)m_Model.AnimationClips).Values[5], TimeSpan.FromSeconds(0.20000000298023224));
				m_Anim = 5;
				m_AnimChanged = true;
				if (m_Team == TEAM.Vampire)
				{
					g.m_SoundManager.Play3D(48, m_Position);
				}
			}
			else if (m_Anim != 0 && m_CharacterController.SupportFinder.HasTraction)
			{
				float num3 = 0.2f;
				if (m_Anim == 4)
				{
					num3 = 0f;
				}
				if (m_Crouch)
				{
					m_AnimationController.CrossFade(((ReadOnlyDictionary<string, AnimationClip>)(object)m_Model.AnimationClips).Values[3], TimeSpan.FromSeconds(num3));
				}
				else
				{
					m_AnimationController.CrossFade(((ReadOnlyDictionary<string, AnimationClip>)(object)m_Model.AnimationClips).Values[0], TimeSpan.FromSeconds(num3));
				}
				m_Anim = 0;
				m_AnimChanged = true;
			}
		}
		else if (m_Anim != 1)
		{
			if (m_Crouch)
			{
				m_AnimationController.CrossFade(((ReadOnlyDictionary<string, AnimationClip>)(object)m_Model.AnimationClips).Values[2], TimeSpan.FromSeconds(0.20000000298023224));
			}
			else
			{
				m_AnimationController.CrossFade(((ReadOnlyDictionary<string, AnimationClip>)(object)m_Model.AnimationClips).Values[1], TimeSpan.FromSeconds(0.20000000298023224));
			}
			m_Anim = 1;
			m_AnimChanged = true;
		}
		if (m_Team == TEAM.Hunter && ((Vector3)(ref move)).LengthSquared() > 0.01f && m_CharacterController.SupportFinder.HasTraction)
		{
			float num4 = (float)m_AnimationController.Time.TotalSeconds;
			float num5 = (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds;
			float num6 = 0.36f;
			float num7 = 0.04f;
			if ((num4 > num6 * 1f && num4 < num6 * 1f + num7) || (num4 > num6 * 2f && num4 < num6 * 2f + num7) || (num4 > num6 * 3f && num4 < num6 * 3f + num7) || (num4 > num6 * 4f && num4 < num6 * 4f + num7 && m_FootstepTime < num5))
			{
				int num8 = g.m_App.m_Rand.Next(6);
				g.m_SoundManager.Play3D(15 + num8, m_Position, GetMoveVol());
				m_FootstepTime = num5 + 0.2f;
			}
		}
	}

	public void UpdatePeerFootsteps()
	{
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		if (m_Team == TEAM.Hunter && m_Anim == 0)
		{
			float num = (float)m_AnimationController.Time.TotalSeconds;
			float num2 = (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds;
			float num3 = 0.36f;
			float num4 = 0.04f;
			if ((num > num3 * 1f && num < num3 * 1f + num4) || (num > num3 * 2f && num < num3 * 2f + num4) || (num > num3 * 3f && num < num3 * 3f + num4) || (num > num3 * 4f && num < num3 * 4f + num4 && m_FootstepTime < num2))
			{
				int num5 = g.m_App.m_Rand.Next(6);
				g.m_SoundManager.Play3D(15 + num5, m_Position, GetMoveVol());
				m_FootstepTime = num2 + 0.2f;
			}
		}
	}

	public void PeerSetAnim(int anim)
	{
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		if (anim == 255 || m_AnimationController == null || m_Model == null)
		{
			return;
		}
		m_Anim = anim;
		switch (anim)
		{
		case 1:
			if (m_Crouch)
			{
				m_AnimationController.CrossFade(((ReadOnlyDictionary<string, AnimationClip>)(object)m_Model.AnimationClips).Values[2], TimeSpan.FromSeconds(0.20000000298023224));
			}
			else
			{
				m_AnimationController.CrossFade(((ReadOnlyDictionary<string, AnimationClip>)(object)m_Model.AnimationClips).Values[1], TimeSpan.FromSeconds(0.20000000298023224));
			}
			break;
		case 0:
			if (m_Crouch)
			{
				m_AnimationController.CrossFade(((ReadOnlyDictionary<string, AnimationClip>)(object)m_Model.AnimationClips).Values[3], TimeSpan.FromSeconds(0.20000000298023224));
			}
			else
			{
				m_AnimationController.CrossFade(((ReadOnlyDictionary<string, AnimationClip>)(object)m_Model.AnimationClips).Values[0], TimeSpan.FromSeconds(0.20000000298023224));
			}
			break;
		case 5:
			if (m_Team == TEAM.Vampire)
			{
				g.m_SoundManager.Play3D(48, m_Position);
			}
			m_AnimationController.CrossFade(((ReadOnlyDictionary<string, AnimationClip>)(object)m_Model.AnimationClips).Values[anim], TimeSpan.FromSeconds(0.20000000298023224));
			break;
		case 4:
		{
			int num = g.m_App.m_Rand.Next(0, 3);
			g.m_SoundManager.Play3D(49 + num, m_Position);
			m_AnimationController.CrossFade(((ReadOnlyDictionary<string, AnimationClip>)(object)m_Model.AnimationClips).Values[anim], TimeSpan.FromSeconds(0.20000000298023224));
			break;
		}
		default:
			m_AnimationController.CrossFade(((ReadOnlyDictionary<string, AnimationClip>)(object)m_Model.AnimationClips).Values[anim], TimeSpan.FromSeconds(0.20000000298023224));
			break;
		}
	}

	public float GetMoveVol()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = m_Position - m_PrevPosition;
		val.Y = 0f;
		float num = ((Vector3)(ref val)).LengthSquared() * 200f;
		return MathHelper.Clamp(num, 0.2f, 1f);
	}

	public void UpdateViewAnimation()
	{
		if (!m_ViewAnimationController.IsPlaying)
		{
			switch (m_CurrentViewAnim)
			{
			case 1:
			case 2:
			case 3:
			case 7:
				PlayViewAnim(0, loop: true, 0f);
				break;
			case 9:
				PlayViewAnim(8, loop: true, 0f);
				break;
			case 11:
				PlayViewAnim(10, loop: true, 0f);
				break;
			case 12:
				ChangeWeapon();
				break;
			case 4:
			case 5:
			case 6:
			case 8:
			case 10:
				break;
			}
		}
	}

	public void PlayViewAnim(int animId, bool loop, float blend)
	{
		if (m_ViewAnimationController != null)
		{
			if (blend == 0f || m_CurrentViewAnim == -1)
			{
				m_ViewAnimationController.StartClip(((ReadOnlyDictionary<string, AnimationClip>)(object)m_ViewModel.AnimationClips).Values[animId]);
			}
			else
			{
				m_ViewAnimationController.CrossFade(((ReadOnlyDictionary<string, AnimationClip>)(object)m_ViewModel.AnimationClips).Values[animId], TimeSpan.FromSeconds(blend));
			}
			m_ViewAnimationController.LoopEnabled = loop;
			m_CurrentViewAnim = animId;
		}
	}

	public void PlayViewIdleAnim(float blend)
	{
		switch (g.m_ItemManager.m_Item[m_WeaponItemIndex].m_Type)
		{
		case 4:
			PlayViewAnim(8, loop: false, blend);
			break;
		case 5:
			PlayViewAnim(10, loop: false, blend);
			break;
		default:
			PlayViewAnim(0, loop: false, blend);
			break;
		}
	}

	public void Serialize(PacketWriter packetWriter)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (packetWriter == null)
		{
			throw new ArgumentNullException("packetWriter");
		}
		packetWriter.Write(m_Position);
		((BinaryWriter)(object)packetWriter).Write(m_Rotation.Y);
	}

	public void RequestDamageOther(int hitZone, int damage, Player playerToDamage, Vector3 vctHitPos)
	{
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		if (m_Team == playerToDamage.m_Team || playerToDamage.m_bStaked)
		{
			return;
		}
		int num = -1;
		if (m_WeaponItemIndex != -1 && g.m_ItemManager.m_Item[m_WeaponItemIndex].m_Id != -1)
		{
			num = g.m_ItemManager.m_Item[m_WeaponItemIndex].m_Type;
			if (num == 2 && hitZone == 2)
			{
				damage *= 5;
			}
			if (num == 1 && hitZone == 2)
			{
				damage += 10;
			}
			if (num == 0 && hitZone == 2)
			{
				damage += 10;
			}
			if (damage > 127)
			{
				damage = 127;
			}
		}
		int num2 = m_RequestedDamageAmount + damage;
		if (num2 > 127)
		{
			num2 = 127;
		}
		if (num == 5 && (playerToDamage.m_bRagdoll || playerToDamage.m_Health <= 0) && playerToDamage.m_Team == TEAM.Vampire)
		{
			m_RequestedStaked = true;
		}
		else
		{
			m_RequestedStaked = false;
		}
		if (m_RequestedStaked || (!playerToDamage.m_bRagdoll && playerToDamage.m_Health > 0))
		{
			m_bRequestSendDamage = true;
			m_RequestedDamageAmount = (sbyte)num2;
			m_RequestedPlayerToDamageNetID = playerToDamage.m_NetId;
			m_RequestedHitZone = (byte)hitZone;
			m_RequestedHitPos = vctHitPos;
			m_RequestedAttacker = m_NetId;
			m_XP += damage;
			CheckRankUp();
			playerToDamage.DoDamage((sbyte)damage, (byte)hitZone, vctHitPos, m_NetId, m_RequestedStaked);
			if ((m_RequestedStaked || m_Team == TEAM.Vampire) && !m_Bot)
			{
				g.m_App.m_RumbleFrames = 10;
			}
		}
	}

	public void DoDamage(sbyte damage, byte hitZone, Vector3 vctHitPos, short attackerNetId, bool bStaked)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		if (m_InvincibilityTime > (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds)
		{
			return;
		}
		if (bStaked)
		{
			DoStakeVampire(damage, hitZone, vctHitPos, attackerNetId);
		}
		else
		{
			if (m_Health <= 0)
			{
				return;
			}
			m_RequestedHitPos = vctHitPos;
			m_RequestedHitZone = hitZone;
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(0f, 0f, 1f);
			int playerExistsWithNetId = g.m_PlayerManager.GetPlayerExistsWithNetId(attackerNetId);
			if (playerExistsWithNetId != -1 && g.m_PlayerManager.m_Player[playerExistsWithNetId].m_Id != -1)
			{
				val = m_Position - g.m_PlayerManager.m_Player[playerExistsWithNetId].m_Position;
				((Vector3)(ref val)).Normalize();
			}
			if (g.m_App.m_OptionsBlood)
			{
				((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)(object)m_BloodSpray).Enabled = true;
				((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)(object)m_BloodSpray).Visible = true;
				((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)(object)m_BloodSpray).Emitter.BurstParticles = damage / 2;
				((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)(object)m_BloodSpray).Emitter.PositionData.Position = vctHitPos;
				((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)(object)m_BloodSpray).Emitter.PositionData.Velocity = Vector3.Zero;
				((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)(object)m_BloodSpray).LerpEmittersPositionAndOrientationOnNextUpdate = false;
				m_BloodSpray.Normal = -val * 10f;
			}
			int num = g.m_App.m_Rand.Next(2);
			g.m_SoundManager.Play3D(13 + num, vctHitPos);
			if (m_Team == TEAM.Hunter)
			{
				g.m_SoundManager.Play3D(44 + num, vctHitPos);
			}
			bool flag = false;
			if (!m_Bot && g.m_PlayerManager.GetLocalPlayer().m_NetId == m_NetId)
			{
				flag = true;
			}
			if (m_Bot && IsHost())
			{
				BotTookDamage();
				flag = true;
			}
			if (!flag)
			{
				return;
			}
			if (!m_DEBUG_Invincible)
			{
				m_Health -= damage;
			}
			m_LastAttackerNetId = attackerNetId;
			if (m_Health > 0)
			{
				return;
			}
			m_bRequestDied = true;
			m_Health = 0;
			if (m_Bot)
			{
				StartLocalBotDeath();
				Kill(m_LastAttackerNetId);
				return;
			}
			StartLocalDeath(val);
			ShowKillMessage(m_LastAttackerNetId);
			if (m_Team == TEAM.Hunter)
			{
				m_Deaths++;
				if (playerExistsWithNetId != -1 && g.m_PlayerManager.m_Player[playerExistsWithNetId].m_Id != -1)
				{
					g.m_PlayerManager.m_Player[playerExistsWithNetId].m_Kills++;
				}
			}
			g.m_App.m_RumbleFrames = 10;
		}
	}

	public void DoStakeVampire(sbyte damage, byte hitZone, Vector3 vctHitPos, short attackerNetId)
	{
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(0f, 0f, 1f);
		int playerExistsWithNetId = g.m_PlayerManager.GetPlayerExistsWithNetId(attackerNetId);
		if (playerExistsWithNetId != -1 && g.m_PlayerManager.m_Player[playerExistsWithNetId].m_Id != -1)
		{
			val = m_Position - g.m_PlayerManager.m_Player[playerExistsWithNetId].m_Position;
			((Vector3)(ref val)).Normalize();
		}
		if (g.m_App.m_OptionsBlood)
		{
			((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)(object)m_BloodSpray).Enabled = true;
			((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)(object)m_BloodSpray).Visible = true;
			((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)(object)m_BloodSpray).Emitter.BurstParticles = 50;
			((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)(object)m_BloodSpray).Emitter.PositionData.Position = vctHitPos;
			((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)(object)m_BloodSpray).Emitter.PositionData.Velocity = Vector3.Zero;
			((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)(object)m_BloodSpray).LerpEmittersPositionAndOrientationOnNextUpdate = false;
			m_BloodSpray.Normal = -val * 10f;
			((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)(object)m_Gibs).Enabled = true;
			((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)(object)m_Gibs).Visible = true;
			((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)(object)m_Gibs).Emitter.BurstParticles = 50;
			((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)(object)m_Gibs).Emitter.PositionData.Position = vctHitPos;
			((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)(object)m_Gibs).Emitter.PositionData.Velocity = Vector3.Zero;
			((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)(object)m_Gibs).LerpEmittersPositionAndOrientationOnNextUpdate = false;
		}
		g.m_SoundManager.Play3D(43, vctHitPos);
		if (m_Class == CLASS.Edgar)
		{
			g.m_SoundManager.Play3D(47, vctHitPos);
		}
		else
		{
			g.m_SoundManager.Play3D(46, vctHitPos);
		}
		bool flag = false;
		if (!m_Bot && g.m_PlayerManager.GetLocalPlayer().m_NetId == m_NetId)
		{
			flag = true;
		}
		if (m_Bot && IsHost())
		{
			flag = true;
		}
		if (!flag)
		{
			return;
		}
		m_LastAttackerNetId = attackerNetId;
		m_bRequestStaked = true;
		m_RespawnTimer = (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds + 10f;
		m_bStaked = true;
		if (m_Bot)
		{
			Staked(m_LastAttackerNetId);
			return;
		}
		ShowStakedMessage(m_LastAttackerNetId);
		m_Deaths++;
		if (playerExistsWithNetId != -1 && g.m_PlayerManager.m_Player[playerExistsWithNetId].m_Id != -1)
		{
			g.m_PlayerManager.m_Player[playerExistsWithNetId].m_Kills++;
		}
	}

	public void StartLocalDeath(Vector3 vctDir)
	{
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		if (m_ViewSceneObject != null)
		{
			SetState(STATE.LocalDeath);
			m_ViewSceneObject.Visibility = (ObjectVisibility)0;
			if (m_TorchLight != null)
			{
				((BaseLight)m_TorchLight).Enabled = false;
			}
			if (m_WeaponItemIndex != -1)
			{
				g.m_ItemManager.m_Item[m_WeaponItemIndex].Hide();
			}
			if (m_Team == TEAM.Hunter)
			{
				m_RespawnTimer = (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds + 10f;
			}
			else
			{
				m_RespawnTimer = (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds + 5f;
			}
			m_RagdollCreatePosition = m_Position;
			((Entity)m_CharacterController.Body).LinearVelocity = vctDir * 30f;
		}
	}

	public void StartLocalBotDeath()
	{
		SetState(STATE.LocalDeath);
		if (m_TorchLight != null)
		{
			((BaseLight)m_TorchLight).Enabled = false;
		}
		if (m_WeaponItemIndex != -1)
		{
			g.m_ItemManager.m_Item[m_WeaponItemIndex].Hide();
		}
		if (m_Team == TEAM.Hunter)
		{
			m_RespawnTimer = (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds + 10f;
		}
		else
		{
			m_RespawnTimer = (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds + 5f;
		}
		DisableCollisionAndGravity();
	}

	private void UpdateLocalDeath()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		m_Position.X = ((Entity)m_CharacterController.Body).Position.X;
		m_Position.Y = ((Entity)m_CharacterController.Body).Position.Y;
		m_Position.Z = ((Entity)m_CharacterController.Body).Position.Z;
		if (!m_Bot)
		{
			m_RagdollCreatePosition = m_Position;
		}
		m_CharacterController.HorizontalMotionConstraint.Speed = 0f;
		m_CharacterController.HorizontalMotionConstraint.CrouchingSpeed = 0f;
		if (m_RespawnTimer > (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds)
		{
			return;
		}
		m_Health = 100;
		if (m_ViewSceneObject != null)
		{
			m_ViewSceneObject.Visibility = (ObjectVisibility)1;
		}
		if (m_SceneObject != null)
		{
			m_SceneObject.Visibility = (ObjectVisibility)1;
			if (m_bRagdoll)
			{
				DisableRagdoll();
			}
		}
		if (m_bStaked || m_Team == TEAM.Hunter)
		{
			Spawn();
		}
		else
		{
			m_State = STATE.InGame;
			m_Health = 50;
			EnableCollisionAndGravity();
			m_RequestResurrect = true;
			m_Anim = -1;
			if (g.m_PlayerManager.GetLocalPlayer().m_Id == m_Id)
			{
				g.m_App.AddSystemMessage($"{GetName()} RESURRECTED", Color.LightGoldenrodYellow);
			}
			else
			{
				g.m_App.AddSystemMessage($"{GetName()} RESURRECTED", Color.White);
			}
			if (m_Class == CLASS.Edgar)
			{
				g.m_SoundManager.Play3D(53, m_Position);
			}
			else
			{
				g.m_SoundManager.Play3D(52, m_Position);
			}
			m_Position = m_RagdollCreatePosition;
			m_NetworkPosition = m_RagdollCreatePosition;
			((Entity)m_CharacterController.Body).Position = m_Position;
			m_InvincibilityTime = (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds + 3f;
		}
		m_bStaked = false;
	}

	public void Kill(short lastAttackerNetId)
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		EnableRagdoll();
		m_Leap = false;
		if (m_Torso == null)
		{
			return;
		}
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(0f, 0f, 1f);
		int playerExistsWithNetId = g.m_PlayerManager.GetPlayerExistsWithNetId(lastAttackerNetId);
		if (playerExistsWithNetId != -1 && g.m_PlayerManager.m_Player[playerExistsWithNetId].m_Id != -1)
		{
			val = m_Position - g.m_PlayerManager.m_Player[playerExistsWithNetId].m_Position;
			((Vector3)(ref val)).Normalize();
		}
		if (m_RequestedHitPos != Vector3.Zero && m_RequestedHitZone != byte.MaxValue)
		{
			switch (m_RequestedHitZone)
			{
			case 1:
				m_Torso.ApplyImpulse(m_RequestedHitPos, val * 800f);
				break;
			case 3:
				m_UpperRightLeg.ApplyImpulse(m_RequestedHitPos, val * 800f);
				break;
			default:
				m_Torso.ApplyImpulse(m_Torso.Position, val * 800f);
				break;
			}
		}
		else
		{
			m_Torso.ApplyImpulse(m_Torso.Position, val * 800f);
		}
		m_RequestedHitPos = Vector3.Zero;
		m_RequestedHitZone = byte.MaxValue;
		ShowKillMessage(lastAttackerNetId);
		if (m_Team == TEAM.Hunter)
		{
			m_Deaths++;
			if (playerExistsWithNetId != -1)
			{
				g.m_PlayerManager.m_Player[playerExistsWithNetId].m_Kills++;
			}
		}
	}

	public void Staked(short lastAttackerNetId)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (m_SceneObject != null)
		{
			m_SceneObject.Visibility = (ObjectVisibility)0;
			if (m_bRagdoll)
			{
				DisableRagdoll();
			}
		}
		m_RequestedHitPos = Vector3.Zero;
		m_RequestedHitZone = byte.MaxValue;
		m_bStaked = true;
		ShowStakedMessage(lastAttackerNetId);
		m_Deaths++;
		int playerExistsWithNetId = g.m_PlayerManager.GetPlayerExistsWithNetId(lastAttackerNetId);
		if (playerExistsWithNetId != -1)
		{
			g.m_PlayerManager.m_Player[playerExistsWithNetId].m_Kills++;
		}
	}

	public void ShowKillMessage(short attackerNetId)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		string text = "";
		int playerExistsWithNetId = g.m_PlayerManager.GetPlayerExistsWithNetId(attackerNetId);
		if (playerExistsWithNetId != -1)
		{
			Color c = Color.White;
			if (playerExistsWithNetId == g.m_PlayerManager.GetLocalPlayer().m_Id || g.m_PlayerManager.GetLocalPlayer().m_Id == m_Id)
			{
				c = Color.LightGoldenrodYellow;
			}
			text = ((m_Team != TEAM.Hunter) ? $"{g.m_PlayerManager.m_Player[playerExistsWithNetId].GetName()} downed {GetName()}" : $"{g.m_PlayerManager.m_Player[playerExistsWithNetId].GetName()} killed {GetName()}");
			g.m_App.AddSystemMessage(text, c);
		}
	}

	public void ShowStakedMessage(short attackerNetId)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		string text = "";
		int playerExistsWithNetId = g.m_PlayerManager.GetPlayerExistsWithNetId(attackerNetId);
		if (playerExistsWithNetId != -1)
		{
			Color c = Color.White;
			if (playerExistsWithNetId == g.m_PlayerManager.GetLocalPlayer().m_Id || g.m_PlayerManager.GetLocalPlayer().m_Id == m_Id)
			{
				c = Color.LightGoldenrodYellow;
			}
			text = $"{g.m_PlayerManager.m_Player[playerExistsWithNetId].GetName()} staked {GetName()}";
			g.m_App.AddSystemMessage(text, c);
		}
	}

	public void InitParticleSystems()
	{
		m_BloodSpray = new BloodQuadSprayParticleSystem((Game)(object)g.m_App);
		((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)(object)m_BloodSpray).Enabled = false;
		((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)(object)m_BloodSpray).AutoInitialize(((Game)g.m_App).GraphicsDevice, ((Game)g.m_App).Content, g.m_App.screenManager.SpriteBatch);
		g.m_App.m_ParticleSystemManager.AddParticleSystem((IDPSFParticleSystem)(object)m_BloodSpray);
		m_Gibs = new ExplosionDebrisParticleSystem((Game)(object)g.m_App);
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)(object)m_Gibs).Enabled = false;
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)(object)m_Gibs).AutoInitialize(((Game)g.m_App).GraphicsDevice, ((Game)g.m_App).Content, g.m_App.screenManager.SpriteBatch);
		g.m_App.m_ParticleSystemManager.AddParticleSystem((IDPSFParticleSystem)(object)m_Gibs);
		((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)(object)m_BloodSpray).Enabled = false;
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)(object)m_Gibs).Enabled = false;
	}

	public bool IsHost()
	{
		bool result = true;
		if (g.m_App.m_NetworkSession != null && !g.m_App.m_NetworkSession.IsHost)
		{
			result = false;
		}
		return result;
	}

	public void RagdollEnd()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		m_AnimationController.ClearBoneControllers();
		m_Position = m_RagdollCreatePosition;
		if (m_CharacterController != null && m_CharacterController.Body != null)
		{
			m_Position = m_RagdollCreatePosition;
			m_NetworkPosition = m_RagdollCreatePosition;
			((Entity)m_CharacterController.Body).Position = m_RagdollCreatePosition;
			((Entity)m_CharacterController.Body).LinearVelocity = Vector3.Zero;
			((Entity)m_CharacterController.Body).AngularVelocity = Vector3.Zero;
		}
	}

	public void SetState(STATE s)
	{
		m_State = s;
	}

	public void AutoChooseTeam()
	{
		int num = g.m_PlayerManager.NumSlayers();
		int num2 = g.m_PlayerManager.NumVampires();
		if (num > num2)
		{
			SetTeam(TEAM.Vampire);
		}
		else if (num2 > num)
		{
			SetTeam(TEAM.Hunter);
		}
		else if (g.m_App.m_Rand.NextDouble() > 0.5)
		{
			SetTeam(TEAM.Hunter);
		}
		else
		{
			SetTeam(TEAM.Vampire);
		}
	}

	public void SetTeam(TEAM t)
	{
		m_Team = t;
		m_RequestSendTeam = true;
	}

	public void PeerSetTeam(TEAM t)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (t != 0)
		{
			m_Team = t;
			if (m_Team == TEAM.Hunter)
			{
				g.m_App.AddSystemMessage($"{GetName()} CHANGED TO SLAYER", Color.White);
			}
			else
			{
				g.m_App.AddSystemMessage($"{GetName()} CHANGED TO VAMPIRE", Color.White);
			}
		}
	}

	public TEAM GetTeam()
	{
		return m_Team;
	}

	public void AutoChooseClass()
	{
		bool flag = g.m_App.m_Rand.NextDouble() > 0.5;
		if (m_Team == TEAM.Hunter)
		{
			if (flag)
			{
				SetClass(CLASS.FatherD);
			}
			else
			{
				SetClass(CLASS.Molly);
			}
		}
		else if (flag)
		{
			SetClass(CLASS.Edgar);
		}
		else
		{
			SetClass(CLASS.Nina);
		}
	}

	public void SetClass(CLASS c)
	{
		m_RequestSendClass = true;
		PeerSetClass(c);
	}

	public void PeerSetClass(CLASS c)
	{
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Expected O, but got Unknown
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Expected O, but got Unknown
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		if (c == CLASS.None)
		{
			return;
		}
		m_Class = c;
		m_Health = 100;
		GiveWeapons();
		CreateHitZones();
		if (IsLocalPlayer() && !m_Bot)
		{
			return;
		}
		switch (m_Class)
		{
		default:
			SetModel(g.m_PlayerManager.m_FullModel_FatherD);
			break;
		case CLASS.Molly:
			SetModel(g.m_PlayerManager.m_FullModel_SlayerF);
			break;
		case CLASS.Edgar:
			SetModel(g.m_PlayerManager.m_FullModel_VampM);
			break;
		case CLASS.Nina:
			SetModel(g.m_PlayerManager.m_FullModel_VampF);
			break;
		}
		m_Model.Model.CopyAbsoluteBoneTransformsTo(m_Transforms);
		m_AnimationController = new AnimationController(m_Model.SkeletonBones);
		m_AnimationController.StartClip(((ReadOnlyDictionary<string, AnimationClip>)(object)m_Model.AnimationClips)[((ReadOnlyDictionary<string, AnimationClip>)(object)m_Model.AnimationClips).Keys[1]]);
		m_AnimationController.TranslationInterpolation = (InterpolationMode)0;
		m_AnimationController.OrientationInterpolation = (InterpolationMode)0;
		m_AnimationController.ScaleInterpolation = (InterpolationMode)0;
		if (m_SceneObject == null)
		{
			SceneObject val = new SceneObject(m_Model.Model);
			((SceneEntity)val).UpdateType = (UpdateType)1;
			val.Visibility = (ObjectVisibility)1;
			val.StaticLightingType = (StaticLightingType)1;
			val.CollisionType = (CollisionType)0;
			val.AffectedByGravity = false;
			((SceneEntity)val).Name = $"Player{m_NetId}";
			((SceneEntity)val).World = Matrix.Identity;
			m_SceneObject = val;
			((ISubmit<SceneEntity>)(object)g.m_App.sceneInterface.ObjectManager).Submit((SceneEntity)(object)m_SceneObject);
			foreach (RenderableMesh item in (ReadOnlyCollection<RenderableMesh>)(object)m_SceneObject.RenderableMeshes)
			{
				BoundingBox meshBoundingBox = item.MeshBoundingBox;
				item.MeshBoundingBox = new BoundingBox(new Vector3(meshBoundingBox.Min.Z, meshBoundingBox.Min.Y + 2.159f, meshBoundingBox.Min.X), new Vector3(meshBoundingBox.Max.Z, meshBoundingBox.Max.Y + 2.159f, meshBoundingBox.Max.X));
			}
			((SceneEntity)m_SceneObject).CalculateBounds();
		}
		SetWeapon(m_StartWeaponItemIndex);
		SetState(STATE.InGame);
	}

	public CLASS GetClass()
	{
		return m_Class;
	}

	public void EnableCollisionAndGravity()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		((BroadPhaseEntry)((Entity<ConvexCollidable<CylinderShape>>)(object)m_CharacterController.Body).CollisionInformation).CollisionRules.Personal = (CollisionRule)0;
		((Entity)m_CharacterController.Body).IsAffectedByGravity = true;
		((Entity)m_CharacterController.Body).Position = m_Position;
		((Entity)m_CharacterController.Body).LinearVelocity = Vector3.Zero;
		((Entity)m_CharacterController.Body).AngularVelocity = Vector3.Zero;
	}

	public void DisableCollisionAndGravity()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		((BroadPhaseEntry)((Entity<ConvexCollidable<CylinderShape>>)(object)m_CharacterController.Body).CollisionInformation).CollisionRules.Personal = (CollisionRule)5;
		((Entity)m_CharacterController.Body).IsAffectedByGravity = false;
		((Entity)m_CharacterController.Body).Position = m_Position;
		((Entity)m_CharacterController.Body).LinearVelocity = Vector3.Zero;
		((Entity)m_CharacterController.Body).AngularVelocity = Vector3.Zero;
	}

	public Vector3 GetAimPosition()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (!m_Bot && IsLocalPlayer())
		{
			return g.m_CameraManager.m_Position;
		}
		return m_Position + new Vector3(0f, 1.65f, 0f);
	}

	public Vector3 GetAimVector()
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (!m_Bot)
		{
			if (m_ViewSceneObject != null)
			{
				Matrix val = Matrix.CreateRotationX(g.m_CameraManager.m_Pitch) * ((SceneEntity)m_ViewSceneObject).World;
				return ((Matrix)(ref val)).Forward;
			}
			Matrix val2 = Matrix.CreateRotationX(g.m_CameraManager.m_Pitch) * ((SceneEntity)m_SceneObject).World;
			return ((Matrix)(ref val2)).Forward;
		}
		return m_BotAimVector;
	}

	public string GetName()
	{
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		if (m_Bot)
		{
			if (m_Team == TEAM.Hunter)
			{
				if (m_Class == CLASS.FatherD)
				{
					return BotMaleSlayerNames[m_BotNameIdx];
				}
				return BotFemaleSlayerNames[m_BotNameIdx];
			}
			if (m_Class == CLASS.Edgar)
			{
				return BotMaleVampireNames[m_BotNameIdx];
			}
			return BotFemaleVampireNames[m_BotNameIdx];
		}
		if (g.m_App.m_NetworkSession != null)
		{
			GamerCollectionEnumerator<NetworkGamer> enumerator = g.m_App.m_NetworkSession.AllGamers.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					NetworkGamer current = enumerator.Current;
					if (current.Id == m_NetId)
					{
						return ((Gamer)current).Gamertag;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
		}
		else if (Gamer.SignedInGamers[g.m_App.m_PlayerOnePadId] != null)
		{
			return ((Gamer)Gamer.SignedInGamers[g.m_App.m_PlayerOnePadId]).Gamertag;
		}
		return "Player";
	}

	public void RemoveWeapons()
	{
		for (int i = 0; i < 6; i++)
		{
			m_Weapons[i] = -1;
		}
	}

	public void GiveWeapon(int itemId)
	{
		for (int i = 0; i < 6; i++)
		{
			if (m_Weapons[i] == -1)
			{
				m_Weapons[i] = itemId;
				break;
			}
		}
	}

	public void SetWeapon(int itemIndex)
	{
		if (itemIndex != -1)
		{
			m_WeaponItemIndex = itemIndex;
			UpdateWeaponToBone();
			g.m_ItemManager.m_Item[m_WeaponItemIndex].Show();
			PlayViewIdleAnim(0.1f);
		}
	}

	public void SetWeaponByType(int weaponType)
	{
		for (int i = 0; i < 6; i++)
		{
			if (g.m_ItemManager.m_Item[m_Weapons[i]].m_Type == weaponType)
			{
				SetWeapon(m_Weapons[i]);
				break;
			}
		}
	}

	public void HolsterWeapon()
	{
		if (m_CurrentViewAnim != 12)
		{
			PlayViewAnim(12, loop: false, 0f);
		}
	}

	public void ChangeWeapon()
	{
		m_bWeaponChanged = true;
		PeerChangeWeapon();
	}

	public void PeerChangeWeapon()
	{
		int num = -1;
		for (int i = 0; i < 6; i++)
		{
			if (m_Weapons[i] == m_WeaponItemIndex)
			{
				num = i;
				break;
			}
		}
		if (num == -1)
		{
			return;
		}
		ZoomWeapon(bZoom: false);
		if (m_TorchLight != null)
		{
			((BaseLight)m_TorchLight).Enabled = false;
		}
		g.m_ItemManager.m_Item[m_Weapons[num]].Hide();
		num++;
		int num2 = 0;
		while (m_Weapons[num] == -1 && num2 < 100)
		{
			num++;
			if (num >= 6)
			{
				num = 0;
			}
		}
		SetWeapon(m_Weapons[num]);
	}

	public void GiveWeapons()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		RemoveWeapons();
		int num = -1;
		switch (m_Class)
		{
		case CLASS.FatherD:
			num = g.m_ItemManager.Create(0, -1, new Vector3(0f, 0f, 0f), 0f, this);
			GiveWeapon(num);
			m_StartWeaponItemIndex = num;
			num = g.m_ItemManager.Create(4, -1, new Vector3(0f, 0f, 0f), 0f, this);
			GiveWeapon(num);
			num = g.m_ItemManager.Create(5, -1, new Vector3(0f, 0f, 0f), 0f, this);
			GiveWeapon(num);
			break;
		case CLASS.Molly:
			num = g.m_ItemManager.Create(1, -1, new Vector3(0f, 0f, 0f), 0f, this);
			GiveWeapon(num);
			m_StartWeaponItemIndex = num;
			num = g.m_ItemManager.Create(2, -1, new Vector3(0f, 0f, 0f), 0f, this);
			GiveWeapon(num);
			num = g.m_ItemManager.Create(5, -1, new Vector3(0f, 0f, 0f), 0f, this);
			GiveWeapon(num);
			break;
		case CLASS.Edgar:
		case CLASS.Nina:
			num = g.m_ItemManager.Create(6, -1, new Vector3(0f, 0f, 0f), 0f, this);
			GiveWeapon(num);
			m_StartWeaponItemIndex = num;
			break;
		case CLASS.None:
			break;
		}
	}

	private void ResetAllAmmo()
	{
		int num = -1;
		for (int i = 0; i < 6; i++)
		{
			num = m_Weapons[i];
			if (num != -1)
			{
				g.m_ItemManager.ResetAmmo(num);
			}
		}
	}

	public int FindWeapon(int wepType)
	{
		for (int i = 0; i < 6; i++)
		{
			if (m_Weapons[i] != -1 && g.m_ItemManager.m_Item[m_Weapons[i]].m_Type == wepType)
			{
				return m_Weapons[i];
			}
		}
		return -1;
	}

	public void Crouch()
	{
		m_Crouch = !m_Crouch;
		m_RequestSendCrouch = true;
		m_Anim = -1;
		if (m_CharacterController != null)
		{
			if (m_Crouch)
			{
				m_CharacterController.StanceManager.DesiredStance = Stance.Crouching;
			}
			else
			{
				m_CharacterController.StanceManager.DesiredStance = Stance.Standing;
			}
		}
	}

	public void PeerCrouch(bool crouch)
	{
		m_Crouch = crouch;
	}

	public void SetXPForNextLevel()
	{
		m_XPForNextRank = 1000 * m_Rank;
	}

	public void CheckRankUp()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (m_XP > m_XPForNextRank)
		{
			m_Rank++;
			m_XP = 0;
			SetXPForNextLevel();
			m_RequestRankUp = true;
			Color c = Color.White;
			if (!m_Bot)
			{
				c = Color.LightGoldenrodYellow;
			}
			g.m_App.AddSystemMessage($"{GetName()} RANKED UP!", c);
		}
	}

	public void PeerRankUp()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		g.m_App.AddSystemMessage($"{GetName()} RANKED UP!", Color.White);
	}

	public string GetNameForRank()
	{
		return (m_Rank / 10) switch
		{
			1 => "Apprentice", 
			2 => "Adequate", 
			3 => "Adroit", 
			4 => "Guardian", 
			5 => "Mentor", 
			6 => "Doyen", 
			7 => "Elder", 
			8 => "Virtuoso", 
			9 => "Veteran", 
			_ => "Neophyte", 
		};
	}

	public void TryFeed()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		if (m_FeedTimeOut > (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds)
		{
			return;
		}
		Vector3 aimPosition = GetAimPosition();
		Vector3 aimVector = GetAimVector();
		float num = 6.35f;
		List<RayCastResult> list = new List<RayCastResult>();
		Ray val = default(Ray);
		((Ray)(ref val))._002Ector(aimPosition, aimVector);
		if (!g.m_App.m_Space.RayCast(val, num, (IList<RayCastResult>)list))
		{
			return;
		}
		list.Sort();
		for (int i = 0; i < list.Count; i++)
		{
			BroadPhaseEntry hitObject = list[i].HitObject;
			EntityCollidable val2 = (EntityCollidable)(object)((hitObject is EntityCollidable) ? hitObject : null);
			if (val2 == null)
			{
				break;
			}
			if (!(val2.Entity.Tag is HitTag hitTag) || hitTag.m_PlayerId == m_Id || hitTag.m_HitZone == 255)
			{
				continue;
			}
			Player player = g.m_PlayerManager.m_Player[hitTag.m_PlayerId];
			if (player.m_bRagdoll && player.m_Team == TEAM.Hunter)
			{
				m_Health += 10;
				if (m_Health > 100)
				{
					m_Health = 100;
				}
				m_RequestFeed = true;
				m_RequestFeedPosition = list[i].HitData.Location;
				FeedVFX(m_RequestFeedPosition);
				m_FeedTimeOut = (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds + 1f;
				break;
			}
		}
	}

	private void FeedVFX(Vector3 pos)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		g.m_SoundManager.Play3D(54, pos);
		if (g.m_App.m_OptionsBlood)
		{
			((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)(object)m_BloodSpray).Enabled = true;
			((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)(object)m_BloodSpray).Visible = true;
			((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)(object)m_BloodSpray).Emitter.BurstParticles = 5;
			((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)(object)m_BloodSpray).Emitter.PositionData.Position = pos;
			((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)(object)m_BloodSpray).Emitter.PositionData.Velocity = Vector3.Zero;
			((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)(object)m_BloodSpray).LerpEmittersPositionAndOrientationOnNextUpdate = false;
			m_BloodSpray.Normal = new Vector3(0f, -1f, 0f) * 10f;
		}
	}

	public void PeerFeed(Vector3 pos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		FeedVFX(pos);
	}

	public void TryPickupAmmo()
	{
		if (m_Team != TEAM.Hunter)
		{
			return;
		}
		Player playerNearMe = g.m_PlayerManager.GetPlayerNearMe(m_Id, 25.806398f);
		if (playerNearMe != null && playerNearMe.m_bRagdoll && playerNearMe.m_HasAmmoToGive)
		{
			if (m_Class == CLASS.FatherD && playerNearMe.m_Class == CLASS.FatherD)
			{
				GiveShotgunAmmo();
				playerNearMe.m_HasAmmoToGive = false;
			}
			if (m_Class == CLASS.Molly && playerNearMe.m_Class == CLASS.Molly)
			{
				GiveMac10AndCrossbowAmmo();
				playerNearMe.m_HasAmmoToGive = false;
			}
		}
	}

	private void GiveShotgunAmmo()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		g.m_SoundManager.Play3D(61, m_Position);
		int num = FindWeapon(0);
		if (num != -1)
		{
			g.m_ItemManager.ResetAmmo(num);
		}
	}

	private void GiveMac10AndCrossbowAmmo()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		g.m_SoundManager.Play3D(61, m_Position);
		int num = FindWeapon(1);
		if (num != -1)
		{
			g.m_ItemManager.ResetAmmo(num);
		}
		num = FindWeapon(2);
		if (num != -1)
		{
			g.m_ItemManager.ResetAmmo(num);
		}
	}

	private void CheckPeerCollision()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		if (!m_CharacterController.SupportFinder.HasTraction)
		{
			return;
		}
		Player playerNearMe = g.m_PlayerManager.GetPlayerNearMe(m_Id, 1.6128999f);
		if (playerNearMe != null && !playerNearMe.m_bRagdoll)
		{
			Vector3 val = m_Position - playerNearMe.m_Position;
			if (!(Math.Abs(val.X) < 0.0001f) || !(Math.Abs(val.Z) < 0.0001f))
			{
				val.Y = 0f;
				((Vector3)(ref val)).Normalize();
				((Entity)m_CharacterController.Body).ApplyImpulse(m_Position, val * 25f);
			}
		}
	}

	private bool IsValid()
	{
		if (m_ViewSceneObject == null && m_SceneObject == null)
		{
			return false;
		}
		if (m_Team == TEAM.None)
		{
			return false;
		}
		return true;
	}

	public void UpdateRumble(PlayerIndex playerIndex)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (g.m_App.m_RumbleFrames > 0 && g.m_App.m_OptionsVibration)
		{
			g.m_App.m_RumbleFrames--;
			GamePad.SetVibration(playerIndex, 0.5f, 0.5f);
		}
		else
		{
			GamePad.SetVibration(playerIndex, 0f, 0f);
		}
	}

	public void CreateHitZones()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Expected O, but got Unknown
		m_HitZone_Torso = (Entity)new Sphere(new Vector3(0f, 5f, 0f), 0.9f);
		((BroadPhaseEntry)m_HitZone_Torso.CollisionInformation).CollisionRules.Personal = (CollisionRule)5;
		m_HitZone_Torso.Tag = new HitTag(m_Id, 1);
		g.m_App.m_Space.Add((ISpaceObject)(object)m_HitZone_Torso);
		m_HitZone_Head = (Entity)new Sphere(m_HitZone_Torso.Position + new Vector3(0f, 2f, 0f), 0.6f);
		((BroadPhaseEntry)m_HitZone_Head.CollisionInformation).CollisionRules.Personal = (CollisionRule)5;
		m_HitZone_Head.Tag = new HitTag(m_Id, 2);
		g.m_App.m_Space.Add((ISpaceObject)(object)m_HitZone_Head);
		m_HitZone_LowerBody = (Entity)new Sphere(m_HitZone_Torso.Position + new Vector3(0f, -2f, 0f), 1f);
		((BroadPhaseEntry)m_HitZone_LowerBody.CollisionInformation).CollisionRules.Personal = (CollisionRule)5;
		m_HitZone_LowerBody.Tag = new HitTag(m_Id, 3);
		g.m_App.m_Space.Add((ISpaceObject)(object)m_HitZone_LowerBody);
	}

	public void DestroyHitZones()
	{
		if (m_HitZone_Torso != null)
		{
			g.m_App.m_Space.Remove((ISpaceObject)(object)m_HitZone_Torso);
			m_HitZone_Torso = null;
		}
		if (m_HitZone_Head != null)
		{
			g.m_App.m_Space.Remove((ISpaceObject)(object)m_HitZone_Head);
			m_HitZone_Head = null;
		}
		if (m_HitZone_LowerBody != null)
		{
			g.m_App.m_Space.Remove((ISpaceObject)(object)m_HitZone_LowerBody);
			m_HitZone_LowerBody = null;
		}
	}

	public void UpdateHitZones()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		if (m_HitZone_Torso != null)
		{
			Matrix val = Matrix.CreateRotationY(m_Rotation.Y);
			if (m_SceneObject != null)
			{
				Entity hitZone_Head = m_HitZone_Head;
				Matrix boneAbsoluteTransform = m_AnimationController.GetBoneAbsoluteTransform("Bip01_Head");
				Vector3 val2 = Vector3.Transform(((Matrix)(ref boneAbsoluteTransform)).Translation, val);
				Matrix world = ((SceneEntity)m_SceneObject).World;
				hitZone_Head.Position = val2 + ((Matrix)(ref world)).Translation + new Vector3(0f, 0.2f, 0f);
				Entity hitZone_Torso = m_HitZone_Torso;
				Matrix boneAbsoluteTransform2 = m_AnimationController.GetBoneAbsoluteTransform("Bip01_Spine1");
				Vector3 translation = ((Matrix)(ref boneAbsoluteTransform2)).Translation;
				Matrix world2 = ((SceneEntity)m_SceneObject).World;
				hitZone_Torso.Position = translation + ((Matrix)(ref world2)).Translation + new Vector3(0f, -0.1f, 0f);
				Entity hitZone_LowerBody = m_HitZone_LowerBody;
				Matrix boneAbsoluteTransform3 = m_AnimationController.GetBoneAbsoluteTransform("Bip01");
				Vector3 translation2 = ((Matrix)(ref boneAbsoluteTransform3)).Translation;
				Matrix world3 = ((SceneEntity)m_SceneObject).World;
				hitZone_LowerBody.Position = translation2 + ((Matrix)(ref world3)).Translation + new Vector3(0f, -1f, 0f);
			}
			else
			{
				Entity hitZone_Head2 = m_HitZone_Head;
				Matrix boneAbsoluteTransform4 = m_ViewAnimationController.GetBoneAbsoluteTransform("Bip01_Head");
				Vector3 val3 = Vector3.Transform(((Matrix)(ref boneAbsoluteTransform4)).Translation, val);
				Matrix world4 = ((SceneEntity)m_ViewSceneObject).World;
				hitZone_Head2.Position = val3 + ((Matrix)(ref world4)).Translation + new Vector3(0f, 0.2f, 0f);
				Entity hitZone_Torso2 = m_HitZone_Torso;
				Matrix boneAbsoluteTransform5 = m_ViewAnimationController.GetBoneAbsoluteTransform("Bip01_Spine1");
				Vector3 translation3 = ((Matrix)(ref boneAbsoluteTransform5)).Translation;
				Matrix world5 = ((SceneEntity)m_ViewSceneObject).World;
				hitZone_Torso2.Position = translation3 + ((Matrix)(ref world5)).Translation + new Vector3(0f, -0.1f, 0f);
				Entity hitZone_LowerBody2 = m_HitZone_LowerBody;
				Matrix boneAbsoluteTransform6 = m_ViewAnimationController.GetBoneAbsoluteTransform("Bip01");
				Vector3 translation4 = ((Matrix)(ref boneAbsoluteTransform6)).Translation;
				Matrix world6 = ((SceneEntity)m_ViewSceneObject).World;
				hitZone_LowerBody2.Position = translation4 + ((Matrix)(ref world6)).Translation + new Vector3(0f, -1f, 0f);
			}
		}
	}

	public void EnableRagdoll()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (m_CharacterController != null)
		{
			((BroadPhaseEntry)((Entity<ConvexCollidable<CylinderShape>>)(object)m_CharacterController.Body).CollisionInformation).CollisionRules.Personal = (CollisionRule)5;
		}
		m_RagdollCreatePosition = m_Position;
		_ = m_NetworkPosition.Y;
		_ = -10f;
		m_NetworkPosition = m_Position;
		m_HasAmmoToGive = true;
		CreateRagdoll();
		m_bRagdoll = true;
		if (m_WeaponItemIndex != -1)
		{
			g.m_ItemManager.m_Item[m_WeaponItemIndex].Hide();
		}
		m_Anim = 1;
	}

	public void DisableRagdoll()
	{
		DestroyRagdoll();
		if (m_CharacterController != null)
		{
			((BroadPhaseEntry)((Entity<ConvexCollidable<CylinderShape>>)(object)m_CharacterController.Body).CollisionInformation).CollisionRules.Personal = (CollisionRule)0;
		}
		m_bRagdoll = false;
		m_HasAmmoToGive = false;
		RagdollEnd();
	}

	public void InitRagdollCollisionGroup()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		m_RagdollGroup = new CollisionGroup();
		CollisionGroupPair key = default(CollisionGroupPair);
		if (g.m_PlayerManager.RAGDOLLS_COLLIDE_WITH_PLAYERS)
		{
			CollisionGroup val = new CollisionGroup();
			((BroadPhaseEntry)((Entity<ConvexCollidable<CylinderShape>>)(object)m_CharacterController.Body).CollisionInformation).CollisionRules.Group = val;
			((CollisionGroupPair)(ref key))._002Ector(val, m_RagdollGroup);
		}
		else
		{
			((CollisionGroupPair)(ref key))._002Ector(g.m_PlayerManager.m_CylinderGroup, m_RagdollGroup);
		}
		CollisionRules.CollisionGroupRules.Add(key, (CollisionRule)5);
	}

	public void CreateRagdoll()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Expected O, but got Unknown
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Expected O, but got Unknown
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Expected O, but got Unknown
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Expected O, but got Unknown
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Expected O, but got Unknown
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Expected O, but got Unknown
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_053d: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e3: Expected O, but got Unknown
		//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_060a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0576: Unknown result type (might be due to invalid IL or missing references)
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_059f: Expected O, but got Unknown
		//IL_0644: Unknown result type (might be due to invalid IL or missing references)
		//IL_065a: Unknown result type (might be due to invalid IL or missing references)
		//IL_065f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0660: Unknown result type (might be due to invalid IL or missing references)
		//IL_0665: Unknown result type (might be due to invalid IL or missing references)
		//IL_066a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0674: Expected O, but got Unknown
		//IL_069e: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a8: Expected O, but got Unknown
		//IL_06ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0705: Unknown result type (might be due to invalid IL or missing references)
		//IL_070a: Unknown result type (might be due to invalid IL or missing references)
		//IL_070b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0710: Unknown result type (might be due to invalid IL or missing references)
		//IL_0715: Unknown result type (might be due to invalid IL or missing references)
		//IL_071f: Expected O, but got Unknown
		//IL_0796: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_07af: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0749: Unknown result type (might be due to invalid IL or missing references)
		//IL_0753: Expected O, but got Unknown
		//IL_07f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_080d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0812: Unknown result type (might be due to invalid IL or missing references)
		//IL_0813: Unknown result type (might be due to invalid IL or missing references)
		//IL_0818: Unknown result type (might be due to invalid IL or missing references)
		//IL_0828: Unknown result type (might be due to invalid IL or missing references)
		//IL_0829: Unknown result type (might be due to invalid IL or missing references)
		//IL_0854: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07df: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e9: Expected O, but got Unknown
		//IL_08b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_090e: Unknown result type (might be due to invalid IL or missing references)
		//IL_087c: Unknown result type (might be due to invalid IL or missing references)
		//IL_089b: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a5: Expected O, but got Unknown
		//IL_0948: Unknown result type (might be due to invalid IL or missing references)
		//IL_095e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0963: Unknown result type (might be due to invalid IL or missing references)
		//IL_0964: Unknown result type (might be due to invalid IL or missing references)
		//IL_0969: Unknown result type (might be due to invalid IL or missing references)
		//IL_096e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0978: Expected O, but got Unknown
		//IL_09a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ac: Expected O, but got Unknown
		//IL_09f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a09: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a14: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a19: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a23: Expected O, but got Unknown
		//IL_0a4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a57: Expected O, but got Unknown
		//IL_0ac1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ade: Unknown result type (might be due to invalid IL or missing references)
		//IL_0adf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b20: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab5: Expected O, but got Unknown
		//IL_0b7d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b93: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b99: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0baf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bda: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b48: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b67: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b71: Expected O, but got Unknown
		//IL_0c14: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c44: Expected O, but got Unknown
		//IL_0c6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c78: Expected O, but got Unknown
		//IL_0cbf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cda: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cdb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cef: Expected O, but got Unknown
		//IL_0d19: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d23: Expected O, but got Unknown
		if (m_AnimationController != null)
		{
			m_AnimationController.StopClip();
		}
		Matrix val = Matrix.CreateRotationY(m_Rotation.Y);
		float num = 0.68f;
		float num2 = 0.25f;
		Matrix val2 = Matrix.CreateRotationY(MathHelper.ToRadians(90f) + m_Rotation.Y);
		val2 = Matrix.Transpose(val2);
		if (m_Torso == null)
		{
			m_Torso = (Entity)new Box(Vector3.Zero, 1f * num, 2f * num, 1.5f * num, 10f);
		}
		m_Torso.Position = new Vector3(0f, 5f * num, 0f);
		((BroadPhaseEntry)m_Torso.CollisionInformation).CollisionRules.Group = m_RagdollGroup;
		m_Torso.OrientationMatrix = Matrix3X3.CreateFromMatrix(val2);
		m_Torso.LinearVelocity = Vector3.Zero;
		m_Torso.AngularVelocity = Vector3.Zero;
		m_Torso.CollisionInformation.Events.ContactCreated += Events_ContactCreated;
		Entity torso = m_Torso;
		torso.Position += m_Position + new Vector3(0f, -2.159f, 0f);
		g.m_App.m_Space.Add((ISpaceObject)(object)m_Torso);
		val2 = Matrix.CreateRotationX(3.14f) * Matrix.CreateRotationY(3.14f + m_Rotation.Y) * Matrix.CreateRotationZ(MathHelper.ToRadians(135f));
		val2 = Matrix.Transpose(val2);
		if (m_UpperLeftArm == null)
		{
			m_UpperLeftArm = (Entity)new Box(Vector3.Zero, 1f * num, 0.5f * num, 0.5f * num, 5f);
		}
		m_UpperLeftArm.Position = m_Torso.Position + Vector3.Transform(new Vector3(-1.4f * num, 0f * num, 0f), val);
		((BroadPhaseEntry)m_UpperLeftArm.CollisionInformation).CollisionRules.Group = m_RagdollGroup;
		m_UpperLeftArm.OrientationMatrix = Matrix3X3.CreateFromMatrix(val2);
		m_UpperLeftArm.LinearVelocity = Vector3.Zero;
		g.m_App.m_Space.Add((ISpaceObject)(object)m_UpperLeftArm);
		if (m_LowerLeftArm == null)
		{
			m_LowerLeftArm = (Entity)new Box(Vector3.Zero, 1f * num, 0.5f * num, 0.5f * num, 5f);
		}
		m_LowerLeftArm.Position = m_UpperLeftArm.Position + Vector3.Transform(new Vector3(-0.7f * num, -0.8f, 0f), val);
		((BroadPhaseEntry)m_LowerLeftArm.CollisionInformation).CollisionRules.Group = m_RagdollGroup;
		m_LowerLeftArm.OrientationMatrix = Matrix3X3.CreateFromMatrix(val2);
		m_LowerLeftArm.LinearVelocity = Vector3.Zero;
		g.m_App.m_Space.Add((ISpaceObject)(object)m_LowerLeftArm);
		if (m_BallSocketJointUpperLeftArmToTorso == null)
		{
			m_BallSocketJointUpperLeftArmToTorso = new BallSocketJoint(m_Torso, m_UpperLeftArm, m_UpperLeftArm.Position + Vector3.Transform(new Vector3(0.7f * num, 0f, 0f), val));
		}
		g.m_App.m_Space.Add((ISpaceObject)(object)m_BallSocketJointUpperLeftArmToTorso);
		if (m_AngularMotorUpperLeftArmToTorso == null)
		{
			m_AngularMotorUpperLeftArmToTorso = new AngularMotor(m_Torso, m_UpperLeftArm);
		}
		((MotorSettings)m_AngularMotorUpperLeftArmToTorso.Settings).MaximumForce = 250f * num2;
		g.m_App.m_Space.Add((ISpaceObject)(object)m_AngularMotorUpperLeftArmToTorso);
		if (m_BallSocketJointLowerLeftArmToUpperLeftArm == null)
		{
			m_BallSocketJointLowerLeftArmToUpperLeftArm = new BallSocketJoint(m_UpperLeftArm, m_LowerLeftArm, m_UpperLeftArm.Position + Vector3.Transform(new Vector3(-0.7f * num, 0f, 0f), val));
		}
		g.m_App.m_Space.Add((ISpaceObject)(object)m_BallSocketJointLowerLeftArmToUpperLeftArm);
		if (m_AngularMotorLowerLeftArmToUpperLeftArm == null)
		{
			m_AngularMotorLowerLeftArmToUpperLeftArm = new AngularMotor(m_UpperLeftArm, m_LowerLeftArm);
		}
		((MotorSettings)m_AngularMotorLowerLeftArmToUpperLeftArm.Settings).MaximumForce = 150f * num2 * 0.5f;
		g.m_App.m_Space.Add((ISpaceObject)(object)m_AngularMotorLowerLeftArmToUpperLeftArm);
		val2 = Matrix.CreateRotationX(3.14f) * Matrix.CreateRotationY(3.14f + m_Rotation.Y) * Matrix.CreateRotationZ(MathHelper.ToRadians(45f));
		val2 = Matrix.Transpose(val2);
		if (m_UpperRightArm == null)
		{
			m_UpperRightArm = (Entity)new Box(Vector3.Zero, 1f * num, 0.5f * num, 0.5f * num, 5f);
		}
		m_UpperRightArm.Position = m_Torso.Position + Vector3.Transform(new Vector3(1.4f * num, 0f * num, 0f), val);
		((BroadPhaseEntry)m_UpperRightArm.CollisionInformation).CollisionRules.Group = m_RagdollGroup;
		m_UpperRightArm.OrientationMatrix = Matrix3X3.CreateFromMatrix(val2);
		m_UpperRightArm.LinearVelocity = Vector3.Zero;
		g.m_App.m_Space.Add((ISpaceObject)(object)m_UpperRightArm);
		if (m_LowerRightArm == null)
		{
			m_LowerRightArm = (Entity)new Box(Vector3.Zero, 1f * num, 0.5f * num, 0.5f * num, 5f);
		}
		m_LowerRightArm.Position = m_UpperRightArm.Position + Vector3.Transform(new Vector3(0.7f * num, -0.8f * num, 0f), val);
		((BroadPhaseEntry)m_LowerRightArm.CollisionInformation).CollisionRules.Group = m_RagdollGroup;
		m_LowerRightArm.OrientationMatrix = Matrix3X3.CreateFromMatrix(val2);
		m_LowerRightArm.LinearVelocity = Vector3.Zero;
		g.m_App.m_Space.Add((ISpaceObject)(object)m_LowerRightArm);
		if (m_BallSocketJointUpperRightArmToTorso == null)
		{
			m_BallSocketJointUpperRightArmToTorso = new BallSocketJoint(m_Torso, m_UpperRightArm, m_UpperRightArm.Position + Vector3.Transform(new Vector3(-0.7f * num, 0f, 0f), val));
		}
		g.m_App.m_Space.Add((ISpaceObject)(object)m_BallSocketJointUpperRightArmToTorso);
		if (m_AngularMotorUpperRightArmToTorso == null)
		{
			m_AngularMotorUpperRightArmToTorso = new AngularMotor(m_Torso, m_UpperRightArm);
		}
		((MotorSettings)m_AngularMotorUpperRightArmToTorso.Settings).MaximumForce = 250f * num2;
		g.m_App.m_Space.Add((ISpaceObject)(object)m_AngularMotorUpperRightArmToTorso);
		if (m_BallSocketJointLowerRightArmToUpperRightArm == null)
		{
			m_BallSocketJointLowerRightArmToUpperRightArm = new BallSocketJoint(m_UpperRightArm, m_LowerRightArm, m_UpperRightArm.Position + Vector3.Transform(new Vector3(0.7f * num, 0f, 0f), val));
		}
		g.m_App.m_Space.Add((ISpaceObject)(object)m_BallSocketJointLowerRightArmToUpperRightArm);
		if (m_AngularMotorLowerRightArmToUpperRightArm == null)
		{
			m_AngularMotorLowerRightArmToUpperRightArm = new AngularMotor(m_UpperRightArm, m_LowerRightArm);
		}
		((MotorSettings)m_AngularMotorLowerRightArmToUpperRightArm.Settings).MaximumForce = 150f * num2 * 0.5f;
		g.m_App.m_Space.Add((ISpaceObject)(object)m_AngularMotorLowerRightArmToUpperRightArm);
		val2 = Matrix.CreateRotationY(3.14f - m_Rotation.Y) * Matrix.CreateRotationZ(MathHelper.ToRadians(-90f));
		val2 = Matrix.Transpose(val2);
		if (m_UpperLeftLeg == null)
		{
			m_UpperLeftLeg = (Entity)new Box(Vector3.Zero, 1.3f * num, 0.5f * num, 0.5f * num, 8f);
		}
		m_UpperLeftLeg.Position = m_Torso.Position + Vector3.Transform(new Vector3(-0.6f * num, -2.1f * num, 0f), val);
		m_UpperLeftLeg.OrientationMatrix = Matrix3X3.CreateFromMatrix(val2);
		((BroadPhaseEntry)m_UpperLeftLeg.CollisionInformation).CollisionRules.Group = m_RagdollGroup;
		m_UpperLeftLeg.LinearVelocity = Vector3.Zero;
		g.m_App.m_Space.Add((ISpaceObject)(object)m_UpperLeftLeg);
		if (m_LowerLeftLeg == null)
		{
			m_LowerLeftLeg = (Entity)new Box(Vector3.Zero, 1.3f * num, 0.5f * num, 0.5f * num, 8f);
		}
		m_LowerLeftLeg.Position = m_UpperLeftLeg.Position + Vector3.Transform(new Vector3(0f, -1.7f * num, 0f), val);
		m_LowerLeftLeg.OrientationMatrix = Matrix3X3.CreateFromMatrix(val2);
		((BroadPhaseEntry)m_LowerLeftLeg.CollisionInformation).CollisionRules.Group = m_RagdollGroup;
		m_LowerLeftLeg.LinearVelocity = Vector3.Zero;
		g.m_App.m_Space.Add((ISpaceObject)(object)m_LowerLeftLeg);
		if (m_BallSocketJointUpperLeftLegToTorso == null)
		{
			m_BallSocketJointUpperLeftLegToTorso = new BallSocketJoint(m_Torso, m_UpperLeftLeg, m_UpperLeftLeg.Position + Vector3.Transform(new Vector3(0f, 0.9f * num, 0f), val));
		}
		g.m_App.m_Space.Add((ISpaceObject)(object)m_BallSocketJointUpperLeftLegToTorso);
		if (m_AngularMotorUpperLeftLegToTorso == null)
		{
			m_AngularMotorUpperLeftLegToTorso = new AngularMotor(m_Torso, m_UpperLeftLeg);
		}
		((MotorSettings)m_AngularMotorUpperLeftLegToTorso.Settings).MaximumForce = 350f * num2;
		g.m_App.m_Space.Add((ISpaceObject)(object)m_AngularMotorUpperLeftLegToTorso);
		if (m_BallSocketJointLowerLeftLegToUpperLeftLeg == null)
		{
			m_BallSocketJointLowerLeftLegToUpperLeftLeg = new BallSocketJoint(m_UpperLeftLeg, m_LowerLeftLeg, m_UpperLeftLeg.Position + Vector3.Transform(new Vector3(0f, -0.9f * num, 0f), val));
		}
		g.m_App.m_Space.Add((ISpaceObject)(object)m_BallSocketJointLowerLeftLegToUpperLeftLeg);
		if (m_AngularMotorLowerLeftLegToUpperLeftLeg == null)
		{
			m_AngularMotorLowerLeftLegToUpperLeftLeg = new AngularMotor(m_UpperLeftLeg, m_LowerLeftLeg);
		}
		((MotorSettings)m_AngularMotorLowerLeftLegToUpperLeftLeg.Settings).MaximumForce = 250f * num2;
		g.m_App.m_Space.Add((ISpaceObject)(object)m_AngularMotorLowerLeftLegToUpperLeftLeg);
		if (m_UpperRightLeg == null)
		{
			m_UpperRightLeg = (Entity)new Box(Vector3.Zero, 1.3f * num, 0.5f * num, 0.5f * num, 8f);
		}
		m_UpperRightLeg.Position = m_Torso.Position + Vector3.Transform(new Vector3(0.6f * num, -2.1f * num, 0f), val);
		m_UpperRightLeg.OrientationMatrix = Matrix3X3.CreateFromMatrix(val2);
		((BroadPhaseEntry)m_UpperRightLeg.CollisionInformation).CollisionRules.Group = m_RagdollGroup;
		m_UpperRightLeg.LinearVelocity = Vector3.Zero;
		g.m_App.m_Space.Add((ISpaceObject)(object)m_UpperRightLeg);
		if (m_LowerRightLeg == null)
		{
			m_LowerRightLeg = (Entity)new Box(Vector3.Zero, 1.3f * num, 0.5f * num, 0.5f * num, 8f);
		}
		m_LowerRightLeg.Position = m_UpperRightLeg.Position + Vector3.Transform(new Vector3(0f, -1.7f * num, 0f), val);
		m_LowerRightLeg.OrientationMatrix = Matrix3X3.CreateFromMatrix(val2);
		((BroadPhaseEntry)m_LowerRightLeg.CollisionInformation).CollisionRules.Group = m_RagdollGroup;
		m_LowerRightLeg.LinearVelocity = Vector3.Zero;
		g.m_App.m_Space.Add((ISpaceObject)(object)m_LowerRightLeg);
		if (m_BallSocketJointUpperRightLegToTorso == null)
		{
			m_BallSocketJointUpperRightLegToTorso = new BallSocketJoint(m_Torso, m_UpperRightLeg, m_UpperRightLeg.Position + Vector3.Transform(new Vector3(0f, 0.9f * num, 0f), val));
		}
		g.m_App.m_Space.Add((ISpaceObject)(object)m_BallSocketJointUpperRightLegToTorso);
		if (m_AngularMotorUpperRightLegToTorso == null)
		{
			m_AngularMotorUpperRightLegToTorso = new AngularMotor(m_Torso, m_UpperRightLeg);
		}
		((MotorSettings)m_AngularMotorUpperRightLegToTorso.Settings).MaximumForce = 350f * num2;
		g.m_App.m_Space.Add((ISpaceObject)(object)m_AngularMotorUpperRightLegToTorso);
		if (m_BallSocketJointLowerRightLegToUpperRightLeg == null)
		{
			m_BallSocketJointLowerRightLegToUpperRightLeg = new BallSocketJoint(m_UpperRightLeg, m_LowerRightLeg, m_UpperRightLeg.Position + Vector3.Transform(new Vector3(0f, -0.9f * num, 0f), val));
		}
		g.m_App.m_Space.Add((ISpaceObject)(object)m_BallSocketJointLowerRightLegToUpperRightLeg);
		if (m_AngularMotorLowerRightLegToUpperRightLeg == null)
		{
			m_AngularMotorLowerRightLegToUpperRightLeg = new AngularMotor(m_UpperRightLeg, m_LowerRightLeg);
		}
		((MotorSettings)m_AngularMotorLowerRightLegToUpperRightLeg.Settings).MaximumForce = 250f * num2;
		g.m_App.m_Space.Add((ISpaceObject)(object)m_AngularMotorLowerRightLegToUpperRightLeg);
		UpdateRagdoll();
	}

	private void Events_ContactCreated(EntityCollidable sender, Collidable other, CollidablePairHandler pair, ContactData contact)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (m_BodyThumpTime < (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds && !(other.Shape is BoxShape))
		{
			int num = g.m_App.m_Rand.Next(0, 5);
			g.m_SoundManager.Play3D(56 + num, m_Position);
			m_BodyThumpTime = (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds + 0.5f;
		}
	}

	public void DestroyRagdoll()
	{
		g.m_App.m_Space.Remove((ISpaceObject)(object)m_Torso);
		g.m_App.m_Space.Remove((ISpaceObject)(object)m_UpperLeftArm);
		g.m_App.m_Space.Remove((ISpaceObject)(object)m_LowerLeftArm);
		g.m_App.m_Space.Remove((ISpaceObject)(object)m_UpperRightArm);
		g.m_App.m_Space.Remove((ISpaceObject)(object)m_LowerRightArm);
		g.m_App.m_Space.Remove((ISpaceObject)(object)m_UpperLeftLeg);
		g.m_App.m_Space.Remove((ISpaceObject)(object)m_LowerLeftLeg);
		g.m_App.m_Space.Remove((ISpaceObject)(object)m_UpperRightLeg);
		g.m_App.m_Space.Remove((ISpaceObject)(object)m_LowerRightLeg);
		g.m_App.m_Space.Remove((ISpaceObject)(object)m_AngularMotorUpperLeftArmToTorso);
		g.m_App.m_Space.Remove((ISpaceObject)(object)m_AngularMotorLowerLeftArmToUpperLeftArm);
		g.m_App.m_Space.Remove((ISpaceObject)(object)m_AngularMotorUpperRightArmToTorso);
		g.m_App.m_Space.Remove((ISpaceObject)(object)m_AngularMotorLowerRightArmToUpperRightArm);
		g.m_App.m_Space.Remove((ISpaceObject)(object)m_AngularMotorUpperLeftLegToTorso);
		g.m_App.m_Space.Remove((ISpaceObject)(object)m_AngularMotorLowerLeftLegToUpperLeftLeg);
		g.m_App.m_Space.Remove((ISpaceObject)(object)m_AngularMotorUpperRightLegToTorso);
		g.m_App.m_Space.Remove((ISpaceObject)(object)m_AngularMotorLowerRightLegToUpperRightLeg);
		g.m_App.m_Space.Remove((ISpaceObject)(object)m_BallSocketJointUpperLeftArmToTorso);
		g.m_App.m_Space.Remove((ISpaceObject)(object)m_BallSocketJointLowerLeftArmToUpperLeftArm);
		g.m_App.m_Space.Remove((ISpaceObject)(object)m_BallSocketJointUpperRightArmToTorso);
		g.m_App.m_Space.Remove((ISpaceObject)(object)m_BallSocketJointLowerRightArmToUpperRightArm);
		g.m_App.m_Space.Remove((ISpaceObject)(object)m_BallSocketJointUpperLeftLegToTorso);
		g.m_App.m_Space.Remove((ISpaceObject)(object)m_BallSocketJointLowerLeftLegToUpperLeftLeg);
		g.m_App.m_Space.Remove((ISpaceObject)(object)m_BallSocketJointUpperRightLegToTorso);
		g.m_App.m_Space.Remove((ISpaceObject)(object)m_BallSocketJointLowerRightLegToUpperRightLeg);
	}

	public void UpdateRagdoll()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		Matrix worldTransform = m_Torso.WorldTransform;
		if (((Matrix)(ref worldTransform)).Translation.Y < -20f)
		{
			((Matrix)(ref worldTransform)).Translation = new Vector3(((Matrix)(ref worldTransform)).Translation.X, 2f, ((Matrix)(ref worldTransform)).Translation.Z);
			m_Torso.LinearVelocity = Vector3.Zero;
			m_UpperLeftArm.LinearVelocity = Vector3.Zero;
			m_UpperRightArm.LinearVelocity = Vector3.Zero;
			m_LowerLeftArm.LinearVelocity = Vector3.Zero;
			m_LowerRightArm.LinearVelocity = Vector3.Zero;
			m_UpperLeftLeg.LinearVelocity = Vector3.Zero;
			m_UpperRightLeg.LinearVelocity = Vector3.Zero;
			m_LowerLeftLeg.LinearVelocity = Vector3.Zero;
			m_LowerRightLeg.LinearVelocity = Vector3.Zero;
		}
		if (((Matrix)(ref worldTransform)).Translation.Y > 100f)
		{
			((Matrix)(ref worldTransform)).Translation = new Vector3(((Matrix)(ref worldTransform)).Translation.X, 2f, ((Matrix)(ref worldTransform)).Translation.Z);
			m_Torso.LinearVelocity = Vector3.Zero;
			m_UpperLeftArm.LinearVelocity = Vector3.Zero;
			m_UpperRightArm.LinearVelocity = Vector3.Zero;
			m_LowerLeftArm.LinearVelocity = Vector3.Zero;
			m_LowerRightArm.LinearVelocity = Vector3.Zero;
			m_UpperLeftLeg.LinearVelocity = Vector3.Zero;
			m_UpperRightLeg.LinearVelocity = Vector3.Zero;
			m_LowerLeftLeg.LinearVelocity = Vector3.Zero;
			m_LowerRightLeg.LinearVelocity = Vector3.Zero;
		}
		if (m_Team == TEAM.Hunter && m_Class == CLASS.FatherD)
		{
			Vector3 translation = ((Matrix)(ref worldTransform)).Translation;
			((Matrix)(ref worldTransform)).Translation = Vector3.Zero;
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(-0.1f, 1.159f, 0f);
			m_Position = translation + Vector3.Transform(val, worldTransform);
			m_NetworkPosition = m_Position;
		}
		else
		{
			((Matrix)(ref worldTransform)).Translation = ((Matrix)(ref worldTransform)).Translation + new Vector3(0f, 2f, 0f);
			Vector3 val2 = default(Vector3);
			((Vector3)(ref val2))._002Ector(-0.1f, -0.85f, 0f);
			m_Position = Vector3.Transform(val2, worldTransform);
			m_NetworkPosition = m_Position;
			((Matrix)(ref worldTransform)).Translation = ((Matrix)(ref worldTransform)).Translation - new Vector3(0f, 2f, 0f);
		}
		Matrix val3 = Matrix.CreateRotationY(0f - m_Rotation.Y);
		Matrix val4 = m_Torso.WorldTransform * val3;
		m_AnimationController.SetBoneController("Bip01", ref val4);
		val4 = m_UpperRightArm.WorldTransform * val3;
		m_AnimationController.SetBoneController("Bip01_R_UpperArm", ref val4);
		val4 = m_LowerRightArm.WorldTransform * val3;
		m_AnimationController.SetBoneController("Bip01_R_Forearm", ref val4);
		val4 = m_UpperLeftArm.WorldTransform * val3;
		m_AnimationController.SetBoneController("Bip01_L_UpperArm", ref val4);
		val4 = m_LowerLeftArm.WorldTransform * val3;
		m_AnimationController.SetBoneController("Bip01_L_Forearm", ref val4);
		val4 = m_UpperRightLeg.WorldTransform * val3;
		m_AnimationController.SetBoneController("Bip01_R_Thigh", ref val4);
		val4 = m_LowerRightLeg.WorldTransform * val3;
		m_AnimationController.SetBoneController("Bip01_R_Calf", ref val4);
		val4 = m_UpperLeftLeg.WorldTransform * val3;
		m_AnimationController.SetBoneController("Bip01_L_Thigh", ref val4);
		val4 = m_LowerLeftLeg.WorldTransform * val3;
		m_AnimationController.SetBoneController("Bip01_L_Calf", ref val4);
	}

	public void InitBot()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		m_Anim = -1;
		m_BotAction = BOTACTION.Idle;
		m_TargetNode = -1;
		m_LastSafeNode = -1;
		m_BotVecTarget = Vector3.Zero;
		m_BotVecMove = Vector3.Zero;
		m_BotTargetRotY = 0f;
		m_BotSpeed = 0f;
		m_NextActionTime = 0f;
		m_EnemyId = -1;
		m_LookForEnemyTime = 0f;
		m_BeserkTime = 0f;
		m_BotNameIdx = (byte)((m_NetId - 1) & 7);
		if (m_BotNameIdx < 0)
		{
			m_BotNameIdx = 0;
		}
		if (m_BotNameIdx > 6)
		{
			m_BotNameIdx = 6;
		}
		m_BotAllowFire = true;
		m_BotAllowMove = true;
		if (g.m_App.m_Rand.Next(2) == 1)
		{
			m_TargetDirectionForward = true;
		}
		else
		{
			m_TargetDirectionForward = false;
		}
		if (IsHost())
		{
			Spawn();
		}
		g.m_App.AddSystemMessage($"{GetName()} JOINED (BOT {m_NetId})", Color.White);
	}

	public void BotSetClassAndTeam()
	{
		int num = g.m_PlayerManager.NumSlayers();
		int num2 = g.m_PlayerManager.NumVampires();
		if (num < num2)
		{
			PeerSetTeam(TEAM.Hunter);
			if (g.m_App.m_Rand.NextDouble() > 0.5)
			{
				PeerSetClass(CLASS.Molly);
			}
			else
			{
				PeerSetClass(CLASS.FatherD);
			}
		}
		else if (num > num2)
		{
			PeerSetTeam(TEAM.Vampire);
			if (g.m_App.m_Rand.NextDouble() > 0.5)
			{
				PeerSetClass(CLASS.Nina);
			}
			else
			{
				PeerSetClass(CLASS.Edgar);
			}
		}
		else if ((m_NetId & 1) == 0)
		{
			PeerSetTeam(TEAM.Hunter);
			if (g.m_App.m_Rand.NextDouble() > 0.5)
			{
				PeerSetClass(CLASS.Molly);
			}
			else
			{
				PeerSetClass(CLASS.FatherD);
			}
		}
		else
		{
			PeerSetTeam(TEAM.Vampire);
			if (g.m_App.m_Rand.NextDouble() > 0.5)
			{
				PeerSetClass(CLASS.Nina);
			}
			else
			{
				PeerSetClass(CLASS.Edgar);
			}
		}
	}

	private void UpdateBot()
	{
		//IL_06fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aeb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0afe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b03: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b05: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b08: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b12: Unknown result type (might be due to invalid IL or missing references)
		//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
		//IL_0613: Unknown result type (might be due to invalid IL or missing references)
		//IL_0618: Unknown result type (might be due to invalid IL or missing references)
		//IL_061a: Unknown result type (might be due to invalid IL or missing references)
		//IL_061b: Unknown result type (might be due to invalid IL or missing references)
		//IL_061d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_0628: Unknown result type (might be due to invalid IL or missing references)
		//IL_062d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b41: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b46: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b77: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0baa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0baf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0896: Unknown result type (might be due to invalid IL or missing references)
		//IL_089b: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08db: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0907: Unknown result type (might be due to invalid IL or missing references)
		//IL_090c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0911: Unknown result type (might be due to invalid IL or missing references)
		//IL_0914: Unknown result type (might be due to invalid IL or missing references)
		//IL_0919: Unknown result type (might be due to invalid IL or missing references)
		//IL_091e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0969: Unknown result type (might be due to invalid IL or missing references)
		//IL_096e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0972: Unknown result type (might be due to invalid IL or missing references)
		//IL_0977: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds;
		switch (m_BotAction)
		{
		case BOTACTION.Idle:
			if (m_bRagdoll)
			{
				m_BotAction = BOTACTION.Dying;
			}
			else
			{
				if (m_NextActionTime > num)
				{
					break;
				}
				if (m_TargetNode == -1)
				{
					m_TargetNode = g.m_BotPathManager.FindNearestNode(m_Position);
				}
				if (m_TargetNode != -1 && m_BotAllowMove)
				{
					m_BotAction = BOTACTION.Searching;
					if (m_Team == TEAM.Hunter)
					{
						ToggleTorchLight();
						m_bTorchChanged = true;
					}
				}
				m_BotSpeed = 0f;
				m_NextActionTime = num + 9f + (float)g.m_App.m_Rand.Next(0, 3);
			}
			break;
		case BOTACTION.Searching:
			if (m_TargetNode == -1)
			{
				m_BotAction = BOTACTION.Idle;
				break;
			}
			m_Leap = false;
			if (m_bRagdoll)
			{
				m_BotAction = BOTACTION.Dying;
				break;
			}
			if (NearTargetNode())
			{
				if (m_TargetDirectionForward)
				{
					m_LastSafeNode = m_TargetNode;
					m_TargetNode++;
					if (m_TargetNode >= g.m_BotPathManager.m_NumNodes)
					{
						m_TargetNode = 0;
					}
				}
				else
				{
					m_TargetNode--;
					if (m_TargetNode < 0)
					{
						m_TargetNode = g.m_BotPathManager.m_NumNodes - 1;
					}
				}
			}
			m_BotVecTarget = g.m_BotPathManager.m_BotNode[m_TargetNode].m_Position - m_Position;
			m_BotVecTarget.Y = 0f;
			((Vector3)(ref m_BotVecTarget)).Normalize();
			m_BotVecMove = m_BotVecTarget * 0.1f;
			m_BotTargetRotY = (float)Math.Atan2(0f - m_BotVecTarget.X, 0f - m_BotVecTarget.Z);
			m_BotSpeed = 12f;
			if (m_LookForEnemyTime < num)
			{
				m_EnemyId = LookForEnemy();
				m_LookForEnemyTime = num + 0.2f + (float)g.m_App.m_Rand.NextDouble() * 0.1f;
			}
			if (m_EnemyId != -1 && m_BotAllowFire && m_AttackTimeout < num)
			{
				if (!CanSeeTargetEnemy())
				{
					m_AttackTimeout = num + 0.4f + (float)g.m_App.m_Rand.NextDouble() * 0.2f;
					break;
				}
				if (m_Team == TEAM.Vampire)
				{
					m_BotAction = BOTACTION.VampireAttacking;
					m_AttackTimeout = num + 9f + (float)g.m_App.m_Rand.Next(0, 3);
				}
				else
				{
					m_BotAction = BOTACTION.Attacking;
					m_AttackTimeout = num + 2.5f + (float)g.m_App.m_Rand.NextDouble();
					m_BotSpeed = 0f;
				}
				m_NextActionTime = num + 0.1f;
			}
			else if (m_NextActionTime < num)
			{
				if (g.m_App.m_Rand.NextDouble() > 0.5)
				{
					m_TargetDirectionForward = !m_TargetDirectionForward;
				}
				m_BotAction = BOTACTION.Idle;
				m_BotSpeed = 0f;
				m_NextActionTime = num + 2.5f + (float)g.m_App.m_Rand.NextDouble();
			}
			break;
		case BOTACTION.Dying:
			if (!m_bRagdoll)
			{
				m_BotAction = BOTACTION.Idle;
				break;
			}
			UpdateRagdoll();
			m_BotSpeed = 0f;
			m_Rotation.Y = 0f;
			((BaseLight)m_TorchLight).Enabled = false;
			m_AttackTimeout = 0f;
			m_EnemyId = -1;
			m_LookForEnemyTime = 0f;
			m_BotLeapTime = 0f;
			m_BeserkTime = 0f;
			m_Leap = false;
			break;
		case BOTACTION.Attacking:
		{
			if (m_bRagdoll)
			{
				m_BotAction = BOTACTION.Dying;
				break;
			}
			if (g.m_PlayerManager.m_Player[m_EnemyId].m_Id == -1 || m_EnemyId == -1)
			{
				m_BotAction = BOTACTION.Idle;
				m_BotSpeed = 0f;
				break;
			}
			if (g.m_PlayerManager.m_Player[m_EnemyId].m_bRagdoll || g.m_PlayerManager.m_Player[m_EnemyId].m_Health <= 0 || !g.m_PlayerManager.m_Player[m_EnemyId].IsValid() || m_AttackTimeout < num)
			{
				int num3 = LookForEnemyToStake();
				if (num3 != -1 && m_BotAllowFire)
				{
					m_EnemyId = num3;
					if (CanSeeTargetEnemy())
					{
						m_BotAction = BOTACTION.Staking;
						m_AttackTimeout = num + 10f + (float)g.m_App.m_Rand.NextDouble();
						m_BotSpeed = 0f;
						m_NextActionTime = num + 0.1f;
						break;
					}
				}
				m_BotAction = BOTACTION.Idle;
				m_BotSpeed = 0f;
				m_AttackTimeout = num + 2.5f + (float)g.m_App.m_Rand.NextDouble();
				if (g.m_App.m_Rand.NextDouble() > 0.5)
				{
					m_TargetDirectionForward = !m_TargetDirectionForward;
				}
				break;
			}
			if (m_WeaponItemIndex != -1 && g.m_ItemManager.m_Item[m_WeaponItemIndex].m_Type != 0 && g.m_ItemManager.m_Item[m_WeaponItemIndex].m_Type != 1)
			{
				ChangeWeapon();
			}
			float num4 = 0f;
			Vector3 zero2 = Vector3.Zero;
			zero2 = g.m_PlayerManager.m_Player[m_EnemyId].m_Position;
			Vector3 val3 = new Vector3((float)g.m_App.m_Rand.NextDouble() - 0.5f, (float)g.m_App.m_Rand.NextDouble() - 0.5f, (float)g.m_App.m_Rand.NextDouble() - 0.5f) * 10f;
			Vector3 val4 = zero2 + val3 - m_Position;
			m_BotTargetRotY = (float)Math.Atan2(0f - val4.X, 0f - val4.Z);
			if (m_NextActionTime < num)
			{
				m_NextActionTime = num + 0.2f;
				if (m_WeaponItemIndex != -1 && g.m_ItemManager.m_Item[m_WeaponItemIndex].m_WeaponAmmoInClip == 0)
				{
					g.m_ItemManager.m_Item[m_WeaponItemIndex].Reload();
					break;
				}
				Matrix val5 = Matrix.CreateRotationY(m_Rotation.Y);
				m_BotAimVector = ((Matrix)(ref val5)).Forward;
				m_bFired = true;
				FireWeapon();
			}
			Vector3 val6 = zero2 - m_Position;
			num4 = ((Vector3)(ref val6)).LengthSquared();
			if (num4 > 16129f)
			{
				m_BotAction = BOTACTION.Searching;
			}
			break;
		}
		case BOTACTION.VampireAttacking:
		{
			int num5 = g.m_BotPathManager.FindNearestNodeInRange(m_Position, 25f);
			if (num5 != -1 && m_BotAllowMove)
			{
				m_LastSafeNode = num5;
			}
			if (m_bRagdoll)
			{
				m_BotAction = BOTACTION.Dying;
				if (m_LastSafeNode != -1)
				{
					m_TargetNode = m_LastSafeNode;
				}
				break;
			}
			if (m_BotLeapTime < num)
			{
				m_Leap = true;
				m_BotLeapTime = num + 0.3f;
			}
			else
			{
				m_Leap = false;
			}
			if (g.m_PlayerManager.m_Player[m_EnemyId].m_Id == -1 || m_EnemyId == -1)
			{
				m_BotAction = BOTACTION.Idle;
				m_BotSpeed = 0f;
				if (m_LastSafeNode != -1)
				{
					m_TargetNode = m_LastSafeNode;
				}
				break;
			}
			if (g.m_PlayerManager.m_Player[m_EnemyId].m_bRagdoll || g.m_PlayerManager.m_Player[m_EnemyId].m_Health <= 0 || !g.m_PlayerManager.m_Player[m_EnemyId].IsValid() || m_AttackTimeout < num)
			{
				m_BotAction = BOTACTION.Idle;
				m_BotSpeed = 0f;
				m_AttackTimeout = num + 2.5f + (float)g.m_App.m_Rand.NextDouble();
				if (g.m_App.m_Rand.NextDouble() > 0.5)
				{
					m_TargetDirectionForward = !m_TargetDirectionForward;
				}
				if (m_LastSafeNode != -1)
				{
					m_TargetNode = m_LastSafeNode;
				}
				break;
			}
			float num6 = 0f;
			Vector3 zero3 = Vector3.Zero;
			zero3 = g.m_PlayerManager.m_Player[m_EnemyId].m_Position;
			Vector3 val7 = zero3 - m_Position;
			num6 = ((Vector3)(ref val7)).LengthSquared();
			m_BotVecTarget = zero3 - m_Position;
			m_BotVecTarget.Y = 0f;
			((Vector3)(ref m_BotVecTarget)).Normalize();
			m_BotVecMove = m_BotVecTarget * 0.1f;
			Vector3 val8 = zero3 - m_Position;
			m_BotTargetRotY = (float)Math.Atan2(0f - val8.X, 0f - val8.Z);
			if (m_NextActionTime < num && num6 < 16f)
			{
				m_NextActionTime = num + 0.2f;
				Matrix val9 = Matrix.CreateRotationY(m_Rotation.Y);
				m_BotAimVector = ((Matrix)(ref val9)).Forward;
				m_bFired = true;
				FireWeapon();
			}
			if (num6 > 16129f)
			{
				m_BotAction = BOTACTION.Searching;
				if (m_LastSafeNode != -1)
				{
					m_TargetNode = m_LastSafeNode;
				}
			}
			break;
		}
		case BOTACTION.Staking:
		{
			if (m_bRagdoll)
			{
				m_BotAction = BOTACTION.Dying;
				break;
			}
			if (g.m_PlayerManager.m_Player[m_EnemyId].m_Id == -1 || m_EnemyId == -1)
			{
				m_BotAction = BOTACTION.Idle;
				m_BotSpeed = 0f;
				break;
			}
			if (g.m_PlayerManager.m_Player[m_EnemyId].m_bStaked)
			{
				m_BotAction = BOTACTION.Idle;
				m_BotSpeed = 0f;
				break;
			}
			if (m_AttackTimeout < num || !g.m_PlayerManager.m_Player[m_EnemyId].IsValid())
			{
				m_BotAction = BOTACTION.Idle;
				m_BotSpeed = 0f;
				m_AttackTimeout = num + 2.5f + (float)g.m_App.m_Rand.NextDouble();
				if (g.m_App.m_Rand.NextDouble() > 0.5)
				{
					m_TargetDirectionForward = !m_TargetDirectionForward;
				}
				break;
			}
			if (m_WeaponItemIndex != -1 && g.m_ItemManager.m_Item[m_WeaponItemIndex].m_Type != 5)
			{
				ChangeWeapon();
			}
			float num2 = 0f;
			Vector3 zero = Vector3.Zero;
			zero = g.m_PlayerManager.m_Player[m_EnemyId].m_Position;
			Vector3 val = zero - m_Position;
			num2 = (m_BotSpeed = ((Vector3)(ref val)).LengthSquared());
			if (m_BotSpeed > 8f)
			{
				m_BotSpeed = 8f;
			}
			m_BotVecTarget = zero - m_Position;
			m_BotVecTarget.Y = 0f;
			((Vector3)(ref m_BotVecTarget)).Normalize();
			m_BotVecMove = m_BotVecTarget * 0.1f;
			m_BotTargetRotY = (float)Math.Atan2(0f - m_BotVecTarget.X, 0f - m_BotVecTarget.Z);
			Vector3 val2 = zero - m_Position;
			m_BotTargetRotY = (float)Math.Atan2(0f - val2.X, 0f - val2.Z);
			if (m_NextActionTime < num && num2 < 16f)
			{
				m_NextActionTime = num + 0.2f;
				SetAimAtRagdoll(m_EnemyId);
				m_bFired = true;
				FireWeapon();
			}
			if (num2 > 16129f)
			{
				m_BotAction = BOTACTION.Searching;
			}
			break;
		}
		}
		_ = m_BotAction;
		_ = m_PrevBotAction;
		m_PrevBotAction = m_BotAction;
	}

	private bool NearTargetNode()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (m_TargetNode == -1)
		{
			return false;
		}
		Vector3 val = m_Position - g.m_BotPathManager.m_BotNode[m_TargetNode].m_Position;
		val.Y = 0f;
		float num = ((Vector3)(ref val)).LengthSquared();
		if ((double)num < 4.0)
		{
			return true;
		}
		return false;
	}

	private int LookForEnemy()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 botVecTarget = m_BotVecTarget;
		Vector3 val = Vector3.Zero;
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		int num4 = -1;
		float num5 = 9999999f;
		if (m_BeserkTime > (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds)
		{
			num3 = 900f;
		}
		for (int i = 0; i < 10; i++)
		{
			bool flag = true;
			if (g.m_PlayerManager.m_Player[i].m_SceneObject == null)
			{
				if (g.m_PlayerManager.m_Player[i].m_Health <= 0)
				{
					flag = false;
				}
			}
			else if ((int)g.m_PlayerManager.m_Player[i].m_SceneObject.Visibility == 0 || g.m_PlayerManager.m_Player[i].m_bRagdoll)
			{
				flag = false;
			}
			if (g.m_PlayerManager.m_Player[i].m_Id != -1 && g.m_PlayerManager.m_Player[i].m_Id != m_Id && g.m_PlayerManager.m_Player[i].m_Team != m_Team && flag)
			{
				val = g.m_PlayerManager.m_Player[i].m_Position - m_Position;
				num2 = ((Vector3)(ref val)).LengthSquared();
				((Vector3)(ref val)).Normalize();
				num = Vector3.Dot(val, botVecTarget);
				if (num > 0.8f && num2 < 16129f && num2 < num5)
				{
					num5 = num2;
					num4 = i;
				}
				if (num2 < 412.90237f + num3 && num2 < num5)
				{
					num5 = num2;
					num4 = i;
				}
			}
		}
		if (num4 == -1)
		{
			return -1;
		}
		return g.m_PlayerManager.m_Player[num4].m_Id;
	}

	private int LookForEnemyToStake()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		Vector3 botVecTarget = m_BotVecTarget;
		Vector3 val = Vector3.Zero;
		float num = 0f;
		float num2 = 0f;
		if (m_Team == TEAM.Vampire)
		{
			return -1;
		}
		for (int i = 0; i < 10; i++)
		{
			if (g.m_PlayerManager.m_Player[i].m_Id != -1 && g.m_PlayerManager.m_Player[i].m_Id != m_Id && (g.m_PlayerManager.m_Player[i].m_bRagdoll || g.m_PlayerManager.m_Player[i].m_Health <= 0) && !g.m_PlayerManager.m_Player[i].m_bStaked && g.m_PlayerManager.m_Player[i].m_Team == TEAM.Vampire)
			{
				val = g.m_PlayerManager.m_Player[i].m_Position - m_Position;
				num2 = ((Vector3)(ref val)).LengthSquared();
				((Vector3)(ref val)).Normalize();
				num = Vector3.Dot(val, botVecTarget);
				if (num > 0.8f && num2 < 16129f)
				{
					return g.m_PlayerManager.m_Player[i].m_Id;
				}
			}
		}
		return -1;
	}

	private bool CanSeeTargetEnemy()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if (g.m_BotPathManager.m_bDoneLineOfSightRaycast)
		{
			return false;
		}
		Vector3 aimPosition = GetAimPosition();
		Vector3 zero = Vector3.Zero;
		zero = ((!g.m_PlayerManager.m_Player[m_EnemyId].m_bRagdoll) ? g.m_PlayerManager.m_Player[m_EnemyId].GetAimPosition() : g.m_PlayerManager.m_Player[m_EnemyId].m_Position);
		Vector3 val = zero - aimPosition;
		val = Vector3.Normalize(val);
		return CanSeeTargetEnemyRaycast(aimPosition, val, 127f);
	}

	private bool CanSeeTargetEnemyRaycast(Vector3 position, Vector3 direction, float range)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		List<RayCastResult> list = new List<RayCastResult>();
		Ray val = default(Ray);
		((Ray)(ref val))._002Ector(position, direction);
		if (g.m_App.m_Space.RayCast(val, range, (IList<RayCastResult>)list))
		{
			list.Sort();
			for (int i = 0; i < list.Count; i++)
			{
				BroadPhaseEntry hitObject = list[i].HitObject;
				EntityCollidable val2 = (EntityCollidable)(object)((hitObject is EntityCollidable) ? hitObject : null);
				if (val2 == null)
				{
					return false;
				}
				if (val2.Entity.Tag is HitTag hitTag && hitTag.m_PlayerId != m_Id && hitTag.m_PlayerId == m_EnemyId && hitTag.m_HitZone != 255)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void SetAimAtRagdoll(int enemyId)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		m_BotAimVector = new Vector3(0f, -1f, 0f);
	}

	public void DeleteBot()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		g.m_App.m_RequestDeleteBotId = m_NetId;
		g.m_App.AddSystemMessage($"{GetName()} (BOT {m_NetId} LEFT)", Color.White);
		Delete();
	}

	public void BotTookDamage()
	{
		m_BeserkTime = (float)g.m_App.m_GameTime.TotalGameTime.TotalSeconds + 5f;
	}
}
