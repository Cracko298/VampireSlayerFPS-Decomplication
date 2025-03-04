using System;
using BEPUphysics;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionTests;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using BEPUphysics.NarrowPhaseSystems;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using BEPUphysics.Settings;
using Microsoft.Xna.Framework;

namespace BEPUphysicsDemos.AlternateMovement.Character;

public class QueryManager
{
	private RawList<ContactData> contacts = new RawList<ContactData>();

	private RawList<ContactData> supportContacts = new RawList<ContactData>();

	private RawList<ContactData> tractionContacts = new RawList<ContactData>();

	private RawList<ContactData> sideContacts = new RawList<ContactData>();

	private RawList<ContactData> headContacts = new RawList<ContactData>();

	private ConvexCollidable<CylinderShape> standingQueryObject;

	private ConvexCollidable<CylinderShape> crouchingQueryObject;

	private ConvexCollidable<CylinderShape> currentQueryObject;

	private CharacterController character;

	private Func<BroadPhaseEntry, bool> SupportRayFilter;

	public RawList<ContactData> Contacts => contacts;

	public RawList<ContactData> SupportContacts => supportContacts;

	public RawList<ContactData> TractionContacts => tractionContacts;

	public RawList<ContactData> SideContacts => sideContacts;

	public RawList<ContactData> HeadContacts => headContacts;

