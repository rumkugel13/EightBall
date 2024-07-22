using Kadro;
using Kadro.Gameobjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EightBall.Shared
{
    public class Table : GameObject
    {
        public static Vector2 Size = new Vector2(2.048f * 10, 1.156f * 10);

        ContentManager content;

        public Table(ContentManager content, bool full = false)
        {
            this.content = content;
            this.Initialize(full);
        }

        private void Initialize(bool full)
        {
            Texture2D texture = this.content.Load<Texture2D>($"Textures/{(full ? "complete" : "table")}");
            SpriteComponent component = new SpriteComponent(new Sprite(texture) { Name = "table", LayerDepth = full ? 0.5f : 0f }, Size.X / texture.Width);
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
