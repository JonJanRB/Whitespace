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
                    SoundManager.MenuSwipe.Play();
                }
                _entries[Index].TargetScale = 0.75f;
            }
        }
        private int _index;

        public bool ShowingSubMenu { get; private set; }
        private bool _drawCredits;
        private bool _drawTutorial;

        public bool Transitioning { get; private set; }

        private List<MenuEntry> _entries;
        private List<string> _credits;
        private List<string> _instructions;

        private SpriteFont _lightFont;
        private SpriteFont _boldFont;

        private Vector2 _position;
        private Vector2 _targetPosition;
        private Vector2 _mainPos;
        private Vector2 _subPos;
        private float _lineSpacing;

        public string ScoreLabel { get; set; } = "";
        public string ScoreTitle { get; set; } = "";

        public Menu(Vector2 top, SpriteFont lightFont, SpriteFont boldFont, float lineSpacing, Vector2 subPosition)
        {
            _entries = new()
            {
                new MenuEntry("Play"),
                new MenuEntry("How to Play"),
                new MenuEntry("Credits"),
                new MenuEntry("Quit")
            };
            _credits = new()
            {
                "(Press any button to go back)",
                "---",
                "Created by ReBuff",
                "Sounds made using BFXR",
                "Textures made in Google Drawings",
                "Font: Comfortaa by Johan Aakerlund",
                "Made using MonoGame and",
                "MonoGame.Extended",
                "---",
                "Made in under a week for the 2023 Devcade Jam",
                "Updated 5/9/2023 with balancing changes"
            };
            _instructions = new()
            {
                "(Press [SPACE] to go back)",
                "---",
                "Press and hold [SPACE] to stop time",
                "Aim with [WASD] to where you want to fling",
                "",
                "Let go of [SPACE] to fling",
                "yourself in that direction",
                "",
                "Smash green orbs to gain flings",
                "",
                "Avoid touching red spikes",
                "(they take away flings)",
                "",
                "Touching the whitespace at the",
                "bottom is game over :("
            };
            _position = top;
            _targetPosition = top;
            _mainPos = top;
            _subPos = subPosition;
            _lightFont = lightFont;
            _boldFont = boldFont;
            _lineSpacing = lineSpacing;
            Index = 0;
        }

        public void Draw(SpriteBatch sb, GameTime gt)
        {
            _position += (_targetPosition - _position) * 5f * gt.GetElapsedSeconds();

            if(_position.Y <= _mainPos.Y + 100f)
            {
                Transitioning = false;
            }

            //Title
            Vector2 titleOrigin = _lightFont.MeasureString("Whitespace") * 0.5f;
            sb.DrawString(_lightFont, "Whitespace", _position, Color.DarkGray, 0f, titleOrigin, 1f, SpriteEffects.None, 0f);

            //Main manu
            for (int i = 0; i < _entries.Count; i++)
            {
                MenuEntry entry = _entries[i];

                Vector2 origin = _lightFont.MeasureString(entry.Name) * 0.5f;

                entry.CurrentScale += (entry.TargetScale - entry.CurrentScale) * 20f * gt.GetElapsedSeconds();

                sb.DrawString(
                    _lightFont,
                    entry.Name,
                    _position + new Vector2(0f, i * _lineSpacing + 1000f),
                    Color.Black,
                    0f,
                    origin,
                    entry.CurrentScale,
                    SpriteEffects.None,
                    0f);
            }

            Vector2 scoreTitleOrigin = _lightFont.MeasureString(ScoreTitle) * 0.5f;

            sb.DrawString(
                _boldFont,
                ScoreTitle,
                _position + new Vector2(0f, 5 * _lineSpacing + 600f),
                Color.Green,
                0f,
                scoreTitleOrigin,
                0.4f,
                SpriteEffects.None,
                0f);


            Vector2 scoreOrigin = _lightFont.MeasureString(ScoreLabel) * 0.5f;

            sb.DrawString(
                _boldFont,
                ScoreLabel,
                _position + new Vector2(0f, 5 * _lineSpacing + 1000f),
                Color.LawnGreen,
                0f,
                scoreOrigin,
                0.8f,
                SpriteEffects.None,
                0f);

            if (_drawCredits)
            {
                for (int i = 0; i < _credits.Count; i++)
                {
                    string entry = _credits[i];

                    Vector2 origin = _boldFont.MeasureString(entry) * 0.5f;

                    sb.DrawString(
                        _boldFont,
                        entry,
                        _position + 1.7f * _subPos + new Vector2(0f, i * _lineSpacing * 0.5f + 1000f),
                        Color.Black,
                        0f,
                        origin,
                        0.3f,
                        SpriteEffects.None,
                        0f);
                }
            }

            if (_drawTutorial)
            {
                for (int i = 0; i < _instructions.Count; i++)
                {
                    string entry = _instructions[i];

                    Vector2 origin = _boldFont.MeasureString(entry) * 0.5f;

                    sb.DrawString(
                        _boldFont,
                        entry,
                        _position + 1.6f * _subPos + new Vector2(0f, i * _lineSpacing * 0.5f + 1000f),
                        Color.Black,
                        0f,
                        origin,
                        0.3f,
                        SpriteEffects.None,
                        0f);
                }
            }


        }

        public void TransitionToMenu()
        {
            _position = _mainPos + 2 * _subPos;
            Transitioning = true;
            _targetPosition = _mainPos;
        }

        public void GoToMainMenu()
        {
            ShowingSubMenu = false;
            _targetPosition = _mainPos;
            SoundManager.MenuBack.Play();
        }

        public void GoToCredits()
        {
            _drawCredits = true;
            _drawTutorial = false;
            ShowingSubMenu = true;
            _targetPosition = _mainPos - 2 * _subPos;
        }

        public void GoToTutorial()
        {
            _drawTutorial = true;
            _drawCredits = false;
            ShowingSubMenu = true;
            _targetPosition = _mainPos - 2 * _subPos;
        }

    }
}
