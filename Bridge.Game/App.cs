using Bridge;
using Bridge.Html5;
using Bridge.Lib;
using Ecs.Core;
using Ecs.EntitySystem;
using Performance = Bridge.Lib.Performance;

namespace Bridge.Game
{
    public class App
    {
        private static readonly Rectangle Bounds = new Rectangle(0, 0, 4000, 4000);
        private const int FPS = 30;
        private const int TARGET_MS = 1000/FPS;

        private static Manager manager;
        private static Generator generator;

        private static bool gameOver = false;

        [Ready]
        public static void Main()
        {
            Initialize();
            AddSystems();
            GenerateLevel();
            SubscribeEvents();
        }

        #region Setup

        private static void Initialize()
        {
            manager = new Manager();
            generator = new Generator(manager);
        }

        private static void AddSystems()
        {
            manager.AddSystem(new RenderSystem());
            manager.AddSystem(new InputSystem());
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
            Window.SetInterval(Update, TARGET_MS);
        }

        #endregion

        #region Event Handlers

        private static void Update()
        {
            if (!manager.EntityExists(generator.GetHeroId()))
            {
                gameOver = true;
            }

            if (!gameOver)
            {
                manager.Update(1f/FPS);
            }
        }
        #endregion
    }
}