using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers.Containers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Whitespace
{
    /// <summary>
    /// The rising white space that the player must avoid
    /// </summary>
    internal class Wave
    {
        private ParticleEffect _waveParticles;

        /// <summary>
        /// The size of this wave
        /// </summary>
        public Vector2 Size { get; set; }

        public Vector2 Position
        {
            get => _waveParticles.Position;
            set => _waveParticles.Position = value;
        }

        public Wave(TextureRegion2D texture)
        {
            //New particle effect
            _waveParticles = new ParticleEffect()
            {
                //Add emitter(s)
                Emitters =
                {
                    new ParticleEmitter(
                        texture, 20, TimeSpan.FromSeconds(10),
                        Profile.Line(Vector2.UnitX, Size.X))
                    {
                        Parameters = 
                        {
                            Speed = new Range<float>(0f, 50f),
                            Quantity = 3,
                            Rotation = new Range<float>(-1f, 1f),
                            Scale = new Range<float>(30f, 40f)
                        },
                        Modifiers =
                        {
                            new AgeModifier
                            {
                                Interpolators =
                                {
                                    //Fade out based on time
                                    new OpacityInterpolator
                                    {
                                        StartValue = 1f, EndValue = 0f,
                                    }
                                }
                            },
                            new RotationModifier {RotationRate = -2.1f},
                            new RectangleContainerModifier {Width = 800, Height = 480},
                            new LinearGravityModifier {Direction = -Vector2.UnitY, Strength = 30f},
                        }
                    }
                }
            };
        }

        public void Update(GameTime gt)
        {
            _waveParticles.Update(gt.GetElapsedSeconds());
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(_waveParticles);
        }
    }
}
