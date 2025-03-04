using System;
using System.IO;
using EasyStorage;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;

namespace VampireFPS;

public class LoadSaveManager
{
	public SaveGame m_SaveGame;

	public LoadSaveManager()
	{
		m_SaveGame = new SaveGame();
	}

	public void SaveGame()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		FileAction val = null;
		try
		{
			if (Guide.IsTrialMode || Guide.IsVisible || !((ISaveDevice)g.m_App.saveDevice).IsReady)
			{
				return;
			}
			IAsyncSaveDevice saveDevice = g.m_App.saveDevice;
			if (val == null)
			{
				val = (FileAction)delegate(Stream stream)
				{
					BinaryWriter binaryWriter = new BinaryWriter(stream);
					m_SaveGame.Save(binaryWriter);
					binaryWriter.Close();
					g.m_App.m_bSaveExists = true;
				};
			}
			((ISaveDevice)saveDevice).Save("VampireSlayerFPS", "5827248A747F", val);
		}
		catch (Exception arg)
		{
			Console.WriteLine("{0} Exception caught.", arg);
		}
	}

	public void LoadGame()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		FileAction val = null;
		try
		{
			if (Guide.IsTrialMode || Guide.IsVisible)
			{
				return;
			}
			if (((ISaveDevice)g.m_App.saveDevice).FileExists("VampireSlayerFPS", "5827248A747F"))
			{
				IAsyncSaveDevice saveDevice = g.m_App.saveDevice;
				if (val == null)
				{
					val = (FileAction)delegate(Stream stream)
					{
						BinaryReader binaryReader = new BinaryReader(stream);
						m_SaveGame.Restore(binaryReader);
						binaryReader.Close();
						g.m_App.m_bSaveExists = true;
					};
				}
				((ISaveDevice)saveDevice).Load("VampireSlayerFPS", "5827248A747F", val);
			}
			else
			{
				g.m_App.m_bSaveExists = false;
			}
		}
		catch (Exception arg)
		{
			Console.WriteLine("{0} Exception caught.", arg);
		}
	}

	public void SaveOptions()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		try
		{
			if (!Guide.IsTrialMode && !Guide.IsVisible && ((ISaveDevice)g.m_App.saveDevice).IsReady)
			{
				((ISaveDevice)g.m_App.saveDevice).Save("VampireSlayerFPS", "C593FF0B553C", (FileAction)delegate(Stream stream)
				{
					BinaryWriter binaryWriter = new BinaryWriter(stream);
					binaryWriter.Write(g.m_App.m_OptionsHoriz);
					binaryWriter.Write(g.m_App.m_OptionsVert);
					binaryWriter.Write(g.m_App.m_OptionsInvertY);
					binaryWriter.Write(g.m_App.m_OptionsVibration);
					binaryWriter.Write(g.m_App.m_OptionsBotsSP);
					binaryWriter.Write(g.m_App.m_OptionsBotsMP);
					binaryWriter.Write(g.m_App.m_OptionsMapTime);
					binaryWriter.Write(g.m_App.m_OptionsBlood);
					binaryWriter.Write(g.m_App.m_OptionsVol);
					binaryWriter.Close();
					g.m_App.m_bSaveExists = true;
				});
			}
		}
		catch (Exception arg)
		{
			Console.WriteLine("{0} Exception caught.", arg);
		}
	}

	public void LoadOptions()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		try
		{
			if (Guide.IsTrialMode || Guide.IsVisible)
			{
				return;
			}
			if (((ISaveDevice)g.m_App.saveDevice).FileExists("VampireSlayerFPS", "C593FF0B553C"))
			{
				((ISaveDevice)g.m_App.saveDevice).Load("VampireSlayerFPS", "C593FF0B553C", (FileAction)delegate(Stream stream)
				{
					BinaryReader binaryReader = new BinaryReader(stream);
					g.m_App.m_OptionsHoriz = binaryReader.ReadSingle();
					g.m_App.m_OptionsVert = binaryReader.ReadSingle();
					g.m_App.m_OptionsInvertY = binaryReader.ReadBoolean();
					g.m_App.m_OptionsVibration = binaryReader.ReadBoolean();
					g.m_App.m_OptionsBotsSP = binaryReader.ReadInt32();
					g.m_App.m_OptionsBotsMP = binaryReader.ReadInt32();
					g.m_App.m_OptionsMapTime = binaryReader.ReadInt32();
					g.m_App.m_OptionsBlood = binaryReader.ReadBoolean();
					g.m_App.m_OptionsVol = binaryReader.ReadSingle();
					binaryReader.Close();
					SoundEffect.MasterVolume = g.m_App.m_OptionsVol;
					g.m_App.m_bSaveExists = true;
				});
			}
			else
			{
				g.m_App.m_bSaveExists = false;
			}
		}
		catch (Exception arg)
		{
			Console.WriteLine("{0} Exception caught.", arg);
		}
	}
}
