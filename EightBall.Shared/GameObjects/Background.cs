using Kadro;
using Kadro.Gameobjects;
using Microsoft.Xna.Framework.Graphics;

namespace EightBall.Shared
{
    public class Background : GameObject
    {
        public Background()
        {
            Texture2D texture = Assets.Get<Texture2D>("Textures", $"background");
            SpriteComponent component = new SpriteComponent(new Sprite(texture) { Name = "background", LayerDepth = 0.5f }, WindowSettings.UnitsVisible.X / texture.Width);
            this.Add(component);
        }

        public override void OnAdded(GameObjectWorld gameObjectWorld)
        {
            base.OnAdded(gameObjectWorld);
        }

        public override void OnDraw(SpriteBatch spriteBatch)
        {
            base.OnDraw(spriteBatch);
        }

        public override void OnRemoved(GameObjectWorld gameObjectWorld)
        {
            base.OnRemoved(gameObjectWorld);
        }

        public override void OnUpdate(float elapsedSeconds)
        {
            base.OnUpdate(elapsedSeconds);
        }
    }
}
