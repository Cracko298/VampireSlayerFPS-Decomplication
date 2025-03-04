using System;
using System.Collections.Generic;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.BroadPhaseSystems;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionTests;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.MathExtensions;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using Microsoft.Xna.Framework;

namespace BEPUphysicsDemos.AlternateMovement.Character;

public class SupportFinder
{
	internal static float SideContactThreshold = 0.01f;

	internal RawList<SupportContact> supports = new RawList<SupportContact>();

	internal RawList<OtherContact> sideContacts = new RawList<OtherContact>();

	internal RawList<OtherContact> headContacts = new RawList<OtherContact>();

	private float bottomHeight;

	private CharacterController character;

	internal float sinMaximumSlope = (float)Math.Sin(MathHelper.ToRadians(60f) + 0.01f);

	internal float cosMaximumSlope = (float)Math.Cos(MathHelper.ToRadians(60f) + 0.01f);

	public float RayLengthToBottom => bottomHeight;

	public SupportData? SupportData
	{
		get
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			if (supports.Count > 0)
			{
				SupportData supportData = default(SupportData);
				supportData.Position = supports.Elements[0].Contact.Position;
				supportData.Normal = supports.Elements[0].Contact.Normal;
				SupportData value = supportData;
				for (int i = 1; i < supports.Count; i++)
				{
					Vector3.Add(ref value.Position, ref supports.Elements[i].Contact.Position, ref value.Position);
					Vector3.Add(ref value.Normal, ref supports.Elements[i].Contact.Normal, ref value.Normal);
				}
				if (supports.Count > 1)
				{
					Vector3.Multiply(ref value.Position, 1f / (float)supports.Count, ref value.Position);
					float num = ((Vector3)(ref value.Normal)).LengthSquared();
					if (num < 1E-07f)
					{
						value.Normal = supports.Elements[0].Contact.Normal;
					}
					else
					{
						Vector3.Multiply(ref value.Normal, 1f / (float)Math.Sqrt(num), ref value.Normal);
					}
				}
				float num2 = float.MinValue;
				Collidable supportObject = null;
				float num3 = default(float);
				for (int j = 0; j < supports.Count; j++)
				{
					Vector3.Dot(ref supports.Elements[j].Contact.Normal, ref value.Normal, ref num3);
					num3 *= supports.Elements[j].Contact.PenetrationDepth;
					if (num3 > num2)
					{
						num2 = num3;
						supportObject = supports.Elements[j].Support;
					}
				}
				value.Depth = num2;
				value.SupportObject = supportObject;
				return value;
			}
			if (SupportRayData.HasValue)
			{
				SupportData value2 = default(SupportData);
				value2.Position = SupportRayData.Value.HitData.Location;
				value2.Normal = SupportRayData.Value.HitData.Normal;
				value2.HasTraction = SupportRayData.Value.HasTraction;
				Matrix3X3 orientationMatrix = ((Entity)character.Body).OrientationMatrix;
				value2.Depth = Vector3.Dot(((Matrix3X3)(ref orientationMatrix)).Down, SupportRayData.Value.HitData.Normal) * (bottomHeight - SupportRayData.Value.HitData.T);
				value2.SupportObject = SupportRayData.Value.HitObject;
				return value2;
			}
			return null;
		}
	}

	public SupportData? TractionData
	{
		get
		{
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			if (supports.Count > 0)
			{
				SupportData value = default(SupportData);
				int num = 0;
				for (int i = 0; i < supports.Count; i++)
				{
					if (supports.Elements[i].HasTraction)
					{
						num++;
						Vector3.Add(ref value.Position, ref supports.Elements[i].Contact.Position, ref value.Position);
						Vector3.Add(ref value.Normal, ref supports.Elements[i].Contact.Normal, ref value.Normal);
					}
				}
				if (num > 1)
				{
					Vector3.Multiply(ref value.Position, 1f / (float)num, ref value.Position);
					float num2 = ((Vector3)(ref value.Normal)).LengthSquared();
					if (num2 < 1E-05f)
					{
						value.Normal = supports.Elements[0].Contact.Normal;
					}
					else
					{
						Vector3.Multiply(ref value.Normal, 1f / (float)Math.Sqrt(num2), ref value.Normal);
					}
				}
				if (num > 0)
				{
					float num3 = float.MinValue;
					Collidable supportObject = null;
					float num4 = default(float);
					for (int j = 0; j < supports.Count; j++)
					{
						if (supports.Elements[j].HasTraction)
						{
							Vector3.Dot(ref supports.Elements[j].Contact.Normal, ref value.Normal, ref num4);
							num4 *= supports.Elements[j].Contact.PenetrationDepth;
							if (num4 > num3)
							{
								num3 = num4;
								supportObject = supports.Elements[j].Support;
							}
						}
					}
					value.Depth = num3;
					value.SupportObject = supportObject;
					value.HasTraction = true;
					return value;
				}
			}
			if (SupportRayData.HasValue && SupportRayData.Value.HasTraction)
			{
				SupportData value2 = default(SupportData);
				value2.Position = SupportRayData.Value.HitData.Location;
				value2.Normal = SupportRayData.Value.HitData.Normal;
				value2.HasTraction = true;
				Matrix3X3 orientationMatrix = ((Entity)character.Body).OrientationMatrix;
				value2.Depth = Vector3.Dot(((Matrix3X3)(ref orientationMatrix)).Down, SupportRayData.Value.HitData.Normal) * (bottomHeight - SupportRayData.Value.HitData.T);
				value2.SupportObject = SupportRayData.Value.HitObject;
				return value2;
			}
			return null;
		}
	}

	public bool HasSupport { get; private set; }

	public bool HasTraction { get; private set; }

	public SupportRayData? SupportRayData { get; private set; }

	public ReadOnlyList<SupportContact> Supports => new ReadOnlyList<SupportContact>((IList<SupportContact>)supports);

	public ReadOnlyList<OtherContact> SideContacts => new ReadOnlyList<OtherContact>((IList<OtherContact>)sideContacts);

	public ReadOnlyList<OtherContact> HeadContacts => new ReadOnlyList<OtherContact>((IList<OtherContact>)headContacts);

	public TractionSupportCollection TractionSupports => new TractionSupportCollection(supports);

	public float MaximumSlope
	{
		get
		{
			return (float)Math.Acos(MathHelper.Clamp(cosMaximumSlope, -1f, 1f));
		}
		set
		{
			cosMaximumSlope = (float)Math.Cos(value);
			sinMaximumSlope = (float)Math.Sin(value);
		}
	}

	public bool GetTractionInDirection(ref Vector3 movementDirection, out SupportData supportData)
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		if (HasTraction)
		{
			int num = -1;
			float num2 = float.MinValue;
			float num3 = default(float);
			for (int i = 0; i < supports.Count; i++)
			{
				if (supports.Elements[i].HasTraction)
				{
					Vector3.Dot(ref movementDirection, ref supports.Elements[i].Contact.Normal, ref num3);
					if (num3 > num2)
					{
						num2 = num3;
						num = i;
					}
				}
			}
			if (num != -1)
			{
				supportData.Position = supports.Elements[num].Contact.Position;
				supportData.Normal = supports.Elements[num].Contact.Normal;
				supportData.SupportObject = supports.Elements[num].Support;
				supportData.HasTraction = true;
				float num4 = float.MinValue;
				float num5 = default(float);
				for (int j = 0; j < supports.Count; j++)
				{
					if (supports.Elements[j].HasTraction)
					{
						Vector3.Dot(ref supports.Elements[j].Contact.Normal, ref supportData.Normal, ref num5);
						num5 *= supports.Elements[j].Contact.PenetrationDepth;
						if (num5 > num4)
						{
							num4 = num5;
						}
					}
				}
				supportData.Depth = num4;
				return true;
			}
			if (SupportRayData.HasValue && SupportRayData.Value.HasTraction)
			{
				supportData.Position = SupportRayData.Value.HitData.Location;
				supportData.Normal = SupportRayData.Value.HitData.Normal;
				Matrix3X3 orientationMatrix = ((Entity)character.Body).OrientationMatrix;
				supportData.Depth = Vector3.Dot(((Matrix3X3)(ref orientationMatrix)).Down, SupportRayData.Value.HitData.Normal) * (bottomHeight - SupportRayData.Value.HitData.T);
				supportData.SupportObject = SupportRayData.Value.HitObject;
				supportData.HasTraction = true;
				return true;
			}
			supportData = default(SupportData);
			return false;
		}
		supportData = default(SupportData);
		return false;
	}

	public SupportFinder(CharacterController character)
	{
		this.character = character;
	}

	public void UpdateSupports()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Invalid comparison between Unknown and I4
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Invalid comparison between Unknown and I4
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0573: Unknown result type (might be due to invalid IL or missing references)
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0580: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_0591: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_059b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0716: Unknown result type (might be due to invalid IL or missing references)
		//IL_071b: Unknown result type (might be due to invalid IL or missing references)
		//IL_071d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0722: Unknown result type (might be due to invalid IL or missing references)
		//IL_0729: Unknown result type (might be due to invalid IL or missing references)
		//IL_0733: Unknown result type (might be due to invalid IL or missing references)
		//IL_0738: Unknown result type (might be due to invalid IL or missing references)
		//IL_073d: Unknown result type (might be due to invalid IL or missing references)
		//IL_073e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0743: Unknown result type (might be due to invalid IL or missing references)
		//IL_074b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0750: Unknown result type (might be due to invalid IL or missing references)
		//IL_0757: Unknown result type (might be due to invalid IL or missing references)
		//IL_0761: Unknown result type (might be due to invalid IL or missing references)
		//IL_0766: Unknown result type (might be due to invalid IL or missing references)
		//IL_076b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0774: Unknown result type (might be due to invalid IL or missing references)
		//IL_077b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0780: Unknown result type (might be due to invalid IL or missing references)
		//IL_0785: Unknown result type (might be due to invalid IL or missing references)
		//IL_0795: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0876: Unknown result type (might be due to invalid IL or missing references)
		//IL_0890: Unknown result type (might be due to invalid IL or missing references)
		//IL_089a: Unknown result type (might be due to invalid IL or missing references)
		//IL_089f: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0903: Unknown result type (might be due to invalid IL or missing references)
		//IL_0908: Unknown result type (might be due to invalid IL or missing references)
		//IL_090d: Unknown result type (might be due to invalid IL or missing references)
		//IL_090e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0913: Unknown result type (might be due to invalid IL or missing references)
		//IL_091b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0920: Unknown result type (might be due to invalid IL or missing references)
		//IL_0927: Unknown result type (might be due to invalid IL or missing references)
		//IL_0931: Unknown result type (might be due to invalid IL or missing references)
		//IL_0936: Unknown result type (might be due to invalid IL or missing references)
		//IL_093b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0944: Unknown result type (might be due to invalid IL or missing references)
		//IL_094b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0950: Unknown result type (might be due to invalid IL or missing references)
		//IL_0955: Unknown result type (might be due to invalid IL or missing references)
		//IL_0965: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Expected O, but got Unknown
		//IL_0356: Expected O, but got Unknown
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Expected O, but got Unknown
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0636: Unknown result type (might be due to invalid IL or missing references)
		//IL_0806: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d6: Unknown result type (might be due to invalid IL or missing references)
		bool hasTraction = HasTraction;
		HasTraction = false;
		HasSupport = false;
		Cylinder body = character.Body;
		Matrix3X3 orientationMatrix = ((Entity)character.Body).OrientationMatrix;
		Vector3 down = ((Matrix3X3)(ref orientationMatrix)).Down;
		supports.Clear();
		sideContacts.Clear();
		headContacts.Clear();
		Vector3 position = ((Entity)character.Body).Position;
		Enumerator<CollidablePairHandler> enumerator = ((Collidable)((Entity<ConvexCollidable<CylinderShape>>)(object)character.Body).CollisionInformation).Pairs.GetEnumerator();
		try
		{
			Vector3 val = default(Vector3);
			float num = default(float);
			OtherContact otherContact = default(OtherContact);
			OtherContact otherContact2 = default(OtherContact);
			while (enumerator.MoveNext())
			{
				CollidablePairHandler current = enumerator.Current;
				if ((int)((NarrowPhasePair)current).CollisionRule != 1)
				{
					continue;
				}
				Enumerator enumerator2 = current.Contacts.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						ContactInformation current2 = ((Enumerator)(ref enumerator2)).Current;
						if ((int)((NarrowPhasePair)current2.Pair).CollisionRule != 1)
						{
							continue;
						}
						Vector3.Subtract(ref current2.Contact.Position, ref position, ref val);
						Vector3.Dot(ref val, ref current2.Contact.Normal, ref num);
						Vector3 normal = current2.Contact.Normal;
						if (num < 0f)
						{
							Vector3.Negate(ref normal, ref normal);
							num = 0f - num;
						}
						Vector3.Dot(ref normal, ref down, ref num);
						if (num > SideContactThreshold)
						{
							HasSupport = true;
							SupportContact supportContact = default(SupportContact);
							supportContact.Contact = new ContactData
							{
								Position = current2.Contact.Position,
								Normal = normal,
								PenetrationDepth = current2.Contact.PenetrationDepth,
								Id = current2.Contact.Id
							};
							BroadPhaseOverlap broadPhaseOverlap = ((NarrowPhasePair)current).BroadPhaseOverlap;
							_003F val2;
							if (((BroadPhaseOverlap)(ref broadPhaseOverlap)).EntryA == ((Entity<ConvexCollidable<CylinderShape>>)(object)body).CollisionInformation)
							{
								BroadPhaseOverlap broadPhaseOverlap2 = ((NarrowPhasePair)current).BroadPhaseOverlap;
								val2 = (Collidable)((BroadPhaseOverlap)(ref broadPhaseOverlap2)).EntryB;
							}
							else
							{
								BroadPhaseOverlap broadPhaseOverlap3 = ((NarrowPhasePair)current).BroadPhaseOverlap;
								val2 = (Collidable)((BroadPhaseOverlap)(ref broadPhaseOverlap3)).EntryA;
							}
							supportContact.Support = (Collidable)val2;
							SupportContact supportContact2 = supportContact;
							if (num > cosMaximumSlope)
							{
								supportContact2.HasTraction = true;
								HasTraction = true;
							}
							else
							{
								sideContacts.Add(new OtherContact
								{
									Collidable = supportContact2.Support,
									Contact = supportContact2.Contact
								});
							}
							supports.Add(supportContact2);
						}
						else if (num < 0f - SideContactThreshold)
						{
							BroadPhaseOverlap broadPhaseOverlap4 = ((NarrowPhasePair)current).BroadPhaseOverlap;
							_003F val3;
							if (((BroadPhaseOverlap)(ref broadPhaseOverlap4)).EntryA == ((Entity<ConvexCollidable<CylinderShape>>)(object)body).CollisionInformation)
							{
								BroadPhaseOverlap broadPhaseOverlap5 = ((NarrowPhasePair)current).BroadPhaseOverlap;
								val3 = (Collidable)((BroadPhaseOverlap)(ref broadPhaseOverlap5)).EntryB;
							}
							else
							{
								BroadPhaseOverlap broadPhaseOverlap6 = ((NarrowPhasePair)current).BroadPhaseOverlap;
								val3 = (Collidable)((BroadPhaseOverlap)(ref broadPhaseOverlap6)).EntryA;
							}
							otherContact.Collidable = (Collidable)val3;
							otherContact.Contact.Position = current2.Contact.Position;
							otherContact.Contact.Normal = normal;
							otherContact.Contact.PenetrationDepth = current2.Contact.PenetrationDepth;
							otherContact.Contact.Id = current2.Contact.Id;
							headContacts.Add(otherContact);
						}
						else
						{
							BroadPhaseOverlap broadPhaseOverlap7 = ((NarrowPhasePair)current).BroadPhaseOverlap;
							_003F val4;
							if (((BroadPhaseOverlap)(ref broadPhaseOverlap7)).EntryA == ((Entity<ConvexCollidable<CylinderShape>>)(object)body).CollisionInformation)
							{
								BroadPhaseOverlap broadPhaseOverlap8 = ((NarrowPhasePair)current).BroadPhaseOverlap;
								val4 = (Collidable)((BroadPhaseOverlap)(ref broadPhaseOverlap8)).EntryB;
							}
							else
							{
								BroadPhaseOverlap broadPhaseOverlap9 = ((NarrowPhasePair)current).BroadPhaseOverlap;
								val4 = (Collidable)((BroadPhaseOverlap)(ref broadPhaseOverlap9)).EntryA;
							}
							otherContact2.Collidable = (Collidable)val4;
							otherContact2.Contact.Position = current2.Contact.Position;
							otherContact2.Contact.Normal = normal;
							otherContact2.Contact.PenetrationDepth = current2.Contact.PenetrationDepth;
							otherContact2.Contact.Id = current2.Contact.Id;
							sideContacts.Add(otherContact2);
						}
					}
				}
				finally
				{
					((IDisposable)(Enumerator)(ref enumerator2)/*cast due to .constrained prefix*/).Dispose();
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		SupportRayData = null;
		bottomHeight = body.Height * 0.25f;
		if (!HasTraction && hasTraction)
		{
			float length = (hasTraction ? (bottomHeight + character.StepManager.MaximumStepHeight) : bottomHeight);
			Ray ray = new Ray(((Entity)body).Position + down * body.Height * 0.25f, down);
			if (TryDownCast(ref ray, length, out var _, out var supportRayData))
			{
				SupportRayData = supportRayData;
				HasTraction = supportRayData.HasTraction;
				HasSupport = true;
			}
		}
		Vector2 movementDirection = character.HorizontalMotionConstraint.MovementDirection;
		bool flag = ((Vector2)(ref movementDirection)).LengthSquared() > 0f;
		if (!HasTraction && hasTraction && flag)
		{
			Ray ray2 = new Ray(((Entity)body).Position + new Vector3(character.HorizontalMotionConstraint.MovementDirection.X, 0f, character.HorizontalMotionConstraint.MovementDirection.Y) * (character.Body.Radius - ((ConvexShape)((Entity<ConvexCollidable<CylinderShape>>)(object)character.Body).CollisionInformation.Shape).CollisionMargin) + down * body.Height * 0.25f, down);
			Ray ray3 = default(Ray);
			ray3.Position = ((Entity)body).Position + down * body.Height * 0.25f;
			ray3.Direction = ray2.Position - ray3.Position;
			if (!character.QueryManager.RayCastHitAnything(ray3, 1f))
			{
				float length2 = (hasTraction ? (bottomHeight + character.StepManager.MaximumStepHeight) : bottomHeight);
				if (TryDownCast(ref ray2, length2, out var hasTraction3, out var supportRayData2) && (!SupportRayData.HasValue || supportRayData2.HitData.T < SupportRayData.Value.HitData.T))
				{
					if (hasTraction3)
					{
						SupportRayData = supportRayData2;
						HasTraction = true;
					}
					else if (!SupportRayData.HasValue)
					{
						SupportRayData = supportRayData2;
					}
					HasSupport = true;
				}
			}
		}
		if (!HasTraction && hasTraction && flag)
		{
			Vector3 val5 = new Vector3(character.HorizontalMotionConstraint.MovementDirection.X, 0f, character.HorizontalMotionConstraint.MovementDirection.Y);
			Vector3.Cross(ref val5, ref down, ref val5);
			Vector3.Multiply(ref val5, character.Body.Radius - ((ConvexShape)((Entity<ConvexCollidable<CylinderShape>>)(object)character.Body).CollisionInformation.Shape).CollisionMargin, ref val5);
			Ray ray4 = new Ray(((Entity)body).Position + val5 + down * body.Height * 0.25f, down);
			Ray ray5 = default(Ray);
			ray5.Position = ((Entity)body).Position + down * body.Height * 0.25f;
			ray5.Direction = ray4.Position - ray5.Position;
			if (!character.QueryManager.RayCastHitAnything(ray5, 1f))
			{
				float length3 = (hasTraction ? (bottomHeight + character.StepManager.MaximumStepHeight) : bottomHeight);
				if (TryDownCast(ref ray4, length3, out var hasTraction4, out var supportRayData3) && (!SupportRayData.HasValue || supportRayData3.HitData.T < SupportRayData.Value.HitData.T))
				{
					if (hasTraction4)
					{
						SupportRayData = supportRayData3;
						HasTraction = true;
					}
					else if (!SupportRayData.HasValue)
					{
						SupportRayData = supportRayData3;
					}
					HasSupport = true;
				}
			}
		}
		if (HasTraction || !hasTraction || !flag)
		{
			return;
		}
		Vector3 val6 = new Vector3(character.HorizontalMotionConstraint.MovementDirection.X, 0f, character.HorizontalMotionConstraint.MovementDirection.Y);
		Vector3.Cross(ref down, ref val6, ref val6);
		Vector3.Multiply(ref val6, character.Body.Radius - ((ConvexShape)((Entity<ConvexCollidable<CylinderShape>>)(object)character.Body).CollisionInformation.Shape).CollisionMargin, ref val6);
		Ray ray6 = new Ray(((Entity)body).Position + val6 + down * body.Height * 0.25f, down);
		Ray ray7 = default(Ray);
		ray7.Position = ((Entity)body).Position + down * body.Height * 0.25f;
		ray7.Direction = ray6.Position - ray7.Position;
		if (character.QueryManager.RayCastHitAnything(ray7, 1f))
		{
			return;
		}
		float length4 = (hasTraction ? (bottomHeight + character.StepManager.MaximumStepHeight) : bottomHeight);
		if (TryDownCast(ref ray6, length4, out var hasTraction5, out var supportRayData4) && (!SupportRayData.HasValue || supportRayData4.HitData.T < SupportRayData.Value.HitData.T))
		{
			if (hasTraction5)
			{
				SupportRayData = supportRayData4;
				HasTraction = true;
			}
			else if (!SupportRayData.HasValue)
			{
				SupportRayData = supportRayData4;
			}
			HasSupport = true;
		}
	}

	private bool TryDownCast(ref Ray ray, float length, out bool hasTraction, out SupportRayData supportRayData)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		supportRayData = default(SupportRayData);
		hasTraction = false;
		if (character.QueryManager.RayCast(ray, length, out var earliestHit, out var hitObject))
		{
			float num = ((Vector3)(ref earliestHit.Normal)).LengthSquared();
			if (num < 1E-07f)
			{
				return false;
			}
			Vector3.Divide(ref earliestHit.Normal, (float)Math.Sqrt(num), ref earliestHit.Normal);
			((Vector3)(ref earliestHit.Normal)).Normalize();
			float num2 = default(float);
			Vector3.Dot(ref ray.Direction, ref earliestHit.Normal, ref num2);
			if (num2 < 0f)
			{
				Vector3.Negate(ref earliestHit.Normal, ref earliestHit.Normal);
				num2 = 0f - num2;
			}
			if (num2 > cosMaximumSlope)
			{
				hasTraction = true;
				supportRayData = new SupportRayData
				{
					HitData = earliestHit,
					HitObject = hitObject,
					HasTraction = true
				};
			}
			else
			{
				if (!(num2 > SideContactThreshold))
				{
					return false;
				}
				supportRayData = new SupportRayData
				{
					HitData = earliestHit,
					HitObject = hitObject
				};
			}
			return true;
		}
		return false;
	}

	internal void ClearSupportData()
	{
		HasSupport = false;
		HasTraction = false;
		supports.Clear();
		SupportRayData = null;
	}
}
