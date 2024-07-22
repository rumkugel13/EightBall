using Kadro;
using Kadro.Input;
using Kadro.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EightBall.Shared
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private GUISceneManager guiSceneManager;
        //private WindowManager windowManager;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
#if DEBUG
            this.Window.Title = "EightBall-dev-debug";
#else
            this.Window.Title = "EightBall-dev-release";
#endif

            //this.windowManager = new WindowManager(this, this.graphics, new Point(1280, 720)/*GameConfig.DesignResolution*/);
            WindowSettings.Initialize(this, this.graphics);
            //WindowManager.SetWindowResolution(new Point(UserConfig.Instance.ScreenWidth, UserConfig.Instance.ScreenHeight));
            WindowSettings.SetWindowResolution(new Point(1280, 720)/*new Point(UserConfig.Instance.ScreenWidth, UserConfig.Instance.ScreenHeight)*/);
            //WindowManager.SetBorderless(UserConfig.Instance.Borderless);
            //WindowManager.MinWindowSize = GameConfig.MinWindowSize;
            WindowSettings.UnitsVisible = new Vector2(3.2f * 10, 1.8f * 10);   // units in m
            this.Window.AllowUserResizing = false;

            this.guiSceneManager = new GUISceneManager(this);

            this.Components.Add(new Kadro.Input.KeyboardInput(this));
            this.Components.Add(new MouseInput(this));
            this.Components.Add(new GamepadInput(this));
            this.Components.Add(new TouchpanelInput(this));

            base.Initialize();  //initialize components
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            GameScene.AddScene(new MainMenuScene(this));
            GameScene.AddScene(new MainGameScene(this));
            GameScene.AddScene(new GameOverScene(this));
            GameScene.AddScene(new SettingsScene(this));

            GameScene.SwitchScene<MainMenuScene>();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);

            this.guiSceneManager.Update(gameTime);

            GameScene.ActiveScene.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            base.Draw(gameTime);

            GameScene.ActiveScene.Draw(gameTime);

            this.guiSceneManager.Draw(gameTime);

            //this.spriteBatch.Begin();
            //this.spriteBatch.End();
        }
    }
}
