using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whitespace.App.Util;

namespace Whitespace.App
{
    internal class PhysicsObject
    {
        public Texture2D Texture { get; }

        public Vector2 Origin { get; }

        public CircleF Collider => new CircleF(Position.ToPoint(), HitboxRadius);

        public float HitboxRadius { get; set; }
            
        public Color Tint { get; set; }

        public float Rotation { get; set; }

        /// <summary>
        /// Making it so every texture is the same size initially
        /// </summary>
        private Vector2 _textureDownscale;
        
        public Vector2 Scale { get; set; }

        public Vector2 Position { get; set; }

        public Vector2 Velocity { get; set; }

        public Vector2 Acceleration { get; set; }

        public PhysicsObject(Texture2D texture)
        {
            Texture = texture;
            Vector2 textureSize = texture.Bounds.Size.ToVector2();
            _textureDownscale = new Vector2(1f / textureSize.X, 1f / textureSize.Y);
            Origin = textureSize / 2f;
        }

        public bool Intersects(IShapeF shape)
        {
            return Collider.Intersects(shape);
        }

        public void Update() => Update(PhysicsManager.IN.TimeSpeed);

        public void Update(float timeSpeed)
        {
            Velocity += Acceleration * timeSpeed;
            Position += Velocity * timeSpeed;

            //Friction
            Velocity *= PhysicsManager.IN.Friction;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                Texture, Position, null, Tint, Rotation, Origin,
                _textureDownscale * Scale, SpriteEffects.None, 0f);
        }

        public void DrawHitbox(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawCircle(Collider, 50, Color.Red, 1f, 1f);
        }
    }
}
