using System;
using System.Collections.Generic;
using BEPUphysics;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Rendering;

namespace VampireFPS;

public class BEPUDebugDrawer : IDisposable
{
	public static class DisplayStaticMesh
	{
		public static void GetShapeMeshData(ISpaceObject shape, List<VertexPositionColor> vertices, List<int> indices)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			StaticMesh val = (StaticMesh)(object)((shape is StaticMesh) ? shape : null);
			if (val == null)
			{
				throw new ArgumentException("Wrong shape type");
			}
			for (int i = 0; i < val.Mesh.Data.Vertices.Length; i++)
			{
				vertices.Add(new VertexPositionColor(val.Mesh.Data.Vertices[i], debugColorStaticMesh));
			}
			indices.AddRange(val.Mesh.Data.Indices);
		}
	}

	public static class DisplaySphere
	{
		public static int NumSides = 24;

		public static void GetShapeMeshData(ISpaceObject shape, List<VertexPositionColor> vertices, List<int> indices)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			Sphere val = (Sphere)(object)((shape is Sphere) ? shape : null);
			if (val == null)
			{
				throw new ArgumentException("Wrong shape type");
			}
			Vector3 val2 = default(Vector3);
			float num = (float)Math.PI * 2f / (float)NumSides;
			float radius = val.Radius;
			vertices.Add(new VertexPositionColor(new Vector3(0f, radius, 0f), debugColorSphere));
			for (int i = 1; i < NumSides / 2; i++)
			{
				float num2 = (float)Math.PI / 2f - (float)i * num;
				float y = (float)Math.Sin(num2);
				float num3 = (float)Math.Cos(num2);
				for (int j = 0; j < NumSides; j++)
				{
					float num4 = (float)j * num;
					val2.X = (float)Math.Cos(num4) * num3;
					val2.Y = y;
					val2.Z = (float)Math.Sin(num4) * num3;
					vertices.Add(new VertexPositionColor(val2 * radius, debugColorSphere));
				}
			}
			vertices.Add(new VertexPositionColor(new Vector3(0f, 0f - radius, 0f), debugColorSphere));
			for (int k = 0; k < NumSides; k++)
			{
				indices.Add((ushort)(vertices.Count - 1));
				indices.Add((ushort)(vertices.Count - 2 - k));
				indices.Add((ushort)(vertices.Count - 2 - (k + 1) % NumSides));
			}
			for (int l = 0; l < NumSides / 2 - 2; l++)
			{
				for (int m = 0; m < NumSides; m++)
				{
					int num5 = (m + 1) % NumSides;
					indices.Add((ushort)(l * NumSides + num5 + 1));
					indices.Add((ushort)(l * NumSides + m + 1));
					indices.Add((ushort)((l + 1) * NumSides + m + 1));
					indices.Add((ushort)((l + 1) * NumSides + num5 + 1));
					indices.Add((ushort)(l * NumSides + num5 + 1));
					indices.Add((ushort)((l + 1) * NumSides + m + 1));
				}
			}
			for (int n = 0; n < NumSides; n++)
			{
				indices.Add(0);
				indices.Add((ushort)(n + 1));
				indices.Add((ushort)((n + 1) % NumSides + 1));
			}
		}
	}

	public static class DisplayBox
	{
		public static void GetShapeMeshData(ISpaceObject shape, List<VertexPositionColor> vertices, List<int> indices)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
			Box val = (Box)(object)((shape is Box) ? shape : null);
			if (val == null)
			{
				throw new ArgumentException("Wrong shape type.");
			}
			BoundingBox val2 = default(BoundingBox);
			((BoundingBox)(ref val2))._002Ector(new Vector3(0f - val.HalfWidth, 0f - val.HalfHeight, 0f - val.HalfLength), new Vector3(val.HalfWidth, val.HalfHeight, val.HalfLength));
			Vector3[] corners = ((BoundingBox)(ref val2)).GetCorners();
			vertices.Add(new VertexPositionColor(corners[0], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[1], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[2], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[3], debugColorBox));
			indices.Add(0);
			indices.Add(1);
			indices.Add(2);
			indices.Add(0);
			indices.Add(2);
			indices.Add(3);
			vertices.Add(new VertexPositionColor(corners[1], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[2], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[5], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[6], debugColorBox));
			indices.Add(4);
			indices.Add(6);
			indices.Add(7);
			indices.Add(4);
			indices.Add(7);
			indices.Add(5);
			vertices.Add(new VertexPositionColor(corners[4], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[5], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[6], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[7], debugColorBox));
			indices.Add(9);
			indices.Add(8);
			indices.Add(11);
			indices.Add(9);
			indices.Add(11);
			indices.Add(10);
			vertices.Add(new VertexPositionColor(corners[0], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[3], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[4], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[7], debugColorBox));
			indices.Add(14);
			indices.Add(12);
			indices.Add(13);
			indices.Add(14);
			indices.Add(13);
			indices.Add(15);
			vertices.Add(new VertexPositionColor(corners[0], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[1], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[4], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[5], debugColorBox));
			indices.Add(16);
			indices.Add(19);
			indices.Add(17);
			indices.Add(16);
			indices.Add(18);
			indices.Add(19);
			vertices.Add(new VertexPositionColor(corners[2], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[3], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[6], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[7], debugColorBox));
			indices.Add(21);
			indices.Add(20);
			indices.Add(22);
			indices.Add(21);
			indices.Add(22);
			indices.Add(23);
		}
	}

	public static class DisplayCapsule
	{
		public static int NumSides = 24;

		public static void GetShapeMeshData(ISpaceObject shape, List<VertexPositionColor> vertices, List<int> indices)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			Capsule val = (Capsule)(object)((shape is Capsule) ? shape : null);
			if (val == null)
			{
				throw new ArgumentException("Wrong shape type.");
			}
			Vector3 val2 = default(Vector3);
			Vector3 val3 = default(Vector3);
			((Vector3)(ref val3))._002Ector(0f, val.Length / 2f, 0f);
			float num = (float)Math.PI * 2f / (float)NumSides;
			float radius = val.Radius;
			vertices.Add(new VertexPositionColor(new Vector3(0f, radius + val.Length / 2f, 0f), debugColorCapsule));
			for (int i = 1; i <= NumSides / 4; i++)
			{
				float num2 = (float)Math.PI / 2f - (float)i * num;
				float y = (float)Math.Sin(num2);
				float num3 = (float)Math.Cos(num2);
				for (int j = 0; j < NumSides; j++)
				{
					float num4 = (float)j * num;
					val2.X = (float)Math.Cos(num4) * num3;
					val2.Y = y;
					val2.Z = (float)Math.Sin(num4) * num3;
					vertices.Add(new VertexPositionColor(val2 * radius + val3, debugColorCapsule));
				}
			}
			for (int k = NumSides / 4; k < NumSides / 2; k++)
			{
				float num5 = (float)Math.PI / 2f - (float)k * num;
				float y2 = (float)Math.Sin(num5);
				float num6 = (float)Math.Cos(num5);
				for (int l = 0; l < NumSides; l++)
				{
					float num7 = (float)l * num;
					val2.X = (float)Math.Cos(num7) * num6;
					val2.Y = y2;
					val2.Z = (float)Math.Sin(num7) * num6;
					vertices.Add(new VertexPositionColor(val2 * radius - val3, debugColorCapsule));
				}
			}
			vertices.Add(new VertexPositionColor(new Vector3(0f, 0f - radius - val.Length / 2f, 0f), debugColorCapsule));
			for (int m = 0; m < NumSides; m++)
			{
				indices.Add(vertices.Count - 1);
				indices.Add(vertices.Count - 2 - m);
				indices.Add(vertices.Count - 2 - (m + 1) % NumSides);
			}
			for (int n = 0; n < NumSides / 2 - 1; n++)
			{
				for (int num8 = 0; num8 < NumSides; num8++)
				{
					int num9 = (num8 + 1) % NumSides;
					indices.Add(n * NumSides + num9 + 1);
					indices.Add(n * NumSides + num8 + 1);
					indices.Add((n + 1) * NumSides + num8 + 1);
					indices.Add((n + 1) * NumSides + num9 + 1);
					indices.Add(n * NumSides + num9 + 1);
					indices.Add((n + 1) * NumSides + num8 + 1);
				}
			}
			for (int num10 = 0; num10 < NumSides; num10++)
			{
				indices.Add(0);
				indices.Add(num10 + 1);
				indices.Add((num10 + 1) % NumSides + 1);
			}
		}
	}

	public static class DisplayCylinder
	{
		public static void GetShapeMeshData(ISpaceObject shape, List<VertexPositionColor> vertices, List<int> indices)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_0398: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03da: Unknown result type (might be due to invalid IL or missing references)
			Cylinder val = (Cylinder)(object)((shape is Cylinder) ? shape : null);
			if (val == null)
			{
				throw new ArgumentException("Wrong shape type.");
			}
			BoundingBox val2 = default(BoundingBox);
			((BoundingBox)(ref val2))._002Ector(new Vector3(0f - val.Radius, (0f - val.Height) * 0.5f, 0f - val.Radius), new Vector3(val.Radius, val.Height * 0.5f, val.Radius));
			Vector3[] corners = ((BoundingBox)(ref val2)).GetCorners();
			vertices.Add(new VertexPositionColor(corners[0], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[1], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[2], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[3], debugColorBox));
			indices.Add(0);
			indices.Add(1);
			indices.Add(2);
			indices.Add(0);
			indices.Add(2);
			indices.Add(3);
			vertices.Add(new VertexPositionColor(corners[1], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[2], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[5], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[6], debugColorBox));
			indices.Add(4);
			indices.Add(6);
			indices.Add(7);
			indices.Add(4);
			indices.Add(7);
			indices.Add(5);
			vertices.Add(new VertexPositionColor(corners[4], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[5], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[6], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[7], debugColorBox));
			indices.Add(9);
			indices.Add(8);
			indices.Add(11);
			indices.Add(9);
			indices.Add(11);
			indices.Add(10);
			vertices.Add(new VertexPositionColor(corners[0], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[3], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[4], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[7], debugColorBox));
			indices.Add(14);
			indices.Add(12);
			indices.Add(13);
			indices.Add(14);
			indices.Add(13);
			indices.Add(15);
			vertices.Add(new VertexPositionColor(corners[0], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[1], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[4], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[5], debugColorBox));
			indices.Add(16);
			indices.Add(19);
			indices.Add(17);
			indices.Add(16);
			indices.Add(18);
			indices.Add(19);
			vertices.Add(new VertexPositionColor(corners[2], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[3], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[6], debugColorBox));
			vertices.Add(new VertexPositionColor(corners[7], debugColorBox));
			indices.Add(21);
			indices.Add(20);
			indices.Add(22);
			indices.Add(21);
			indices.Add(22);
			indices.Add(23);
		}
	}

	public static class DisplayBoundingBox
	{
		public static void GetShapeMeshData(SceneEntity shape, List<VertexPositionColor> vertices, List<int> indices)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			BoundingBox worldBoundingBox = shape.WorldBoundingBox;
			BoundingBox val = default(BoundingBox);
			((BoundingBox)(ref val))._002Ector(new Vector3(worldBoundingBox.Min.X, worldBoundingBox.Min.Y, worldBoundingBox.Min.Z), new Vector3(worldBoundingBox.Max.X, worldBoundingBox.Max.Y, worldBoundingBox.Max.Z));
			Vector3[] corners = ((BoundingBox)(ref val)).GetCorners();
			vertices.Add(new VertexPositionColor(corners[0], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[1], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[2], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[3], debugColorBB));
			indices.Add(0);
			indices.Add(1);
			indices.Add(2);
			indices.Add(0);
			indices.Add(2);
			indices.Add(3);
			vertices.Add(new VertexPositionColor(corners[1], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[2], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[5], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[6], debugColorBB));
			indices.Add(4);
			indices.Add(6);
			indices.Add(7);
			indices.Add(4);
			indices.Add(7);
			indices.Add(5);
			vertices.Add(new VertexPositionColor(corners[4], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[5], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[6], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[7], debugColorBB));
			indices.Add(9);
			indices.Add(8);
			indices.Add(11);
			indices.Add(9);
			indices.Add(11);
			indices.Add(10);
			vertices.Add(new VertexPositionColor(corners[0], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[3], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[4], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[7], debugColorBB));
			indices.Add(14);
			indices.Add(12);
			indices.Add(13);
			indices.Add(14);
			indices.Add(13);
			indices.Add(15);
			vertices.Add(new VertexPositionColor(corners[0], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[1], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[4], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[5], debugColorBB));
			indices.Add(16);
			indices.Add(19);
			indices.Add(17);
			indices.Add(16);
			indices.Add(18);
			indices.Add(19);
			vertices.Add(new VertexPositionColor(corners[2], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[3], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[6], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[7], debugColorBB));
			indices.Add(21);
			indices.Add(20);
			indices.Add(22);
			indices.Add(21);
			indices.Add(22);
			indices.Add(23);
		}
	}

	public static class DisplayMarker
	{
		public static void GetShapeMeshData(List<VertexPositionColor> vertices, List<int> indices)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			BoundingBox val = default(BoundingBox);
			((BoundingBox)(ref val))._002Ector(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, 0.5f));
			Vector3[] corners = ((BoundingBox)(ref val)).GetCorners();
			vertices.Add(new VertexPositionColor(corners[0], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[1], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[2], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[3], debugColorBB));
			indices.Add(0);
			indices.Add(1);
			indices.Add(2);
			indices.Add(0);
			indices.Add(2);
			indices.Add(3);
			vertices.Add(new VertexPositionColor(corners[1], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[2], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[5], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[6], debugColorBB));
			indices.Add(4);
			indices.Add(6);
			indices.Add(7);
			indices.Add(4);
			indices.Add(7);
			indices.Add(5);
			vertices.Add(new VertexPositionColor(corners[4], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[5], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[6], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[7], debugColorBB));
			indices.Add(9);
			indices.Add(8);
			indices.Add(11);
			indices.Add(9);
			indices.Add(11);
			indices.Add(10);
			vertices.Add(new VertexPositionColor(corners[0], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[3], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[4], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[7], debugColorBB));
			indices.Add(14);
			indices.Add(12);
			indices.Add(13);
			indices.Add(14);
			indices.Add(13);
			indices.Add(15);
			vertices.Add(new VertexPositionColor(corners[0], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[1], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[4], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[5], debugColorBB));
			indices.Add(16);
			indices.Add(19);
			indices.Add(17);
			indices.Add(16);
			indices.Add(18);
			indices.Add(19);
			vertices.Add(new VertexPositionColor(corners[2], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[3], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[6], debugColorBB));
			vertices.Add(new VertexPositionColor(corners[7], debugColorBB));
			indices.Add(21);
			indices.Add(20);
			indices.Add(22);
			indices.Add(21);
			indices.Add(22);
			indices.Add(23);
		}
	}

	public static class DisplayArrow
	{
		public static void GetShapeMeshData(List<VertexPositionColor> vertices, List<int> indices)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			vertices.Add(new VertexPositionColor(new Vector3(0f, 0f, 0f), Color.White));
			vertices.Add(new VertexPositionColor(new Vector3(0f, 0f, 1f), Color.White));
			vertices.Add(new VertexPositionColor(new Vector3(0f, 0.1f, 0f), Color.White));
			indices.Add(0);
			indices.Add(1);
			indices.Add(2);
			indices.Add(2);
			indices.Add(1);
			indices.Add(0);
		}
	}

	private static Color debugColorStaticMesh = Color.White;

	private static Color debugColorSphere = Color.Green;

	private static Color debugColorBox = Color.Red;

	private static Color debugColorCapsule = Color.Blue;

	private static Color debugColorBB = Color.Yellow;

	private BasicEffect basicEffect;

	private GraphicsDevice device;

	private List<VertexPositionColor> vertices;

	private List<int> indices;

	private List<VertexPositionColor> vertices2;

	private List<int> indices2;

	private RasterizerState wireFrameState;

	private RasterizerState oldFrameState;

	public BEPUDebugDrawer(GraphicsDevice g)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		device = g;
		basicEffect = new BasicEffect(device);
		basicEffect.AmbientLightColor = Vector3.One;
		basicEffect.VertexColorEnabled = true;
		wireFrameState = new RasterizerState();
		wireFrameState.FillMode = (FillMode)1;
	}

	public void Dispose()
	{
		if (basicEffect != null)
		{
			((GraphicsResource)basicEffect).Dispose();
		}
		if (vertices != null)
		{
			vertices.Clear();
		}
		if (indices != null)
		{
			indices.Clear();
		}
	}

	public void Draw(ISpaceObject shape, Matrix viewMatrix, Matrix projectionMatrix)
	{
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		vertices = new List<VertexPositionColor>();
		indices = new List<int>();
		if (shape is Capsule)
		{
			DisplayCapsule.GetShapeMeshData(shape, vertices, indices);
		}
		else if (shape is Box)
		{
			DisplayBox.GetShapeMeshData(shape, vertices, indices);
		}
		else if (shape is Sphere)
		{
			DisplaySphere.GetShapeMeshData(shape, vertices, indices);
		}
		else if (shape is StaticMesh)
		{
			DisplayStaticMesh.GetShapeMeshData(shape, vertices, indices);
		}
		else if (shape is Cylinder)
		{
			DisplayCylinder.GetShapeMeshData(shape, vertices, indices);
		}
		bool flag = false;
		if (shape is Capsule)
		{
			flag = true;
		}
		else if (shape is Box)
		{
			flag = true;
		}
		else if (shape is Sphere)
		{
			flag = true;
		}
		else if (shape is StaticMesh)
		{
			flag = true;
		}
		else if (shape is Cylinder)
		{
			flag = true;
		}
		if (!flag || vertices.Count <= 0 || indices.Count <= 0)
		{
			return;
		}
		device.BlendState = BlendState.Opaque;
		device.DepthStencilState = DepthStencilState.Default;
		oldFrameState = device.RasterizerState;
		device.RasterizerState = wireFrameState;
		basicEffect.View = viewMatrix;
		basicEffect.Projection = projectionMatrix;
		if (shape is Capsule)
		{
			basicEffect.World = ((Entity)((shape is Capsule) ? shape : null)).WorldTransform;
		}
		else if (shape is Box)
		{
			basicEffect.World = ((Entity)((shape is Box) ? shape : null)).WorldTransform;
		}
		else if (shape is Sphere)
		{
			basicEffect.World = ((Entity)((shape is Sphere) ? shape : null)).WorldTransform;
		}
		else if (shape is StaticMesh)
		{
			BasicEffect obj = basicEffect;
			AffineTransform worldTransform = ((StaticMesh)((shape is StaticMesh) ? shape : null)).WorldTransform;
			obj.World = ((AffineTransform)(ref worldTransform)).Matrix;
		}
		else if (shape is Cylinder)
		{
			basicEffect.World = ((Entity)((shape is Cylinder) ? shape : null)).WorldTransform;
		}
		foreach (EffectPass pass in ((Effect)basicEffect).CurrentTechnique.Passes)
		{
			pass.Apply();
			device.DrawUserIndexedPrimitives<VertexPositionColor>((PrimitiveType)0, vertices.ToArray(), 0, vertices.Count, indices.ToArray(), 0, indices.Count / 3);
		}
		device.RasterizerState = oldFrameState;
	}

	public void Draw(SceneEntity shape, Matrix viewMatrix, Matrix projectionMatrix)
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		vertices2 = new List<VertexPositionColor>();
		indices2 = new List<int>();
		DisplayBoundingBox.GetShapeMeshData(shape, vertices2, indices2);
		if (vertices2.Count <= 0 || indices2.Count <= 0)
		{
			return;
		}
		device.BlendState = BlendState.Opaque;
		device.DepthStencilState = DepthStencilState.Default;
		oldFrameState = device.RasterizerState;
		device.RasterizerState = wireFrameState;
		basicEffect.View = viewMatrix;
		basicEffect.Projection = projectionMatrix;
		basicEffect.World = Matrix.Identity;
		foreach (EffectPass pass in ((Effect)basicEffect).CurrentTechnique.Passes)
		{
			pass.Apply();
			device.DrawUserIndexedPrimitives<VertexPositionColor>((PrimitiveType)0, vertices2.ToArray(), 0, vertices2.Count, indices2.ToArray(), 0, indices2.Count / 3);
		}
		device.RasterizerState = oldFrameState;
	}

	public void Draw(Vector3 pos, Matrix viewMatrix, Matrix projectionMatrix)
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		vertices2 = new List<VertexPositionColor>();
		indices2 = new List<int>();
		DisplayMarker.GetShapeMeshData(vertices2, indices2);
		if (vertices2.Count <= 0 || indices2.Count <= 0)
		{
			return;
		}
		device.BlendState = BlendState.Opaque;
		device.DepthStencilState = DepthStencilState.Default;
		oldFrameState = device.RasterizerState;
		device.RasterizerState = wireFrameState;
		basicEffect.View = viewMatrix;
		basicEffect.Projection = projectionMatrix;
		basicEffect.World = Matrix.CreateTranslation(pos);
		foreach (EffectPass pass in ((Effect)basicEffect).CurrentTechnique.Passes)
		{
			pass.Apply();
			device.DrawUserIndexedPrimitives<VertexPositionColor>((PrimitiveType)0, vertices2.ToArray(), 0, vertices2.Count, indices2.ToArray(), 0, indices2.Count / 3);
		}
		device.RasterizerState = oldFrameState;
	}

	public void Draw(Matrix world, Matrix viewMatrix, Matrix projectionMatrix)
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		vertices2 = new List<VertexPositionColor>();
		indices2 = new List<int>();
		DisplayArrow.GetShapeMeshData(vertices2, indices2);
		if (vertices2.Count <= 0 || indices2.Count <= 0)
		{
			return;
		}
		device.BlendState = BlendState.Opaque;
		device.DepthStencilState = DepthStencilState.Default;
		oldFrameState = device.RasterizerState;
		device.RasterizerState = wireFrameState;
		basicEffect.View = viewMatrix;
		basicEffect.Projection = projectionMatrix;
		basicEffect.World = world;
		foreach (EffectPass pass in ((Effect)basicEffect).CurrentTechnique.Passes)
		{
			pass.Apply();
			device.DrawUserIndexedPrimitives<VertexPositionColor>((PrimitiveType)0, vertices2.ToArray(), 0, vertices2.Count, indices2.ToArray(), 0, indices2.Count / 3);
		}
		device.RasterizerState = oldFrameState;
	}
}
