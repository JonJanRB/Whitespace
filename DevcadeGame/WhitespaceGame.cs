using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Devcade;
using System;
using Whitespace.Util;

namespace Whitespace
{
	public class WhitespaceGame : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		private Rectangle _something;

		
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
			_spriteBatch.Begin();
			
			
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