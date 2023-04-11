using Microsoft.Xna.Framework;
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

        public float Acceleration { get; set; }

        public float TimeSpeed { get; set; }

        public void Update(GameTime gameTime)
        {
            TimeSpeed += Acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
            TimeSpeed *= Friction;
        }
    }
}
