using System;
using DPSF;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace VampireFPS;

public class ExplosionDebrisParticleSystem : DefaultSprite3DBillboardTextureCoordinatesParticleSystem
{
	private Rectangle _debris1TextureCoordinates = new Rectangle(0, 0, 137, 291);

	private Rectangle _debris2TextureCoordinates = new Rectangle(138, 0, 128, 219);

	private Rectangle _debris3TextureCoordinates = new Rectangle(268, 0, 242, 190);

	private Rectangle _debris4TextureCoordinates = new Rectangle(0, 293, 165, 146);

	private Rectangle _debris5TextureCoordinates = new Rectangle(166, 220, 237, 146);

	private Rectangle _debris6TextureCoordinates = new Rectangle(405, 191, 106, 127);

	private Rectangle _debris7TextureCoordinates = new Rectangle(165, 366, 239, 145);

	private Rectangle _debris8TextureCoordinates = new Rectangle(404, 337, 107, 103);

	public Color ExplosionColor { get; set; }

	public int ExplosionParticleSize { get; set; }

	public int ExplosionIntensity { get; set; }

	public ExplosionDebrisParticleSystem(Game game)
		: base(game)
	{
	}//IL_000d: Unknown result type (might be due to invalid IL or missing references)
	//IL_0012: Unknown result type (might be due to invalid IL or missing references)
	//IL_0028: Unknown result type (might be due to invalid IL or missing references)
	//IL_002d: Unknown result type (might be due to invalid IL or missing references)
	//IL_0043: Unknown result type (might be due to invalid IL or missing references)
	//IL_0048: Unknown result type (might be due to invalid IL or missing references)
	//IL_005e: Unknown result type (might be due to invalid IL or missing references)
	//IL_0063: Unknown result type (might be due to invalid IL or missing references)
	//IL_007d: Unknown result type (might be due to invalid IL or missing references)
	//IL_0082: Unknown result type (might be due to invalid IL or missing references)
	//IL_0096: Unknown result type (might be due to invalid IL or missing references)
	//IL_009b: Unknown result type (might be due to invalid IL or missing references)
	//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
	//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
	//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
	//IL_00d3: Unknown result type (might be due to invalid IL or missing references)


