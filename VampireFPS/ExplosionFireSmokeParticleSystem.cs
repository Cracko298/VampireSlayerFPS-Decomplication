using System;
using DPSF;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace VampireFPS;

public class ExplosionFireSmokeParticleSystem : DefaultSprite3DBillboardTextureCoordinatesParticleSystem
{
	private Rectangle _flameSmoke1TextureCoordinates = new Rectangle(0, 0, 128, 128);

	private Rectangle _flameSmoke2TextureCoordinates = new Rectangle(128, 0, 128, 128);

	private Rectangle _flameSmoke3TextureCoordinates = new Rectangle(0, 128, 128, 128);

	private Rectangle _flameSmoke4TextureCoordinates = new Rectangle(128, 128, 128, 128);

	public Color ExplosionColor { get; set; }

	public int ExplosionParticleSize { get; set; }

	public int ExplosionIntensity { get; set; }

	public ExplosionFireSmokeParticleSystem(Game game)
		: base(game)
	{
	}//IL_000d: Unknown result type (might be due to invalid IL or missing references)
	//IL_0012: Unknown result type (might be due to invalid IL or missing references)
	//IL_0028: Unknown result type (might be due to invalid IL or missing references)
	//IL_002d: Unknown result type (might be due to invalid IL or missing references)
	//IL_0043: Unknown result type (might be due to invalid IL or missing references)
	//IL_0048: Unknown result type (might be due to invalid IL or missing references)
	//IL_0062: Unknown result type (might be due to invalid IL or missing references)
	//IL_0067: Unknown result type (might be due to invalid IL or missing references)


	protected override void InitializeRenderProperties()
	{
		((DPSFDefaultSprite3DBillboardParticleSystem<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).InitializeRenderProperties();
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).RenderProperties.BlendState = BlendState.AlphaBlend;
	}

	public override void AutoInitialize(GraphicsDevice graphicsDevice, ContentManager contentManager, SpriteBatch spriteBatch)
	{
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).InitializeSpriteParticleSystem(graphicsDevice, contentManager, 100, 100, "Sprites/Smoke");
		((DPSFDefaultBaseParticleSystem<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).Name = "Explosion - Fire Smoke";
		LoadEvents();
	}

	public void LoadEvents()
	{
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).ParticleInitializationFunction = InitializeParticleExplosion;
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).ParticleEvents.RemoveAllEvents();
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).ParticleEvents.AddEveryTimeEvent((UpdateParticleDelegate<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)((DPSFDefaultBaseParticleSystem<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).UpdateParticleVelocityUsingExternalForce);
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).ParticleEvents.AddEveryTimeEvent((UpdateParticleDelegate<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)((DPSFDefaultBaseParticleSystem<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).UpdateParticlePositionAndVelocityUsingAcceleration);
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).ParticleEvents.AddEveryTimeEvent((UpdateParticleDelegate<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)((DPSFDefaultSpriteParticleSystem<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).UpdateParticleRotationAndRotationalVelocityUsingRotationalAcceleration);
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).ParticleEvents.AddEveryTimeEvent((UpdateParticleDelegate<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)((DPSFDefaultBaseParticleSystem<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).UpdateParticleTransparencyWithQuickFadeInAndSlowFadeOut, 100);
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).Emitter.PositionData.Position = new Vector3(0f, 0f, 0f);
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).Emitter.ParticlesPerSecond = 20f;
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).Emitter.EmitParticlesAutomatically = false;
		ExplosionColor = new Color(255, 120, 0);
	}

	public void InitializeParticleExplosion(DefaultSprite3DBillboardTextureCoordinatesParticle particle)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		((DPSFParticle)particle).Lifetime = ((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).RandomNumber.Between(0.5f, 0.75f);
		((DPSFDefaultBaseParticle)particle).Color = (((DPSFDefaultBaseParticle)particle).StartColor = Color.Gray);
		((DPSFDefaultBaseParticle)particle).EndColor = Color.Black;
		((DPSFDefaultBaseParticle)particle).Position = ((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).Emitter.PositionData.Position;
		((DPSFDefaultBaseParticle)particle).Velocity = DPSFHelper.RandomNormalizedVector() * (float)((Random)(object)((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).RandomNumber).Next(1, 50) * 0.02f;
		((DPSFDefaultBaseParticle)particle).ExternalForce = new Vector3(0f, 0.5f, 0f);
		float size = (((DefaultSpriteParticle)particle).StartSize = 2f);
		((DefaultSpriteParticle)particle).Size = size;
		((DefaultSpriteParticle)particle).EndSize = 4f;
		((DefaultSpriteParticle)particle).Rotation = ((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).RandomNumber.Between(0f, (float)Math.PI * 2f);
		((DefaultSpriteParticle)particle).RotationalVelocity = ((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).RandomNumber.Between(-(float)Math.PI / 2f, (float)Math.PI / 2f) * 0.3f;
		particle.SetTextureCoordinates(new Rectangle(0, 0, 64, 64));
	}

	public void ChangeExplosionColor()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		ExplosionColor = DPSFHelper.RandomColor();
	}

	public void ChangeExplosionColor(Color color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		ExplosionColor = color;
	}
}
