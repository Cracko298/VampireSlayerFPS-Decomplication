using System;
using BEPUphysics;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.Constraints;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysicsDemos.AlternateMovement.Character;

public class HorizontalMotionConstraint : EntitySolverUpdateable
{
	private CharacterController character;

	private SupportData supportData;

	private Vector2 movementDirection;

	private float speed = 8f;

	private float crouchingSpeed = 3f;

	private float slidingSpeed = 6f;

	private float airSpeed = 8f;

	private float maximumForce = 1000f;

	private float maximumSlidingForce = 50f;

	private float maximumAirForce = 250f;

	private float supportForceFactor = 1f;

	private float maxSpeed;

	private float maxForce;

	private Matrix2X2 massMatrix;

	private Entity supportEntity;

	private Vector3 linearJacobianA1;

	private Vector3 linearJacobianA2;

	private Vector3 linearJacobianB1;

	private Vector3 linearJacobianB2;

	private Vector3 angularJacobianB1;

	private Vector3 angularJacobianB2;

	private Vector2 accumulatedImpulse;

	private Vector2 targetVelocity;

	private Vector2 positionCorrectionBias;

	private Vector3 positionLocalOffset;

	private bool wasTryingToMove;

	private bool hadTraction;

	private Entity previousSupportEntity;

