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
        public static readonly FastRandom RNG = new FastRandom();

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
            //Capacity of objects on screen at once
            Orbs = new Orb[10];
            Spikes = new Spike[7];

            

            Reset(currentFrame);//Populate objects
        }

        #endregion

        public static Texture2D OrbTexture { get; set; }
        public static Texture2D OrbDestroyParticle { get; set; }
        public static Color OrbColor { get; set; } = Color.Lime;
        public static Texture2D SpikeTexture { get; set; }
        public static Texture2D SpikeDestroyParticle { get; set; }
        public static Color SpikeColor { get; set; } = Color.OrangeRed;



        public Orb[] Orbs { get; private set; }
        public Spike[] Spikes { get; private set; }

        public void Update(Player player, float waveTop, RectangleF currentFrame)
        {
            RectangleF nextFrame = currentFrame;
            nextFrame.Y -= currentFrame.Height;

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
                if(orb.Position.Y > waveTop)
                {
                    RefreshOrb(orb, nextFrame);
                }
                orb.Update();
            }
            foreach(Spike spike in Spikes)
            {
                if (player.Intersects(spike.Collider) && spike.Enabled)
                {
                    spike.Destroy(player.Velocity);
                    player.Flings--;
                    player.Velocity = -player.Velocity * 0.5f;
                }
                if (spike.Position.Y > waveTop)
                {
                    RefreshSpike(spike, nextFrame);
                }
                spike.Update();
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (Orb orb in Orbs)
            {
                orb.Draw(sb);
            }
            foreach (Spike spike in Spikes)
            {
                spike.Draw(sb);
            }
        }

        public void DrawHitboxes(SpriteBatch sb)
        {
            foreach (Orb orb in Orbs)
            {
                orb.DrawHitbox(sb);
            }
            foreach (Spike spike in Spikes)
            {
                spike.DrawHitbox(sb);
            }
        }

        /// <summary>
        /// Resets everything
        /// </summary>
        public void Reset(RectangleF startingFrame)
        {
            CircleF startingZone = new CircleF(Vector2.Zero, 1000f);

            for (int i = 0; i < Orbs.Length; i++)
            {
                //Make sure not inside starting zone
                Orb tryOrb = new Orb();
                tryOrb = RefreshOrb(tryOrb, startingFrame);
                while(tryOrb.Intersects(startingZone))
                    tryOrb = RefreshOrb(tryOrb, startingFrame);
                Orbs[i] = tryOrb;
            }

            for (int i = 0; i < Spikes.Length; i++)
            {
                Spike trySpike = new Spike();
                trySpike = RefreshSpike(trySpike, startingFrame);
                while (trySpike.Intersects(startingZone))
                    trySpike = RefreshSpike(trySpike, startingFrame);
                Spikes[i] = trySpike;
            }
        }


        /// <summary>
        /// random spike val in given frame
        /// </summary>
        /// <param name="spike"></param>
        /// <param name="frame"></param>
        /// <returns></returns>
        public Spike RefreshSpike(Spike spike, RectangleF frame)
        {
            spike.Position = new Vector2(
                RNG.NextSingle(frame.X, frame.Right),
                RNG.NextSingle(frame.Y, frame.Bottom));
            spike.Scale = new Vector2(RNG.NextSingle(200f, 400f));
            spike.Rotation = RNG.NextAngle();//That's nice
            spike.Enabled = true;
            //DebugLog.Instance.LogPersistant("Spike refreshed", Color.OrangeRed, 5f);
            return spike;
        }

        public Orb RefreshOrb(Orb orb, RectangleF frame)
        {
            orb.Position = new Vector2(
                RNG.NextSingle(frame.X, frame.Right),
                RNG.NextSingle(frame.Y, frame.Bottom));
            orb.Scale = new Vector2(RNG.NextSingle(75f, 150f));
            orb.Enabled = true;
            //DebugLog.Instance.LogPersistant("Orb refreshed", Color.GreenYellow, 5f);
            return orb;
        }
    }
}
