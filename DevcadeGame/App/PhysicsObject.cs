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

        public Vector2 MomentOfAcceleration { get; set; }

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

        public virtual void Update() => Update(PhysicsManager.IN.TimeSpeed);

        public virtual void Update(float timeSpeed)
        {
            Velocity += MomentOfAcceleration * timeSpeed;
            Position += Velocity * timeSpeed;

            //Friction
            Velocity -= Velocity * PhysicsManager.IN.Friction * timeSpeed;

            //Reset acceleration
            MomentOfAcceleration = Vector2.Zero;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                Texture, Position, null, Tint, Rotation, Origin,
                _textureDownscale * Scale, SpriteEffects.None, 0f);
        }

        public virtual void DrawHitbox(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawCircle(Collider, 50, Color.Red, 10f, 1f);
        }
    }
}
