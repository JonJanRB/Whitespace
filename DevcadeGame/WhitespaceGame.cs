using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Devcade;
using System;
using Whitespace.Util;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers.Containers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended;
using System.Collections.Generic;

namespace Whitespace
{
	public class WhitespaceGame : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		private Rectangle _something;

		private ParticleEffect _particleEffect;
		private Texture2D _texture;

		private Wave _wave;
		
		public WhitespaceGame()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = false;
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

            //Yoink
            _texture = new Texture2D(GraphicsDevice, 1, 1);
            _texture.SetData(new[] { Color.White });

            TextureRegion2D textureRegion = new TextureRegion2D(_texture);
            _particleEffect = new ParticleEffect(autoTrigger: false)
            {
                Position = Vector2.Zero,
                Emitters = new List<ParticleEmitter>
				{
					new ParticleEmitter(textureRegion, 500, TimeSpan.FromSeconds(2.5),
						Profile.BoxUniform(100,250))
					{
						Parameters = new ParticleReleaseParameters
						{
							Speed = new Range<float>(0f, 50f),
							Quantity = 3,
							Rotation = new Range<float>(-1f, 1f),
							Scale = new Range<float>(3.0f, 4.0f)
						},
						Modifiers =
						{
							new AgeModifier
							{
								Interpolators =
								{
									new ColorInterpolator
									{
										StartValue = new HslColor(0.33f, 0.5f, 0.5f),
										EndValue = new HslColor(0.5f, 0.9f, 1.0f)
									}
								}
							},
							new RotationModifier {RotationRate = -2.1f},
							new RectangleContainerModifier {Width = 800, Height = 480},
							new LinearGravityModifier {Direction = -Vector2.UnitY, Strength = 30f},
						}
					}
				}
            };

			_wave = new Wave(textureRegion);

        }

        protected override void UnloadContent()
        {
			//IDK why but unload these things
            _texture.Dispose();
            _particleEffect.Dispose();
        }

        protected override void Update(GameTime gameTime)
		{
			Input.Update(); // Updates the state of the input library
			//Emergency Exit
			if (Keyboard.GetState().IsKeyDown(Keys.Escape) ||
				(Input.GetButton(1, Input.ArcadeButtons.Menu) &&
				Input.GetButton(2, Input.ArcadeButtons.Menu)))
			{
				Exit();
			}
			////


			_something = new Rectangle(
				100, 100,
				(int)((Math.Sin(gameTime.TotalGameTime.TotalSeconds*2)+1)*100)+50,
				(int)((Math.Sin(gameTime.TotalGameTime.TotalSeconds*2)+1)*100)+50);

			_particleEffect.Position = Mouse.GetState().Position.ToVector2();
			_particleEffect.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            _wave.Position = Mouse.GetState().Position.ToVector2();
            _wave.Update(gameTime);

			////
			base.Update(gameTime);
		}

		/// <summary>
		/// Your main draw loop. This runs once every frame, over and over.
		/// </summary>
		/// <param name="gameTime">This is the gameTime object you can use to get the time since last frame.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(new Color(new Vector3(0.3f)));
            ////
            
			_spriteBatch.Begin(blendState: BlendState.AlphaBlend);


            _spriteBatch.Draw(_particleEffect);
            
			_wave.Draw(_spriteBatch);

			_spriteBatch.End();


            ShapeBatch.Begin(GraphicsDevice);

			ShapeBatch.BoxOutline(_something, Color.Red);

			ShapeBatch.Triangle(
				new Vector2(300),
				(float)((Math.Sin(gameTime.TotalGameTime.TotalSeconds*2)+1)*100)+50,
				Color.Blue);

			ShapeBatch.End();

			////
			base.Draw(gameTime);
		}
	}
}