using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whitespace.App
{
    internal class PhysicsObject
    {
        public Texture2D Texture { get; set; }

        public CircleF Collider { get; set; }

        public Color Tint { get; set; }

        public float Rotation { get; set; }

        public float Scale { get; set; }

        public Vector2 Origin { get; set; }

        public Vector2 Position { get; set; }

        public Vector2 Velocity { get; set; }

        public Vector2 Acceleration { get; set; }

        public PhysicsObject()
        {

        }

        public bool Intersects(IShapeF shape)
        {
            return Collider.Intersects(shape);
        }

        public void Update(GameTime gameTime)
        {
            Velocity += Acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Friction
            Velocity *= 0.9f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                Texture, Position, null, Tint,
                Rotation, Origin, Scale, SpriteEffects.None, 0f);
        }

        public void DrawHitbox(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawCircle(Collider, 32, Color.Red, 10f, 1f);
        }
    }
}