	public override void SetCameraPosition(Vector3 cameraPosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((DPSFDefaultSprite3DBillboardParticleSystem<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).CameraPosition = cameraPosition;
	}

	public override void AutoInitialize(GraphicsDevice graphicsDevice, ContentManager contentManager, SpriteBatch spriteBatch)
	{
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).InitializeSpriteParticleSystem(graphicsDevice, contentManager, 100, 100, "Sprites/Gibs");
		((DPSFDefaultBaseParticleSystem<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).Name = "Explosion - Debris";
		LoadEvents();
	}

	public void LoadEvents()
	{
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).ParticleInitializationFunction = InitializeParticleExplosion;
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).ParticleEvents.RemoveAllEvents();
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).ParticleEvents.AddEveryTimeEvent((UpdateParticleDelegate<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)((DPSFDefaultBaseParticleSystem<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).UpdateParticleVelocityUsingExternalForce);
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).ParticleEvents.AddEveryTimeEvent((UpdateParticleDelegate<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)((DPSFDefaultBaseParticleSystem<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).UpdateParticlePositionAndVelocityUsingAcceleration);
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).ParticleEvents.AddEveryTimeEvent((UpdateParticleDelegate<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)((DPSFDefaultSpriteParticleSystem<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).UpdateParticleRotationAndRotationalVelocityUsingRotationalAcceleration);
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).ParticleEvents.AddEveryTimeEvent((UpdateParticleDelegate<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)((DPSFDefaultBaseParticleSystem<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).UpdateParticleTransparencyWithQuickFadeInAndQuickFadeOut, 100);
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).ParticleEvents.AddEveryTimeEvent((UpdateParticleDelegate<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)((DPSFDefaultBaseParticleSystem<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).UpdateParticleColorUsingLerp);
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).Emitter.PositionData.Position = new Vector3(0f, 0f, 0f);
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).Emitter.ParticlesPerSecond = 10000f;
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).Emitter.EmitParticlesAutomatically = false;
		ExplosionColor = new Color(128, 128, 128);
		ExplosionParticleSize = 1;
		ExplosionIntensity = 100;
	}

	public void SetupToAutoExplodeEveryInterval(float intervalInSeconds)
	{
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).ParticleSystemEvents.RemoveAllEventsInGroup(1);
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).ParticleSystemEvents.LifetimeData.EndOfLifeOption = (EParticleSystemEndOfLifeOptions<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)1;
		((DPSFParticle)((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).ParticleSystemEvents.LifetimeData).Lifetime = intervalInSeconds;
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).ParticleSystemEvents.AddTimedEvent(0f, (UpdateParticleSystemDelegate<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)UpdateParticleSystemToExplode, 0, 1);
	}

	public void InitializeParticleExplosion(DefaultSprite3DBillboardTextureCoordinatesParticle particle)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		((DPSFParticle)particle).Lifetime = ((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).RandomNumber.Between(1f, 1.2f);
		((DPSFDefaultBaseParticle)particle).Color = (((DPSFDefaultBaseParticle)particle).StartColor = ExplosionColor);
		((DPSFDefaultBaseParticle)particle).EndColor = Color.Black;
		((DPSFDefaultBaseParticle)particle).Position = ((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).Emitter.PositionData.Position;
		((DPSFDefaultBaseParticle)particle).ExternalForce = new Vector3(0f, -40f, 0f);
		((DefaultSpriteParticle)particle).RotationalVelocity = ((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).RandomNumber.Between(-(float)Math.PI / 2f, (float)Math.PI / 2f);
		((DPSFDefaultBaseParticle)particle).Velocity = DPSFHelper.RandomNormalizedVector();
		ref Vector3 velocity = ref ((DPSFDefaultBaseParticle)particle).Velocity;
		velocity.Y *= 2f;
		if (((DPSFDefaultBaseParticle)particle).Velocity.Y < 0f)
		{
			ref Vector3 velocity2 = ref ((DPSFDefaultBaseParticle)particle).Velocity;
			velocity2.Y *= -1f;
		}
		if (((DPSFDefaultBaseParticle)particle).Velocity.Y < 0.3f)
		{
			ref Vector3 velocity3 = ref ((DPSFDefaultBaseParticle)particle).Velocity;
			velocity3.Y += 0.3f;
		}
		if (((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).RandomNumber.Between(0f, 5f) == 0f)
		{
			int num = 10;
			new Vector3(((DPSFDefaultSprite3DBillboardParticleSystem<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).CameraPosition.X + (float)((Random)(object)((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).RandomNumber).Next(-num, num), ((DPSFDefaultSprite3DBillboardParticleSystem<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).CameraPosition.Y + (float)((Random)(object)((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).RandomNumber).Next(-num, num), ((DPSFDefaultSprite3DBillboardParticleSystem<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).CameraPosition.Z + (float)((Random)(object)((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).RandomNumber).Next(-num, num));
			((DPSFDefaultBaseParticle)particle).Velocity = ((DPSFDefaultBaseParticle)particle).Position;
			((Vector3)(ref ((DPSFDefaultBaseParticle)particle).Velocity)).Normalize();
		}
		((DPSFDefaultBaseParticle)particle).Velocity = ((DPSFDefaultBaseParticle)particle).Velocity * (float)((Random)(object)((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).RandomNumber).Next(10, 15);
		Rectangle textureCoordinates = (Rectangle)(((Random)(object)((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).RandomNumber).Next(0, 8) switch
		{
			1 => _debris2TextureCoordinates, 
			2 => _debris3TextureCoordinates, 
			3 => _debris4TextureCoordinates, 
			4 => _debris5TextureCoordinates, 
			5 => _debris6TextureCoordinates, 
			6 => _debris7TextureCoordinates, 
			7 => _debris8TextureCoordinates, 
			_ => _debris1TextureCoordinates, 
		});
		particle.SetTextureCoordinates(textureCoordinates);
		((DefaultSpriteParticle)particle).Width = textureCoordinates.Width;
		((DefaultSpriteParticle)particle).Height = textureCoordinates.Height;
		((DefaultSpriteParticle)particle).Size = 0.5f;
		((DefaultSpriteParticle)particle).ScaleToWidth((float)ExplosionParticleSize * ((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).RandomNumber.Between(0.75f, 1.25f));
	}

	protected void UpdateParticleSystemToExplode(float elapsedTimeInSeconds)
	{
		Explode();
	}

	public void Explode()
	{
		((DPSF<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>)this).Emitter.BurstParticles = ExplosionIntensity;
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
