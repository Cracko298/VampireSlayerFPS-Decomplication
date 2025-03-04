using System;
using BEPUphysics;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionTests;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using BEPUphysics.Settings;
using Microsoft.Xna.Framework;

namespace BEPUphysicsDemos.AlternateMovement.Character;

public class StepManager
{
	private CharacterController character;

	private float maximumStepHeight = 1f;

	private float minimumDownStepHeight = 0.1f;

	private float minimumUpStepHeight;

	private RawList<ContactData> stepContacts = new RawList<ContactData>();

	private float upStepMargin = 0.1f;

	public float MaximumStepHeight
	{
		get
		{
			return maximumStepHeight;
		}
		set
		{
			if (maximumStepHeight < 0f)
			{
				throw new Exception("Value must be nonnegative.");
			}
			maximumStepHeight = value;
		}
	}

	public float MinimumDownStepHeight
	{
		get
		{
			return minimumDownStepHeight;
		}
		set
		{
			if (minimumDownStepHeight < 0f)
			{
				throw new Exception("Value must be nonnegative.");
			}
			minimumDownStepHeight = value;
		}
	}

	public StepManager(CharacterController character)
	{
		this.character = character;
		minimumUpStepHeight = CollisionDetectionSettings.AllowedPenetration * 1.1f;
	}

	private bool IsDownStepObstructed(RawList<ContactData> sideContacts)
	{
		for (int i = 0; i < sideContacts.Count; i++)
		{
			if (IsObstructiveToDownStepping(ref sideContacts.Elements[i]))
			{
				return true;
			}
		}
		return false;
	}

	private bool IsObstructiveToDownStepping(ref ContactData contact)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (character.SupportFinder.SideContacts.Count == 0 && contact.PenetrationDepth > CollisionDetectionSettings.AllowedPenetration)
		{
			return true;
		}
		Enumerator<OtherContact> enumerator = character.SupportFinder.SideContacts.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				OtherContact current = enumerator.Current;
				float num = Vector3.Dot(contact.Normal, current.Contact.Normal);
				float num2 = num * current.Contact.PenetrationDepth;
				if (num2 > current.Contact.PenetrationDepth)
				{
					return true;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		return false;
	}

	public bool TryToStepDown(out Vector3 newPosition)
	{
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		if (character.SupportFinder.supports.Count == 0 && character.SupportFinder.SupportRayData.HasValue && character.SupportFinder.SupportRayData.Value.HasTraction && character.SupportFinder.SupportRayData.Value.HitData.T - character.SupportFinder.RayLengthToBottom > minimumDownStepHeight)
		{
			Vector3 normal = character.SupportFinder.SupportRayData.Value.HitData.Normal;
			Matrix3X3 orientationMatrix = ((Entity)character.Body).OrientationMatrix;
			Vector3 down = ((Matrix3X3)(ref orientationMatrix)).Down;
			RigidTransform worldTransform = ((EntityCollidable)((Entity<ConvexCollidable<CylinderShape>>)(object)character.Body).CollisionInformation).WorldTransform;
			Ray val = default(Ray);
			((ConvexShape)((Entity<ConvexCollidable<CylinderShape>>)(object)character.Body).CollisionInformation.Shape).GetExtremePoint(normal, ref worldTransform, ref val.Position);
			val.Direction = down;
			Plane val2 = new Plane(normal, Vector3.Dot(character.SupportFinder.SupportRayData.Value.HitData.Location, normal));
			float num = 0f;
			float num2 = ((ConvexShape)((Entity<ConvexCollidable<CylinderShape>>)(object)character.Body).CollisionInformation.Shape).CollisionMargin + character.SupportFinder.SupportRayData.Value.HitData.T - character.SupportFinder.RayLengthToBottom;
			float num3 = num2;
			float num4 = default(float);
			Vector3 val3 = default(Vector3);
			float hintOffset;
			if (Toolbox.GetRayPlaneIntersection(ref val, ref val2, ref num4, ref val3))
			{
				num3 = num4 + CollisionDetectionSettings.AllowedPenetration;
				Vector3 position = ((Entity)character.Body).Position + down * num3;
				switch (TryDownStepPosition(ref position, out hintOffset))
				{
				case PositionState.Accepted:
					num3 += hintOffset;
					if (num3 > minimumDownStepHeight && num3 < maximumStepHeight)
					{
						newPosition = ((Entity)character.Body).Position + num3 * down;
						return true;
					}
					newPosition = default(Vector3);
					return false;
				case PositionState.NoHit:
					num = num3 + hintOffset;
					num3 = (num2 + num3) * 0.5f;
					break;
				case PositionState.Obstructed:
					num2 = num3;
					num3 = (num + num3) * 0.5f;
					break;
				case PositionState.TooDeep:
					num3 += hintOffset;
					num2 = num3;
					break;
				}
			}
			int num5 = 0;
			while (num5++ < 5 && num2 - num > 1E-05f)
			{
				Vector3 position = ((Entity)character.Body).Position + num3 * down;
				switch (TryDownStepPosition(ref position, out hintOffset))
				{
				case PositionState.Accepted:
					num3 += hintOffset;
					if (num3 > minimumDownStepHeight && num3 < maximumStepHeight)
					{
						newPosition = ((Entity)character.Body).Position + num3 * down;
						return true;
					}
					newPosition = default(Vector3);
					return false;
				case PositionState.NoHit:
					num = num3 + hintOffset;
					num3 = (num2 + num) * 0.5f;
					break;
				case PositionState.Obstructed:
					num2 = num3;
					num3 = (num + num2) * 0.5f;
					break;
				case PositionState.TooDeep:
					num3 += hintOffset;
					num2 = num3;
					break;
				}
			}
			newPosition = default(Vector3);
			return false;
		}
		newPosition = default(Vector3);
		return false;
	}

