using Kadro;
using Kadro.Physics.Colliders;
using Kadro.Gameobjects;
using Kadro.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace EightBall.Shared
{
    public class Ball : GameObject, IPhysicsCollision
    {
        public static float Diameter = 0.063f * 10;    // units in m
        public static Material Material = new Material(0.8f, 0.95f, 0.05f);
        public static Vector2 WhiteBallPos = new Vector2(0.493f * 10, 0);

        public static Vector2 StatusBarPos = new Vector2(0, -0.7f * 10);

        public int Number { get; private set; }

        ContentManager content;

        public Ball(Vector2 position, int number, ContentManager content)
        {
            this.content = content;
            this.Initialize(position, number);
        }

        public static Vector2 BallPlaceholderPos(int number)
        {
            if (number == 8)
            {
                return StatusBarPos;
            }

            return (((number - 8) * Diameter + ((number < 8) ? -Diameter: +Diameter))* Vector2.UnitX) + StatusBarPos;
        }

        private void Initialize(Vector2 position, int number)
        {
            this.Transform.Position = position;
            this.Number = number;

            Collision.VelocityThreshold = 0f;   //<-- this caused sticking to the rails

            //TODO: figure out why balls seem to stick after colliding (even with rails)
            //note: seems to be better when objects are scaled by 10 (box2d recommends sizes between 0.1 and 10)
            RigidBodyComponent rigidComponent = new RigidBodyComponent();
            RigidBody r = rigidComponent.RigidBody;
            r.SetMaterial(Material);
            r.SetCollider(new CircleCollider(Diameter * 0.5f));
            r.CollisionMatrix.Clear();
            r.CollisionMatrix.AddLayer((int)Layers.Ball);
            //r.CollisionLayer.AddLayer((long)Layers.Ball);
            if (number == 0)
            {
                r.CollisionMatrix.AddLayer((int)Layers.WhiteBall);
                //r.CollisionLayer.AddLayer((long)Layers.WhiteBall);
            }
            r.CollisionMatrix.AddMaskLayer((int)Layers.Ball);
            //r.CollisionMask.AddLayer((long)Layers.Ball);
            // note: new damping formula required new values (previous: 0.05 or 0.95)
            r.LinearDamping = 1.33f;
            r.AngularDamping = 1.33f;
            r.CollisionListener = this;

            this.Add(rigidComponent);

            System.Diagnostics.Debug.Assert(number >= 0 && number < 16);

            Texture2D texture = this.content.Load<Texture2D>($"Textures/ball_{number}");
            SpriteComponent component = new SpriteComponent(new Sprite(texture) { Name = $"ball_{number}" }, Diameter / Math.Max(texture.Width, texture.Height));
            this.Add(component);

            // todo: create better sounds for future versions (e.g. make them last longer, by stretching last part?)
            this.Add(new SoundComponent(this.content.Load<SoundEffect>(Folders.SoundEffects + "/ball_ball_mockup")) { Name = "ball_ball_mockup" });
            this.Add(new SoundComponent(this.content.Load<SoundEffect>(Folders.SoundEffects + "/ball_hole_mockup")) { Name = "ball_hole_mockup" });
            this.Add(new SoundComponent(this.content.Load<SoundEffect>(Folders.SoundEffects + "/cue_ball_mockup")) { Name = "ball_rail_mockup" });
        }

        public override void OnAdded(GameObjectWorld gameObjectWorld)
        {
            base.OnAdded(gameObjectWorld);
        }

        public void OnCollision(Collision collision)
        {
            RigidBody localBody = this.Components.Get<RigidBodyComponent>().RigidBody;
            RigidBody otherBody = collision.GetOther(localBody);

            //Console.WriteLine(collision.RelativeVelocity.Length());
            //Console.WriteLine(collision.RelativeVelocity.LengthSquared());

            //if (other is Ball)
            if (otherBody.CollisionMatrix.HasLayer((int)Layers.Ball))
            {
                // TODO: the sound should only be played once;
                // - use soundmanager, and check with id? (e.g. ball numbers colliding)
                if (localBody.Velocity != Vector2.Zero)
                {
                    // todo: play sound independent of physics collision (remove dependency to physics, e.g. for multiplayer)
                    foreach (SoundComponent s in this.Components.GetAll<SoundComponent>())
                    {
                        if (s.Name == "ball_ball_mockup")
                        {
                            s.Volume = collision.ImpactVelocity.Length() * 0.4f;
                            //s.Volume = 0.5f;
                            if (!s.IsRunning)
                                s.PlayPitched(0.5f);
                        }
                    }
                }
            }
            //else if (other is Hole)
            else if (otherBody.CollisionMatrix.HasLayer((int)Layers.Hole))
                {
                // todo: this should be coming from a trigger in hole object
                RigidBody hole = otherBody;

                if (hole.Collider.Contains(this.Transform.Position))
                {
                    localBody.Velocity = Vector2.Zero;
                    localBody.AngularVelocity = 0f;
                    this.Transform.Position = BallPlaceholderPos(this.Number);
                    this.Transform.Rotation = 0f;
                    this.Active = false;

                    if (this.Number == 0)   //white ball
                    {
                        this.Visible = false;
                    }

                    foreach (SoundComponent s in this.Components.GetAll<SoundComponent>())
                    {
                        if (s.Name == "ball_hole_mockup")
                        {
                            if (!s.IsRunning)
                                s.PlayOnce();
                        }
                    }
                }
            }
            //else if(other is Rail)
            else if (otherBody.CollisionMatrix.HasLayer((int)Layers.Rail))
            {
                if (localBody.Velocity != Vector2.Zero)
                {
                    foreach (SoundComponent s in this.Components.GetAll<SoundComponent>())
                    {
                        if (s.Name == "ball_rail_mockup")
                        {
                            s.Volume = collision.ImpactVelocity.Length() * 0.4f;
                            //s.Volume = 0.5f;
                            if (!s.IsRunning)
                                s.PlayPitched(0.5f);
                        }
                    }
                }
            }
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
