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

        public float Direction { get; set; }

        public float TargetDirection { get; set; }

        public Player(Texture2D texture) : base(texture) { }


        public override void Update(float timeSpeed)
        {
            base.Update(timeSpeed);

            Direction += MathHelper.WrapAngle(TargetDirection - Direction) * 0.05f;
        }

        public override void DrawHitbox(SpriteBatch spriteBatch)
        {
            base.DrawHitbox(spriteBatch);
            spriteBatch.DrawLine(Position, 1000f, Direction - MathHelper.PiOver2, Color.GreenYellow, 50f, 1f);
        }
    }
}
