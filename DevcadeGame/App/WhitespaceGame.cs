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
        private SpriteFont _lightFont;
        private SpriteFont _boldFont;

        private Color _bg;
        private Vector2 _xBounds;
        private const float _defaultZoom = 0.1f;

        //Physics objects
        private Player _player;


        //Managers
        private PhysicsManager _physMan;
        private ObjectManager _objMan;

        //FSM
        private GameState _gameState;

        private Menu _mainMenu;


        //Wav
        private Wave _wave;



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

            //Sound manager
            SoundManager.MenuSwipe = Content.Load<SoundEffect>("Sound/Swipe");
            SoundManager.MenuBack = Content.Load<SoundEffect>("Sound/Back");
            SoundManager.MenuSelect = Content.Load<SoundEffect>("Sound/Select");
            SoundManager.OrbDestroy = Content.Load<SoundEffect>("Sound/Orb");
            SoundManager.SpikeHit = Content.Load<SoundEffect>("Sound/Spike");
            SoundManager.TimeStop = Content.Load<SoundEffect>("Sound/Time");
            SoundManager.Fling = Content.Load<SoundEffect>("Sound/Fling");
            SoundManager.WhitespaceTouch = Content.Load<SoundEffect>("Sound/Death");

            //Setup camera to work for any window size
            _cam = new OrthographicCamera(
                    new BoxingViewportAdapter(
                        Window, GraphicsDevice,
                        Ratio.X * 10, Ratio.Y * 10));
            _cam.Zoom = _defaultZoom;
            _xBounds = new Vector2(
                _cam.BoundingRectangle.TopLeft.X,
                _cam.BoundingRectangle.BottomRight.X);


            //1 pixel texture
            _squareTexture = new Texture2D(GraphicsDevice, 1, 1);
            _squareTexture.SetData(new[] { Color.White });

            _circleTexture = Content.Load<Texture2D>("Filled Circle");
            _triangleTexture = Content.Load<Texture2D>("Filled Triangle");

            _lightFont = Content.Load<SpriteFont>("Comfortaa200");
            _boldFont = Content.Load<SpriteFont>("Comfortaa200Bold");

            DebugLog.Instance.Font = _lightFont;
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
            _mainMenu = new Menu(new Vector2(_cam.Center.X, _cam.BoundingRectangle.Y + 800f), _lightFont, _boldFont, 500f, new Vector2(_cam.Center.X, _cam.BoundingRectangle.Bottom));

            //Wave
            _wave = new Wave(_squareTexture, _cam.BoundingRectangle);

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
#if DEBUG
            _pk = Keyboard.GetState();
