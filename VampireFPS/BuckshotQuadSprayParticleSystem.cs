using System;
using DPSF;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace VampireFPS;

public class BuckshotQuadSprayParticleSystem : DefaultTexturedQuadTextureCoordinatesParticleSystem
{
	public Vector3 Normal;

	public BuckshotQuadSprayParticleSystem(Game cGame)
		: base(cGame)
	{
	}

	public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
	{
		((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).InitializeTexturedQuadParticleSystem(cGraphicsDevice, cContentManager, 100, 100, (UpdateVertexDelegate<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)((DPSFDefaultTexturedQuadParticleSystem<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)this).UpdateVertexProperties, "Sprites/White");
		((DPSFDefaultBaseParticleSystem<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).Name = "Quad Spray";
		LoadSprayEvents();
	}

	public void LoadSprayEvents()
	{
		((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).ParticleInitializationFunction = InitializeParticleSpray;
		((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).Emitter.ParticlesPerSecond = 10000f;
		((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).Emitter.EmitParticlesAutomatically = false;
		((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).ParticleEvents.RemoveAllEventsInGroup(0);
		((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).ParticleEvents.AddEveryTimeEvent((UpdateParticleDelegate<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)((DPSFDefaultBaseParticleSystem<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).UpdateParticlePositionAndVelocityUsingAcceleration, 100);
		((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).ParticleEvents.AddEveryTimeEvent((UpdateParticleDelegate<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)((DPSFDefaultQuadParticleSystem<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).UpdateParticleToFaceTheCamera, 200);
		((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).ParticleEvents.AddEveryTimeEvent((UpdateParticleDelegate<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)((DPSFDefaultBaseParticleSystem<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).UpdateParticleTransparencyWithQuickFadeInAndSlowFadeOut, 100);
		((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).ParticleEvents.AddEveryTimeEvent((UpdateParticleDelegate<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)((DPSFDefaultBaseParticleSystem<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).UpdateParticleVelocityUsingExternalForce, 0, 1);
	}

	public void InitializeParticleSpray(DefaultTextureQuadTextureCoordinatesParticle cParticle)
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
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		((DPSFParticle)cParticle).Lifetime = ((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).RandomNumber.Between(0.1f, 0.3f);
		((DPSFDefaultBaseParticle)cParticle).Position = ((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).Emitter.PositionData.Position;
		Vector3 val = DPSFHelper.RandomNormalizedVector();
		val = Vector3.Transform(Normal, Quaternion.CreateFromAxisAngle(val, (float)((((Random)(object)((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).RandomNumber).NextDouble() - 0.5) * (double)MathHelper.ToRadians(30f))));
		((DPSFDefaultBaseParticle)cParticle).Velocity = val * (float)((Random)(object)((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).RandomNumber).Next(100, 225) * 0.08f;
		((DefaultQuadParticle)cParticle).Size = 0.08f;
		((DPSFDefaultBaseParticle)cParticle).ExternalForce = new Vector3(0f, -30f, 0f);
		((DPSFDefaultBaseParticle)cParticle).Color = new Color(0, 0, 0);
	}
}
