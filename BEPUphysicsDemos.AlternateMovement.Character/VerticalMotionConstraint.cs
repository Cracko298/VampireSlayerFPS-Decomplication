using System;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.Constraints;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using BEPUphysics.Settings;
using BEPUphysics.SolverSystems;
using Microsoft.Xna.Framework;

namespace BEPUphysicsDemos.AlternateMovement.Character;

public class VerticalMotionConstraint : EntitySolverUpdateable
{
	private CharacterController character;

	private SupportData supportData;

	private float maximumGlueForce = 5000f;

	private float maximumForce;

	private float supportForceFactor = 1f;

	private float effectiveMass;

	private Entity supportEntity;

	private Vector3 linearJacobianA;

	private Vector3 linearJacobianB;

	private Vector3 angularJacobianB;

	private float accumulatedImpulse;

	private float permittedVelocity;

	public SupportData SupportData
	{
		get
		{
			return supportData;
		}
		set
		{
			Collidable supportObject = supportData.SupportObject;
			supportData = value;
			if (supportObject != supportData.SupportObject)
			{
				((EntitySolverUpdateable)this).OnInvolvedEntitiesChanged();
			}
		}
	}

	public float MaximumGlueForce
	{
		get
		{
			return maximumGlueForce;
		}
		set
		{
			if (maximumGlueForce < 0f)
			{
				throw new Exception("Value must be nonnegative.");
			}
			maximumGlueForce = value;
		}
	}

	public float SupportForceFactor
	{
		get
		{
			return supportForceFactor;
		}
		set
		{
			if (value < 0f)
			{
				throw new Exception("Value must be nonnegative.");
			}
			supportForceFactor = value;
		}
	}

	public float EffectiveMass => effectiveMass;

	public float RelativeVelocity
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			Vector3 linearVelocity = ((Entity)character.Body).LinearVelocity;
			float num = default(float);
			Vector3.Dot(ref linearJacobianA, ref linearVelocity, ref num);
			if (supportEntity != null)
			{
				Vector3 linearVelocity2 = supportEntity.LinearVelocity;
				Vector3 angularVelocity = supportEntity.AngularVelocity;
				float num2 = default(float);
				Vector3.Dot(ref linearJacobianB, ref linearVelocity2, ref num2);
				num += num2;
				Vector3.Dot(ref angularJacobianB, ref angularVelocity, ref num2);
				return num + num2;
			}
			return num;
		}
	}

	public VerticalMotionConstraint(CharacterController characterController)
	{
		character = characterController;
	}

	protected override void CollectInvolvedEntities(RawList<Entity> outputInvolvedEntities)
	{
		Collidable supportObject = supportData.SupportObject;
		EntityCollidable val = (EntityCollidable)(object)((supportObject is EntityCollidable) ? supportObject : null);
		if (val != null)
		{
			outputInvolvedEntities.Add(val.Entity);
		}
		outputInvolvedEntities.Add((Entity)(object)character.Body);
	}

	public override void UpdateSolverActivity()
	{
		if (supportData.HasTraction)
		{
			((SolverUpdateable)this).UpdateSolverActivity();
		}
		else
		{
			((SolverUpdateable)this).isActiveInSolver = false;
		}
	}

	public override void Update(float dt)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		if (supportData.SupportObject != null)
		{
			Collidable supportObject = supportData.SupportObject;
			EntityCollidable val = (EntityCollidable)(object)((supportObject is EntityCollidable) ? supportObject : null);
			if (val != null)
			{
				supportEntity = val.Entity;
			}
			else
			{
				supportEntity = null;
			}
		}
		else
		{
			supportEntity = null;
		}
		maximumForce = maximumGlueForce * dt;
		if (supportData.Depth > 0f)
		{
			permittedVelocity = CollisionResponseSettings.MaximumPenetrationCorrectionSpeed;
		}
		else
		{
			permittedVelocity = 0f;
		}
		linearJacobianA = supportData.Normal;
		Vector3.Negate(ref linearJacobianA, ref linearJacobianB);
		effectiveMass = ((Entity)character.Body).InverseMass;
		if (supportEntity != null)
		{
			Vector3 val2 = supportData.Position - supportEntity.Position;
			Vector3.Cross(ref val2, ref linearJacobianB, ref angularJacobianB);
			if (supportEntity.IsDynamic)
			{
				Matrix3X3 inertiaTensorInverse = supportEntity.InertiaTensorInverse;
				Vector3 val3 = default(Vector3);
				Matrix3X3.Transform(ref angularJacobianB, ref inertiaTensorInverse, ref val3);
				float num = default(float);
				Vector3.Dot(ref val3, ref angularJacobianB, ref num);
				effectiveMass += supportForceFactor * (num + supportEntity.InverseMass);
			}
		}
		effectiveMass = 1f / effectiveMass;
	}

	public override void ExclusiveUpdate()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		Vector3 val2 = default(Vector3);
		Vector3.Multiply(ref linearJacobianA, accumulatedImpulse, ref val);
		((Entity)character.Body).ApplyLinearImpulse(ref val);
		if (supportEntity != null && supportEntity.IsDynamic)
		{
			Vector3.Multiply(ref val, 0f - supportForceFactor, ref val);
			Vector3.Multiply(ref angularJacobianB, accumulatedImpulse * supportForceFactor, ref val2);
			supportEntity.ApplyLinearImpulse(ref val);
			supportEntity.ApplyAngularImpulse(ref val2);
		}
	}

	public override float SolveIteration()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		float num = RelativeVelocity + permittedVelocity;
		float num2 = (0f - num) * effectiveMass;
		float num3 = accumulatedImpulse;
		accumulatedImpulse = MathHelper.Clamp(accumulatedImpulse + num2, 0f, maximumForce);
		num2 = accumulatedImpulse - num3;
		Vector3 val = default(Vector3);
		Vector3 val2 = default(Vector3);
		Vector3.Multiply(ref linearJacobianA, num2, ref val);
		((Entity)character.Body).ApplyLinearImpulse(ref val);
		if (supportEntity != null && supportEntity.IsDynamic)
		{
			Vector3.Multiply(ref val, 0f - supportForceFactor, ref val);
			Vector3.Multiply(ref angularJacobianB, num2 * supportForceFactor, ref val2);
			supportEntity.ApplyLinearImpulse(ref val);
			supportEntity.ApplyAngularImpulse(ref val2);
		}
		return Math.Abs(num2);
	}
}
