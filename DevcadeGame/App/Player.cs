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

            DebugLog.Instance.LogFrame("Target: "+MathHelper.ToDegrees(TargetDirection), Color.Red);
            DebugLog.Instance.LogFrame("Current: " + MathHelper.ToDegrees(Direction), Color.Red);
            DebugLog.Instance.LogFrame("Target-current: "+ MathHelper.ToDegrees(TargetDirection - Direction), Color.Red);
            DebugLog.Instance.LogFrame("Wrapped: "+ MathHelper.ToDegrees(MathHelper.WrapAngle(TargetDirection - Direction)), Color.Red);
            

            Direction += MathHelper.WrapAngle(TargetDirection - Direction) * 0.05f;
        }

        public override void DrawHitbox(SpriteBatch spriteBatch)
        {
            base.DrawHitbox(spriteBatch);
            spriteBatch.DrawLine(Position, 100f, Direction - MathHelper.PiOver2, Color.GreenYellow, 5, 1f);
        }
    }
}
