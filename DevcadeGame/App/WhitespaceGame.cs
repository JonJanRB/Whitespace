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

namespace Whitespace.App
{
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
        private const float _defaultZoom = 0.1f;

        //Physics objects
        private Player _player;
        private PhysicsObject _test;

        private List<PhysicsObject> _orbs;

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

            _bg = new Color(0.1f, 0.1f, 0.1f);

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

            DebugLog.Instance.LogPersistant(_xBounds);

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
                HitboxRadius = 100f,
                Tint = Color.Blue,
                Scale = new Vector2(100f),
            };

            _test = new PhysicsObject(_triangleTexture)
            {
                HitboxRadius = 100f,
                Tint = Color.Red,
                Scale = new Vector2(100f),
                Position = new Vector2(100f)
            };

            _orbs = new List<PhysicsObject>
            {
                new PhysicsObject(_circleTexture)
                {
                    HitboxRadius = 70f,
                    Tint = Color.Green,
                    Scale = new Vector2(100f),
                    Position = new Vector2((_xBounds.Y + _xBounds.X) * 0.5f)
                }
            };
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState ks = Keyboard.GetState();

            Input.Update(); // Updates the state of the input library
            //Emergency Exit
            if (ks.IsKeyDown(Keys.Escape) ||
                Input.GetButton(1, Input.ArcadeButtons.Menu) &&
                Input.GetButton(2, Input.ArcadeButtons.Menu))
            {
                Exit();
            }
            ////

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
            if(Input.GetButton(1, Input.ArcadeButtons.A1))
            {
                targetGameSpeed = 0.02f;
                _cam.ZoomToWorldPoint(
                    _player.Position + _player.DirectionVector * 1000f,
                    _defaultZoom * 2f, 0.1f, _xBounds);
            }
#if DEBUG
            if(ks.IsKeyDown(Keys.Space))
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
                
            }
#if DEBUG
            if (ks.IsKeyUp(Keys.Space) && _pk.IsKeyDown(Keys.Space))
            {
                _player.Velocity = _player.DirectionVector * 10000f;
            }
#endif
            //Default zoom to
            _cam.ZoomToWorldPoint(_player.Position, _defaultZoom, 0.1f, _xBounds);


            if(_player.Position.X < _xBounds.X)
            {
                _player.Velocity = new Vector2(MathF.Abs(_player.Velocity.X), _player.Velocity.Y);
            }
            else if(_player.Position.X > _xBounds.Y)
            {
                _player.Velocity = new Vector2(-MathF.Abs(_player.Velocity.X), _player.Velocity.Y);
            }

            //Update physics manager
            PhysicsManager.IN.Update(gameTime, targetGameSpeed, 0.1f);



            _player.Update();


            //_test.MomentOfAcceleration = new Vector2(50f);
            _test.Update();

            foreach(PhysicsObject orb in _orbs)
            {
                if(_player.Intersects(orb.Collider))
                {
                    _player.Velocity = _player.Velocity + new Vector2(0, -1000);
                }
                orb.Update();
            }



#if DEBUG
            _pk = ks;
#endif
            DebugLog.Instance.LogFrame("\n\n\n\n" + _player.Velocity);

            ////
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            ////
            
            //Camera matrix
            Matrix transformation = _cam.GetViewMatrix();
            _spriteBatch.Begin(blendState: BlendState.AlphaBlend, transformMatrix: transformation);

            //Draw background of canvas
            _spriteBatch.Draw(_squareTexture, _cam.BoundingRectangle.ToRectangle(), _bg);

            foreach(PhysicsObject orb in _orbs)
            {
                orb.Draw(_spriteBatch);
                orb.DrawHitbox(_spriteBatch);
            }


            _player.Draw(_spriteBatch);
            _player.DrawHitbox(_spriteBatch);

            _test.Draw(_spriteBatch);
            _test.DrawHitbox(_spriteBatch);

            _spriteBatch.End();

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

    }
}