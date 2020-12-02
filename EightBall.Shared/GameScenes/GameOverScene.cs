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
    public class GameOverScene : GameScene
    {
        private TextBlock text;
        private GUIScene screen;
        private Button exit;

        private GameObjectWorld gameObjectWorld;

        public GameOverScene(Game game) : base(game)
        {
            this.screen = new GUIScene();

            this.text = new TextBlock(Assets.Get<SpriteFont>("Fonts/Arial24"), "Game Over! Press Escape to get back to main menu");
            this.text.Alignment = Alignment.Center;
            this.text.PreferredPosition = new Point(0, -30);
            this.screen.AddChild(this.text);

            this.exit = new Button(Assets.Get<SpriteFont>("Fonts/Arial16"), "Exit");
            this.exit.Alignment = Alignment.Center;
            this.exit.PreferredPosition = new Point(0, 30);
            this.screen.AddChild(this.exit);

            this.gameObjectWorld = new GameObjectWorld();
            this.CreateBackground();
        }

        private void CreateBackground()
        {
            this.gameObjectWorld.Add(new Background());
            this.gameObjectWorld.Add(new Table(true));
        }

        protected override void OnEnter()
        {
            this.Game.IsMouseVisible = true;
            GUISceneManager.SwitchScene(this.screen);
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            if (Kadro.Input.KeyboardInput.OnKeyUp(Keys.Escape) || this.exit.OnClick())
            {
                SwitchScene<MainMenuScene>();
                return;
            }

            this.Camera.Origin = WindowSettings.RenderArea.Size.ToVector2() * 0.5f;
            this.Camera.FocusOn(Vector2.Zero - WindowSettings.RenderArea.Location.ToVector2(), 1.0f);

            this.gameObjectWorld.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        protected override void OnDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.OnDraw(gameTime, spriteBatch);

            this.gameObjectWorld.Draw(spriteBatch);
        }

        protected override void OnExit()
        {
            base.OnExit();
        }
    }
}
