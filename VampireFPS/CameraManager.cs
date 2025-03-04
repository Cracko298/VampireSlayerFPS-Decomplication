using System;
using BEPUphysics.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Rendering;

namespace VampireFPS;

public class CameraManager
{
	private const float DEFAULT_FOV_DEGREES = 45f;

	public Vector3 m_Position;

	public Vector3 m_LookAt;

	public Matrix m_ProjectionMatrix;

	public Matrix m_ViewMatrix;

	private float m_Fov;

	public float m_TargetFov;

	public float m_BobY;

	public float m_Pitch;

	public void Init()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		float num = MathHelper.ToRadians(45f);
		Viewport viewport = g.m_App.graphics.GraphicsDevice.Viewport;
		m_ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(num, ((Viewport)(ref viewport)).AspectRatio, 0.1f, 200f);
		m_BobY = 0f;
		m_Pitch = 0f;
		m_Fov = 45f;
		m_TargetFov = 45f;
	}

	public void Update()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		switch (g.m_PlayerManager.GetLocalPlayer().m_State)
		{
		case Player.STATE.JoinTeam:
		case Player.STATE.ChooseCharacter:
			m_Position = new Vector3(0f, 20f, 0f);
			m_LookAt = new Vector3(0f, 15f, 10f);
			m_ViewMatrix = Matrix.CreateLookAt(m_Position, m_LookAt, Vector3.Up);
			break;
		case Player.STATE.InGame:
		{
			Matrix val = Matrix.CreateRotationY((float)Math.PI) * g.m_PlayerManager.GetLocalPlayer().m_ViewAnimationController.GetBoneAbsoluteTransform("Bip01_Head");
			val *= ((SceneEntity)g.m_PlayerManager.GetLocalPlayer().m_ViewSceneObject).World;
			m_Position = ((Matrix)(ref val)).Translation;
			m_Position += GetBob();
			Matrix val2 = Matrix.CreateRotationX(m_Pitch + g.m_PlayerManager.GetLocalPlayer().m_PunchAngle) * ((SceneEntity)g.m_PlayerManager.GetLocalPlayer().m_ViewSceneObject).World;
			m_LookAt = m_Position + ((Matrix)(ref val2)).Forward * 10f;
			m_ViewMatrix = Matrix.CreateLookAt(m_Position, m_LookAt, Vector3.Up);
			m_Fov = MathHelper.Lerp(m_Fov, m_TargetFov, 0.1f);
			float num2 = MathHelper.ToRadians(m_Fov);
			Viewport viewport = g.m_App.graphics.GraphicsDevice.Viewport;
			m_ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(num2, ((Viewport)(ref viewport)).AspectRatio, 0.1f, g.m_App.environment.VisibleDistance);
			break;
		}
		case Player.STATE.LocalDeath:
		{
			m_Position.X = ((Entity)g.m_PlayerManager.GetLocalPlayer().m_CharacterController.Body).Position.X;
			m_Position.Y = ((Entity)g.m_PlayerManager.GetLocalPlayer().m_CharacterController.Body).Position.Y;
			m_Position.Z = ((Entity)g.m_PlayerManager.GetLocalPlayer().m_CharacterController.Body).Position.Z;
			float num = g.m_PlayerManager.GetLocalPlayer().m_Position.Y - 1.778f;
			if (m_Position.Y > num)
			{
				ref Vector3 position = ref m_Position;
				position.Y -= 0.25f;
			}
			else
			{
				m_Position.Y = num;
			}
			m_ViewMatrix = Matrix.CreateLookAt(m_Position, m_LookAt, Vector3.Up);
			m_ViewMatrix *= Matrix.CreateRotationZ(MathHelper.ToRadians(10f));
			break;
		}
		}
	}

	public Vector3 GetBob()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		if (!g.m_PlayerManager.GetLocalPlayer().m_CharacterController.SupportFinder.HasTraction)
		{
			return Vector3.Zero;
		}
		Vector3 val = g.m_PlayerManager.GetLocalPlayer().m_FrameMove * 0.25f;
		val.Y = 0f;
		float num = ((Vector3)(ref val)).Length();
		float num2 = (float)Math.Sin(m_BobY);
		if (num > 1E-06f)
		{
			g.m_SoundManager.PlayLocalFootsteps(num2);
		}
		num2 *= 0.75f;
		num2 *= num;
		m_BobY += 4.5f * num;
		return new Vector3(0f, 0f - num2, 0f);
	}

	public Vector3 WorldToScreen(Vector3 position)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		Matrix identity = Matrix.Identity;
		((Matrix)(ref identity)).Translation = position;
		Viewport viewport = ((Game)g.m_App).GraphicsDevice.Viewport;
		return ((Viewport)(ref viewport)).Project(Vector3.Zero, m_ProjectionMatrix, m_ViewMatrix, identity);
	}

	public void SetTargetFov(float degrees)
	{
		m_TargetFov = degrees;
	}

	public void SetDefaultFov()
	{
		m_TargetFov = 45f;
	}

	public Vector3 QuaternionToEuler(Quaternion q)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		Vector3 result = default(Vector3);
		result.X = (float)Math.Atan2(2f * q.Y * q.W - 2f * q.X * q.Z, 1.0 - 2.0 * Math.Pow(q.Y, 2.0) - 2.0 * Math.Pow(q.Z, 2.0));
		result.Y = (float)Math.Asin(2f * q.X * q.Y + 2f * q.Z * q.W);
		result.Z = (float)Math.Atan2(2f * q.X * q.W - 2f * q.Y * q.Z, 1.0 - 2.0 * Math.Pow(q.X, 2.0) - 2.0 * Math.Pow(q.Z, 2.0));
		if ((double)(q.X * q.Y + q.Z * q.W) == 0.5)
		{
			result.X = (float)(2.0 * Math.Atan2(q.X, q.W));
			result.Z = 0f;
		}
		else if ((double)(q.X * q.Y + q.Z * q.W) == -0.5)
		{
			result.X = (float)(-2.0 * Math.Atan2(q.X, q.W));
			result.Z = 0f;
		}
		return result;
	}
}
