using System.IO;

namespace VampireFPS;

public class SaveGame
{
	private const int XOR = 65521;

	public void Save(BinaryWriter writer)
	{
		writer.Write(g.m_PlayerManager.GetLocalPlayer().m_Rank ^ 0xFFF1);
		writer.Write(g.m_PlayerManager.GetLocalPlayer().m_XP ^ 0xFFF1);
	}

	public void Restore(BinaryReader reader)
	{
		g.m_PlayerManager.GetLocalPlayer().m_Rank = reader.ReadInt32() ^ 0xFFF1;
		g.m_PlayerManager.GetLocalPlayer().m_XP = reader.ReadInt32() ^ 0xFFF1;
		g.m_PlayerManager.GetLocalPlayer().SetXPForNextLevel();
	}
}
