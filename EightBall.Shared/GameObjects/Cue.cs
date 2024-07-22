using Kadro;
using Kadro.Physics.Colliders;
using Kadro.Extensions;
using Kadro.Gameobjects;
using Kadro.Input;
using Kadro.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace EightBall.Shared
{
    public class Cue : GameObject, IPhysicsCollision
    {
        public static Material Material = new Material(0.6f, 0.75f, 0.55f);
        public static Vector2 Size = new Vector2(1.581f * 10, 0.029f * 10);

        private Vector2 focusPosition;
        private Vector2 offset = new Vector2(0.85f * 10, 0f);
        private Vector2 restingPosition = new Vector2(0, 0.6f * 10);
        private float poweringUpVelocity = 20f * 10;
        private float releaseForce = 10f * 10 * 10 * 10;
        private float resolutionsPerSecond = MathHelper.Pi * 0.5f;

        private bool poweringUp = false;
        private bool releasing = false;
        private float timeSinceStart = 0f;
        private float releaseTime = 0.5f;

        ContentManager content;

        public Cue(Vector2 focusPosition, Vector2[] collisionMesh, ContentManager content)
        {
            this.content = content;
            this.Initialize(focusPosition, collisionMesh);
        }

        private void Initialize(Vector2 focusPosition, Vector2[] collisionMesh)
        {
            this.Transform.Position = focusPosition + offset;

            RigidBodyComponent rigidComponent = new RigidBodyComponent();
            RigidBody r = rigidComponent.RigidBody;
            r.SetBodyType(BodyType.Dynamic);
            r.SetMaterial(Material);
            r.SetCollider(new PolygonCollider(collisionMesh));
            r.CollisionMatrix.Clear();
            r.CollisionMatrix.AddLayer((int)Layers.Cue);
            r.CollisionMatrix.AddMaskLayer((int)Layers.WhiteBall);
            //r.CollisionLayer.AddLayer((long)Layers.Cue);
            //r.CollisionMask.AddLayer((long)Layers.WhiteBall);
            r.CollisionListener = this;

            this.Add(rigidComponent);

            Texture2D texture = this.content.Load<Texture2D>("Textures/cue");
            SpriteComponent component = new SpriteComponent(new Sprite(texture) { Name = "cue" }, Size.X / texture.Width);
            this.Add(component);

            this.Add(new SoundComponent(this.content.Load<SoundEffect>("SoundEffects/cue_ball_mockup")));
        }

        public override void OnAdded(GameObjectWorld gameObjectWorld)
        {
            base.OnAdded(gameObjectWorld);
        }

        public void OnCollision(Collision collision)
        {
            this.releasing = false;
            this.poweringUp = false;
            // disable here?
            RigidBody r = this.Components.Get<RigidBodyComponent>().RigidBody;
            r.Velocity = Vector2.Zero;

            this.Components.Get<SoundComponent>().Volume = collision.ImpactVelocity.Length() * 0.4f;
            this.Components.Get<SoundComponent>().PlayPitched(0.5f);
        }

        public override void OnDraw(SpriteBatch spriteBatch)
        {
            base.OnDraw(spriteBatch);

            // todo: only show when enabled, limit length to next gameobject (check with ray for intersectionPoint)
            if (!this.poweringUp && this.Active)
            {
                // show direction vector
                Vector2 ballPos = focusPosition - offset;
                Vector2 lineStart = ballPos + this.Transform.Rotation.ToCartesian().PerpRight() * Ball.Diameter * 0.75f;
                Vector2 lineEnd = lineStart + this.Transform.Rotation.ToCartesian().PerpRight() * 1.5f * 10;

                //note: this is already affected by UnitScale
                spriteBatch.DrawLineSegment(lineStart, lineEnd, Color.LightGray, 1);
            }
        }

        public override void OnRemoved(GameObjectWorld gameObjectWorld)
        {
            base.OnRemoved(gameObjectWorld);
        }

        public override void OnUpdate(float elapsedSeconds)
        {
            base.OnUpdate(elapsedSeconds);

            if (!this.poweringUp)
            {
                if (Kadro.Input.KeyboardInput.IsKeyDown(Keys.Left))
                {
                    this.Transform.Rotation -= this.resolutionsPerSecond * elapsedSeconds;
                    this.Transform.Position = VectorExtensions.Rotate(this.Transform.Position, -this.resolutionsPerSecond * elapsedSeconds, focusPosition - offset);
                }

                if (Kadro.Input.KeyboardInput.IsKeyDown(Keys.Right))
                {
                    this.Transform.Rotation += this.resolutionsPerSecond * elapsedSeconds;
                    this.Transform.Position = VectorExtensions.Rotate(this.Transform.Position, this.resolutionsPerSecond * elapsedSeconds, focusPosition - offset);
                }

                if (MouseInput.OnMouseMoved())
                {
                    float targetAngle = (GameScene.ActiveScene.ViewToWorld(MouseInput.PositionVector) - focusPosition + offset).PerpRight().ToPolar();
                    //targetAngle = MathExtensions.LerpAngle(this.Transform.Rotation, targetAngle, 1.0f * elapsedSeconds);
                    this.Transform.Rotation = targetAngle;
                    this.Transform.Position = VectorExtensions.Rotate(focusPosition, targetAngle, focusPosition - offset);
                }

                if (Kadro.Input.KeyboardInput.OnKeyDown(Keys.Space))
                {
                    this.poweringUp = true;

                    // add constant velocity to move back
                    RigidBody r = this.Components.Get<RigidBodyComponent>().RigidBody;
                    r.AddVelocityChange(VectorExtensions.AngleToVector2(this.Transform.Rotation).PerpLeft() * this.poweringUpVelocity * elapsedSeconds);
                }
            }

            if (this.poweringUp && Kadro.Input.KeyboardInput.IsKeyDown(Keys.Space))
            {
                this.timeSinceStart += elapsedSeconds;
            }

            if ((this.poweringUp && Kadro.Input.KeyboardInput.OnKeyUp(Keys.Space)) || this.timeSinceStart > this.releaseTime)
            {
                // release
                this.timeSinceStart = 0f;
                this.releasing = true;
                RigidBody r = this.Components.Get<RigidBodyComponent>().RigidBody;
                r.Velocity = Vector2.Zero;
                r.AngularVelocity = 0;
            }

            if (this.releasing)
            {
                // accelerate cue until it hits the ball
                RigidBody r = this.Components.Get<RigidBodyComponent>().RigidBody;
                r.AddForce(-VectorExtensions.AngleToVector2(this.Transform.Rotation).PerpLeft() * this.releaseForce * elapsedSeconds);
            }
        }

        public void FocusOnBall(Vector2 position)
        {
            focusPosition = position + offset;
            this.Transform.Position = focusPosition;
            this.Active = true;
            //this.Visible = true;
            this.Transform.Rotation = 0;
        }

        // go to resting position to not interfer with white ball
        public void Rest()
        {
            this.Active = false;
            //this.Visible = false;
            this.Transform.Rotation = 0;
            this.Transform.Position = this.restingPosition;
        }
    }
}
