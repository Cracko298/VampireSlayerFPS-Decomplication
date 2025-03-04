using Microsoft.Xna.Framework;
using SgMotion;
using SynapseGaming.LightingSystem.Rendering;

namespace VampireFPS;

public class Actor
{
	public int m_Id;

	public Vector3 m_Position;

	public Vector3 m_PrevPosition;

	public Vector3 m_Rotation;

	public Vector3 m_PrevRotation;

	public Vector3 m_NetworkPosition;

	public float m_NetworkRotation;

	public SkinnedModel m_Model;

	public SceneObject m_SceneObject;

	public Matrix[] m_Transforms = (Matrix[])(object)new Matrix[75];

	public Actor()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		m_Id = -1;
		m_Position = Vector3.Zero;
		m_Rotation = Vector3.Zero;
		m_PrevPosition = Vector3.Zero;
		m_PrevRotation = Vector3.Zero;
		m_NetworkPosition = Vector3.Zero;
		m_NetworkRotation = 0f;
		m_Model = null;
		m_SceneObject = null;
	}

	public Vector3 Position()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return m_Position;
	}

	public Vector3 Rotation()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return m_Rotation;
	}

	public virtual void SetPosition(Vector3 pos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Position = pos;
	}

	public virtual void SetRotation(Vector3 rot)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Rotation = rot;
	}

	public void SetModel(SkinnedModel m)
	{
		m_Model = m;
	}
}
