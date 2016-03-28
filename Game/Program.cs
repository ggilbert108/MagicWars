using System;
using System.Drawing;
using System.Drawing.Text;
using Ecs.Core;
using Ecs.EntitySystem;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Game
{
    public class Program
    {
        private static readonly Rectangle Bounds = new Rectangle(0, 0, 5000, 5000);

        private static Manager manager;
        private static Generator generator;
        private static GameWindow game;

        private static bool gameOver = false;
        private static TextRenderer renderer;

        public static void Main(string[] args)
        {
            Initialize();
            AddSystems();
            GenerateLevel();
            SubscribeEvents();
            Start();
        }

        #region Setup

        private static void Initialize()
        {
            manager = new Manager();
            generator = new Generator(manager);
            game = new GameWindow(800, 600, new GraphicsMode(32, 24, 0, 4));


        }

        private static void AddSystems()
        {
            manager.AddSystem(new RenderSystem());
            manager.AddSystem(new InputSystem(game));
            manager.AddSystem(new IntentSystem());
            manager.AddSystem(new MovementSystem());
            manager.AddSystem(new GarbageSystem());
            manager.AddSystem(new WeaponSystem());
            manager.AddSystem(new CollisionSystem());
            manager.AddSystem(new SteeringSystem());
            manager.AddSystem(new FacingSystem());
            manager.AddSystem(new AiSystem());
            manager.AddSystem(new CameraSystem());
            manager.AddSystem(new StatsSystem());
        }

        private static void GenerateLevel()
        {
            generator.SetupBounds(Bounds);
            generator.GenerateLevel();
            manager.BindHero(generator.GetHeroId());
        }

        private static void SubscribeEvents()
        {
            game.Load += Load;
            game.Resize += Resize;
            game.UpdateFrame += UpdateFrame;
            game.RenderFrame += RenderFrame;
        }

        private static void Start()
        {
            game.Run(60.0);
        }

        #endregion

        #region Event Handlers

        private static void Load(object sender, EventArgs e)
        {
            game.VSync = VSyncMode.On;
            renderer = new TextRenderer(game.Width, game.Height);

            renderer.SetFont(new Font(new FontFamily(GenericFontFamilies.Serif), 24));
            renderer.SetBrush(Brushes.Red);
        }

        private static void Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, game.Width, game.Height);
        }

        private static void UpdateFrame(object sender, FrameEventArgs e)
        {
            if (game.Keyboard[Key.Escape])
            {
                game.Exit();
            }

            if (!manager.EntityExists(generator.GetHeroId()))
            {
                gameOver = true;
            }

            if (!gameOver)
            {
                float elapsed = (float)e.Time;
                manager.Update(elapsed);
            }
        }

        private static void RenderFrame(object sender, FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, game.Width, game.Height, 0, 0.0, 4.0);

            if (!manager.EntityExists(generator.GetHeroId()))
            {
                gameOver = true;
            }


            if (!gameOver)
            {
                DrawGrid();
                var renderSystem = manager.GetSystem<RenderSystem>();
                renderSystem.Render();
            }
            else
            {
                renderer.RenderText("Game Over", Vector2.Zero);
            }

            game.SwapBuffers();
        }

        #endregion

        #region Helper Methods

        private static void DrawGrid()
        {
            const int gridSize = 50;

            Rectangle viewport = manager.GetEntityById(generator.GetHeroId()).GetComponent<Camera>().Viewport;

            int xOff = gridSize - (viewport.X % gridSize);
            int yOff = gridSize - (viewport.Y % gridSize);

            for (int x = xOff; x < viewport.Width; x += gridSize)
            {
                if (!Util.ValueInRange(x + viewport.X, Bounds.Left, Bounds.Right))
                    continue;

                Rectangle line = new Rectangle(x, 0, 3, viewport.Height);
                RenderSystem.DrawRectangle(line, Color.MediumBlue);
            }

            for (int y = yOff; y < viewport.Height; y += gridSize)
            {
                if (!Util.ValueInRange(y + viewport.Y, Bounds.Top, Bounds.Bottom))
                    continue;

                Rectangle line = new Rectangle(0, y, viewport.Width, 3);
                RenderSystem.DrawRectangle(line, Color.MediumBlue);
            }
        }

        #endregion
    }
}