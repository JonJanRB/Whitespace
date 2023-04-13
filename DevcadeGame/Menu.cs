using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Whitespace
{
    internal class Menu
    {
        private class MenuPage
        {
            public string Name { get; set; }
            public bool Selected { get; set; }
            public List<MenuPage> Children { get; set; }
            public Vector2 _currentScale;
            public Color _currentColor;

            public MenuPage(string name, Vector2 initialScale, Color initialColor)
            {
                Name = name;
                Selected = false;
                Children = new();
                _currentScale = initialScale;
                _currentColor = initialColor;
            }

            public void Draw(SpriteBatch sb, Vector2 pos, SpriteFont font, Vector2 scale, Color color)
            {
                _currentScale += (_currentScale - scale) * 0.2f;
                _currentColor = new Color(
                    _currentColor.ToVector4() + 
                    (_currentColor.ToVector4() - color.ToVector4()) * 0.2f);

                Vector2 size = font.MeasureString(Name);
                sb.DrawString(font, Name, pos, _currentColor, 0f, size * 0.5f, _currentScale, SpriteEffects.None, 0f);
            }
        }

        private MenuPage _mainPage;
        public SpriteFont Font { get; set; }
        public float Spacing { get; set; }
        public Vector2 InactiveScale { get; set; }
        public Vector2 HoverScale { get; set; }
        public Color InactiveColor { get; set; }
        public Color HoverColor { get; set; }

    }
}
