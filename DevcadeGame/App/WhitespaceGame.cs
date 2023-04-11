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

        //Physics objects
        private PhysicsObject _player;
        private PhysicsObject _test;

        public WhitespaceGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
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

            //Setup camera to work for any window size
            _cam = new OrthographicCamera(
                    new BoxingViewportAdapter(
                        Window, GraphicsDevice,
                        Ratio.X * 10, Ratio.Y * 10));

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

            //1 pixel texture
            _squareTexture = new Texture2D(GraphicsDevice, 1, 1);
            _squareTexture.SetData(new[] { Color.White });

            _circleTexture = Content.Load<Texture2D>("Filled Circle");
            _triangleTexture = Content.Load<Texture2D>("Filled Triangle");

            _font = Content.Load<SpriteFont>("Comfortaa200");

            DebugLog.Instance.Font = _font;
            DebugLog.Instance.Scale = 0.1f;

            _player = new PhysicsObject(_squareTexture)
            {
                HitboxRadius = 50,
                Tint = Color.Blue,
                Scale = new Vector2(10f),
            };

            _test = new PhysicsObject(_triangleTexture)
            {
                HitboxRadius = 50,
                Tint = Color.Red,
                Scale = new Vector2(10f),
                Position = new Vector2(100, 100)
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
            if(stickDirection.IsNaN()) stickDirection = Vector2.Zero;



            _player.Acceleration = stickDirection * 500f;

            _player.Rotation = (float)gameTime.TotalGameTime.TotalSeconds;

            _player.Update();


            //Update physics manager
            PhysicsManager.IN.Acceleration = stickDirection.LengthSquared();
            PhysicsManager.IN.Update(gameTime, 1f);

            float timeSpeed = PhysicsManager.IN.GameSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            //_test.Scale = (PhysicsManager.IN.TimeSpeed + 1)*100;
            _test.Acceleration = new Vector2(50f);
            _test.Update();

            DebugLog.Instance.LogFrame(PhysicsManager.IN.GameSpeed);

            ////
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(new Vector3(0.3f)));
            ////
            
            //Camera matrix
            Matrix transformation = _cam.GetViewMatrix();
            _spriteBatch.Begin(blendState: BlendState.AlphaBlend, transformMatrix: transformation);


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

        private Vector2 GetKeyboardStickDirection()
        {
            Vector2 direction = Vector2.Zero;
            if (Keyboard.GetState().IsKeyDown(Keys.W)) direction.Y -= 1;
            if (Keyboard.GetState().IsKeyDown(Keys.S)) direction.Y += 1;
            if (Keyboard.GetState().IsKeyDown(Keys.D)) direction.X += 1;
            if (Keyboard.GetState().IsKeyDown(Keys.A)) direction.X -= 1;
            return direction;
        }
    }
}