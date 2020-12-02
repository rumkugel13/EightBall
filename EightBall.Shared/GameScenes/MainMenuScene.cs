using Kadro;
using Kadro.Gameobjects;
using Kadro.Input;
using Kadro.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace EightBall.Shared
{
    public class MainMenuScene : GameScene
    {
        private GUIScene scene;
        private Button singleplayer, multiplayer, settings, exit;

        private GameObjectWorld gameObjectWorld;

        public MainMenuScene(Game game) : base(game)
        {
            this.scene = new GUIScene();

            this.CreateScene();

            this.gameObjectWorld = new GameObjectWorld();
            this.CreateBackground();
        }

        private void CreateBackground()
        {
            this.gameObjectWorld.Add(new Background());
            this.gameObjectWorld.Add(new Table(true));
        }

        protected override void OnDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.OnDraw(gameTime, spriteBatch);

            this.gameObjectWorld.Draw(spriteBatch);
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            this.Game.IsMouseVisible = true;
            GUISceneManager.SwitchScene(this.scene);
        }

        protected override void OnExit()
        {
            base.OnExit();
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (Kadro.Input.KeyboardInput.OnKeyUp(Keys.Escape) || this.exit.OnClick())
            {
                this.Game.Exit();
                return;
            }

            if (this.singleplayer.OnClick())
            {
                SwitchScene<MainGameScene>();
            }

            if (this.multiplayer.OnClick())
            {
                //SwitchScene<ConnectingScene>();
            }

            if (this.settings.OnClick())
            {
                SwitchScene<SettingsScene>();
            }

            this.Camera.Origin = WindowSettings.RenderArea.Size.ToVector2() * 0.5f;
            this.Camera.FocusOn(Vector2.Zero - WindowSettings.RenderArea.Location.ToVector2(), 1.0f);

            this.gameObjectWorld.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        private void CreateScene()
        {
            SpriteFont largeFont = Assets.Get<SpriteFont>("Fonts/Arial24");

            // TODO: decorate uielements to match game style
            TextBlock headLine = new TextBlock(largeFont, "Main Menu");
            headLine.Alignment = Alignment.Center;
            headLine.PreferredPosition = new Point(0, -170);
            this.scene.AddChild(headLine);

            Point menuButtonSize = new Point(400, 50);
            float menuOpacity = 0.65f;

            this.singleplayer = new Button(largeFont, "Training");
            this.singleplayer.Alignment = Alignment.Center;
            this.singleplayer.PreferredSize = menuButtonSize;
            this.singleplayer.PreferredPosition = new Point(0, -90);
            this.singleplayer.Opacity = menuOpacity;
            //this.singleplayer.Border.Thickness = 4;
            this.scene.AddChild(this.singleplayer);

            this.multiplayer = new Button(largeFont, "Multiplayer");
            this.multiplayer.Alignment = Alignment.Center;
            this.multiplayer.PreferredSize = menuButtonSize;
            this.multiplayer.PreferredPosition = new Point(0, -30);
            this.multiplayer.Opacity = menuOpacity;
            //this.multiplayer.Border.Thickness = 4;
            this.scene.AddChild(this.multiplayer);
            this.multiplayer.SetEnabled(false);

            this.settings = new Button(largeFont, "Settings");
            this.settings.Alignment = Alignment.Center;
            this.settings.PreferredSize = menuButtonSize;
            this.settings.PreferredPosition = new Point(0, 30);
            this.settings.Opacity = menuOpacity;
            //this.settings.Border.Thickness = 4;
            this.scene.AddChild(this.settings);
            //this.settings.SetEnabled(false);

            this.exit = new Button(largeFont, "Exit");
            this.exit.Alignment = Alignment.Center;
            this.exit.PreferredSize = menuButtonSize;
            this.exit.PreferredPosition = new Point(0, 90);
            this.exit.Opacity = menuOpacity;
            //this.exit.Border.Thickness = 4;
            this.scene.AddChild(this.exit);
        }
    }
}
