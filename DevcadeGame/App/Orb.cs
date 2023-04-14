using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended.Particles;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Timers;
using Whitespace.App.Util;

namespace Whitespace.App
{
    internal class Orb : PhysicsObject
    {
        private ParticleEffect _destroyedParticles;

        private ParticleEffect _idleParticles;

        public Orb() : 
            this(ObjectManager.OrbTexture, ObjectManager.OrbDestroyParticle, ObjectManager.OrbColor)
        {
            HitboxRadius = 100f;
            Scale = new Vector2(100f);
        }

        public Orb(Texture2D texture, Texture2D particleTexture, Color tint) : base(texture)
        {
            Tint = tint;
            _destroyedParticles = new ParticleEffect(autoTrigger: false)
            {
                Emitters = new List<ParticleEmitter>
                {
                    new ParticleEmitter(
                        new TextureRegion2D(particleTexture),
                        5, TimeSpan.FromSeconds(0.5d), Profile.Point())
                    {
                        AutoTrigger = false,
                        Parameters = new ParticleReleaseParameters()
                        {
                            Speed = new(5000f, 10000f),
                            Rotation = new (0f, MathHelper.TwoPi),
                            Opacity = 1f,
                            Color = tint.ToHsl()
                        },
                        Modifiers =
                        {
                            new AgeModifier
                            {
                                Interpolators =
                                {
                                    new ScaleInterpolator()
                                    {
                                        StartValue = new Vector2(100f),
                                        EndValue = Vector2.Zero
                                    }
                                }
                            },
                            new RotationModifier() { RotationRate = 1f },
                            new LinearGravityModifier()
                            {
                                Direction = Vector2.UnitY,
                                Strength = 10000f
                            },


                        },

                    }
                }
            };
        }

        public void Destroy(Vector2 velocity)
        {
            SoundManager.OrbDestroy.Play();
            _destroyedParticles.Position = Position;
            foreach(var emitter in _destroyedParticles.Emitters)
            {
                emitter.Profile = Profile.Spray(velocity, 2f);
            }
            for (int i = 0; i < 5; i++)
            {
                _destroyedParticles.Trigger();
            }
            Enabled = false;
        }

        public override void Update(float timeSpeed)
        {
            base.Update(timeSpeed);
            _destroyedParticles.Update(timeSpeed);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(Enabled == true)
                base.Draw(spriteBatch);
            spriteBatch.Draw(_destroyedParticles);
        }

        public override void DrawHitbox(SpriteBatch spriteBatch)
        {
            if (Enabled == true)
                base.DrawHitbox(spriteBatch);
            
        }

    }
}
