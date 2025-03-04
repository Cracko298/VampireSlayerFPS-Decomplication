using System;
using DPSF;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace VampireFPS;

public class BloodQuadSprayParticleSystem : DefaultSprite3DBillboardParticleSystem
{
	public Vector3 Normal;

	public BloodQuadSprayParticleSystem(Game cGame)
		: base(cGame)
	{
	}

	public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
	{
		((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)this).InitializeSpriteParticleSystem(cGraphicsDevice, cContentManager, 100, 100, "Sprites/Smoke");
		((DPSFDefaultBaseParticleSystem<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)this).Name = "Quad Spray";
		LoadSprayEvents();
	}

	public void LoadSprayEvents()
	{
		((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)this).ParticleInitializationFunction = InitializeParticleSpray;
		((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)this).Emitter.ParticlesPerSecond = 10000f;
		((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)this).Emitter.EmitParticlesAutomatically = false;
		((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)this).ParticleEvents.RemoveAllEventsInGroup(0);
		((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)this).ParticleEvents.AddEveryTimeEvent((UpdateParticleDelegate<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)((DPSFDefaultBaseParticleSystem<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)this).UpdateParticlePositionAndVelocityUsingAcceleration, 100);
		((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)this).ParticleEvents.AddEveryTimeEvent((UpdateParticleDelegate<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)((DPSFDefaultBaseParticleSystem<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)this).UpdateParticleTransparencyWithQuickFadeInAndSlowFadeOut, 100);
		((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)this).ParticleEvents.AddEveryTimeEvent((UpdateParticleDelegate<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)((DPSFDefaultBaseParticleSystem<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)this).UpdateParticleVelocityUsingExternalForce, 0, 1);
		((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)this).ParticleEvents.AddEveryTimeEvent((UpdateParticleDelegate<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)((DPSFDefaultSpriteParticleSystem<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)this).UpdateParticleRotationAndRotationalVelocityUsingRotationalAcceleration);
	}

	public void InitializeParticleSpray(DefaultSprite3DBillboardParticle cParticle)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		((DPSFParticle)cParticle).Lifetime = ((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)this).RandomNumber.Between(1.1f, 1.3f);
		((DPSFDefaultBaseParticle)cParticle).Position = ((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)this).Emitter.PositionData.Position;
		Vector3 val = DPSFHelper.RandomNormalizedVector();
		val = Vector3.Transform(Normal, Quaternion.CreateFromAxisAngle(val, (float)((((Random)(object)((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)this).RandomNumber).NextDouble() - 0.5) * (double)MathHelper.ToRadians(90f))));
		((DPSFDefaultBaseParticle)cParticle).Velocity = val * 0.3f + new Vector3(0f, 10f, 0f);
		((DefaultSpriteParticle)cParticle).Size = 1f;
		((DPSFDefaultBaseParticle)cParticle).ExternalForce = new Vector3(0f, -30f, 0f);
		((DPSFDefaultBaseParticle)cParticle).Color = new Color(128, 0, 0);
		((DefaultSpriteParticle)cParticle).Rotation = ((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)this).RandomNumber.Between(0f, (float)Math.PI * 2f);
		((DefaultSpriteParticle)cParticle).RotationalVelocity = ((DPSF<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>)this).RandomNumber.Between(-(float)Math.PI / 2f, (float)Math.PI / 2f);
	}
}
