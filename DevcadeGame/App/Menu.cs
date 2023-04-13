using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whitespace.App
{
    internal class Menu
    {
        private class MenuEntry
        {
            public string Name { get; set; }
            public float CurrentScale { get; set; }
            public float TargetScale { get; set; }

            public MenuEntry(string name)
            {
                Name = name;
                CurrentScale = 0.5f;
                TargetScale = 0.5f;
            }
        }

        public int Index
        {
            get => _index;
            set
            {
                _entries[Index].TargetScale = 0.5f;
                if(value < 0)
                {
                    _index = 0;
                }
                else if(value >= _entries.Count)
                {
                    _index = _entries.Count - 1;
                }
                else
                {
                    _index = value;
                }
                _entries[Index].TargetScale = 0.75f;
            }
        }
        private int _index;

        private List<MenuEntry> _entries;

        private SpriteFont _font;

        private Vector2 _position;
        private float _lineSpacing;

        public Menu(Vector2 top, SpriteFont font, float lineSpacing)
        {
            _entries = new()
            {
                new MenuEntry("Play"),
                new MenuEntry("How to Play"),
                new MenuEntry("Options"),
                new MenuEntry("Quit")
            };
            _position = top;
            _font = font;
            _lineSpacing = lineSpacing;
            Index = 0;
        }

        public void Draw(SpriteBatch sb, GameTime gt)
        {
            //Title
            Vector2 titleOrigin = _font.MeasureString("Whitespace") * 0.5f;
            sb.DrawString(_font, "Whitespace", _position, Color.DarkGray, 0f, titleOrigin, 1f, SpriteEffects.None, 0f);

            for(int i = 0; i < _entries.Count; i++)
            {
                MenuEntry entry = _entries[i];

                Vector2 origin = _font.MeasureString(entry.Name) * 0.5f;

                entry.CurrentScale += (entry.TargetScale - entry.CurrentScale) * 20f * gt.GetElapsedSeconds();

                sb.DrawString(
                    _font,
                    entry.Name,
                    _position + new Vector2(0f, i * _lineSpacing + 1000f),
                    Color.Black,
                    0f,
                    origin,
                    entry.CurrentScale,
                    SpriteEffects.None,
                    0f);
            }

            
        }

    }
}
