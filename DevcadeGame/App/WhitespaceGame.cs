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
        private Texture2D _arrowTexture;
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
        private float _menuLerp;

        //Wav
        private Wave _wave;

        //Begining
        private bool _newGame;



        private KeyboardState _pk;


        private double _beginTime;


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
            _arrowTexture = Content.Load<Texture2D>("Arrow");

            _lightFont = Content.Load<SpriteFont>("Comfortaa200");
            _boldFont = Content.Load<SpriteFont>("Comfortaa200Bold");

            DebugLog.Instance.Font = _lightFont;
            DebugLog.Instance.Scale = 0.1f;

            _player = new Player(_squareTexture, _arrowTexture, _boldFont)
            {
                HitboxRadius = 50f,
                Tint = Color.Blue,
                Scale = new Vector2(100f),
                Position = Vector2.Zero
            };

            //Object manager
            ObjectManager.OrbTexture = _circleTexture;
            ObjectManager.OrbDestroyParticle = _squareTexture;
            ObjectManager.OrbColor = Color.Lime;
            ObjectManager.SpikeTexture = _triangleTexture;
            ObjectManager.SpikeDestroyParticle = _triangleTexture;
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

            //DEBUG
            //DebugLog.Instance.LogFrame(_objMan.Spikes[0].Scale, Color.Red);

            ////

            _pk = Keyboard.GetState();

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

            //Draw debug log
            _spriteBatch.Begin();
            DebugLog.Instance.Draw(
                _spriteBatch,
                _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight);
            _spriteBatch.End();

            ////
            base.Draw(gameTime);
        }


        //Game states
        private void UpdateGame(GameTime gameTime)
        {
            if(_player.IsAlive)
            {
                //Change game speed based on button press
                float targetGameSpeed = 1f;
                if (ButtonJustPressed() && !_newGame)
                {
                    SoundManager.TimeStop.Play();
                }
                if (ButtonPressed())
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
                        if (!float.IsNaN(_player.TargetDirection))
                        {
                            if(_newGame)
                            {
                                _newGame = false;
                                _beginTime = gameTime.TotalGameTime.TotalSeconds;
                            }
                            _player.TargetDirection = _player.Direction;
                        }
                    }
                    

                    targetGameSpeed = 0.02f;
                    _cam.ZoomToWorldPoint(
                        _player.Position + _player.DirectionVector * 3000f,
                        _defaultZoom * 1.5f, 0.1f, _xBounds);
                    _player.TargetArrowScale = 250f;
                }
                //Check let go
                if (ButtonJustReleased())
                {
                    //If not new game
                    if(!_newGame)
                    {
                        //If able to fling
                        if (_player.Flings != 0)
                        {
                            _player.Velocity = _player.DirectionVector * 10000f;
                            SoundManager.Fling.Play();
                            _player.Flings--;

                        }
                        else//If not
                        {
                            SoundManager.SpikeHit.Play();
                        }
                        _player.TargetArrowScale = 0f;
                    }
                    
                }
                


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

                if(_newGame)
                {
                    targetGameSpeed = 0f;
                }

                //Update physics manager
                _physMan.Update(gameTime, targetGameSpeed, 0.1f);



                
            }
            else//If DEAD
            {
                //Update physics manager to stop time completely
                _physMan.Update(gameTime, 1f, 1f);
                //Stop horizontsal movement
                _player.Velocity = new Vector2(0f, _player.Velocity.Y);

                //Check if far enough under
                if(_cam.BoundingRectangle.Top > _wave.Top)
                {
                    TransitionToMenu(gameTime);
                }
            }

            //Default zoom to
            _cam.ZoomToWorldPoint(_player.Position, _defaultZoom, 0.1f, _xBounds);

            _player.Update();


            _wave.Update(_player);


            _objMan.Update(_player, _wave.Top, _cam.BoundingRectangle);


            //DebugLog.Instance.LogFrame(_player.Position.X.ToString("0000") + ", " + _player.Position.Y.ToString("0000"));

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
            //_player.DrawHitbox(_spriteBatch);


            _wave.Draw(_spriteBatch);

            
            _spriteBatch.End();

            
        }

        private void UpdateMenu(GameTime gameTime)
        {
            if(!_mainMenu.Transitioning)
            {
                if (!_mainMenu.ShowingSubMenu)
                {
                    if (StickDownJust())
                    {
                        _mainMenu.Index++;
                    }
                    if (StickUpJust())
                    {
                        _mainMenu.Index--;
                    }
                    if (ButtonJustPressed())
                    {
                        switch (_mainMenu.Index)
                        {
                            case 0://Play
                                ResetGame();
                                _gameState = GameState.Playing;
                                SoundManager.TimeStop.Play();
                                break;
                            case 1://How to play
                                _mainMenu.GoToTutorial();
                                SoundManager.MenuSelect.Play();
                                break;
                            case 2://Credits
                                _mainMenu.GoToCredits();
                                SoundManager.MenuSelect.Play();
                                break;
                            case 3://Quit
                                Exit();
                                break;

                        }
                    }

                }
                else//Not the main menu
                {
                    if (ButtonJustPressed())
                    {
                        _mainMenu.GoToMainMenu();
                    }
                }
            }

            _cam.ZoomToWorldPoint(new Vector2(0f, 300f), _defaultZoom, 0.1f, _xBounds);


            

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
                || Input.GetButtonDown(1, Input.ArcadeButtons.A4)
                || Input.GetButtonDown(1, Input.ArcadeButtons.B1)
                || Input.GetButtonDown(1, Input.ArcadeButtons.B2)
                || Input.GetButtonDown(1, Input.ArcadeButtons.B3)
                || Input.GetButtonDown(1, Input.ArcadeButtons.B4)
                || Input.GetButtonDown(2, Input.ArcadeButtons.A1)
                || Input.GetButtonDown(2, Input.ArcadeButtons.A2)
                || Input.GetButtonDown(2, Input.ArcadeButtons.A3)
                || Input.GetButtonDown(2, Input.ArcadeButtons.A4)
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
                || Input.GetButtonUp(1, Input.ArcadeButtons.A4)
                || Input.GetButtonUp(1, Input.ArcadeButtons.B1)
                || Input.GetButtonUp(1, Input.ArcadeButtons.B2)
                || Input.GetButtonUp(1, Input.ArcadeButtons.B3)
                || Input.GetButtonUp(1, Input.ArcadeButtons.B4)
                || Input.GetButtonUp(2, Input.ArcadeButtons.A1)
                || Input.GetButtonUp(2, Input.ArcadeButtons.A2)
                || Input.GetButtonUp(2, Input.ArcadeButtons.A3)
                || Input.GetButtonUp(2, Input.ArcadeButtons.A4)
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
                || Input.GetButton(1, Input.ArcadeButtons.A4)
                || Input.GetButton(1, Input.ArcadeButtons.B1)
                || Input.GetButton(1, Input.ArcadeButtons.B2)
                || Input.GetButton(1, Input.ArcadeButtons.B3)
                || Input.GetButton(1, Input.ArcadeButtons.B4)
                || Input.GetButton(2, Input.ArcadeButtons.A1)
                || Input.GetButton(2, Input.ArcadeButtons.A2)
                || Input.GetButton(2, Input.ArcadeButtons.A3)
                || Input.GetButton(2, Input.ArcadeButtons.A4)
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

        private void ResetGame()
        {
            _player.Position = Vector2.Zero;
            _player.IsAlive = true;
            _player.Flings = 5;
            _player.Velocity = Vector2.Zero;
            _player.TargetDirection = float.NaN;
            _player.Direction = MathHelper.PiOver2;//Up
            _cam.Zoom = _defaultZoom;
            _cam.Move(-_cam.Center);
            _objMan.Reset(_cam.BoundingRectangle);
            _wave.Reset();
            _newGame = true;
        }

        private void TransitionToMenu(GameTime gt)
        {
            _cam.Position = Vector2.Zero;
            _cam.Zoom = _defaultZoom;
            _mainMenu.TransitionToMenu();
            _gameState = GameState.Menu;
            _mainMenu.ScoreTitle = "You Survived:";
            double score = gt.TotalGameTime.TotalSeconds - _beginTime;
            if(score < 1000000d)
                _mainMenu.ScoreLabel = score.ToString("#####0.00") + " seconds";
            else
                _mainMenu.ScoreLabel = "A really long time";//around 11.5 days
        }
    }
}