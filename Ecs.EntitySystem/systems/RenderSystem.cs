using System;
using System.Drawing;
using System.Windows.Forms;
using Ecs.Core;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Ecs.EntitySystem
{
    public class RenderSystem : Core.System
    {
        private const int WIDTH = 800;
        private const int HEIGHT = 600;

        private TextRenderer textRenderer;

        public RenderSystem()
        {
            AddRequiredComponent<Shape>();
            AddRequiredComponent<Location>();

            textRenderer = new TextRenderer(WIDTH, HEIGHT);
            textRenderer.SetFont(new Font(FontFamily.GenericSansSerif, 13));
            textRenderer.SetBrush(Brushes.Red);
        }

        #region Render

        public void Render()
        {
            Vector3 cameraOffset = new Vector3(HeroViewport.X, HeroViewport.Y, 0);
            GL.Translate(-cameraOffset);
            foreach (Entity entity in Entities)
            {
                var entityBounds = CollisionUtil.GetBounds(entity);
                if (entityBounds.IntersectsWith(HeroViewport))
                {
                    Render(entity);
                }
            }
            GL.Translate(cameraOffset);

            RenderHud();
        }

        private void Render(Entity entity)
        {
            var position = entity.GetComponent<Location>().Position;
            var shape = entity.GetComponent<Shape>();
            float angle = EntityUtil.GetRotation(entity);

            GL.Translate(new Vector3(position));
            GL.Begin(PrimitiveType.Polygon);


            GL.Color3(shape.Color);
            for (int i = 0; i < shape.Sides; i++)
            {
                Vector2 vertex = shape.GetVertex(i, angle);
                if (shape.Sides == 3 )
                {
                    Color changed;
                    if (i == 1)
                    {
                        changed = ControlPaint.LightLight(shape.Color);
                    }
                    else
                    {
                        changed = ControlPaint.Dark(shape.Color);
                    }
                    GL.Color3(changed);
                    GL.Vertex2(vertex);
                }
                else
                {
                    GL.Vertex2(vertex);
                }
            }
            GL.End();

            if (entity.HasComponent<Health>())
            {
                RenderHealth(entity);
            }

            GL.Translate(new Vector3(-position));
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
            var stats = Hero.GetComponent<Stats>();

            int x = 300;
            const int y = 10 + HUD_Y;

            string display = $"ATT: {stats.Attack} ({stats.BaseAtt} + {stats.ModAtt})\n" +
                             $"DEF: {stats.Defense} ({stats.BaseDef} + {stats.ModDef})\n" +
                             $"VIT: {stats.Vitality} ({stats.BaseVit} + {stats.ModVit})";

            textRenderer.RenderText(display, new Vector2(x, y));

            x += 130;
            display = $"SPD: {stats.Speed} ({stats.BaseSpd} + {stats.ModSpd})\n" +
                      $"DEX: {stats.Dexterity} ({stats.BaseDex} + {stats.ModDex})\n" +
                      $"WIS: {stats.Wisdom} ({stats.BaseWis} + {stats.ModWis})";

            textRenderer.RenderText(display, new Vector2(x, y));
        }

        #endregion

        #region Misc

        public static void DrawRectangle(Rectangle rectangle, Color color)
        {
            GL.Begin(PrimitiveType.Polygon);
            GL.Color3(color);
            GL.Vertex2(rectangle.Left, rectangle.Top);
            GL.Vertex2(rectangle.Right, rectangle.Top);
            GL.Vertex2(rectangle.Right, rectangle.Bottom);
            GL.Vertex2(rectangle.Left, rectangle.Bottom);
            GL.End();
        }

        public static void OutlineRectangle(Rectangle rectangle, Color color)
        {
            GL.Begin(PrimitiveType.LineLoop);
            GL.Color3(color);
            GL.Vertex2(rectangle.Left, rectangle.Top);
            GL.Vertex2(rectangle.Right, rectangle.Top);
            GL.Vertex2(rectangle.Right, rectangle.Bottom);
            GL.Vertex2(rectangle.Left, rectangle.Bottom);
            GL.End();
        }

        private Rectangle HeroViewport
        {
            get { return Hero.GetComponent<Camera>().Viewport; }
        }

        public override void UpdateAll(float deltaTime){}

        protected override void Update(Entity entity, float deltaTime){}

        #endregion
    }
}