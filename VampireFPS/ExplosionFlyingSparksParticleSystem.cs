using System;
using DPSF;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace VampireFPS;

public class ExplosionFlyingSparksParticleSystem : DefaultTexturedQuadTextureCoordinatesParticleSystem
{
	public Vector3 Normal;

	private Rectangle _sparkTextureCoordinates = new Rectangle(0, 0, 16, 16);

	public Color ExplosionColor { get; set; }

	public int ExplosionParticleSize { get; set; }

	public int ExplosionIntensity { get; set; }

	public ExplosionFlyingSparksParticleSystem(Game game)
		: base(game)
	{
	}//IL_0007: Unknown result type (might be due to invalid IL or missing references)
	//IL_000c: Unknown result type (might be due to invalid IL or missing references)


	protected override void InitializeRenderProperties()
	{
		((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).InitializeRenderProperties();
		((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).RenderProperties.RasterizerState.CullMode = (CullMode)0;
		((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).RenderProperties.BlendState = BlendState.Additive;
	}

	public override void AutoInitialize(GraphicsDevice graphicsDevice, ContentManager contentManager, SpriteBatch spriteBatch)
	{
		((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).InitializeTexturedQuadParticleSystem(graphicsDevice, contentManager, 100, 100, (UpdateVertexDelegate<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)((DPSFDefaultTexturedQuadParticleSystem<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)(object)this).UpdateVertexProperties, "Sprites/White");
		((DPSFDefaultBaseParticleSystem<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).Name = "Explosion - Flying Sparks";
		LoadEvents();
	}

	public void LoadEvents()
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).ParticleInitializationFunction = InitializeParticleExplosion;
		((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).ParticleEvents.RemoveAllEvents();
		((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).ParticleEvents.AddEveryTimeEvent((UpdateParticleDelegate<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)((DPSFDefaultBaseParticleSystem<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).UpdateParticlePositionAndVelocityUsingAcceleration);
		((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).ParticleEvents.AddEveryTimeEvent((UpdateParticleDelegate<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)((DPSFDefaultBaseParticleSystem<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).UpdateParticleTransparencyWithQuickFadeInAndSlowFadeOut, 100);
		((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).Emitter.PositionData.Position = new Vector3(0f, 0f, 0f);
		((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).Emitter.ParticlesPerSecond = 10000f;
		((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).Emitter.EmitParticlesAutomatically = false;
		ExplosionColor = new Color(255, 255, 0);
		ExplosionParticleSize = 1;
	}

	public void InitializeParticleExplosion(DefaultTextureQuadTextureCoordinatesParticle particle)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		((DPSFParticle)particle).Lifetime = ((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).RandomNumber.Between(0.1f, 0.2f);
		((DPSFDefaultBaseParticle)particle).Color = ExplosionColor;
		((DPSFDefaultBaseParticle)particle).Position = ((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).Emitter.PositionData.Position;
		Vector3 val = DPSFHelper.RandomNormalizedVector();
		val = Vector3.Transform(Normal, Quaternion.CreateFromAxisAngle(val, (float)((((Random)(object)((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).RandomNumber).NextDouble() - 0.5) * (double)MathHelper.ToRadians(120f))));
		((DPSFDefaultBaseParticle)particle).Velocity = val * (float)((Random)(object)((DPSF<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>)this).RandomNumber).Next(100, 225) * 0.1f;
		((DefaultQuadParticle)particle).Right = -((DPSFDefaultBaseParticle)particle).Velocity;
		((DefaultQuadParticle)particle).Width = ExplosionParticleSize;
		((DefaultQuadParticle)particle).Height = (float)ExplosionParticleSize * 0.01f;
	}
}
