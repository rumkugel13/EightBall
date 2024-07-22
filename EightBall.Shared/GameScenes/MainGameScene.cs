using Kadro;
using Kadro.Extensions;
using Kadro.Gameobjects;
using Kadro.Input;
using Kadro.Physics;
using Kadro.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EightBall.Shared
{
    public class MainGameScene : GameScene
    {
        private GUIScene scene;

        private bool isPaused;
        private Button btContinue, btEndGame, btBackToMain;
        private Label lbPlayerTurn, lbLeftPlayer, lbRightPlayer, lbWinner;
        private Panel pausePanel;
        private bool debugDraw, debugPause;

        private GameObjectWorld gameObjectWorld;

        private Vector2[] ballStartPositions;
        private Ball whiteBall, eightBall;
        private Cue cue;

        private GameState gameState;

        public MainGameScene(Game game) : base(game)
        {
            this.scene = new GUIScene();

            this.gameObjectWorld = new GameObjectWorld();

            PhysicsSystem physics = new PhysicsSystem();
            this.gameObjectWorld.AddSystem(physics);
            physics.Visible = false;
            PhysicsWorld.GlobalGravity = Vector2.Zero;
            PhysicsVisualizer.SetDrawOption(PhysicsVisualizer.DrawOptions.Colliders | PhysicsVisualizer.DrawOptions.Direction | PhysicsVisualizer.DrawOptions.Contacts);
            // TODO: fine tune all the values related to physics
            Collision.BiasThreshold = 0.005f;
            Collision.BiasFactor = 0.5f;

            this.CreateScene();
            this.CreatePauseMenu();
        }

        private void CreateScene()
        {
            SpriteFont labelFont = this.Game.Content.Load<SpriteFont>("Fonts/Arial16");

            this.lbPlayerTurn = new Label(labelFont, "Player 1 plays");
            this.lbPlayerTurn.Alignment = Alignment.Top;
            this.lbPlayerTurn.PreferredPosition = new Point(0, 20);
            this.lbPlayerTurn.Opacity = 0.75f;
            this.scene.AddChild(this.lbPlayerTurn);

            this.lbLeftPlayer = new Label(labelFont, "Player 1");
            this.lbLeftPlayer.Alignment = Alignment.Top;
            this.lbLeftPlayer.PreferredPosition = new Point(-300, 65);
            this.lbLeftPlayer.Opacity = 0.75f;
            this.lbLeftPlayer.SetVisible(false);
            this.scene.AddChild(this.lbLeftPlayer);

            this.lbRightPlayer = new Label(labelFont, "Player 2");
            this.lbRightPlayer.Alignment = Alignment.Top;
            this.lbRightPlayer.PreferredPosition = new Point(300, 65);
            this.lbRightPlayer.Opacity = 0.75f;
            this.lbRightPlayer.SetVisible(false);
            this.scene.AddChild(this.lbRightPlayer);

            this.lbWinner = new Label(this.Game.Content.Load<SpriteFont>("Fonts/Arial24"), "Winner");
            this.lbWinner.Alignment = Alignment.Center;
            this.lbWinner.Opacity = 0.75f;
            this.lbWinner.SetVisible(false);
            this.scene.AddChild(this.lbWinner);
        }

        private void CreatePauseMenu()
        {
            this.pausePanel = new Panel(this.scene.PreferredSize);
            this.pausePanel.Alignment = Alignment.Stretch;
            this.pausePanel.Opacity = 0.75f;
            this.scene.AddChild(this.pausePanel);

            TextBlock headLine = new TextBlock(this.Game.Content.Load<SpriteFont>("Fonts/Arial24"), "Pause Menu");
            headLine.Alignment = Alignment.Center;
            headLine.PreferredPosition = new Point(0, -150);
            this.pausePanel.AddChild(headLine);

            float buttonOpacity = 0.65f;
            SpriteFont buttonFont = this.Game.Content.Load<SpriteFont>("Fonts/Arial16");

            this.btContinue = new Button(buttonFont, "Continue");
            this.btContinue.Alignment = Alignment.Center;
            this.btContinue.PreferredPosition = new Point(0, -50);
            this.btContinue.Opacity = buttonOpacity;
            this.pausePanel.AddChild(this.btContinue);

            this.btEndGame = new Button(buttonFont, "End Game");
            this.btEndGame.Alignment = Alignment.Center;
            this.btEndGame.Opacity = buttonOpacity;
            this.pausePanel.AddChild(this.btEndGame);

            this.btBackToMain = new Button(buttonFont, "Back to Main");
            this.btBackToMain.Alignment = Alignment.Center;
            this.btBackToMain.PreferredPosition = new Point(0, 50);
            this.btBackToMain.Opacity = buttonOpacity;
            this.pausePanel.AddChild(this.btBackToMain);

            this.pausePanel.Hide();
        }

        protected override void OnDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.OnDraw(gameTime, spriteBatch);

            this.gameObjectWorld.Draw(spriteBatch);

            if (this.debugDraw)
            {
                this.gameObjectWorld.GameSystems.Get<PhysicsSystem>().Draw(spriteBatch);
            }

            //for (int i = 0; i < ballStartPositions.Length; i++)
            //{
            //    spriteBatch.DrawCircle(ballStartPositions[i], 30, Color.Brown, 1);
            //}
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            this.Game.IsMouseVisible = false;
            GUISceneManager.SwitchScene(this.scene);

            this.CreateBackground();
            this.CreateTable();
            this.CreateBalls();

            this.gameState.Reset();

            this.lbLeftPlayer.SetVisible(false);
            this.lbRightPlayer.SetVisible(false);
            this.lbWinner.SetVisible(false);
            this.UpdatePlayerTurn();
        }

        protected override void OnExit()
        {
            base.OnExit();

            this.gameObjectWorld.GameObjects.Clear();
            this.SetPaused(false);
        }

        private void SetPaused(bool value)
        {
            if (value)
            {
                this.pausePanel.Show();
            }
            else
            {
                this.pausePanel.Hide();
            }

            this.isPaused = value;
            this.Game.IsMouseVisible = value;
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (Kadro.Input.KeyboardInput.OnKeyUp(Keys.Escape) || this.btContinue.OnClick())
            {
                this.SetPaused(!this.isPaused);
            }

            if (Kadro.Input.KeyboardInput.OnKeyUp(Keys.F1))
            {
                this.debugDraw = !this.debugDraw;
            }

            if (Kadro.Input.KeyboardInput.OnKeyUp(Keys.F2))
            {
                this.debugPause = !this.debugPause;
            }

            if (this.debugPause)
            {
                if (!Kadro.Input.KeyboardInput.OnKeyUp(Keys.OemComma))
                {
                    return;
                }
            }

            if (this.isPaused)
            {
                if (this.btEndGame.OnClick())
                {
                    // todo: show game end overlay instead of switching to other scene
                    SwitchScene<GameOverScene>();
                }

                if (this.btBackToMain.OnClick())
                {
                    SwitchScene<MainMenuScene>();
                }

                return;
            }

            if (Kadro.Input.KeyboardInput.IsKeyDown(Keys.LeftControl) && MouseInput.OnScrollWheelChange())
            {
                if (MouseInput.OnScrollWheelUp())
                {
                    this.Camera.Zoom *= 1.125f;
                }

                if (MouseInput.OnScrollWheelDown())
                {
                    this.Camera.Zoom /= 1.125f;
                }
            }
            else if (Kadro.Input.KeyboardInput.IsKeyUp(Keys.LeftControl))
            {
                this.Camera.Zoom = 1.0f;
            }

            if (Kadro.Input.KeyboardInput.IsKeyDown(Keys.LeftControl) && MouseInput.IsButtonDown(MouseButton.Middle))
            {
                this.Camera.Position -= MouseInput.MouseMoveDelta().ToVector2();
            }

            this.Camera.Origin = WindowSettings.RenderArea.Size.ToVector2() * 0.5f;

            if (Kadro.Input.KeyboardInput.IsKeyUp(Keys.LeftControl))
                this.Camera.FocusOn(Vector2.Zero - WindowSettings.RenderArea.Location.ToVector2(), 1.0f);

            // input / pre update

            if (this.gameState.Winner != Player.None)
            {
                return;
            }

            bool ballsMoving = false;
            foreach (Ball b in this.gameObjectWorld.GameObjects.GetAll<Ball>())
            {
                RigidBody r = b.Components.Get<RigidBodyComponent>().RigidBody;
                if (r.Velocity != Vector2.Zero)
                {
                    ballsMoving = true;
                    break;
                }
            }

            if (ballsMoving)
            {
                if (this.cue.Active)
                {
                    this.cue.Rest();
                }
            }
            else
            {
                // cue state
                if (!this.cue.Active)
                {
                    bool scoredSameType = false;
                    foreach (Ball b in this.gameObjectWorld.GameObjects.GetAll<Ball>())
                    {
                        if (!b.Equals(this.whiteBall) && !(b == this.eightBall) && !b.Active)
                        {
                            int mask = 1 << b.Number;

                            if ((this.gameState.InactiveBalls & mask) == 0)
                            {
                                // ball was active before
                                this.gameState.InactiveBalls |= mask;

                                if ((this.gameState.LeftPlayer & this.gameState.RightPlayer) == Player.None)
                                {
                                    // no player scored before
                                    if (b.Number < 8)
                                    {
                                        this.gameState.LeftPlayer = this.gameState.ActivePlayer;
                                        this.gameState.RightPlayer = this.gameState.InactivePlayer;
                                    }
                                    else
                                    {
                                        this.gameState.RightPlayer = this.gameState.ActivePlayer;
                                        this.gameState.LeftPlayer = this.gameState.InactivePlayer;
                                    }

                                    this.ShowPlayerSides();
                                    scoredSameType = true;
                                }

                                if (b.Number < 8 && this.gameState.LeftPlayer == this.gameState.ActivePlayer ||
                                    b.Number > 8 && this.gameState.RightPlayer == this.gameState.ActivePlayer)
                                {
                                    scoredSameType = true;
                                }
                            }
                        }
                    }

                    if (!this.eightBall.Active)
                    {
                        int leftMask = 0x00FE;  // bits 7 to 1; 0 is white
                        int rightMask = 0xFE00; // bits 15 to 9; 8 is black
                        bool leftFull = (leftMask & this.gameState.InactiveBalls) == leftMask;
                        bool rightFull = (rightMask & this.gameState.InactiveBalls) == rightMask;

                        this.gameState.Winner = (((this.gameState.LeftPlayer == this.gameState.ActivePlayer) && leftFull) ||
                                                (!(this.gameState.LeftPlayer == this.gameState.ActivePlayer) && !rightFull)) ? this.gameState.LeftPlayer : this.gameState.RightPlayer;

                        this.ShowWinner();
                        return;
                    }

                    if (!this.whiteBall.Active)
                    {
                        this.whiteBall.Active = true;
                        this.whiteBall.Visible = true;
                        // todo: allow placing the ball with mousepos
                        this.whiteBall.Transform.Position = Ball.WhiteBallPos;
                        this.gameState.ActivePlayer = this.gameState.InactivePlayer;
                    }
                    else if (!scoredSameType)
                    {
                        this.gameState.ActivePlayer = this.gameState.InactivePlayer;
                    }

                    this.UpdatePlayerTurn();

                    this.cue.FocusOnBall(this.whiteBall.Transform.Position);    //activates cue; note: must be called after white ball has been set
                }
            }

            this.gameObjectWorld.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            // post update

            //if (MouseInput.OnMouseMoved())
            //{
            //    Console.WriteLine($"Raw mouse: {MouseInput.GetCursorPosition()}");
            //    Console.WriteLine($"Leal: {Vector2.Transform(MouseInput.GetCursorPosition().ToVector2(), this.Camera.InverseMatrix) / WindowManager.UnitScale}");
            //    Console.WriteLine($"Scene: {this.ViewToWorld(MouseInput.PositionVector)}");
            //}
        }

        private void UpdatePlayerTurn()
        {
            this.lbPlayerTurn.TextBlock.Text = $"Player {this.gameState.ActivePlayer} plays";
        }

        private void ShowWinner()
        {
            this.lbWinner.TextBlock.Text = "Winner: Player " + this.gameState.Winner;
            this.lbWinner.SetVisible(true);
        }

        private void ShowPlayerSides()
        {
            this.lbLeftPlayer.TextBlock.Text = "Player " + this.gameState.LeftPlayer;
            this.lbLeftPlayer.SetVisible(true);
            this.lbRightPlayer.TextBlock.Text = "Player " + this.gameState.RightPlayer;
            this.lbRightPlayer.SetVisible(true);
        }

        private void CreateBackground()
        {
            this.gameObjectWorld.Add(new Background(this.Game.Content));
        }

        private void CreateBalls()
        {
            // create number array excluding ball eight and the white one
            int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 9, 10, 11, 12, 13, 14, 15 };
            numbers.Shuffle();

            // note: the ball nearest to the white one must be 1, the farthest two corners must be different type
            // place them inside the triangle, eight ball and white ball always on same spot
            /*
                ° .
                ° . ° .
                ° . ° . °
                ° . °
                °
             */

            Vector2 eightBallPos = new Vector2(-0.418f * 10, 0);
            float ballSize = Ball.Diameter;
            float ballHalf = ballSize * 0.5f;
            float triangleHeight = (MathExtensions.Sqrt3 / 2f) * ballSize;

            Vector2 start = eightBallPos - new Vector2(2 * triangleHeight, 2 * ballSize);
            ballStartPositions = new Vector2[15];
            int index = 0;

            for (int i = 5; i > 0; i--)
            {
                for (int j = 0; j < i; j++)
                {
                    ballStartPositions[index] = start + new Vector2(j * triangleHeight, j * ballHalf);
                    index++;
                }
                start += Vector2.UnitY * ballSize;
            }

            for (int i = 0; i < ballStartPositions.Length - 1; i++)
            {
                if (i < 7)
                {
                    this.gameObjectWorld.Add(new Ball(ballStartPositions[i], numbers[i], this.Game.Content));
                }
                else
                {
                    this.gameObjectWorld.Add(new Ball(ballStartPositions[i + 1], numbers[i], this.Game.Content));
                }
            }

            this.eightBall = new Ball(eightBallPos, 8, this.Game.Content);
            this.gameObjectWorld.Add(this.eightBall);

            // create placeholder balls
            for (int i = 1; i < 16; i++)
            {
                Ball b = new Ball(Ball.BallPlaceholderPos(i), i, this.Game.Content);
                b.Components.Get<RigidBodyComponent>().Active = false;
                b.Components.Get<SpriteComponent>().Sprite.Color = Color.White * 0.5f;
                this.gameObjectWorld.Add(b);
            }

            this.whiteBall = new Ball(Ball.WhiteBallPos, 0, this.Game.Content);
            this.gameObjectWorld.Add(this.whiteBall);

            this.cue = new Cue(Ball.WhiteBallPos, ScaledMesh(cueMesh, 10), this.Game.Content);
            this.gameObjectWorld.Add(this.cue);
            this.cue.FocusOnBall(this.whiteBall.Transform.Position);
        }

        private void CreateTable()
        {
            this.gameObjectWorld.Add(new Table(this.Game.Content));
            const int railNum = 6, holeNum = 6;

            for (int i = 0; i < railNum; i++)
            {
                this.gameObjectWorld.Add(new Rail(railPositions[i] * 10, ScaledMesh(meshes[i], 10)));
            }

            for (int i = 0; i < holeNum; i++)
            {
                this.gameObjectWorld.Add(new Hole(holePositions[i] * 10, holeDiameters[i] * 10));
            }
        }

        private Vector2[] ScaledMesh(Vector2[] mesh, float scale)
        {
            Vector2[] scaledMesh = new Vector2[mesh.Length];
            Matrix scaleMatric = Matrix.CreateScale(scale);

            Vector2.Transform(mesh, ref scaleMatric, scaledMesh);
            return scaledMesh;
        }

        private Vector2[][] meshes =
        {
            new Vector2[] { new Vector2(-0.929f, -0.415f), new Vector2(-0.893f, -0.378f), new Vector2(-0.893f, +0.378f), new Vector2( -0.929f, +0.415f) },
            new Vector2[] { new Vector2(+0.929f, -0.415f), new Vector2(+0.929f, +0.415f), new Vector2(+0.893f, +0.378f), new Vector2( +0.893f, -0.378f) },
            new Vector2[] { new Vector2(-0.861f, +0.483f), new Vector2(-0.824f, +0.447f), new Vector2(-0.064f, +0.447f), new Vector2( -0.048f, +0.483f) },
            new Vector2[] { new Vector2(+0.048f, +0.483f), new Vector2(+0.063f, +0.447f), new Vector2(+0.823f, +0.447f), new Vector2( +0.861f, +0.483f) },
            new Vector2[] { new Vector2(-0.861f, -0.483f), new Vector2(-0.048f, -0.483f), new Vector2(-0.064f, -0.447f), new Vector2( -0.824f, -0.447f) },
            new Vector2[] { new Vector2(+0.048f, -0.483f), new Vector2(+0.861f, -0.483f), new Vector2(+0.823f, -0.447f), new Vector2( +0.063f, -0.447f) },
        };

        private Vector2[] railPositions =
        {
            new Vector2(-0.911f, 0f),
            new Vector2(+0.911f, 0f),
            new Vector2(-0.44925f, 0.465f),
            new Vector2(+0.44875f, 0.465f),
            new Vector2(-0.44925f, -0.465f),
            new Vector2(+0.44875f, -0.465f),
        };

        private Vector2[] holePositions =
        {
            new Vector2(-0.916f, -0.47f),
            new Vector2(0f, -0.496f),
            new Vector2(0.916f, -0.47f),
            new Vector2(-0.916f, 0.47f),
            new Vector2(0f, 0.496f),
            new Vector2(0.916f, 0.47f),
        };

        private float[] holeDiameters =
        {
            0.116f,
            0.104f,
            0.116f,
            0.116f,
            0.104f,
            0.116f,
        };

        private Vector2[] cueMesh =
        {
            new Vector2(-0.79f, 0f),
            new Vector2(-0.789f, -0.0085f),
            new Vector2(0.789f, -0.0145f),
            new Vector2(0.79f, 0f),
            new Vector2(0.789f, 0.0145f),
            new Vector2(-0.789f, 0.0085f),
        };
    }
}
