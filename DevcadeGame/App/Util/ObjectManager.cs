using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whitespace.App.Util
{
    /// <summary>
    /// Singelton that has orbs and spikes
    /// </summary>
    internal class ObjectManager
    {
        #region Singleton Design

        /// <summary>
        /// The instance of this DebugLog
        /// </summary>
        public static ObjectManager IN { get; private set; }

        public static void Initialize(RectangleF currentFrame)
        {
            IN = new ObjectManager(currentFrame);
        }

        /// <summary>
        /// Creates a new object manager
        /// </summary>
        private ObjectManager(RectangleF currentFrame)
        {
            //Capacity of 30 orbs on screen at once
            Orbs = new Orb[30];
            RNG = new FastRandom();

            for (int i = 0; i < Orbs.Length; i++)
            {
                Orbs[i] = new Orb();
                Orbs[i].Position = new Vector2(
                    RNG.NextSingle(currentFrame.X, currentFrame.Right),
                    RNG.NextSingle(currentFrame.Y, currentFrame.Bottom));//Range from top to top+100
                Orbs[i].Enabled = true;
            }

            Reset();//Populate objects
        }

        #endregion

        public static Texture2D OrbTexture { get; set; }
        public static Texture2D OrbDestroyParticle { get; set; }
        public static Color OrbColor { get; set; } = Color.Lime;
        public static Texture2D SpikeTexture { get; set; }
        public static Texture2D SpikeDestroyParticle { get; set; }
        public static Color SpikeColor { get; set; } = Color.OrangeRed;


        public FastRandom RNG { get; }

        public Orb[] Orbs { get; private set; }

        public void Update(Player player, RectangleF currentFrame)
        {
            foreach (Orb orb in Orbs)
            {
                if (player.Intersects(orb.Collider) && orb.Enabled)
                {
                    orb.Destroy(player.Velocity);
                    player.Flings++;
                    player.Velocity =
                        new Vector2(-player.Velocity.X,
                        -MathF.Abs(player.Velocity.Y) + -1000f);
                }
                if(orb.Position.Y > currentFrame.Bottom)
                {
                    RefreshObject(orb, currentFrame);
                }
                orb.Update();
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (Orb orb in Orbs)
            {
                orb.Draw(sb);
            }
        }

        public void DrawHitboxes(SpriteBatch sb)
        {
            foreach (Orb orb in Orbs)
            {
                orb.DrawHitbox(sb);
            }
        }

        /// <summary>
        /// Resets everything
        /// </summary>
        public void Reset()
        {
            
        }

        public void RefreshObject(PhysicsObject phys, RectangleF currentFrame)
        {
            phys.Position =
                new Vector2(
                    RNG.NextSingle(currentFrame.X, currentFrame.Right),
                    RNG.NextSingle(currentFrame.Y - 100f, currentFrame.Y));//Range from top to top+100
            phys.Enabled = true;

            DebugLog.Instance.LogPersistant("Object refreshed", Color.Purple, 5f);
        }
    }
}
