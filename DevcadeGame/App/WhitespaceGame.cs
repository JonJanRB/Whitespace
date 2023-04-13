using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Devcade;
using System;
using Whitespace.App.Util;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers.Containers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended;
using System.Collections.Generic;
using MonoGame.Extended.ViewportAdapters;
using static System.Formats.Asn1.AsnWriter;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Whitespace.App
{
    public enum GameState
    {
        Menu,
        Playing
    }

    public class WhitespaceGame : Game
    {
        /// <summary>
        /// The ratio of the devcade's resolution
        /// </summary>
        public readonly Point Ratio = new Point(21, 49);

        //Drawings
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private OrthographicCamera _cam;


        //Textures
        private Texture2D _squareTexture;
        private Texture2D _circleTexture;
        private Texture2D _triangleTexture;
        private SpriteFont _font;

        private Color _bg;
        private Vector2 _xBounds;
        private Vector2 _yBounds;
        private const float _defaultZoom = 0.1f;

        //Physics objects
        private Player _player;


        //Managers
        private PhysicsManager _physMan;
        private ObjectManager _objMan;

        //FSM
        private GameState _gameState;

        private Menu _mainMenu;


        //Sounds
        private SoundEffect _menuMove;
        private Song _testMus;

#if DEBUG
        private KeyboardState _pk;
#endif

        public WhitespaceGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            // Sets up the input library
            Input.Initialize();
            #region Set to Devcade Resolution
#if DEBUG
            _graphics.PreferredBackBufferWidth = 420;
            _graphics.PreferredBackBufferHeight = 980;
            _graphics.ApplyChanges();
#else
			_graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
			_graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
			_graphics.ApplyChanges();
#endif
            #endregion
            ////

            //Set up singeltons
            PhysicsManager.Initialize();
            _physMan = PhysicsManager.IN;


            _bg = new Color(0.1f, 0.1f, 0.1f);

            _gameState = GameState.Menu;

            ////
            base.Initialize();
        }

        /// <summary>
        /// Performs any setup that requires loaded content before the first frame.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            ////

            //Setup camera to work for any window size
            _cam = new OrthographicCamera(
                    new BoxingViewportAdapter(
                        Window, GraphicsDevice,
                        Ratio.X * 10, Ratio.Y * 10));
            _cam.Zoom = _defaultZoom;
            _xBounds = new Vector2(
                _cam.BoundingRectangle.TopLeft.X,
                _cam.BoundingRectangle.BottomRight.X);
            _yBounds = new Vector2(
                _cam.BoundingRectangle.TopLeft.Y,
                _cam.BoundingRectangle.BottomRight.Y);


            //1 pixel texture
            _squareTexture = new Texture2D(GraphicsDevice, 1, 1);
            _squareTexture.SetData(new[] { Color.White });

            _circleTexture = Content.Load<Texture2D>("Filled Circle");
            _triangleTexture = Content.Load<Texture2D>("Filled Triangle");

            _font = Content.Load<SpriteFont>("Comfortaa200");

            DebugLog.Instance.Font = _font;
            DebugLog.Instance.Scale = 0.1f;

            _player = new Player(_squareTexture)
            {
                HitboxRadius = 50f,
                Tint = Color.Blue,
                Scale = new Vector2(100f),
                Position = new Vector2((_xBounds.Y + _xBounds.X) * 0.5f, -1000f)
            };

            //Object manager
            ObjectManager.OrbTexture = _circleTexture;
            ObjectManager.OrbDestroyParticle = _squareTexture;
            ObjectManager.OrbColor = Color.Lime;
            ObjectManager.SpikeTexture = _triangleTexture;
            ObjectManager.SpikeDestroyParticle = _squareTexture;
            ObjectManager.SpikeColor = Color.OrangeRed;

            ObjectManager.Initialize(_cam.BoundingRectangle);
            _objMan = ObjectManager.IN;

            //Menu
            _mainMenu = new Menu(new Vector2(_cam.Center.X, _cam.BoundingRectangle.Y + 800f), _font, 500f);

            //Sound
            _menuMove = Content.Load<SoundEffect>("Whitespace_MenuSelect");
            _testMus = Content.Load<Song>("Test_quotemusic");   

        }

        protected override void Update(GameTime gameTime)
        {
            Input.Update(); // Updates the state of the input library
            //Emergency Exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) ||
                Input.GetButton(1, Input.ArcadeButtons.Menu) &&
                Input.GetButton(2, Input.ArcadeButtons.Menu))
            {
                Exit();
            }
            ////

            switch(_gameState)
            {
                case GameState.Menu:
                    UpdateMenu(gameTime);
                    break;
                case GameState.Playing:
                    UpdateGame(gameTime);
                    break;
            }
            
            ////
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            ////

            switch (_gameState)
            {
                case GameState.Menu:
                    DrawMenu(gameTime);
                    break;
                case GameState.Playing:
                    DrawGame(gameTime);
                    break;
            }

            ////
            base.Draw(gameTime);
        }

#if DEBUG

        private Vector2 GetKeyboardStickDirection()
        {
            Vector2 direction = Vector2.Zero;
            if (Keyboard.GetState().IsKeyDown(Keys.W)) direction.Y -= 1;
            if (Keyboard.GetState().IsKeyDown(Keys.S)) direction.Y += 1;
            if (Keyboard.GetState().IsKeyDown(Keys.D)) direction.X += 1;
            if (Keyboard.GetState().IsKeyDown(Keys.A)) direction.X -= 1;
            return direction;
        }