	public QueryManager(CharacterController character)
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		this.character = character;
		currentQueryObject = new ConvexCollidable<CylinderShape>(((Entity<ConvexCollidable<CylinderShape>>)(object)character.Body).CollisionInformation.Shape);
		standingQueryObject = new ConvexCollidable<CylinderShape>(new CylinderShape(character.StanceManager.StandingHeight, character.Body.Radius));
		crouchingQueryObject = new ConvexCollidable<CylinderShape>(new CylinderShape(character.StanceManager.CrouchingHeight, character.Body.Radius));
		((BroadPhaseEntry)currentQueryObject).CollisionRules = ((BroadPhaseEntry)((Entity<ConvexCollidable<CylinderShape>>)(object)character.Body).CollisionInformation).CollisionRules;
		((BroadPhaseEntry)standingQueryObject).CollisionRules = ((BroadPhaseEntry)((Entity<ConvexCollidable<CylinderShape>>)(object)character.Body).CollisionInformation).CollisionRules;
		((BroadPhaseEntry)crouchingQueryObject).CollisionRules = ((BroadPhaseEntry)((Entity<ConvexCollidable<CylinderShape>>)(object)character.Body).CollisionInformation).CollisionRules;
		SupportRayFilter = SupportRayFilterFunction;
	}

	internal void UpdateQueryShapes()
	{
		standingQueryObject.Shape.Radius = ((Entity<ConvexCollidable<CylinderShape>>)(object)character.Body).CollisionInformation.Shape.Radius;
		standingQueryObject.Shape.Height = character.StanceManager.StandingHeight;
		crouchingQueryObject.Shape.Radius = ((Entity<ConvexCollidable<CylinderShape>>)(object)character.Body).CollisionInformation.Shape.Radius;
		crouchingQueryObject.Shape.Height = character.StanceManager.CrouchingHeight;
	}

	private bool SupportRayFilterFunction(BroadPhaseEntry entry)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Invalid comparison between Unknown and I4
		return (int)CollisionRules.CollisionRuleCalculator((ICollisionRulesOwner)(object)entry, (ICollisionRulesOwner)(object)((Entity<ConvexCollidable<CylinderShape>>)(object)character.Body).CollisionInformation) == 1;
	}

	public bool RayCast(Ray ray, float length, out RayHit earliestHit)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		earliestHit = default(RayHit);
		earliestHit.T = float.MaxValue;
		CollidableCollection overlappedCollidables = ((Collidable)((Entity<ConvexCollidable<CylinderShape>>)(object)character.Body).CollisionInformation).OverlappedCollidables;
		Enumerator enumerator = ((CollidableCollection)(ref overlappedCollidables)).GetEnumerator();
		try
		{
			RayHit val = default(RayHit);
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				Collidable current = ((Enumerator)(ref enumerator)).Current;
				float? num = ((Ray)(ref ray)).Intersects(((BroadPhaseEntry)current).BoundingBox);
				if (num.HasValue)
				{
					float? num2 = num;
					if (num2.GetValueOrDefault() < length && num2.HasValue && ((BroadPhaseEntry)current).RayCast(ray, length, SupportRayFilter, ref val) && val.T < earliestHit.T)
					{
						earliestHit = val;
					}
				}
			}
		}
		finally
		{
			((IDisposable)(Enumerator)(ref enumerator)/*cast due to .constrained prefix*/).Dispose();
		}
		if (earliestHit.T == float.MaxValue)
		{
			return false;
		}
		return true;
	}

	public bool RayCast(Ray ray, float length, out RayHit earliestHit, out Collidable hitObject)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		earliestHit = default(RayHit);
		earliestHit.T = float.MaxValue;
		hitObject = null;
		CollidableCollection overlappedCollidables = ((Collidable)((Entity<ConvexCollidable<CylinderShape>>)(object)character.Body).CollisionInformation).OverlappedCollidables;
		Enumerator enumerator = ((CollidableCollection)(ref overlappedCollidables)).GetEnumerator();
		try
		{
			RayHit val = default(RayHit);
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				Collidable current = ((Enumerator)(ref enumerator)).Current;
				float? num = ((Ray)(ref ray)).Intersects(((BroadPhaseEntry)current).BoundingBox);
				if (num.HasValue)
				{
					float? num2 = num;
					if (num2.GetValueOrDefault() < length && num2.HasValue && ((BroadPhaseEntry)current).RayCast(ray, length, SupportRayFilter, ref val) && val.T < earliestHit.T)
					{
						earliestHit = val;
						hitObject = current;
					}
				}
			}
		}
		finally
		{
			((IDisposable)(Enumerator)(ref enumerator)/*cast due to .constrained prefix*/).Dispose();
		}
		if (earliestHit.T == float.MaxValue)
		{
			return false;
		}
		return true;
	}

	public bool RayCastHitAnything(Ray ray, float length)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		CollidableCollection overlappedCollidables = ((Collidable)((Entity<ConvexCollidable<CylinderShape>>)(object)character.Body).CollisionInformation).OverlappedCollidables;
		Enumerator enumerator = ((CollidableCollection)(ref overlappedCollidables)).GetEnumerator();
		try
		{
			RayHit val = default(RayHit);
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				Collidable current = ((Enumerator)(ref enumerator)).Current;
				float? num = ((Ray)(ref ray)).Intersects(((BroadPhaseEntry)current).BoundingBox);
				if (num.HasValue)
				{
					float? num2 = num;
					if (num2.GetValueOrDefault() < length && num2.HasValue && ((BroadPhaseEntry)current).RayCast(ray, length, SupportRayFilter, ref val))
					{
						return true;
					}
				}
			}
		}
		finally
		{
			((IDisposable)(Enumerator)(ref enumerator)/*cast due to .constrained prefix*/).Dispose();
		}
		return false;
	}

	private void ClearContacts()
	{
		contacts.Clear();
		supportContacts.Clear();
		tractionContacts.Clear();
		sideContacts.Clear();
		headContacts.Clear();
	}

	public void QueryContacts(Vector3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		QueryContacts(position, (EntityCollidable)(object)currentQueryObject);
	}

	public void QueryContacts(Vector3 position, Stance stance)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		QueryContacts(position, (EntityCollidable)(object)((stance == Stance.Standing) ? standingQueryObject : crouchingQueryObject));
	}

	private void QueryContacts(Vector3 position, EntityCollidable queryObject)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Invalid comparison between Unknown and I4
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Invalid comparison between Unknown and I4
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		ClearContacts();
		RigidTransform val = default(RigidTransform);
		val.Position = position;
		val.Orientation = ((Entity)character.Body).Orientation;
		queryObject.UpdateBoundingBoxForTransform(ref val, 0f);
		CollidableCollection overlappedCollidables = ((Collidable)((Entity<ConvexCollidable<CylinderShape>>)(object)character.Body).CollisionInformation).OverlappedCollidables;
		Enumerator enumerator = ((CollidableCollection)(ref overlappedCollidables)).GetEnumerator();
		try
		{
			ContactData val3 = default(ContactData);
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				Collidable current = ((Enumerator)(ref enumerator)).Current;
				BoundingBox boundingBox = ((BroadPhaseEntry)current).BoundingBox;
				if (!((BoundingBox)(ref boundingBox)).Intersects(((BroadPhaseEntry)queryObject).BoundingBox))
				{
					continue;
				}
				CollidablePair val2 = new CollidablePair(current, (Collidable)(object)queryObject);
				CollidablePairHandler pairHandler = NarrowPhaseHelper.GetPairHandler(ref val2);
				if ((int)((NarrowPhasePair)pairHandler).CollisionRule == 1)
				{
					((NarrowPhasePair)pairHandler).UpdateCollision(0f);
					Enumerator enumerator2 = pairHandler.Contacts.GetEnumerator();
					try
					{
						while (((Enumerator)(ref enumerator2)).MoveNext())
						{
							ContactInformation current2 = ((Enumerator)(ref enumerator2)).Current;
							if ((int)((NarrowPhasePair)current2.Pair).CollisionRule == 1)
							{
								val3.Position = current2.Contact.Position;
								val3.Normal = current2.Contact.Normal;
								val3.Id = current2.Contact.Id;
								val3.PenetrationDepth = current2.Contact.PenetrationDepth;
								contacts.Add(val3);
							}
						}
					}
					finally
					{
						((IDisposable)(Enumerator)(ref enumerator2)/*cast due to .constrained prefix*/).Dispose();
					}
				}
				((NarrowPhasePair)pairHandler).CleanUp();
				((NarrowPhasePair)pairHandler).Factory.GiveBack((NarrowPhasePair)(object)pairHandler);
			}
		}
		finally
		{
			((IDisposable)(Enumerator)(ref enumerator)/*cast due to .constrained prefix*/).Dispose();
		}
		CategorizeContacts(ref position);
	}

	private void CategorizeContacts(ref Vector3 position)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		Matrix3X3 orientationMatrix = ((Entity)character.Body).OrientationMatrix;
		Vector3 down = ((Matrix3X3)(ref orientationMatrix)).Down;
		Vector3 val = default(Vector3);
		float num = default(float);
		for (int i = 0; i < contacts.Count; i++)
		{
			Vector3.Subtract(ref contacts.Elements[i].Position, ref position, ref val);
			Vector3.Dot(ref contacts.Elements[i].Normal, ref val, ref num);
			ContactData val2 = contacts.Elements[i];
			if (num < 0f)
			{
				num = 0f - num;
				Vector3.Negate(ref val2.Normal, ref val2.Normal);
			}
			Vector3.Dot(ref val2.Normal, ref down, ref num);
			if (num > SupportFinder.SideContactThreshold)
			{
				supportContacts.Add(val2);
				if (num > character.SupportFinder.cosMaximumSlope)
				{
					tractionContacts.Add(val2);
				}
				else
				{
					sideContacts.Add(val2);
				}
			}
			else if (num < 0f - SupportFinder.SideContactThreshold)
			{
				headContacts.Add(val2);
			}
			else
			{
				sideContacts.Add(val2);
			}
		}
	}

	internal bool HasSupports(out bool hasTraction, out PositionState state, out ContactData supportContact)
	{
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		float num = float.MinValue;
		int num2 = -1;
		if (tractionContacts.Count > 0)
		{
			for (int i = 0; i < tractionContacts.Count; i++)
			{
				if (tractionContacts.Elements[i].PenetrationDepth > num)
				{
					num = tractionContacts.Elements[i].PenetrationDepth;
					num2 = i;
				}
			}
			hasTraction = true;
			supportContact = tractionContacts.Elements[num2];
		}
		else
		{
			if (supportContacts.Count <= 0)
			{
				hasTraction = false;
				state = PositionState.NoHit;
				supportContact = default(ContactData);
				return false;
			}
			for (int j = 0; j < supportContacts.Count; j++)
			{
				if (supportContacts.Elements[j].PenetrationDepth > num)
				{
					num = supportContacts.Elements[j].PenetrationDepth;
					num2 = j;
				}
			}
			hasTraction = false;
			supportContact = supportContacts.Elements[num2];
		}
		if (num > CollisionDetectionSettings.AllowedPenetration)
		{
			state = PositionState.TooDeep;
		}
		else if (num < 0f)
		{
			state = PositionState.NoHit;
		}
		else
		{
			state = PositionState.Accepted;
		}
		return true;
	}
}
