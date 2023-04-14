using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whitespace.App.Util;

namespace Whitespace.App
{
    internal class Player : PhysicsObject
    {

        public float Direction
        {
            get => _direction;
            set
            {
                _direction = value;
                DirectionVector = new Vector2(MathF.Cos(_direction), MathF.Sin(_direction));
            }
        }
        private float _direction;

        public float TargetDirection { get; set; }

        /// <summary>
        /// Normalized vector of where you are facing
        /// </summary>
        public Vector2 DirectionVector { get; private set; }

        public uint Flings { get; set; }

        public bool IsAlive { get; set; }

        public Player(Texture2D texture) : base(texture) { IsAlive = true; }

        /// <summary>
        /// Updates direction vector
        /// </summary>
        /// <param name="timeSpeed"></param>
        public override void Update(float timeSpeed)
        {
            MomentOfAcceleration += PhysicsManager.IN.Gravity * timeSpeed;

            base.Update(timeSpeed);

            Direction += MathHelper.WrapAngle(TargetDirection - Direction) * 0.02f;
        }

        public override void DrawHitbox(SpriteBatch spriteBatch)
        {
            base.DrawHitbox(spriteBatch);
            spriteBatch.DrawLine(Position, 1000f, Direction, Color.GreenYellow, 50f);
        }

        public void GameOver()
        {
            IsAlive = false;
            SoundManager.WhitespaceTouch.Play();
        }
    }
}
