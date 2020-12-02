using Kadro.Physics.Colliders;
using Kadro.Gameobjects;
using Kadro.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace EightBall.Shared
{
    public class Rail : GameObject, IPhysicsCollision
    {
        public static Material Material = new Material(0.8f, 0.85f, 0.05f);

        public Rail(Vector2 position, Vector2[] collisionMesh)
        {
            this.Initialize(position, collisionMesh);
        }

        private void Initialize(Vector2 position, Vector2[] collisionMesh)
        {
            // note: collisionMesh origin is currently at tablecenter, so position is not needed

            RigidBodyComponent rigidComponent = new RigidBodyComponent();
            RigidBody r = rigidComponent.RigidBody;
            r.SetBodyType(BodyType.Static);
            r.SetMaterial(Material);
            r.SetCollider(new PolygonCollider(collisionMesh));
            r.CollisionMatrix.Clear();
            r.CollisionMatrix.AddLayer((int)Layers.Rail);
            r.CollisionMatrix.AddMaskLayer((int)Layers.Ball);
            //r.CollisionLayer.AddLayer((long)Layers.Rail);
            //r.CollisionMask.AddLayer((long)Layers.Ball);

            this.Add(rigidComponent);
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

        public void OnCollision(Collision collision)
        {
            //throw new NotImplementedException();
        }
    }
}
