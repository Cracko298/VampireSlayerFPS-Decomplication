using System;
using System.Collections.Generic;
using System.Threading;
using BEPUphysics;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.BroadPhaseSystems;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionTests;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.Materials;
using BEPUphysics.MathExtensions;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using BEPUphysics.PositionUpdating;
using BEPUphysics.UpdateableSystems;
using Microsoft.Xna.Framework;
using VampireFPS;

namespace BEPUphysicsDemos.AlternateMovement.Character;

public class CharacterController : Updateable, IBeforeSolverUpdateable, ISpaceUpdateable, ISpaceObject
{
	private class Comparer : IComparer<ICharacterTag>
	{
		public int Compare(ICharacterTag x, ICharacterTag y)
		{
			if (x.InstanceId < y.InstanceId)
			{
				return -1;
			}
			if (x.InstanceId > y.InstanceId)
			{
				return 1;
			}
			return 0;
		}
	}

	private float jumpSpeed = 4.5f;

	private float leapUpSpeed = 10f;

	private float leapFwdSpeed = 25f;

	private float slidingJumpSpeed = 3f;

	private float jumpForceFactor = 1f;

	private List<ICharacterTag> involvedCharacters = new List<ICharacterTag>();

	private static Comparer comparer = new Comparer();

	private SupportData supportData;

	private bool tryToJump;

	private bool tryToLeap;

	public Cylinder Body { get; private set; }

	public StepManager StepManager { get; private set; }

	public StanceManager StanceManager { get; private set; }

	public QueryManager QueryManager { get; private set; }

	public HorizontalMotionConstraint HorizontalMotionConstraint { get; private set; }

	public VerticalMotionConstraint VerticalMotionConstraint { get; private set; }

	public float JumpSpeed
	{
		get
		{
			return jumpSpeed;
		}
		set
		{
			if (value < 0f)
			{
				throw new Exception("Value must be nonnegative.");
			}
			jumpSpeed = value;
		}
	}

	public float SlidingJumpSpeed
	{
		get
		{
			return slidingJumpSpeed;
		}
		set
		{
			if (value < 0f)
			{
				throw new Exception("Value must be nonnegative.");
			}
			slidingJumpSpeed = value;
		}
	}

	public float JumpForceFactor
	{
		get
		{
			return jumpForceFactor;
		}
		set
		{
			if (value < 0f)
			{
				throw new Exception("Value must be nonnegative.");
			}
			jumpForceFactor = value;
		}
	}

	public float BodyRadius
	{
		get
		{
			return ((Entity<ConvexCollidable<CylinderShape>>)(object)Body).CollisionInformation.Shape.Radius;
		}
		set
		{
			if (value <= 0f)
			{
				throw new Exception("Radius must be positive.");
			}
			((Entity<ConvexCollidable<CylinderShape>>)(object)Body).CollisionInformation.Shape.Radius = value;
			QueryManager.UpdateQueryShapes();
		}
	}

	public SupportFinder SupportFinder { get; private set; }

	public CharacterController()
		: this(default(Vector3), 1.7f, 1.19f, 0.6f, 10f)
	{
	}//IL_0003: Unknown result type (might be due to invalid IL or missing references)
	//IL_0009: Unknown result type (might be due to invalid IL or missing references)