#endif

        //Game states
        private void UpdateGame(GameTime gameTime)
        {
            //Direction of movement
            Vector2 stickDirection = Input.GetStick(1);
#if DEBUG
            stickDirection = GetKeyboardStickDirection();
#endif
            stickDirection.Normalize();
            //Set target direction only if in a nuetral position
            if (!stickDirection.IsNaN())
            {
                _player.TargetDirection = stickDirection.ToAngle() - MathHelper.PiOver2;
            }
            else
            {
                _player.TargetDirection = _player.Direction;
            }

            //Change game speed based on button press
            float targetGameSpeed = 1f;
            if (Input.GetButton(1, Input.ArcadeButtons.A1))
            {
                targetGameSpeed = 0.02f;
                _cam.ZoomToWorldPoint(
                    _player.Position + _player.DirectionVector * 1000f,
                    _defaultZoom * 2f, 0.1f, _xBounds);
            }
#if DEBUG
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                targetGameSpeed = 0.02f;
                _cam.ZoomToWorldPoint(
                    _player.Position + _player.DirectionVector * 1000f,
                    _defaultZoom * 2f, 0.1f, _xBounds);
            }
#endif
            //Check let go
            if (Input.GetButtonUp(1, Input.ArcadeButtons.A1))
            {
                _player.Velocity = _player.DirectionVector * 10000f;
            }
#if DEBUG
            if (Keyboard.GetState().IsKeyUp(Keys.Space) && _pk.IsKeyDown(Keys.Space))
            {
                _player.Velocity = _player.DirectionVector * 10000f;
            }
#endif
            //Default zoom to
            _cam.ZoomToWorldPoint(_player.Position, _defaultZoom, 0.1f, _xBounds);

            //bounce off screen edge
            if (_player.Position.X < _xBounds.X)
            {
                _player.Velocity = new Vector2(MathF.Abs(_player.Velocity.X), _player.Velocity.Y);
            }
            else if (_player.Position.X > _xBounds.Y)
            {
                _player.Velocity = new Vector2(-MathF.Abs(_player.Velocity.X), _player.Velocity.Y);
            }


            //Update physics manager
            _physMan.Update(gameTime, targetGameSpeed, 0.1f);



            _player.Update();



            _objMan.Update(
                _player,
                _cam.BoundingRectangle);


#if DEBUG
            _pk = Keyboard.GetState();
#endif
            DebugLog.Instance.LogFrame(_player.Position.X.ToString("0000") + ", " + _player.Position.Y.ToString("0000"));

        }

        public void DrawGame(GameTime gameTime)
        {
            //Camera matrix
            Matrix transformation = _cam.GetViewMatrix();
            _spriteBatch.Begin(blendState: BlendState.AlphaBlend, transformMatrix: transformation);

            //Draw background of canvas
            _spriteBatch.Draw(_squareTexture, _cam.BoundingRectangle.ToRectangle(), _bg);

            _objMan.Draw(_spriteBatch);
            //_objMan.DrawHitboxes(_spriteBatch);

            _player.Draw(_spriteBatch);
            _player.DrawHitbox(_spriteBatch);


            _spriteBatch.End();

            //Draw debug log
            _spriteBatch.Begin();
            DebugLog.Instance.Draw(
                _spriteBatch,
                _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight);
            _spriteBatch.End();
        }

        private void UpdateMenu(GameTime gameTime)
        {

#if DEBUG
            if (_pk.IsKeyUp(Keys.S) && Keyboard.GetState().IsKeyDown(Keys.S))
            {
                _mainMenu.Index++;
                _menuMove.Play();
            }
            if(_pk.IsKeyUp(Keys.W) && Keyboard.GetState().IsKeyDown(Keys.W))
            {
                _mainMenu.Index--;
                _menuMove.Play();
            }
            if (_pk.IsKeyUp(Keys.Space) && Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                switch (_mainMenu.Index)
                {
                    case 0://Play
                        _gameState = GameState.Playing;
                        MediaPlayer.IsRepeating = true;
                        MediaPlayer.Play(_testMus);
                        break;
                    case 1://How to play

                        break;
                    case 2://Options

                        break;
                    case 3://Quit
                        Exit();
                        break;

                }
            }
#endif
            if (Input.GetButtonDown(1, Input.ArcadeButtons.StickDown))
            {
                _mainMenu.Index++;

            }
            if (Input.GetButtonDown(1, Input.ArcadeButtons.StickUp))
            {
                _mainMenu.Index--;
            }
            if (Input.GetButtonDown(1, Input.ArcadeButtons.A1))
            {
                switch (_mainMenu.Index)
                {
                    case 0://Play
                        _gameState = GameState.Playing;
                        break;
                    case 1://How to play

                        break;
                    case 2://Options

                        break;
                    case 3://Quit
                        Exit();
                        break;

                }
            }




#if DEBUG
            _pk = Keyboard.GetState();
#endif
        }

        public void DrawMenu(GameTime gameTime)
        {
            //Camera matrix
            Matrix transformation = _cam.GetViewMatrix();
            _spriteBatch.Begin(blendState: BlendState.AlphaBlend, transformMatrix: transformation);

            //Draw background of canvas
            _spriteBatch.Draw(_squareTexture, _cam.BoundingRectangle.ToRectangle(), Color.White);

            _mainMenu.Draw(_spriteBatch, gameTime);


            _spriteBatch.End();
        }
    }
}