	private float timeSinceTransition;

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
				Collidable supportObject2 = supportData.SupportObject;
				EntityCollidable val = (EntityCollidable)(object)((supportObject2 is EntityCollidable) ? supportObject2 : null);
				if (val != null)
				{
					supportEntity = val.Entity;
				}
				else
				{
					supportEntity = null;
				}
			}
		}
	}

	public Vector2 MovementDirection
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return movementDirection;
		}
		set
		{
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			float num = ((Vector2)(ref value)).LengthSquared();
			if (num > 1E-07f)
			{
				((Entity)character.Body).ActivityInformation.Activate();
				Vector2.Divide(ref value, (float)Math.Sqrt(num), ref movementDirection);
			}
			else
			{
				((Entity)character.Body).ActivityInformation.Activate();
				movementDirection = default(Vector2);
			}
		}
	}

	public float Speed
	{
		get
		{
			return speed;
		}
		set
		{
			if (value < 0f)
			{
				throw new Exception("Value must be nonnegative.");
			}
			speed = value;
		}
	}

	public float CrouchingSpeed
	{
		get
		{
			return crouchingSpeed;
		}
		set
		{
			if (value < 0f)
			{
				throw new Exception("Value must be nonnegative.");
			}
			crouchingSpeed = value;
		}
	}

	public float SlidingSpeed
	{
		get
		{
			return slidingSpeed;
		}
		set
		{
			if (value < 0f)
			{
				throw new Exception("Value must be nonnegative.");
			}
			slidingSpeed = value;
		}
	}

	public float AirSpeed
	{
		get
		{
			return airSpeed;
		}
		set
		{
			if (value < 0f)
			{
				throw new Exception("Value must be nonnegative.");
			}
			airSpeed = value;
		}
	}

	public float MaximumForce
	{
		get
		{
			return maximumForce;
		}
		set
		{
			if (value < 0f)
			{
				throw new Exception("Value must be nonnegative.");
			}
			maximumForce = value;
		}
	}

	public float MaximumSlidingForce
	{
		get
		{
			return maximumSlidingForce;
		}
		set
		{
			if (value < 0f)
			{
				throw new Exception("Value must be nonnegative.");
			}
			maximumSlidingForce = value;
		}
	}

	public float MaximumAirForce
	{
		get
		{
			return maximumAirForce;
		}
		set
		{
			if (value < 0f)
			{
				throw new Exception("Value must be nonnegative.");
			}
			maximumAirForce = value;
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

	public MovementMode MovementMode { get; private set; }

	public Vector2 RelativeVelocity
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			Vector2 result = default(Vector2);
			Vector3 linearVelocity = ((Entity)character.Body).LinearVelocity;
			Vector3.Dot(ref linearJacobianA1, ref linearVelocity, ref result.X);
			Vector3.Dot(ref linearJacobianA2, ref linearVelocity, ref result.Y);
			if (supportEntity != null)
			{
				Vector3 linearVelocity2 = supportEntity.LinearVelocity;
				Vector3 angularVelocity = supportEntity.AngularVelocity;
				float num = default(float);
				Vector3.Dot(ref linearJacobianB1, ref linearVelocity2, ref num);
				float num2 = default(float);
				Vector3.Dot(ref linearJacobianB2, ref linearVelocity2, ref num2);
				result.X += num;
				result.Y += num2;
				Vector3.Dot(ref angularJacobianB1, ref angularVelocity, ref num);
				Vector3.Dot(ref angularJacobianB2, ref angularVelocity, ref num2);
				result.X += num;
				result.Y += num2;
			}
			return result;
		}
	}

	public Vector3 RelativeWorldVelocity
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			Vector3 linearVelocity = ((Entity)character.Body).LinearVelocity;
			if (supportEntity != null)
			{
				return linearVelocity - Toolbox.GetVelocityOfPoint(supportData.Position, supportEntity);
			}
			return linearVelocity;
		}
	}

	public HorizontalMotionConstraint(CharacterController characterController)
	{
		character = characterController;
		((EntitySolverUpdateable)this).CollectInvolvedEntities();
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

	public override void Update(float dt)
	{
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_073e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_059b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0618: Unknown result type (might be due to invalid IL or missing references)
		//IL_061d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0622: Unknown result type (might be due to invalid IL or missing references)
		//IL_0625: Unknown result type (might be due to invalid IL or missing references)
		//IL_0630: Unknown result type (might be due to invalid IL or missing references)
		//IL_0635: Unknown result type (might be due to invalid IL or missing references)
		//IL_0640: Unknown result type (might be due to invalid IL or missing references)
		//IL_0645: Unknown result type (might be due to invalid IL or missing references)
		//IL_064a: Unknown result type (might be due to invalid IL or missing references)
		//IL_068a: Unknown result type (might be due to invalid IL or missing references)
		//IL_069a: Unknown result type (might be due to invalid IL or missing references)
		//IL_069f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06db: Unknown result type (might be due to invalid IL or missing references)
		bool flag = ((Vector2)(ref movementDirection)).LengthSquared() > 0f;
		if (supportData.SupportObject != null)
		{
			if (supportData.HasTraction)
			{
				MovementMode = MovementMode.Traction;
				if (character.StanceManager.CurrentStance == Stance.Standing)
				{
					maxSpeed = speed;
				}
				else
				{
					maxSpeed = crouchingSpeed;
				}
				maxForce = maximumForce;
			}
			else
			{
				MovementMode = MovementMode.Sliding;
				maxSpeed = slidingSpeed;
				maxForce = maximumSlidingForce;
			}
		}
		else
		{
			MovementMode = MovementMode.Floating;
			maxSpeed = airSpeed;
			maxForce = maximumAirForce;
			supportEntity = null;
		}
		if (!flag)
		{
			maxSpeed = 0f;
		}
		maxForce *= dt;
		Matrix3X3 orientationMatrix = ((Entity)character.Body).OrientationMatrix;
		Vector3 down = ((Matrix3X3)(ref orientationMatrix)).Down;
		if (MovementMode != MovementMode.Floating)
		{
			if (flag)
			{
				Vector3 val = new Vector3(movementDirection.X, 0f, movementDirection.Y);
				Vector3 val2 = default(Vector3);
				Vector3.Add(ref val, ref down, ref val2);
				Plane val3 = new Plane(supportData.Normal, 0f);
				float num = default(float);
				Vector3 val4 = default(Vector3);
				Toolbox.GetLinePlaneIntersection(ref val, ref val2, ref val3, ref num, ref val4);
				((Vector3)(ref val4)).Normalize();
				Vector3 val5 = default(Vector3);
				Vector3.Cross(ref val4, ref supportData.Normal, ref val5);
				linearJacobianA1 = val4;
				linearJacobianA2 = val5;
				linearJacobianB1 = -val4;
				linearJacobianB2 = -val5;
			}
			else
			{
				float num2 = default(float);
				Vector3.Dot(ref linearJacobianA1, ref supportData.Normal, ref num2);
				Vector3 val6 = default(Vector3);
				Vector3.Multiply(ref supportData.Normal, num2, ref val6);
				Vector3.Subtract(ref linearJacobianA1, ref val6, ref linearJacobianA1);
				float num3 = ((Vector3)(ref linearJacobianA1)).LengthSquared();
				if (num3 < 1E-07f)
				{
					Vector3.Cross(ref Toolbox.RightVector, ref supportData.Normal, ref linearJacobianA1);
					num3 = ((Vector3)(ref linearJacobianA1)).LengthSquared();
					if (num3 < 1E-07f)
					{
						Vector3.Cross(ref Toolbox.ForwardVector, ref supportData.Normal, ref linearJacobianA1);
						num3 = ((Vector3)(ref linearJacobianA1)).LengthSquared();
					}
				}
				Vector3.Divide(ref linearJacobianA1, (float)Math.Sqrt(num3), ref linearJacobianA1);
				Vector3.Cross(ref linearJacobianA1, ref supportData.Normal, ref linearJacobianA2);
				linearJacobianB1 = -linearJacobianA1;
				linearJacobianB2 = -linearJacobianA2;
			}
			if (supportEntity != null)
			{
				Vector3 val7 = supportData.Position - supportEntity.Position;
				Vector3.Cross(ref linearJacobianA1, ref val7, ref angularJacobianB1);
				Vector3.Cross(ref linearJacobianA2, ref val7, ref angularJacobianB2);
			}
			else
			{
				angularJacobianB1 = default(Vector3);
				angularJacobianB2 = default(Vector3);
			}
		}
		else
		{
			linearJacobianA1 = new Vector3(movementDirection.X, 0f, movementDirection.Y);
			linearJacobianA2 = new Vector3(movementDirection.Y, 0f, 0f - movementDirection.X);
		}
		targetVelocity.X = maxSpeed;
		targetVelocity.Y = 0f;
		if (supportEntity != null && supportEntity.IsDynamic)
		{
			float num4 = 0f;
			float inverseMass = ((Entity)character.Body).InverseMass;
			float num5 = inverseMass;
			float num6 = inverseMass;
			Matrix3X3 inertiaTensorInverse = supportEntity.InertiaTensorInverse;
			Matrix3X3.Multiply(ref inertiaTensorInverse, supportForceFactor, ref inertiaTensorInverse);
			inverseMass = supportForceFactor * supportEntity.InverseMass;
			Vector3 val8 = default(Vector3);
			Matrix3X3.Transform(ref angularJacobianB1, ref inertiaTensorInverse, ref val8);
			float num7 = default(float);
			Vector3.Dot(ref val8, ref angularJacobianB1, ref num7);
			num5 += inverseMass + num7;
			Vector3.Dot(ref val8, ref angularJacobianB2, ref num7);
			num4 += num7;
			Matrix3X3.Transform(ref angularJacobianB2, ref inertiaTensorInverse, ref val8);
			Vector3.Dot(ref val8, ref angularJacobianB2, ref num7);
			num6 += inverseMass + num7;
			massMatrix.M11 = num5;
			massMatrix.M12 = num4;
			massMatrix.M21 = num4;
			massMatrix.M22 = num6;
			Matrix2X2.Invert(ref massMatrix, ref massMatrix);
		}
		else
		{
			Matrix2X2.CreateScale(((Entity)character.Body).Mass, ref massMatrix);
		}
		if (supportEntity != null && ((wasTryingToMove && !flag) || (!hadTraction && supportData.HasTraction) || supportEntity != previousSupportEntity))
		{
			timeSinceTransition = 0f;
		}
		if (!flag && supportData.HasTraction && supportEntity != null)
		{
			float num8 = speed / (maximumForce * ((Entity)character.Body).InverseMass);
			if (timeSinceTransition >= 0f && timeSinceTransition < num8)
			{
				timeSinceTransition += dt;
			}
			if (timeSinceTransition >= num8)
			{
				Vector3.Multiply(ref down, character.Body.Height * 0.5f, ref positionLocalOffset);
				positionLocalOffset = positionLocalOffset + ((Entity)character.Body).Position - supportEntity.Position;
				positionLocalOffset = Matrix3X3.TransformTranspose(positionLocalOffset, supportEntity.OrientationMatrix);
				timeSinceTransition = -1f;
			}
			if (timeSinceTransition < 0f)
			{
				Vector3 val9 = default(Vector3);
				Vector3.Multiply(ref down, character.Body.Height * 0.5f, ref val9);
				val9 += ((Entity)character.Body).Position;
				Vector3 val10 = Matrix3X3.Transform(positionLocalOffset, supportEntity.OrientationMatrix) + supportEntity.Position;
				Vector3 val11 = default(Vector3);
				Vector3.Subtract(ref val9, ref val10, ref val11);
				if (((Vector3)(ref val11)).LengthSquared() > 0.0225f)
				{
					Vector3.Multiply(ref down, character.Body.Height * 0.5f, ref positionLocalOffset);
					positionLocalOffset = positionLocalOffset + ((Entity)character.Body).Position - supportEntity.Position;
					positionLocalOffset = Matrix3X3.TransformTranspose(positionLocalOffset, supportEntity.OrientationMatrix);
					positionCorrectionBias = default(Vector2);
				}
				else
				{
					Vector3.Dot(ref val11, ref linearJacobianA1, ref positionCorrectionBias.X);
					Vector3.Dot(ref val11, ref linearJacobianA2, ref positionCorrectionBias.Y);
					Vector2.Multiply(ref positionCorrectionBias, 0.2f / dt, ref positionCorrectionBias);
				}
			}
		}
		else
		{
			timeSinceTransition = 0f;
			positionCorrectionBias = default(Vector2);
		}
		wasTryingToMove = flag;
		hadTraction = supportData.HasTraction;
		previousSupportEntity = supportEntity;
	}

	public override void ExclusiveUpdate()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		Vector3 val2 = default(Vector3);
		float x = accumulatedImpulse.X;
		float y = accumulatedImpulse.Y;
		val.X = linearJacobianA1.X * x + linearJacobianA2.X * y;
		val.Y = linearJacobianA1.Y * x + linearJacobianA2.Y * y;
		val.Z = linearJacobianA1.Z * x + linearJacobianA2.Z * y;
		((Entity)character.Body).ApplyLinearImpulse(ref val);
		if (supportEntity != null && supportEntity.IsDynamic)
		{
			Vector3.Multiply(ref val, 0f - supportForceFactor, ref val);
			x *= supportForceFactor;
			y *= supportForceFactor;
			val2.X = x * angularJacobianB1.X + y * angularJacobianB2.X;
			val2.Y = x * angularJacobianB1.Y + y * angularJacobianB2.Y;
			val2.Z = x * angularJacobianB1.Z + y * angularJacobianB2.Z;
			supportEntity.ApplyLinearImpulse(ref val);
			supportEntity.ApplyAngularImpulse(ref val2);
		}
	}

	public override float SolveIteration()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		Vector2 relativeVelocity = RelativeVelocity;
		Vector2.Add(ref relativeVelocity, ref positionCorrectionBias, ref relativeVelocity);
		Vector2 val = default(Vector2);
		Vector2.Subtract(ref targetVelocity, ref relativeVelocity, ref val);
		Matrix2X2.Transform(ref val, ref massMatrix, ref val);
		Vector2 val2 = accumulatedImpulse;
		if (MovementMode == MovementMode.Floating)
		{
			accumulatedImpulse.X = MathHelper.Clamp(accumulatedImpulse.X + val.X, 0f, maxForce);
			accumulatedImpulse.Y = 0f;
		}
		else
		{
			Vector2.Add(ref val, ref accumulatedImpulse, ref accumulatedImpulse);
			float num = ((Vector2)(ref accumulatedImpulse)).LengthSquared();
			if (num > maxForce * maxForce)
			{
				Vector2.Multiply(ref accumulatedImpulse, maxForce / (float)Math.Sqrt(num), ref accumulatedImpulse);
			}
		}
		Vector2.Subtract(ref accumulatedImpulse, ref val2, ref val);
		Vector3 val3 = default(Vector3);
		Vector3 val4 = default(Vector3);
		float x = val.X;
		float y = val.Y;
		val3.X = linearJacobianA1.X * x + linearJacobianA2.X * y;
		val3.Y = linearJacobianA1.Y * x + linearJacobianA2.Y * y;
		val3.Z = linearJacobianA1.Z * x + linearJacobianA2.Z * y;
		((Entity)character.Body).ApplyLinearImpulse(ref val3);
		if (supportEntity != null && supportEntity.IsDynamic)
		{
			Vector3.Multiply(ref val3, 0f - supportForceFactor, ref val3);
			x *= supportForceFactor;
			y *= supportForceFactor;
			val4.X = x * angularJacobianB1.X + y * angularJacobianB2.X;
			val4.Y = x * angularJacobianB1.Y + y * angularJacobianB2.Y;
			val4.Z = x * angularJacobianB1.Z + y * angularJacobianB2.Z;
			supportEntity.ApplyLinearImpulse(ref val3);
			supportEntity.ApplyAngularImpulse(ref val4);
		}
		return Math.Abs(val.X) + Math.Abs(val.Y);
	}
}
