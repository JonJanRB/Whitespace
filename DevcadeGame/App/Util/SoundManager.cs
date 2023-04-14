using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whitespace.App.Util
{
    internal static class SoundManager
    {
        public static SoundEffect MenuSwipe { get; set; }
        public static SoundEffect MenuSelect { get; set; }
        public static SoundEffect MenuBack { get; set; }
        public static SoundEffect Fling { get; set; }
        public static SoundEffect TimeStop { get; set; }
        public static SoundEffect OrbDestroy { get; set; }
        public static SoundEffect SpikeHit { get; set; }
        public static SoundEffect WhitespaceTouch { get; set; }

        public static Song GameSong { get; set; }
        public static Song MenuSong { get; set; }
    }
}
