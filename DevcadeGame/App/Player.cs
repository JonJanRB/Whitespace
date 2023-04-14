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

        private Texture2D _arrowTexture;
        private Vector2 _arrowOrigin;
        private Vector2 _arrowTextureDownscalse;
        private float _currentArrowScale;
        public float TargetArrowScale { get; set; }

        public Player(Texture2D texture, Texture2D arrow) : base(texture)
        {
            IsAlive = true;
            _arrowTexture = arrow;
            Vector2 textureSize = arrow.Bounds.Size.ToVector2();
            _arrowTextureDownscalse = new Vector2(1f / textureSize.X, 1f / textureSize.Y);
            _arrowOrigin = textureSize / 2f;
        }

        /// <summary>
        /// Updates direction vector
        /// </summary>
        /// <param name="timeSpeed"></param>
        public override void Update(float timeSpeed)
        {
            MomentOfAcceleration += PhysicsManager.IN.Gravity * timeSpeed;

            base.Update(timeSpeed);

            Direction += MathHelper.WrapAngle(TargetDirection - Direction) * 0.02f;
            _currentArrowScale += (TargetArrowScale - _currentArrowScale) * 0.4f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            //Arrow
            spriteBatch.Draw(
                _arrowTexture,
                Position + DirectionVector * 1000f,
                null,
                Tint * 0.5f,
                Direction + MathHelper.PiOver2,
                _arrowOrigin,
                _arrowTextureDownscalse * _currentArrowScale,
                SpriteEffects.None,
                0f);
        }

        public override void DrawHitbox(SpriteBatch spriteBatch)
        {
            base.DrawHitbox(spriteBatch);
            spriteBatch.DrawLine(Position, 1000f, Direction, Color.GreenYellow, 50f);
        }

        public void GameOver()
        {
            if(IsAlive)
            {
                IsAlive = false;
                SoundManager.WhitespaceTouch.Play();
            }
        }
    }
}
