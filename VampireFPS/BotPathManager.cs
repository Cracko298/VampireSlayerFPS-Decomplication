using System.IO;
using Microsoft.Xna.Framework;

namespace VampireFPS;

public class BotPathManager
{
	public enum BotNodeType
	{
		FOLLOW,
		END
	}

	public const int MAX_BOTNODES = 1024;

	public BotNode[] m_BotNode;

	public bool m_bCreatePath;

	public int m_PrevBotNodeID = -1;

	public int m_NumNodes = -1;

	public bool m_bDoneLineOfSightRaycast;

	public BotPathManager()
	{
		m_BotNode = new BotNode[1024];
		for (int i = 0; i < 1024; i++)
		{
			m_BotNode[i] = default(BotNode);
		}
		DeleteAll();
		m_bCreatePath = false;
	}

	public int Create(int type, Vector3 pos)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		int num = -1;
		for (int i = 0; i < 1024; i++)
		{
			if (m_BotNode[i].m_Type == -1)
			{
				flag = true;
				num = i;
				break;
			}
		}
		if (flag)
		{
			m_BotNode[num].m_Type = type;
			m_BotNode[num].m_Position = pos;
			return num;
		}
		return -1;
	}

	public void Delete(int Id)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		m_BotNode[Id].m_Type = -1;
		m_BotNode[Id].m_Position = Vector3.Zero;
	}

	public void DeleteAll()
	{
		for (int i = 0; i < 1024; i++)
		{
			if (m_BotNode[i].m_Type != -1)
			{
				Delete(i);
			}
		}
	}

	public void Update()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (m_bCreatePath)
		{
			Vector3 position = g.m_PlayerManager.GetLocalPlayer().m_Position;
			if (m_PrevBotNodeID != -1)
			{
				Vector3 val = position - m_BotNode[m_PrevBotNodeID].m_Position;
				float num = ((Vector3)(ref val)).LengthSquared();
				if (num > 25f)
				{
					m_PrevBotNodeID = Create(0, position);
				}
			}
			else
			{
				m_PrevBotNodeID = Create(0, position);
			}
		}
		m_bDoneLineOfSightRaycast = false;
	}

	public void Render()
	{
	}

	public void Save()
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (!m_bCreatePath)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < 1024; i++)
		{
			if (m_BotNode[i].m_Type != -1)
			{
				num++;
			}
		}
		string path = $"botdata{g.m_App.m_Level}.dat";
		path = Path.Combine("C:\\Dev\\XNA\\VampireFPS\\VampireFPSContent", path);
		FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Write);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream);
		binaryWriter.Write(num);
		_ = Vector2.Zero;
		for (int j = 0; j < 1024; j++)
		{
			if (m_BotNode[j].m_Type != -1)
			{
				binaryWriter.Write(m_BotNode[j].m_Type);
				binaryWriter.Write((int)(m_BotNode[j].m_Position.X * 1000f));
				binaryWriter.Write((int)(m_BotNode[j].m_Position.Y * 1000f));
				binaryWriter.Write((int)(m_BotNode[j].m_Position.Z * 1000f));
			}
		}
		fileStream.Close();
		m_bCreatePath = false;
		m_PrevBotNodeID = -1;
	}

	public void LoadBotPath()
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		if (m_BotNode[0].m_Type != -1)
		{
			return;
		}
		string path = $"Content\\botdata{g.m_App.m_Level}.dat";
		FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
		if (fileStream.Length != 0)
		{
			BinaryReader binaryReader = new BinaryReader(fileStream);
			m_NumNodes = binaryReader.ReadInt32();
			Vector3 zero = Vector3.Zero;
			for (int i = 0; i < m_NumNodes; i++)
			{
				int type = binaryReader.ReadInt32();
				int num = binaryReader.ReadInt32();
				zero.X = (float)num * 0.001f;
				int num2 = binaryReader.ReadInt32();
				zero.Y = (float)num2 * 0.001f;
				int num3 = binaryReader.ReadInt32();
				zero.Z = (float)num3 * 0.001f;
				Create(type, zero);
			}
			fileStream.Close();
		}
	}

	public int FindNearestNode(Vector3 pos)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		float num = 9999999f;
		int result = -1;
		float num2 = 0f;
		for (int i = 0; i < 1024; i++)
		{
			if (m_BotNode[i].m_Type != -1)
			{
				Vector3 val = m_BotNode[i].m_Position - pos;
				num2 = ((Vector3)(ref val)).LengthSquared();
				if (num2 < num)
				{
					num = num2;
					result = i;
				}
			}
		}
		return result;
	}

	public int FindNearestNodeInRange(Vector3 pos, float radiusSq)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		float num = 9999999f;
		int result = -1;
		float num2 = 0f;
		for (int i = 0; i < 1024; i++)
		{
			if (m_BotNode[i].m_Type != -1)
			{
				Vector3 val = m_BotNode[i].m_Position - pos;
				num2 = ((Vector3)(ref val)).LengthSquared();
				if (num2 < num && num2 < radiusSq)
				{
					num = num2;
					result = i;
				}
			}
		}
		return result;
	}
}
