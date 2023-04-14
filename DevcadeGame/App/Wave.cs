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
using Whitespace.App.Util;

namespace Whitespace.App
{
    /// <summary>
    /// The rising white space that the player must avoid
    /// </summary>
    internal class Wave
    {
        private ParticleEffect _waveParticles;

        private Texture2D _texture;

        private RectangleF _destination;

        private float _farthestHeight;

        private float _speed;

        public Wave(Texture2D texture, RectangleF boundingRect)
        {
            _texture = texture;
            _destination = boundingRect;
            _destination.Offset(0f, boundingRect.Height);
            _destination.Height = boundingRect.Height * 10f;
            _destination.Width = boundingRect.Width * 1.1f;
            _destination.X -= (_destination.Width - boundingRect.Width) * 0.5f;
            _farthestHeight = boundingRect.Height;
        }

        public void Update(Player player)
        {
            float timeSpeed = PhysicsManager.IN.TimeSpeed;

            //Too high, "speed up"
            if (player.Position.Y < _destination.Y - _farthestHeight)
            {
                _destination.Y = player.Position.Y + _farthestHeight;
            }
            else if(player.Position.Y > _destination.Y)//Ded
            {
                player.GameOver();
            }
            else
            {
                //Move up normally
                _destination.Y -= _speed * timeSpeed;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(_texture, _destination.ToRectangle(), Color.White);
        }
    }
}
