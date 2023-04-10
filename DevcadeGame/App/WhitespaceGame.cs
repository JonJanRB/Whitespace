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
        /// THE unit
        /// </summary>
        //public static float u;

        private OrthographicCamera _cam;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Rectangle _something;

        private Wave _wave;

        //Textures
        private Texture2D _squareTexture;
        private Texture2D _circleTexture;
        private Texture2D _triangleTexture;
        private SpriteFont _font;

        //Physics objects
        private PhysicsObject _player;

        //Resolution
        private Vector2 _resolution;

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



            _resolution = new Vector2(
                _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight);

            //21 by 49 so get the factor of 21 that this screen's width is
            //u = _graphics.PreferredBackBufferWidth / 21f;

            _cam = new OrthographicCamera(new BoxingViewportAdapter(Window, GraphicsDevice, 21, 49));

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

            _wave = new Wave(new TextureRegion2D(_squareTexture));

            _player = new PhysicsObject()
            {
                Texture = _squareTexture,
                Collider = new CircleF(new Vector2(100), 50),
                Tint = Color.Red,
                //Scale = _resolution.X / 10f,
                Scale = 10f,
            };

        }

        protected override void UnloadContent()
        {
            //IDK why but unload these things
            _squareTexture.Dispose();
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


            _something = new Rectangle(
                100, 100,
                (int)((Math.Sin(gameTime.TotalGameTime.TotalSeconds * 2) + 1) * 100) + 50,
                (int)((Math.Sin(gameTime.TotalGameTime.TotalSeconds * 2) + 1) * 100) + 50);

            _wave.Position = Mouse.GetState().Position.ToVector2();
            _wave.Update(gameTime);



            Vector2 accelChange = Vector2.Zero;


            if (Keyboard.GetState().IsKeyDown(Keys.W)) accelChange.Y += -1;
            if (Keyboard.GetState().IsKeyDown(Keys.S)) accelChange.Y += 1;
            if (Keyboard.GetState().IsKeyDown(Keys.A)) accelChange.X += -1;
            if (Keyboard.GetState().IsKeyDown(Keys.D)) accelChange.X += 1;

            //accelChange = accelChange.NormalizedCopy() * _resolution.Y;
            accelChange = accelChange.NormalizedCopy();

            if(accelChange.IsNaN())
                accelChange = Vector2.Zero;

            _player.Acceleration = accelChange * 100;

            DebugLog.Instance.LogFrame(accelChange);

            _player.Update(gameTime);

            //_cam.Move(accelChange * u * gameTime.GetElapsedSeconds());


            ////
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(new Vector3(0.3f)));
            ////
            Matrix transformation = _cam.GetViewMatrix();


            _spriteBatch.Begin(blendState: BlendState.AlphaBlend, transformMatrix: transformation);


            _wave.Draw(_spriteBatch);

            _player.Draw(_spriteBatch);

            _spriteBatch.End();

            

            //Draw debug log
            _spriteBatch.Begin();
            DebugLog.Instance.Draw(_spriteBatch, _resolution);
            _spriteBatch.End();

            ////
            base.Draw(gameTime);
        }
    }
}