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

namespace Whitespace.App
{
    internal class Orb : PhysicsObject
    {
        private ParticleEffect _destroyedParticles;

        private ParticleEffect _idleParticles;

        public Orb(Texture2D texture, Texture2D particleTexture) : base(texture)
        {
            _destroyedParticles = new ParticleEffect(autoTrigger: false)
            {
                Emitters = new List<ParticleEmitter>
                {
                    new ParticleEmitter(
                        new TextureRegion2D(particleTexture),
                        500, TimeSpan.FromSeconds(0.5d), Profile.Point())
                    {
                        AutoTrigger = false,
                        Parameters = new ParticleReleaseParameters()
                        {
                            Speed = new(1000f, 2000f),
                            Rotation = new (0f, 2f),
                        },
                        Modifiers =
                        {
                            new AgeModifier
                            {
                                Interpolators =
                                {
                                    new ScaleInterpolator()
                                    {
                                        StartValue = new Vector2(1000f),
                                        EndValue = Vector2.Zero
                                    }
                                }
                            },
                            new RotationModifier() { RotationRate = 1f },
                            new LinearGravityModifier()
                            {
                                Direction = Vector2.UnitY,
                                Strength = 100f
                            },


                        },
                        
                    }
                }
            };


        }

        public void Destroy()
        {
            _destroyedParticles.Position = Mouse.GetState().Position.ToVector2();
            _destroyedParticles.Trigger();
        }

        public override void Update(float timeSpeed)
        {
            base.Update(timeSpeed);
            _destroyedParticles.Update(timeSpeed);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            spriteBatch.Draw(_destroyedParticles);
        }
    }
}
