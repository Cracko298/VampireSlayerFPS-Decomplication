using System;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionTests;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using BEPUphysics.Settings;
using Microsoft.Xna.Framework;

namespace BEPUphysicsDemos.AlternateMovement.Character;

public class StanceManager
{
	private float standingHeight;

	private float crouchingHeight;

	private CharacterController character;

	public float StandingHeight
	{
		get
		{
			return standingHeight;
		}
		set
		{
			if (value <= 0f || value < CrouchingHeight)
			{
				throw new Exception("Standing height must be positive and greater than the crouching height.");
			}
			standingHeight = value;
			character.QueryManager.UpdateQueryShapes();
			if (CurrentStance == Stance.Standing)
			{
				((Entity<ConvexCollidable<CylinderShape>>)(object)character.Body).CollisionInformation.Shape.Height = standingHeight;
			}
		}
	}

	public float CrouchingHeight
	{
		get
		{
			return crouchingHeight;
		}
		set
		{
			if (value <= 0f || value > StandingHeight)
			{
				throw new Exception("Crouching height must be positive and less than the standing height.");
			}
			crouchingHeight = value;
			character.QueryManager.UpdateQueryShapes();
			if (CurrentStance == Stance.Crouching)
			{
				((Entity<ConvexCollidable<CylinderShape>>)(object)character.Body).CollisionInformation.Shape.Height = crouchingHeight;
			}
		}
	}

	public Stance CurrentStance { get; private set; }

	public Stance DesiredStance { get; set; }

	public StanceManager(CharacterController character, float crouchingHeight)
	{
		this.character = character;
		standingHeight = character.Body.Height;
		if (crouchingHeight < standingHeight)
		{
			this.crouchingHeight = StandingHeight * 0.7f;
			return;
		}
		throw new Exception("Crouching height must be less than standing height.");
	}

	public bool UpdateStance(out Vector3 newPosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		newPosition = default(Vector3);
		if (CurrentStance != DesiredStance)
		{
			if (CurrentStance == Stance.Standing && DesiredStance == Stance.Crouching)
			{
				if (character.SupportFinder.HasSupport)
				{
					Vector3 position = ((Entity)character.Body).Position;
					Matrix3X3 orientationMatrix = ((Entity)character.Body).OrientationMatrix;
					newPosition = position + ((Matrix3X3)(ref orientationMatrix)).Down * ((StandingHeight - CrouchingHeight) * 0.5f);
					character.Body.Height = CrouchingHeight;
					CurrentStance = Stance.Crouching;
				}
				else
				{
					newPosition = ((Entity)character.Body).Position;
					character.Body.Height = CrouchingHeight;
					CurrentStance = Stance.Crouching;
				}
				return true;
			}
			if (CurrentStance == Stance.Crouching && DesiredStance == Stance.Standing)
			{
				if (character.SupportFinder.HasSupport)
				{
					Vector3 position2 = ((Entity)character.Body).Position;
					Matrix3X3 orientationMatrix2 = ((Entity)character.Body).OrientationMatrix;
					newPosition = position2 - ((Matrix3X3)(ref orientationMatrix2)).Down * ((StandingHeight - CrouchingHeight) * 0.5f);
					character.QueryManager.QueryContacts(newPosition, Stance.Standing);
					if (IsObstructed(character.QueryManager.SideContacts, character.QueryManager.HeadContacts))
					{
						return false;
					}
					character.Body.Height = StandingHeight;
					CurrentStance = Stance.Standing;
					return true;
				}
				float num = 0f;
				float num2 = (StandingHeight - CrouchingHeight) * 0.5f;
				float num3 = num2;
				float num4 = num2;
				int num5 = 0;
				Matrix3X3 orientationMatrix3 = ((Entity)character.Body).OrientationMatrix;
				Vector3 down = ((Matrix3X3)(ref orientationMatrix3)).Down;
				while (num5++ < 5 && num2 - num > 1E-05f)
				{
					Vector3 position3 = ((Entity)character.Body).Position + num3 * down;
					float hintOffset;
					switch (TrySupportLocation(ref position3, out hintOffset))
					{
					case PositionState.Accepted:
						num3 += hintOffset;
						if (num3 > 0f && num3 < num4)
						{
							newPosition = ((Entity)character.Body).Position + num3 * down;
							character.Body.Height = StandingHeight;
							CurrentStance = Stance.Standing;
							return true;
						}
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
				newPosition = ((Entity)character.Body).Position;
				character.Body.Height = StandingHeight;
				CurrentStance = Stance.Standing;
				return true;
			}
		}
		return false;
	}

	private bool IsObstructed(RawList<ContactData> sideContacts, RawList<ContactData> headContacts)
	{
		if (headContacts.Count > 0)
		{
			return true;
		}
		for (int i = 0; i < sideContacts.Count; i++)
		{
			if (IsObstructive(ref sideContacts.Elements[i]))
			{
				return true;
			}
		}
		return false;
	}

	private bool IsObstructive(ref ContactData contact)
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

	private PositionState TrySupportLocation(ref Vector3 position, out float hintOffset)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		hintOffset = 0f;
		character.QueryManager.QueryContacts(position, Stance.Standing);
		bool flag = IsObstructed(character.QueryManager.SideContacts, character.QueryManager.HeadContacts);
		if (character.QueryManager.HasSupports(out var _, out var state, out var supportContact) && !flag)
		{
			switch (state)
			{
			case PositionState.Accepted:
			{
				Vector3 normal3 = supportContact.Normal;
				Matrix3X3 orientationMatrix3 = ((Entity)character.Body).OrientationMatrix;
				hintOffset = Math.Min(0f, Vector3.Dot(normal3, ((Matrix3X3)(ref orientationMatrix3)).Down) * (CollisionDetectionSettings.AllowedPenetration * 0.5f - supportContact.PenetrationDepth));
				return PositionState.Accepted;
			}
			case PositionState.TooDeep:
			{
				Vector3 normal2 = supportContact.Normal;
				Matrix3X3 orientationMatrix2 = ((Entity)character.Body).OrientationMatrix;
				hintOffset = Math.Min(0f, Vector3.Dot(normal2, ((Matrix3X3)(ref orientationMatrix2)).Down) * (CollisionDetectionSettings.AllowedPenetration * 0.5f - supportContact.PenetrationDepth));
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
}
