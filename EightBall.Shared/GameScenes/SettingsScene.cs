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
    public class SettingsScene : GameScene
    {
        //private TextBlock text;
        private GUIScene screen;
        private Slider volumeSlider;
        private Label volumeLabel;
        private Button exit;

        private GameObjectWorld gameObjectWorld;

        public SettingsScene(Game game) : base(game)
        {
            this.screen = new GUIScene();
            this.screen.Opacity = 0.5f;

            SpriteFont font = game.Content.Load<SpriteFont>("Fonts/Arial16");

            //this.text = new TextBlock("Game Over! Press Escape to get back to main menu", Alignment.Center);
            //this.text.FontSize = Font.FontSizes.Large;
            //this.text.PreferredPosition = new Point(0, -30);
            //this.screen.AddChild(this.text);

            this.volumeSlider = new Slider();
            this.volumeSlider.Alignment = Alignment.Center;
            this.volumeSlider.PreferredPosition = new Point(150, -30);
            this.screen.AddChild(this.volumeSlider);

            this.volumeLabel = new Label(font, "SoundEffect Volume:");
            this.volumeLabel.Alignment = Alignment.Center;
            this.volumeLabel.PreferredPosition = new Point(-150, -30);
            this.volumeLabel.Opacity = 0.75f;
            this.screen.AddChild(this.volumeLabel);

            this.exit = new Button(font, "Exit");
            this.exit.Alignment = Alignment.Center;
            this.exit.PreferredPosition = new Point(0, 30);
            this.exit.Opacity = 0.75f;
            this.screen.AddChild(this.exit);

            this.gameObjectWorld = new GameObjectWorld();
            this.CreateBackground();

            UserSettings.SetSoundEffectVolume(this.volumeSlider.Progress.RawValue);
        }

        private void CreateBackground()
        {
            this.gameObjectWorld.Add(new Background(this.Game.Content));
            this.gameObjectWorld.Add(new Table(this.Game.Content, true));
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

            if (this.volumeSlider.OnValueChanged())
            {
                UserSettings.SetSoundEffectVolume(this.volumeSlider.Progress.RawValue);
            }
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
