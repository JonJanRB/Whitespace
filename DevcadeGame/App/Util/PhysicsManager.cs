using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whitespace.App.Util
{
    internal class PhysicsManager
    {
        #region Singleton Design

        /// <summary>
        /// The instance of this DebugLog
        /// </summary>
        public static PhysicsManager IN { get; private set; }

        public static void Initialize()
        {
            IN = new PhysicsManager();
        }

        /// <summary>
        /// Creates a new player manager
        /// </summary>
        private PhysicsManager()
        {

        }

        #endregion

        public float Friction { get; set; } = 0.9f;

        public float GameSpeed { get; private set; }

        public float ElapsedTime { get; private set; }

        public Vector2 Gravity { get; set; } = new Vector2(0f, 500000f);

        /// <summary>
        /// The thing to multiply everything to get the amount they should move this frame
        /// </summary>
        public float TimeSpeed { get; private set; }

        public void Update(GameTime gameTime, float targetGameSpeed, float easing)
        {
            ElapsedTime = gameTime.GetElapsedSeconds();
            GameSpeed += (targetGameSpeed - GameSpeed) * easing;
            TimeSpeed = GameSpeed * ElapsedTime;
        }
    }
}
