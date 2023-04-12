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

namespace Whitespace.App
{
    internal class Orb : PhysicsObject
    {
        private ParticleEffect _destroyedParticles;

        private ParticleEffect _idleParticles;

        public Orb(Texture2D texture, Texture2D particleTexture) : base(texture)
        {
            _destroyedParticles = new ParticleEffect()
            {
                Emitters = new List<ParticleEmitter>
                {
                    new ParticleEmitter(
                        new TextureRegion2D(particleTexture),
                        10, TimeSpan.FromSeconds(5d), Profile.Point())
                    {
                        Parameters = new ParticleReleaseParameters()
                        {
                            Speed = 100f
                        },
                        Modifiers =
                        {
                            new AgeModifier
                            {

                            }
                        }
                    }
                }
            };


        }
    }
}