	public CharacterController(Vector3 position, float height, float crouchingHeight, float radius, float mass)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		Body = new Cylinder(position, height, radius, mass);
		((Entity)Body).IgnoreShapeChanges = true;
		((ConvexShape)((Entity<ConvexCollidable<CylinderShape>>)(object)Body).CollisionInformation.Shape).CollisionMargin = 0.1f;
		((Entity)Body).PositionUpdateMode = (PositionUpdateMode)2;
		((Entity)Body).LocalInertiaTensorInverse = default(Matrix3X3);
		((EntityCollidable)((Entity<ConvexCollidable<CylinderShape>>)(object)Body).CollisionInformation).Events.DetectingInitialCollision += RemoveFriction;
		((Entity)Body).LinearDamping = 0f;
		SupportFinder = new SupportFinder(this);
		HorizontalMotionConstraint = new HorizontalMotionConstraint(this);
		VerticalMotionConstraint = new VerticalMotionConstraint(this);
		StepManager = new StepManager(this);
		StanceManager = new StanceManager(this, crouchingHeight);
		QueryManager = new QueryManager(this);
		((Updateable)this).IsUpdatedSequentially = false;
		((BroadPhaseEntry)((Entity<ConvexCollidable<CylinderShape>>)(object)Body).CollisionInformation).Tag = new CharacterSynchronizer((Entity)(object)Body);
	}

	public void LockCharacterPairs()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		Enumerator<CollidablePairHandler> enumerator = ((Collidable)((Entity<ConvexCollidable<CylinderShape>>)(object)Body).CollisionInformation).Pairs.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				CollidablePairHandler current = enumerator.Current;
				BroadPhaseOverlap broadPhaseOverlap = ((NarrowPhasePair)current).BroadPhaseOverlap;
				BroadPhaseEntry obj;
				if (((BroadPhaseOverlap)(ref broadPhaseOverlap)).EntryA != ((Entity<ConvexCollidable<CylinderShape>>)(object)Body).CollisionInformation)
				{
					BroadPhaseOverlap broadPhaseOverlap2 = ((NarrowPhasePair)current).BroadPhaseOverlap;
					obj = ((BroadPhaseOverlap)(ref broadPhaseOverlap2)).EntryA;
				}
				else
				{
					BroadPhaseOverlap broadPhaseOverlap3 = ((NarrowPhasePair)current).BroadPhaseOverlap;
					obj = ((BroadPhaseOverlap)(ref broadPhaseOverlap3)).EntryB;
				}
				BroadPhaseEntry val = obj;
				if (val.Tag is ICharacterTag item)
				{
					involvedCharacters.Add(item);
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		if (involvedCharacters.Count > 0)
		{
			involvedCharacters.Add((ICharacterTag)((BroadPhaseEntry)((Entity<ConvexCollidable<CylinderShape>>)(object)Body).CollisionInformation).Tag);
			involvedCharacters.Sort(comparer);
			for (int i = 0; i < involvedCharacters.Count; i++)
			{
				Monitor.Enter(involvedCharacters[i]);
			}
		}
	}

	public void UnlockCharacterPairs()
	{
		for (int num = involvedCharacters.Count - 1; num >= 0; num--)
		{
			Monitor.Exit(involvedCharacters[num]);
		}
		involvedCharacters.Clear();
	}

	private void RemoveFriction(EntityCollidable sender, BroadPhaseEntry other, NarrowPhasePair pair)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		CollidablePairHandler val = (CollidablePairHandler)(object)((pair is CollidablePairHandler) ? pair : null);
		if (val != null)
		{
			val.UpdateMaterialProperties(default(InteractionProperties));
		}
	}

	private void ExpandBoundingBox()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		if (((Entity)Body).ActivityInformation.IsActive)
		{
			float num = ((ConvexShape)((Entity<ConvexCollidable<CylinderShape>>)(object)Body).CollisionInformation.Shape).CollisionMargin * 1.1f;
			Vector3 val = default(Vector3);
			val.X = num;
			val.Y = StepManager.MaximumStepHeight;
			val.Z = num;
			BoundingBox boundingBox = ((BroadPhaseEntry)((Entity<ConvexCollidable<CylinderShape>>)(object)Body).CollisionInformation).BoundingBox;
			Vector3.Add(ref boundingBox.Max, ref val, ref boundingBox.Max);
			Vector3.Subtract(ref boundingBox.Min, ref val, ref boundingBox.Min);
			((BroadPhaseEntry)((Entity<ConvexCollidable<CylinderShape>>)(object)Body).CollisionInformation).BoundingBox = boundingBox;
		}
	}

	private void CollectSupportData()
	{
		SupportFinder.UpdateSupports();
		if (SupportFinder.HasSupport)
		{
			if (SupportFinder.HasTraction)
			{
				supportData = SupportFinder.TractionData.Value;
			}
			else
			{
				supportData = SupportFinder.SupportData.Value;
			}
		}
		else
		{
			supportData = default(SupportData);
		}
	}

	void IBeforeSolverUpdateable.Update(float dt)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		LockCharacterPairs();
		try
		{
			CorrectContacts();
			bool hasTraction = SupportFinder.HasTraction;
			CollectSupportData();
			ComputeRelativeVelocity(ref this.supportData, out var relativeVelocity);
			float num = Vector3.Dot(this.supportData.Normal, relativeVelocity);
			if (SupportFinder.HasTraction && !hasTraction && num < 0f)
			{
				SupportFinder.ClearSupportData();
				this.supportData = default(SupportData);
			}
			if (tryToJump && StanceManager.CurrentStance != Stance.Crouching)
			{
				if (SupportFinder.HasTraction)
				{
					Matrix3X3 orientationMatrix = ((Entity)Body).OrientationMatrix;
					float num2 = Vector3.Dot(((Matrix3X3)(ref orientationMatrix)).Up, relativeVelocity);
					float num3 = Math.Max(jumpSpeed - num2, 0f);
					ref SupportData reference = ref this.supportData;
					Matrix3X3 orientationMatrix2 = ((Entity)Body).OrientationMatrix;
					ApplyJumpVelocity(ref reference, ((Matrix3X3)(ref orientationMatrix2)).Up * num3, ref relativeVelocity);
					Enumerator<CollidablePairHandler> enumerator = ((Collidable)((Entity<ConvexCollidable<CylinderShape>>)(object)Body).CollisionInformation).Pairs.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							CollidablePairHandler current = enumerator.Current;
							current.ClearContacts();
						}
					}
					finally
					{
						((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
					}
					SupportFinder.ClearSupportData();
					this.supportData = default(SupportData);
					g.m_PlayerManager.GetLocalPlayer().PlayJumpSFX();
				}
				else if (SupportFinder.HasSupport)
				{
					float num4 = Vector3.Dot(this.supportData.Normal, relativeVelocity);
					float num5 = Math.Max(slidingJumpSpeed - num4, 0f);
					ApplyJumpVelocity(ref this.supportData, this.supportData.Normal * (0f - num5), ref relativeVelocity);
					Enumerator<CollidablePairHandler> enumerator2 = ((Collidable)((Entity<ConvexCollidable<CylinderShape>>)(object)Body).CollisionInformation).Pairs.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							CollidablePairHandler current2 = enumerator2.Current;
							current2.ClearContacts();
						}
					}
					finally
					{
						((IDisposable)enumerator2/*cast due to .constrained prefix*/).Dispose();
					}
					SupportFinder.ClearSupportData();
					this.supportData = default(SupportData);
					g.m_PlayerManager.GetLocalPlayer().PlayJumpSFX();
				}
			}
			tryToJump = false;
			if (tryToLeap)
			{
				if (SupportFinder.HasTraction)
				{
					Cylinder body = Body;
					Matrix3X3 orientationMatrix3 = ((Entity)Body).OrientationMatrix;
					Vector3 val = ((Matrix3X3)(ref orientationMatrix3)).Up * leapUpSpeed;
					Matrix3X3 orientationMatrix4 = ((Entity)Body).OrientationMatrix;
					((Entity)body).LinearVelocity = val + ((Matrix3X3)(ref orientationMatrix4)).Forward * leapFwdSpeed;
					relativeVelocity = ((Entity)Body).LinearVelocity;
					Enumerator<CollidablePairHandler> enumerator3 = ((Collidable)((Entity<ConvexCollidable<CylinderShape>>)(object)Body).CollisionInformation).Pairs.GetEnumerator();
					try
					{
						while (enumerator3.MoveNext())
						{
							CollidablePairHandler current3 = enumerator3.Current;
							current3.ClearContacts();
						}
					}
					finally
					{
						((IDisposable)enumerator3/*cast due to .constrained prefix*/).Dispose();
					}
					SupportFinder.ClearSupportData();
					this.supportData = default(SupportData);
					g.m_PlayerManager.GetLocalPlayer().PlayLeapSFX();
				}
				else if (SupportFinder.HasSupport)
				{
					Cylinder body2 = Body;
					Matrix3X3 orientationMatrix5 = ((Entity)Body).OrientationMatrix;
					Vector3 val2 = ((Matrix3X3)(ref orientationMatrix5)).Up * leapUpSpeed;
					Matrix3X3 orientationMatrix6 = ((Entity)Body).OrientationMatrix;
					((Entity)body2).LinearVelocity = val2 + ((Matrix3X3)(ref orientationMatrix6)).Forward * leapFwdSpeed;
					relativeVelocity = ((Entity)Body).LinearVelocity;
					Enumerator<CollidablePairHandler> enumerator4 = ((Collidable)((Entity<ConvexCollidable<CylinderShape>>)(object)Body).CollisionInformation).Pairs.GetEnumerator();
					try
					{
						while (enumerator4.MoveNext())
						{
							CollidablePairHandler current4 = enumerator4.Current;
							current4.ClearContacts();
						}
					}
					finally
					{
						((IDisposable)enumerator4/*cast due to .constrained prefix*/).Dispose();
					}
					SupportFinder.ClearSupportData();
					this.supportData = default(SupportData);
					g.m_PlayerManager.GetLocalPlayer().PlayLeapSFX();
				}
			}
			tryToLeap = false;
			if (StepManager.TryToStepDown(out var newPosition) || StepManager.TryToStepUp(out newPosition))
			{
				TeleportToPosition(newPosition, dt);
			}
			if (StanceManager.UpdateStance(out newPosition))
			{
				TeleportToPosition(newPosition, dt);
			}
		}
		finally
		{
			UnlockCharacterPairs();
		}
		Vector3 movementDirection = new Vector3(HorizontalMotionConstraint.MovementDirection.X, 0f, HorizontalMotionConstraint.MovementDirection.Y);
		SupportFinder.GetTractionInDirection(ref movementDirection, out var supportData);
		bool flag = HorizontalMotionConstraint.SupportData.SupportObject != this.supportData.SupportObject || VerticalMotionConstraint.SupportData.SupportObject != supportData.SupportObject;
		if (flag)
		{
			CharacterSynchronizer.ConstraintAccessLocker.Enter();
		}
		HorizontalMotionConstraint.SupportData = this.supportData;
		VerticalMotionConstraint.SupportData = supportData;
		if (flag)
		{
			CharacterSynchronizer.ConstraintAccessLocker.Exit();
		}
	}

	private void TeleportToPosition(Vector3 newPosition, float dt)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		((Entity)Body).Position = newPosition;
		Quaternion orientation = ((Entity)Body).Orientation;
		((EntityCollidable)((Entity<ConvexCollidable<CylinderShape>>)(object)Body).CollisionInformation).UpdateWorldTransform(ref newPosition, ref orientation);
		Enumerator<CollidablePairHandler> enumerator = ((Collidable)((Entity<ConvexCollidable<CylinderShape>>)(object)Body).CollisionInformation).Pairs.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				CollidablePairHandler current = enumerator.Current;
				current.ClearContacts();
				((NarrowPhasePair)current).UpdateCollision(dt);
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		CollectSupportData();
	}

	private void CorrectContacts()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		Matrix3X3 orientationMatrix = ((Entity)Body).OrientationMatrix;
		Vector3 down = ((Matrix3X3)(ref orientationMatrix)).Down;
		Vector3 position = ((Entity)Body).Position;
		float collisionMargin = ((ConvexShape)((Entity<ConvexCollidable<CylinderShape>>)(object)Body).CollisionInformation.Shape).CollisionMargin;
		float num = Body.Height * 0.5f - collisionMargin;
		float num2 = Body.Radius - collisionMargin;
		float num3 = num2 * num2;
		Enumerator<CollidablePairHandler> enumerator = ((Collidable)((Entity<ConvexCollidable<CylinderShape>>)(object)Body).CollisionInformation).Pairs.GetEnumerator();
		try
		{
			float num4 = default(float);
			Vector3 val2 = default(Vector3);
			Vector3 val3 = default(Vector3);
			Vector3 normal = default(Vector3);
			float value = default(float);
			while (enumerator.MoveNext())
			{
				CollidablePairHandler current = enumerator.Current;
				Enumerator enumerator2 = current.Contacts.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						Contact contact = ((Enumerator)(ref enumerator2)).Current.Contact;
						Vector3 val = contact.Position - ((Entity)Body).Position;
						Vector3.Dot(ref val, ref down, ref num4);
						if (!(num4 > num))
						{
							continue;
						}
						Vector3.Dot(ref val, ref down, ref num4);
						Vector3.Multiply(ref down, num4, ref val2);
						Vector3.Subtract(ref val, ref val2, ref val2);
						float num5 = ((Vector3)(ref val2)).LengthSquared();
						if (num5 > num3)
						{
							Vector3.Multiply(ref val2, num2 / (float)Math.Sqrt(num5), ref val2);
						}
						Vector3.Multiply(ref down, num, ref val3);
						Vector3.Add(ref val3, ref val2, ref val3);
						Vector3.Add(ref val3, ref position, ref val3);
						Vector3.Subtract(ref contact.Position, ref val3, ref normal);
						num5 = ((Vector3)(ref normal)).LengthSquared();
						if (!(num5 > 1E-07f))
						{
							continue;
						}
						Vector3.Divide(ref normal, (float)Math.Sqrt(num5), ref normal);
						Vector3.Dot(ref normal, ref down, ref num4);
						Vector3.Dot(ref contact.Normal, ref down, ref value);
						if (Math.Abs(num4) > Math.Abs(value))
						{
							Vector3.Dot(ref normal, ref contact.Normal, ref num4);
							if (num4 < 0f)
							{
								Vector3.Negate(ref normal, ref normal);
								num4 = 0f - num4;
							}
							contact.PenetrationDepth *= num4;
							contact.Normal = normal;
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
	}

	private void ComputeRelativeVelocity(ref SupportData supportData, out Vector3 relativeVelocity)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		relativeVelocity = ((Entity)Body).LinearVelocity;
		if (!SupportFinder.HasSupport)
		{
			return;
		}
		Collidable supportObject = supportData.SupportObject;
		EntityCollidable val = (EntityCollidable)(object)((supportObject is EntityCollidable) ? supportObject : null);
		if (val == null)
		{
			return;
		}
		bool isDynamic;
		if (isDynamic = val.Entity.IsDynamic)
		{
			val.Entity.Locker.Enter();
		}
		Vector3 velocityOfPoint;
		try
		{
			velocityOfPoint = Toolbox.GetVelocityOfPoint(supportData.Position, val.Entity);
		}
		finally
		{
			if (isDynamic)
			{
				val.Entity.Locker.Exit();
			}
		}
		Vector3.Subtract(ref relativeVelocity, ref velocityOfPoint, ref relativeVelocity);
	}

	private void ApplyJumpVelocity(ref SupportData supportData, Vector3 velocityChange, ref Vector3 relativeVelocity)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		Cylinder body = Body;
		((Entity)body).LinearVelocity = ((Entity)body).LinearVelocity + velocityChange;
		Collidable supportObject = supportData.SupportObject;
		EntityCollidable val = (EntityCollidable)(object)((supportObject is EntityCollidable) ? supportObject : null);
		if (val != null && val.Entity.IsDynamic)
		{
			Vector3 val2 = velocityChange * jumpForceFactor;
			val.Entity.Locker.Enter();
			try
			{
				Entity entity = val.Entity;
				entity.LinearMomentum += val2 * (0f - ((Entity)Body).Mass);
			}
			finally
			{
				val.Entity.Locker.Exit();
			}
			velocityChange += val2;
		}
		Vector3.Add(ref relativeVelocity, ref velocityChange, ref relativeVelocity);
	}

	private void ChangeVelocityUnilaterally(Vector3 velocityChange, ref Vector3 relativeVelocity)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Cylinder body = Body;
		((Entity)body).LinearVelocity = ((Entity)body).LinearVelocity + velocityChange;
		Vector3.Add(ref relativeVelocity, ref velocityChange, ref relativeVelocity);
	}

	public void Jump()
	{
		tryToJump = true;
	}

	public void Leap()
	{
		tryToLeap = true;
	}

	public override void OnAdditionToSpace(ISpace newSpace)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		newSpace.Add((ISpaceObject)(object)Body);
		newSpace.Add((ISpaceObject)(object)HorizontalMotionConstraint);
		newSpace.Add((ISpaceObject)(object)VerticalMotionConstraint);
		((MultithreadedProcessingStage)((Space)newSpace).BoundingBoxUpdater).Finishing += ExpandBoundingBox;
		((Entity)Body).AngularVelocity = default(Vector3);
		((Entity)Body).LinearVelocity = default(Vector3);
	}

	public override void OnRemovalFromSpace(ISpace oldSpace)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		oldSpace.Remove((ISpaceObject)(object)Body);
		oldSpace.Remove((ISpaceObject)(object)HorizontalMotionConstraint);
		oldSpace.Remove((ISpaceObject)(object)VerticalMotionConstraint);
		((MultithreadedProcessingStage)((Space)oldSpace).BoundingBoxUpdater).Finishing -= ExpandBoundingBox;
		SupportFinder.ClearSupportData();
		((Entity)Body).AngularVelocity = default(Vector3);
		((Entity)Body).LinearVelocity = default(Vector3);
	}
}