	private PositionState TryDownStepPosition(ref Vector3 position, out float hintOffset)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		hintOffset = 0f;
		character.QueryManager.QueryContacts(position);
		bool flag = IsDownStepObstructed(character.QueryManager.SideContacts);
		if (character.QueryManager.HasSupports(out var _, out var state, out var supportContact) && !flag)
		{
			switch (state)
			{
			case PositionState.Accepted:
			{
				Vector3 normal3 = supportContact.Normal;
				Matrix3X3 orientationMatrix3 = ((Entity)character.Body).OrientationMatrix;
				hintOffset = (0f - Vector3.Dot(normal3, ((Matrix3X3)(ref orientationMatrix3)).Down)) * supportContact.PenetrationDepth;
				return PositionState.Accepted;
			}
			case PositionState.TooDeep:
			{
				Vector3 normal2 = supportContact.Normal;
				Matrix3X3 orientationMatrix2 = ((Entity)character.Body).OrientationMatrix;
				hintOffset = Math.Min(0f, 0.001f - Vector3.Dot(normal2, ((Matrix3X3)(ref orientationMatrix2)).Down) * supportContact.PenetrationDepth);
				return PositionState.TooDeep;
			}
			default:
			{
				Vector3 normal = supportContact.Normal;
				Matrix3X3 orientationMatrix = ((Entity)character.Body).OrientationMatrix;
				hintOffset = -0.001f - Vector3.Dot(normal, ((Matrix3X3)(ref orientationMatrix)).Down) * supportContact.PenetrationDepth;
				return PositionState.NoHit;
			}
			}
		}
		if (flag)
		{
			return PositionState.Obstructed;
		}
		return PositionState.NoHit;
	}

	public bool TryToStepUp(out Vector3 newPosition)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (character.SupportFinder.HasTraction)
		{
			stepContacts.Clear();
			FindUpStepCandidates(stepContacts);
			for (int i = 0; i < stepContacts.Count; i++)
			{
				if (TryToStepUsingContact(ref stepContacts.Elements[i], out newPosition))
				{
					return true;
				}
			}
		}
		newPosition = default(Vector3);
		return false;
	}

	private void FindUpStepCandidates(RawList<ContactData> outputStepCandidates)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		Enumerator<OtherContact> enumerator = character.SupportFinder.sideContacts.GetEnumerator();
		try
		{
			float num = default(float);
			while (enumerator.MoveNext())
			{
				OtherContact current = enumerator.Current;
				Vector3 val = default(Vector3);
				val.X = character.HorizontalMotionConstraint.MovementDirection.X;
				val.Z = character.HorizontalMotionConstraint.MovementDirection.Y;
				Vector3 val2 = val;
				ContactData contact = current.Contact;
				Vector3.Dot(ref contact.Normal, ref val2, ref num);
				if (!(num > 0f))
				{
					continue;
				}
				Matrix3X3 orientationMatrix = ((Entity)character.Body).OrientationMatrix;
				num = Vector3.Dot(((Matrix3X3)(ref orientationMatrix)).Down, current.Contact.Position - ((Entity)character.Body).Position);
				if (!(num < character.Body.Height * 0.5f) || !(num > character.Body.Height * 0.5f - maximumStepHeight - upStepMargin))
				{
					continue;
				}
				bool flag = true;
				for (int i = 0; i < outputStepCandidates.Count; i++)
				{
					Vector3.Dot(ref outputStepCandidates.Elements[i].Normal, ref contact.Normal, ref num);
					if (num > 0.99f)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					outputStepCandidates.Add(contact);
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
	}

	private bool TryToStepUsingContact(ref ContactData contact, out Vector3 newPosition)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		Matrix3X3 orientationMatrix = ((Entity)character.Body).OrientationMatrix;
		Vector3 down = ((Matrix3X3)(ref orientationMatrix)).Down;
		Vector3 position = ((Entity)character.Body).Position;
		Vector3 sideNormal = contact.Normal;
		float num = default(float);
		Vector3.Dot(ref sideNormal, ref down, ref num);
		Vector3 val = default(Vector3);
		Vector3.Multiply(ref down, num, ref val);
		Vector3.Subtract(ref sideNormal, ref val, ref sideNormal);
		((Vector3)(ref sideNormal)).Normalize();
		float height = character.Body.Height;
		Ray ray = default(Ray);
		Vector3.Multiply(ref down, character.Body.Height * 0.5f - height, ref ray.Position);
		Vector3.Add(ref ray.Position, ref position, ref ray.Position);
		ray.Direction = sideNormal;
		float collisionMargin = ((ConvexShape)((Entity<ConvexCollidable<CylinderShape>>)(object)character.Body).CollisionInformation.Shape).CollisionMargin;
		float num2 = character.Body.Radius + collisionMargin;
		if (character.QueryManager.RayCastHitAnything(ray, num2))
		{
			newPosition = default(Vector3);
			return false;
		}
		Vector3 val2 = default(Vector3);
		Vector3.Multiply(ref sideNormal, num2, ref val2);
		Vector3.Add(ref ray.Position, ref val2, ref ray.Position);
		ray.Direction = down;
		RayHit earliestHit = default(RayHit);
		if (!character.QueryManager.RayCast(ray, height, out earliestHit) || earliestHit.T <= 0f || earliestHit.T - height > 0f - minimumUpStepHeight || earliestHit.T - height < 0f - maximumStepHeight - upStepMargin)
		{
			newPosition = default(Vector3);
			return false;
		}
		Vector3 val3 = default(Vector3);
		Vector3.Normalize(ref earliestHit.Normal, ref val3);
		Vector3.Dot(ref val3, ref down, ref num);
		if (num < 0f)
		{
			Vector3.Negate(ref val3, ref val3);
			num = 0f - num;
		}
		if (num < character.SupportFinder.cosMaximumSlope)
		{
			newPosition = default(Vector3);
			return false;
		}
		Vector3.Negate(ref down, ref ray.Direction);
		float length = character.Body.Height - earliestHit.T;
		if (character.QueryManager.RayCastHitAnything(ray, length))
		{
			newPosition = default(Vector3);
			return false;
		}
		RigidTransform worldTransform = ((EntityCollidable)((Entity<ConvexCollidable<CylinderShape>>)(object)character.Body).CollisionInformation).WorldTransform;
		Vector3.Multiply(ref sideNormal, collisionMargin, ref val2);
		Vector3.Add(ref worldTransform.Position, ref val2, ref worldTransform.Position);
		Vector3 val4 = default(Vector3);
		Vector3.Multiply(ref down, 0f - height, ref val4);
		Vector3.Add(ref worldTransform.Position, ref val4, ref worldTransform.Position);
		Ray val5 = default(Ray);
		((ConvexShape)((Entity<ConvexCollidable<CylinderShape>>)(object)character.Body).CollisionInformation.Shape).GetExtremePoint(val3, ref worldTransform, ref val5.Position);
		val5.Direction = down;
		Vector3.Dot(ref earliestHit.Location, ref val3, ref num);
		Plane val6 = new Plane(val3, num);
		float num3 = 0f - maximumStepHeight;
		float num4 = ((ConvexShape)((Entity<ConvexCollidable<CylinderShape>>)(object)character.Body).CollisionInformation.Shape).CollisionMargin - height + earliestHit.T;
		float num5 = num4;
		Vector3 val7 = default(Vector3);
		float num6 = default(float);
		float hintOffset;
		if (Toolbox.GetRayPlaneIntersection(ref val5, ref val6, ref num6, ref val7))
		{
			num6 = 0f - height + num6 + CollisionDetectionSettings.AllowedPenetration;
			if (num6 < num3)
			{
				num6 = num3;
			}
			num5 = num6;
			if (num5 > num4)
			{
				num4 = num5;
			}
			Vector3 position2 = ((Entity)character.Body).Position + down * num5 + val2;
			switch (TryUpStepPosition(ref sideNormal, ref position2, out hintOffset))
			{
			case PositionState.Accepted:
				num5 += hintOffset;
				if (num5 < 0f && num5 > 0f - maximumStepHeight - CollisionDetectionSettings.AllowedPenetration)
				{
					newPosition = ((Entity)character.Body).Position + Math.Max(0f - maximumStepHeight, num5) * down + val2;
					return true;
				}
				newPosition = default(Vector3);
				return false;
			case PositionState.Rejected:
				newPosition = default(Vector3);
				return false;
			case PositionState.NoHit:
				num3 = num5 + hintOffset;
				num5 = (num4 + num5) * 0.5f;
				break;
			case PositionState.Obstructed:
				num4 = num5;
				num5 = (num3 + num5) * 0.5f;
				break;
			case PositionState.HeadObstructed:
				num3 = num5 + hintOffset;
				num5 = (num4 + num5) * 0.5f;
				break;
			case PositionState.TooDeep:
				num5 += hintOffset;
				num4 = num5;
				break;
			}
		}
		int num7 = 0;
		while (num7++ < 5 && num4 - num3 > 1E-05f)
		{
			Vector3 position2 = ((Entity)character.Body).Position + num5 * down + val2;
			switch (TryUpStepPosition(ref sideNormal, ref position2, out hintOffset))
			{
			case PositionState.Accepted:
				num5 += hintOffset;
				if (num5 < 0f && num5 > 0f - maximumStepHeight - CollisionDetectionSettings.AllowedPenetration)
				{
					newPosition = ((Entity)character.Body).Position + Math.Max(0f - maximumStepHeight, num5) * down + val2;
					return true;
				}
				newPosition = default(Vector3);
				return false;
			case PositionState.Rejected:
				newPosition = default(Vector3);
				return false;
			case PositionState.NoHit:
				num3 = num5 + hintOffset;
				num5 = (num4 + num3) * 0.5f;
				break;
			case PositionState.Obstructed:
				num4 = num5;
				num5 = (num3 + num4) * 0.5f;
				break;
			case PositionState.HeadObstructed:
				num3 = num5 + hintOffset;
				num5 = (num4 + num5) * 0.5f;
				break;
			case PositionState.TooDeep:
				num5 += hintOffset;
				num4 = num5;
				break;
			}
		}
		newPosition = default(Vector3);
		return false;
	}

	private PositionState TryUpStepPosition(ref Vector3 sideNormal, ref Vector3 position, out float hintOffset)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		hintOffset = 0f;
		character.QueryManager.QueryContacts(position);
		if (character.QueryManager.HeadContacts.Count > 0)
		{
			Matrix3X3 orientationMatrix = ((Entity)character.Body).OrientationMatrix;
			Vector3 up = ((Matrix3X3)(ref orientationMatrix)).Up;
			float num = default(float);
			Vector3.Dot(ref up, ref character.QueryManager.HeadContacts.Elements[0].Normal, ref num);
			hintOffset = num * character.QueryManager.HeadContacts.Elements[0].PenetrationDepth;
			for (int i = 1; i < character.QueryManager.HeadContacts.Count; i++)
			{
				Vector3.Dot(ref up, ref character.QueryManager.HeadContacts.Elements[i].Normal, ref num);
				num *= character.QueryManager.HeadContacts.Elements[i].PenetrationDepth;
				if (num > hintOffset)
				{
					hintOffset = num;
				}
			}
			return PositionState.HeadObstructed;
		}
		bool flag = IsUpStepObstructed(ref sideNormal, character.QueryManager.SideContacts, character.QueryManager.HeadContacts);
		if (character.QueryManager.HasSupports(out var hasTraction, out var state, out var supportContact) && !flag)
		{
			switch (state)
			{
			case PositionState.Accepted:
			{
				if (hasTraction)
				{
					Vector3 normal3 = supportContact.Normal;
					Matrix3X3 orientationMatrix4 = ((Entity)character.Body).OrientationMatrix;
					hintOffset = Math.Min(0f, Vector3.Dot(normal3, ((Matrix3X3)(ref orientationMatrix4)).Down) * (CollisionDetectionSettings.AllowedPenetration * 0.5f - supportContact.PenetrationDepth));
					return PositionState.Accepted;
				}
				Matrix3X3 orientationMatrix5 = ((Entity)character.Body).OrientationMatrix;
				Vector3 down = ((Matrix3X3)(ref orientationMatrix5)).Down;
				Ray ray = default(Ray);
				ray.Position = supportContact.Position + sideNormal * 0.001f;
				float num2 = Vector3.Dot(ray.Position - position, down);
				num2 = character.Body.Height * 0.5f + num2;
				ray.Position -= num2 * down;
				ray.Direction = down;
				Vector3 val = position;
				Matrix3X3 orientationMatrix6 = ((Entity)character.Body).OrientationMatrix;
				Ray ray2 = default(Ray);
				ray2.Position = val + ((Matrix3X3)(ref orientationMatrix6)).Up * (character.Body.Height * 0.5f);
				ray2.Direction = ray.Position - ray2.Position;
				if (!character.QueryManager.RayCastHitAnything(ray2, 1f) && character.QueryManager.RayCast(ray, character.Body.Height, out var earliestHit) && character.Body.Height - maximumStepHeight < earliestHit.T)
				{
					((Vector3)(ref earliestHit.Normal)).Normalize();
					float value = default(float);
					Vector3.Dot(ref earliestHit.Normal, ref down, ref value);
					if (Math.Abs(value) > character.SupportFinder.cosMaximumSlope)
					{
						Vector3 normal4 = supportContact.Normal;
						Matrix3X3 orientationMatrix7 = ((Entity)character.Body).OrientationMatrix;
						hintOffset = Math.Min(0f, Vector3.Dot(normal4, ((Matrix3X3)(ref orientationMatrix7)).Down) * (CollisionDetectionSettings.AllowedPenetration * 0.5f - supportContact.PenetrationDepth));
						ray.Position = position;
						if (character.QueryManager.RayCast(ray, character.Body.Height * 0.5f + maximumStepHeight, out earliestHit))
						{
							((Vector3)(ref earliestHit.Normal)).Normalize();
							Vector3.Dot(ref earliestHit.Normal, ref down, ref value);
							if (Math.Abs(value) > character.SupportFinder.cosMaximumSlope)
							{
								return PositionState.Accepted;
							}
						}
					}
				}
				return PositionState.Rejected;
			}
			case PositionState.TooDeep:
			{
				Vector3 normal2 = supportContact.Normal;
				Matrix3X3 orientationMatrix3 = ((Entity)character.Body).OrientationMatrix;
				hintOffset = Math.Min(0f, Vector3.Dot(normal2, ((Matrix3X3)(ref orientationMatrix3)).Down) * (CollisionDetectionSettings.AllowedPenetration * 0.5f - supportContact.PenetrationDepth));
				return PositionState.TooDeep;
			}
			default:
			{
				Vector3 normal = supportContact.Normal;
				Matrix3X3 orientationMatrix2 = ((Entity)character.Body).OrientationMatrix;
				hintOffset = -0.001f - Vector3.Dot(normal, ((Matrix3X3)(ref orientationMatrix2)).Down) * supportContact.PenetrationDepth;
				return PositionState.NoHit;
			}
			}
		}
		if (flag)
		{
			return PositionState.Obstructed;
		}
		return PositionState.NoHit;
	}

	private bool IsUpStepObstructed(ref Vector3 sideNormal, RawList<ContactData> sideContacts, RawList<ContactData> headContacts)
	{
		for (int i = 0; i < sideContacts.Count; i++)
		{
			if (IsObstructiveToUpStepping(ref sideNormal, ref sideContacts.Elements[i]))
			{
				return true;
			}
		}
		return false;
	}

	private bool IsObstructiveToUpStepping(ref Vector3 sideNormal, ref ContactData contact)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		float num = default(float);
		Vector3.Dot(ref contact.Normal, ref sideNormal, ref num);
		if (num * contact.PenetrationDepth > CollisionDetectionSettings.AllowedPenetration)
		{
			return true;
		}
		Enumerator<OtherContact> enumerator = character.SupportFinder.SideContacts.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				OtherContact current = enumerator.Current;
				num = Vector3.Dot(contact.Normal, current.Contact.Normal);
				float num2 = num * current.Contact.PenetrationDepth;
				if (num2 > Math.Max(current.Contact.PenetrationDepth, CollisionDetectionSettings.AllowedPenetration))
				{
					return true;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		return false;
	}
}