#endif
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


        //Game states
        private void UpdateGame(GameTime gameTime)
        {
            //Direction of movement
            Vector2 stickDirection = GetStickDirection();
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
            if(ButtonJustPressed())
            {
                SoundManager.TimeStop.Play();
            }
            if(ButtonPressed())
            {
                targetGameSpeed = 0.02f;
                _cam.ZoomToWorldPoint(
                    _player.Position + _player.DirectionVector * 1000f,
                    _defaultZoom * 2f, 0.1f, _xBounds);
            }
            //Check let go
            if(ButtonJustReleased())
            {
                _player.Velocity = _player.DirectionVector * 10000f;
                SoundManager.Fling.Play();
            }
            //Default zoom to
            _cam.ZoomToWorldPoint(_player.Position, _defaultZoom, 0.1f, _xBounds);


            //bounce off screen edge
            if (_player.Position.X < _xBounds.X)
            {
                _player.Velocity = new Vector2(MathF.Abs(_player.Velocity.X), _player.Velocity.Y);
                SoundManager.MenuBack.Play();
            }
            else if (_player.Position.X > _xBounds.Y)
            {
                _player.Velocity = new Vector2(-MathF.Abs(_player.Velocity.X), _player.Velocity.Y);
                SoundManager.MenuBack.Play();
            }


            //Update physics manager
            _physMan.Update(gameTime, targetGameSpeed, 0.1f);



            _player.Update();


            _wave.Update(_player);


            _objMan.Update(
                _player,
                _cam.BoundingRectangle);



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


            _wave.Draw(_spriteBatch);

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
            if(!_mainMenu.ShowingSubMenu)
            {
                if(StickDownJust())
                {
                    _mainMenu.Index++;
                }
                if(StickUpJust())
                {
                    _mainMenu.Index--;
                }
                if(ButtonJustPressed())
                {
                    SoundManager.MenuSelect.Play();
                    switch (_mainMenu.Index)
                    {
                        case 0://Play
                            _gameState = GameState.Playing;
                            break;
                        case 1://How to play
                            _mainMenu.GoToTutorial();
                            break;
                        case 2://Credits
                            _mainMenu.GoToCredits();
                            break;
                        case 3://Quit
                            Exit();
                            break;

                    }
                }

            }
            else//Not the main menu
            {
                if(ButtonJustPressed())
                {
                    _mainMenu.GoToMainMenu();
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

        /// <summary>
        /// Im sick of being dumb, who cares if this is less efficient than using the #if DEBUG
        /// </summary>
        /// <returns></returns>
        private bool ButtonJustPressed()
        {
            //Called when any button is pressed
            return (_pk.IsKeyUp(Keys.Space) && Keyboard.GetState().IsKeyDown(Keys.Space))
                || Input.GetButtonDown(1, Input.ArcadeButtons.A1)
                || Input.GetButtonDown(1, Input.ArcadeButtons.A2)
                || Input.GetButtonDown(1, Input.ArcadeButtons.A3)
                || Input.GetButtonDown(1, Input.ArcadeButtons.B4)
                || Input.GetButtonDown(1, Input.ArcadeButtons.B1)
                || Input.GetButtonDown(1, Input.ArcadeButtons.B2)
                || Input.GetButtonDown(1, Input.ArcadeButtons.B3)
                || Input.GetButtonDown(1, Input.ArcadeButtons.B4)
                || Input.GetButtonDown(2, Input.ArcadeButtons.A1)
                || Input.GetButtonDown(2, Input.ArcadeButtons.A2)
                || Input.GetButtonDown(2, Input.ArcadeButtons.A3)
                || Input.GetButtonDown(2, Input.ArcadeButtons.B4)
                || Input.GetButtonDown(2, Input.ArcadeButtons.B1)
                || Input.GetButtonDown(2, Input.ArcadeButtons.B2)
                || Input.GetButtonDown(2, Input.ArcadeButtons.B3)
                || Input.GetButtonDown(2, Input.ArcadeButtons.B4);
        }

        private bool ButtonJustReleased()
        {
            //Called when any button is pressed
            return (_pk.IsKeyDown(Keys.Space) && Keyboard.GetState().IsKeyUp(Keys.Space))
                || Input.GetButtonUp(1, Input.ArcadeButtons.A1)
                || Input.GetButtonUp(1, Input.ArcadeButtons.A2)
                || Input.GetButtonUp(1, Input.ArcadeButtons.A3)
                || Input.GetButtonUp(1, Input.ArcadeButtons.B4)
                || Input.GetButtonUp(1, Input.ArcadeButtons.B1)
                || Input.GetButtonUp(1, Input.ArcadeButtons.B2)
                || Input.GetButtonUp(1, Input.ArcadeButtons.B3)
                || Input.GetButtonUp(1, Input.ArcadeButtons.B4)
                || Input.GetButtonUp(2, Input.ArcadeButtons.A1)
                || Input.GetButtonUp(2, Input.ArcadeButtons.A2)
                || Input.GetButtonUp(2, Input.ArcadeButtons.A3)
                || Input.GetButtonUp(2, Input.ArcadeButtons.B4)
                || Input.GetButtonUp(2, Input.ArcadeButtons.B1)
                || Input.GetButtonUp(2, Input.ArcadeButtons.B2)
                || Input.GetButtonUp(2, Input.ArcadeButtons.B3)
                || Input.GetButtonUp(2, Input.ArcadeButtons.B4);
        }

        private bool ButtonPressed()
        {
            //Called when any button is pressed
            return Keyboard.GetState().IsKeyDown(Keys.Space)
                || Input.GetButton(1, Input.ArcadeButtons.A1)
                || Input.GetButton(1, Input.ArcadeButtons.A2)
                || Input.GetButton(1, Input.ArcadeButtons.A3)
                || Input.GetButton(1, Input.ArcadeButtons.B4)
                || Input.GetButton(1, Input.ArcadeButtons.B1)
                || Input.GetButton(1, Input.ArcadeButtons.B2)
                || Input.GetButton(1, Input.ArcadeButtons.B3)
                || Input.GetButton(1, Input.ArcadeButtons.B4)
                || Input.GetButton(2, Input.ArcadeButtons.A1)
                || Input.GetButton(2, Input.ArcadeButtons.A2)
                || Input.GetButton(2, Input.ArcadeButtons.A3)
                || Input.GetButton(2, Input.ArcadeButtons.B4)
                || Input.GetButton(2, Input.ArcadeButtons.B1)
                || Input.GetButton(2, Input.ArcadeButtons.B2)
                || Input.GetButton(2, Input.ArcadeButtons.B3)
                || Input.GetButton(2, Input.ArcadeButtons.B4);
        }

        private Vector2 GetStickDirection()
        {
            Vector2 direction = Vector2.Zero;
#if DEBUG
            if (Keyboard.GetState().IsKeyDown(Keys.W)) direction.Y -= 1;
            if (Keyboard.GetState().IsKeyDown(Keys.S)) direction.Y += 1;
            if (Keyboard.GetState().IsKeyDown(Keys.D)) direction.X += 1;
            if (Keyboard.GetState().IsKeyDown(Keys.A)) direction.X -= 1;
#else
            direction = Input.GetStick(1);
            direction = new Vector2(direction.X, -direction.Y);
#endif
            return direction;
        }

        private bool StickDownJust()
        {
            //Called when any button is pressed
            return (_pk.IsKeyUp(Keys.S) && Keyboard.GetState().IsKeyDown(Keys.S))
                || Input.GetButtonDown(1, Input.ArcadeButtons.StickDown);
        }

        private bool StickUpJust()
        {
            //Called when any button is pressed
            return (_pk.IsKeyUp(Keys.W) && Keyboard.GetState().IsKeyDown(Keys.W))
                || Input.GetButtonDown(1, Input.ArcadeButtons.StickUp);
        }

    }
}