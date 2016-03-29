using System;
using Bridge.Html5;
using Bridge.Lib;
using Ecs.Core;

namespace Ecs.EntitySystem
{
    public class RenderSystem : Core.System
    {
        private const int WIDTH = 800;
        private const int HEIGHT = 600;

        private CanvasRenderingContext2D context;

        public RenderSystem()
        {
            AddRequiredComponent<Shape>();
            AddRequiredComponent<Location>();

            var canvas = Document.GetElementById<CanvasElement>("canvas");
            context = canvas.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);
        }

        public override void UpdateAll(float deltaTime)
        {
            context.ClearRect(0, 0, WIDTH, HEIGHT);
            context.FillStyle = "black";
            context.FillRect(0, 0, WIDTH, HEIGHT);

            DrawGrid();

            context.Translate(-HeroViewport.X, -HeroViewport.Y);
            base.UpdateAll(deltaTime);
            context.Translate(HeroViewport.X, HeroViewport.Y);

            RenderHud();
        }

        protected override void Update(Entity entity, float deltaTime)
        {
            Rectangle bounds = CollisionUtil.GetBounds(entity);
            if (!bounds.IntersectsWith(HeroViewport))
            {
                return;
            }

            var position = entity.GetComponent<Location>().Position;
            var shape = entity.GetComponent<Shape>();
            var angle = EntityUtil.GetRotation(entity);


            context.Translate(position.X, position.Y);
            context.FillStyle = shape.Color.ColorName;

            if (shape.IsCircle)
            {
                RenderCircle(shape);
            }
            else
            {
                RenderPolygon(shape, angle);
            }

            if (entity.HasComponent<Health>())
            {
                RenderHealth(entity);
            }

            context.Translate(-position.X, -position.Y);
        }

        #region Render

        private void DrawGrid()
        {
            var viewport = Hero.GetComponent<Camera>().Viewport;

            const int gridSize = 50;
            const int width = 800;
            const int height = 600;
            int xOff = gridSize - (viewport.X % gridSize);
            int yOff = gridSize - (viewport.Y % gridSize);

            context.StrokeStyle = "blue";
            for (int x = xOff; x < width; x += gridSize)
            {
                context.BeginPath();
                context.MoveTo(x, 0);
                context.LineTo(x, height);
                context.Stroke();
                context.ClosePath();
            }

            for (int y = yOff; y < height; y += gridSize)
            {
                context.BeginPath();
                context.MoveTo(0, y);
                context.LineTo(width, y);
                context.Stroke();
                context.ClosePath();
            }
        }

        private void RenderPolygon(Shape shape, float angle)
        {
            context.BeginPath();

            MoveTo(shape.GetVertex(0, angle));
            for (int i = 1; i < shape.Sides + 1; i++)
            {
                LineTo(shape.GetVertex(i, angle));
            }
            context.Fill();
            context.ClosePath();
        }

        private void RenderCircle(Shape shape)
        {
            context.BeginPath();

            context.Arc(0, 0, shape.Radius, 0, Math.PI*2);
            context.Fill();
            
            context.ClosePath();
        }

        private void RenderHealth(Entity entity)
        {
            var health = entity.GetComponent<Health>();
            float ratio = 1f*health.Hp/health.MaxHp;
            var shape = entity.GetComponent<Shape>();

            Rectangle bar = new Rectangle(
                -shape.Radius,
                -(shape.Radius + 5),
                shape.Radius * 2,
                5);

            DrawRectangle(bar, Color.Gray);

            bar.Width = (int) (bar.Width * ratio);
            DrawRectangle(bar, Color.Red);
        }

        #endregion

        #region HUD

        private const int HUD_HEIGHT = 80;
        private const int HUD_Y = HEIGHT - HUD_HEIGHT;

        private void RenderHud()
        {
            DrawRectangle(new Rectangle(0, HUD_Y, WIDTH, HUD_HEIGHT), Color.Gray);

            RenderHealth();
            RenderExperience();
            RenderStats();
        }

        private void RenderHealth()
        {
            const int width = 150;
            const int height = 20;
            const int x = 20;
            const int y = 10 + HUD_Y;

            var health = Hero.GetComponent<Health>();

            Rectangle healthBar = new Rectangle(x, y, width, height);
            float healthRatio = 1f * health.Hp / health.MaxHp;
            healthBar.Width = (int)(healthBar.Width * healthRatio);

            context.FillStyle = "red";
            DrawRectangle(healthBar, Color.Red);
            healthBar.Width = width;
            OutlineRectangle(healthBar, Color.Black);
        }

        private void RenderExperience()
        {
            const int width = 150;
            const int height = 20;
            const int x = 20;
            const int y = HEIGHT - (10 + height);

            var experience = Hero.GetComponent<Experience>();

            Rectangle xpBar = new Rectangle(x, y, width, height);
            float xpRatio = 1f*experience.Xp/experience.ToNextLevel;
            xpBar.Width = (int) (xpBar.Width*xpRatio);

            DrawRectangle(xpBar, Color.Green);
            xpBar.Width = width;
            OutlineRectangle(xpBar, Color.Black);
        }

        private void RenderStats()
        {
            //var stats = Hero.GetComponent<Stats>();

            //int x = 300;
            ////const int y = 10 + HUD_Y;

            //string display = $"ATT: {stats.Attack} ({stats.BaseAtt} + {stats.ModAtt})\n" +
            //                 $"DEF: {stats.Defense} ({stats.BaseDef} + {stats.ModDef})\n" +
            //                 $"VIT: {stats.Vitality} ({stats.BaseVit} + {stats.ModVit})";

            ////textRenderer.RenderText(display, new Vector2(x, y));

            //x += 130;
            //display = $"SPD: {stats.Speed} ({stats.BaseSpd} + {stats.ModSpd})\n" +
            //          $"DEX: {stats.Dexterity} ({stats.BaseDex} + {stats.ModDex})\n" +
            //          $"WIS: {stats.Wisdom} ({stats.BaseWis} + {stats.ModWis})";

            //textRenderer.RenderText(display, new Vector2(x, y));
        }

        #endregion

        #region Misc

        private Rectangle HeroViewport
        {
            get { return Hero.GetComponent<Camera>().Viewport; }
        }

        private void MoveTo(Vector2 vector)
        {
            context.MoveTo(vector.X, vector.Y);
        }

        private void LineTo(Vector2 vector)
        {
            context.LineTo(vector.X, vector.Y);
        }

        private void DrawRectangle(Rectangle rect, Color color)
        {
            context.FillStyle = color.ColorName;
            context.FillRect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        private void OutlineRectangle(Rectangle rect, Color color)
        {
            context.StrokeStyle = color.ColorName;
            context.StrokeRect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        #endregion
    }
}