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
    public class Hole : GameObject, IPhysicsTrigger
    {
        public Hole(Vector2 position, float size)
        {
            this.Initialize(position, size);
        }

        private void Initialize(Vector2 position, float size)
        {
            this.Transform.Position = position;

            //TODO: replace with trigger
            //TriggerComponent triggerComponent = new TriggerComponent();
            //Trigger r = triggerComponent.Trigger;
            RigidBodyComponent rigidBodyComponent = new RigidBodyComponent();
            RigidBody r = rigidBodyComponent.RigidBody;
            r.SetBodyType(BodyType.Static);
            r.SetCollider(new CircleCollider(size * 0.5f));
            r.CollisionMatrix.Clear();
            r.CollisionMatrix.AddLayer((int)Layers.Hole);
            r.CollisionMatrix.AddMaskLayer((int)Layers.Ball);
            //r.TriggerListener = this;
            //r.CollisionLayer.AddLayer((long)Layers.Hole);
            //r.CollisionMask.AddLayer((long)Layers.Ball);
            r.ResolveCollisions = false;

            this.Add(rigidBodyComponent);
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

        public void OnTrigger(RigidBody rigidBody)
        {
            //throw new NotImplementedException();
        }
    }
}
